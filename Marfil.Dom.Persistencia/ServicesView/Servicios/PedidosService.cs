using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.BusquedasMovil;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RPresupuestos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Presupuestos;
using RProspectos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Prospectos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    internal enum TipoMovimiento
    {
        Entrada,
        Salida
    }

    public interface IPedidosService
    {

    }

    public class PedidosService : GestionService<PedidosModel, Pedidos>, IDocumentosServices,IBuscarDocumento, IDocumentosVentasPorReferencia<PedidosModel>, IPedidosService
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
                ((PedidosValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion
       
        #region CTR

        public PedidosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            _importarService = new ImportacionService(context);
        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService = new EstadosService(_context,_db);
            //var user = _db.Usuarios.Where(f => f.usuario == _context.Usuario).SingleOrDefault();
            //st.List = user.usuario_cliente == true ? st.List.OfType<PedidosModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia).Where(f => f.Fkclientes == user.codigoclienteusuario)
            //    : st.List.OfType<PedidosModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);

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

            st.List = st.List.OfType<PedidosModel>().Where(s => seriesrol.Contains(s.Fkseries)).OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkclientes", "Nombrecliente", "Fkestados", "Importebaseimponible" };
            var propiedades = Helpers.Helper.getProperties<PedidosModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.PedidosVentas, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            return st;
        }

        public IEnumerable<PedidosLinModel> RecalculaLineas(IEnumerable<PedidosLinModel> model,
            double descuentopp, double descuentocomercial, string fkregimeniva, double portes, int decimalesmoneda)
        {
            var result = new List<PedidosLinModel>();

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

        public IEnumerable<PedidosTotalesModel> Recalculartotales(IEnumerable<PedidosLinModel> model, double descuentopp, double descuentocomercial, double portes, int decimalesmoneda, bool esimportado)
        {
            var result = new List<PedidosTotalesModel>();

            var vector = model.GroupBy(f => f.Fktiposiva);

            foreach (var item in vector)
            {
                var newItem = new PedidosTotalesModel();
                var objIva = _db.TiposIva.Single(f => f.empresa == Empresa && f.id == item.Key);
                newItem.Decimalesmonedas = decimalesmoneda;
                newItem.Fktiposiva = item.Key;
                newItem.Porcentajeiva = objIva.porcentajeiva;
                newItem.Brutototal = Math.Round((item.Sum(f => f.Importe)) ?? 0, decimalesmoneda) ;
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

        public PedidosModel Clonar(string id)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var obj = _converterModel.CreateView(id) as PedidosModel;

                obj.Fechadocumento = DateTime.Now;
                obj.Fechavalidez = DateTime.Now.AddMonths(1);
                foreach (var pedidosLinModel in obj.Lineas)
                {
                    pedidosLinModel.Fkpresupuestos = null;
                    pedidosLinModel.Fkpresupuestosid = null;
                    pedidosLinModel.Fkpresupuestosreferencia = "";
                }
                foreach (var item in obj.Lineas)
                {
                    item.Cantidadpedida = 0;
                }
                var appService=new ApplicationHelper(_context);
                obj.Fkestados = appService.GetConfiguracion().Estadopedidosventasinicial;
                var tipodocumento = "PED"; //Pedido Venta
                var contador = ServiceHelper.GetNextId<Pedidos>(_db, Empresa, obj.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                obj.Referencia = ServiceHelper.GetReference<Pedidos>(_db, obj.Empresa, obj.Fkseries, tipodocumento, contador, obj.Fechadocumento.Value, out identificadorsegmento);
                obj.Identificadorsegmento = identificadorsegmento;

                var newItem = _converterModel.CreatePersitance(obj);
                if (_validationService.ValidarGrabar(newItem))
                {
                    PedidosModel result;
                    result = _converterModel.GetModelView(newItem) as PedidosModel;
                    DocumentosHelpers.GenerarCarpetaAsociada(result, TipoDocumentos.PedidosVentas, _context, _db);
                    newItem.fkcarpetas = result.Fkcarpetas;
                    _db.Set<Pedidos>().Add(newItem);
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

        public PedidosModel GetByReferencia(string referencia)
        {
            var obj =
                _db.Pedidos.Include("PedidosLin")
                    .Include("PedidosTotales")                    
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            return _converterModel.GetModelView(obj) as PedidosModel;
        }

        public bool ExistsByReferencia(string referencia)
        {
            return _db.Pedidos.Any(f => f.empresa == Empresa && f.referencia == referencia);
        }

        public override bool exists(string id)
        {
            var intid = int.Parse(id);
            return _db.Pedidos.Any(f => f.empresa == Empresa && f.id == intid);
        }

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as PedidosModel;
                var validation = _validationService as PedidosValidation;
                validation.EjercicioId = EjercicioId;
                //Calculo ID
                var tipodocumento = "PED"; //Pedido Venta
                var contador = ServiceHelper.GetNextId<Pedidos>(_db, Empresa, model.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<Pedidos>(_db, model.Empresa, model.Fkseries, tipodocumento, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                //Obtener el pais del cliente (tercero)
                var cuentasService = new CuentasService(_context);
                var cuenta = cuentasService.get(model.Fkclientes) as CuentasModel;

                using (var service = new TablasVariasService(_context, MarfilEntities.ConnectToSqlServer(_context.BaseDatos)))
                {
                    model.Clientepais = service.GetListPaises().Where(f => f.Valor == cuenta.FkPais).Select(f => f.Descripcion).SingleOrDefault();
                }

                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.PedidosVentas, _context, _db);

                base.create(model);

                ModificarCantidadesPedidasPresupuestos(obj as PedidosModel);

                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var validation = _validationService as PedidosValidation;
                validation.EjercicioId = EjercicioId;
                DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.PedidosVentas, _context, _db);
                base.edit(obj);


                ModificarCantidadesPedidasPresupuestos(obj as PedidosModel);
                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                base.delete(obj);
                _db.SaveChanges();
                ModificarCantidadesPedidasPresupuestos(obj as PedidosModel);
                tran.Complete();
            }

        }

        #region Importar presupuestos 

        public PedidosModel ImportarPresupuesto(PresupuestosModel presupuesto)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                //calcular serie asociada
                if (presupuesto.Lineas.Any(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0) > 0))
                {
                    var prospectoService = FService.Instance.GetService(typeof(ProspectosModel), _context);
                    if (prospectoService.exists(presupuesto.Fkclientes))
                    {
                        throw new Exception(RProspectos.ErrorCrearPedidoProspecto);
                    }

                    var result = Helper.fModel.GetModel<PedidosModel>(_context);
                    result.Importado = true;
                    ImportarCabecera(presupuesto, result);
                    var maxId = result.Lineas.Any() ? result.Lineas.Max(f => f.Id) : 0;
                    result.Lineas.AddRange(ImportarLineas(maxId, ConvertLineasModelToILineas(presupuesto.Id.Value.ToString(), presupuesto.Referencia, presupuesto.Lineas)));
                    EstablecerSerie(presupuesto.Fkseries, result);

                    //recalculo importes lineas y totales
                    RecalculaLineas(result.Lineas, result.Porcentajedescuentoprontopago ?? 0, result.Porcentajedescuentocomercial ?? 0, result.Fkregimeniva, result.Importeportes ?? 0, result.Decimalesmonedas);
                    result.Totales = Recalculartotales(result.Lineas, result.Porcentajedescuentoprontopago ?? 0,
                        result.Porcentajedescuentocomercial ?? 0, result.Importeportes ?? 0,
                        result.Decimalesmonedas, (bool)result.Importado).ToList();

                    //Cambiamos el estado del presupuesto
                    var Confservice = FService.Instance.GetService(typeof(ConfiguracionModel), _context) as ConfiguracionService;
                    var presupuestosService = FService.Instance.GetService(typeof(PresupuestosModel), _context) as PresupuestosService;
                    presupuesto.Fkestados = Confservice.GetEstadoFinPresupuestoVentas();

                    var newItem = presupuestosService._converterModel.CreatePersitance(presupuesto);
                    newItem.id = (int)presupuesto.Id; //Que tome el id del presupuesto que modificamos para que no cree uno nuevo
                    _db.Set<Presupuestos>().AddOrUpdate(newItem);

                    _db.SaveChanges();
                    tran.Complete();

                    return result;
                }
            }

            throw new ValidationException(RPresupuestos.ErrorSinCantidadPendiente);
        }

        private IEnumerable<ILineaImportar> ConvertLineasModelToILineas(string idcabecera, string referencia, IEnumerable<PresupuestosLinModel> lineas)
        {
            var id = lineas.Any() ? lineas.Max(f => f.Id) : 0;
            id++;
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var vectorArticulos = new Hashtable();
            var result = new List<LineaImportarModel>();
            foreach (var f in lineas)
            {
                ArticulosModel articulo;
                if (!vectorArticulos.ContainsKey(f.Fkarticulos))
                {
                    vectorArticulos.Add(f.Fkarticulos, articulosService.get(f.Fkarticulos) as ArticulosModel);
                }

                articulo = vectorArticulos[f.Fkarticulos] as ArticulosModel;
                if ((f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0) != 0 || articulo.Articulocomentariovista)
                {
                    result.Add(new LineaImportarModel()
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
                        Articulocomentario = articulo.Articulocomentariovista,
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
                        Fkdocumentoreferencia = referencia,
                        Orden = f.Orden
                    });
                }
            }

            return result;
        }

        private void EstablecerSerie(string fkseries, PedidosModel result)
        {
            var service = FService.Instance.GetService(typeof(SeriesModel), _context);

            var serieObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.PresupuestosVentas), fkseries)) as SeriesModel;
            var serieAsociada = serieObj.Fkseriesasociada;
            if (!string.IsNullOrEmpty(serieAsociada))
            {
                var serieasociadaObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.PedidosVentas), serieAsociada)) as SeriesModel;
                result.Fkseries = serieasociadaObj.Id;
            }
            else
                throw new ValidationException(Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Presupuestos.ErrorNoExisteSerieAsociada);
        }

        public IEnumerable<PedidosLinModel> ImportarLineas(int maxId, IEnumerable<ILineaImportar> linea)
        {
            return linea.Where(f => f.Cantidad != 0 ||f.Articulocomentario).OrderBy(f => f.Orden).Select(f => _importarService.ImportarLinea(f)).Select(f => ConvertILineaImportarToPedidoLinModel(++maxId, f));
        }

        private PedidosLinModel ConvertILineaImportarToPedidoLinModel(int Id, ILineaImportar linea)
        {
            var result = _importarService.ImportarLinea(linea);
            return new PedidosLinModel()
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
                Fkpresupuestos = Funciones.Qint(result.Fkdocumento),
                Fkpresupuestosid = Funciones.Qint(result.Fkdocumentoid),
                Fkpresupuestosreferencia = result.Fkdocumentoreferencia,
                Orden = result.Orden ?? 0
            };

        }


        private void ImportarCabecera(PresupuestosModel presupuesto, PedidosModel result)
        {
            var properties = typeof(PedidosModel).GetProperties();

            foreach (var item in properties)
            {
                if (item.Name != "Lineas" && item.Name != "Totales")
                {
                    var property = typeof(PresupuestosModel).GetProperty(item.Name);
                    if (property != null && property.CanWrite)
                    {

                        var value = property.GetValue(presupuesto);
                        item.SetValue(result, value);
                    }
                }
            }

            result.Fechadocumento = DateTime.Now;
            var appService=new ApplicationHelper(_context);
            result.Fkestados = appService.GetConfiguracion().Estadopedidosventasinicial;
        }

        #endregion

        #region Helper

        private void ModificarCantidadesPedidasPresupuestos(PedidosModel model)
        {
            var presupuestosService = new PresupuestosService(_context,_db);
            presupuestosService.EjercicioId = EjercicioId;
            var vector = model.Lineas.Where(f => f.Fkpresupuestos.HasValue);

            foreach (var item in vector)
            {
                var presupuesto = _db.Presupuestos.Include("PresupuestosLin").Single(
                    f =>
                        f.empresa == model.Empresa && f.id == item.Fkpresupuestos);

                var cantidadpedida = _db.PedidosLin.Where(
                    f =>
                        f.empresa == model.Empresa && f.fkpresupuestos == item.Fkpresupuestos &&
                        f.fkpresupuestosid == item.Fkpresupuestosid).Sum(f => f.cantidad);

                var linea = presupuesto.PresupuestosLin.SingleOrDefault(f => f.id == item.Fkpresupuestosid);
                if (linea != null)
                {
                    linea.cantidadpedida = cantidadpedida;
                    var validationService = presupuestosService._validationService as PresupuestosValidation;
                    validationService.EjercicioId = EjercicioId;
                    validationService.FlagActualizarCantidadesPedidas = true;
                    validationService.ValidarGrabar(presupuesto);
                    _db.Presupuestos.AddOrUpdate(presupuesto);
                }

            }
            _db.SaveChanges();
        }

        #endregion

        #region ModificarCostes
        public void ModificarCostes(PedidosModel model)
        {
            var currentValidationService = _validationService as PedidosValidation;
            currentValidationService.ModificarCostes = true;
            edit(model);
            currentValidationService.ModificarCostes = false;
        }
        #endregion

        public IEnumerable<PedidosModel> BuscarPedidosImportar(string cliente)
        {

            var list = _db.Pedidos.Include("PedidosLin")
                  .Where(
                      f =>
                          f.empresa == Empresa && f.fkclientes == cliente &&
                          f.PedidosLin.Any(j => ((j.cantidad ?? 0) - (j.cantidadpedida ?? 0) > 0))).Join(_db.Estados, p => p.fkestados, e => e.documento + "-" + e.id, (a, b) => new { a, b }).ToList();
            return list.Where(f => f.b.tipoestado <= (int)TipoEstado.Curso).Select(f => _converterModel.GetModelView(f.a) as PedidosModel).OrderByDescending(f => f.Id);
        }

        public IEnumerable<PedidosLinImportarModel> BuscarLineasPedidos(string cliente, string presupuestodesde, string presupuestohasta)
        {

            return _db.PedidosLin.Include("Pedidos").Where(f => f.empresa == Empresa && f.Pedidos.fkclientes == cliente && (string.IsNullOrEmpty(presupuestodesde) || (string.Compare(f.Pedidos.referencia, presupuestodesde, StringComparison.Ordinal) >= 0)) &&
                                                                         (string.IsNullOrEmpty(presupuestohasta) || (string.Compare(f.Pedidos.referencia, presupuestohasta, StringComparison.Ordinal) <= 0)) && ((f.cantidad ?? 0) - (f.cantidadpedida ?? 0) > 0)).Join(_db.Estados, c => c.Pedidos.fkestados, h => h.documento + "-" + h.id, (c, h) => new { Linea = c, Estado = h }).Where(x => x.Estado.tipoestado <= (int)TipoEstado.Curso).Select(y => y.Linea).OrderByDescending(f => f.id).ToList().Select(f => new PedidosLinImportarModel()
                                                                         {
                                                                             Ancho = f.ancho,
                                                                             Canal = f.canal,
                                                                             Cantidad = (f.cantidad ?? 0) - (f.cantidadpedida ?? 0),
                                                                             Cantidadpedida = f.cantidadpedida,
                                                                             Cuotaiva = f.cuotaiva,
                                                                             Cuotarecargoequivalencia = f.cuotarecargoequivalencia,
                                                                             Decimalesmedidas = f.decimalesmedidas,
                                                                             Decimalesmonedas = f.decimalesmonedas,
                                                                             Descripcion = f.descripcion,
                                                                             Fkarticulos = f.fkarticulos,
                                                                             Fktiposiva = f.fktiposiva,
                                                                             Fkunidades = f.fkunidades,
                                                                             Grueso = f.grueso,
                                                                             Id = f.id,
                                                                             Importe = f.importe,
                                                                             Importedescuento = f.importedescuento,
                                                                             Largo = f.largo,
                                                                             Lote = f.lote,
                                                                             Metros = f.metros,
                                                                             Notas = f.notas,
                                                                             Porcentajedescuento = f.porcentajedescuento,
                                                                             Porcentajeiva = f.porcentajeiva,
                                                                             Porcentajerecargoequivalencia = f.porcentajerecargoequivalencia,
                                                                             Precio = f.precio,
                                                                             Precioanterior = f.precioanterior,
                                                                             Revision = f.revision,
                                                                             Tabla = f.tabla,
                                                                             Fkpedidos = f.Pedidos.id,
                                                                             Fkpedidosreferencia = f.Pedidos.referencia,
                                                                             Fkmonedas = f.Pedidos.fkmonedas,
                                                                             Cabecera = _converterModel.GetModelView(f.Pedidos) as PedidosModel
                                                                         });

        }

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as PedidosValidation;
            currentValidationService.CambiarEstado = true;
            model.set("fkestados", nuevoEstado.CampoId);
            edit(model);
            currentValidationService.CambiarEstado = false;
        }

        public IEnumerable<DocumentosBusqueda> Buscar(IDocumentosFiltros filtros, out int registrostotales)
        {
            var service = new BuscarDocumentosService(_db, Empresa);
            return service.Buscar(filtros, out registrostotales);
        }

        public IEnumerable<IItemResultadoMovile> BuscarDocumento(string referencia)
        {
            var service = new BuscarDocumentosService(_db, Empresa);
            return service.Get<PedidosModel,PedidosLinModel,PedidosTotalesModel>(this, referencia);
        }
    }
}
