﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.Model.Documentos.PresupuestosCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RPresupuestosCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PresupuestosCompras;
using RProspectos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Prospectos;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IPedidosComprasService
    {

    }

    public class PedidosComprasService : GestionService<PedidosComprasModel, PedidosCompras>, IDocumentosServices, IPedidosComprasService
    {
        public struct StLineasAsistentePedidos
        {
            public string ReferenciaPedidoVenta { get; set; }
            public string IdLinea { get; set; }
        }
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
                ((PedidosComprasValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion
       
        #region CTR

        public PedidosComprasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            _importarService = new ImportacionService(context);
        }

        #endregion

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

            st.List = st.List.OfType<PedidosComprasModel>().Where(s => seriesrol.Contains(s.Fkseries)).OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkproveedores", "Nombrecliente", "Fkestados", "Importebaseimponible" };
            var propiedades = Helpers.Helper.getProperties<PedidosComprasModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.PedidosCompras, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            return st;
        }

        public IEnumerable<PedidosComprasLinModel> RecalculaLineas(IEnumerable<PedidosComprasLinModel> model,
            double descuentopp, double descuentocomercial, string fkregimeniva, double portes, int decimalesmoneda)
        {
            var result = new List<PedidosComprasLinModel>();

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

        public IEnumerable<PedidosComprasTotalesModel> Recalculartotales(IEnumerable<PedidosComprasLinModel> model, double descuentopp, double descuentocomercial, double portes, int decimalesmoneda)
        {
            var result = new List<PedidosComprasTotalesModel>();

            var vector = model.GroupBy(f => f.Fktiposiva);

            foreach (var item in vector)
            {
                var newItem = new PedidosComprasTotalesModel();
                var objIva = _db.TiposIva.Single(f => f.empresa == Empresa && f.id == item.Key);
                newItem.Decimalesmonedas = decimalesmoneda;
                newItem.Fktiposiva = item.Key;
                newItem.Porcentajeiva = objIva.porcentajeiva;
                newItem.Brutototal = Math.Round((item.Sum(f => f.Importe) - item.Sum(f => f.Importedescuento)) ?? 0, decimalesmoneda);
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

        public PedidosComprasModel Clonar(string id)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var obj = _converterModel.CreateView(id) as PedidosComprasModel;

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
                var appService= new ApplicationHelper(_context);
                obj.Fkestados = appService.GetConfiguracion().Estadopedidosventasinicial;
                var tipodocumento = "PEC"; //Pedido compra
                var contador = ServiceHelper.GetNextId<PedidosCompras>(_db, Empresa, obj.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                obj.Referencia = ServiceHelper.GetReference<PedidosCompras>(_db, obj.Empresa, obj.Fkseries, tipodocumento, contador, obj.Fechadocumento.Value, out identificadorsegmento);
                obj.Identificadorsegmento = identificadorsegmento;

                var newItem = _converterModel.CreatePersitance(obj);
                if (_validationService.ValidarGrabar(newItem))
                {
                    PedidosComprasModel result;
                    result = _converterModel.GetModelView(newItem) as PedidosComprasModel;
                    DocumentosHelpers.GenerarCarpetaAsociada(result, TipoDocumentos.PedidosCompras, _context, _db);
                    newItem.fkcarpetas = result.Fkcarpetas;
                    _db.Set<PedidosCompras>().Add(newItem);
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

        public PedidosComprasModel GetByReferencia(string referencia)
        {
            var obj =
                _db.PedidosCompras.Include("PedidosComprasLin")
                    .Include("PedidosComprasTotales")
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            return _converterModel.GetModelView(obj) as PedidosComprasModel;
        }

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as PedidosComprasModel;
                var validation = _validationService as PedidosComprasValidation;
                validation.EjercicioId = EjercicioId;
                //Calculo ID
                var tipodocumento = "PEC"; //Pedido compra
                var contador = ServiceHelper.GetNextId<PedidosCompras>(_db, Empresa, model.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<PedidosCompras>(_db, model.Empresa, model.Fkseries, tipodocumento, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.PedidosCompras, _context, _db);

                base.create(obj);

                ModificarCantidadesPedidasPresupuestosCompras(obj as PedidosComprasModel);

                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var validation = _validationService as PedidosComprasValidation;
                validation.EjercicioId = EjercicioId;
                DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.PedidosCompras, _context, _db);
                base.edit(obj);


                ModificarCantidadesPedidasPresupuestosCompras(obj as PedidosComprasModel);
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
                ModificarCantidadesPedidasPresupuestosCompras(obj as PedidosComprasModel);
                tran.Complete();
            }

        }

        #region Generar pedido compra a partir de lineas de pedidos de venta

        public PedidosComprasModel GenerarPedidoCompraDeLineasPedidos(string fkserie, string fecha, string fkproveedor, IEnumerable<StLineasAsistentePedidos> lineas)
        {

            if (!lineas.Any())
                throw new Exception("No existen lineas para generar un pedido de compra");

            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var result = Helper.fModel.GetModel<PedidosComprasModel>(_context);
                using (var pedidosService = FService.Instance.GetService(typeof(PedidosModel), _context) as PedidosService)
                {

                    ImportarCabecera(fkserie, fkproveedor, fecha, result);
                    GenerarLineas(pedidosService,result, result.Lineas, lineas);

                    result.Totales = Recalculartotales(result.Lineas, result.Porcentajedescuentoprontopago ?? 0,
                        result.Porcentajedescuentocomercial ?? 0, result.Importeportes ?? 0,
                        result.Decimalesmonedas).ToList();

                    result.Importetotaldoc = Math.Round(result.Totales.Sum(f => f.Subtotal) ?? 0.0, result.Decimalesmonedas);

                    create(result);

                    tran.Complete();

                    return result;
                }
            }
        }

        private void ImportarCabecera(string fkserie, string fkproveedores, string fecha, PedidosComprasModel result)
        {
            var proveedoresService =
                 FService.Instance.GetService(typeof(ProveedoresModel), _context, _db) as ProveedoresService;
            var proveedoresObj = proveedoresService.get(fkproveedores) as ProveedoresModel;

            result.Fechadocumento = DateTime.Parse(fecha);
            result.Fkproveedores = fkproveedores;
            result.Fkseries = fkserie;
            var appService =new ApplicationHelper(_context);
            result.Fkestados = appService.GetConfiguracion().Estadopedidoscomprasinicial;

            //datos defecto proveedor
            result.Fkmonedas = proveedoresObj.Fkmonedas;
            result.Nombrecliente = proveedoresObj.Descripcion;
            result.Clientedireccion = proveedoresObj.Direccion;
            result.Clientepoblacion = proveedoresObj.Poblacion;
            result.Clientecp = proveedoresObj.Cp;
            result.Clientepais = proveedoresObj.Pais;
            result.Clienteprovincia = proveedoresObj.Provincia;
            result.Clientetelefono = proveedoresObj.Fax;
            result.Clienteemail = proveedoresObj.Email;
            result.Clientenif = proveedoresObj.Nif;
            result.Porcentajedescuentocomercial = proveedoresObj.Descuentocomercial;
            result.Porcentajedescuentoprontopago = proveedoresObj.Descuentoprontopago;
            result.Fktransportista = proveedoresObj.Fktransportistahabitual;
            result.Fkregimeniva = proveedoresObj.Fkregimeniva;
            result.Incoterm = proveedoresObj.Fkincoterm;
            result.Unidadnegocio = proveedoresObj.Fkunidadnegocio;
            result.Fkpuertos = new PuertoscontrolModel();
            result.Fkpuertos.Fkpaises = proveedoresObj.Fkpuertos.Fkpaises;
            result.Fkpuertos.Id = proveedoresObj.Fkpuertos.Id;
            result.Fkcuentastesoreria = proveedoresObj.Cuentatesoreria;
            result.Fkformaspago = proveedoresObj.Fkformaspago;
            result.Fktransportista = proveedoresObj.Fktransportistahabitual;



        }

        private void GenerarLineas(PedidosService service,PedidosComprasModel pedidocompra, List<PedidosComprasLinModel> lineas, IEnumerable<StLineasAsistentePedidos> nuevasLineas)
        {
            var dictionaryPedidosVentas = new Dictionary<string, PedidosModel>();
            var dictionaryArticulos = new Dictionary<string, ArticulosDocumentosModel>();
            var serviceArticulos = FService.Instance.GetService(typeof (ArticulosModel), _context, _db) as ArticulosService;

            var i = 1;
            foreach (var item in nuevasLineas)
            {
                var pedidoObj = dictionaryPedidosVentas.ContainsKey(item.ReferenciaPedidoVenta)
                    ? dictionaryPedidosVentas[item.ReferenciaPedidoVenta]
                    : service.get(service.GetByReferencia(item.ReferenciaPedidoVenta).Id.ToString()) as PedidosModel;

                if (!dictionaryPedidosVentas.ContainsKey(item.ReferenciaPedidoVenta))
                    dictionaryPedidosVentas.Add(item.ReferenciaPedidoVenta, pedidoObj);

                var lineaPedidoVenta = pedidoObj.Lineas.Single(f => f.Id == int.Parse(item.IdLinea));
                var lineaPedidoCompra = new PedidosComprasLinModel();

                var articulosObj = dictionaryArticulos.ContainsKey(lineaPedidoVenta.Fkarticulos)
                  ? dictionaryArticulos[lineaPedidoVenta.Fkarticulos]
                  : serviceArticulos.GetArticulo(lineaPedidoVenta.Fkarticulos, pedidocompra.Fkproveedores,(pedidocompra.Fkmonedas??978).ToString(),pedidocompra.Fkregimeniva,TipoFlujo.Compra ) as ArticulosDocumentosModel;

                if (!dictionaryArticulos.ContainsKey(lineaPedidoVenta.Fkarticulos))
                    dictionaryArticulos.Add(lineaPedidoVenta.Fkarticulos, articulosObj);

                var properties = lineaPedidoCompra.GetType().GetProperties();
                foreach (var p in properties)
                {
                    if(lineaPedidoVenta.GetType().GetProperty(p.Name)!=null)
                        p.SetValue(lineaPedidoCompra,lineaPedidoVenta.GetType().GetProperty(p.Name).GetValue(lineaPedidoVenta));
                }
                lineaPedidoCompra.Fkpresupuestos = null;
                lineaPedidoCompra.Fkpresupuestosid = null;
                lineaPedidoCompra.Fkpresupuestosreferencia = "";
                lineaPedidoCompra.Largo = lineaPedidoVenta.Largo;
                lineaPedidoCompra.Ancho = lineaPedidoVenta.Ancho;
                lineaPedidoCompra.Grueso = lineaPedidoVenta.Grueso;
                lineaPedidoCompra.Metros = lineaPedidoVenta.Metros;
                lineaPedidoCompra.Id = i++;
                lineaPedidoCompra.Precio = articulosObj.Precio;
                var elem = _importarService.ImportarLinea(lineaPedidoCompra);

                lineaPedidoCompra.Metros = elem.Metros;
                lineaPedidoCompra.Importe = elem.Importe;
                lineaPedidoCompra.Fkpedidosventas = pedidoObj.Id;
                lineaPedidoCompra.Fkpedidosventasreferencia = pedidoObj.Referencia;
                lineas.Add(lineaPedidoCompra);
            }

        }
        #endregion

        #region Importar presupuestos 

        public PedidosComprasModel ImportarPresupuesto(PresupuestosComprasModel presupuesto)
        {
            //calcular serie asociada
            if (presupuesto.Lineas.Any(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0) > 0))
            {
                var prospectoService = FService.Instance.GetService(typeof(ProspectosModel), _context);
                if (prospectoService.exists(presupuesto.Fkproveedores))
                {
                    throw new Exception(RProspectos.ErrorCrearPedidoProspecto);
                }

                var result = Helper.fModel.GetModel<PedidosComprasModel>(_context);
                result.Importado = true;
                ImportarCabecera(presupuesto, result);
                var maxId = result.Lineas.Any() ? result.Lineas.Max(f => f.Id) : 0;
                result.Lineas.AddRange(ImportarLineas(maxId, ConvertLineasModelToILineas(presupuesto.Id.Value.ToString(), presupuesto.Referencia, presupuesto.Lineas)));
                EstablecerSerie(presupuesto.Fkseries, result);

                //recalculo importes lineas y totales
                RecalculaLineas(result.Lineas, result.Porcentajedescuentoprontopago ?? 0, result.Porcentajedescuentocomercial ?? 0, result.Fkregimeniva, result.Importeportes ?? 0, result.Decimalesmonedas);
                result.Totales = Recalculartotales(result.Lineas, result.Porcentajedescuentoprontopago ?? 0,
                    result.Porcentajedescuentocomercial ?? 0, result.Importeportes ?? 0,
                    result.Decimalesmonedas).ToList();

                return result;
            }

            throw new ValidationException(RPresupuestosCompras.ErrorSinCantidadPendiente);
        }

        private IEnumerable<ILineaImportar> ConvertLineasModelToILineas(string idcabecera, string referencia, IEnumerable<PresupuestosComprasLinModel> lineas)
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

        private void EstablecerSerie(string fkseries, PedidosComprasModel result)
        {
            var service = FService.Instance.GetService(typeof(SeriesModel), _context);
            var serieObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.PresupuestosCompras), fkseries)) as SeriesModel;
            var serieAsociada = serieObj.Fkseriesasociada;
            if (!string.IsNullOrEmpty(serieAsociada))
            {
                var serieasociadaObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.PedidosCompras), serieAsociada)) as SeriesModel;
                result.Fkseries = serieasociadaObj.Id;
            }
            else
                throw new ValidationException(Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PresupuestosCompras.ErrorNoExisteSerieAsociada);
        }

        public IEnumerable<PedidosComprasLinModel> ImportarLineas(int maxId, IEnumerable<ILineaImportar> linea)
        {
            return linea.Where(f => f.Cantidad != 0 || f.Articulocomentario).OrderBy(f => f.Orden).Select(f => _importarService.ImportarLinea(f)).Select(f => ConvertILineaImportarToPedidoLinModel(++maxId, f));
        }

        private PedidosComprasLinModel ConvertILineaImportarToPedidoLinModel(int Id, ILineaImportar linea)
        {
            var result = _importarService.ImportarLinea(linea);
            return new PedidosComprasLinModel()
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


        private void ImportarCabecera(PresupuestosComprasModel presupuesto, PedidosComprasModel result)
        {
            var properties = typeof(PedidosComprasModel).GetProperties();

            foreach (var item in properties)
            {
                if (item.Name != "Lineas" && item.Name != "Totales")
                {
                    var property = typeof(PresupuestosComprasModel).GetProperty(item.Name);
                    if (property != null && property.CanWrite)
                    {

                        var value = property.GetValue(presupuesto);
                        item.SetValue(result, value);
                    }
                }
            }

            result.Fechadocumento = DateTime.Now;
            var appService=new ApplicationHelper(_context);
            result.Fkestados = appService.GetConfiguracion().Estadopedidoscomprasinicial;
        }



        #endregion

        #region Helper

        private void ModificarCantidadesPedidasPresupuestosCompras(PedidosComprasModel model)
        {
            var presupuestosService = new PresupuestosComprasService(_context,_db);
            presupuestosService.EjercicioId = EjercicioId;
            var vector = model.Lineas.Where(f => f.Fkpresupuestos.HasValue);

            foreach (var item in vector)
            {
                var presupuesto = _db.PresupuestosCompras.Include("PresupuestosComprasLin").Single(
                    f =>
                        f.empresa == model.Empresa && f.id == item.Fkpresupuestos);

                var cantidadpedida = _db.PedidosComprasLin.Where(
                    f =>
                        f.empresa == model.Empresa && f.fkpresupuestos == item.Fkpresupuestos &&
                        f.fkpresupuestosid == item.Fkpresupuestosid).Sum(f => f.cantidad);

                var linea = presupuesto.PresupuestosComprasLin.SingleOrDefault(f => f.id == item.Fkpresupuestosid);
                if (linea != null)
                {
                    linea.cantidadpedida = cantidadpedida;
                    var validationService = presupuestosService._validationService as PresupuestosComprasValidation;
                    validationService.EjercicioId = EjercicioId;
                    validationService.FlagActualizarCantidadesPedidas = true;
                    validationService.ValidarGrabar(presupuesto);
                    _db.PresupuestosCompras.AddOrUpdate(presupuesto);
                }

            }
            _db.SaveChanges();
        }

        #endregion

        public IEnumerable<PedidosComprasModel> BuscarPedidosComprasImportar(string cliente)
        {

            var list = _db.PedidosCompras.Include("PedidosComprasLin")
                  .Where(
                      f =>
                          f.empresa == Empresa && f.fkproveedores == cliente &&
                          f.PedidosComprasLin.Any(j => ((j.cantidad ?? 0) - (j.cantidadpedida ?? 0) > 0))).Join(_db.Estados, p => p.fkestados, e => e.documento + "-" + e.id, (a, b) => new { a, b }).ToList();
            return list.Where(f => f.b.tipoestado <= (int)TipoEstado.Curso).Select(f => _converterModel.GetModelView(f.a) as PedidosComprasModel).OrderByDescending(f => f.Id);
        }

        public IEnumerable<PedidosComprasLinImportarModel> BuscarLineasPedidosCompras(string cliente, string presupuestodesde, string presupuestohasta)
        {

            return _db.PedidosComprasLin.Include("PedidosCompras").Where(f => f.empresa == Empresa && f.PedidosCompras.fkproveedores == cliente && (string.IsNullOrEmpty(presupuestodesde) || (string.Compare(f.PedidosCompras.referencia, presupuestodesde, StringComparison.Ordinal) >= 0)) &&
                                                                         (string.IsNullOrEmpty(presupuestohasta) || (string.Compare(f.PedidosCompras.referencia, presupuestohasta, StringComparison.Ordinal) <= 0)) && ((f.cantidad ?? 0) - (f.cantidadpedida ?? 0) > 0)).Join(_db.Estados, c => c.PedidosCompras.fkestados, h => h.documento + "-" + h.id, (c, h) => new { Linea = c, Estado = h }).Where(x => x.Estado.tipoestado <= (int)TipoEstado.Curso).Select(y => y.Linea).OrderByDescending(f => f.id).ToList().Select(f => new PedidosComprasLinImportarModel()
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
                                                                             FkPedidosCompras = f.PedidosCompras.id,
                                                                             FkPedidosComprasreferencia = f.PedidosCompras.referencia,
                                                                             Fkmonedas = f.PedidosCompras.fkmonedas,
                                                                             Cabecera = _converterModel.GetModelView(f.PedidosCompras) as PedidosComprasModel
                                                                         });

        }

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as PedidosComprasValidation;
            currentValidationService.CambiarEstado = true;
            model.set("fkestados", nuevoEstado.CampoId);
            edit(model);
            currentValidationService.CambiarEstado = false;
        }
    }
}
