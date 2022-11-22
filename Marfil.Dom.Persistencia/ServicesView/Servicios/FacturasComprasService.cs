using System;
using System.Collections;
using System.Collections.Generic;
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
using Marfil.Dom.Persistencia.Model.Documentos.FacturasCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
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
using RFacturasCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.FacturasCompras;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IFacturasComprasService
    {

    }

    public class FacturasComprasService : GestionService<FacturasComprasModel, FacturasCompras>, IDocumentosServices, IFacturasComprasService
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
                ((FacturasComprasValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion
        
        #region CTR

        public FacturasComprasService(IContextService context, MarfilEntities db = null) : base(context, db)
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

            st.List = st.List.OfType<FacturasComprasModel>().Where(s => seriesrol.Contains(s.Fkseries)).OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkproveedores", "Nombrecliente", "Fkestados", "Importebaseimponible" };
            var propiedades = Helpers.Helper.getProperties<FacturasComprasModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.FacturasCompras, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
           

            return st;
        }

        public IEnumerable<FacturasComprasLinModel> RecalculaLineas(IEnumerable<FacturasComprasLinModel> model,
            double descuentopp, double descuentocomercial, string fkregimeniva, double portes, int decimalesmoneda)
        {
            var result = new List<FacturasComprasLinModel>();

            foreach (var item in model)
            {
                if (item.Fkregimeniva != fkregimeniva)
                {
                    var tiposivaService = FService.Instance.GetService(typeof(TiposIvaModel), _context) as TiposivaService;
                    var grupo = _db.Articulos.Single(f => f.empresa == Empresa && f.id == item.Fkarticulos);
                    var ivaObj = tiposivaService.GetTipoIva(grupo.fkgruposiva, fkregimeniva);
                    item.Fktiposiva = ivaObj.Id;
                    item.Porcentajeiva = ivaObj.PorcentajeIva;
                    item.Porcentajerecargoequivalencia = ivaObj.PorcentajeRecargoEquivalencia;
                    item.Fkregimeniva = fkregimeniva;
                }

                result.Add(item);
            }

            return result;
        }

        public IEnumerable<FacturasComprasTotalesModel> Recalculartotales(IEnumerable<FacturasComprasLinModel> model, double descuentopp, double descuentocomercial, double portes, int decimalesmoneda, double porcentajeretencion = 0)
        {
            var result = new List<FacturasComprasTotalesModel>();

            var vector = model.GroupBy(f => f.Fktiposiva);

            foreach (var item in vector)
            {
                var newItem = new FacturasComprasTotalesModel();
                var objIva = _db.TiposIva.Single(f => f.empresa == Empresa && f.id == item.Key);
                newItem.Decimalesmonedas = decimalesmoneda;
                newItem.Fktiposiva = item.Key;
                newItem.Porcentajeiva = objIva.porcentajeiva;
                //newItem.Brutototal = Math.Round((item.Sum(f => f.Importe) - item.Sum(f => f.Importedescuento)) ?? 0, decimalesmoneda);
                newItem.Brutototal = Math.Round((item.Sum(f => f.Importe)) ?? 0, decimalesmoneda); // 26/03/2021 Cambio
                newItem.Porcentajerecargoequivalencia = objIva.porcentajerecargoequivalente;
                newItem.Porcentajedescuentoprontopago = descuentopp;
                newItem.Porcentajedescuentocomercial = descuentocomercial;
                newItem.Porcentajeretencion = porcentajeretencion;
                newItem.Importedescuentocomercial = Math.Round((double)((newItem.Brutototal * descuentocomercial ?? 0) / 100.0), decimalesmoneda);
                newItem.Importedescuentoprontopago = Math.Round((double)((double)(newItem.Brutototal - newItem.Importedescuentocomercial) * (descuentopp / 100.0)), decimalesmoneda);

                var baseimponible = (newItem.Brutototal ?? 0) - ((newItem.Importedescuentocomercial ?? 0) + (newItem.Importedescuentoprontopago ?? 0));
                newItem.Baseimponible = baseimponible;
                newItem.Cuotaiva = Math.Round(baseimponible * ((objIva.porcentajeiva ?? 0) / 100.0), decimalesmoneda);
                newItem.Importerecargoequivalencia = Math.Round(baseimponible * ((objIva.porcentajerecargoequivalente ?? 0) / 100.0), decimalesmoneda);
                newItem.Baseretencion = baseimponible; // Cambio futuro
                newItem.Importeretencion = Math.Round(baseimponible * ((newItem.Porcentajeretencion ?? 0) / 100.0), decimalesmoneda);
                newItem.Subtotal = Math.Round(baseimponible + (newItem.Cuotaiva ?? 0) + (newItem.Importerecargoequivalencia ?? 0) 
                                                - (newItem.Importeretencion ?? 0), decimalesmoneda);
                result.Add(newItem);
            }

            return result;
        }

        private void CalcularTotalesCabecera(FacturasComprasModel model)
        {
            var decimales = _db.Monedas.Single(f => f.id == model.Fkmonedas).decimales ?? 0;

            model.Importebruto = Math.Round((double)model.Totales.Sum(f => f.Brutototal), decimales);
            model.Importedescuentoprontopago = Math.Round((double)model.Totales.Sum(f => f.Importedescuentoprontopago), decimales);
            model.Importedescuentocomercial = Math.Round((double)model.Totales.Sum(f => f.Importedescuentocomercial), decimales);
            model.Importebaseimponible = Math.Round((double)model.Totales.Sum(f => f.Baseimponible), decimales);
            model.Importetotaldoc = Math.Round((double)model.Totales.Sum(f => f.Subtotal), decimales);

            //todo revisar esto y recalcular el importe total
            model.Importetotalmonedabase = model.Totales.Sum(f => f.Subtotal * (model.Cambioadicional ?? 1.0));

            //ang - comprobar total neto lineas
            if (model.Importebaseimponible != model.Lineas.Sum(l => l.Importenetolinea))
            {
                model.Lineas.Last().Importenetolinea += model.Importebaseimponible - model.Lineas.Sum(l => l.Importenetolinea);
            }
        }

        public FacturasComprasModel GetByReferencia(string referencia)
        {
            var obj =
                _db.FacturasCompras.Include("FacturasComprasLin")
                    .Include("FacturasComprasTotales")
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            return _converterModel.GetModelView(obj) as FacturasComprasModel;
        }

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as FacturasComprasModel;
                var validation = _validationService as FacturasComprasValidation;
                validation.EjercicioId = EjercicioId;
                var tipodocumento = "FRC"; //Facturas compra
                var contador = ServiceHelper.GetNextId<FacturasCompras>(_db, Empresa, model.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<FacturasCompras>(_db, model.Empresa, model.Fkseries, tipodocumento, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;
                DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.FacturasCompras, _context, _db);
                var newItem = _converterModel.CreatePersitance(obj);

                if (_validationService.ValidarGrabar(newItem))
                {
                    _db.Set<FacturasCompras>().Add(newItem);
                    try
                    {
                       
                        _db.SaveChanges();
                        obj.set("Id",newItem.id);
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

                }

                ModificarCantidadesPedidasAlbaranes(obj as FacturasComprasModel);

                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var validation = _validationService as FacturasComprasValidation;
                validation.EjercicioId = EjercicioId;
                DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.FacturasCompras, _context, _db);
                      
                var model = obj as FacturasComprasModel;
                model.Totales = Recalculartotales(model.Lineas, model.Porcentajedescuentoprontopago ?? 0, model.Porcentajedescuentocomercial ?? 0,
                                    model.Importeportes ?? 0, model.Decimalesmonedas, model.Porcentajeretencion ?? 0).ToList();
                CalcularTotalesCabecera(model);
                model.Vencimientos = RefrescarVencimientos(model, model.Fkformaspago.ToString());

                base.edit(obj);
              
                ModificarCantidadesPedidasAlbaranes(obj as FacturasComprasModel);
                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as FacturasComprasModel;

                base.delete(obj);
                _db.SaveChanges();
                ModificarCantidadesPedidasAlbaranes(obj as FacturasComprasModel, TipoOperacion.Baja);                

                var service = FService.Instance.GetService(typeof(AlbaranesComprasModel), _context) as AlbaranesComprasService;
                var _converterModel = FConverterModel.Instance.CreateConverterModelService<AlbaranesComprasModel, AlbaranesCompras>(_context, _db, Empresa);
                var serviceEstados = new EstadosService(_context);

                foreach (var linea in model.Lineas)
                {
                    var modelview = _converterModel.CreateView(linea.Fkalbaranes.ToString());

                    var listEstadosInicial = serviceEstados.GetStates(DocumentoEstado.AlbaranesCompras, TipoEstado.Diseño).Where(f => f.Tipoestado == TipoEstado.Diseño);
                    var nuevoEstado = listEstadosInicial.Where(f => f.Documento == DocumentoEstado.AlbaranesCompras).SingleOrDefault() ?? listEstadosInicial.First();

                    service.SetEstado(modelview, nuevoEstado);
                }

                tran.Complete();
            }

        }

        #region Importar Albaran 

        public double AgregarAlbaranes(IEnumerable<string> vector, List<FacturasComprasLinModel> lineasactuales, ref List<FacturasComprasTotalesModel> totalesactuales, double porcentajecomercial, double porcentajeprontopago, int decimalesmonedas)
        {
            using (
                var albaranesService =
                    FService.Instance.GetService(typeof(AlbaranesComprasModel), _context) as AlbaranesComprasService)
            {                
                GenerarLineas(albaranesService, lineasactuales, vector);
                
                totalesactuales = Recalculartotales(lineasactuales, porcentajeprontopago,
                   porcentajecomercial, 0,
                    decimalesmonedas).ToList();

                //return (double)lineasactuales.Where(f => vector.Contains(f.Fkalbaranesreferencia)).Select(f => f.Importe).Sum();
                return totalesactuales.Select(f => f.Subtotal).Sum() ?? 0;
            }

        }

        public FacturasComprasModel ImportarAlbaranes(string serie, string fecha, IEnumerable<string> albaranesreferencia)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var result = Helper.fModel.GetModel<FacturasComprasModel>(_context);
                using (var albaranesService = FService.Instance.GetService(typeof(AlbaranesComprasModel), _context) as AlbaranesComprasService)
                {

                    var albaran = albaranesService.GetByReferencia(albaranesreferencia.First());
                    
                    result.Importado = true;
                    ImportarCabecera(albaran, serie, fecha, result);

                 
                    ObtenerCriterioIva(result.Fkproveedores,result);

                    ObtenerTPCtiporetencion(result.Fkproveedores, result);

                    GenerarLineas(albaranesService, result.Lineas, albaranesreferencia);

                    //retención
                    result.Fktiposretenciones = _db.Proveedores.Where(f => f.empresa == Empresa && f.fkcuentas == result.Fkproveedores)
                                                .Select(f => f.fktiposretencion).SingleOrDefault();

                    if (!string.IsNullOrEmpty(result.Fktiposretenciones))
                    {
                        result.Porcentajeretencion = _db.Tiposretenciones.Where(f => f.empresa == Empresa && f.id == result.Fktiposretenciones)
                                                                        .Select(f => f.porcentajeretencion).SingleOrDefault();
                    }

                    result.Totales = Recalculartotales(result.Lineas, result.Porcentajedescuentoprontopago ?? 0,
                        result.Porcentajedescuentocomercial ?? 0, result.Importeportes ?? 0,
                        result.Decimalesmonedas, result.Porcentajeretencion ?? 0).ToList();

                    result.Importetotaldoc = Math.Round(result.Totales.Sum(f => f.Subtotal)??0.0,result.Decimalesmonedas);
                    result.Importefacturaproveedor = result.Importetotaldoc;
                    GenerarVencimientos(result);
                    //create(result);

                    //  Cambiamos el estado del/los albarán/es
                    foreach (var item in albaranesreferencia)
                    {
                        var albaranEstado = albaranesService.GetByReferencia(item);

                        var Confservice = FService.Instance.GetService(typeof(ConfiguracionModel), _context) as ConfiguracionService;
                        albaranEstado.Fkestados = Confservice.GetEstadoFinAlbaranesCompras();

                        var newItem = albaranesService._converterModel.CreatePersitance(albaranEstado);
                        _db.Set<AlbaranesCompras>().AddOrUpdate(newItem);
                    }

                    _db.SaveChanges();
                    tran.Complete();

                    return result;
                }
            }

        }

        private void GenerarLineas(AlbaranesComprasService albaranesService, List<FacturasComprasLinModel> lineas, IEnumerable<string> albaranesreferencia)
        {
            var list = albaranesreferencia.Where(f => CanImportarLinea(lineas, f));
            foreach (var item in list)
            {
                GenerarLinea(albaranesService, lineas, item);
            }
        }

        private bool CanImportarLinea(List<FacturasComprasLinModel> lineas, string referencia)
        {
            //Todo revisar esto
            return true;// lineas.All(f => f.Fkalbaranesreferencia != referencia);
        }

        private void GenerarLinea(AlbaranesComprasService albaranesService, List<FacturasComprasLinModel> lineas, string referencia)
        {
            var albaran = albaranesService.GetByReferencia(referencia);
            albaran = albaranesService.get(albaran.Id.ToString()) as AlbaranesComprasModel;
            //if (albaran.Lineas.Any(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0) > 0)) ANG - Se quita la comprobación de cantidad pedida. El albaran se factura entero
            if (albaran.Lineas.Any(f => (f.Cantidad ?? 0) != 0))
            {
                var maxId = lineas.Any() ? lineas.Max(f => f.Id) + 1 : 1;
                lineas.AddRange(ImportarLineas(albaran.Id,maxId, albaranesService.GetLineasImportarAlbaran(referencia)));
            }

            if (lineas.Count == 0)
                throw new ValidationException("Los albaranes seleccionados no han generado ninguna linea");
        }

        public IEnumerable<FacturasComprasLinModel> ImportarLineas(int albaranId,int maxId, IEnumerable<ILineaImportar> linea)
        {

            return linea.Select(f => ConvertILineaImportarToPedidoLinModel( ++maxId,  f)).OrderBy(f=>f.Orden);
        }



        private FacturasComprasLinModel ConvertILineaImportarToPedidoLinModel(int idlinea, ILineaImportar linea)
        {
            var idalbaran = Funciones.Qint(linea.Fkdocumento);
            var serviceEmpresa = FService.Instance.GetService(typeof(EmpresaModel), _context);
            var empresa = serviceEmpresa.get(_context.Empresa) as EmpresaModel;

            var albaran =
                _db.AlbaranesCompras.SingleOrDefault(
                    f =>
                        f.empresa == Empresa && f.id == idalbaran);
            var metros = Math.Round(linea.Metros, linea.Decimalesmedidas);
            var precio = Math.Round(linea.Precio, empresa.Decimalesprecios ?? 2);
            var bruto = metros*precio;
            var cuotadescuento = Math.Round(bruto*linea.Porcentajedescuento/100.0, linea.Decimalesmonedas);
            var baseimpo = bruto - cuotadescuento;
            return new FacturasComprasLinModel()
            {
                Cantidadpedida = 0,
                Id = idlinea,
                Ancho = linea.Ancho,
                Canal = linea.Canal,
                Cantidad = linea.Cantidad,
                Cuotaiva = linea.Cuotaiva,
                Cuotarecargoequivalencia = linea.Cuotarecargoequivalencia,
                Decimalesmedidas = linea.Decimalesmedidas,
                Decimalesmonedas = linea.Decimalesmonedas,
                Fkregimeniva = linea.Fkregimeniva,
                Fkunidades = linea.Fkunidades,
                Metros = metros,
                Precio = precio,
                Fkarticulos = linea.Fkarticulos,
                Descripcion = linea.Descripcion,
                Fktiposiva = linea.Fktiposiva,
                Grueso = linea.Grueso,
                Importe = baseimpo,
                Importedescuento = cuotadescuento,
                Largo = linea.Largo,
                Lote = linea.Lote,
                Notas = linea.Notas,
                Porcentajedescuento = linea.Porcentajedescuento,
                Porcentajeiva = linea.Porcentajeiva,
                Porcentajerecargoequivalencia = linea.Porcentajerecargoequivalencia,
                Precioanterior = linea.Precioanterior,
                Revision = linea.Revision,
                Tabla = linea.Tabla,
                Fkalbaranes = idalbaran,
                Fkalbaranesfecha = albaran?.fechadocumento,
                Fkalbaranesreferencia = albaran?.referencia,
                Fkalbaranesfkcriteriosagrupacion = albaran?.fkcriteriosagrupacion,
                Orden=linea.Orden??0
            };

        }

        private void ObtenerCriterioIva(string id, FacturasComprasModel result)
        {
            
            var list = _db.Acreedores.Where(f => f.empresa == Empresa)
                                     .Where(f => f.fkcuentas == id)
                                     .SingleOrDefault();

            if (list != null)
            {
                result.Criterioiva = (Model.Terceros.CriterioIVA)list.criterioiva;
            }
            else
            { 

                var list2 = _db.Proveedores.Where(f => f.empresa == Empresa)
                                     .Where(f => f.fkcuentas == id)
                                     .SingleOrDefault();
                if (list2 != null)
                {
                    result.Criterioiva = (Model.Terceros.CriterioIVA)list2.criterioiva;
                }
            }

            
        }

        private void ObtenerTPCtiporetencion(string id, FacturasComprasModel result)
        {

            var list = _db.Acreedores.Where(f => f.empresa == Empresa)
                                        .Where(f => f.fkcuentas == id)
                                        .SingleOrDefault();

            if (list != null)
            {
                var tiporetencion = _db.Tiposretenciones.Where(f => f.id == list.fktiposretencion).SingleOrDefault();
                if (tiporetencion != null)
                    result.Porcentajeretencion = tiporetencion.porcentajeretencion; 
            }
            else
            {
                var list2 = _db.Proveedores.Where(f => f.empresa == Empresa)
                                            .Where(f => f.fkcuentas == id)
                                            .SingleOrDefault();
                if (list2 != null)
                {
                    var tiporetencion = _db.Tiposretenciones.Where(f => f.id == list2.fktiposretencion).SingleOrDefault();
                    if (tiporetencion != null)
                        result.Porcentajeretencion = tiporetencion.porcentajeretencion;
                }
            }

        }

        private void ImportarCabecera(AlbaranesComprasModel presupuesto, string serie, string fecha, FacturasComprasModel result)
        {
            var appService = new ApplicationHelper(_context);
            var properties = typeof(FacturasComprasModel).GetProperties();
            foreach (var item in properties)
            {
                if (item.Name != "Lineas" && item.Name != "Totales")
                {
                    var property = typeof(AlbaranesComprasModel).GetProperty(item.Name);
                    if (property != null && property.CanWrite)
                    {
                        var value = property.GetValue(presupuesto);
                        item.SetValue(result, value);
                    }
                }
            }

            result.Nombrecliente = presupuesto.Nombreproveedor;
            result.Clientedireccion = presupuesto.Proveedordireccion;
            result.Clientepoblacion = presupuesto.Proveedorpoblacion;
            result.Clienteprovincia = presupuesto.Proveedorprovincia;
            result.Clientetelefono = presupuesto.Proveedortelefono;
            result.Clientepais = presupuesto.Proveedorpais;
            result.Clienteemail = presupuesto.Proveedoremail;
            result.Clientenif = presupuesto.Proveedornif;

            result.Fechadocumento = DateTime.Parse(fecha);
            result.Fechadocumentoproveedor = result.Fechadocumento;
            // result.Numerodocumentoproveedor = "*";

            result.Fkseries = serie;
            result.Fkestados = appService.GetConfiguracion().Estadofacturascomprasinicial;

            //asignamos el tipo de factura según el proveedor o acreedor
            if (_db.Proveedores.Where(f => f.empresa == Empresa && f.fkcuentas == presupuesto.Fkproveedores).FirstOrDefault() != null)
            {
                var tipofactura = _db.Proveedores.Where(f => f.empresa == Empresa && f.fkcuentas == presupuesto.Fkproveedores).FirstOrDefault().fktipofactura;
                if (tipofactura != null)
                {
                    result.Fktipofactura = tipofactura;
                }
            } else if (_db.Acreedores.Where(f => f.empresa == Empresa && f.fkcuentas == presupuesto.Fkproveedores).FirstOrDefault() != null)
            {
                var tipofactura = _db.Acreedores.Where(f => f.empresa == Empresa && f.fkcuentas == presupuesto.Fkproveedores).FirstOrDefault().fktipofactura;
                if (tipofactura != null)
                {
                    result.Fktipofactura = tipofactura;
                }
            }

            var decimalesmonedas = _db.Monedas.Single(f => f.id == result.Fkmonedas.Value).decimales;
            result.Decimalesmonedas = decimalesmonedas.Value;
            
        }


        public List<FacturasComprasVencimientosModel> RefrescarVencimientos(FacturasComprasModel cabecera, string idformapago)
        {
            
            cabecera.Fkformaspago = Funciones.Qint(idformapago);
            return GenerarLineasVencimientos(cabecera);
        }

        private void GenerarVencimientos(FacturasComprasModel cabecera)
        {
            cabecera.Vencimientos = GenerarLineasVencimientos(cabecera);
        }

        private List<FacturasComprasVencimientosModel> GenerarLineasVencimientos(FacturasComprasModel cabecera)
        {

            var result = new List<FacturasComprasVencimientosModel>();
            var formapagoService = FService.Instance.GetService(typeof(FormasPagoModel), _context) as FormasPagoService;
            var formapagoObj = formapagoService.get(cabecera.Fkformaspago.ToString()) as FormasPagoModel;

            var tipocuentaService = new TiposcuentasService(_context);
            var tipocuenta = tipocuentaService.GetTipoTercero(cabecera.Fkproveedores).SingleOrDefault().ToString();

            //Cuentas
            var cuentasService = FService.Instance.GetService(typeof(CuentasModel), _context) as CuentasService;
            var cuenta = cuentasService.get(cabecera.Fkproveedores) as CuentasModel;

            int diafijopago1;
            int diafijopago2;

            //Proveedor
            if (cuenta.Tiposcuentas == 1)
            {
                var proveedoresService = FService.Instance.GetService(typeof(ProveedoresModel), _context) as ProveedoresService;
                var cuentaProveedor = proveedoresService.get(cabecera.Fkproveedores) as ProveedoresModel;
                diafijopago1 = cuentaProveedor.Diafijopago1;
                diafijopago2 = cuentaProveedor.Diafijopago2;
            }

            else
            {
                var acreedoresService = FService.Instance.GetService(typeof(AcreedoresModel), _context) as AcreedoresService;
                var cuentaAcreedor = acreedoresService.get(cabecera.Fkproveedores) as AcreedoresModel;
                diafijopago1 = cuentaAcreedor.Diafijopago1;
                diafijopago2 = cuentaAcreedor.Diafijopago2;
            }  
            
            var monedasService = FService.Instance.GetService(typeof(MonedasModel), _context) as MonedasService;
            var monedasObj = monedasService.get(cabecera.Fkmonedas.ToString()) as MonedasModel;
            var diafijo1 = diafijopago1;
            var diafijo2 = diafijopago2;
            var decimales = monedasObj.Decimales;
            var maxId = result.Any() ? result.Max(f => f.Id) + 1 : 1;
            var list = formapagoObj.Lineas.ToList();

            var acumulado = 0.0;
            var totaldocumento = cabecera.Importetotaldoc.Value;
            for (var i = 0; i < list.Count(); i++)
            {
                var cuotavencimiento = i >= list.Count() - 1 ? totaldocumento - acumulado : Math.Round((totaldocumento * list[i].PorcentajePago) / 100.0, decimales);
                acumulado += cuotavencimiento;
                result.Add(new FacturasComprasVencimientosModel()
                {
                    Id = maxId++,
                    Diasvencimiento = list[i].DiasVencimiento,
                    Fechavencimiento = GetFechavencimiento(cabecera.Fechadocumentoproveedor.Value, list[i].DiasVencimiento, diafijo1, diafijo2, formapagoObj.ExcluirFestivos),
                    Importevencimiento = cuotavencimiento,
                    Decimalesmonedas = decimales
                });
            }

            return result;
        }

        private static DateTime GetFechavencimiento(DateTime fechafactura, int diavencimiento, int diapago1, int diapago2, bool excluirfestivos)
        {
            var resultado = fechafactura.AddDays(diavencimiento);

            if (diapago1 == 0 && diapago2 == 0)
            {
                if (excluirfestivos)
                {
                    var increment = 0;
                    if (resultado.DayOfWeek == DayOfWeek.Sunday)
                    {
                        increment = 1;
                    }
                    else if (resultado.DayOfWeek == DayOfWeek.Saturday)
                    {
                        increment = 2;
                    }
                    if (increment > 0)
                        resultado = resultado.AddDays(increment);
                }
            }
            else
            {
                var dia1 = diapago1 < diapago2 ? diapago1 : diapago2;
                var dia2 = diapago1 < diapago2 ? diapago2 : diapago1;

                if (dia1 > DateTime.DaysInMonth(resultado.Year, resultado.Month))
                {
                    dia1 = DateTime.DaysInMonth(resultado.Year, resultado.Month);
                }
                if (dia2 > DateTime.DaysInMonth(resultado.Year, resultado.Month))
                {
                    dia2 = DateTime.DaysInMonth(resultado.Year, resultado.Month);
                }

                if (resultado.Day <= dia1)
                {
                    //asignar al dia1
                    resultado = new DateTime(resultado.Year, resultado.Month, dia1);
                }
                else if (resultado.Day <= dia2)
                {
                    //asignar al dia2
                    resultado = new DateTime(resultado.Year, resultado.Month, dia2);
                }
                else if (resultado.Day > dia2)
                {
                    //asignar al dia1 del siguiente mes
                    var aux = new DateTime(resultado.Year, resultado.Month, dia1);
                    resultado = aux.AddMonths(1);
                }
            }

            return resultado;
        }

        public void GetVencimiento(FacturasComprasModel cabecera, List<FacturasComprasVencimientosModel> lineas, FacturasComprasVencimientosModel item)
        {
            var formapagoService = FService.Instance.GetService(typeof(FormasPagoModel), _context) as FormasPagoService;
            var formapagoObj = formapagoService.get(cabecera.Fkformaspago.ToString()) as FormasPagoModel;
            var proveedoresService = FService.Instance.GetService(typeof(ProveedoresModel), _context) as ProveedoresService;
            var clienteObj = proveedoresService.get(cabecera.Fkproveedores) as ProveedoresModel;
            var monedasService = FService.Instance.GetService(typeof(MonedasModel), _context) as MonedasService;
            var monedasObj = monedasService.get(cabecera.Fkmonedas.ToString()) as MonedasModel;

            var diafijo1 = clienteObj.Diafijopago1;
            var diafijo2 = clienteObj.Diafijopago2;
            var fechaFactura = cabecera.Fechadocumento;
            var decimales = monedasObj.Decimales;

            item.Fechavencimiento = GetFechavencimiento(fechaFactura.Value, item.Diasvencimiento.Value, diafijo1,
                diafijo2, formapagoObj.ExcluirFestivos);
            item.Importevencimiento = Math.Round(item.Importevencimiento.Value, decimales);
            item.Decimalesmonedas = monedasObj.Decimales;
        }

        #endregion

        #region Helper

        private void ModificarCantidadesPedidasAlbaranes(FacturasComprasModel model, TipoOperacion tipo = TipoOperacion.Editar)
        {
            var AlbaranesComprasService = new AlbaranesComprasService(_context,_db);
            AlbaranesComprasService.EjercicioId = EjercicioId;

            foreach (var item in model.Lineas)
            {
                var albaran = _db.AlbaranesCompras.Include("AlbaranesComprasLin").Single(
                    f =>
                        f.empresa == model.Empresa && f.id == item.Fkalbaranes);
              
                foreach (var linea in albaran.AlbaranesComprasLin)
                {
                    
                    linea.cantidadpedida = tipo == TipoOperacion.Baja ? 0 : linea.cantidad;

                }
                var validationService = AlbaranesComprasService._validationService as AlbaranesComprasValidation;
                validationService.EjercicioId = EjercicioId;
                validationService.FlagActualizarCantidadesFacturadas = true;
                validationService.ValidarGrabar(albaran);
                _db.AlbaranesCompras.AddOrUpdate(albaran);
            }
            _db.SaveChanges();
        }

        #endregion

       
        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var aux = model as FacturasComprasModel;
            //var modelFacturas =get(aux.Id.ToString()) as FacturasComprasModel;
            //if (modelFacturas.Estado.Tipoestado < TipoEstado.Finalizado)
            //{
                var currentValidationService = _validationService as FacturasComprasValidation;
                currentValidationService.CambiarEstado = true;
                currentValidationService.Margenfactura = _appService.GetConfiguracion(_db).Margenfacturacompra;
                model.set("fkestados", nuevoEstado.CampoId);
                edit(model);
                currentValidationService.CambiarEstado = false;
            //}
            //else
            //{
            //    throw new Exception("No se puede modificar una factura en estado " + modelFacturas.Estadodescripcion);
            //}            
        }

    }
}
