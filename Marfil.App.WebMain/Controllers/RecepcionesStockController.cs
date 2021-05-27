using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.StateMachine;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;
using Articulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using RAlbaranes= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using RRecepcion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.RecepcionStock;
using System.Text.RegularExpressions;
using RFamilias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;

namespace Marfil.App.WebMain.Controllers
{


    public class RecepcionesStockController : GenericController<RecepcionesStockModel>
    {
        private const string session = "_recepcionstocklin_";
        private const string sessiontotales = "_recepcionstocktotales_";
        private const string sessioncostes = "_recepcionstockcostes";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "recepcionesstock";
            var permisos = appService.GetPermisosMenu("recepcionesstock");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public RecepcionesStockController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        public ActionResult Importar(string id, string returnUrl)
        {
            var model = Helper.fModel.GetModel<RecepcionesStockModel>(ContextService);
            try
            {
                using (var service = createService(model) as RecepcionStockService)
                {
                    using (var pedidosService = FService.Instance.GetService(typeof(PedidosComprasModel), ContextService) as PedidosComprasService)
                    {
                        model = new RecepcionesStockModel(service.ImportarPedido(pedidosService.get(id) as PedidosComprasModel));

                    }

                }
            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;
                return Redirect(returnUrl);
            }
            model.Context = ContextService;
            TempData["model"] = model;
            return RedirectToAction("Create");
        }

        #region Importar linea

        [HttpPost]
        public ActionResult ImportarLineas(string lineas, string descuentocomercial, string descuentopp)
        {

            if (!string.IsNullOrEmpty(lineas))
            {
                var data = JsonConvert.DeserializeObject(lineas);
                var model = Session[session] as List<AlbaranesComprasLinModel>;
                var list = data is JArray ? data as JArray : new JArray(new[] { data });
                var decimales = 2;
                var portes = 0;
                var RecepcionStockService = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService;
                var maxId = model.Any() ? model.Max(f => f.Id) : 0;
                model.AddRange(RecepcionStockService.ImportarLineas(maxId, list.Select(CrearLineaImportar)));

                var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService;
                Session[session] = model;
                Session[sessiontotales] = service.Recalculartotales(model, Funciones.Qdouble(descuentopp) ?? 0, Funciones.Qdouble(descuentocomercial) ?? 0, portes, decimales);
            }

            return new EmptyResult();
        }

