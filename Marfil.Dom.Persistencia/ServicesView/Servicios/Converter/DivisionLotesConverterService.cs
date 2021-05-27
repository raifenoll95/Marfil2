using Marfil.Dom.Persistencia.Model.Documentos.DivisionLotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class DivisionLotesConverterService : BaseConverterModel<DivisionLotesModel, DivisionLotes>
    {

        #region constructor
        public DivisionLotesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<DivisionLotes>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as DivisionLotesModel).ToList();

            return result;
        }

        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.DivisionLotes.Any(f => f.empresa == Context.Empresa && f.id == intid);
        }

        public override IModelView CreateView(string id)
        {

            var identificador = Funciones.Qint(id);
            var obj = _db.Set<DivisionLotes>().Where(f => f.empresa == Empresa && f.id == identificador).Include(f => f.DivisionLotesentradalin).Include(f => f.DivisionLotessalidalin).Include(f => f.DivisionLotescostesadicionales).Single();

            var result = GetModelView(obj) as DivisionLotesModel;

            result.Tipodealmacenlote = (TipoAlmacenlote?)obj.tipoalmacenlote;
            var configuracion = _appService.GetConfiguracion(_db);
            result.Materialsalidaigualentrada = configuracion.Materialentradaigualsalida;
            result.LineasEntrada = obj.DivisionLotesentradalin.ToList().Select(f => new DivisionLotesentradaLinModel()
            {

                Id = f.id,
                Fkarticulos = f.fkarticulos,
                Descripcion = f.descripcion,
                Lote = f.lote,
                Tabla = f.tabla,
                Cantidad = f.cantidad,
                Largo = f.largo,
                Ancho = f.ancho,
                Grueso = f.grueso,
                Fkunidades = f.fkunidades,
                Metros = f.metros,
                Notas = f.notas,
                Canal = f.canal,
                Revision = f.revision,
                Decimalesmonedas = f.decimalesmonedas,
                Decimalesmedidas = f.decimalesmedidas,
                Orden = f.orden ?? f.id,
                Costeadicionalvariable = f.costeadicionalvariable,
                Costeadicionalmaterial = f.costeadicionalmaterial,
                Costeadicionalotro = f.costeadicionalotro,
                Costeadicionalportes = f.costeadicionalportes,
                Loteautomaticoid = f.loteautomaticoid,
                Precio = f.precio
            }).ToList();



            result.LineasSalida = obj.DivisionLotessalidalin.ToList().Select(f => new DivisionLotessalidaLinModel(Context)
            {
                Id = f.id,
                Fkarticulos = f.fkarticulos,
                Descripcion = f.descripcion,
                Lote = f.lote,
                Tabla = f.tabla,
                Cantidad = f.cantidad,
                Largo = f.largo,
                Ancho = f.ancho,
                Grueso = f.grueso,
                Fkunidades = f.fkunidades,
                Metros = f.metros,
                Notas = f.notas,
                Canal = f.canal,
                Revision = f.revision,
                Decimalesmonedas = f.decimalesmonedas,
                Decimalesmedidas = f.decimalesmedidas,
                Orden = f.orden ?? f.id,
                //Flagidentifier = f.flagidentifier,
                Precio = f.precio,
            }).ToList();




            var primeritem = result.LineasSalida.FirstOrDefault();
            if (primeritem != null)
            {
                var serviceFamilia = FService.Instance.GetService(typeof(FamiliasproductosModel), Context, _db) as FamiliasproductosService;
                var familia = serviceFamilia.get(ArticulosService.GetCodigoFamilia(primeritem.Fkarticulos)) as FamiliasproductosModel;
                if (familia.Tipofamilia == TipoFamilia.Bloque)
                {
                    result.Lotesalida = primeritem.Lote;
                }
            }



            result.Costes = obj.DivisionLotescostesadicionales.ToList().Select(f => new DivisionLotesCostesadicionalesModel()
            {
                Id = f.id,
                Tipodocumento = (TipoCosteAdicional)f.tipodocumento,
                Referenciadocumento = f.referenciadocumento,
                Importe = f.importe,
                Porcentaje = f.porcentaje,
                Total = f.total,
                Tipocoste = (TipoCoste)f.tipocoste,
                Tiporeparto = (TipoReparto)f.tiporeparto,
                Notas = f.notas
            }).ToList();

            return result;
        }


        public override DivisionLotes CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as DivisionLotesModel;
            var result = _db.Set<DivisionLotes>().Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            result.fechaalta = DateTime.Now;
            result.fechamodificacion = result.fechaalta;
            result.fkusuarioalta = Context.Id;
            result.fkusuariomodificacion = Context.Id;
            result.empresa = Empresa;
            result.tipoalmacenlote = (int?)viewmodel.Tipodealmacenlote;

            foreach (var item in viewmodel.LineasEntrada)
            {
                var newItem = _db.Set<DivisionLotesentradalin>().Create();
                newItem.empresa = Empresa;
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden ?? item.Id;
                newItem.costeadicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                newItem.precio = item.Precio;
                newItem.precio = item.Precio;
                newItem.loteautomaticoid = item.Loteautomaticoid;
                newItem.flagidentifier = Guid.NewGuid();

                result.DivisionLotesentradalin.Add(newItem);
            }

            foreach (var item in viewmodel.LineasSalida)
            {
                var newItem = _db.Set<DivisionLotessalidalin>().Create();
                newItem.empresa = Empresa;
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden ?? item.Id;
                newItem.precio = item.Precio;
                newItem.flagidentifier = Guid.NewGuid();
                result.DivisionLotessalidalin.Add(newItem);
            }


            foreach (var item in viewmodel.Costes)
            {
                var newItem = _db.Set<DivisionLotescostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fkdivisionlotes = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int)item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int)item.Tiporeparto;
                newItem.notas = item.Notas;
                result.DivisionLotescostesadicionales.Add(newItem);
            }

            return result;
        }


        public override DivisionLotes EditPersitance(IModelView obj)
        {
            var viewmodel = obj as DivisionLotesModel;
            var result = _db.DivisionLotes.Where(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id).Include(b => b.DivisionLotesentradalin).Include(b => b.DivisionLotessalidalin).ToList().Single();
            //todo asignar
            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }
            //todo asignar contador y referencia

            result.fechamodificacion = DateTime.Now;
            result.fkusuariomodificacion = Context.Id;
            result.tipoalmacenlote = (int?)viewmodel.Tipodealmacenlote;

            result.DivisionLotesentradalin.Clear();

            foreach (var item in viewmodel.LineasEntrada)
            {
                var newItem = _db.Set<DivisionLotesentradalin>().Create();
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden ?? item.Id;
                newItem.costeadicionalvariable = item.Costeadicionalvariable;
                newItem.costeadicionalmaterial = item.Costeadicionalmaterial;
                newItem.costeadicionalotro = item.Costeadicionalotro;
                newItem.costeadicionalportes = item.Costeadicionalportes;
                newItem.precio = item.Precio;
                newItem.loteautomaticoid = item.Loteautomaticoid;
                newItem.flagidentifier = item.Flagidentifier;
                result.DivisionLotesentradalin.Add(newItem);
            }

            result.DivisionLotessalidalin.Clear();
            foreach (var item in viewmodel.LineasSalida)
            {
                var newItem = _db.Set<DivisionLotessalidalin>().Create();
                newItem.id = item.Id;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.tabla = item.Tabla;
                newItem.cantidad = item.Cantidad;
                newItem.largo = item.Largo;
                newItem.ancho = item.Ancho;
                newItem.grueso = item.Grueso;
                newItem.fkunidades = item.Fkunidades;
                newItem.metros = item.Metros;
                newItem.notas = item.Notas;
                newItem.canal = item.Canal;
                newItem.revision = item.Revision;
                newItem.decimalesmonedas = item.Decimalesmonedas;
                newItem.decimalesmedidas = item.Decimalesmedidas;
                newItem.orden = item.Orden ?? item.Id;
                newItem.precio = item.Precio;
                newItem.flagidentifier = item.Flagidentifier;
                result.DivisionLotessalidalin.Add(newItem);
            }

            foreach (var item in viewmodel.Costes)
            {
                var newItem = _db.Set<DivisionLotescostesadicionales>().Create();
                newItem.empresa = Empresa;
                newItem.fkdivisionlotes = result.id;
                newItem.id = item.Id;
                newItem.tipodocumento = (int)item.Tipodocumento;
                newItem.referenciadocumento = item.Referenciadocumento;
                newItem.importe = item.Importe;
                newItem.porcentaje = item.Porcentaje;
                newItem.total = item.Total;
                newItem.tipocoste = (int)item.Tipocoste;
                newItem.tiporeparto = (int)item.Tiporeparto;
                newItem.notas = item.Notas;
                result.DivisionLotescostesadicionales.Add(newItem);
            }
            return result;
        }
    }
}
