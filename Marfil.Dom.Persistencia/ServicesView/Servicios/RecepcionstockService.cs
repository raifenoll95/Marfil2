using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using System.Data.Entity.Migrations;
using System.Globalization;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IRecepcionStockService
    {

    }

    public class RecepcionStockService : AlbaranesComprasService, IRecepcionStockService
    {
        #region Member

        private string _ejercicioId;
        private IImportacionService _importarService;
        #endregion

        #region Properties

        public string EjercicioId
        {
            get { return _ejercicioId; }
            set
            {
                _ejercicioId = value;
                ((AlbaranesComprasValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion

        #region CTR

        public RecepcionStockService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            Modo = ModoAlbaran.Constock;
            _importarService = new ImportacionService(context);
            _converterModel = FConverterModel.Instance.CreateConverterModelService<AlbaranesComprasModel, AlbaranesCompras>(_context, db, Empresa);
        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.List = st.List.OfType<AlbaranesComprasModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            return st;
        }

        public override string GetSelectPrincipal()
        {
            //return string.Format("select * from albaranescompras where empresa='{0}'{1}", Empresa, " and modo=" + (int)ModoAlbaran.Constock);
            return string.Format("select * from albaranescompras where empresa='{0}'{1}{2}", Empresa, " and modo=" + (int)ModoAlbaran.Constock, " and tipoalbaran<>" + (int)TipoAlbaran.VariosAlmacen);
        }

        #endregion

        #region Generar lineas

        public IEnumerable<ILineaImportar> GetLineasImportarAlbaran(string referencia)
        {
            var result = new List<ILineaImportar>();
            using (var con = new SqlConnection(_db.Database.Connection.ConnectionString))
            {
                var albaran = GetByReferencia(referencia);
                using (var cmd = new SqlCommand(GenerarCadena(referencia, albaran), con))
                {
                    cmd.Parameters.AddWithValue("empresa", Empresa);
                    cmd.Parameters.AddWithValue("referencia", referencia);
                    using (var ad = new SqlDataAdapter(cmd))
                    {

                        var lineas = new DataTable();
                        ad.Fill(lineas);
                        var id = 1;

                        foreach (DataRow row in lineas.Rows)
                        {
                            result.Add(new LineaImportarModel
                            {
                                Id = id++,

                                Canal = lineas.Columns.Contains("Canal") ? Funciones.Qnull(row["Canal"]) : string.Empty,
                                Cantidad = Funciones.Qdouble(row["Cantidad"]) ?? 0,
                                Cuotaiva = Funciones.Qdouble(row["Cuotaiva"]) ?? 0,
                                Cuotarecargoequivalencia = Funciones.Qdouble(row["Cuotarecargoequivalencia"]) ?? 0,
                                Decimalesmedidas = Funciones.Qint(row["Decimalesmedidas"]) ?? 0,
                                Decimalesmonedas = Funciones.Qint(row["Decimalesmonedas"]) ?? 0,
                                Descripcion = Funciones.Qnull(row["Descripcion"]),
                                Fkregimeniva = Funciones.Qnull(row["Fkregimeniva"]),
                                Fkunidades = Funciones.Qnull(row["Fkunidades"]),
                                Metros = Funciones.Qdouble(row["Metros"]) ?? 0,
                                Precio = Funciones.Qdouble(row["Precio"]) ?? 0,
                                Fkarticulos = Funciones.Qnull(row["Fkarticulos"]),
                                Fktiposiva = Funciones.Qnull(row["Fktiposiva"]),
                                Ancho = lineas.Columns.Contains("Ancho") ? Funciones.Qdouble(row["Ancho"]) : null,
                                Grueso = lineas.Columns.Contains("Grueso") ? Funciones.Qdouble(row["Grueso"]) : null,
                                Largo = lineas.Columns.Contains("Largo") ? Funciones.Qdouble(row["Largo"]) : null,
                                Importe = Funciones.Qdouble(row["Importe"]) ?? 0,
                                Importedescuento = Funciones.Qdouble(row["Importedescuento"]) ?? 0,
                                Porcentajedescuento = Funciones.Qdouble(row["Porcentajedescuento"]) ?? 0,
                                Porcentajeiva = Funciones.Qdouble(row["Porcentajeiva"]) ?? 0,
                                Porcentajerecargoequivalencia = Funciones.Qdouble(row["Porcentajerecargoequivalencia"]) ?? 0,
                                Lote = lineas.Columns.Contains("Lote") ? Funciones.Qnull(row["Lote"]) : string.Empty,
                                Notas = string.Empty,//Funciones.Qnull(row["Notas"]),
                                Precioanterior = 0,
                                Revision = "",
                                Tabla = null,
                                Fkdocumento = albaran.Id.ToString(),
                                Fkdocumentoid = "",
                                Fkdocumentoreferencia = ""
                            });
                        }
                    }
                }
            }

            return result;
        }

        private string GenerarCadena(string referencia, AlbaranesComprasModel albaran)
        {
            var sb = new StringBuilder();

            string agrupacion;
            var columnas = GenerarColumnas(referencia, out agrupacion, albaran);
            sb.AppendFormat("select {0} from albaranescompraslin as al inner join albaranescompras as a on a.empresa=@empresa and a.referencia=@referencia and a.id=al.fkalbaranes and a.empresa=al.empresa left join articulos as art on art.id=al.fkarticulos where al.empresa=@empresa {1}", columnas, agrupacion);

            return sb.ToString();
        }

        private string GenerarColumnas(string referencia, out string agrupacion, AlbaranesComprasModel albaran)
        {
            var agrupacionService = FService.Instance.GetService(typeof(CriteriosagrupacionModel), _context) as CriteriosagrupacionService;

            var criterio = agrupacionService.get(albaran.Fkcriteriosagrupacion) as CriteriosagrupacionModel;

            var sb = new StringBuilder();

            if (criterio.Lineas.Any())
            {
                var lineas = criterio.Lineas.OrderBy(f => f.Orden);
                agrupacion = lineas.Any() ? "Group by " : string.Empty;
                agrupacion += string.Join(",", lineas.Select(f => "al." + f.Campoenum.ToString()));

                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Fkarticulos))
                    agrupacion += ",al.Fkarticulos";
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Precio))
                    agrupacion += ",al.Precio";
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Porcentajedescuento))
                    agrupacion += ",al.Porcentajedescuento";

                agrupacion += ",al.empresa,al.Fkunidades,al.Fktiposiva,al.Porcentajerecargoequivalencia,al.Porcentajeiva,al.Decimalesmedidas,al.Decimalesmonedas";

                sb.Append("(select top 1 art.descripcion from articulos as art where art.id=al.fkarticulos and art.empresa=al.empresa) as Descripcion,Min(al.Id) as Id,Sum(al.cantidad) as Cantidad,sum(al.Metros) as Metros,al.Porcentajeiva,Sum(al.Cuotaiva) as Cuotaiva," +
                          " al.Porcentajerecargoequivalencia,sum(al.Cuotarecargoequivalencia) as Cuotarecargoequivalencia,'" + albaran.Fkregimeniva + "' as Fkregimeniva," +
                          " al.Fkunidades, al.Fktiposiva,Sum(al.Importe) as Importe,Sum(al.importedescuento) as Importedescuento,al.Decimalesmedidas,al.Decimalesmonedas, " + string.Join(",", lineas.Select(f => "al." + f.Campoenum.ToString())));

                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Fkarticulos))
                    sb.Append(", al.Fkarticulos");
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Precio))
                    sb.Append(", al.Precio");
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Porcentajedescuento))
                    sb.Append(", al.Porcentajedescuento");
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Descripcion))
                    sb.Append(", (select top 1 art.descripcion from articulos as art where art.id=al.fkarticulos and art.empresa=al.empresa) as Descripcion");


                var cadenaCondiciones = "";
                var vectorAgrupacion = agrupacion.Replace("Group by", "").Split(',');
                foreach (var item in vectorAgrupacion)
                {
                    cadenaCondiciones += " and isnull(" + item + ",-1)= isnull(" + item.Replace("al.", "al2.") + ",-1) ";
                }

                var cadenaFormat =
                    ", (select (case when count(distinct al2.{0}) >1 then 0 else min(al2.{0}) end) from albaranescompraslin as al2 where al2.empresa=@empresa and al2.fkalbaranes={2} {3} {1}) as {0}  ";
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Largo))
                    sb.AppendFormat(cadenaFormat, CamposAgrupacionAlbaran.Largo, agrupacion.Replace("al.", "al2."), albaran.Id, cadenaCondiciones);

                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Ancho))
                    sb.AppendFormat(cadenaFormat, CamposAgrupacionAlbaran.Ancho, agrupacion.Replace("al.", "al2."), albaran.Id, cadenaCondiciones);

                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Grueso))
                    sb.AppendFormat(cadenaFormat, CamposAgrupacionAlbaran.Grueso, agrupacion.Replace("al.", "al2."), albaran.Id, cadenaCondiciones);

            }
            else
            {
                agrupacion = string.Empty;
                sb.Append(" al.*,a.Fkregimeniva");
            }


            return sb.ToString();
        }

        #endregion

        #region Api main

        public void CalcularCosteTotalMetros(List<AlbaranesComprasLinModel> lineas, List<AlbaranesComprasCostesadicionalesModel> costes)
        {
            foreach (var i in costes)
            {
                var tipoDocumento = i.Tipodocumento;
                var codUnidadMedida = "-1";

                if (tipoDocumento == TipoCosteAdicional.Costexm2)
                {
                    codUnidadMedida = "02";
                }
                else if (tipoDocumento == TipoCosteAdicional.Costexm3)
                {
                    codUnidadMedida = "03";
                }

                if (tipoDocumento == TipoCosteAdicional.Costexm2 || tipoDocumento == TipoCosteAdicional.Costexm3)
                {

                    var totalMetros = 0.0d;
                    foreach (var j in lineas)
                    {
                        //MODIFICADO ERROR EN AZURE
                        var unidadMedida = _db.Articulos.Where(f => f.empresa == Empresa && f.id == j.Fkarticulos).Select(f => j.Fkunidades).SingleOrDefault();
                        if (unidadMedida.Equals(codUnidadMedida))
                        {
                            totalMetros += Math.Round((double)j.Metros, 3);
                        }
                    }

                    i.Total = Math.Round((double)i.Importe * totalMetros, 2);
                }
            }
        }

        public void Reclamar(IModelView obj, List<LineaImportarModel> lineas)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as AlbaranesComprasModel;

                List<AlbaranesComprasLinModel> antiguas = new List<AlbaranesComprasLinModel>();
                model.Lineas.Clear();

                foreach(var linea in lineas)
                {
                    if(linea.Cantidad > 0)
                    {
                        model.Lineas.Add(new AlbaranesComprasLinModel
                        {
                            Id = linea.Id,
                            Fkarticulos = linea.Fkarticulos,
                            Descripcion = linea.Descripcion,
                            Lote = linea.Lote,
                            Tabla = linea.Tabla,
                            Cantidad = linea.Cantidad,
                            Canal = linea.Canal,
                            Cuotaiva = linea.Cuotaiva,
                            Cuotarecargoequivalencia = linea.Cuotarecargoequivalencia,
                            Decimalesmedidas = linea.Decimalesmedidas,
                            Decimalesmonedas = linea.Decimalesmonedas,
                            Fkregimeniva = linea.Fkregimeniva,
                            Fkunidades = linea.Fkunidades,
                            Largo = linea.Largo,
                            Ancho = linea.Ancho,
                            Grueso = linea.Grueso,
                            Metros = linea.Metros,
                            Precio = linea.Precio,
                            Fktiposiva = linea.Fktiposiva,
                            Importe = linea.Importe,
                            Importedescuento = linea.Importedescuento,
                            Notas = linea.Notas,
                            Porcentajedescuento = linea.Porcentajedescuento,
                            Porcentajeiva = linea.Porcentajeiva,
                            Precioanterior = linea.Precioanterior,
                            Revision = linea.Revision,
                            Bundle = linea.Bundle,
                            Orden = linea.Orden
                        });
                    }
                    
                    else
                    {
                        antiguas.Add(new AlbaranesComprasLinModel
                        {
                            Id = linea.Id,
                            Fkarticulos = linea.Fkarticulos,
                            Descripcion = linea.Descripcion,
                            Lote = linea.Lote,
                            Tabla = linea.Tabla,
                            Cantidad = linea.Cantidad * (-1),
                            Canal = linea.Canal,
                            Cuotaiva = linea.Cuotaiva,
                            Cuotarecargoequivalencia = linea.Cuotarecargoequivalencia,
                            Decimalesmedidas = linea.Decimalesmedidas,
                            Decimalesmonedas = linea.Decimalesmonedas,
                            Fkregimeniva = linea.Fkregimeniva,
                            Fkunidades = linea.Fkunidades,
                            Largo = linea.Largo,
                            Ancho = linea.Ancho,
                            Grueso = linea.Grueso,
                            Metros = linea.Metros * (-1),
                            Precio = linea.Precio,
                            Fktiposiva = linea.Fktiposiva,
                            Importe = linea.Importe * (-1),
                            Importedescuento = linea.Importedescuento,
                            Notas = linea.Notas,
                            Porcentajedescuento = linea.Porcentajedescuento,
                            Porcentajeiva = linea.Porcentajeiva,
                            Precioanterior = linea.Precioanterior,
                            Revision = linea.Revision,
                            Bundle = linea.Bundle,
                            Orden = linea.Orden
                        });
                    }
                }

                //Se editan las lineas del albaran
                base.edit(model);
                
                GenerarMovimientosLineas(antiguas, model, TipoOperacionService.EliminarRecepcionStock);
                GenerarMovimientosLineas(model.Lineas, model, TipoOperacionService.InsertarRecepcionStock); //Insertar

                GestionarBundleLineas(obj as AlbaranesComprasModel);
                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as AlbaranesComprasModel;
                CalcularCosteTotalMetros(model.Lineas, model.Costes);

                RepartirCostesLineas(model.Lineas, model.Costes);
                ModificarLotesLineas(model);

                base.create(obj);

                if (model.Tipoalbaran == (int)TipoAlbaran.Devolucion)
                {
                    GenerarMovimientosLineas(model.Lineas, model, TipoOperacionService.InsertarRecepcionStockDevolucion);
                }
                
                else if(model.Tipoalbaran == (int)TipoAlbaran.Reclamacion)
                {
                    var antiguas = model.Lineas.Where(f => f.Cantidad < 0).ToList();
                    var nuevas = model.Lineas.Where(f => f.Cantidad > 0).ToList();

                    GenerarMovimientosLineas(antiguas, model, TipoOperacionService.EliminarRecepcionStock);
                    GenerarMovimientosLineas(nuevas, model, TipoOperacionService.InsertarRecepcionStock);

                    //Cuando creamos el albaran de reclamacion, necesitamos introducir en las lineas del albaran original el id y la referencia del reclamado
                    var original = get(model.idOriginalReclamado.ToString()) as AlbaranesComprasModel;
                    foreach (var linea in original.Lineas)
                    {
                        linea.Fkreclamado = model.Id;
                        linea.Fkreclamadoreferencia = model.Referencia;
                    }

                    base.edit(original);  
                }

                else
                    GenerarMovimientosLineas(model.Lineas, model, TipoOperacionService.InsertarRecepcionStock);

                GestionarBundleLineas(obj as AlbaranesComprasModel);
                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void edit(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {

                var original = get(Funciones.Qnull(obj.get("id"))) as AlbaranesComprasModel;
                var editado = obj as AlbaranesComprasModel;


                    // Calcular costesadicionales costexm2 o costexm3
                    CalcularCosteTotalMetros(editado.Lineas, editado.Costes);

                    RepartirCostesLineas(editado.Lineas, editado.Costes, original.Costes);
                    ModificarLotesLineas(editado);
                    
                    base.edit(obj);
                    //ActualizarStock(original, obj as AlbaranesComprasModel);
                    GenerarMovimientosLineas(original.Lineas, original, TipoOperacionService.EliminarRecepcionStock);
                    GenerarMovimientosLineas(editado.Lineas, editado, TipoOperacionService.InsertarRecepcionStock);
                    _db.SaveChanges();
                    tran.Complete();
                
            }
        }

        public override void delete(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                base.delete(obj);
                _db.SaveChanges();
                var model = obj as AlbaranesComprasModel;
                //model.Tipoalbaranenum = TipoAlbaran.Habitual;
                //EliminarStock(model);
                if (model.Tipoalbaran == (int)TipoAlbaran.Devolucion)
                    GenerarMovimientosLineas(model.Lineas, model, TipoOperacionService.ActualizarRecepcionStockDevolucion);
                else
                    GenerarMovimientosLineas(model.Lineas, model, TipoOperacionService.EliminarRecepcionStock);
                tran.Complete();
            }

        }

        #endregion

        #region Get documentos costes adicionales

        public IEnumerable<StDocumentosCompras> GetDocumentosCompras(string id = "")
        {
            return _db.Database.SqlQuery<StDocumentosCompras>(GetSelectDocumentosCompras(id), new SqlParameter("empresa", Empresa), new SqlParameter("fkejercicio", EjercicioId), new SqlParameter("id", id)).ToList();
        }

        private string GetSelectDocumentosCompras(string id)
        {
            var sb = new StringBuilder();

            sb.Append("select 'Albaran de compra' as [Tipo], ac.Referencia as [Referencia], ac.fechadocumento as [Fecha], ac.importebaseimponible as [Base], ac.nombreproveedor as [Proveedor] from Albaranescompras as ac where ac.empresa=@empresa and ac.fkejercicio=@fkejercicio ");
            if (!string.IsNullOrEmpty(id))
                sb.Append(" AND ac.Referencia=@id ");
            sb.Append(" UNION ");
            sb.Append("select'Factura de compra' as [Tipo], fac.Referencia as [Referencia], fac.fechadocumento as [Fecha], fac.importebaseimponible as [Base],fac.nombrecliente as [Proveedor] from Facturascompras as fac where fac.empresa=@empresa and fac.fkejercicio=@fkejercicio ");
            if (!string.IsNullOrEmpty(id))
                sb.Append(" AND fac.Referencia=@id ");

            return sb.ToString();
        }

        #endregion

        #region Movimientos stock

        private void GenerarMovimientosLineas(IEnumerable<AlbaranesComprasLinModel> lineas, AlbaranesComprasModel nuevo, TipoOperacionService movimiento)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<AlbaranesComprasDiarioStockSerializable>();
            var vectorArticulos = new Hashtable();

            //jmm
            var operacion = 1;
            if (movimiento == TipoOperacionService.EliminarRecepcionStock || movimiento == TipoOperacionService.ActualizarRecepcionStockDevolucion)
                operacion = -1;

            foreach (var linea in lineas)
            {
                ArticulosModel articuloObj;
                if (vectorArticulos.ContainsKey(linea.Fkarticulos))
                    articuloObj = vectorArticulos[linea.Fkarticulos] as ArticulosModel;
                else
                {
                    articuloObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;
                    vectorArticulos.Add(linea.Fkarticulos, articuloObj);
                }

                var aux = Funciones.ConverterGeneric<AlbaranesComprasLinSerialized>(linea);

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = nuevo.Fkalmacen,
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = linea.Fkarticulos,
                        Referenciaproveedor = "",
                        Fkcontadorlote = linea.Fkcontadoreslotes,
                        Lote = linea.Lote,
                        Loteid = (linea.Tabla ?? 0).ToString(),
                        Tag = "",
                        Fkunidadesmedida = linea.Fkunidades,
                        Largo = linea.Largo ?? 0,
                        Ancho = linea.Ancho ?? 0,
                        Grueso = linea.Grueso ?? 0,

                        Documentomovimiento = serializer.GetXml(
                            new AlbaranesComprasDiarioStockSerializable
                            {
                                Id = nuevo.Id,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Codigoproveedor = nuevo.Fkproveedores,
                                Linea = aux
                            }),

                        Fkusuarios = Usuarioid,
                        Tipodealmacenlote = nuevo.Tipodealmacenlote,

                        //En los albaranes de reclamacion, las lineas negativas (eliminar stock) ya vienen con la cantidad y los metros en negativo, no hace falta hacer *-1
                        Cantidad = nuevo.Tipoalbaran == (int)TipoAlbaran.Reclamacion && linea.Cantidad < 0 ? (linea.Cantidad ?? 0) : (linea.Cantidad ?? 0) * operacion,
                        Metros = nuevo.Tipoalbaran == (int)TipoAlbaran.Reclamacion && linea.Cantidad < 0 ? (linea.Metros ?? 0) : (linea.Metros ?? 0) * operacion,
                        Pesoneto = nuevo.Tipoalbaran == (int)TipoAlbaran.Reclamacion && linea.Cantidad < 0 ? ((articuloObj.Kilosud ?? 0) * linea.Metros) : ((articuloObj.Kilosud ?? 0) * linea.Metros) * operacion,

                        //Seguimos
                        Costeadicionalmaterial = linea.Costeadicionalmaterial * operacion,
                        Costeadicionalotro = linea.Costeadicionalotro * operacion,
                        Costeadicionalvariable = linea.Costeadicionalvariable * operacion,
                        Costeadicionalportes = linea.Costeadicionalportes * operacion,

                        Tipomovimiento = movimiento
                    };

                    var operacionServicio = linea.Nueva
                        ? TipoOperacionService.InsertarRecepcionStock
                        : TipoOperacionService.ActualizarRecepcionStock;
                    if (nuevo.Tipoalbaranenum == TipoAlbaran.Devolucion)
                    {
                        operacionServicio = linea.Nueva
                        ? TipoOperacionService.InsertarRecepcionStockDevolucion
                        : TipoOperacionService.ActualizarRecepcionStockDevolucion;
                    }

                    if (nuevo.Tipoalbaranenum == TipoAlbaran.Reclamacion && linea.Cantidad < 0)
                    {
                        operacionServicio = TipoOperacionService.SalidaReclamacion;

                    }
                    if (nuevo.Tipoalbaranenum == TipoAlbaran.Reclamacion && linea.Cantidad > 0)
                    {
                        operacionServicio = TipoOperacionService.EntradaReclamacion;

                    }
                    movimientosStockService.GenerarMovimiento(model, operacionServicio);
                }

            }
        }

        #endregion

        #region Importar Pedidos 

        public override AlbaranesComprasModel ImportarPedido(PedidosComprasModel presupuesto)
        {
            var result = Helper.fModel.GetModel<AlbaranesComprasModel>(_context);
            result.Importado = true;
            ImportarCabecera(presupuesto, result);
            base.EstablecerSerie(presupuesto.Fkseries, result);
            result.Fkalmacen = _context.Fkalmacen;
            result.Fkpedidoscompras = presupuesto.Referencia;
            return result;
        }

        private IEnumerable<ILineaImportar> ConvertLineasModelToILineas(string idcabecera, string referencia, IEnumerable<PedidosComprasLinModel> lineas)
        {
            var id = lineas.Any() ? lineas.Max(f => f.Id) : 0;
            id++;

            return lineas.Where(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0) != 0).Select(f => new LineaImportarModel
            {
                Id = id++,
                Ancho = f.Ancho ?? 0,
                Canal = f.Canal,
                Cantidad = (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0),
                Cuotaiva = f.Cuotaiva ?? 0,
                Cuotarecargoequivalencia = f.Cuotarecargoequivalencia ?? 0,
                Decimalesmedidas = f.Decimalesmedidas ?? 0,
                Decimalesmonedas = f.Decimalesmonedas ?? 0,
                Descripcion = f.Descripcion,
                Fkregimeniva = f.Fkregimeniva,
                Fkunidades = f.Fkunidades,
                Metros = f.Metros ?? 0,
                Precio = f.Precio ?? 0,
                Fkarticulos = f.Fkarticulos,
                Fktiposiva = f.Fktiposiva,
                Grueso = f.Grueso ?? 0,
                Importe = f.Importe ?? 0,
                Importedescuento = f.Importedescuento ?? 0,
                Largo = f.Largo ?? 0,
                Lote = f.Lote,
                Notas = f.Notas,
                Porcentajedescuento = f.Porcentajedescuento ?? 0,
                Porcentajeiva = f.Porcentajeiva ?? 0,
                Porcentajerecargoequivalencia = f.Porcentajerecargoequivalencia ?? 0,
                Precioanterior = f.Precioanterior ?? 0,
                Revision = f.Revision,
                Tabla = f.Tabla ?? 0,
                Fkdocumento = idcabecera,
                Fkdocumentoid = f.Id.ToString(),
                Fkdocumentoreferencia = referencia
            });
        }

        private void EstablecerSerie(string fkseries, AlbaranesComprasModel result)
        {
            var service = FService.Instance.GetService(typeof(SeriesModel), _context);

            var serieObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.PedidosVentas), fkseries)) as SeriesModel;
            var serieAsociada = serieObj.Fkseriesasociada;
            if (!string.IsNullOrEmpty(serieAsociada))
            {
                var serieasociadaObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.AlbaranesCompras), serieAsociada)) as SeriesModel;
                result.Fkseries = serieasociadaObj.Id;
            }
            else
                throw new ValidationException(Inf.ResourcesGlobalization.Textos.Entidades.Presupuestos.ErrorNoExisteSerieAsociada);
        }

        public IEnumerable<AlbaranesComprasLinModel> ImportarLineasReclamadas(IEnumerable<ILineaImportar> duplicadas, int maxId)
        {
            List<AlbaranesComprasLinModel> lineas = new List<AlbaranesComprasLinModel>();
            var id = maxId++;

            foreach(var linea in duplicadas)
            {
                lineas.Add(new AlbaranesComprasLinModel
                {         
                    Cantidadpedida = 0,
                    Id = id,
                    Ancho = linea.Ancho,
                    Canal = linea.Canal,
                    Cantidad = linea.Cantidad,
                    Cuotaiva = linea.Cuotaiva,
                    Cuotarecargoequivalencia = linea.Cuotarecargoequivalencia,
                    Decimalesmedidas = linea.Decimalesmedidas,
                    Decimalesmonedas = linea.Decimalesmonedas,
                    Descripcion = linea.Descripcion,
                    Fkregimeniva = linea.Fkregimeniva,
                    Fkunidades = linea.Fkunidades,
                    Metros = linea.Metros,
                    Precio = linea.Precio,
                    Fkarticulos = linea.Fkarticulos,
                    Fktiposiva = linea.Fktiposiva,
                    Grueso = linea.Grueso,
                    Importe = linea.Importe,
                    Importedescuento = linea.Importedescuento,
                    Largo = linea.Largo,
                    Lote = linea.Lote,
                    Notas = linea.Notas,
                    Porcentajedescuento = linea.Porcentajedescuento,
                    Porcentajeiva = linea.Porcentajeiva,
                    Porcentajerecargoequivalencia = linea.Porcentajerecargoequivalencia,
                    Precioanterior = linea.Precioanterior,
                    Revision = linea.Revision,
                    Tabla = linea.Tabla,
                    Bundle = linea.Bundle,
                    Fkpedidos = Funciones.Qint(linea.Fkdocumento),
                    Fkpedidosid = Funciones.Qint(linea.Fkdocumentoid),
                    Fkpedidosreferencia = linea.Fkdocumentoreferencia
                });
                id++;
            }

            return lineas;
        }

        public IEnumerable<AlbaranesComprasLinModel> ImportarLineas(int maxId, IEnumerable<ILineaImportar> linea)
        {
            return linea.Where(f => f.Cantidad != 0).Select(f => _importarService.ImportarLinea(f)).Select(f => ConvertILineaImportarToPedidoLinModel(++maxId, f));
        }

        private AlbaranesComprasLinModel ConvertILineaImportarToPedidoLinModel(int Id, ILineaImportar linea)
        {
            var result = _importarService.ImportarLinea(linea);
            return new AlbaranesComprasLinModel
            {
                Cantidadpedida = 0,
                Id = Id,
                Ancho = result.Ancho,
                Canal = result.Canal,
                Cantidad = result.Cantidad,
                Cuotaiva = result.Cuotaiva,
                Cuotarecargoequivalencia = result.Cuotarecargoequivalencia,
                Decimalesmedidas = result.Decimalesmedidas,
                Decimalesmonedas = result.Decimalesmonedas,
                Descripcion = result.Descripcion,
                Fkregimeniva = result.Fkregimeniva,
                Fkunidades = result.Fkunidades,
                Metros = result.Metros,
                Precio = result.Precio,
                Fkarticulos = result.Fkarticulos,
                Fktiposiva = result.Fktiposiva,
                Grueso = result.Grueso,
                Importe = result.Importe,
                Importedescuento = result.Importedescuento,
                Largo = result.Largo,
                Lote = result.Lote,
                Notas = result.Notas,
                Porcentajedescuento = result.Porcentajedescuento,
                Porcentajeiva = result.Porcentajeiva,
                Porcentajerecargoequivalencia = result.Porcentajerecargoequivalencia,
                Precioanterior = result.Precioanterior,
                Revision = result.Revision,
                Tabla = result.Tabla,
                Bundle = result.Bundle,
                Fkpedidos = Funciones.Qint(result.Fkdocumento),
                Fkpedidosid = Funciones.Qint(result.Fkdocumentoid),
                Fkpedidosreferencia = result.Fkdocumentoreferencia
            };

        }


        private void ImportarCabecera(PedidosComprasModel presupuesto, AlbaranesComprasModel result)
        {
            var properties = typeof(AlbaranesComprasModel).GetProperties();

            foreach (var item in properties)
            {
                if (item.Name != "Lineas" && item.Name != "Totales")
                {
                    var property = typeof(PedidosComprasModel).GetProperty(item.Name);
                    if (property != null && property.CanWrite)
                    {

                        var value = property.GetValue(presupuesto);
                        item.SetValue(result, value);
                    }
                }
            }

            result.Fechadocumento = DateTime.Now;
            var appService = new ApplicationHelper(_context);
            result.Fkestados = appService.GetConfiguracion().Estadoalbaranescomprasinicial;
        }

        public IEnumerable<AlbaranesComprasModel> BuscarAlbaranesComprasImportar(string cliente, string albaraninicio, string albaranfin)
        {
            int? fkalbaraninicio = null;
            int? fkalbaranfin = null;

            if (!string.IsNullOrEmpty(albaraninicio))
            {
                var objalbaran = _db.AlbaranesCompras.Single(f => f.empresa == Empresa && f.referencia == albaraninicio);
                fkalbaraninicio = objalbaran.id;

            }

            if (!string.IsNullOrEmpty(albaranfin))
            {
                var objalbaran = _db.AlbaranesCompras.Single(f => f.empresa == Empresa && f.referencia == albaranfin);
                fkalbaranfin = objalbaran.id;

            }

            var list = _db.AlbaranesCompras.Where(
                      f =>
                          f.empresa == Empresa && f.fkproveedores == cliente &&
                          (!fkalbaraninicio.HasValue || f.id >= fkalbaraninicio.Value) &&
                          (!fkalbaranfin.HasValue || f.id <= fkalbaranfin.Value) &&
                          !_db.FacturasLin.Any(j => j.fkalbaranesreferencia == f.referencia)).ToList().Select(f => _converterModel.GetModelView(f) as AlbaranesComprasModel).ToList();

            list = list.Join(_db.Estados, p => p.Fkestados, e => e.documento + "-" + e.id, (a, b) => new { a, b }).ToList().Where(f => f.b.tipoestado <= (int)TipoEstado.Curso).Select(f => f.a).OrderByDescending(f => f.Id).ToList();

            var filterItems = list.Where(f => !string.IsNullOrEmpty(f.Fkobras));
            foreach (var item in filterItems)
            {

                var obraInt = Funciones.Qint(item.Fkobras);
                if (obraInt.HasValue)
                {
                    var obraobj = _db.Obras.SingleOrDefault(f => f.empresa == Empresa && f.id == obraInt);
                    item.Obradescripcion = obraobj?.nombreobra;
                }

            }

            return list;




        }

        #endregion

        #region Helper

        #region Gestion Bundles 

        private void GestionarBundleLineas(AlbaranesComprasModel model)
        {

            var lineas = model.Lineas.Where(f => !string.IsNullOrEmpty(f.Bundle));
            var group = lineas.GroupBy(f => new { f.Lote, f.Bundle });

            foreach (var item in group)
            {
                GenerarBundle(item.Key.Lote, item.Key.Bundle, model, lineas.Where(f => f.Lote == item.Key.Lote && f.Bundle == item.Key.Bundle));
            }
        }

        private void GenerarBundle(string lote, string bundle, AlbaranesComprasModel model, IEnumerable<AlbaranesComprasLinModel> lineas)
        {
            var serviceBundle = FService.Instance.GetService(typeof(BundleModel), _context, _db) as BundleService;

            var existe = serviceBundle.exists(string.Format("{0};{1}", lote, bundle));
            var bundleObj = existe
                ? serviceBundle.get(string.Format("{0};{1}", lote, bundle)) as BundleModel
                : new BundleModel
                {
                    Empresa = Empresa,
                    Lote = lote,
                    Codigo = bundle,
                    Descripcion = bundle,
                    Fecha = DateTime.Now,
                    Fkalmacen = model.Fkalmacen,
                    Fkzonaalmacen = Funciones.Qint(model.Fkzonas)
                };

            var maxId = bundleObj.Lineas.Any() ? bundleObj.Lineas.Max(f => f.Id) + 1 : 1;

            var lineasAgregar =
                lineas.Where(
                    j =>
                        !bundleObj.Lineas.Any(f => f.Lote == j.Lote && f.Loteid == (j.Tabla?.ToString() ?? "0"))).Select(f => new BundleLinModel
                        {
                            Id = maxId++,
                            Fkalmacenes = bundleObj.Fkalmacen,
                            Lote = f.Lote,
                            Loteid = (f.Tabla?.ToString() ?? "0"),
                            Fkarticulos = f.Fkarticulos,
                            Descripcion = f.Descripcion,
                            //todo pendiente el coste
                            Cantidad = f.Cantidad,
                            Largo = f.Largo,
                            Ancho = f.Ancho,
                            Grueso = f.Grueso,
                            Metros = f.Metros,
                            Fkunidades = f.Fkunidades,
                            Decimalesunidades = f.Decimalesmedidas,
                            Decimalesprecio = f.Decimalesmonedas
                        }).ToList();

            if (lineasAgregar.Any())
            {
                bundleObj.Lineas.AddRange(lineasAgregar);
            }

            if (existe)
                serviceBundle.edit(bundleObj);
            else
                serviceBundle.create(bundleObj);

        }

        #endregion 

        private struct StLote
        {
            public string Lote { get; set; }
            public int Numero { get; set; }
        }

        private void ModificarLotesLineas(AlbaranesComprasModel model)
        {
            if (model.Tipoalbaranenum == TipoAlbaran.Devolucion) return;
            var familiaService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db);

            var contadorlotesService = FService.Instance.GetService(typeof(ContadoresLotesModel), _context, _db) as ContadoresLotesService;
            var vectoridentificadorlotes = new Dictionary<string, StLote>();
            var vecetorincrementocontadores = new Dictionary<string, int>();


            foreach (var item in model.Lineas)
            {
                if (item.Nueva && string.IsNullOrEmpty(item.Loteautomaticoid))
                {
                    if (_db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == item.Lote && f.loteid == item.Tabla.ToString()))
                        throw new ValidationException(string.Format("El Lote: {0}.{1} ya existe en el Stock", item.Lote, item.Tabla));

                }
                else if (item.Nueva && !string.IsNullOrEmpty(item.Loteautomaticoid))
                {


                    var familiaObj = familiaService.get(ArticulosService.GetCodigoFamilia(item.Fkarticulos)) as FamiliasproductosModel;
                    if (familiaObj.Tipogestionlotes > Tipogestionlotes.Singestion)
                    {
                        if (!vecetorincrementocontadores.ContainsKey(familiaObj.Fkcontador))
                            vecetorincrementocontadores.Add(familiaObj.Fkcontador, 0);

                        var loteObj = contadorlotesService.get(familiaObj.Fkcontador) as ContadoresLotesModel;
                        var objlote = new StLote();
                        if (vectoridentificadorlotes.ContainsKey(item.Loteautomaticoid))
                        {
                            objlote = vectoridentificadorlotes[item.Loteautomaticoid];
                        }
                        else
                        {
                            var incremento = vecetorincrementocontadores[familiaObj.Fkcontador];
                            objlote = new StLote { Lote = contadorlotesService.CreateLoteId(loteObj, ref incremento), Numero = 0 };
                            vecetorincrementocontadores[familiaObj.Fkcontador] = incremento;
                        }

                        if (familiaObj.Tipofamilia != TipoFamilia.Tabla)
                            objlote.Numero = 0;
                        else
                            objlote.Numero++;

                        if (vectoridentificadorlotes.ContainsKey(item.Loteautomaticoid))
                            vectoridentificadorlotes.Remove(item.Loteautomaticoid);



                        item.Lote = objlote.Lote;
                        item.Tabla = objlote.Numero;
                        item.Tblnum = objlote.Numero;

                        vectoridentificadorlotes.Add(item.Loteautomaticoid, objlote);

                    }
                }


            }

            // si tipolotelineas diferente a de cabecera: actualiza todas las lineas
            if (model.Lineas.Count > 0)
            {
                if (model.Tipodealmacenlote != model.Lineas[0].Tipodealmacenlote)
                {
                    foreach (var item in model.Lineas)
                    {
                        item.Tipodealmacenlote = model.Tipodealmacenlote;
                    }
                }
            }
        }

        #region Reparto de costes

        private void RepartirCostesLineas(List<AlbaranesComprasLinModel> lineas, List<AlbaranesComprasCostesadicionalesModel> costes, List<AlbaranesComprasCostesadicionalesModel> costesOriginal = null)
        {
            //limpiar costes
            if (costesOriginal != null)
            {
                if (SonIgualesCostesOriginalEditado(costes, costesOriginal))
                {
                    return;
                }
            }

            foreach (var item in lineas)
            {
                item.Costeadicionalmaterial = 0;
                item.Costeadicionalotro = 0;
                item.Costeadicionalportes = 0;
                item.Costeadicionalvariable = 0;
                item.Flagidentifier = Guid.NewGuid();
            }
            var costesGrupo = costes.GroupBy(f => new { f.Tipocoste, f.Tiporeparto });
            foreach (var item in costesGrupo)
            {
                ReparteCoste(lineas, costes, item.Key);
            }
        }

        private bool SonIgualesCostesOriginalEditado(List<AlbaranesComprasCostesadicionalesModel> costes,
            List<AlbaranesComprasCostesadicionalesModel> costesOriginal)
        {
            var result = true;

            try
            {
                if (costes.Count != costesOriginal.Count || costes.Any(item => !costesOriginal.Any(
                    f =>
                        f.Id == item.Id && f.Total == item.Total && f.Tipodocumento == item.Tipodocumento &&
                        f.Tipocoste == item.Tipocoste && f.Tiporeparto == item.Tiporeparto)))
                {
                    result = false;
                }
            }
            catch (Exception)
            {

                result = false;
            }

            return result;
        }

        private void ReparteCoste(List<AlbaranesComprasLinModel> lineas,
            List<AlbaranesComprasCostesadicionalesModel> costes, dynamic reparto)
        {
            TipoReparto tiporeparto = reparto.Tiporeparto;
            TipoCoste tipocoste = reparto.Tipocoste;
            var costeTotal = costes.Where(f => f.Tiporeparto == tiporeparto && f.Tipocoste == tipocoste).Sum(f => f.Total);


            var costeUnidad = 0.0;
            if (tiporeparto == TipoReparto.Cantidad)
            {
                var d = costeTotal / lineas.Sum(f => f.Cantidad);
                if (d != null)
                    costeUnidad = (double)d;
            }
            else if (tiporeparto == TipoReparto.Importe)
            {
                var d = costeTotal / lineas.Sum(f => f.Importe);
                if (d != null)
                    costeUnidad = (double)d;
            }
            else if (tiporeparto == TipoReparto.Metros)
            {
                var d = costeTotal / lineas.Sum(f => f.Metros);
                if (d != null)
                    costeUnidad = (double)d;
            }
            else if (tiporeparto == TipoReparto.Peso)
            {
                var d = costeTotal / lineas.Sum(f => (f.Largo * f.Ancho * f.Grueso));
                if (d != null)
                    costeUnidad = (double)d;
                //var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
                //var d = costeTotal / lineas.Sum(f => ModeloNegocioFunciones.CalculaEquivalentePeso(((UnidadesModel)unidadesService.get(f.Fkunidades)).Formula == TipoStockFormulas.Superficie, f.Metros ?? 0, f.Grueso ?? 0));
                //if (d != null)
                //    costeUnidad = (double)d;
            }

            RepartirCosteEnLinea(lineas, tiporeparto, tipocoste, costeTotal ?? 0, costeUnidad);



        }

        private void RepartirCosteEnLinea(List<AlbaranesComprasLinModel> lineas, TipoReparto reparto, TipoCoste coste, double costeTotal, double costeUnidad)
        {
            foreach (var item in lineas)
            {
                item.Flagidentifier = Guid.NewGuid();
                var costeLineas = 0.0;
                if (reparto == TipoReparto.Cantidad)
                {
                    var d = item.Cantidad * costeUnidad;
                    if (d != null) costeLineas = (double)d;
                }
                else if (reparto == TipoReparto.Importe)
                {
                    var d = item.Importe * costeUnidad;
                    if (d != null) costeLineas = (double)d;
                }
                else if (reparto == TipoReparto.Metros)
                {
                    var d = item.Metros * costeUnidad;
                    if (d != null) costeLineas = (double)d;
                }
                else if (reparto == TipoReparto.Peso)
                {
                    var d = item.Largo * item.Ancho * item.Grueso * costeUnidad;
                    if (d != null) costeLineas = (double)d;
                    //var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
                    //var d = ModeloNegocioFunciones.CalculaEquivalentePeso(((UnidadesModel)unidadesService.get(item.Fkunidades)).Formula == TipoStockFormulas.Superficie, item.Metros ?? 0, item.Grueso ?? 0) * costeUnidad;
                    //if (d != null) costeLineas = (double)d;
                }

                costeLineas = Math.Round(costeLineas, item.Decimalesmonedas ?? 2);

                if (coste == TipoCoste.Material)
                {
                    item.Costeadicionalmaterial += costeLineas;
                }
                else if (coste == TipoCoste.Otros)
                {
                    item.Costeadicionalotro += costeLineas;
                }
                else if (coste == TipoCoste.Portes)
                {
                    item.Costeadicionalportes += costeLineas;
                }
                costeTotal -= costeLineas;
            }
            //Esto lo hacemos para asegurarnos de que los costes cuadran con lo que se debe asignar ya que puede haber problemas de redondeos
            if (costeTotal != 0)
            {
                var ultimaLinea = lineas.LastOrDefault();
                if (ultimaLinea != null)
                {
                    costeTotal = Math.Round(costeTotal, ultimaLinea.Decimalesmonedas ?? 2);

                    if (coste == TipoCoste.Material)
                    {
                        ultimaLinea.Costeadicionalmaterial += costeTotal;
                    }
                    else if (coste == TipoCoste.Otros)
                    {
                        ultimaLinea.Costeadicionalotro += costeTotal;
                    }
                    else if (coste == TipoCoste.Portes)
                    {
                        ultimaLinea.Costeadicionalportes += costeTotal;
                    }
                }

            }
        }

        #endregion

        #endregion

        #region Api importar stock

        public IEnumerable<AlbaranesComprasLinModel> CrearNuevasLineas(List<AlbaranesComprasLinModel> lineas, AlbaranesComprasLinVistaModel linea)
        {
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), _context, _db) as TiposivaService;

            var articuloObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;
            var familiaObj = familiasService.get(ArticulosService.GetCodigoFamilia(linea.Fkarticulos)) as FamiliasproductosModel;
            var unidadesObj = unidadesService.get(familiaObj.Fkunidadesmedida) as UnidadesModel;

            //Cuando el tipo de iva sea null, captura el primero que haya. 8 M 2021
            if(linea.Fktiposiva == null)
            {
                linea.Fktiposiva = "000";
            }

            var tiposivaObj = tiposivaService.get(linea.Fktiposiva) as TiposIvaModel;

            var result = new List<AlbaranesComprasLinModel>();
            if (!string.IsNullOrEmpty(linea.Lote))
                linea.Lote = linea.Lote.ToUpper();
            if (articuloObj.Articulocomentariovista)
            {
                linea.Cantidad = 1;
            }
            var incremento = (familiaObj.Tipofamilia == TipoFamilia.Bloque ||
                              familiaObj.Tipofamilia == TipoFamilia.Tabla)
                ? 1
                : linea.Cantidad;

            string lote;
            string loteautomaticoid;
            int lotenuevocontador;
            int? tabla;
            AlbaranesComprasGesionLotesService.GestionarLote(articuloObj, familiaObj, lineas.Select(f => (IDocumentosLinModel)f), linea, out lote, out loteautomaticoid, out lotenuevocontador, out tabla);

            var metros = UnidadesService.CalculaResultado(unidadesObj, incremento, linea.Largo, linea.Ancho,
               linea.Grueso, linea.Metros);



            for (var i = 0; i < linea.Cantidad; i += incremento)
            {
                linea.Metros = metros;
                var bruto = linea.Metros * linea.Precio;
                var importedescuento = Math.Round(bruto * linea.Descuento / 100.0, linea.Decimalesmonedas);
                var total = bruto - importedescuento;

                result.Add(new AlbaranesComprasLinModel
                {
                    Nueva = true,
                    Fkarticulos = linea.Fkarticulos,
                    Descripcion = articuloObj.Descripcion,
                    Fkcontadoreslotes = linea.Loteautomatico ? familiaObj.Fkcontador : string.Empty,
                    Lote = lote,
                    Loteautomaticoid = loteautomaticoid,
                    Lotenuevocontador = lotenuevocontador,
                    Tabla = tabla,
                    Tblnum = tabla,
                    Cantidad = articuloObj.Articulocomentariovista ? 0 : incremento,
                    Largo = linea.Largo,
                    Ancho = linea.Ancho,
                    Grueso = linea.Grueso,
                    Fkunidades = linea.Fkunidades,
                    Metros = articuloObj.Articulocomentariovista ? 0 : metros,
                    Precio = linea.Precio,
                    Porcentajedescuento = linea.Descuento,
                    Importedescuento = importedescuento,
                    Importe = total,
                    Decimalesmedidas = linea.Decimalesmedidas,
                    Decimalesmonedas = linea.Decimalesmonedas,
                    Fktiposiva = tiposivaObj.Id,
                    Porcentajeiva = tiposivaObj.PorcentajeIva,
                    Porcentajerecargoequivalencia = tiposivaObj.PorcentajeRecargoEquivalencia,
                    Bundle = linea.Bundle?.ToUpper(),
                    Flagidentifier = Guid.NewGuid()
                }
                 );

                if (tabla.HasValue)
                    tabla++;
            }

            return result;

        }

        #endregion

        #region Saldar pedido

        public void SaldarPedidos(OperacionSaldarPedidosModel model, AlbaranesComprasModel entrega)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                entrega.Pedidosaldado = true;
                SaldarLineaPedidos(model);
                edit(entrega);
                _db.SaveChanges();
                tran.Complete();

            }
        }

        private void SaldarLineaPedidos(OperacionSaldarPedidosModel model)
        {
            var pedidosService = FService.Instance.GetService(typeof(PedidosComprasModel), _context, _db) as PedidosComprasService;
            var pedidoreferenciaObj = pedidosService.GetByReferencia(model.Fkpedidos);
            var pedidoobj = pedidosService.get(pedidoreferenciaObj.Id.ToString()) as PedidosComprasModel;



            var agrupacionArticulos = pedidoobj.Lineas.GroupBy(f => f.Fkarticulos);
            foreach (var item in agrupacionArticulos)
                AsignarCantidadesPedidas(item, pedidoobj, model);


            pedidosService.edit(pedidoobj);

        }

        private void AsignarCantidadesPedidas(IGrouping<string, PedidosComprasLinModel> item, PedidosComprasModel pedido, OperacionSaldarPedidosModel model)
        {
            if (pedido.Lineas.Count(f => f.Fkarticulos == item.Key) > 1)
            {
                var agrupacionArticulos =
                    pedido.Lineas.Where(f => f.Fkarticulos == item.Key)
                        .GroupBy(f => new { f.Fkarticulos, f.Largo, f.Ancho, f.Grueso });

                foreach (var lineaarticulo in agrupacionArticulos)
                    AsignarPorArticuloMedidas(lineaarticulo.Key.Fkarticulos, lineaarticulo.Key.Largo,
                        lineaarticulo.Key.Ancho, lineaarticulo.Key.Grueso, pedido, model);

            }
            else
            {
                AsignarPorArticulo(item.Key, pedido, model);
            }
        }

        private void AsignarPorArticuloMedidas(string fkarticulos, double? largo, double? ancho, double? grueso, PedidosComprasModel pedido, OperacionSaldarPedidosModel model)
        {
            var linea = model.Lineas.Single(f => f.Codigo == fkarticulos && Funciones.Qdouble(f.Largo.Replace(".", "").Replace(",", ".")) == largo && Funciones.Qdouble(f.Ancho.Replace(".", "").Replace(",", ".")) == ancho && Funciones.Qdouble(f.Grueso.Replace(".", "").Replace(",", ".")) == grueso);
            var cantidadpedida = linea.Cantidadpedida - linea.Cantidadpendiente;
            foreach (var item in pedido.Lineas)
            {
                if (item.Fkarticulos == fkarticulos
                    && item.Largo == largo
                    && item.Ancho == ancho
                    && item.Grueso == grueso)
                {
                    var cantidadpendiente = (item.Cantidad ?? 0) - (item.Cantidadpedida ?? 0);
                    var auxcantidad = cantidadpendiente >= cantidadpedida ? cantidadpedida : cantidadpendiente;
                    item.Cantidadpedida = (item.Cantidadpedida ?? 0) + auxcantidad;
                    cantidadpedida -= auxcantidad;
                }
            }
        }

        private void AsignarPorArticulo(string fkarticulo, PedidosComprasModel pedido, OperacionSaldarPedidosModel model)
        {
            var linea = model.Lineas.Single(f => f.Codigo == fkarticulo);

            var cantidadpedida = linea.Cantidadpedida - linea.Cantidadpendiente;
            foreach (var item in pedido.Lineas)
            {
                if (item.Fkarticulos == fkarticulo)
                {
                    var cantidadpendiente = (item.Cantidad ?? 0) - (item.Cantidadpedida ?? 0);
                    var auxcantidad = cantidadpendiente >= cantidadpedida ? cantidadpedida : cantidadpendiente;
                    item.Cantidadpedida = (item.Cantidadpedida ?? 0) + auxcantidad;
                    cantidadpedida -= auxcantidad;
                }
            }
        }


        public IEnumerable<SaldarPedidosModel> GetLineasSaldarPedidos(string pedidoreferencia, List<AlbaranesComprasLinModel> lineas)
        {
            var result = new List<SaldarPedidosModel>();
            var pedidosService = FService.Instance.GetService(typeof(PedidosComprasModel), _context, _db) as PedidosComprasService;
            var pedidosreferenciaObj = pedidosService.GetByReferencia(pedidoreferencia);
            var pedidosObj = pedidosService.get(pedidosreferenciaObj.Id.ToString()) as PedidosComprasModel;

            var agrupacionLineas = pedidosObj.Lineas.GroupBy(f => f.Fkarticulos);


            foreach (var item in agrupacionLineas)
            {
                if (pedidosObj.Lineas.Count(f => f.Fkarticulos == item.Key) > 1)
                {
                    var lineasArticulos = pedidosObj.Lineas.Where(f => f.Fkarticulos == item.Key).GroupBy(f => new { f.Fkarticulos, f.Largo, f.Ancho, f.Grueso, f.Decimalesmedidas });
                    foreach (var art in lineasArticulos)
                    {
                        var selectedArts = pedidosObj.Lineas.Where(f => f.Fkarticulos == art.Key.Fkarticulos && Funciones.Qdouble(f.Largo) == art.Key.Largo && Funciones.Qdouble(f.Ancho) == art.Key.Ancho && Funciones.Qdouble(f.Grueso) == art.Key.Grueso).ToList();
                        var cantidadtotal = selectedArts.Sum(f => (f.Cantidad ?? 0));
                        var cantidadpedida = selectedArts.Sum(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0));
                        var metrosalbaran = (lineas.Where(f => f.Fkarticulos == item.Key && Funciones.Qdouble(f.Largo) == art.Key.Largo && Funciones.Qdouble(f.Ancho) == art.Key.Ancho && Funciones.Qdouble(f.Grueso) == art.Key.Grueso).Sum(f => f.Metros) ?? 0);

                        var metrospedidos = selectedArts.Sum(f => f.Metros) ?? 0;
                        metrospedidos = metrospedidos * (cantidadpedida / cantidadtotal);
                        result.Add(new SaldarPedidosModel()
                        {
                            Id = Guid.NewGuid(),
                            Codigo = art.Key.Fkarticulos,
                            Cantidadpedida = cantidadpedida,
                            Largo = art.Key.Largo?.ToString("N" + (art.Key.Decimalesmedidas ?? 2)),
                            Ancho = art.Key.Ancho?.ToString("N" + (art.Key.Decimalesmedidas ?? 2)),
                            Grueso = art.Key.Grueso?.ToString("N" + (art.Key.Decimalesmedidas ?? 2)),
                            Metros = metrospedidos.ToString("N" + (art.Key.Decimalesmedidas ?? 2)),
                            Cantidadalbaran = lineas.Where(f => f.Fkarticulos == art.Key.Fkarticulos && Funciones.Qdouble(f.Largo) == art.Key.Largo && Funciones.Qdouble(f.Ancho) == art.Key.Ancho && Funciones.Qdouble(f.Grueso) == art.Key.Grueso).Sum(f => f.Cantidad) ?? 0,
                            Metrosalbaran = (metrosalbaran).ToString("N" + art.Key.Decimalesmedidas),
                            Cantidadpendiente = metrospedidos > 0 ? cantidadpedida - (cantidadpedida * metrosalbaran / metrospedidos) : 0,
                        });
                    }
                }
                else
                {
                    var cantidadtotal = item.Sum(f => f.Cantidad ?? 0);
                    var cantidadpedida = item.Sum(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0));
                    var metrosalbaran = (lineas.Where(f => f.Fkarticulos == item.Key).Sum(f => f.Metros) ?? 0);
                    var metrospedidos = pedidosObj.Lineas.Where(f => f.Fkarticulos == item.Key).Sum(f => f.Metros) ?? 0;
                    metrospedidos = metrospedidos * (cantidadpedida / cantidadtotal);
                    var linea = pedidosObj.Lineas.Single(f => f.Fkarticulos == item.Key);

                    result.Add(new SaldarPedidosModel()
                    {
                        Id = Guid.NewGuid(),
                        Codigo = item.Key,
                        Cantidadpedida = cantidadpedida,
                        Largo = linea.Largo?.ToString("N" + (linea.Decimalesmedidas ?? 2)),
                        Ancho = linea.Ancho?.ToString("N" + (linea.Decimalesmedidas ?? 2)),
                        Grueso = linea.Grueso?.ToString("N" + (linea.Decimalesmedidas ?? 2)),
                        Metros = metrospedidos.ToString("N" + (linea.Decimalesmedidas ?? 2)),
                        Cantidadalbaran = lineas.Where(f => f.Fkarticulos == item.Key).Sum(f => f.Cantidad) ?? 0,
                        Metrosalbaran = (metrosalbaran).ToString("N" + linea.Decimalesmedidas),
                        Cantidadpendiente = metrospedidos > 0 ? cantidadpedida - (cantidadpedida * metrosalbaran / metrospedidos) : 0,
                    });
                }

            }

            return result;
        }

        #endregion

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as AlbaranesComprasValidation;
            currentValidationService.CambiarEstado = true;
            model.set("fkestados", nuevoEstado.CampoId);
            edit(model);
            currentValidationService.CambiarEstado = false;
        }

        public void ModificarCostes(AlbaranesComprasModel model)
        {
            var currentValidationService = _validationService as AlbaranesComprasValidation;
            currentValidationService.ModificarCostes = true;
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {

                var original = get(Funciones.Qnull(model.get("id"))) as AlbaranesComprasModel;
                var editado = model as AlbaranesComprasModel;


                // Calcular costesadicionales costexm2 o costexm3
                CalcularCosteTotalMetros(editado.Lineas, editado.Costes);

                RepartirCostesLineas(editado.Lineas, editado.Costes, original.Costes);

                base.edit(model);
                //ActualizarStock(original, obj as AlbaranesComprasModel);
                GenerarMovimientosLineas(original.Lineas, original, TipoOperacionService.EliminarCostes);
                GenerarMovimientosLineas(editado.Lineas, editado, TipoOperacionService.InsertarCostes);
                _db.SaveChanges();
                tran.Complete();

            }
            currentValidationService.ModificarCostes = false;
        }


        #region Importar

        public void Importar(DataTable dt, string serie, int tipoLote, int idPeticion, IContextService context)
        {
            // Ordenar por Referencia
            DataView dv = dt.DefaultView;
            dv.Sort = "Proveedor asc, Fecha asc";
            DataTable sorted = dv.ToTable();

            string errores = "";
            var moneda = _db.Empresas.Where(f => f.id == Empresa).Select(f => f.fkMonedabase).SingleOrDefault();

            List<RecepcionesStockModel> ListaAlbaranes = new List<RecepcionesStockModel>();
            RecepcionesStockModel albaran = new FModel().GetModel<RecepcionesStockModel>(context);

            albaran.Fkproveedores = sorted.Rows[0]["Proveedor"].ToString();
            albaran.Fechadocumento = Convert.ToDateTime("01/01/" + Convert.ToDateTime(sorted.Rows[0]["Fecha"].ToString()).Year);
            albaran.Fkseries = serie;
            albaran.Fkejercicio = _db.Ejercicios.Where(f => f.empresa == Empresa && f.desde <= albaran.Fechadocumento
                                                        && f.hasta >= albaran.Fechadocumento).Select(f => f.id).SingleOrDefault();
            albaran.Tipoalbaran = (int)TipoAlbaran.EntradaRegularizacion;
            albaran.Fkmonedas = moneda;
            albaran.Fkformaspago = _db.Proveedores.Where(f => f.empresa == Empresa && f.fkcuentas == albaran.Fkproveedores).Select(f => f.fkformaspago).SingleOrDefault();
            albaran.Fkregimeniva = _db.Proveedores.Where(f => f.empresa == Empresa && f.fkcuentas == albaran.Fkproveedores).Select(f => f.fkregimeniva).SingleOrDefault();
            albaran.Fkcriteriosagrupacion = _db.Proveedores.Where(f => f.empresa == Empresa && f.fkcuentas == albaran.Fkproveedores).Select(f => f.fkcriteriosagrupacion).SingleOrDefault();
            albaran.Tipodealmacenlote = (TipoAlmacenlote)tipoLote;

            foreach (DataRow row in sorted.Rows)
            {

                // Si cambia el proveedor generamos un nuevo albaran
                // Si cambia la fecha generamos un nuevo albaran del mismo proveedor pero fecha diferente                
                if (albaran.Fkproveedores != row["Proveedor"].ToString() ||
                    albaran.Fechadocumento.Value.Year != DateTime.Parse(row["Fecha"].ToString()).Year)
                {
                    ListaAlbaranes.Add(albaran);
                    albaran = new FModel().GetModel<RecepcionesStockModel>(context);
                    albaran.Fkproveedores = row["Proveedor"].ToString();
                    albaran.Fechadocumento = Convert.ToDateTime("01/01/" + Convert.ToDateTime(row["Fecha"].ToString()).Year);
                    albaran.Fkseries = serie;
                    albaran.Fkejercicio = _db.Ejercicios.Where(f => f.empresa == Empresa && f.desde <= albaran.Fechadocumento
                                                                && f.hasta >= albaran.Fechadocumento).Select(f => f.id).SingleOrDefault();
                    albaran.Tipoalbaran = (int)TipoAlbaran.EntradaRegularizacion;
                    albaran.Fkmonedas = moneda;
                    albaran.Fkformaspago = _db.Proveedores.Where(f => f.empresa == Empresa && f.fkcuentas == albaran.Fkproveedores).Select(f => f.fkformaspago).SingleOrDefault();
                    albaran.Fkregimeniva = _db.Proveedores.Where(f => f.empresa == Empresa && f.fkcuentas == albaran.Fkproveedores).Select(f => f.fkregimeniva).SingleOrDefault();
                    albaran.Fkcriteriosagrupacion = _db.Proveedores.Where(f => f.empresa == Empresa && f.fkcuentas == albaran.Fkproveedores).Select(f => f.fkcriteriosagrupacion).SingleOrDefault();
                    albaran.Tipodealmacenlote = (TipoAlmacenlote)tipoLote;
                }

                AlbaranesComprasLinModel linea = new AlbaranesComprasLinModel();
                linea.Id = albaran.Lineas.Count + 1;
                linea.Fkarticulos = row["CodArticulo"].ToString();
                linea.Descripcion = row["Descripcion"].ToString();
                linea.Lote = row["Lote"].ToString();
                linea.Tabla = int.Parse(row["Tabla"].ToString());
                linea.Cantidad = double.Parse(row["Cantidad"].ToString());
                linea.Largo = double.Parse(row["Largo"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                linea.Ancho = double.Parse(row["Ancho"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                linea.Grueso = double.Parse(row["Grueso"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                linea.Fkunidades = row["UM"].ToString();
                linea.Metros = double.Parse(row["Metros"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                linea.Precio = double.Parse(row["Precio"].ToString().Replace('.', ','), CultureInfo.CreateSpecificCulture("es-ES"));
                linea.Importe = Math.Round((double)(linea.Metros * linea.Precio), 2);
                var grupoiva = _db.Articulos.Where(f => f.empresa == Empresa && f.id == linea.Fkarticulos).Select(f => f.fkgruposiva).SingleOrDefault();
                var grupo = _db.GruposIva.Include("GruposIvaLin").Where(f => f.empresa == Empresa && f.id == grupoiva).SingleOrDefault();
                linea.Fktiposiva = grupo.GruposIvaLin.Select(f => f.fktiposivasinrecargo).LastOrDefault();
                linea.Porcentajeiva = Convert.ToDouble(linea.Fktiposiva);

                albaran.Lineas.Add(linea);
            }

            //for (var i = 0; i < ListaAlbaranes.Count; i++)
            //{
            //    try
            //    {
            //        create(ListaAlbaranes[i]);
            //    }
            //    catch (Exception ex)
            //    {
            //        for (var j = 0; i < ListaAlbaranes[i].Lineas.Count; i++)
            //        {
            //            errores += ListaAlbaranes[i].Fkproveedores + ";" + ListaAlbaranes[i].Fechadocumento + ";" + ListaAlbaranes[i].Lineas[j].Fkarticulos +
            //                 ";" + ex.Message + Environment.NewLine;
            //        }
            //    }
            //}

            foreach (var itemAlbaran in ListaAlbaranes)
            {
                try
                {
                    create(itemAlbaran);
                }
                catch (Exception ex)
                {
                    errores += itemAlbaran.Fkproveedores + ";" + itemAlbaran.Fechadocumento + ";" + ex.Message + Environment.NewLine;
                }
            }

            var item = _db.PeticionesAsincronas.Where(f => f.empresa == context.Empresa && f.id == idPeticion).SingleOrDefault();

            item.estado = (int)EstadoPeticion.Finalizada;
            item.resultado = errores;

            _db.PeticionesAsincronas.AddOrUpdate(item);
            _db.SaveChanges();
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
            item.configuracion = (((int)TipoImportacion.ImportarStock).ToString() + "-").ToString();

            _db.PeticionesAsincronas.AddOrUpdate(item);
            _db.SaveChanges();

            return item.id;
        }

        #endregion
    }
}
