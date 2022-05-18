using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
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
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public enum TipoOperacionStock
    {

        Entrada,

        Salida,

        Actualizacion,

        MovimientoStock
    }

    public interface IAlbaranesComprasService
    {
        
    }

    public  class AlbaranesComprasService : GestionService<AlbaranesComprasModel, AlbaranesCompras>, IDocumentosServices, IAlbaranesComprasService
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

        public AlbaranesComprasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            _importarService = new ImportacionService(context);

        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService = new EstadosService(_context,_db);

            //Comprobamos si el usuario tiene el bloqueo de series
            List<string> seriesrol;
            var tienebloqueo = _db.Usuarios.Where(f => f.id == _context.Id).FirstOrDefault().bloquearseries;

            //Si tiene comprobamos el grupo de usuarios y a que series corresponden
            if (tienebloqueo == true)
            {
                //Comprobamos el rol de usuario para mostrar las series que le correspondan al usuario
                seriesrol = _db.Series.Where(f => f.empresa == _context.Empresa && (f.fkgruposusuarios == _context.RoleId.ToString() || f.fkgruposusuarios == null || f.fkgruposusuarios == "")).Select(x => x.id).ToList();
            }
            //Si no tiene bloqueo se ven todas las series
            else
            {
                seriesrol = _db.Series.Where(f => f.empresa == _context.Empresa).Select(x => x.id).ToList();
            }

            st.List = st.List.OfType<AlbaranesComprasModel>().Where(s => seriesrol.Contains(s.Fkseries)).OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkproveedores", "Nombreproveedor", "Fkestados", "Importebaseimponible", "Tipoalbaran" };
            var propiedades = Helpers.Helper.getProperties<AlbaranesComprasModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.AlbaranesCompras, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            st.ColumnasCombo.Add("Tipoalbaran", Enum.GetValues(typeof(TipoAlbaran)).OfType<TipoAlbaran>().Select(f => new Tuple<string, string>(((int)f).ToString(), Funciones.GetEnumByStringValueAttribute(f))));
            st.EstiloFilas.Add("Tipoalbaran", new EstiloFilas() { Estilos = new[] { new Tuple<object, string>(2, "#FCF8E3"), new Tuple<object, string>(3, "#F2DEDE") } });
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * from albaranescompras where empresa='{0}'{1}", Empresa, " and modo=" + (int)ModoAlbaran.Sinstock);
        }

        #endregion

        public IEnumerable<AlbaranesComprasLinModel> RecalculaLineas(IEnumerable<AlbaranesComprasLinModel> model,
            double descuentopp, double descuentocomercial, string fkregimeniva, double portes, int decimalesmoneda)
        {
            var result = new List<AlbaranesComprasLinModel>();

            foreach (var item in model)
            {
                if (item.Fkregimeniva != fkregimeniva)
                {
                    var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), _context) as TiposivaService;
                    var grupo = _db.Articulos.Single(f => f.empresa == Empresa && f.id == item.Fkarticulos);
                    if (!grupo.tipoivavariable)
                    {
                        var ivaObj = tiposivaService.GetTipoIva(grupo.fkgruposiva, fkregimeniva);
                        item.Fktiposiva = ivaObj.Id;
                        item.Porcentajeiva = ivaObj.PorcentajeIva;
                        item.Porcentajerecargoequivalencia = ivaObj.PorcentajeRecargoEquivalencia;
                        item.Fkregimeniva = fkregimeniva;
                    }
                    
                }

                result.Add(item);
            }

            return result;
        }

        public IEnumerable<AlbaranesComprasTotalesModel> Recalculartotales(IEnumerable<AlbaranesComprasLinModel> model, double descuentopp, double descuentocomercial, double portes, int decimalesmoneda)
        {
            var result = new List<AlbaranesComprasTotalesModel>();

            var vector = model.GroupBy(f => f.Fktiposiva);

            foreach (var item in vector)
            {
                var newItem = new AlbaranesComprasTotalesModel();
                var objIva = _db.TiposIva.Single(f => f.empresa == Empresa && f.id == item.Key);
                newItem.Decimalesmonedas = decimalesmoneda;
                newItem.Fktiposiva = item.Key;
                newItem.Porcentajeiva = objIva.porcentajeiva;
                
                newItem.Brutototal = Math.Round((item.Sum(f => (f.Metros) * (f.Precio)) - item.Sum(f => f.Importedescuento)) ?? 0, decimalesmoneda);
                newItem.Porcentajerecargoequivalencia = objIva.porcentajerecargoequivalente;
                newItem.Porcentajedescuentoprontopago = descuentopp;
                newItem.Porcentajedescuentocomercial = descuentocomercial;
                newItem.Importedescuentocomercial = Math.Round((double)((newItem.Brutototal * descuentocomercial ?? 0) / 100.0), decimalesmoneda);
                newItem.Importedescuentoprontopago = Math.Round((double)((double)(newItem.Brutototal - newItem.Importedescuentocomercial) * (descuentopp / 100.0)), decimalesmoneda);

                var baseimponible = (newItem.Brutototal ?? 0) - ((newItem.Importedescuentocomercial ?? 0) + (newItem.Importedescuentoprontopago ?? 0));
                newItem.Baseimponible = baseimponible;
                newItem.Cuotaiva = Math.Round(baseimponible * ((objIva.porcentajeiva ?? 0) / 100.0), decimalesmoneda);
                newItem.Importerecargoequivalencia = Math.Round(baseimponible * ((objIva.porcentajerecargoequivalente ?? 0) / 100.0), decimalesmoneda);
                newItem.Subtotal = Math.Round(baseimponible + (newItem.Cuotaiva ?? 0) + (newItem.Importerecargoequivalencia ?? 0), decimalesmoneda);
                result.Add(newItem);
            }

            return result;
        }

        public AlbaranesComprasModel Clonar(string id)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var obj = _converterModel.CreateView(id) as AlbaranesComprasModel;

                obj.Fechadocumento = DateTime.Now;
                obj.Fechavalidez = DateTime.Now.AddMonths(1);
                foreach (var AlbaranesComprasLinModel in obj.Lineas)
                {
                    AlbaranesComprasLinModel.Fkpedidos = null;
                    AlbaranesComprasLinModel.Fkpedidosid = null;
                    AlbaranesComprasLinModel.Fkpedidosreferencia = string.Empty;
                }
                foreach (var item in obj.Lineas)
                {
                    item.Cantidadpedida = 0;
                }
                obj.Fkestados = _appService.GetConfiguracion().Estadoalbaranescomprasinicial;
                var contador = ServiceHelper.GetNextId<AlbaranesCompras>(_db, Empresa, obj.Fkseries);
                var identificadorsegmento = "";
                obj.Referencia = ServiceHelper.GetReference<AlbaranesCompras>(_db, obj.Empresa, obj.Fkseries, contador, obj.Fechadocumento.Value, out identificadorsegmento);
                obj.Identificadorsegmento = identificadorsegmento;
                var newItem = _converterModel.CreatePersitance(obj);
                if (_validationService.ValidarGrabar(newItem))
                {
                    AlbaranesComprasModel result;
                    result = _converterModel.GetModelView(newItem) as AlbaranesComprasModel;
                    //generar carpeta
                    DocumentosHelpers.GenerarCarpetaAsociada(result, TipoDocumentos.AlbaranesCompras, _context, _db);
                    newItem.fkcarpetas = result.Fkcarpetas;
                    _db.Set<AlbaranesCompras>().Add(newItem);
                    try
                    {
                        _db.SaveChanges();

                        tran.Complete();
                        result.Id = newItem.id;
                    }
                    catch (DbUpdateException ex)
                    {
                        if (ex.InnerException != null
                            && ex.InnerException.InnerException != null)
                        {
                            var inner = ex.InnerException.InnerException as SqlException;
                            if (inner != null)
                            {
                                if (inner.Number == 2627 || inner.Number == 2601)
                                {
                                    throw new ValidationException(General.ErrorRegistroExistente);
                                }
                            }
                        }


                        throw;
                    }

                    return result;
                }
            }

            throw new ValidationException(General.ErrorClonarGeneral);
        }

        public AlbaranesComprasModel GetByReferencia(string referencia)
        {
            var obj =
                _db.AlbaranesCompras.Include("AlbaranesComprasLin")
                    .Include("AlbaranesComprasTotales")
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            ((AlbaranesComprasConverterService)_converterModel).Ejercicio = EjercicioId;
            return _converterModel.GetModelView(obj) as AlbaranesComprasModel;
        }

        public override IModelView get(string id)
        {
            ((AlbaranesComprasConverterService)_converterModel).Ejercicio = EjercicioId;
            return base.get(id);
        }

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
                            result.Add(new LineaImportarModel()
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

                sb.Append("(select top 1 al.descripcion) as Descripcion, Min(al.Id) as Id,Sum(al.cantidad) as Cantidad,sum(al.Metros) as Metros,al.Porcentajeiva,Sum(al.Cuotaiva) as Cuotaiva," +
                          " al.Porcentajerecargoequivalencia,sum(al.Cuotarecargoequivalencia) as Cuotarecargoequivalencia,'" + albaran.Fkregimeniva + "' as Fkregimeniva," +
                          " al.Fkunidades, al.Fktiposiva,Sum(al.Importe) as Importe,Sum(al.importedescuento) as Importedescuento,al.Decimalesmedidas,al.Decimalesmonedas, " + string.Join(",", lineas.Select(f => "al." + f.Campoenum.ToString())));

                //sb.Append("(select top 1 art.descripcion from articulos as art where art.id=al.fkarticulos and art.empresa=al.empresa) as Descripcion,Min(al.Id) as Id,Sum(al.cantidad) as Cantidad,sum(al.Metros) as Metros,al.Porcentajeiva,Sum(al.Cuotaiva) as Cuotaiva," +
                          //" al.Porcentajerecargoequivalencia,sum(al.Cuotarecargoequivalencia) as Cuotarecargoequivalencia,'" + albaran.Fkregimeniva + "' as Fkregimeniva," +
                          //" al.Fkunidades, al.Fktiposiva,Sum(al.Importe) as Importe,Sum(al.importedescuento) as Importedescuento,al.Decimalesmedidas,al.Decimalesmonedas, " + string.Join(",", lineas.Select(f => "al." + f.Campoenum.ToString())));

                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Fkarticulos))
                    sb.Append(", al.Fkarticulos");
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Precio))
                    sb.Append(", al.Precio");
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Porcentajedescuento))
                    sb.Append(", al.Porcentajedescuento");
                if (!lineas.Any(f => f.Campoenum == CamposAgrupacionAlbaran.Descripcion))
                    sb.Append(", (select top 1 al.descripcion) as Descripcion");
                    //sb.Append(", al.Descripcion");


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

        public bool ExisteReferencia(string referencia)
        {
            return _db.AlbaranesCompras.Any(f => f.empresa == _context.Empresa && f.referencia == referencia);
        }

        #endregion

        #region Api main

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as AlbaranesComprasModel;
                var validation = _validationService as AlbaranesComprasValidation;
                validation.EjercicioId = EjercicioId;

                //Calculo ID
                var contador = ServiceHelper.GetNextId<AlbaranesCompras>(_db, Empresa, model.Fkseries);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<AlbaranesCompras>(_db, model.Empresa, model.Fkseries, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;
                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.AlbaranesCompras, _context, _db);

                //Actualizar precios si estos son = 0
                if (!String.IsNullOrWhiteSpace(model.Fkpedidoscompras))
                {
                    foreach (var lineaAlbaranCompra in model.Lineas)
                    {
                        if (lineaAlbaranCompra.Precio == 0)
                        {
                            var idPedidoCompra = _db.PedidosCompras.Where(f => f.empresa == model.Empresa && f.referencia == model.Fkpedidoscompras).Select(f => f.id).SingleOrDefault();
                            var lineasPedidosCompra = _db.PedidosComprasLin.Where(f => f.empresa == model.Empresa && f.fkpedidoscompras == idPedidoCompra);

                            foreach (var lineaPedido in lineasPedidosCompra)
                            {
                                if (lineaAlbaranCompra.Fkarticulos == lineaPedido.fkarticulos)
                                {
                                    lineaAlbaranCompra.Precio = lineaPedido.precio;
                                    lineaAlbaranCompra.Importe = Math.Round((double)((lineaAlbaranCompra.Metros * lineaAlbaranCompra.Precio) - (lineaAlbaranCompra.Metros * lineaAlbaranCompra.Precio
                                        * (lineaAlbaranCompra.Porcentajedescuento / 100))), 2);
                                }
                            }
                        }
                    }
                }
                //fin actualizar costes

                base.create(obj);

                ModificarCantidadesPedidasPedidos(obj as AlbaranesComprasModel);
                ModificarMovimientosArticulos(obj as AlbaranesComprasModel);

                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as AlbaranesComprasModel;
                var editado = obj as AlbaranesComprasModel;

                if (original.Integridadreferencialflag == editado.Integridadreferencialflag)
                {
                    if (original.Fkzonas == editado.Fkzonas)
                    { 
                        editado.Fkzonas = string.Empty;
                        original.Fkzonas = string.Empty;
                    }
                    var validation = _validationService as AlbaranesComprasValidation;
                    validation.EjercicioId = EjercicioId;
                    DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.AlbaranesCompras, _context, _db);

                    base.edit(obj);

                    ModificarCantidadesPedidasPedidos(obj as AlbaranesComprasModel);
                    
                    _db.SaveChanges();
                    tran.Complete();
                }
                else throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RAlbaranesCompras.TituloEntidad, original.Referencia));

            }

        }

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                base.delete(obj);
                _db.SaveChanges();
                ModificarCantidadesPedidasPedidos(obj as AlbaranesComprasModel);

                tran.Complete();
            }

        }

        #endregion

        #region ModificarMovimientosArticulos

        private void ModificarMovimientosArticulos(AlbaranesComprasModel model)
        {
            List<string> listaCodArticulos = new List<string>();

            foreach (var linea in model.Lineas)
            {
                if (string.IsNullOrEmpty(listaCodArticulos.Where(f => f.Contains(linea.Fkarticulos)).SingleOrDefault()))
                {
                    listaCodArticulos.Add(linea.Fkarticulos);
                }
            }

            var service = FService.Instance.GetService(typeof(ArticulosModel), _context) as ArticulosService;

            for (var i = 0; i < listaCodArticulos.Count; i++)
            {
                var articulo = service.GetArticulo(listaCodArticulos[i]);
                if (articulo.Fechaultimaentrada < model.Fechadocumento || articulo.Fechaultimaentrada == null)
                {
                    articulo.Ultimaentrada = model.Referencia;
                    articulo.Fechaultimaentrada = model.Fechadocumento;
                    service.edit(articulo);
                }
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
        
        #region Importar Pedidos 

        public virtual AlbaranesComprasModel ImportarPedido(PedidosComprasModel presupuesto)
        {
            //calcular serie asociada
            if (presupuesto.Lineas.Any(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0) > 0))
            {
                var result = Helper.fModel.GetModel<AlbaranesComprasModel>(_context);
                result.Importado = true;
                ImportarCabecera(presupuesto, result);
                var maxId = result.Lineas.Any() ? result.Lineas.Max(f => f.Id) : 0;
                result.Lineas.AddRange(ImportarLineas(maxId, ConvertLineasModelToILineas(presupuesto.Id.ToString(), presupuesto.Referencia, presupuesto.Lineas)));
                EstablecerSerie(presupuesto.Fkseries, result);

                //recalculo importes lineas y totales
                RecalculaLineas(result.Lineas, result.Porcentajedescuentoprontopago ?? 0, result.Porcentajedescuentocomercial ?? 0, result.Fkregimeniva, result.Importeportes ?? 0, result.Decimalesmonedas);
                result.Totales = Recalculartotales(result.Lineas, result.Porcentajedescuentoprontopago ?? 0,
                    result.Porcentajedescuentocomercial ?? 0, result.Importeportes ?? 0,
                    result.Decimalesmonedas).ToList();

                return result;
            }

            throw new ValidationException(RAlbaranesCompras.ErrorSinCantidadPendiente);
        }

        private IEnumerable<ILineaImportar> ConvertLineasModelToILineas(string idcabecera, string referencia, IEnumerable<PedidosComprasLinModel> lineas)
        {
            var id = lineas.Any() ? lineas.Max(f => f.Id) : 0;
            id++;

            return lineas.Where(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0) != 0).Select(f => new LineaImportarModel()
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

        protected void EstablecerSerie(string fkseries, AlbaranesComprasModel result)
        {
            var service = FService.Instance.GetService(typeof(SeriesModel), _context);

            var serieObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.PedidosCompras), fkseries)) as SeriesModel;
            var serieAsociada = serieObj.Fkseriesasociada;
            if (!string.IsNullOrEmpty(serieAsociada))
            {
                var serieasociadaObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.AlbaranesCompras), serieAsociada)) as SeriesModel;
                result.Fkseries = serieasociadaObj.Id;
            }
            else
                throw new ValidationException(Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Presupuestos.ErrorNoExisteSerieAsociada);
        }

        public IEnumerable<AlbaranesComprasModel> GetAlbaranesImportar()
        {
            //Albaranes de compra entradas varias, de este mes, que no estén finalizados y no tengan ya líneas
            var primerDía = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var list = new List<AlbaranesComprasModel>();
            if (_db.Series.Where(f => f.empresa == Empresa && f.tipodocumento == "ALC" && f.entradasvarias == true).FirstOrDefault() != null)
            {
                var serieEstradasVarias = _db.Series.Where(f => f.empresa == Empresa && f.tipodocumento == "ALC" && f.entradasvarias == true).FirstOrDefault().id;
                list = _db.AlbaranesCompras.Where(
                          f =>
                              f.empresa == Empresa && f.fkseries == serieEstradasVarias && f.fechadocumento >= primerDía && f.fechadocumento <= DateTime.Now
                              && f.fkestados != "99-003" && f.AlbaranesComprasLin.Count == 0
                          ).ToList().Select(f => _converterModel.GetModelView(f) as AlbaranesComprasModel).ToList();
            }
           
            return list;
        }

        public IEnumerable<AlbaranesComprasLinModel> ImportarLineas(int maxId, IEnumerable<ILineaImportar> linea)
        {
            return linea.Where(f => f.Cantidad != 0).Select(f => _importarService.ImportarLinea(f)).Select(f => ConvertILineaImportarToPedidoLinModel(++maxId, f));
        }

        private AlbaranesComprasLinModel ConvertILineaImportarToPedidoLinModel(int Id, ILineaImportar linea)
        {
            var result = _importarService.ImportarLinea(linea);
            return new AlbaranesComprasLinModel()
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
                if (item.Name != "Lineas" && item.Name != "Totales" && item.Name!="Fkalmacen")
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
            result.Fkestados = _appService.GetConfiguracion().Estadoalbaranescomprasinicial;
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

        public IEnumerable<AlbaranesComprasModel> BuscarAlbaranesComprasImportar(string serie)
        {
            var primerDía = new DateTime(DateTime.Now.Year,1,1);
            var list = _db.AlbaranesCompras.Where(
                      f =>
                          f.empresa == Empresa && f.fkseries == serie && f.fechadocumento >= primerDía && f.fechadocumento <= DateTime.Now
                      ).ToList().Select(f => _converterModel.GetModelView(f) as AlbaranesComprasModel).ToList();

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
                : new BundleModel()
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
                        !bundleObj.Lineas.Any(f => f.Lote == j.Lote && f.Loteid == (j.Tabla?.ToString() ?? "0"))).Select(f => new BundleLinModel()
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
                            objlote = new StLote() { Lote = contadorlotesService.CreateLoteId(loteObj, ref incremento), Numero = 0 };
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
        }

        private void ModificarCantidadesPedidasPedidos(AlbaranesComprasModel model)
        {
            var PedidosService = new PedidosComprasService(_context,_db);
            PedidosService.EjercicioId = EjercicioId;
            var vector = model.Lineas.Where(f => f.Fkpedidos.HasValue);

            foreach (var item in vector)
            {
                var pedido = _db.PedidosCompras.Include("PedidosComprasLin").Single(
                    f =>
                        f.empresa == model.Empresa && f.id == item.Fkpedidos.Value);


                var cantidadpedida = _db.AlbaranesComprasLin.Where(f => f.empresa == model.Empresa && f.fkpedidos == item.Fkpedidos &&
                        f.fkpedidosid == item.Fkpedidosid).Sum(f => f.cantidad);

                var linea = pedido.PedidosComprasLin.SingleOrDefault(f => f.id == item.Fkpedidosid);
                if (linea != null)
                {
                    linea.cantidadpedida = cantidadpedida;
                    var validationService = PedidosService._validationService as PedidosComprasValidation;
                    validationService.EjercicioId = EjercicioId;
                    validationService.FlagActualizarCantidadesPedidas = true;
                    validationService.ValidarGrabar(pedido);
                    _db.PedidosCompras.AddOrUpdate(pedido);
                }

            }
            _db.SaveChanges();
        }

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
            AlbaranesComprasGesionLotesService.GestionarLote(articuloObj, familiaObj, lineas.Select(f=>(IDocumentosLinModel)f), linea, out lote, out loteautomaticoid, out lotenuevocontador, out tabla);

            var metros = UnidadesService.CalculaResultado(unidadesObj, incremento, linea.Largo, linea.Ancho,
               linea.Grueso, linea.Metros);



            for (var i = 0; i < linea.Cantidad; i += incremento)
            {
                linea.Metros = metros;
                var bruto = linea.Metros * linea.Precio;
                var importedescuento = Math.Round(bruto * linea.Descuento / 100.0, linea.Decimalesmonedas);
                var total = bruto - importedescuento;

                result.Add(new AlbaranesComprasLinModel()
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
                    Fkunidades = linea.Fkunidades.ToString(),
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
                    Bundle = linea.Bundle?.ToUpper()
                }
                 );

                if (tabla.HasValue)
                    tabla++;
            }

            return result;

        }

        #endregion

        #region Proceso de remedir

        public void RemedirLotes(StRemedir model)
        {
            var vector = model.Lotesreferencia.Split(';');
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var serviceMovimientostock = new MovimientosstockService(_context,_db);
                var stockactualService = new StockactualService(_context,_db);
                foreach (var item in vector)
                {
                    var vectorids = item.Split('|');
                    RemedirLote(serviceMovimientostock, stockactualService, vectorids[0], vectorids[1], vectorids[2], vectorids[3],
                        model.Nuevolargo,
                        model.Nuevoancho,
                        model.Nuevogrueso,
                        model.Sumarlargo,
                        model.Sumarancho,
                        model.Sumargrueso,
                        model.Loteproveedor,
                        model.Fkincidenciasmaterial,
                        model.Zona,
                        model.Fkcalificacioncomercial,
                        model.Fktipograno,
                        model.Fktonomaterial,
                        model.Fkvariedades,
                        model.Pesopieza);
                }

                tran.Complete();
            }

        }

        private void RemedirLote(MovimientosstockService servicemovimiento, StockactualService servicestock, string fkalmacen, string fkarticulos, string lote, string loteid, double? nuevolargo, double? nuevoancho, double? nuevogrueso, bool? sumarlargo, bool? sumarancho, bool? sumargrueso, string loteproveedor, string fkincidenciasmaterial, string zona, string fkcalificacioncomercial, string fktipograno, string fktonomaterial, string fkvariedades,double? pesopieza)
        {
            var loterefencia = string.Format("{0}{1}", lote, Funciones.RellenaCod(loteid, 3));
            var pieza = servicestock.GetArticuloPorLoteOCodigo(loterefencia, fkalmacen, Empresa) as MovimientosstockModel;

            pieza.Tipomovimiento = TipoOperacionService.MovimientoRemedir;


            if (nuevolargo.HasValue)
            {
                if (sumarlargo ?? false)
                {
                    pieza.Largo += nuevolargo.Value / 100.0;
                }
                else
                {
                    pieza.Largo = nuevolargo.Value;
                }
            }

            if (nuevoancho.HasValue)
            {
                if (sumarancho ?? false)
                {
                    pieza.Ancho += nuevoancho.Value / 100.0;
                }
                else
                {
                    pieza.Ancho = nuevoancho.Value;
                }
            }

            if (nuevogrueso.HasValue)
            {
                if (sumargrueso ?? false)
                {
                    pieza.Grueso += nuevogrueso.Value / 100.0;
                }
                else
                {
                    pieza.Grueso = nuevogrueso.Value;
                }
            }

            if (pieza.Largo < 0)
                throw new ValidationException("Largo debe ser mayor que 0");
            if (pieza.Ancho < 0)
                throw new ValidationException("Ancho debe ser mayor que 0");
            if (pieza.Grueso < 0)
                throw new ValidationException("Grueso debe ser mayor que 0");

            if (!string.IsNullOrEmpty(loteproveedor))
                pieza.Referenciaproveedor = loteproveedor;

            if (!string.IsNullOrEmpty(zona))
                pieza.Fkalmaceneszona = Funciones.Qint(zona);

            if (!string.IsNullOrEmpty(fkcalificacioncomercial))
                pieza.Fkcalificacioncomercial = fkcalificacioncomercial;

            if (!string.IsNullOrEmpty(fkincidenciasmaterial))
                pieza.Fkincidenciasmaterial = fkincidenciasmaterial;

            if (!string.IsNullOrEmpty(fktipograno))
                pieza.Fktipograno = fktipograno;

            if (!string.IsNullOrEmpty(fktonomaterial))
                pieza.Fktonomaterial = fktonomaterial;

            if (!string.IsNullOrEmpty(fkvariedades))
                pieza.Fkvariedades = fkvariedades;

            pieza.Pesoneto = null;
            if (pesopieza.HasValue)
                pieza.Pesoneto = pesopieza;

            servicemovimiento.GenerarMovimiento(pieza, TipoOperacionService.MovimientoRemedir);

        }


        public AlbaranesComprasModel Devolucion(string id, List<LineaImportarModel> vector)
        {
            if (vector != null && vector.Count > 0)
            {
                using (var service = FService.Instance.GetService(typeof(AlbaranesComprasModel), _context) as AlbaranesComprasService)
                {
                    var model = service.get(id) as AlbaranesComprasModel;
                    model.Id = 0;
                    model.Modo = ModoAlbaran.Sinstock;
                    model.Tipoalbaran = (int)TipoAlbaran.Devolucion;
                    model.Fechadocumento = DateTime.Now;
                    //model.Fkmotivosdevolucion = appService.GetListMotivosDevolucion().FirstOrDefault()?.Valor;
                   

                    var maxId = model.Lineas.Any() ? model.Lineas.Max(f => f.Id) : 0;
                    model.Lineas.Clear();
                    
                    foreach (var item in vector)
                        item.Cantidad *= -1;
                    model.Lineas.AddRange(ImportarLineas(maxId, vector));
                    var newmodel = Helper.fModel.GetModel<AlbaranesComprasModel>(_context);
                    model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
                    model.Totales = service.Recalculartotales(model.Lineas, model.Porcentajedescuentoprontopago ?? 0, model.Porcentajedescuentocomercial ?? 0, 0, model.Decimalesmonedas).ToList();

                    return model;
                }
            }
            return null;

        }

        #endregion

        #region Anterior/Siguiente

        public ModoAlbaran Modo = ModoAlbaran.Sinstock;

        public override string LastRegister()
        {
            var keyNames = GetprimarykeyColumns();
            var enumerable = keyNames as string[] ?? keyNames.ToArray();
            var modoint = (int)Modo;
            var query = _db.AlbaranesCompras.Where(f => f.empresa == Empresa && f.modo == modoint);

            var flagFirst = true;
            IOrderedQueryable<AlbaranesCompras> orderedQuery = null;
            foreach (var item in enumerable)
            {

                orderedQuery = flagFirst ? query.OrderByDescending(item) : orderedQuery.ThenByDescending(item);
                flagFirst = false;

            }
            var obj = orderedQuery.FirstOrDefault();
            if (obj == null) return string.Empty;

            var modelObj = _converterModel.GetModelView(obj);
            return modelObj.GetPrimaryKey();
        }

        protected override string GetRegister(string id, TipoSelect tipo)
        {
            using (var con = new SqlConnection(_db.Database.Connection.ConnectionString))
            {
                var keyNames = GetprimarykeyColumns().ToArray();
                using (var cmd = new SqlCommand(GetSelect(keyNames, tipo), con))
                {
                    cmd.Parameters.AddWithValue("empresa", Empresa);
                    cmd.Parameters.AddWithValue("modo", (int)Modo);
                    var pkColumns = keyNames.Count(c => c != "empresa");
                    var pkvector = pkColumns > 1 ? id.Split(SeparatorPk) : new[] { id };
                    var j = 0;
                    foreach (var item in keyNames.Where(item => item != "empresa"))
                    {
                        cmd.Parameters.AddWithValue(item, pkvector[j++]);
                    }
                    var tabla = new DataTable();
                    using (var ad = new SqlDataAdapter(cmd))
                    {

                        ad.Fill(tabla);
                        if (tabla.Rows.Count > 0)
                        {
                            var vector = GetKeys(keyNames, tabla.Rows[0]);
                            var obj = _db.Set<AlbaranesCompras>().Find(vector);
                            return _converterModel.GetModelView(obj).GetPrimaryKey();
                        }
                    }
                }

            }

            return string.Empty;
        }

        protected override string GetSelect(string[] keyNames, TipoSelect tipo)
        {
            var sb = new StringBuilder();

            sb.AppendFormat("select top 1 * from AlbaranesCompras Where modo={0} ", (int)Modo);
            var flagFirst = false;
            for (var i = 0; i < keyNames.Count(); i++)
            {
                if (!flagFirst)
                    sb.Append(" AND ");

                if (i == keyNames.Count() - 1)
                    sb.AppendFormat("{0}{1}@{0}", keyNames[i], tipo == TipoSelect.Next ? ">" : "<");
                else
                    sb.AppendFormat("{0}=@{0}", keyNames[i]);
            }

            if (tipo == TipoSelect.Previous)
                sb.AppendFormat(" order by {0} desc", string.Join(",", keyNames));

            return sb.ToString();
        }

        public override string FirstRegister()
        {
            var modoint = (int)Modo;
            var obj = _db.Set<AlbaranesCompras>().FirstOrDefault(f => f.empresa == Empresa && f.modo == modoint);
            if (obj == null) return string.Empty;

            var modelObj = _converterModel.GetModelView(obj);
            return modelObj.GetPrimaryKey();
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

    }
}
