using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Inf.Genericos.Helper;
using System.Globalization;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IArticulosService
    {

    }

    public class ArticulosService : GestionService<ArticulosModel, Articulos>, IArticulosService
    {
        #region CTR

        public ArticulosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public IEnumerable<ArticulosComboModel> GetArticles(string empresa)
        {
            using (var service = FService.Instance.GetService(typeof(ArticulosModel), _context))
            {
                return service.getAll().OfType<ArticulosModel>().Select(f => new ArticulosComboModel() { Id = f.Id, Descripcion = f.Descripcion }).OrderBy(f => f.Id);
            }
        }

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var unidadesService = new UnidadesService(_context, _db);
            var propiedadesVisibles = new[] { "Id", "Descripcion", "Fkgruposmateriales", "Fkunidades" };
            var propiedades = Helpers.Helper.getProperties<ArticulosModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.FiltroColumnas.Add("Id", FiltroColumnas.EmpiezaPor);
            model.ColumnasCombo.Add("Fkunidades", unidadesService.getAll().OfType<UnidadesModel>().Select(f => new Tuple<string, string>(f.Id, f.Codigounidad)));
            //model.ColumnasCombo.Add("Fkgruposmateriales", ListGruposmateriales());
            return model;
        }

        public override string GetSelectPrincipal()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("select a.id, a.descripcion,a.fkgruposmateriales,fp.fkunidadesmedida as [Fkunidades] from articulos as a ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.empresa=a.empresa and fp.id=substring(a.id,0,3) ");
            sb.AppendFormat(" where a.empresa='{0}' ", Empresa);

            return sb.ToString();
        }

        #endregion


        /*
        private IEnumerable<Tuple<string, string>> ListGruposmateriales()
        {
            return
                _appService.GetListGrupoMateriales()
                    .Select(f => new Tuple<string, string>(f.Valor, f.Descripcion));
        }
        */

        #region CRUD

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ArticulosModel;

                //Heredamos la clasificacion del articulo de la familia
                var numeroFamilia = model.Familia ?? "";
                var clasificacionFamilia = _db.Familiasproductos.Where(f => f.empresa == Empresa && f.id == numeroFamilia).Select(f => f.clasificacion).SingleOrDefault();

                if (!String.IsNullOrEmpty(clasificacionFamilia))
                {
                    model.Clasificacion = clasificacionFamilia;
                }

                else
                {
                    model.Clasificacion = "";
                }

                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.Articulos, _context, _db);

                foreach (var tercero in model.ArticulosTercero)
                {
                    tercero.CodArticulo = model.Id;

                    if (tercero.Descripcion == "")
                    {
                        tercero.Descripcion = model.Descripcion;
                    }
                }

                foreach (var componente in model.ArticulosComponentes)
                {
                    componente.FkArticulo = model.Id;
                }

                base.create(model);

                //gestionar tarifas
                ManagementRates(model, TipoFlujo.Venta);
                ManagementRates(model, TipoFlujo.Compra);
                ManagementSpecificRates(model, TipoFlujo.Venta);
                ManagementSpecificRates(model, TipoFlujo.Compra);
                ManagementAutomaticRates(model, TipoFlujo.Venta);
                ManagementAutomaticRates(model, TipoFlujo.Compra);
                tran.Complete();
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ArticulosModel;

                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.Articulos, _context, _db);

                foreach (var tercero in model.ArticulosTercero)
                {
                    tercero.Empresa = Empresa;
                }

                base.edit(model);

                //gestionar tarifas
                ManagementRates(model, TipoFlujo.Venta);
                ManagementRates(model, TipoFlujo.Compra);
                ManagementSpecificRates(model, TipoFlujo.Venta);
                ManagementSpecificRates(model, TipoFlujo.Compra);
                ManagementAutomaticRates(model, TipoFlujo.Venta);
                ManagementAutomaticRates(model, TipoFlujo.Compra);
                tran.Complete();
            }

        }

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            { //remove tarifas asociadas
                var model = obj as ArticulosModel;
                var list = _db.TarifasLin.Where(f => f.empresa == model.Empresa && f.fkarticulos == model.Id).ToList();
                _db.TarifasLin.RemoveRange(list);

                base.delete(obj);
                tran.Complete();
            }
        }

        private void ManagementRates(ArticulosModel model, TipoFlujo flujo)
        {
            var vector = flujo == TipoFlujo.Venta ? model.TarifasSistemaVenta : model.TarifasSistemaCompra;
            foreach (var item in vector)
            {
                var tarifa = _db.TarifasLin.SingleOrDefault(
                    f => f.empresa == model.Empresa && f.fktarifas == item.Id && f.fkarticulos == model.Id);

                if (tarifa == null)
                {
                    tarifa = _db.TarifasLin.Create();
                    tarifa.Unidades = model.Fkunidades;
                    tarifa.empresa = model.Empresa;
                    tarifa.fktarifas = item.Id;
                    tarifa.fkarticulos = model.Id;
                    tarifa.descuento = 0;
                    tarifa.precio = model.Articulocomentariovista ? 0 : item.Precio;
                    _db.TarifasLin.Add(tarifa);
                }
                else
                {
                    tarifa.Unidades = model.Fkunidades;
                    tarifa.precio = model.Articulocomentariovista ? 0 : item.Precio;
                }

            }

            _db.SaveChanges();
        }

        private void ManagementSpecificRates(ArticulosModel model, TipoFlujo flujo)
        {
            var vector = flujo == TipoFlujo.Venta ? model.TarifasEspecificasVentas.Lineas : model.TarifasEspecificasCompras.Lineas;

            foreach (var item in vector)
            {
                var tarifa = _db.TarifasLin.SingleOrDefault(
                    f => f.empresa == model.Empresa && f.fktarifas == item.Id && f.fkarticulos == model.Id);

                if (tarifa == null)
                {
                    tarifa = _db.TarifasLin.Create();
                    tarifa.empresa = model.Empresa;
                    tarifa.Unidades = model.Fkunidades;
                    tarifa.fktarifas = item.Id;
                    tarifa.fkarticulos = model.Id;
                    tarifa.descuento = item.Descuento;
                    tarifa.precio = model.Articulocomentariovista ? 0 : item.Precio;
                    _db.TarifasLin.Add(tarifa);
                }
                else
                {
                    tarifa.Unidades = model.Fkunidades;
                    tarifa.descuento = item.Descuento;
                    tarifa.precio = model.Articulocomentariovista ? 0 : item.Precio;
                }

            }

            _db.SaveChanges();
        }

        private void ManagementAutomaticRates(ArticulosModel model, TipoFlujo flujo)
        {
            var tarifas = _db.Tarifas.Where(f => f.empresa == model.Empresa && f.tipoflujo == (int)flujo && f.tipotarifa == (int)TipoTarifa.Especifica && f.asignartarifaalcreararticulos == true).ToList();
            var vector = flujo == TipoFlujo.Venta ? model.TarifasEspecificasVentas.Lineas : model.TarifasEspecificasCompras.Lineas;



            foreach (var item in tarifas)
            {
                var tarifa = _db.TarifasLin.SingleOrDefault(
                    f => f.empresa == model.Empresa && f.fktarifas == item.id && f.fkarticulos == model.Id);

                if (tarifa == null)
                {
                    tarifa = _db.TarifasLin.Create();
                }

                if (vector.All(f => f.Id != tarifa.fktarifas))
                {
                    tarifa.empresa = model.Empresa;
                    tarifa.fktarifas = item.id;
                    tarifa.Unidades = model.Fkunidades;
                    tarifa.fkarticulos = model.Id;
                    tarifa.descuento = 0;
                    tarifa.precio = model.Articulocomentariovista ? 0 : CalcularPrecio(item, model);



                    _db.TarifasLin.AddOrUpdate(tarifa);
                }


            }

            _db.SaveChanges();
        }


        private double CalcularPrecio(Tarifas tarifa, ArticulosModel articulo)
        {
            var result = 0.0;
            var tarifalinprecio =
                _db.TarifasLin.SingleOrDefault(
                    f => f.empresa == articulo.Empresa && f.fkarticulos == articulo.Id && f.fktarifas == tarifa.precioautomaticobase);
            var preciobase = tarifalinprecio?.precio ?? articulo.Coste;

            var d = preciobase + (preciobase * tarifa.precioautomaticoporcentajebase / 100.0) +
                     tarifa.precioautomaticoporcentajefijo;
            if (d != null)
                result = (double)d;

            return result;
        }

        #endregion

        public List<TarifasEspecificasArticulosViewModel> GetTarifas(TipoFlujo flujo, string articulo)
        {

            var serviceTarifas = FService.Instance.GetService(typeof(TarifasModel), _context, _db) as TarifasService;

            return serviceTarifas.GetTarifasEspecificas(flujo, articulo, Empresa).ToList();


        }

        public ArticulosModel GetArticulo(string id)
        {

            return _converterModel.GetModelView(_db.Articulos.Single(f => f.empresa == Empresa && f.id == id)) as ArticulosModel;
        }

        public ArticulosDocumentosModel GetArticulo(string id, string fkcuenta, string fkmonedas, string fkregimeniva, TipoFlujo flujo = TipoFlujo.Venta)
        {
            //existe precio especial
            var PrecioEspecial = _db.Database.SqlQuery<int>(GetCadenaSelectPrecioEspecial(id, fkcuenta)).SingleOrDefault();

            return _db.Database.SqlQuery<ArticulosDocumentosModel>(GetCadenaSelectArticulo(id, fkcuenta, fkmonedas,
                fkregimeniva, flujo, PrecioEspecial)).SingleOrDefault();
        }

        private string GetCadenaSelectPrecioEspecial(string id, string fkcuenta)
        {
            var sb = new StringBuilder();

            sb.Append("Select id from PreciosEspeciales where empresa = '" + Empresa + "' and fkclientes = '" + fkcuenta + "' and fkarticulo = '" + id + "' and fechavalidez >= '" + DateTime.Today.ToString("yyyy-MM-dd") + " 00:00:00.000'");

            return sb.ToString();

        }

        private string GetCadenaSelectArticulo(string id, string fkcuenta, string fkmonedas, string fkregimeniva, TipoFlujo flujo, int precioEspecial)
        {

            var hayEspecial = precioEspecial != 0 ? true : false;
            string idioma = "";

            if (!string.IsNullOrEmpty(fkcuenta))
            {
                //Rai -- Necesito saber si la cuenta que estoy recibiendo es Cliente, Proveedor o Acreedor
                if (fkcuenta.StartsWith("43"))
                {
                    var serviceClientes = FService.Instance.GetService(typeof(ClientesModel), _context, _db) as ClientesService;
                    var cliente = serviceClientes.get(fkcuenta) as ClientesModel;
                    idioma = cliente.Fkidiomas;
                }

                else
                {
                    if (fkcuenta.StartsWith("40"))
                    {
                        var serviceProveedores = FService.Instance.GetService(typeof(ProveedoresModel), _context, _db) as ProveedoresService;
                        var proveedor = serviceProveedores.get(fkcuenta) as ProveedoresModel;
                        idioma = proveedor.Fkidiomas;
                    }

                    else
                    {
                        var serviceAcreedores = FService.Instance.GetService(typeof(AcreedoresModel), _context, _db) as AcreedoresService;
                        var acreedor = serviceAcreedores.get(fkcuenta) as AcreedoresModel;
                        idioma = acreedor.Fkidiomas;
                    }
                }
            }

            var sb = new StringBuilder();
            if (flujo == TipoFlujo.Venta)
            {

                sb.Append("select a.categoria as Categoria, case when u.tipototal=1 then convert(bit,1) else convert(bit,0) end as Permitemodificarmetros,a.Piezascaja as Piezascaja, " +
                        " u.id as Fkunidades,u.decimalestotales as [Decimalestotales],u.formula as [Formulas] ,a.id as [Id], ");

                if (idioma.Equals("ENG"))
                {
                    sb.Append("a.descripcion2 as [Descripcion]");
                }

                else
                {
                    sb.Append("a.descripcion as [Descripcion]");
                }

                sb.Append(" , isnull(a.largo, 0) as [Largo], isnull(a.ancho, 0) as [Ancho], isnull(a.grueso, 0) as [Grueso], isnull(a.editarlargo, 0) as [Permitemodificarlargo], isnull(a.editarancho, 0) as [Permitemodificarancho], " +
                      " isnull(a.editargrueso,0) as [Permitemodificargrueso],mo.id as Fkmonedas,fp.tipofamilia as [Tipofamilia],a.tipogestionlotes as [Tipogestionlotes],isnull(a.lotefraccionable,0) as [Lotefraccionable],a.tipoivavariable as [Tipoivavariable], ");

                sb.Append(" mo.decimales as Decimalesmonedas, mo.cambiomonedabase as Cambiomonedabase,mo.cambiomonedaadicional as Cambiomonedaadicional,");
                sb.Append(" ti.id as Fktiposiva, ti.nombre as Descripcioniva, ti.Porcentajeiva as Porcentajeiva,ti.porcentajerecargoequivalente as PorcentajeRecargoEquivalencia,");
                sb.Append(" isnull(tl.precio, 0.0) as Precio, isnull(tl.descuento, 0.0) as Descuento,isnull(a.articulocomentario,0) as Articulocomentario,fp.fkcontador  as Fkcontador ");
                sb.Append(" from articulos as a");
                sb.Append(" inner join Familiasproductos as fp on fp.empresa = a.empresa and fp.id = substring(a.id, 0, 3)");
                sb.Append(" left join unidades as u on u.id = fp.fkunidadesmedida");
                sb.AppendFormat(" left join clientes as cli on cli.empresa = a.empresa and cli.fkcuentas = '{0}'", fkcuenta);
                sb.AppendFormat(" left join prospectos as pro on pro.empresa = a.empresa and pro.fkcuentas = '{0}'", fkcuenta);
                sb.Append(" left join monedas as mo on mo.id = isnull(cli.fkmonedas, pro.fkmonedas)");
                sb.AppendFormat(" left join RegimenIva as ri  on ri.empresa = a.empresa and ri.id = isnull({0}, isnull(cli.fkregimeniva, pro.fkregimeniva))", string.IsNullOrEmpty(fkregimeniva) ? "NULL" : "'" + fkregimeniva + "'");

                //sb.Append("select a.categoria as Categoria, case when u.tipototal=1 then convert(bit,1) else convert(bit,0) end as Permitemodificarmetros,a.Piezascaja as Piezascaja, " +
                //        " u.id as Fkunidades,u.decimalestotales as [Decimalestotales],u.formula as [Formulas] ,a.id as [Id], a.descripcion as [Descripcion],a.descripcion2 as [Descripcion2],isnull(a.largo,0) as [Largo],isnull(a.ancho,0) as [Ancho],isnull(a.grueso,0) as [Grueso],isnull(a.editarlargo,0) as [Permitemodificarlargo],isnull(a.editarancho,0) as [Permitemodificarancho], " +
                //      " isnull(a.editargrueso,0) as [Permitemodificargrueso],mo.id as Fkmonedas,fp.tipofamilia as [Tipofamilia],a.tipogestionlotes as [Tipogestionlotes],isnull(a.lotefraccionable,0) as [Lotefraccionable],a.tipoivavariable as [Tipoivavariable], ");
                //sb.Append(" mo.decimales as Decimalesmonedas, mo.cambiomonedabase as Cambiomonedabase,mo.cambiomonedaadicional as Cambiomonedaadicional,");
                //sb.Append(" ti.id as Fktiposiva, ti.nombre as Descripcioniva, ti.Porcentajeiva as Porcentajeiva,ti.porcentajerecargoequivalente as PorcentajeRecargoEquivalencia,");
                //sb.Append(" isnull(tl.precio, 0.0) as Precio, isnull(tl.descuento, 0.0) as Descuento,isnull(a.articulocomentario,0) as Articulocomentario,fp.fkcontador  as Fkcontador ");
                //sb.Append(" from articulos as a");
                //sb.Append(" inner join Familiasproductos as fp on fp.empresa = a.empresa and fp.id = substring(a.id, 0, 3)");
                //sb.Append(" left join unidades as u on u.id = fp.fkunidadesmedida");
                //sb.AppendFormat(" left join clientes as cli on cli.empresa = a.empresa and cli.fkcuentas = '{0}'", fkcuenta);
                //sb.AppendFormat(" left join prospectos as pro on pro.empresa = a.empresa and pro.fkcuentas = '{0}'", fkcuenta);
                //sb.Append(" left join monedas as mo on mo.id = isnull(cli.fkmonedas, pro.fkmonedas)");
                //sb.AppendFormat(" left join RegimenIva as ri  on ri.empresa = a.empresa and ri.id = isnull({0}, isnull(cli.fkregimeniva, pro.fkregimeniva))", string.IsNullOrEmpty(fkregimeniva) ? "NULL" : "'" + fkregimeniva + "'");

                //Precios Especiales
                if (hayEspecial)
                {
                    sb.AppendFormat(" left join PreciosEspeciales as tl on tl.empresa = a.empresa and tl.fkclientes = '{0}' and tl.fkarticulo = a.id", fkcuenta);
                }
                else
                {
                    sb.Append(" left join TarifasLin as tl on tl.empresa = a.empresa and tl.fktarifas = isnull(cli.fktarifas, pro.fktarifas) and tl.fkarticulos = a.id");
                }

                sb.Append(" left join TiposIva as ti on ti.empresa = a.empresa and");
                sb.Append(" ti.id = (select top 1 case concat(isnull(ri.normal, 0), isnull(ri.recargo, 0), isnull(ri.exentotasa, 0))");
                sb.Append(" when '100'  then gi.fktiposivasinrecargo");
                sb.Append(" when '010'  then gi.fktiposivaconrecargo");
                sb.Append(" when '001'  then gi.fktiposivaexentoiva");
                sb.Append(" end");
                sb.Append(" from GruposIvaLin as gi where gi.empresa = a.empresa and gi.fkgruposiva = a.fkgruposiva and gi.desde <= getdate() order by gi.desde desc)");
                sb.AppendFormat(" where a.empresa = '{0}' and a.id='{1}' and (a.categoria=0 or a.categoria=1)", Empresa, id);
            }
            else
            {
                sb.Append("select a.categoria as Categoria,case when u.tipototal=1 then convert(bit,1) else convert(bit,0) end as Permitemodificarmetros,a.Piezascaja as Piezascaja, " +
                       " u.id as Fkunidades,u.decimalestotales as [Decimalestotales],u.formula as [Formulas] ,a.id as [Id], a.descripcion as [Descripcion],a.descripcion2 as [Descripcion2],isnull(a.largo,0) as [Largo],isnull(a.ancho,0) as [Ancho],isnull(a.grueso,0) as [Grueso],isnull(a.editarlargo,0) as [Permitemodificarlargo],isnull(a.editarancho,0) as [Permitemodificarancho], " +
                     " isnull(a.editargrueso,0) as [Permitemodificargrueso],mo.id as Fkmonedas, fp.tipofamilia as [Tipofamilia],a.tipogestionlotes as [Tipogestionlotes],isnull(a.lotefraccionable,0) as [Lotefraccionable],a.tipoivavariable as [Tipoivavariable], ");
                sb.Append(" mo.decimales as Decimalesmonedas, mo.cambiomonedabase as Cambiomonedabase,mo.cambiomonedaadicional as Cambiomonedaadicional,");
                sb.Append(" ti.id as Fktiposiva, ti.nombre as Descripcioniva, ti.Porcentajeiva as Porcentajeiva,ti.porcentajerecargoequivalente as PorcentajeRecargoEquivalencia,");
                sb.Append(" isnull(tl.precio, 0.0) as Precio, isnull(a.articulocomentario,0) as Articulocomentario,fp.fkcontador  as Fkcontador ");
                sb.Append(" from articulos as a");
                sb.Append(" inner join Familiasproductos as fp on fp.empresa = a.empresa and fp.id = substring(a.id, 0, 3)");
                sb.Append(" left join unidades as u on u.id = fp.fkunidadesmedida");
                sb.AppendFormat(" left join proveedores as cli on cli.empresa = a.empresa and cli.fkcuentas = '{0}'", fkcuenta);
                sb.Append(" left join monedas as mo on mo.id = cli.fkmonedas ");
                sb.AppendFormat(" left join RegimenIva as ri  on ri.empresa = a.empresa and ri.id = isnull({0}, cli.fkregimeniva)", string.IsNullOrEmpty(fkregimeniva) ? "NULL" : "'" + fkregimeniva + "'");
                sb.Append(" left join TarifasLin as tl on tl.empresa = a.empresa and tl.fktarifas = cli.tarifa and tl.fkarticulos = a.id");
                sb.Append(" left join TiposIva as ti on ti.empresa = a.empresa and");
                sb.Append(" ti.id = (select top 1 case concat(isnull(ri.normal, 0), isnull(ri.recargo, 0), isnull(ri.exentotasa, 0))");
                sb.Append(" when '100'  then gi.fktiposivasinrecargo");
                sb.Append(" when '010'  then gi.fktiposivaconrecargo");
                sb.Append(" when '001'  then gi.fktiposivaexentoiva");
                sb.Append(" end");
                sb.Append(" from GruposIvaLin as gi where gi.empresa = a.empresa and gi.fkgruposiva = a.fkgruposiva and gi.desde <= getdate() order by gi.desde desc)");
                sb.AppendFormat(" where a.empresa = '{0}' and a.id='{1}' and (a.categoria=0 or a.categoria=2)", Empresa, id);
            }



            return sb.ToString();
        }

        public double GetPrecioVenta(string id, IProspectoCliente fkcuenta, string fkmonedas = "", string fkregimeniva = "")
        {
            return GetPrecio(id, TipoFlujo.Venta, fkcuenta, fkmonedas);
        }

        public double GetPrecioCompra(string id, IProspectoCliente fkcuenta, string fkmonedas = "", string fkregimeniva = "")
        {
            return GetPrecio(id, TipoFlujo.Compra, fkcuenta, fkmonedas);
        }

        private double GetPrecio(string fkarticulo, TipoFlujo tipoflujo, IProspectoCliente fkcuenta, string fkmonedas = "", string fkregimeniva = "")
        {
            double result = 0;

            var codTarifa = fkcuenta.Fktarifas;
            var tarifa = _db.Tarifas.Include("TarifasLin").SingleOrDefault(f => f.empresa == Empresa && f.id == codTarifa && f.tipoflujo == (int)tipoflujo);
            if (tarifa != null)
            {
                if (string.IsNullOrEmpty(fkmonedas) || (!string.IsNullOrEmpty(fkmonedas) && tarifa.fkmonedas == Funciones.Qint(fkmonedas)))
                {
                    result = tarifa.TarifasLin.SingleOrDefault(f => f.empresa == Empresa && f.fktarifas == codTarifa && f.fkarticulos == fkarticulo)?.precio ?? 0.0;
                }
            }

            return result;
        }

        #region CodificacionArticulo
        public static string GetCodigoFamilia(string codigoarticulo)
        {
            return codigoarticulo.Substring(0, 2);
        }

        public static string GetCodigoMaterial(string codigoarticulo)
        {
            return codigoarticulo.Substring(2, 3);
        }

        public static string GetCodigoCaracteristica(string codigoarticulo)
        {
            return codigoarticulo.Substring(5, 2);
        }

        public static string GetCodigoGrosor(string codigoarticulo)
        {
            return codigoarticulo.Substring(7, 2);
        }
        public static string GetCodigoAcabado(string codigoarticulo)
        {
            return codigoarticulo.Substring(9, 2);
        }
        #endregion

        public IEnumerable<ArticulosModel> GetArticulosBusquedas(TipoCategoria flujo = TipoCategoria.Ambas)
        {
            using (var con = new SqlConnection(_db.Database.Connection.ConnectionString))
            {
                using (var cmd = new SqlCommand(CadenaBusquedaArticulos(flujo), con))
                {
                    cmd.Parameters.Add(new SqlParameter("@empresa", Empresa));

                    using (var ad = new SqlDataAdapter(cmd))
                    {
                        var tabla = new DataTable();
                        ad.Fill(tabla);
                        foreach (DataRow row in tabla.Rows)
                        {
                            yield return new ArticulosModel()
                            {
                                Id = Funciones.Qnull(row["id"]),
                                Familia = Funciones.Qnull(row["Fkfamilias"]),
                                FamiliaDescripcion = Funciones.Qnull(row["Familias"]),
                                Materiales = Funciones.Qnull(row["Fkmateriales"]),
                                MaterialesDescripcion = Funciones.Qnull(row["Materiales"]),
                                Caracteristicas = Funciones.Qnull(row["Fkcaracteristicas"]),
                                CaracteristicasDescripcion = Funciones.Qnull(row["Caracteristicas"]),
                                Grosores = Funciones.Qnull(row["Fkgrosores"]),
                                GrosoresDescripcion = Funciones.Qnull(row["Grosores"]),
                                Acabados = Funciones.Qnull(row["Fkacabados"]),
                                AcabadosDescripcion = Funciones.Qnull(row["Acabados"]),
                                Descripcion = Funciones.Qnull(row["Descripcion"])
                            };
                        }

                    }
                }

            }
        }

        private string CadenaBusquedaArticulos(TipoCategoria flujo)
        {
            var sb = new StringBuilder();
            sb.Append("select  art.id,art.descripcionabreviada as Descripcion,SUBSTRING(art.id,0,3) as Fkfamilias, fp.descripcion as Familias ");
            sb.Append(" ,SUBSTRING(art.id,3,3) as Fkmateriales, mp.descripcion as Materiales ");
            sb.Append(" ,SUBSTRING(art.id,6,2) as Fkcaracteristicas, cp.descripcion as Caracteristicas ");
            sb.Append(" ,SUBSTRING(art.id,8,2) as Fkgrosores, gp.descripcion as Grosores ");
            sb.Append(" ,SUBSTRING(art.id,10,2) as Fkacabados, ap.descripcion as Acabados ");
            sb.Append(" from articulos as art ");
            sb.Append(" left join Familiasproductos as fp on fp.empresa=art.empresa and fp.id=SUBSTRING(art.id,0,3) ");
            sb.Append(" left join Materiales as mp on mp.empresa=art.empresa and mp.id=SUBSTRING(art.id,3,3) ");
            sb.Append(" left join Caracteristicaslin as cp on cp.empresa=art.empresa and cp.fkcaracteristicas=SUBSTRING(art.id,0,3) AND cp.id=SUBSTRING(art.id,6,2) ");
            sb.Append(" left join Grosores as gp on gp.empresa = art.empresa and gp.id=SUBSTRING(art.id,8,2) ");
            sb.Append(" left join Acabados as ap on ap.empresa = art.empresa and ap.id=SUBSTRING(art.id,10,2) ");
            sb.AppendFormat(" Where art.empresa=@empresa and (art.categoria=0 or art.categoria={0})", (int)flujo);

            var a = sb.ToString();

            return a;
        }

        public static string GetCodigoLibre(string id, bool validarmaterial, bool validarcaracteristica)
        {
            var inicio = 2;
            inicio += validarmaterial ? 3 : 0;
            inicio += validarcaracteristica ? 2 : 0;
            return id.Substring(inicio);
        }

        public IEnumerable<ArticulosBusqueda> GetArticulosBusquedasMobile(IArticulosFiltros filtros, out int totales)
        {
            var cadenabusquedas = CadenaBusquedaArticulos(TipoCategoria.Ambas);

            cadenabusquedas += GenerarFiltrosBusquedasMobile(filtros.Filtros);
            cadenabusquedas += string.Format(" order by art.id offset {0}*({1}-1) rows fetch next {0} rows only option (recompile)", filtros.RegistrosPagina, filtros.Pagina);

            totales = _db.Database.SqlQuery<int>("select count(*) from articulos as art where art.empresa=@empresa " + GenerarFiltrosBusquedasMobile(filtros.Filtros), new SqlParameter("filtros", filtros.Filtros ?? string.Empty), new SqlParameter("empresa", Empresa)).Single();
            return _db.Database.SqlQuery<ArticulosBusqueda>(cadenabusquedas, new SqlParameter("filtros", filtros.Filtros ?? string.Empty), new SqlParameter("empresa", Empresa)).ToList();
        }

        private string GenerarFiltrosBusquedasMobile(string filtros)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(filtros))
            {
                sb.AppendFormat(" AND (art.id like @filtros+'%' OR art.descripcion like '%'+@filtros +'%') ");
            }

            return sb.ToString();
        }

        #region Importar

        public void Importar(DataTable dt, int idPeticion, IContextService context, ImportarModel model)
        {
            string errores = "";
            List<ArticulosModel> ListaArticulos = new List<ArticulosModel>();
            List<TarifasModel> ListaTarifas = new List<TarifasModel>();
            using (var service = new TarifasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                TarifasModel tarifa1 = service.GetTarifaCompleta(model.Tarifa1);
                ListaTarifas.Add(tarifa1);
                TarifasModel tarifa2 = service.GetTarifaCompleta(model.Tarifa2);
                ListaTarifas.Add(tarifa2);
                TarifasModel tarifa3 = service.GetTarifaCompleta(model.Tarifa3);
                ListaTarifas.Add(tarifa3);
            }

            foreach (DataRow row in dt.Rows)
            {
                ArticulosModel articulo = new FModel().GetModel<ArticulosModel>(context);

                var codArticulo = row["CodArticulo"].ToString();

                if (codArticulo.Length == 10)
                {
                    codArticulo = codArticulo.Insert(5, "0");
                }

                if (codArticulo.Length != 11)
                {
                    errores += codArticulo + "El código del artículo no tiene la longitud correcta" + Environment.NewLine;
                    continue;
                }

                var existeArticulo = _db.Articulos.Where(f => f.empresa == Empresa && f.id == codArticulo).SingleOrDefault();

                if (existeArticulo == null)
                {

                    articulo.Id = codArticulo;

                    var codFamilia = codArticulo.Substring(0, 2);
                    var codMateriales = codArticulo.Substring(2, 3);
                    var codCaracteristicas = codArticulo.Substring(5, 2);
                    var codGrosores = codArticulo.Substring(7, 2);
                    var codAcabados = codArticulo.Substring(9, 2);

                    var familia = _db.Familiasproductos.Where(f => f.empresa == Empresa && f.id == codFamilia).SingleOrDefault();
                    if (familia == null)
                    {
                        errores += codArticulo + " No existe la familia" + Environment.NewLine;
                        continue;
                    }

                    var materiales = _db.Materiales.Where(f => f.empresa == Empresa && f.id == codMateriales).SingleOrDefault();
                    if (materiales == null)
                    {
                        errores += codArticulo + " No existe el material" + Environment.NewLine;
                        continue;
                    }

                    var caracteristicas = _db.CaracteristicasLin.Where(f => f.empresa == Empresa && f.fkcaracteristicas == codFamilia && f.id == codCaracteristicas).SingleOrDefault();
                    if (caracteristicas == null)
                    {
                        errores += codArticulo + " No existe la caracteristicas" + Environment.NewLine;
                        continue;
                    }

                    var grosores = _db.Grosores.Where(f => f.empresa == Empresa && f.id == codGrosores).SingleOrDefault();
                    if (grosores == null)
                    {
                        errores += codArticulo + " No existe el grosor" + Environment.NewLine;
                        continue;
                    }

                    var acabados = _db.Acabados.Where(f => f.empresa == Empresa && f.id == codAcabados).SingleOrDefault();
                    if (acabados == null)
                    {
                        errores += codArticulo + " No existe el acabado" + Environment.NewLine;
                        continue;
                    }

                    // TODO: Generar el código del artículo por lo que ponga en la familia
                    //if (string.IsNullOrEmpty((row["Descripcion"]).ToString()))
                    //    articulo.Descripcion = familia.descripcion + " " + materiales.descripcion + " " + caracteristicas.descripcion + " " + grosores.descripcion + " " + acabados.descripcion;
                    //if (!string.IsNullOrEmpty((row["Descripcion2"]).ToString()))
                    //    articulo.Descripcion2 = familia.descripcion2 + " " + materiales.descripcion2 + " " + caracteristicas.descripcion2 + " " + grosores.descripcion2 + " " + acabados.descripcion2;
                    //if (!string.IsNullOrEmpty((row["DescripcionAbreviada"]).ToString()))
                    //    articulo.Descripcionabreviada = familia.descripcionabreviada + " " + materiales.descripcionabreviada + " " + caracteristicas.descripcionabreviada + " " + grosores.descripcionabreviada + " " + acabados.descripcionabreviada;

                    articulo.Descripcion = row["Descripcion"].ToString();
                    articulo.Descripcion2 = row["Descripcion2"].ToString();
                    articulo.Descripcionabreviada = row["Descripcion"].ToString();
                    articulo.Partidaarancelaria = row["PartidaArancelaria"].ToString();
                    articulo.Ean13 = row["Ean13"].ToString();

                    articulo.Categoria = (TipoCategoria)familia.categoria;
                    articulo.Gestionarcaducidad = (bool)familia.gestionarcaducidad;
                    articulo.Fkguiascontables = familia.fkguiascontables;
                    articulo.Fkgruposiva = familia.fkgruposiva;
                    articulo.Tipofamilia = (int)(familia.tipofamilia ?? 0);
                    articulo.Consumibles = (bool)familia.consumibles;

                    articulo.Fkunidades = familia.fkunidadesmedida;
                    articulo.Editarlargo = (bool)familia.editarlargo;
                    articulo.Editarancho = (bool)familia.editarancho;
                    articulo.Editargrueso = (bool)familia.editargrueso;

                    articulo.Gestionstock = (bool)familia.gestionstock;
                    articulo.Tipogestionlotes = (Tipogestionlotes)familia.tipogestionlotes;
                    articulo.Stocknegativoautorizado = (bool)familia.stocknegativoautorizado;
                    articulo.Lotefraccionable = familia.lotefraccionable;
                    articulo.Fkcontadores = familia.fkcontador;
                    /*articulo.Existenciasminimasmetros = familia.existenciasminimasmetros;
                    articulo.Existenciasmaximasmetros = familia.existenciasmaximasmetros;
                    articulo.Existenciasminimasunidades = familia.existenciasminimasunidades;
                    articulo.Existenciasmaximasunidades = familia.existenciasmaximasunidades;*/

                    articulo.Validarmateriales = (bool)familia.validarmaterial;
                    articulo.Validarcaracteristicas = (bool)familia.validarcaracteristica;
                    articulo.Validargrosores = (bool)familia.validargrosor;
                    articulo.Validaracabados = (bool)familia.validaracabado;

                    articulo.Largo = double.Parse(row["Largo"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Ancho = double.Parse(row["Ancho"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Grueso = double.Parse(row["Grueso"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Kilosud = double.Parse(row["KilosUd"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    /*articulo.Existenciasminimasmetros = double.Parse(row["ExistenciasMinimasMetros"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Existenciasmaximasmetros = double.Parse(row["ExistenciasMaximasMetros"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));*/

                    if (row["ExistenciasMinimasUnidades"].ToString() != "" || row["ExistenciasMaximasUnidades"].ToString() != "")
                    {
                        articulo.Stockseguridad = TipoStockSeguridad.Piezas;
                        articulo.Stockminimo = double.Parse(row["ExistenciasMinimasUnidades"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                        articulo.Stockmaximo = double.Parse(row["ExistenciasMaximasUnidades"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    }
                    else if (row["ExistenciasMinimasMetros"].ToString() != "" || row["ExistenciasMaximasMetros"].ToString() != "")
                    {
                        articulo.Stockseguridad = TipoStockSeguridad.Metros;
                        articulo.Stockminimo = double.Parse(row["ExistenciasMinimasMetros"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                        articulo.Stockmaximo = double.Parse(row["ExistenciasMaximasMetros"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    }
                    else
                    {
                        articulo.Stockseguridad = TipoStockSeguridad.Piezas;
                        articulo.Stockminimo = 0;
                        articulo.Stockmaximo = 0;
                    }
                    
                    articulo.Costemateriaprima = double.Parse(row["CosteMateriaPrima"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Porcentajemerma = double.Parse(row["PorcentajeMerma"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Costeelaboracionmateriaprima = double.Parse(row["CosteElaboracionMateriaPrima"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Costefabricacion = double.Parse(row["CosteFabricacion"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Otroscostes = double.Parse(row["OtrosCostes"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Costeportes = double.Parse(row["CostesPortes"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Costeindirecto = double.Parse(row["CosteIndirecto"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Coste = double.Parse(row["Coste"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Rendimientom2m3 = double.Parse(row["Rendimiento"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Piezascaja = int.Parse(row["PiezasCaja"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Preciominimoventa = double.Parse(row["PrecioMinimoVenta"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.Tiempostandardfabricacion = double.Parse(row["TiempoFab"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));

                    if (row["MedidaLibre"].ToString().ToLower() == "verdadero" || row["MedidaLibre"].ToString().ToLower() == "v")
                        articulo.Medidalibre = true;
                    else
                        articulo.Medidalibre = false;

                    if (row["ExcluirComisiones"].ToString().ToLower() == "verdadero" || row["ExcluirComisiones"].ToString().ToLower() == "v")
                        articulo.Excluircomisiones = true;
                    else
                        articulo.Excluircomisiones = false;

                    if (row["ExentoRetencion"].ToString().ToLower() == "verdadero" || row["ExentoRetencion"].ToString().ToLower() == "v")
                        articulo.Exentoretencion = true;
                    else
                        articulo.Exentoretencion = false;

                    if (row["Web"].ToString().ToLower() == "verdadero" || row["Web"].ToString().ToLower() == "v")
                        articulo.Web = true;
                    else
                        articulo.Web = false;

                    if (row["Labor"].ToString().ToLower() == "verdadero" || row["Labor"].ToString().ToLower() == "v")
                        articulo.Labor = true;
                    else
                        articulo.Labor = false;

                    /*articulo.TarifasSistemaVenta[0].Precio = double.Parse(row["PrecioVenta"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                    articulo.TarifasSistemaCompra[0].Precio = double.Parse(row["PrecioCompra"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));*/

                    ListaArticulos.Add(articulo);

                    //Asignamos las tarifas. Si no tiene, la creamos a 0
                    if (model.Tarifa1 != null)
                    {
                        TarifasLinModel tarifa1Lin = new TarifasLinModel();
                        tarifa1Lin.Fkarticulos = codArticulo;
                        tarifa1Lin.Precio = double.Parse(row["PVenta1"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                        tarifa1Lin.Descuento = 0;
                        tarifa1Lin.Unidades = articulo.Fkunidades;
                        //tarifa1.Lineas.Add(tarifa1Lin);

                        ListaTarifas[0].Lineas.Add(tarifa1Lin);
                    }
                    if (model.Tarifa2 != null)
                    {
                        TarifasLinModel tarifa2Lin = new TarifasLinModel();
                        tarifa2Lin.Fkarticulos = codArticulo;
                        tarifa2Lin.Precio = double.Parse(row["PVenta2"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                        tarifa2Lin.Descuento = 0;
                        tarifa2Lin.Unidades = articulo.Fkunidades;
                        //tarifa1.Lineas.Add(tarifa2Lin);

                        ListaTarifas[1].Lineas.Add(tarifa2Lin);
                    }
                    if (model.Tarifa3 != null)
                    { 
                        TarifasLinModel tarifa3Lin = new TarifasLinModel();
                        tarifa3Lin.Fkarticulos = codArticulo;
                        tarifa3Lin.Precio = double.Parse(row["PVenta3"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                        tarifa3Lin.Descuento = 0;
                        tarifa3Lin.Unidades = articulo.Fkunidades;
                        //tarifa1.Lineas.Add(tarifa3Lin);

                        ListaTarifas[2].Lineas.Add(tarifa3Lin);
                    }
                }
                else
                {
                    errores += codArticulo + " El código del artículo ya existe" + Environment.NewLine;
                }
            }


            //for (var i = 0; i < ListaArticulos.Count; i++)
            //{
            //    try
            //    {
            //        create(ListaArticulos[i]);
            //    }
            //    catch (Exception ex)
            //    {
            //        errores += ListaArticulos[i].Id + ";" + ListaArticulos[i].Descripcion + ";" + ex.Message + Environment.NewLine;
            //    }
            //}

            //Creamos los articulos
            foreach (var art in ListaArticulos)
            {
                try
                {
                    create(art);
                }
                catch (Exception ex)
                {
                    errores += art.Id + ";" + art.Descripcion + ";" + ex.Message + Environment.NewLine;
                }
            }

            //Creamos las líneas de las tarifas
            using (var service = new TarifasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
            {
                foreach (var tarifa in ListaTarifas)
                {
                    try
                    {
                        service.edit(tarifa);
                    }
                    catch (Exception ex)
                    {
                        errores += tarifa.Id + ";" + tarifa.Descripcion + ";" + ex.Message + Environment.NewLine;
                    }
                }
            }

            var item = _db.PeticionesAsincronas.Where(f => f.empresa == context.Empresa && f.id == idPeticion).SingleOrDefault();

            if (errores == "")
            {
                item.estado = (int)EstadoPeticion.Finalizada;
                item.resultado = errores;
            }
            else
            {
                item.estado = (int)EstadoPeticion.FinalizadaLogs;
                item.resultado = errores;
            }

            _db.PeticionesAsincronas.AddOrUpdate(item);

            try
            {
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int CrearPeticionImportacion(IContextService context)
        {
            var item = _db.PeticionesAsincronas.Create();

            item.empresa = context.Empresa;
            item.id = _db.PeticionesAsincronas.Any() ? _db.PeticionesAsincronas.Max(f => f.id) + 1 : 1;
            item.usuario = context.Usuario;
            item.fecha = DateTime.Today;
            item.estado = (int)EstadoPeticion.EnCurso;
            item.tipo = (int)TipoPeticion.Importacion;
            item.configuracion = (((int)TipoImportacion.ImportarArticulos).ToString() + "-").ToString();

            _db.PeticionesAsincronas.AddOrUpdate(item);
            _db.SaveChanges();

            return item.id;
        }


        public string descripcionCuenta(string numeroCuenta)
        {
            string descripcion = "";
            var descripcion2 = _db.Cuentas.Where(f => f.empresa == Empresa && f.id == numeroCuenta).Select(f => f.descripcion).SingleOrDefault();

            if (!String.IsNullOrEmpty(descripcion2))
            {
                descripcion = descripcion2;
            }

            else
            {
                descripcion = "";
            }

            return descripcion;
        }

        public string descripcionArticulo(string idArticulo)
        {
            string descripcion = "";
            var descripcion2 = _db.Articulos.Where(f => f.empresa == Empresa && f.id == idArticulo).Select(f => f.descripcion).SingleOrDefault();

            if (!String.IsNullOrEmpty(descripcion2))
            {
                descripcion = descripcion2;
            }

            else
            {
                descripcion = "";
            }

            return descripcion;
        }


        #endregion
    }
}