        private LineaImportarModel CrearLineaImportar(JToken item)
        {
            var result = new LineaImportarModel();

            result.Id = item.Value<int>("Id");
            result.Decimalesmedidas = item.Value<int?>("Decimalesmedidas") ?? 0;
            result.Decimalesmonedas = item.Value<int?>("Decimalesmonedas") ?? 0;
            result.Ancho = item.Value<double?>("Ancho") ?? 0;
            result.Largo = item.Value<double?>("Largo") ?? 0;
            result.Grueso = item.Value<double?>("Grueso") ?? 0;
            result.Canal = item.Value<string>("Canal");
            result.Cantidad = item.Value<double?>("Cantidad") ?? 0;

            result.Fkarticulos = item.Value<string>("Fkarticulos");
            result.Descripcion = item.Value<string>("Descripcion");
            result.Metros = item.Value<double?>("Metros") ?? 0;
            result.Cuotaiva = item.Value<double?>("Cuotaiva") ?? 0;
            result.Cuotarecargoequivalencia = item.Value<double?>("Cuotarecargoequivalencia") ?? 0;
            result.Fktiposiva = item.Value<string>("Fktiposiva");
            result.Porcentajeiva = item.Value<double?>("Porcentajeiva") ?? 0;
            result.Porcentajerecargoequivalencia = item.Value<double?>("Porcentajerecargoequivalencia") ?? 0;
            result.Fkunidades = item.Value<string>("Fkunidades");
            result.Importe = Math.Round(item.Value<double?>("Importe") ?? 0, result.Decimalesmonedas);
            result.Importedescuento = item.Value<double?>("Importedescuento") ?? 0;
            result.Lote = item.Value<string>("Lote");
            result.Precio = Math.Round(item.Value<double?>("Precio") ?? 0, result.Decimalesmonedas);
            result.Precioanterior = item.Value<double?>("Precioanterior") ?? 0;
            result.Porcentajedescuento = item.Value<double?>("Porcentajedescuento") ?? 0;
            result.Tabla = item.Value<int?>("Tabla") ?? 0;
            result.Bundle = item.Value<string>("Bundle");
            result.Revision = "";
            result.Fkdocumento = item.Value<string>("Fkpedidos");
            result.Fkdocumentoid = item.Value<int?>("Id")?.ToString() ?? "";
            result.Fkdocumentoreferencia = item.Value<string>("Fkpedidosreferencia");

            return result;
        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService))
            {
                var model = TempData["model"] as RecepcionesStockModel ?? Helper.fModel.GetModel<RecepcionesStockModel>(ContextService);
                Session[session] = model.Lineas;
                Session[sessiontotales] = model.Totales;
                Session[sessioncostes] = model.Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }






        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(RecepcionesStockModel model)
        {
            try
            {
                model.Context = ContextService;
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<RecepcionesStockModel>(ContextService);

                model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
                model.Lineas = Session[session] as List<AlbaranesComprasLinModel>;
                model.Totales = Session[sessiontotales] as List<AlbaranesComprasTotalesModel>;
                model.Costes = Session[sessioncostes] as List<AlbaranesComprasCostesadicionalesModel>;
                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {

                        gestionService.create(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }


                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
            catch (IntegridadReferencialException ex2)
            {
                TempData["errors"] = ex2.Message;
                return RedirectToAction("Edit", new { id = model.Id });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
        }

        public override ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newModel = Helper.fModel.GetModel<RecepcionesStockModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as RecepcionesStockModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((AlbaranesComprasModel)model).Lineas;
                Session[sessiontotales] = ((AlbaranesComprasModel)model).Totales;
                Session[sessioncostes] = ((AlbaranesComprasModel)model).Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(RecepcionesStockModel model)
        {
            model.Context = ContextService;
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            var fmodel = new FModel();
            var newmodel = fmodel.GetModel<RecepcionesStockModel>(ContextService);
            model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
            model.Lineas = Session[session] as List<AlbaranesComprasLinModel>;
            model.Totales = Session[sessiontotales] as List<AlbaranesComprasTotalesModel>;
            model.Costes = Session[sessioncostes] as List<AlbaranesComprasCostesadicionalesModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {


                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }

                model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = model.Id });
            }
            catch (IntegridadReferencialException ex2)
            {
                TempData["errors"] = ex2.Message;
                return RedirectToAction("Edit", new { id = model.Id });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = model.Id });
            }

        }

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<RecepcionesStockModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((AlbaranesComprasModel)model).Lineas;
                Session[sessiontotales] = ((AlbaranesComprasModel)model).Totales;
                Session[sessioncostes] = ((AlbaranesComprasModel)model).Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }

        public ActionResult Clonar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var newModel = Helper.fModel.GetModel<RecepcionesStockModel>(ContextService);
            using (var gestionService = createService(newModel) as RecepcionStockService)
            {
                try
                {

                    var cloned = gestionService.Clonar(id);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoClonar;
                    return RedirectToAction("Edit", new { id = cloned.Id });
                }
                catch (Exception ex)
                {
                    TempData["errors"] = ex.Message;
                    return RedirectToAction("Edit", new { id = id });
                }

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Devolucion(string id, string lineas)
        {
            if (!string.IsNullOrEmpty(lineas))
            {
                using (var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService)
                {
                    var model = service.get(id) as AlbaranesComprasModel;
                    model.Id = 0;
                    model.Tipoalbaran = (int)TipoAlbaran.Devolucion;
                    model.Fechadocumento = DateTime.Now;
                    model.Fkmotivosdevolucion = appService.GetListMotivosDevolucion().FirstOrDefault()?.Valor;
                    model.Fkestados = appService.GetConfiguracion()?.Estadoalbaranescomprasinicial ?? string.Empty;
                    var data = JsonConvert.DeserializeObject(lineas);

                    var list = data is JArray ? data as JArray : new JArray(new[] { data });

                    var maxId = model.Lineas.Any() ? model.Lineas.Max(f => f.Id) : 0;
                    model.Lineas.Clear();

                    var vector = list.Select(CrearLineaImportar).ToList();
                    foreach (var item in vector)
                    {
                        item.Cantidad *= -1;
                    }
                    model.Lineas.AddRange(service.ImportarLineas(maxId, vector));
                    foreach (var item in model.Lineas)
                    {
                        item.Nueva = true;
                    }
                    var newmodel = Helper.fModel.GetModel<RecepcionesStockModel>(ContextService);
                    model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
                    model.Totales = service.Recalculartotales(model.Lineas, model.Porcentajedescuentoprontopago ?? 0, model.Porcentajedescuentocomercial ?? 0, 0, model.Decimalesmonedas).ToList();
                    var modelDevolucion=new RecepcionesStockModel(model);
                    TempData["model"] = modelDevolucion;
                    return RedirectToAction("Create");
                }
            }

            TempData["errors"] = "Ocurrió un problema al generar el albarán de devolución";
            return RedirectToAction("Edit", new { id = id });
        }

        public ActionResult getLineasDuplicadas(string id, string lineas)
        {
            using (var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService)
            {
                var model = service.get(id) as AlbaranesComprasModel;
                var data = JsonConvert.DeserializeObject(lineas);
                var list = data is JArray ? data as JArray : new JArray(new[] { data });

                var maxId = model.Lineas.Any() ? model.Lineas.Max(f => f.Id) : 0;
                model.Lineas.Clear();

                var positivas = list.Select(CrearLineaImportar).ToList();
                var negativas = list.Select(CrearLineaImportar).ToList();

                foreach (var item in negativas)
                {
                    item.Cantidad *= -1;
                }

                model.Lineas.AddRange(service.ImportarLineas(maxId, positivas));
                model.Lineas.AddRange(service.ImportarLineas(maxId, negativas));

                var dataModel = JsonConvert.SerializeObject(model, Formatting.Indented);
                return Json(dataModel, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reclamar(string id, string lineas)
        {
            if (!string.IsNullOrEmpty(lineas))
            {
                using (var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService)
                {
                    var model = service.get(id) as AlbaranesComprasModel;
                    model.Tipoalbaran = (int)TipoAlbaran.Reclamacion;
                    model.Fechadocumento = DateTime.Now;
                    model.Fkestados = appService.GetConfiguracion()?.Estadoalbaranescomprasinicial ?? string.Empty;
                    model.Fkmotivosdevolucion = appService.GetListMotivosDevolucion().FirstOrDefault()?.Valor;
                    var maxId = model.Lineas.Any() ? model.Lineas.Max(f => f.Id) : 0;
                    var data = JsonConvert.DeserializeObject(lineas);
                    var list = data is JArray ? data as JArray : new JArray(new[] { data });
                    var lineasimputadas = list.Select(CrearLineaImportar).ToList(); //Lineas de clase "Importar"

                    var newmodel = Helper.fModel.GetModel<RecepcionesStockModel>(ContextService);
                    model.Lineas.Clear();
                    model.Lineas.AddRange(service.ImportarLineasReclamadas(lineasimputadas,maxId).OrderBy(f => f.Lote).ThenBy(f => f.Tabla).ThenBy(f => f.Cantidad));
                    
                    foreach(var linea in model.Lineas)
                    {
                        linea.Fkreclamado = Int32.Parse(id);
                        linea.Fkreclamadoreferencia = model.Referencia;
                    }

                    model.idOriginalReclamado = Int32.Parse(id);
                    model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
                    model.Totales = service.Recalculartotales(model.Lineas, model.Porcentajedescuentoprontopago ?? 0, model.Porcentajedescuentocomercial ?? 0, 0, model.Decimalesmonedas).ToList();
                    var modelDevolucion = new RecepcionesStockModel(model);
                    TempData["model"] = modelDevolucion;
                    return RedirectToAction("Create");
                }
            }

            TempData["errors"] = "Ocurrió un problema al generar el albarán de reclamación";
            return RedirectToAction("Edit", new { id = id });
        }

        public ActionResult ForzarCambiarCostes(string documentoReferencia, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService)
                {
                    service.EjercicioId = ContextService.Ejercicio;
                    var model = service.get(documentoReferencia) as AlbaranesComprasModel;
                    model.Costes = Session[sessioncostes] as List<AlbaranesComprasCostesadicionalesModel>;
                    service.ModificarCostes(model);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                }
            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;
            }
            return Redirect(returnUrl);
        }

        public ActionResult CambiarEstado(string documentoReferencia, string estadoNuevo, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService)
                {

                    service.EjercicioId = ContextService.Ejercicio;
                    using (var estadosService = new EstadosService(ContextService))
                    {
                        var model = service.get(documentoReferencia) as IDocument;
                        var nuevoEstado = estadosService.get(estadoNuevo) as EstadosModel;
                        var cambiarEstadoService = new MachineStateService();
                        cambiarEstadoService.SetState(service, model, nuevoEstado);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;
            }
            return Redirect(returnUrl);
        }

        public ActionResult AsistenteRemedirRecepcionStock(string id)
        {
            var model = new AsistenteRemedirStockModel(ContextService);
            model.Desderecepcionstock = id;
            model.Hastarecepcionstock = id;

            model.Toolbar.Acciones = HelpItem();
            return View("AsistenteRemedir", model);
        }

        public ActionResult AsistenteRemedir()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var model = new AsistenteRemedirStockModel(ContextService);
            model.Toolbar.Acciones = HelpItem();
            return View(model);
        }

        [HttpPost]
        public ActionResult Remedir(StRemedir model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (
                        var service =
                            FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as
                                RecepcionStockService)
                    {
                        service.RemedirLotes(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                    }
                }
                else
                    TempData["errors"] = string.Join(",", ModelState.Values.Where(f => f.Errors.Any()).Select(f => string.Join(",", f.Errors.Select(j => j.ErrorMessage))));

            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;
            }
            return RedirectToAction("AsistenteRemedir");
        }

        #endregion

        #region Saldar Pedidos

        public ActionResult GetLineasSaldarPedido(string id)
        {
            var model = Helper.fModel.GetModel<RecepcionesStockModel>(ContextService);
            using (var service = createService(model) as RecepcionStockService)
            {

                var result = service.GetLineasSaldarPedidos(id, Session[session] as List<AlbaranesComprasLinModel>);

                return Content(JsonConvert.SerializeObject(result), "application/json");


            }
        }

        [HttpPost]
        public ActionResult SaldarPedido(OperacionSaldarPedidosModel model)
        {
            try
            {
                var vmodel = Helper.fModel.GetModel<RecepcionesStockModel>(ContextService);
                using (var service = createService(vmodel) as RecepcionStockService)
                {
                    var entregaReferencia = service.GetByReferencia(model.Referenciaentrega);
                    var entrega = service.get(entregaReferencia.Id.ToString()) as AlbaranesComprasModel;
                    entrega.Lineas = Session[session] as List<AlbaranesComprasLinModel>;
                    entrega.Totales = Session[sessiontotales] as List<AlbaranesComprasTotalesModel>;
                    service.SaldarPedidos(model, entrega);

                    return Content("");


                }
            }
            catch (Exception ex)
            {
                return Content(JsonConvert.SerializeObject(new ErrorModel() { Error = ex.Message }), "application/json");
            }

            throw new Exception("ups! algo no fue bien");
        }

        #endregion

        #region Grid Devexpress

        public struct StContadoresLotes
        {
            public string IdLote { get; set; }
            public string Texto { get; set; }
        }

        [HttpPost]
        public ActionResult NuevaLinea(AlbaranesComprasLinVistaModel model)
        {
            var errormessage = "";
            try
            {
                var listado = Session[session] as List<AlbaranesComprasLinModel> ?? new List<AlbaranesComprasLinModel>();
                using (var RecepcionStockService = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService)
                {
                    // Validar dimensiones artículo
                    var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), ContextService) as FamiliasproductosService;
                    familiasProductosService.ValidarDimensiones(model.Fkarticulos, model.Largo, model.Ancho, model.Grueso);
                    
                    var lineas = RecepcionStockService.CrearNuevasLineas(listado, model);
                    var maxid = listado.Any() ? listado.Max(f => f.Id) : 0;
                    foreach (var item in lineas)
                        item.Id = ++maxid;

                    listado.AddRange(lineas);
                    Session[session] = listado;
                    var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService;
                    Session[sessiontotales] = service.Recalculartotales(listado, Funciones.Qdouble(model.Descuentoprontopago) ?? 0, Funciones.Qdouble(model.Descuentocomercial) ?? 0, Funciones.Qdouble(model.Portes) ?? 0, model.Decimalesmonedas);
                }

                return Content(JsonConvert.SerializeObject(model), "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                errormessage = ex.Message?.Replace(Environment.NewLine, "\\n");
            }
            
            return Content("{\"error\":\"" + errormessage + "\"}", "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult GetLotesAutomaticos(string fkarticulo)
        {
            var errormessage = "";
            try
            {
                var listado = Session[session] as List<AlbaranesComprasLinModel>;

                return Content(JsonConvert.SerializeObject(listado.Where(f => f.Nueva && !string.IsNullOrEmpty(f.Loteautomaticoid) && ArticulosService.GetCodigoFamilia(f.Fkarticulos) == ArticulosService.GetCodigoFamilia(fkarticulo) && ArticulosService.GetCodigoMaterial(f.Fkarticulos) == ArticulosService.GetCodigoMaterial(fkarticulo)).GroupBy(f => f.Loteautomaticoid).Select(f => new StContadoresLotes() { IdLote = f.Key, Texto = listado.First(j => j.Loteautomaticoid == f.Key).Lote }).ToList()), "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
                Response.StatusCode = 500;
            }

            return Content(errormessage);
        }

        public ActionResult CustomGridViewEditingPartial(string key, string buttonid)
        {
            var model = Session[session] as List<AlbaranesComprasLinModel>;

            if (buttonid.Equals("btnSplit"))
            {
                var linea = model.Single(f => f.Id == Funciones.Qint(key));
                if (linea.Cantidad > 1)
                {
                    ViewData["key"] = key;
                    ViewData["buttonid"] = buttonid;
                }
            }
            else
            {
                ViewData["key"] = key;
                ViewData["buttonid"] = buttonid;
            }

            return PartialView("_AlbaranesCompraslin", model);
        }

        [ValidateInput(false)]
        public ActionResult AlbaranesComprasLin()
        {
            var model = Session[session] as List<AlbaranesComprasLinModel>;
            return PartialView("_AlbaranesCompraslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AlbaranesComprasLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] AlbaranesComprasLinModel item)
        {
            var model = Session[session] as List<AlbaranesComprasLinModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    var max = model.Any() ? model.Max(f => f.Id) : 0;
                    item.Id = max + 1;

                    var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);

                    var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
                    var serviceArticulos = FService.Instance.GetService(typeof(ArticulosModel), ContextService);
                    var serviceEmpresa = FService.Instance.GetService(typeof(EmpresaModel), ContextService);
                    var empresa = serviceEmpresa.get(ContextService.Empresa) as EmpresaModel;

                    if (serviceArticulos.exists(item.Fkarticulos))
                    {
                        var configuracionAplicacion = appService.GetConfiguracion();
                        if (configuracionAplicacion.VentasUsarCanal && configuracionAplicacion.VentasCanalObligatorio && string.IsNullOrEmpty(item.Canal))
                        {
                            ModelState.AddModelError("Canal", string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Canal));
                        }
                        else
                        {
                            var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
                            var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
                            var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
                            var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
                            var portes = 0;
                            if (item.Lineaasociada.HasValue)
                            {
                                var linea = model.SingleOrDefault(f => f.Id == item.Lineaasociada.Value);

                                var cantidadRestante = linea.Cantidad - item.Cantidad;
                                if (cantidadRestante <= 0)
                                {
                                    ModelState.AddModelError("Fkarticulos", RAlbaranesCompras.ErrorDividirLineaCantidadCero);
                                    return PartialView("_AlbaranesCompraslin", model);
                                }

                                linea.Cantidad = cantidadRestante;
                                using (var albaranesService = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService)
                                {
                                    var nuevalinea = albaranesService.ImportarLineas(0, new[] { new LineaImportarModel()
                                {
                                    Fkarticulos = linea.Fkarticulos,
                                    Fkunidades = linea.Fkunidades,
                                    Ancho = linea.Ancho.Value,Cantidad = linea.Cantidad.Value,Largo = linea.Largo.Value,Grueso=linea.Grueso.Value,Metros=linea.Metros.Value,Precio = linea.Precio.Value,Porcentajedescuento = linea.Porcentajedescuento.Value
                                }}).First();

                                    linea.Metros = nuevalinea.Metros;
                                    linea.Importe = nuevalinea.Importe;
                                    linea.Importedescuento = nuevalinea.Importedescuento;
                                    item.Fkpedidos = linea.Fkpedidos;
                                    item.Fkpedidosid = linea.Fkpedidosid;
                                    item.Fkpedidosreferencia = linea.Fkpedidosreferencia;
                                    item.Orden = linea.Id * ApplicationHelper.EspacioOrdenLineas;
                                }


                            }
                            item.Importe = Math.Round(item.Importe ?? 0, (item.Decimalesmonedas ?? 0));
                            item.Precio = Math.Round(item.Precio ?? 0, empresa.Decimalesprecios ?? 2);
                            item.Decimalesmedidas = decimalesunidades ?? 0;
                            item.Revision = item.Revision?.ToUpper();
                            item.Bundle = item.Bundle?.ToUpper();

                            // Validar dimensiones artículo
                            try
                            {
                                var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), ContextService) as FamiliasproductosService;
                                familiasProductosService.ValidarDimensiones(item.Fkarticulos, item.Largo, item.Ancho, item.Grueso);
                            }
                            catch (ValidationException ex)
                            {
                                Regex rgx = new Regex(@"\{.*\}");

                                if (Regex.IsMatch(ex.Message, rgx.Replace(RFamilias.ErrorLargo, ".*") + ".*"))
                                    ModelState.AddModelError("SLargo", Regex.Match(ex.Message, rgx.Replace(RFamilias.ErrorLargo, ".*")).Value);
                                if (Regex.IsMatch(ex.Message, ".*" + rgx.Replace(RFamilias.ErrorAncho, ".*") + ".*"))
                                    ModelState.AddModelError("SAncho", Regex.Match(ex.Message, rgx.Replace(RFamilias.ErrorAncho, ".*")).Value);
                                if (Regex.IsMatch(ex.Message, ".*" + rgx.Replace(RFamilias.ErrorGrueso, ".*") + "."))
                                    ModelState.AddModelError("SGrueso", Regex.Match(ex.Message, rgx.Replace(RFamilias.ErrorGrueso, ".*")).Value);

                                return PartialView("_AlbaranesCompraslin", model);
                            }

                            model.Add(item);

                            Session[session] = model;
                            var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService;
                            Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales);
                        }

                    }
                    else
                        ModelState.AddModelError("Fkarticulos", Articulos.ErrorArticuloInexistente);


                }
            }
            catch (ValidationException)
            {
                model.Remove(item);
                throw;
            }



            return PartialView("_AlbaranesCompraslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AlbaranesComprasLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] AlbaranesComprasLinModel item)
        {
            var model = Session[session] as List<AlbaranesComprasLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var configuracionAplicacion = appService.GetConfiguracion();
                    if (configuracionAplicacion.VentasUsarCanal && configuracionAplicacion.VentasCanalObligatorio &&
                        string.IsNullOrEmpty(item.Canal))
                    {
                        ModelState.AddModelError("Canal",
                            string.Format(General.ErrorCampoObligatorio, RAlbaranesCompras.Canal));
                    }
                    else
                    {
                        var editItem = model.Single(f => f.Id == item.Id);
                        var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);
                        var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
                        var decimalesmonedas = Funciones.Qint(Request.Params["decimalesmonedas"]);
                        var serviceEmpresa = FService.Instance.GetService(typeof(EmpresaModel), ContextService);
                        var empresa = serviceEmpresa.get(ContextService.Empresa) as EmpresaModel;

                        var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
                        var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
                        editItem.Decimalesmedidas = decimalesunidades ?? 0;
                        editItem.Decimalesmonedas = decimalesmonedas ?? 0;
                        editItem.Ancho = item.Ancho;
                        editItem.Largo = item.Largo;
                        editItem.Grueso = item.Grueso;
                        editItem.Canal = item.Canal;
                        editItem.Cantidad = item.Cantidad;
                        editItem.Cantidadpedida = item.Cantidadpedida;
                        editItem.Fkarticulos = item.Fkarticulos;
                        editItem.Descripcion = item.Descripcion;
                        editItem.Metros = item.Metros;
                        editItem.Cuotaiva = item.Cuotaiva;
                        editItem.Cuotarecargoequivalencia = item.Cuotarecargoequivalencia;
                        editItem.Fktiposiva = item.Fktiposiva;
                        editItem.Porcentajeiva = item.Porcentajeiva;
                        editItem.Porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia;
                        //editItem.Fkunidades = item.Fkunidades;
                        editItem.Importe = Math.Round(item.Importe ?? 0, (editItem.Decimalesmonedas ?? 0));
                        editItem.Importedescuento = item.Importedescuento;
                        editItem.Lote = item.Lote;
                        editItem.Precio = Math.Round(item.Precio ?? 0, empresa.Decimalesprecios ?? 2);
                        editItem.Precioanterior = item.Precioanterior;
                        editItem.Porcentajedescuento = item.Porcentajedescuento;
                        editItem.Tabla = item.Tabla;
                        editItem.Revision = item.Revision?.ToUpper();
                        editItem.Caja = item.Caja;
                        editItem.Bundle = item.Bundle?.ToUpper();
                        editItem.Orden = item.Orden;
                        editItem.Flagidentifier = Guid.NewGuid();

                        // Validar dimensiones artículo
                        try
                        {
                            var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), ContextService) as FamiliasproductosService;
                            familiasProductosService.ValidarDimensiones(item.Fkarticulos, item.Largo, item.Ancho, item.Grueso);
                        }
                        catch (ValidationException ex)
                        {
                            Regex rgx = new Regex(@"\{.*\}");

                            if (Regex.IsMatch(ex.Message, rgx.Replace(RFamilias.ErrorLargo, ".*") + ".*"))
                                ModelState.AddModelError("SLargo", Regex.Match(ex.Message, rgx.Replace(RFamilias.ErrorLargo, ".*")).Value);
                            if (Regex.IsMatch(ex.Message, ".*" + rgx.Replace(RFamilias.ErrorAncho, ".*") + ".*"))
                                ModelState.AddModelError("SAncho", Regex.Match(ex.Message, rgx.Replace(RFamilias.ErrorAncho, ".*")).Value);
                            if (Regex.IsMatch(ex.Message, ".*" + rgx.Replace(RFamilias.ErrorGrueso, ".*") + "."))
                                ModelState.AddModelError("SGrueso", Regex.Match(ex.Message, rgx.Replace(RFamilias.ErrorGrueso, ".*")).Value);

                            return PartialView("_AlbaranesCompraslin", model);
                        }

                        Session[session] = model;

                        var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
                        var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
                        var portes = 0;

                        var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService;
                        Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales);
                    }
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_AlbaranesCompraslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AlbaranesComprasLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<AlbaranesComprasLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);

            var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
            var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
            var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
            var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
            var portes = 0;

            var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService;
            Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales);

            return PartialView("_AlbaranesCompraslin", model);
        }

        public ActionResult AlbaranesComprasTotales()
        {
            var model = Session[sessiontotales] as List<AlbaranesComprasTotalesModel>;
            return PartialView(model);
        }

        [ValidateInput(false)]
        public ActionResult AlbaranesComprasCostesAdicionales()
        {
            var model = Session[sessioncostes] as List<AlbaranesComprasCostesadicionalesModel>;
            return PartialView("_albaranesComprascostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesNew([ModelBinder(typeof(DevExpressEditorsBinder))] AlbaranesComprasCostesadicionalesModel item)
        {
            var model = Session[sessioncostes] as List<AlbaranesComprasCostesadicionalesModel>;
            if (ModelState.IsValid)
            {
                var max = model.Any() ? model.Max(f => f.Id) : 0;
                item.Id = max + 1;
                if (item.Tipodocumento == TipoCosteAdicional.Documento &&
                    string.IsNullOrEmpty(item.Referenciadocumento))
                {
                    ModelState.AddModelError("Referenciadocumento", "El campo es obligatorio");
                }
                else {
                    if (item.Tipodocumento == TipoCosteAdicional.Importefijo)
                    {
                        item.Referenciadocumento = string.Empty;

                        item.Total = Math.Round((double)(item.Importe * (item.Porcentaje / 100.0)), 2);

                        model.Add(item);
                        Session[sessioncostes] = model;
                    }else
                    {
                        item.Referenciadocumento = string.Empty;

                        // Código para calcular el coste adicional cuando tipoDocumento es xm2 o xm3
                        var tipoDocumento = item.Tipodocumento;
                        var codUnidadMedida = "-1";

                        if (tipoDocumento == TipoCosteAdicional.Costexm2)
                        {
                            codUnidadMedida = "02";
                        }
                        else if (tipoDocumento == TipoCosteAdicional.Costexm3)
                        {
                            codUnidadMedida = "03";
                        }

                        var lineas = Session[session] as List<AlbaranesComprasLinModel>;
                        var totalMetros = 0.0d;
                        foreach (var l in lineas)
                        {
                            var unidadMedida = l.Fkunidades;
                            if (unidadMedida.Equals(codUnidadMedida))
                            {
                                totalMetros += Math.Round((double)l.Metros, 3);
                            }
                        }
                        item.Total = Math.Round((double)item.Importe * totalMetros, 2);
                        model.Add(item);
                        Session[sessioncostes] = model;
                    }                    
                }
            }

            return PartialView("_albaranesComprascostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] AlbaranesComprasCostesadicionalesModel item)
        {
            var model = Session[sessioncostes] as List<AlbaranesComprasCostesadicionalesModel>;

            if (ModelState.IsValid)
            {
                var editItem = model.Single(f => f.Id == item.Id);
                if (item.Tipodocumento == TipoCosteAdicional.Documento && string.IsNullOrEmpty(item.Referenciadocumento))
                {
                    ModelState.AddModelError("Referenciadocumento", "El campo es obligatorio");
                }
                {
                    if (item.Tipodocumento == TipoCosteAdicional.Importefijo)
                    {
                        item.Referenciadocumento = string.Empty;
                        item.Total = Math.Round((double)(item.Importe * (item.Porcentaje / 100.0)), 2);
                    }
                    if(item.Tipodocumento == TipoCosteAdicional.Costexm2 || item.Tipodocumento == TipoCosteAdicional.Costexm3)
                    {
                        item.Referenciadocumento = string.Empty;

                        // Código para calcular el coste adicional cuando tipoDocumento es xm2 o xm3
                        var tipoDocumento = item.Tipodocumento;
                        var codUnidadMedida = "-1";

                        if (tipoDocumento == TipoCosteAdicional.Costexm2)
                        {
                            codUnidadMedida = "02";
                        }
                        else if (tipoDocumento == TipoCosteAdicional.Costexm3)
                        {
                            codUnidadMedida = "03";
                        }
                        
                        var lineas = Session[session] as List<AlbaranesComprasLinModel>;
                        var totalMetros = 0.0d;
                        foreach (var l in lineas)
                        {
                            var unidadMedida = l.Fkunidades;                            
                            if (unidadMedida.Equals(codUnidadMedida))
                            {
                                totalMetros += Math.Round((double)l.Metros, 3);
                            }                            
                        }
                        item.Total = Math.Round((double)item.Importe * totalMetros, 2);
                    }
                        
                    editItem.Tipodocumento = item.Tipodocumento;
                    editItem.Referenciadocumento = item.Referenciadocumento;
                    editItem.Importe = item.Importe;
                    editItem.Porcentaje = item.Porcentaje;
                    editItem.Total = item.Total;
                    editItem.Tipocoste = item.Tipocoste;
                    editItem.Tiporeparto = item.Tiporeparto;
                    editItem.Notas = item.Notas;

                    Session[sessioncostes] = model;                    
                }



            }

            return PartialView("_albaranescomprascostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[sessioncostes] as List<AlbaranesComprasCostesadicionalesModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[sessioncostes] = model;


            return PartialView("_albaranescomprascostesadicionales", model);
        }

        // GET: api/Supercuentas/5
        public void AlbaranesComprasRefresh()
        {
            var model = Session[session] as List<AlbaranesComprasLinModel>;
            var decimales = model.FirstOrDefault()?.Decimalesmonedas ?? 0;
            var porcentajedescuentopp = HttpContext.Request.Params["porcentajedescuentopp"];
            var porcentajedescuentocomercial = HttpContext.Request.Params["porcentajedescuentocomercial"];
            var fkregimeniva = HttpContext.Request.Params["fkregimeniva"];
            var service = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService;
            var lineas = service.RecalculaLineas(model, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, fkregimeniva, 0, decimales);
            Session[session] = lineas.ToList();
            Session[sessiontotales] = service.Recalculartotales(lineas, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, 0, decimales);
        }

        #endregion

        #region Action toolbar

        protected override ToolbarModel GenerateToolbar(IGestionService service, TipoOperacion operacion, dynamic model = null)
        {
            var result = base.GenerateToolbar(service, operacion, model as object);
            result.Titulo = RRecepcion.TituloEntidad;
            return result;
        }

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as AlbaranesComprasModel;
            var result = base.EditToolbar(service, model).ToList();

            result.Add(new ToolbarSeparatorModel());
            if (objModel.Tipoalbaran < (int)TipoAlbaran.Devolucion)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-share",
                    Texto = RAlbaranes.LblGestionDevoluciones,
                    Url = "javascript:CallGenerarDevolucion();"
                });
            }

            result.Add(new ToolbarSeparatorModel());
            if (objModel.Tipoalbaran < (int)TipoAlbaran.Reclamacion)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-undo",
                    Texto = RAlbaranes.LblGestionReclamaciones,
                    Url = "javascript:CallGenerarReclamacion();"
                });
            }
            
            if(objModel.Tipoalbaran != (int)TipoAlbaran.Reclamacion)
            {
                result.Add(new ToolbarSeparatorModel());
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-gear",
                    Texto = RAlbaranesCompras.Remedirlotes,
                    Url = Url.Action("AsistenteRemedirRecepcionStock", new { id = objModel.Referencia })
                });
            }
            
            if (objModel.Estado?.Tipoestado < TipoEstado.Finalizado && !string.IsNullOrEmpty(objModel.Fkpedidoscompras))
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-gear",
                    Texto = General.LblSaldarPedido,
                    Url = "javascript:CallSaldarPedido()"
                });
            }
            

            if (!string.IsNullOrEmpty(objModel.Fkseriefactura) && objModel.Estado?.Tipoestado < TipoEstado.Finalizado)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-gear",
                    Texto = General.LblFacturar,
                    Url = "javascript:CallFacturarAlbaran()"
                });
            }

            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = "javascript:eventAggregator.Publish('Enviaralbaran',\'\')"
            });
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboEstados(objModel));
            return result;
        }

        private ToolbarActionComboModel CreateComboEstados(AlbaranesComprasModel objModel)
        {
            var estadosService = new EstadosService(ContextService);

            var estados = estadosService.GetStates(DocumentoEstado.AlbaranesCompras, objModel.Tipoestado(ContextService));
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-refresh",
                Texto = General.LblCambiarEstado,
                Url = "#",
                Desactivado = true,
                Items = estados.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("CambiarEstado", "RecepcionesStock", new { documentoReferencia = objModel.Id, estadoNuevo = f.CampoId, returnUrl = Url.Action("Edit", "RecepcionesStock", new { id = objModel.Id }) }),
                    Texto = f.Descripcion
                })
            };
        }

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as AlbaranesComprasModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-gear",
                Texto = RAlbaranesCompras.Remedirlotes,
                Url = Url.Action("AsistenteRemedirRecepcionStock", new { id = objModel.Referencia })
            });

            result.Add(new ToolbarSeparatorModel());

            result.Add(CreateComboImprimir(objModel));
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = "javascript:eventAggregator.Publish('Enviaralbaran',\'\')"
            });
            return result;
        }

        private ToolbarActionComboModel CreateComboImprimir(AlbaranesComprasModel objModel)
        {


            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.AlbaranesCompras, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.AlbaranesCompras, reportId = f }),
                    Texto = f,
                    Target = "_blank"
                })
            };
        }

        #endregion

        protected override IGestionService createService(IModelView model)
        {

            var result = FService.Instance.GetService(typeof(RecepcionesStockModel), ContextService) as RecepcionStockService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }
        
    }
}