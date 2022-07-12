using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Reservasstock;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.BusquedasMovil;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RReservasstock = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Reservas;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Inf.Genericos;
using System.Data.Entity.Migrations;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IReservasstockService
    {

    }

    public class ReservasstockService : GestionService<ReservasstockModel, Reservasstock>, IDocumentosServices,IBuscarDocumento, IDocumentosVentasPorReferencia<ReservasstockModel>,IAgregarLineaDocumentoMovile, IReservasstockService
    {
        #region Member

        private string _ejercicioId;
        private IImportacionService _importarService;
        private bool _flagconsumirreserva = false;

        #endregion

        #region Properties

        public string EjercicioId
        {
            get { return _ejercicioId; }
            set
            {
                _ejercicioId = value;
                ((ReservasstockValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion
        
        #region CTR

        public ReservasstockService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            _importarService = new ImportacionService(context);
        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService = new EstadosService(_context,_db);
            st.List = st.List.OfType<ReservasstockModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento","Fechavalidez", "Fkclientes", "Nombrecliente", "Fkestados", "Importebaseimponible" };
            var propiedades = Helpers.Helper.getProperties<ReservasstockModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.Reservasstock, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            
            return st;
        }

        #endregion

        public IEnumerable<ReservasstockLinModel> RecalculaLineas(IEnumerable<ReservasstockLinModel> model,
            double descuentopp, double descuentocomercial, string fkregimeniva, double portes, int decimalesmoneda)
        {
            var result = new List<ReservasstockLinModel>();

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

        public IEnumerable<ReservasstockTotalesModel> Recalculartotales(IEnumerable<ReservasstockLinModel> model, double descuentopp, double descuentocomercial, double portes, int decimalesmoneda)
        {
            var result = new List<ReservasstockTotalesModel>();

            var vector = model.GroupBy(f => f.Fktiposiva);

            foreach (var item in vector)
            {
                var newItem = new ReservasstockTotalesModel();
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

        public ReservasstockModel Clonar(string id)
        {
            var appService=new ApplicationHelper(_context);
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var obj = _converterModel.CreateView(id) as ReservasstockModel;

                obj.Fechadocumento = DateTime.Now;
                obj.Fechavalidez = DateTime.Now.AddMonths(1);
                foreach (var ReservasstockLinModel in obj.Lineas)
                {
                    ReservasstockLinModel.Fkpedidos = null;
                    ReservasstockLinModel.Fkpedidosid = null;
                    ReservasstockLinModel.Fkpedidosreferencia = string.Empty;
                }
                foreach (var item in obj.Lineas)
                {
                    item.Cantidadpedida = 0;
                }
                obj.Fkestados = appService.GetConfiguracion().Estadoreservasinicial;
                var tipodocumento = "RES"; //Reserva stock
                var contador = ServiceHelper.GetNextId<Reservasstock>(_db, Empresa, obj.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                obj.Referencia = ServiceHelper.GetReference<Reservasstock>(_db, obj.Empresa, obj.Fkseries, tipodocumento, contador, obj.Fechadocumento.Value, out identificadorsegmento);
                obj.Identificadorsegmento = identificadorsegmento;
                var newItem = _converterModel.CreatePersitance(obj);
                if (_validationService.ValidarGrabar(newItem))
                {
                    ReservasstockModel result;
                    result = _converterModel.GetModelView(newItem) as ReservasstockModel;
                    //generar carpeta
                    DocumentosHelpers.GenerarCarpetaAsociada(result, TipoDocumentos.Reservas, _context, _db);
                    newItem.fkcarpetas = result.Fkcarpetas;
                    _db.Set<Reservasstock>().Add(newItem);
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

        public ReservasstockModel GetByReferencia(string referencia)
        {
            var obj =
                _db.Reservasstock.Include("ReservasstockLin")
                    .Include("ReservasstockTotales")
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            ((ReservasstockConverterService)_converterModel).Ejercicio = EjercicioId;
            return _converterModel.GetModelView(obj) as ReservasstockModel;
        }

        public override IModelView get(string id)
        {
            ((ReservasstockConverterService)_converterModel).Ejercicio = EjercicioId;
            return base.get(id);
        }

        public override bool exists(string id)
        {
            var intid = int.Parse(id);
            return _db.Reservasstock.Any(f => f.empresa == Empresa && f.id == intid);
        }

        public bool ExistsByReferencia(string referencia)
        {
            return _db.Reservasstock.Any(f => f.empresa == Empresa && f.referencia == referencia);
        }

        #region Create

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ReservasstockModel;
                var validation = _validationService as ReservasstockValidation;
                validation.EjercicioId = EjercicioId;

                //Calculo ID
                var tipodocumento = "RES"; //Reserva stock
                var contador = ServiceHelper.GetNextId<Reservasstock>(_db, Empresa, model.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<Reservasstock>(_db, model.Empresa, model.Fkseries, tipodocumento, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.Reservas, _context, _db);

                base.create(obj);


                //EntregarStock(obj as ReservasstockModel);
                GenerarMovimientosLineas(model, model.Lineas, TipoOperacionService.InsertarReservaStock);

                _db.SaveChanges();
                tran.Complete();
            }

        }

        //private void EntregarStock(ReservasstockModel nuevo)
        //{
        //    foreach (var item in nuevo.Lineas)
        //        item.Cantidad *= -1;
        //    OperarStock(nuevo, TipoOperacionStock.Salida, TipoOperacionService.InsertarReservaStock);
        //}

        #endregion

        #region Edit

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as ReservasstockModel;
                var editado = obj as ReservasstockModel;
                if (original.Integridadreferencial == editado.Integridadreferencial)
                {
                    var validation = _validationService as ReservasstockValidation;
                    validation.EjercicioId = EjercicioId;
                    DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.Reservas, _context, _db);
                    base.edit(obj);

                    if (!_flagconsumirreserva)
                    {
                        //Siempre
                        GenerarMovimientosLineas(original, original.Lineas, TipoOperacionService.EliminarReservaStock);

                        if(editado.Estado.Tipoestado != TipoEstado.Anulado && editado.Estado.Tipoestado != TipoEstado.Caducado)
                        {
                            GenerarMovimientosLineas(editado, editado.Lineas, TipoOperacionService.InsertarReservaStock);
                        } 
                    }

                    _db.SaveChanges();
                    tran.Complete();
                }
                else throw new IntegridadReferencialException(string.Format(General.ErrorIntegridadReferencial, RReservasstock.TituloEntidad, original.Referencia));
            }

        }

        #endregion

        #region Delete

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as ReservasstockModel;

                if(model.Estado.Tipoestado == TipoEstado.Anulado || model.Estado.Tipoestado == TipoEstado.Caducado || model.Estado.Tipoestado == TipoEstado.Finalizado)
                {
                    throw new ValidationException("Reserva finalizada, no se puede borrar");
                }

                else
                {
                    base.delete(obj);
                    _db.SaveChanges();
                    GenerarMovimientosLineas(model, model.Lineas, TipoOperacionService.EliminarReservaStock);
                    tran.Complete();
                }                
            }
        }


        #endregion

        #region Helper

        private void GenerarMovimientosLineas(ReservasstockModel nuevo, List<ReservasstockLinModel> lineas, TipoOperacionService movimiento)
            //TipoOperacionStock operacion, TipoOperacionService serviciotipo)
        {
            var movimientosStockService = new MovimientosstockService(_context,_db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<ReservasstockDiarioStockSerializable>();
            var vectorArticulos = new Hashtable();

            var operacion = 1;
            if (movimiento == TipoOperacionService.InsertarReservaStock)
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

                var aux = Funciones.ConverterGeneric<ReservasstockLinSerialized>(linea);

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel()
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = nuevo.Fkalmacen,
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = linea.Fkarticulos,
                        Referenciaproveedor = "",
                        Lote = linea.Lote,
                        Loteid = (linea.Tabla ?? 0).ToString(),
                        Tag = "",
                        Fkunidadesmedida = linea.Fkunidades,
                        Cantidad = (linea.Cantidad ?? 0) * operacion,
                        Largo = linea.Largo ?? 0,
                        Ancho = linea.Ancho ?? 0,
                        Grueso = linea.Grueso ?? 0,
                        Metros = (linea.Metros ?? 0) * operacion,
                        Documentomovimiento = serializer.GetXml(
                            new ReservasstockDiarioStockSerializable()
                            {
                                Id = nuevo.Id,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Codigocliente = nuevo.Fkclientes,
                                Linea = aux
                            }),
                        Fkusuarios = Usuarioid,
                        //Tipooperacion = operacion
                        Tipomovimiento = movimiento
                    };

                    movimientosStockService.GenerarMovimiento(model, movimiento);
                }

            }
        }

        #endregion

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as ReservasstockValidation;
            currentValidationService.CambiarEstado = true;
            model.set("fkestados", nuevoEstado.CampoId);
            edit(model);
            currentValidationService.CambiarEstado = false;
        }

        #region Buscar documentos ventas

        public IEnumerable<DocumentosBusqueda> Buscar(IDocumentosFiltros filtros, out int registrostotales)
        {
            var service = new BuscarDocumentosReservasstockService(_db, Empresa);
            return service.Buscar(filtros, out registrostotales);
        }

        public IEnumerable<IItemResultadoMovile> BuscarDocumento(string referencia)
        {
            var service = new BuscarDocumentosReservasstockService(_db, Empresa);
            return service.Get<ReservasstockModel,ReservasstockLinModel,ReservasstockTotalesModel>(this, referencia);
        }

        #endregion

        #region Crear lineas entrega de stock

        public List<ReservasstockLinModel> CrearNuevasLineas(List<ReservasstockLinModel> listado, ReservasstockLinVistaModel model)
        {
            var stockactualService = new StockactualService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), _context, _db) as TiposivaService;
            var monedasService = FService.Instance.GetService(typeof(MonedasModel), _context, _db) as MonedasService;
            var monedasObj = monedasService.get(model.Fkmonedas) as MonedasModel;

            var maxId = listado.Any() ? listado.Max(f => f.Id) + 1 : 1;
            foreach (var linea in model.Lineas)
            {
                if (!listado.Any(f => f.Lote == linea.Lote && f.Tabla == Funciones.Qint(linea.Loteid)))
                {

                    var articuloObj = articulosService.GetArticulo(linea.Fkarticulos, model.Fkcuenta, model.Fkmonedas, model.Fkregimeniva, model.Flujo);

                    var familiaObj = familiasService.get(ArticulosService.GetCodigoFamilia(linea.Fkarticulos)) as FamiliasproductosModel;

                    var ancho = linea.Ancho;
                    var largo = linea.Largo;
                    var grueso = linea.Grueso;
                    if (model.Modificarmedidas)
                    {
                        ancho = model.Ancho;
                        largo = model.Largo;
                        grueso = model.Grueso;
                    }
                    else
                    {
                        var item = familiaObj.Gestionstock
                        ? stockactualService.GetArticuloPorLoteOCodigo(
                            string.Format("{0}{1}", linea.Lote, Funciones.RellenaCod(linea.Loteid, 3)), model.Fkalmacen,
                            Empresa) as MovimientosstockModel : null;
                        ancho = item?.Ancho ?? linea.Ancho;
                        largo = item?.Largo ?? linea.Largo;
                        grueso = item?.Grueso ?? linea.Grueso;
                    }

                    var unidadesObj = unidadesService.get(familiaObj.Fkunidadesmedida) as UnidadesModel;
                    var tiposivaObj = tiposivaService.get(articuloObj.Fktiposiva) as TiposIvaModel;
                    var metros = UnidadesService.CalculaResultado(unidadesObj, linea.Cantidad, largo, ancho, grueso, model.Metros);
                    linea.Metros = metros;
                    var bruto = linea.Metros * model.Precio;
                    var importedescuento = Math.Round(((bruto) * model.Descuento / 100.0), model.Decimalesmonedas);
                    var total = bruto - importedescuento;

                    listado.Add(new ReservasstockLinModel()
                    {
                        Id = maxId++,
                        Fkarticulos = linea.Fkarticulos,
                        Descripcion = articuloObj.Descripcion,
                        Lote = linea.Lote,
                        Tabla = Funciones.Qint(linea.Loteid),
                        Tblnum = Funciones.Qint(linea.Loteid),
                        Cantidad = linea.Cantidad,
                        Largo = largo,
                        Ancho = ancho,
                        Grueso = grueso,
                        Fkunidades = articuloObj.Fkunidades,
                        Metros = metros,
                        Precio = model.Precio,
                        Porcentajedescuento = model.Descuento,
                        Importedescuento = importedescuento,
                        Importe = total,
                        Decimalesmedidas = unidadesObj.Decimalestotales,
                        Decimalesmonedas = monedasObj.Decimales,
                        Fktiposiva = tiposivaObj.Id,
                        Porcentajeiva = tiposivaObj.PorcentajeIva,
                        Porcentajerecargoequivalencia = tiposivaObj.PorcentajeRecargoEquivalencia,
                        Bundle = model.Tipopieza == TipoPieza.Bundle ? model.Lote.Replace(linea.Lote, string.Empty) : string.Empty,
                        Caja = model.Caja,
                        Canal = model.Canal
                    }
                     );
                }

            }

            ValidarKit(listado, model);

            return listado;
        }

        private void ValidarKit(List<ReservasstockLinModel> listado, ReservasstockLinVistaModel model)
        {
            var serviceKit = FService.Instance.GetService(typeof (KitModel), _context, _db);
            if (serviceKit.exists(model.Lote))
            {
                var kitobj = serviceKit.get(model.Lote) as KitModel;

                foreach (var item in kitobj.Lineas)
                {
                    if (!listado.Any(f => item.Lote == f.Lote && Funciones.Qint(item.Loteid) == f.Tabla))
                    {
                        throw new ValidationException(string.Format("El Kit {0} no está completo, falta añadir el lote {1}{2}", model.Lote, item.Lote, Funciones.RellenaCod(item.Loteid, 3)));
                    }
                }
            }
            
        }

        #endregion

        internal void ConsumirReserva(string entregaid)
        {
             var appService=new ApplicationHelper(_context);
            using (var tran = new  TransactionScope())
            {                
                var model = get(entregaid) as ReservasstockModel;
                GenerarMovimientosLineas(model, model.Lineas, TipoOperacionService.EliminarReservaStock);
                var configuracionModel = appService.GetConfiguracion(_db);
                model.Fkestados = configuracionModel.Estadoreservasparcial;
                var aux = _validationService as ReservasstockValidation;
                aux.CambiarEstado = true;
                _flagconsumirreserva = true;
                edit(model);
                _flagconsumirreserva = false;
                aux.CambiarEstado = false;
                tran.Complete();
            }
            
        }

        #region Agregar linea mobile api

        public AgregarLineaDocumentosModel AgregarLinea(string referencia, string lote)
        {
            return OperarLinea(referencia, lote, true);
        }

        public AgregarLineaDocumentosModel EliminarLinea(string referencia, string lote)
        {
            return OperarLinea(referencia, lote, false);
        }

        private AgregarLineaDocumentosModel OperarLinea(string referencia, string lote, bool agregar)
        {
            var result = new AgregarLineaDocumentosModel();
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = GetByReferencia(referencia);
                model = get(model.Id.ToString()) as ReservasstockModel;

                result.Referencia = model.Referencia;
                result.Fecha = model.Fechadocumentocadena;
                model.Lineas = (agregar
                    ? CrearNuevasLineas(model.Lineas, GenerarModeloLin(model, lote))
                    : EliminarLineas(model.Lineas, lote));

                edit(model);

                result.Lineas = model.Lineas
                    .Select(f => new AgregarLineaDocumentosLinModel()
                    {
                        Lote = string.Format("{0}{1}", f.Lote, Funciones.RellenaCod(f.Tabla?.ToString() ?? "0", 3)),
                        Largo = f.SLargo,
                        Ancho = f.SAncho,
                        Grueso = f.SGrueso,
                        Cantidad = f.Cantidad.ToString(),
                        Descripcion = f.Descripcion,
                        Fkarticulos = f.Fkarticulos,
                        Metros = f.SMetros
                    }).ToList();

                tran.Complete();
            }


            return result;
        }

        private List<ReservasstockLinModel> EliminarLineas(List<ReservasstockLinModel> lineas, string lote)
        {
            var item =
                lineas.SingleOrDefault(f => string.Format("{0}{1}", f.Lote, Funciones.RellenaCod(f.Tabla?.ToString() ?? "0", 3)) == lote);
            if (item != null)
            {
                lineas.Remove(item);
            }

            return lineas;

        }

        private ReservasstockLinVistaModel GenerarModeloLin(ReservasstockModel albaranObj, string lote)
        {

            var serviceStock = new StockactualService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            var obj = serviceStock.GetArticuloPorLoteOCodigo(lote, albaranObj.Fkalmacen, albaranObj.Empresa) as MovimientosstockModel;
            if (obj != null)
            {
                var fkarticulos = obj.Fkarticulos;
                var articulosObj = articulosService.GetArticulo(fkarticulos, albaranObj.Fkclientes,
                    albaranObj.Fkmonedas.ToString(), albaranObj.Fkregimeniva, TipoFlujo.Venta);
                var familiaObj = familiasService.get(ArticulosService.GetCodigoFamilia(fkarticulos)) as FamiliasproductosModel;
                var unidadesObj = unidadesService.get(familiaObj.Fkunidadesmedida) as UnidadesModel;
                var metros = UnidadesService.CalculaResultado(unidadesObj, obj.Cantidad, obj.Largo, obj.Ancho, obj.Grueso, obj.Metros);
                obj.Metros = metros;
                return new ReservasstockLinVistaModel()
                {
                    Modificarmedidas = false,
                    Lote = lote,
                    Decimalesmonedas = albaranObj.Decimalesmonedas,
                    Descuentocomercial = albaranObj.Porcentajedescuentocomercialcadena,
                    Descuentoprontopago = albaranObj.Porcentajedescuentoprontopagocadena,
                    Fkcuenta = albaranObj.Fkclientes,
                    Fkmonedas = albaranObj.Fkmonedas.ToString(),
                    Flujo = TipoFlujo.Venta,
                    Fkregimeniva = albaranObj.Fkregimeniva,
                    Portes = albaranObj.Costeportes.ToString(),
                    Fkalmacen = albaranObj.Fkalmacen,
                    Descuento = 0,
                    Precio = articulosObj.Precio ?? 0,
                    Fkarticulos = fkarticulos,
                    Lineas = new List<MovimientosstockModel>(new[] { obj })
                };
            }

            return new ReservasstockLinVistaModel()
            {
                Fkmonedas = albaranObj.Fkmonedas.ToString()
            };
        }


        #endregion
    }
}
