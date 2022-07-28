using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.StateMachine;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;
using System.Text.RegularExpressions;
using RFamilias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;

namespace Marfil.App.WebMain.Controllers
{
    public class PedidosController : GenericController<PedidosModel>
    {       
        private const string session = "_pedidoslin_";
        private const string sessiontotales = "_pedidostotales_";
        private const string sessionFabricacion = "_fabricacion_";
        private const string sessionImputacionMateriales = "_imputacionMateriales_";
        private const string sessionTotalesImputacion = "_totalesimputacion_";


        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "pedidos";
            var permisos = appService.GetPermisosMenu("pedidos");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public PedidosController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        public ActionResult Importar(string id, string returnUrl)
        {
            var model = Helper.fModel.GetModel<PedidosModel>(ContextService);
            try
            {
                using (var service = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService)
                {
                    using (var servicepresupuesto = FService.Instance.GetService(typeof(PresupuestosModel), ContextService) as PresupuestosService)
                    {
                        model = service.ImportarPresupuesto(servicepresupuesto.get(id) as PresupuestosModel);
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
            return RedirectToAction("Create","Pedidos");
        }

        public ActionResult RedirigirPedidoReferencia(string id)
        {
            using (var service = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService)
            {
                var model=service.GetByReferencia(id);
                return RedirectToAction("Details", new {id = model.Id});

            }
        }

        #region Importar linea

        [HttpPost]
        public ActionResult ImportarLineas(string lineas,string descuentocomercial, string descuentopp)
        {

            if (!string.IsNullOrEmpty(lineas))
            {
                var data = JsonConvert.DeserializeObject(lineas);
                var model = Session[session] as List<PedidosLinModel>;
                var list = data is JArray ? data as JArray : new JArray(new[] { data });
                var decimales = 2;
                var portes = 0;
                var pedidosService =FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService;
                var maxId = model.Any() ? model.Max(f => f.Id) : 0;
                model.AddRange(pedidosService.ImportarLineas(maxId,list.Select(CrearLineaImportar)));
                
                var service = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService;
                Session[session] = model;
                Session[sessiontotales] = service.Recalculartotales(model, Funciones.Qdouble(descuentopp) ?? 0,Funciones.Qdouble(descuentocomercial)??0, portes, decimales,true);
                //  SESSIONFABRICACION?
            }
            
            return new EmptyResult();
        }

        private LineaImportarModel CrearLineaImportar(JToken item)
        {
            var result = new LineaImportarModel();
            var moneda = item.Value<int?>("Fkmonedas");

            var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
            var monedaObj = serviceMonedas.get(moneda?.ToString() ?? "") as MonedasModel;
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
            result.Importe = Math.Round(item.Value<double?>("Importe")?? 0, monedaObj.Decimales);
            result.Importedescuento = item.Value<double?>("Importedescuento") ?? 0;
            result.Lote = item.Value<string>("Lote");
            result.Precio = Math.Round(item.Value<double?>("Precio") ?? 0, monedaObj.Decimales);
            result.Precioanterior = item.Value<double?>("Precioanterior") ?? 0;
            result.Porcentajedescuento = item.Value<double?>("Porcentajedescuento") ?? 0;
            result.Tabla = item.Value<int?>("Tabla") ?? 0;
            result.Revision ="";
            result.Fkdocumento = item.Value<string>("Fkpresupuestos");
            result.Fkdocumentoid = item.Value<int?>("Id")?.ToString() ?? "";
            result.Fkdocumentoreferencia = item.Value<string>("Fkpresupuestosreferencia")?.ToString() ?? "";

            return result;
        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            

            using (var gestionService = FService.Instance.GetService(typeof(PedidosModel), ContextService))
            {
                var model = TempData["model"] as PedidosModel ?? Helper.fModel.GetModel<PedidosModel>(ContextService);
                Session[session] = model.Lineas;
                Session[sessiontotales] = model.Totales;
                Session[sessionFabricacion] = model.CostesFabricacion;
                Session[sessionImputacionMateriales] = model.ImputacionMateriales;
                Session[sessionTotalesImputacion] = model.ImputacionMaterialesTotales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);

                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(PedidosModel model)
        {
            try
            {
                model.Context = ContextService;
                model.Lineas = Session[session] as List<PedidosLinModel>;
                model.Totales = Session[sessiontotales] as List<PedidosTotalesModel>;
                model.CostesFabricacion = Session[sessionFabricacion] as List<PedidosCostesFabricacionModel>;
                model.ImputacionMateriales = Session[sessionImputacionMateriales] as List<AlbaranesLinModel>;
                model.ImputacionMaterialesTotales = Session[sessionTotalesImputacion] as List<AlbaranesTotalesModel>;
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
            var newModel = Helper.fModel.GetModel<PedidosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as PedidosModel : gestionService.get(id);
                
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((PedidosModel)model).Lineas;
                Session[sessiontotales] = ((PedidosModel)model).Totales;
                Session[sessionFabricacion] = ((PedidosModel)model).CostesFabricacion;
                Session[sessionImputacionMateriales] = ((PedidosModel)model).ImputacionMateriales;
                Session[sessionTotalesImputacion] = ((PedidosModel)model).ImputacionMaterialesTotales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(PedidosModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                model.Context = ContextService;
                model.Lineas = Session[session] as List<PedidosLinModel>;
                model.Totales = Session[sessiontotales] as List<PedidosTotalesModel>;
                model.CostesFabricacion = Session[sessionFabricacion] as List<PedidosCostesFabricacionModel>;
                model.ImputacionMateriales = Session[sessionImputacionMateriales] as List<AlbaranesLinModel>;
                model.ImputacionMaterialesTotales = Session[sessionTotalesImputacion] as List<AlbaranesTotalesModel>;
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        
                       
                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
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

            var newModel = Helper.fModel.GetModel<PedidosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                
                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((PedidosModel)model).Lineas;
                Session[sessiontotales] = ((PedidosModel)model).Totales;
                Session[sessionFabricacion] = ((PedidosModel)model).CostesFabricacion;
                Session[sessionImputacionMateriales] = ((PedidosModel)model).ImputacionMateriales;
                Session[sessionTotalesImputacion] = ((PedidosModel)model).ImputacionMaterialesTotales;
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

            var newModel = Helper.fModel.GetModel<PedidosModel>(ContextService);
            using (var gestionService = createService(newModel) as PedidosService)
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

        public ActionResult CambiarEstado(string documentoReferencia, string estadoNuevo, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService)
                {
                   
                    service.EjercicioId = ContextService.Ejercicio;
                    using (var estadosService = new EstadosService(ContextService))
                    {
                        var model = service.get(documentoReferencia) as PedidosModel;
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

        public ActionResult AsistentePedidosCompra()
        {
            return View(new AsistentePedidosComprasModel()
            {
                Fechapedido = DateTime.Now
            });
        }

        public ActionResult GenerarPedidosCompra(string pedido)
        {
            using (var service = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService)
            {
                var pedidoObj = service.GetByReferencia(pedido);
                return View("AsistentePedidosCompra", new AsistentePedidosComprasModel()
                {
                    Fkclientes = pedidoObj.Fkclientes,
                    Fechapedido = DateTime.Now,
                    Fkpedidoinicio = pedido,
                    Fkpedidofin = pedido
                });
            }
               
        }

        #endregion

        #region Grid Devexpress

        public ActionResult CustomGridViewEditingPartial(string key)
        {
            ViewData["key"] = key;
            var model = Session[session] as List<PedidosLinModel>;
            return PartialView("_Pedidoslin", model);
        }

        [ValidateInput(false)]
        public ActionResult PedidosLin()
        {
            var model = Session[session] as List<PedidosLinModel>;
            return PartialView("_Pedidoslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PedidosLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] PedidosLinModel item)
        {
            var model = Session[session] as List<PedidosLinModel>;
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
                            ModelState.AddModelError("Canal", string.Format(General.ErrorCampoObligatorio, Pedidos.Canal));
                        }
                        else
                        {
                            var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
                            var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
                            var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
                            var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
                            var portes = 0;

                            item.Decimalesmonedas = monedaObj.Decimales;
                            item.Importe = Math.Round(item.Importe ?? 0, monedaObj.Decimales);
                            item.Importedescuento = Math.Round(item.Importedescuento ?? 0, monedaObj.Decimales);
                            item.Precio = Math.Round(item.Precio ?? 0, empresa.Decimalesprecios ?? 2);
                            item.Decimalesmedidas = decimalesunidades ?? 0;
                            item.Revision = item.Revision?.ToUpper();

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

                                return PartialView("_Pedidoslin", model);
                            }

                            model.Add(item);

                            Session[session] = model;
                            var service = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService;
                            Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales,false);
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



            return PartialView("_pedidoslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PedidosLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] PedidosLinModel item)
        {
            var model = Session[session] as List<PedidosLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var configuracionAplicacion = appService.GetConfiguracion();
                    if (configuracionAplicacion.VentasUsarCanal && configuracionAplicacion.VentasCanalObligatorio &&
                        string.IsNullOrEmpty(item.Canal))
                    {
                        ModelState.AddModelError("Canal",
                            string.Format(General.ErrorCampoObligatorio, Pedidos.Canal));
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
                        editItem.Fkunidades = item.Fkunidades;
                        editItem.Importe = Math.Round(item.Importe ?? 0, monedaObj.Decimales);
                        editItem.Importedescuento = item.Importedescuento;
                        editItem.Lote = item.Lote;
                        editItem.Precio = Math.Round(item.Precio ?? 0, empresa.Decimalesprecios ?? 2);
                        editItem.Precioanterior = item.Precioanterior;
                        editItem.Porcentajedescuento = item.Porcentajedescuento;
                        editItem.Tabla = item.Tabla;
                        editItem.Revision = item.Revision?.ToUpper();
                        editItem.Orden = item.Orden;

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

                            return PartialView("_Pedidoslin", model);
                        }

                        Session[session] = model;

                        var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
                        var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
                        var portes = 0;

                        var service = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService;
                        Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales,false);
                    }
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_Pedidoslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PedidosLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<PedidosLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);

            var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
            var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
            var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
            var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
            var portes = 0;

            var service = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService;
            Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales,false);

            return PartialView("_Pedidoslin", model);
        }

        public ActionResult PedidosTotales()
        {
            var model = Session[sessiontotales] as List<PedidosTotalesModel>;            
            return PartialView(model);
        }

        // GET: api/Supercuentas/5
        public void PedidosRefresh()
        {
            var model = Session[session] as List<PedidosLinModel>;
            var decimales = model.FirstOrDefault()?.Decimalesmonedas ?? 0;
            var porcentajedescuentopp = HttpContext.Request.Params["porcentajedescuentopp"];
            var porcentajedescuentocomercial = HttpContext.Request.Params["porcentajedescuentocomercial"];
            var fkregimeniva = HttpContext.Request.Params["fkregimeniva"];
            var service = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService;
            var lineas = service.RecalculaLineas(model, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, fkregimeniva, 0, decimales);
            Session[session] = lineas.ToList();
            Session[sessiontotales] = service.Recalculartotales(lineas, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, 0, decimales,true);   
        }

        #endregion

        #region Grid Fabricación   

        public ActionResult GridViewCostesFabricacion(string key)
        {
            ViewData["key"] = key;
            var model = Session[sessionFabricacion] as List<PedidosCostesFabricacionModel>;
            return PartialView("_Pedidoscostesfabricacion", model);
        }

        [ValidateInput(false)]
        public ActionResult CostesFabricacion()
        {
            var model = Session[sessionFabricacion] as List<PedidosCostesFabricacionModel>;
            return PartialView("_Pedidoscostesfabricacion", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesFabricacionNew([ModelBinder(typeof(DevExpressEditorsBinder))] PedidosCostesFabricacionModel item)
        {
            var model = Session[sessionFabricacion] as List<PedidosCostesFabricacionModel>;
            if (ModelState.IsValid)
            {
                var max = model.Any() ? model.Max(f => f.Id) : 0;
                item.Id = max + 1;
                model.Add(item);                
                Session[sessionFabricacion] = model;                                 
            }

            return PartialView("_Pedidoscostesfabricacion", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesFabricacionUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] PedidosCostesFabricacionModel item)
        {
            var model = Session[sessionFabricacion] as List<PedidosCostesFabricacionModel>;
            if (ModelState.IsValid)
            {                

                var editItem = model.Single(f => f.Id == item.Id);
                editItem.Fecha = item.Fecha;
                editItem.Fkoperario = item.Fkoperario;
                editItem.Fktarea = item.Fktarea;
                editItem.Descripcion = item.Descripcion;
                editItem.Cantidad = item.Cantidad;
                editItem.Precio = item.Precio;
                editItem.Total = item.Total;
                
                Session[sessionFabricacion] = model;
            }

            return PartialView("_Pedidoscostesfabricacion", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesFabricacionDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[sessionFabricacion] as List<PedidosCostesFabricacionModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[sessionFabricacion] = model;

            return PartialView("_Pedidoscostesfabricacion", model);
        }

        public ActionResult ForzarCambiarCostes(string documentoReferencia, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService)
                {
                    service.EjercicioId = ContextService.Ejercicio;
                    var model = service.get(documentoReferencia) as PedidosModel;
                    model.CostesFabricacion = Session[sessionFabricacion] as List<PedidosCostesFabricacionModel>;
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



        #endregion

        #region Grid Imputación materiales

        public ActionResult GridViewImputacionMateriales(string key)
        {
            ViewData["key"] = key;
            var model = Session[sessionImputacionMateriales] as List<AlbaranesLinModel>;
            return PartialView("_Imputacionmaterialeslin", model);
        }

        [ValidateInput(false)]
        public ActionResult ImputacionMateriales()
        {
            var model = Session[sessionImputacionMateriales] as List<AlbaranesLinModel>;
            return PartialView("_Imputacionmaterialeslin", model);
        }

        public ActionResult ImputacionMaterialesTotales()
        {
            var model = Session[sessionTotalesImputacion] as List<AlbaranesTotalesModel>;
            return PartialView(model);
        }

        #endregion

        #region Action toolbar

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as PedidosModel;
            var result = base.EditToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            if (objModel.Estado?.Tipoestado < TipoEstado.Finalizado)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-gear",
                    Texto = General.LblGenerarAlbaran,
                    Url = Url.Action("Importar", "Albaranes", new { id = objModel.Id, returnUrl = Url.Action("Edit", new { id = objModel.Id }) })
                });

                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-gear",
                    Texto = General.LblGenerarEntrega,
                    Url = Url.Action("Importar", "EntregasStock", new { id = objModel.Id, returnUrl = Url.Action("Edit", new { id = objModel.Id }) })
                });
            }

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-gear",
                Texto = PedidosCompras.Generarpedidoscompras,
                Url = Url.Action("GenerarPedidosCompra", "Pedidos", new { pedido = objModel.Referencia })
            });

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-gear",
                Texto = General.LblGenerarImputacionMateriales,
                Url = Url.Action("ImputacionMateriales", "EntregasStock", new { id = objModel.Id, returnUrl = Url.Action("Edit", new { id = objModel.Id }) })
            });

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-copy",
                Texto = General.LblClonar,
                Url = Url.Action("Clonar", new { id = objModel.Id })
            });

            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = "javascript:eventAggregator.Publish('Enviarpedido',\'\')"
            });
            result.Add(new ToolbarSeparatorModel());
            if (objModel.Estado.Tipoestado != TipoEstado.Finalizado && objModel.Estado.Tipoestado != TipoEstado.Anulado)
            {
                result.Add(CreateComboEstados(objModel));
            }
            
            return result;
        }

        private ToolbarActionComboModel CreateComboEstados(PedidosModel objModel)
        {
            var estadosService = new EstadosService(ContextService);

            var estados = estadosService.GetStates(DocumentoEstado.PedidosVentas, objModel.Tipoestado(ContextService));
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-refresh",
                Texto = General.LblCambiarEstado,
                Url = "#",
                Desactivado = true,
                Items = estados.Where(f => f.Tipomovimiento != TipoMovimiento.Automatico).Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("CambiarEstado", "Pedidos", new { documentoReferencia = objModel.Id, estadoNuevo = f.CampoId, returnUrl = Url.Action("Edit", "Pedidos", new { id = objModel.Id }) }),
                    Texto = f.Descripcion
                })
            };
        }

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as PedidosModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());

            result.Add(CreateComboImprimir(objModel));
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = "javascript:eventAggregator.Publish('Enviarpedido',\'\')"
            });

           
            return result;
        }

        private ToolbarActionComboModel CreateComboImprimir(PedidosModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.PedidosVentas, reportId = objModel.DocumentosImpresion.Defecto }),
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.PedidosVentas, reportId = f }),
                    Texto = f
                }),
                Target = "_blank"
            };
        }

        #endregion

        protected override IGestionService createService(IModelView model)
        {
           
            var result = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }
    }
}