using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Web;
using DevExpress.Data.Helpers;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
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
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using IsolationLevel = System.Transactions.IsolationLevel;
using RTraspasosalmacen = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public interface ITraspasosalmacenService
    {

    }

    public  class TraspasosalmacenService : GestionService<TraspasosalmacenModel, Traspasosalmacen>, IDocumentosServices, ITraspasosalmacenService
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
                ((TraspasosalmacenValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion

        #region CTR

        public TraspasosalmacenService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            _importarService = new ImportacionService(context);

        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService = new EstadosService(_context,_db);
            st.List = st.List.OfType<TraspasosalmacenModel>().OrderByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento","Fkalmacen","Almacenorigen","Fkalmacendestino","Almacendestino", "Nombretransportista" };
            var propiedades = Helpers.Helper.getProperties<TraspasosalmacenModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            
            return st;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select t.*,a.descripcion as [Almacenorigen],a2.descripcion as [Almacendestino] from Traspasosalmacen as t " +
                   "inner join almacenes as a on a.empresa=t.empresa and a.id= t.fkalmacen " +
                   "inner join almacenes as a2 on a2.empresa= t.empresa and a2.id= t.fkalmacendestino " +
                   "where t.empresa = '{0}'", Empresa);
        }

        #endregion

        public IEnumerable<TraspasosalmacenLinModel> RecalculaLineas(IEnumerable<TraspasosalmacenLinModel> model,
            double descuentopp, double descuentocomercial, string fkregimeniva, double portes, int decimalesmoneda)
        {
            var result = new List<TraspasosalmacenLinModel>();

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

        public IEnumerable<TraspasosalmacenTotalesModel> Recalculartotales(IEnumerable<TraspasosalmacenLinModel> model, double descuentopp, double descuentocomercial, double portes, int decimalesmoneda)
        {
            var result = new List<TraspasosalmacenTotalesModel>();

            var vector = model.GroupBy(f => f.Fktiposiva);

            foreach (var item in vector)
            {
                var newItem = new TraspasosalmacenTotalesModel();
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

        public TraspasosalmacenModel Clonar(string id)
        {
            var appService=new ApplicationHelper(_context);
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var obj = _converterModel.CreateView(id) as TraspasosalmacenModel;

                obj.Fechadocumento = DateTime.Now;
                obj.Fechavalidez = DateTime.Now.AddMonths(1);
                foreach (var TraspasosalmacenLinModel in obj.Lineas)
                {
                    TraspasosalmacenLinModel.Fkpedidos = null;
                    TraspasosalmacenLinModel.Fkpedidosid = null;
                    TraspasosalmacenLinModel.Fkpedidosreferencia = string.Empty;
                }
                foreach (var item in obj.Lineas)
                {
                    item.Cantidadpedida = 0;
                }
                obj.Fkestados = appService.GetConfiguracion().Estadotraspasosalmaceninicial;
                var contador = ServiceHelper.GetNextId<Traspasosalmacen>(_db, _context.Empresa, obj.Fkseries);
                var identificadorsegmento = "";
                obj.Referencia = ServiceHelper.GetReference<Traspasosalmacen>(_db, obj.Empresa, obj.Fkseries, contador, obj.Fechadocumento.Value, out identificadorsegmento);
                obj.Identificadorsegmento = identificadorsegmento;
                var newItem = _converterModel.CreatePersitance(obj);
                if (_validationService.ValidarGrabar(newItem))
                {
                    TraspasosalmacenModel result;
                    result = _converterModel.GetModelView(newItem) as TraspasosalmacenModel;
                    //generar carpeta
                    DocumentosHelpers.GenerarCarpetaAsociada(result, TipoDocumentos.Traspasosalmacen, _context, _db);
                    newItem.fkcarpetas = result.Fkcarpetas;
                    _db.Set<Traspasosalmacen>().Add(newItem);
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

        public TraspasosalmacenModel GetByReferencia(string referencia)
        {
            var obj =
                _db.Traspasosalmacen.Include("TraspasosalmacenLin")
                    .Include("TraspasosalmacenTotales")
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            ((TraspasosalmacenConverterService)_converterModel).Ejercicio = EjercicioId;
            return _converterModel.GetModelView(obj) as TraspasosalmacenModel;
        }

        public override IModelView get(string id)
        {
            ((TraspasosalmacenConverterService)_converterModel).Ejercicio = EjercicioId;
            return base.get(id);
        }

        #region Api main

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as TraspasosalmacenModel;
                var validation = _validationService as TraspasosalmacenValidation;
                validation.EjercicioId = EjercicioId;

                //Calculo ID
                var contador = ServiceHelper.GetNextId<Traspasosalmacen>(_db, Empresa, model.Fkseries);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<Traspasosalmacen>(_db, model.Empresa, model.Fkseries, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;

                DocumentosHelpers.GenerarCarpetaAsociada(model, TipoDocumentos.Traspasosalmacen, _context, _db);
                RepartirCostesLineas(model.Lineas, model.Costes);
                ModificarLotesLineas(model);

                base.create(obj);

                //CrearStock(obj as TraspasosalmacenModel);
                GenerarMovimientosLineas(model.Lineas, model, TipoOperacionService.InsertarTraspasosalmacen);

                GestionarBundleLineas(obj as TraspasosalmacenModel);

                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            var currentValidationService = _validationService as TraspasosalmacenValidation;
            if(!currentValidationService.ModificarCostes)
                throw new NotImplementedException();

            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as TraspasosalmacenModel;
                var editado = obj as TraspasosalmacenModel;
                RepartirCostesLineas(editado.Lineas, editado.Costes, original.Costes);
                ModificarLotesLineas(editado);

                base.edit(obj);
                //ActualizarStock(original, obj as TraspasosalmacenModel);
                GenerarMovimientosLineas(original.Lineas, original, TipoOperacionService.EliminarTraspasosalmacen);
                GenerarMovimientosLineas(editado.Lineas, editado, TipoOperacionService.ActualizarTraspasosalmacen);
                _db.SaveChanges();
                tran.Complete();
            }
        }

        public override void delete(IModelView obj)
        {
            throw new NotImplementedException("Operación no válida");
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

            sb.Append("select 'Albaran de compra' as [Tipo], ac.Referencia as [Referencia], ac.fechadocumento as [Fecha], ac.importebaseimponible as [Base], ac.nombreproveedor as [Proveedor] from Traspasosalmacen as ac where ac.empresa=@empresa and ac.fkejercicio=@fkejercicio ");
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

        //private void CrearStock(TraspasosalmacenModel nuevo)
        //{
        //    foreach (var item in nuevo.Lineas)
        //        item.Nueva = true;

        //    OperarStock(nuevo, TipoOperacionStock.Entrada);
        //}

        //private void ActualizarStock(TraspasosalmacenModel original, TraspasosalmacenModel nuevo)
        //{
        //    var list = new List<TraspasosalmacenLinModel>();
        //    var lineasModificadas = nuevo.Lineas.Where(f => !original.Lineas.Any(j => j.Flagidentifier == f.Flagidentifier)).ToList();

        //    var lineasEliminadas = original.Lineas.Where(f => !nuevo.Lineas.Where(j => !lineasModificadas.Any(h => h.Flagidentifier == f.Flagidentifier)).Any(j => j.Flagidentifier == f.Flagidentifier)).ToList();
        //    foreach (var item in lineasEliminadas)
        //        item.Cantidad *= -1;

        //    list = lineasModificadas.Union(lineasEliminadas).ToList();


        //    GenerarMovimientosLineas(list, nuevo, TipoOperacionStock.Actualizacion);
        //}

        //private void OperarStock(TraspasosalmacenModel nuevo, TipoOperacionStock operacion)
        //{
        //    GenerarMovimientosLineas(nuevo.Lineas, nuevo, operacion);
            
        //}

        private void GenerarMovimientosLineas(IEnumerable<TraspasosalmacenLinModel> lineas, TraspasosalmacenModel nuevo, TipoOperacionService movimiento)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<TraspasosalmacenDiarioStockSerializable>();
            var vectorArticulos = new Hashtable();

            var operacion = 1;
            if (movimiento == TipoOperacionService.EliminarTraspasosalmacen)
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
                if (articuloObj?.Gestionstock ?? false)
                {

                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        //Fkalmacenes = nuevo.Fkalmacen,
                        Fkalmacenes = nuevo.Fkalmacendestino,
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        //Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = linea.Fkarticulos,
                        Referenciaproveedor = "",
                        Fkcontadorlote = linea.Fkcontadoreslotes,
                        Lote = linea.Lote,
                        Loteid = (linea.Tabla ?? 0).ToString(),
                        Tag = "",
                        Fkunidadesmedida = linea.Fkunidades,
                        Cantidad = (linea.Cantidad ?? 0),
                        Largo = linea.Largo ?? 0,
                        Ancho = linea.Ancho ?? 0,
                        Grueso = linea.Grueso ?? 0,
                        Metros = linea.Metros ?? 0,
                        Documentomovimiento = serializer.GetXml(
                            new TraspasosalmacenDiarioStockSerializable
                            {
                                Id = nuevo.Id,
                                Referencia = nuevo.Referencia,
                                Fkalmacenorigen = nuevo.Fkalmacen,
                                Fkalmacendestino =nuevo.Fkalmacendestino,
                                Linea = linea
                            }),
                        Fkusuarios = Usuarioid,
                        //Tipooperacion = operacion,
                        Costeadicionalmaterial = linea.Costeadicionalmaterial,
                        Costeadicionalotro = linea.Costeadicionalotro,
                        Costeadicionalvariable = linea.Costeadicionalvariable,
                        Costeadicionalportes = linea.Costeadicionalportes,

                        Tipomovimiento = movimiento
                    };

                    movimientosStockService.GenerarMovimiento(model, linea.Nueva ? TipoOperacionService.InsertarTraspasosalmacen : TipoOperacionService.ActualizarTraspasosalmacen);
                }

            }
        }

        #endregion

        #region Helper

        #region Reparto de costes

        private void RepartirCostesLineas(List<TraspasosalmacenLinModel> lineas, List<TraspasosalmacenCostesadicionalesModel> costes, List<TraspasosalmacenCostesadicionalesModel> costesOriginal = null)
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

        private bool SonIgualesCostesOriginalEditado(List<TraspasosalmacenCostesadicionalesModel> costes,
            List<TraspasosalmacenCostesadicionalesModel> costesOriginal)
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

        private void ReparteCoste(List<TraspasosalmacenLinModel> lineas,
            List<TraspasosalmacenCostesadicionalesModel> costes, dynamic reparto)
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

        private void RepartirCosteEnLinea(List<TraspasosalmacenLinModel> lineas, TipoReparto reparto, TipoCoste coste, double costeTotal, double costeUnidad)
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
                if (ultimaLinea !=null)
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

        #region Gestion Bundles 

        private void GestionarBundleLineas(TraspasosalmacenModel model)
        {

            var lineas = model.Lineas.Where(f => !string.IsNullOrEmpty(f.Bundle));
            var group = lineas.GroupBy(f => new { f.Lote, f.Bundle });

            foreach (var item in group)
            {
                GenerarBundle(item.Key.Lote, item.Key.Bundle, model, lineas.Where(f => f.Lote == item.Key.Lote && f.Bundle == item.Key.Bundle));
            }
        }

        private void GenerarBundle(string lote, string bundle, TraspasosalmacenModel model, IEnumerable<TraspasosalmacenLinModel> lineas)
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
                    Fkalmacen = model.Fkalmacendestino,
                    Fkzonaalmacen = Funciones.Qint(model.Fkzonas)
                };

            bundleObj.Fkalmacen = model.Fkalmacendestino;

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

        private void ModificarLotesLineas(TraspasosalmacenModel model)
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

        #endregion

        #region Crear lineas entrega de stock

        public List<TraspasosalmacenLinModel> CrearNuevasLineas(List<TraspasosalmacenLinModel> listado, TraspasosalmacenLinVistaModel model)
        {
            var stockactualService = new StockactualService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), _context, _db) as TiposivaService;
          
          

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
                    
                    var metros = UnidadesService.CalculaResultado(unidadesObj, linea.Cantidad, largo, ancho, grueso, model.Metros);
                    linea.Metros = metros;
                   

                    listado.Add(new TraspasosalmacenLinModel()
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
                        Importedescuento = 0,
                        Importe =0,
                        Decimalesmedidas = unidadesObj.Decimalestotales,
                        Decimalesmonedas = 0,
                        Fktiposiva = "",
                        Porcentajeiva = 0,
                        Porcentajerecargoequivalencia = 0,
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

        private void ValidarKit(List<TraspasosalmacenLinModel> listado, TraspasosalmacenLinVistaModel model)
        {
            var serviceKit = FService.Instance.GetService(typeof(KitModel), _context, _db);
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

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as TraspasosalmacenValidation;
            currentValidationService.CambiarEstado = true;
            model.set("fkestados", nuevoEstado.CampoId);
            edit(model);
            currentValidationService.CambiarEstado = false;
        }

        public void ModificarCostes(TraspasosalmacenModel model)
        {
            var currentValidationService = _validationService as TraspasosalmacenValidation;
            currentValidationService.ModificarCostes = true;
            edit(model);
            currentValidationService.ModificarCostes = false;
        }

        public Task SetEstadoAsync(IModelView model, EstadosModel nuevoEstado)
        {
            throw new NotImplementedException();
        }
    }
}
