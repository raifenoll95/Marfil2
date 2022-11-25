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
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
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
using RFacturas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Facturas;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public interface IFacturasService
    {

    }

    public class FacturasService : GestionService<FacturasModel, Facturas>, IDocumentosServices, IBuscarDocumento, IDocumentosVentasPorReferencia<FacturasModel>, IFacturasService
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
                ((FacturasValidation)_validationService).EjercicioId = value;
            }
        }

        #endregion


        #region CTR

        public FacturasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            _importarService = new ImportacionService(context);
        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var estadosService = new EstadosService(_context, _db);

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

            st.List = st.List.OfType<FacturasModel>().Where(s => seriesrol.Contains(s.Fkseries)).OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            var propiedadesVisibles = new[] { "Referencia", "Fechadocumento", "Fkclientes", "Nombrecliente", "Fkestados", "Importebaseimponible" };
            var propiedades = Helpers.Helper.getProperties<FacturasModel>();
            st.PrimaryColumnns = new[] { "Id" };
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            st.ColumnasCombo.Add("Fkestados", estadosService.GetStates(DocumentoEstado.FacturasVentas, TipoMovimientos.Todos).Select(f => new Tuple<string, string>(f.CampoId, f.Descripcion)));
            st.EstiloFilas.Add("Facturarectificativa", new EstiloFilas() { Estilos = new[] { new Tuple<object, string>(true, "#FCF8E3")} });

            return st;
        }

        public IEnumerable<FacturasLinModel> RecalculaLineas(IEnumerable<FacturasLinModel> model,
            double descuentopp, double descuentocomercial, string fkregimeniva, double portes, int decimalesmoneda)
        {
            var result = new List<FacturasLinModel>();

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

        public IEnumerable<FacturasTotalesModel> Recalculartotales(IEnumerable<FacturasLinModel> model, double descuentopp, double descuentocomercial, double portes, int decimalesmoneda, double porcentajeretencion = 0)
        {
            var result = new List<FacturasTotalesModel>();

            var vector = model.GroupBy(f => f.Fktiposiva);

            var fkalbaranes = model.FirstOrDefault().Fkalbaranes;

            foreach (var item in vector)
            {
                var newItem = new FacturasTotalesModel();
                var objIva = _db.TiposIva.Single(f => f.empresa == Empresa && f.id == item.Key);
                newItem.Decimalesmonedas = decimalesmoneda;
                newItem.Fktiposiva = item.Key;
                newItem.Porcentajeiva = objIva.porcentajeiva;
                newItem.Brutototal = Math.Round((item.Sum(f => (f.Metros) * (f.Precio)) - item.Sum(f => f.Importedescuento)) ?? 0, decimalesmoneda);
                newItem.Porcentajerecargoequivalencia = objIva.porcentajerecargoequivalente;
                newItem.Porcentajedescuentoprontopago = descuentopp;
                newItem.Porcentajedescuentocomercial = descuentocomercial;
                newItem.Porcentajeretencion = porcentajeretencion;
                //
                //var fkcliente = _db.Albaranes.Where(f => f.empresa == Empresa && f.id == fkalbaranes).Select(f => f.fkclientes).SingleOrDefault();
                //var fktiposretencion = _db.Clientes.Where(f => f.empresa == Empresa && f.fkcuentas == fkcliente)
                //                                        .Select(f => f.fktiposretencion).SingleOrDefault();

                //if (!string.IsNullOrEmpty(fktiposretencion))
                //{
                //    newItem.Porcentajeretencion = _db.Tiposretenciones.Where(f => f.empresa == Empresa && f.id == fktiposretencion)
                //                                                    .Select(f => f.porcentajeretencion).SingleOrDefault();
                //}
                //

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

        private void CalcularTotalesCabecera(FacturasModel model)
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

        public FacturasModel GetByReferencia(string referencia)
        {
            var obj =
                _db.Facturas.Include("FacturasLin")
                    .Include("FacturasTotales")
                    .Single(f => f.empresa == Empresa && f.referencia == referencia);

            return _converterModel.GetModelView(obj) as FacturasModel;
        }

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as FacturasModel;
                var validation = _validationService as FacturasValidation;
                validation.EjercicioId = EjercicioId;
                var tipodocumento = "FRA"; //Factura ventas 
                var contador = ServiceHelper.GetNextId<Facturas>(_db, Empresa, model.Fkseries, tipodocumento);
                var identificadorsegmento = "";
                model.Referencia = ServiceHelper.GetReference<Facturas>(_db, model.Empresa, model.Fkseries, tipodocumento, contador, model.Fechadocumento.Value, out identificadorsegmento);
                model.Identificadorsegmento = identificadorsegmento;
                DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.FacturasVentas, _context, _db);
                var newItem = _converterModel.CreatePersitance(obj);

                if (_validationService.ValidarGrabar(newItem))
                {
                    _db.Set<Facturas>().Add(newItem);
                    try
                    {

                        _db.SaveChanges();
                        obj.set("Id", newItem.id);
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

                ModificarCantidadesPedidasAlbaranes(obj as FacturasModel);

                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var validation = _validationService as FacturasValidation;
                validation.EjercicioId = EjercicioId;
                DocumentosHelpers.GenerarCarpetaAsociada(obj, TipoDocumentos.FacturasVentas, _context, _db);

                var model = obj as FacturasModel;
                // Esta línea añande a 'FacturasLin' el campo de fecha de los albaranes

                model.Pesobruto = 0;
                model.Pesoneto = 0;

                List<String> referencias = new List<String>();

                //Todas las lineas de esa factura
                foreach (var linea in model.Lineas)
                {
                    linea.Fkalbaranesfecha = _db.Albaranes.Where(f => f.empresa == Empresa && f.id == linea.Fkalbaranes).Select(f => f.fechadocumento).FirstOrDefault();

                    if (!referencias.Any(f => f == linea.Fkalbaranesreferencia))
                    {
                        var albaranesService = FService.Instance.GetService(typeof(AlbaranesModel), _context) as AlbaranesService;
                        var albaran = albaranesService.GetByReferencia(linea.Fkalbaranesreferencia);

                        model.Pesobruto += albaran.Pesobruto;
                        model.Pesoneto += albaran.Pesoneto;

                        referencias.Add(linea.Fkalbaranesreferencia);
                    }
                }

                model.Totales = Recalculartotales(model.Lineas, model.Porcentajedescuentoprontopago ?? 0, model.Porcentajedescuentocomercial ?? 0,
                                    model.Importeportes ?? 0, model.Decimalesmonedas, model.Porcentajeretencion ?? 0).ToList();

                CalcularTotalesCabecera(model);
                model.Vencimientos = RefrescarVencimientos(model, model.Fkformaspago.ToString());

                base.edit(obj);

                ModificarCantidadesPedidasAlbaranes(obj as FacturasModel);
                _db.SaveChanges();
                tran.Complete();
            }

        }

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as FacturasModel;

                base.delete(obj);
                _db.SaveChanges();
                ModificarCantidadesPedidasAlbaranes(obj as FacturasModel, TipoOperacion.Baja);

                var service = FService.Instance.GetService(typeof(AlbaranesModel), _context) as AlbaranesService;
                var _converterModel = FConverterModel.Instance.CreateConverterModelService<AlbaranesModel, Albaranes>(_context, _db, Empresa);
                var serviceEstados = new EstadosService(_context);

                var lista = model.Lineas.GroupBy(f => f.Fkalbaranes).Select(g => g.First()).ToList();
                foreach (var linea in lista)
                {
                    var modelview = _converterModel.CreateView(linea.Fkalbaranes.ToString());

                    var listEstadosInicial = serviceEstados.GetStates(DocumentoEstado.AlbaranesVentas, Model.Configuracion.TipoEstado.Diseño).Where(f => f.Tipoestado == Model.Configuracion.TipoEstado.Diseño);
                    var nuevoEstado = listEstadosInicial.Where(f => f.Documento == DocumentoEstado.AlbaranesVentas).SingleOrDefault() ?? listEstadosInicial.First();

                    service.SetEstado(modelview, nuevoEstado);
                }

                tran.Complete();

            }
        }

        #region Importar Albaran 

        public void AgregarAlbaranes(IEnumerable<string> vector, List<FacturasLinModel> lineasactuales, ref List<FacturasTotalesModel> totalesactuales, double porcentajecomercial, double porcentajeprontopago, int decimalesmonedas)
        {
            using (
                var albaranesService =
                    FService.Instance.GetService(typeof(AlbaranesModel), _context) as AlbaranesService)
            {
                GenerarLineas(albaranesService, lineasactuales, vector);

                totalesactuales = Recalculartotales(lineasactuales, porcentajeprontopago,
                   porcentajecomercial, 0,
                    decimalesmonedas).ToList();
            }

        }

        public FacturasModel ImportarAlbaranes(string serie, string fecha, IEnumerable<string> albaranesreferencia)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var result = Helper.fModel.GetModel<FacturasModel>(_context);
                using (var albaranesService = FService.Instance.GetService(typeof(AlbaranesModel), _context) as AlbaranesService)
                {

                    var albaran = albaranesService.GetByReferencia(albaranesreferencia.First());
                    result.Importado = true;               

                    ImportarCabecera(albaran, serie, fecha, result);

                    GenerarLineas(albaranesService, result.Lineas, albaranesreferencia);

                    //retención
                    result.Fktiposretenciones = _db.Clientes.Where(f => f.empresa == Empresa && f.fkcuentas == result.Fkclientes)
                                                .Select(f => f.fktiposretencion).SingleOrDefault();

                    if (!string.IsNullOrEmpty(result.Fktiposretenciones))
                    {
                        result.Porcentajeretencion = _db.Tiposretenciones.Where(f => f.empresa == Empresa && f.id == result.Fktiposretenciones)
                                                                        .Select(f => f.porcentajeretencion).SingleOrDefault();
                    }

                    result.Totales = Recalculartotales(result.Lineas, result.Porcentajedescuentoprontopago ?? 0,
                        result.Porcentajedescuentocomercial ?? 0, result.Importeportes ?? 0,
                        result.Decimalesmonedas, result.Porcentajeretencion ?? 0).ToList();

                    result.Importetotaldoc = Math.Round(result.Totales.Sum(f => f.Subtotal) ?? 0.0, result.Decimalesmonedas);
                    GenerarVencimientos(result);
                    //create(result);

                    //Cambiamos el estado del/los albarán/es
                    foreach (var item in albaranesreferencia)
                    {
                        var albaranEstado = albaranesService.GetByReferencia(item);

                        var Confservice = FService.Instance.GetService(typeof(ConfiguracionModel), _context) as ConfiguracionService;
                        albaranEstado.Fkestados = Confservice.GetEstadoFinAlbaranesVentas();

                        //Sumar pesos
                        result.Peso += albaranEstado.Peso;
                        result.Pesobruto += albaranEstado.Pesobruto;
                        result.Pesoneto += albaranEstado.Pesoneto;

                        var newItem = albaranesService._converterModel.CreatePersitance(albaranEstado);
                        _db.Set<Albaranes>().AddOrUpdate(newItem);
                    }                  

                    _db.SaveChanges();
                    tran.Complete();

                    return result;
                }
            }

        }

        private void GenerarLineas(AlbaranesService albaranesService, List<FacturasLinModel> lineas, IEnumerable<string> albaranesreferencia)
        {
            var list = albaranesreferencia.Where(f => CanImportarLinea(lineas, f));
            foreach (var item in list)
            {
                GenerarLinea(albaranesService, lineas, item);
            }
        }

        private bool CanImportarLinea(List<FacturasLinModel> lineas, string referencia)
        {
            //Todo revisar esto
            return true;// lineas.All(f => f.Fkalbaranesreferencia != referencia);
        }

        private void GenerarLinea(AlbaranesService albaranesService, List<FacturasLinModel> lineas, string referencia)
        {
            var albaran = albaranesService.GetByReferencia(referencia);
            albaran = albaranesService.get(albaran.Id.ToString()) as AlbaranesModel;
            //if (albaran.Lineas.Any(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0) > 0)) - ANG - se quita la comprobación de cantidad pedida, el albaran siempre se factura entero
            if (albaran.Lineas.Any(f => (f.Cantidad ?? 0) != 0))
            {
                var maxId = lineas.Any() ? lineas.Max(f => f.Id) + 1 : 1;
                lineas.AddRange(ImportarLineas(albaran.Id, maxId, albaranesService.GetLineasImportarAlbaran(referencia)));
            }

            if (lineas.Count == 0)
                throw new ValidationException("Los albaranes seleccionados no han generado ninguna linea");
        }

        public IEnumerable<FacturasLinModel> ImportarLineas(int albaranId, int maxId, IEnumerable<ILineaImportar> linea)
        {

            return linea.Select(f => ConvertILineaImportarToPedidoLinModel(++maxId, f)).OrderBy(f => f.Orden);
        }



        private FacturasLinModel ConvertILineaImportarToPedidoLinModel(int idlinea, ILineaImportar linea)
        {
            var serviceEmpresa = FService.Instance.GetService(typeof(EmpresaModel), _context);
            var empresa = serviceEmpresa.get(_context.Empresa) as EmpresaModel;

            var idalbaran = Funciones.Qint(linea.Fkdocumento);

            var albaran =
                _db.Albaranes.SingleOrDefault(
                    f =>
                        f.empresa == Empresa && f.id == idalbaran);
            var metros = Math.Round(linea.Metros, linea.Decimalesmedidas);
            var precio = Math.Round(linea.Precio, empresa.Decimalesprecios ?? 2);
            var bruto = metros * precio;
            var cuotadescuento = Math.Round(bruto * linea.Porcentajedescuento / 100.0, linea.Decimalesmonedas);
            var baseimpo = bruto - cuotadescuento;
            return new FacturasLinModel()
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
                Orden = linea.Orden ?? 0,
                Contenedor = linea.Contenedor,
                Sello = linea.Sello,
                Caja = linea.Caja,
                Pesoneto = linea.Pesoneto
            };

        }


        private void ImportarCabecera(AlbaranesModel presupuesto, string serie, string fecha, FacturasModel result)
        {
            var properties = typeof(FacturasModel).GetProperties();
            foreach (var item in properties)
            {
                //Nov2022 - Tampoco tenemos en cuenta los peso porque puede haber más de un albarán y aquí solo tomaría el primero, se calculan en ImportarAlbaranes
                if (item.Name != "Lineas" && item.Name != "Totales" && item.Name != "Peso" && item.Name != "Pesoneto" && item.Name != "Pesobruto")
                {
                    var property = typeof(AlbaranesModel).GetProperty(item.Name);
                    if (property != null && property.CanWrite)
                    {
                        var value = property.GetValue(presupuesto);
                        item.SetValue(result, value);
                    }
                }
            }

            result.Fechadocumento = DateTime.Parse(fecha);

            result.Fkseries = serie;

            //asignamos el tipo de factura según el cliente
            var tipofactura = _db.Clientes.Where(f => f.empresa == Empresa && f.fkcuentas == presupuesto.Fkclientes).FirstOrDefault().fktipofactura;
            if (tipofactura != null)
            {
                result.Fktipofactura = tipofactura;
            }

            //Nov2022 - Inicializamos el peso para con no sea null y poder calcularlo
            result.Pesobruto = 0;
            result.Pesoneto = 0;

            var appService = new ApplicationHelper(_context);
            result.Fkestados = appService.GetConfiguracion().Estadofacturasventasinicial;
            var decimalesmonedas = _db.Monedas.Single(f => f.id == result.Fkmonedas.Value).decimales;
            result.Decimalesmonedas = decimalesmonedas.Value;
            result.Fkmotivorectificacion = presupuesto.Fkmotivosdevolucion;
            result.Fkzonas = presupuesto.Fkzonas;
        }

        public List<FacturasVencimientosModel> RefrescarVencimientos(FacturasModel cabecera, string idformapago)
        {

            cabecera.Fkformaspago = Funciones.Qint(idformapago);
            return GenerarLineasVencimientos(cabecera);
        }

        private void GenerarVencimientos(FacturasModel cabecera)
        {
            cabecera.Vencimientos = GenerarLineasVencimientos(cabecera);
        }

        private List<FacturasVencimientosModel> GenerarLineasVencimientos(FacturasModel cabecera)
        {

            var result = new List<FacturasVencimientosModel>();
            var formapagoService = FService.Instance.GetService(typeof(FormasPagoModel), _context) as FormasPagoService;
            var formapagoObj = formapagoService.get(cabecera.Fkformaspago.ToString()) as FormasPagoModel;
            var clienteService = FService.Instance.GetService(typeof(ClientesModel), _context) as ClientesService;
            var clienteObj = clienteService.get(cabecera.Fkclientes) as ClientesModel;
            var monedasService = FService.Instance.GetService(typeof(MonedasModel), _context) as MonedasService;
            var monedasObj = monedasService.get(cabecera.Fkmonedas.ToString()) as MonedasModel;
            var diafijo1 = clienteObj.Diafijopago1;
            var diafijo2 = clienteObj.Diafijopago2;
            var decimales = monedasObj.Decimales;
            var maxId = result.Any() ? result.Max(f => f.Id) + 1 : 1;
            var list = formapagoObj.Lineas.ToList();

            var acumulado = 0.0;
            var totaldocumento = cabecera.Importetotaldoc.Value;
            for (var i = 0; i < list.Count(); i++)
            {
                var cuotavencimiento = i >= list.Count() - 1 ? totaldocumento - acumulado : Math.Round((totaldocumento * list[i].PorcentajePago) / 100.0, decimales);
                acumulado += cuotavencimiento;
                result.Add(new FacturasVencimientosModel()
                {
                    Id = maxId++,
                    Diasvencimiento = list[i].DiasVencimiento,
                    Fechavencimiento = GetFechavencimiento(cabecera.Fechadocumento.Value, list[i].DiasVencimiento, diafijo1, diafijo2, formapagoObj.ExcluirFestivos),
                    Importevencimiento = cuotavencimiento,
                    Decimalesmonedas = decimales
                });
            }

            return result;
        }

        public static DateTime GetFechavencimiento(DateTime fechafactura, int diavencimiento, int diapago1, int diapago2, bool excluirfestivos)
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

        public void GetVencimiento(FacturasModel cabecera, List<FacturasVencimientosModel> lineas, FacturasVencimientosModel item)
        {
            var formapagoService = FService.Instance.GetService(typeof(FormasPagoModel), _context) as FormasPagoService;
            var formapagoObj = formapagoService.get(cabecera.Fkformaspago.ToString()) as FormasPagoModel;
            var clienteService = FService.Instance.GetService(typeof(ClientesModel), _context) as ClientesService;
            var clienteObj = clienteService.get(cabecera.Fkclientes) as ClientesModel;
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

        //public void GetVencimientoId(FacturasModel cabecera,    FacturasVencimientosModel item)
        //{
        //    var formapagoService = FService.Instance.GetService(typeof(FormasPagoModel), _context) as FormasPagoService;
        //    var formapagoObj = formapagoService.get(cabecera.Fkformaspago.ToString()) as FormasPagoModel;

        //    var clienteService = FService.Instance.GetService(typeof(ClientesModel), _context) as ClientesService;

        //    var clienteObj = clienteService.get(cabecera.Fkclientes) as ClientesModel;
        //    var monedasService = FService.Instance.GetService(typeof(MonedasModel), _context) as MonedasService;
        //    var monedasObj = monedasService.get(cabecera.Fkmonedas.ToString()) as MonedasModel;

        //    var diafijo1 = clienteObj.Diafijopago1;
        //    var diafijo2 = clienteObj.Diafijopago2;
        //    var fechaFactura = cabecera.Fechadocumento;
        //    var decimales = monedasObj.Decimales;

        //    item.Fechavencimiento = GetFechavencimiento(fechaFactura.Value, item.Diasvencimiento.Value, diafijo1,
        //        diafijo2, formapagoObj.ExcluirFestivos);
        //    item.Importevencimiento = Math.Round(item.Importevencimiento.Value, decimales);
        //    item.Decimalesmonedas = monedasObj.Decimales;
        //}

        #endregion

        #region Helper

        private void ModificarCantidadesPedidasAlbaranes(FacturasModel model, TipoOperacion tipo = TipoOperacion.Editar)
        {
            var AlbaranesService = new AlbaranesService(_context, _db);
            AlbaranesService.EjercicioId = EjercicioId;

            var lista = model.Lineas.GroupBy(f => f.Fkalbaranes).Select(g => g.First()).ToList();

            foreach (var item in lista)
            {
                var albaran = _db.Albaranes.Include("AlbaranesLin").Single(
                    f =>
                        f.empresa == model.Empresa && f.id == item.Fkalbaranes);

                foreach (var linea in albaran.AlbaranesLin)
                {

                    linea.cantidadpedida = tipo == TipoOperacion.Baja ? 0 : linea.cantidad;

                }

                var validationService = AlbaranesService._validationService as AlbaranesValidation;
                validationService.EjercicioId = EjercicioId;
                validationService.FlagActualizarCantidadesFacturadas = true;
                validationService.ValidarGrabar(albaran);
                _db.Albaranes.AddOrUpdate(albaran);
            }
            _db.SaveChanges();
        }

        #endregion

        public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        {
            var currentValidationService = _validationService as FacturasValidation;
            currentValidationService.CambiarEstado = true;
            model.set("fkestados", nuevoEstado.CampoId);
            edit(model);
            currentValidationService.CambiarEstado = false;
        }

        //public void SetEstado(IModelView model, EstadosModel nuevoEstado)
        //{
        //    throw new NotImplementedException();
        //}


        public IEnumerable<DocumentosBusqueda> Buscar(IDocumentosFiltros filtros, out int registrostotales)
        {
            var service = new BuscarDocumentosService(_db, Empresa);
            return service.Buscar(filtros, out registrostotales);
        }

        public IEnumerable<IItemResultadoMovile> BuscarDocumento(string referencia)
        {
            var service = new BuscarDocumentosService(_db, Empresa);
            return service.Get<FacturasModel, FacturasLinModel, FacturasTotalesModel>(this, referencia);
        }


        //Rai -- Me devuelve las facturas a partir de un cliente -- Necesario para las facturas rectificativas
        public IEnumerable<FacturasModel> getDocumentosRelacionados(string cliente)
        {
            return _db.Database.SqlQuery<FacturasModel>(string.Format(
            "SELECT f.id as Id, f.referencia as Referencia, FORMAT(f.fechadocumento, 'dd/MM/yyyy') as Fecha, f.fkclientes as Fkclientes, f.fktipofactura as Fktipofactura, f.importebaseimponible as Importebaseimponible, f.importetotaldoc as Importetotaldoc "
            + " FROM Facturas as f "
            + " WHERE f.empresa = '" + Empresa + "' AND"
            + " f.fkclientes = '" + cliente + "'"));
        }

        public bool? Facturarectificativa(FacturasModel model)
        {
            return _db.Series.Where(f => f.empresa == Empresa && f.tipodocumento == "FRA" && f.id == model.Fkseries).Select(f => f.rectificativa).SingleOrDefault();
        }

    }
}
