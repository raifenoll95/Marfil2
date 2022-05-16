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
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;

using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;

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
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes;
using Marfil.Dom.Persistencia.Model.Documentos.Transformaciones;
using System.Text.RegularExpressions;
using RFamilias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;
using Marfil.Dom.Persistencia.Model.Documentos.DivisionLotes;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;

namespace Marfil.App.WebMain.Controllers
{
    public class FacturasController : GenericController<FacturasModel>
    {
        private const string sessioncabecera = "_facturascabecera_";
        private const string session = "_Facturaslin_";
        private const string sessiontotales = "_Facturastotales_";
        private const string sessionvencimientos = "_vencimientoslin_";


        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "facturas";
            var permisos = appService.GetPermisosMenu("facturas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public FacturasController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Facturar(string serie, string fecha, string albaranesreferencia, string returnUrl)
        {
            var model = Helper.fModel.GetModel<FacturasModel>(ContextService);
            try
            {
                using (var service = createService(model) as FacturasService)
                { 
                    var vector = albaranesreferencia.Split(';');
                    model = service.ImportarAlbaranes(serie, fecha, vector);
                    TempData[Constantes.VariableMensajeWarning] = service.WarningList.Any() ? string.Join("<br/>", service.WarningList) : string.Empty;
                }
            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;
                return Redirect(returnUrl);
            }


            return RedirectToAction("Edit", "Facturas", new { id = model.Id });
        }

        public ActionResult RedirigirAlbaran(string id)
        {
            var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService;
            var servicetransformacioneslotes = FService.Instance.GetService(typeof(TransformacioneslotesModel), ContextService) as TransformacioneslotesService;
            var servicetransformaciones = FService.Instance.GetService(typeof(TransformacionesModel), ContextService) as ImputacionCosteservice;
            
            if (service.exists(id))
            {
                var alb = service.get(id) as AlbaranesModel;
                if (alb.Modo == ModoAlbaran.Constock)
                    return RedirectToAction("Details", "EntregasStock", new { id = alb.Id });

                return RedirectToAction("Details", "Albaranes", new { id = alb.Id });
            }
            else if (servicetransformacioneslotes.exists(id))
            {
                var alb = servicetransformacioneslotes.get(id) as TransformacioneslotesModel;

                return RedirectToAction("Details", "Transformacioneslotes", new { id = alb.Id });
            }
            else if (servicetransformaciones.exists(id))
            {
                var alb = servicetransformaciones.get(id) as TransformacionesModel;

                return RedirectToAction("Details", "Transformaciones", new { id = alb.Id });
            }

            throw new Exception("Documento no encontrado");

        }

        public ActionResult RedirigirAlbaranReferencia(string id)
        {
            var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService;
            var servicetransformacioneslotes = FService.Instance.GetService(typeof(TransformacioneslotesModel), ContextService) as TransformacioneslotesService;
            var servicetransformaciones = FService.Instance.GetService(typeof(TransformacionesModel), ContextService) as ImputacionCosteservice;
            var servicedivisionlotes = FService.Instance.GetService(typeof(DivisionLotesModel), ContextService) as DivisionLotesService;

            if (service.ExisteReferencia(id))
            {
                
                var alb = service.GetByReferencia(id);
                if (alb.Modo == ModoAlbaran.Constock)
                    return RedirectToAction("Details", "EntregasStock", new { id = alb.Id });

                return RedirectToAction("Details", "Albaranes", new { id = alb.Id });
            }
            else if (servicetransformacioneslotes.ExisteReferencia(id))
            {
                var alb = servicetransformacioneslotes.GetByReferencia(id);

                return RedirectToAction("Details", "Transformacioneslotes", new { id = alb.Id });
            }
            else if (servicetransformaciones.ExisteReferencia(id))
            {
                var alb = servicetransformaciones.GetByReferencia(id);

                return RedirectToAction("Details", "Transformaciones", new { id = alb.Id });
            }

            else if (servicedivisionlotes.ExisteReferencia(id))
            {
                var alb = servicedivisionlotes.GetByReferencia(id);

                return RedirectToAction("Details", "DivisionLotes", new { id = alb.Id });
            }

            throw new Exception("Documento no encontrado");

        }
        #region Importar linea

        [HttpPost]
        public ActionResult ImportarLineas(string lineas, string descuentocomercial, string descuentopp, string decimalesmonedas)
        {

            if (!string.IsNullOrEmpty(lineas))
            {
                var data = JsonConvert.DeserializeObject(lineas);
                var lineasactuales = Session[session] as List<FacturasLinModel>;
                var totalesactuales = Session[sessiontotales] as List<FacturasTotalesModel>;
                var list = data is JArray ? data as JArray : new JArray(new[] { data });

                using (var service = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService)
                {
                    service.EjercicioId = ContextService.Ejercicio;
                    var vector = list.Select(f => f.Value<string>("Referencia"));
                    service.AgregarAlbaranes(vector, lineasactuales, ref totalesactuales, Funciones.Qdouble(descuentocomercial) ?? 0, Funciones.Qdouble(descuentopp) ?? 0, Funciones.Qint(decimalesmonedas) ?? 2);
                }
                Session[session] = lineasactuales;
                Session[sessiontotales] = totalesactuales;
            }

            return new EmptyResult();
        }

        private LineaImportarModel CrearLineaImportar(JToken item)
        {
            var result = new LineaImportarModel();


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
            result.Revision = "";
            result.Fkdocumento = item.Value<string>("FkAlbaranes");
            result.Fkdocumentoid = item.Value<int?>("Id")?.ToString() ?? "";

            return result;
        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(FacturasModel), ContextService))
            {
                var model = TempData["model"] as FacturasModel ?? Helper.fModel.GetModel<FacturasModel>(ContextService);
                Session[session] = model.Lineas;
                Session[sessiontotales] = model.Totales;
                Session[sessionvencimientos] = model.Vencimientos;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }






        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(FacturasModel model)
        {
            try
            {
                model.Context = ContextService;
                model.Lineas = Session[session] as List<FacturasLinModel>;
                model.Totales = Session[sessiontotales] as List<FacturasTotalesModel>;
                model.Vencimientos = Session[sessionvencimientos] as List<FacturasVencimientosModel>;
                if (ModelState.IsValid)
                {
                    //Rai -- Si la factura que vamos a hacer es rectificativa y no tiene el motivo de rectificacion, ni la factura rectificada
                    if(model.Facturarectificativa == true && String.IsNullOrEmpty(model.Fkmotivorectificacion) && String.IsNullOrEmpty(model.Fkfacturarectificativa))
                    {
                        TempData["errors"] = "Motivo de rectificacion y Factura rectificativa deben tener valor";
                        TempData["model"] = model;
                        return RedirectToAction("Create");
                    }
                    
                    else
                    {
                        using (var gestionService = createService(model))
                        {
                            gestionService.create(model);
                            TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                            return RedirectToAction("Index");
                        }
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
            var newModel = Helper.fModel.GetModel<FacturasModel>(ContextService);
            using (var gestionService = createService(newModel) as FacturasService)
            {
                var model = TempData["model"] != null ? TempData["model"] as FacturasModel : gestionService.get(id) as FacturasModel;

                //Puede ser que sea rectificativo
                if (gestionService.Facturarectificativa(model) == true)
                {
                    model.Facturarectificativa = true;
                }

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[sessioncabecera] = ((FacturasModel)model);
                Session[session] = ((FacturasModel)model).Lineas;
                Session[sessiontotales] = ((FacturasModel)model).Totales;
                Session[sessionvencimientos] = ((FacturasModel)model).Vencimientos;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);               

                //return View(model);
                return View(urlAsiento(model));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(FacturasModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                model.Context = ContextService;
                model.Lineas = Session[session] as List<FacturasLinModel>;
                model.Totales = Session[sessiontotales] as List<FacturasTotalesModel>;
                model.Vencimientos = Session[sessionvencimientos] as List<FacturasVencimientosModel>;
                if (ModelState.IsValid)
                {
                    //Rai -- Si la factura que vamos a hacer es rectificativa y no tiene el motivo de rectificacion, ni la factura rectificada
                    if (model.Facturarectificativa == true && (String.IsNullOrEmpty(model.Fkmotivorectificacion) || String.IsNullOrEmpty(model.Fkfacturarectificativa)))
                    {
                        TempData["errors"] = "Motivo de rectificacion y Factura rectificativa deben tener valor";
                        TempData["model"] = model;
                        return RedirectToAction("Edit", new { id = model.Id });
                    }

                    else
                    {
                        using (var gestionService = createService(model))
                        {
                            gestionService.edit(model);
                            TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                            return RedirectToAction("Index");
                        }
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

            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var newModel = Helper.fModel.GetModel<FacturasModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((FacturasModel)model).Lineas;
                Session[sessiontotales] = ((FacturasModel)model).Totales;
                Session[sessionvencimientos] = ((FacturasModel)model).Vencimientos;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                //return View(model);
                return View(urlAsiento(model));
            }
        }

        public ActionResult Devolucion(string id, string lineas)
        {
            if (!string.IsNullOrEmpty(lineas))
            {
                using (var service = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService)
                {
                    var model = service.get(id) as FacturasModel;


                    model.Fkmotivosdevolucion = appService.GetListMotivosDevolucion().FirstOrDefault()?.Valor;

                    var data = JsonConvert.DeserializeObject(lineas);

                    var list = data is JArray ? data as JArray : new JArray(new[] { data });

                    var maxId = model.Lineas.Any() ? model.Lineas.Max(f => f.Id) : 0;
                    model.Lineas.Clear();

                    var vector = list.Select(CrearLineaImportar).ToList();
                    foreach (var item in vector)
                        item.Cantidad *= -1;
                    model.Lineas.AddRange(service.ImportarLineas(model.Id, maxId, vector));

                    model.Totales = service.Recalculartotales(model.Lineas, model.Porcentajedescuentoprontopago ?? 0, model.Porcentajedescuentocomercial ?? 0, 0, model.Decimalesmonedas).ToList();
                    TempData["model"] = model;
                    return RedirectToAction("Create");
                }
            }

            TempData["errors"] = "Ocurrión un problema al generar el albarán de devolución";
            return RedirectToAction("Edit", new { id = id });
        }

        public ActionResult CambiarEstado(string documentoReferencia, string estadoNuevo, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService)
                {

                    service.EjercicioId = ContextService.Ejercicio;
                    using (var estadosService = new EstadosService(ContextService))
                    {
                        var model = service.get(documentoReferencia) as FacturasModel;
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

        

        public ActionResult AsistenteFacturacion()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var fmodel = new FModel();
            var model = fmodel.GetModel<AsistenteFacturacionModel>(ContextService);


            model.Toolbar.Acciones = HelpItem();

            return View(model);
        }

        public ActionResult RefrescarVencimientos(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                using (var facturasService = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService)
                {
                    Session[sessionvencimientos] = facturasService.RefrescarVencimientos(Session[sessioncabecera] as FacturasModel, id);
                }
            }


            return new EmptyResult();
        }
        #endregion

        #region Grid Devexpress

        public ActionResult CustomGridViewEditingPartial(string key, string buttonid)
        {
            var model = Session[session] as List<FacturasLinModel>;

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

            return PartialView("_Facturaslin", model);
        }

        [ValidateInput(false)]
        public ActionResult FacturasLin()
        {
            var model = Session[session] as List<FacturasLinModel>;
            return PartialView("_Facturaslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FacturasLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] FacturasLinModel item)
        {
            var model = Session[session] as List<FacturasLinModel>;
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
                            ModelState.AddModelError("Canal", string.Format(General.ErrorCampoObligatorio, Facturas.Canal));
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
                                linea.Cantidad = linea.Cantidad - item.Cantidad;
                                using (var FacturasService = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService)
                                {
                                    var nuevalinea = FacturasService.ImportarLineas(1, 0, new[] { new LineaImportarModel()
                                {
                                    Fkarticulos = linea.Fkarticulos,
                                    Fkunidades = linea.Fkunidades,
                                    Ancho = linea.Ancho.Value,Cantidad = linea.Cantidad.Value,Largo = linea.Largo.Value,Grueso=linea.Grueso.Value,Metros=linea.Metros.Value,Precio = linea.Precio.Value,Porcentajedescuento = linea.Porcentajedescuento.Value
                                }}).First();

                                    linea.Metros = nuevalinea.Metros;
                                    linea.Importe = nuevalinea.Importe;
                                    linea.Importedescuento = nuevalinea.Importedescuento;
                                    item.Fkalbaranes = linea.Fkalbaranes;
                                    item.Fkalbaranesreferencia = linea.Fkalbaranesreferencia;
                                    item.Orden = linea.Id * ApplicationHelper.EspacioOrdenLineas;
                                }
                            }

                            item.Decimalesmonedas = monedaObj.Decimales;
                            item.Importe = Math.Round(item.Importe ?? 0, monedaObj.Decimales);
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

                                return PartialView("_Facturaslin", model);
                            }

                            model.Add(item);

                            Session[session] = model;
                            var service = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService;
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



            return PartialView("_Facturaslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FacturasLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] FacturasLinModel item)
        {
            var model = Session[session] as List<FacturasLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var configuracionAplicacion = appService.GetConfiguracion();
                    if (configuracionAplicacion.VentasUsarCanal && configuracionAplicacion.VentasCanalObligatorio &&
                        string.IsNullOrEmpty(item.Canal))
                    {
                        ModelState.AddModelError("Canal",
                            string.Format(General.ErrorCampoObligatorio, Facturas.Canal));
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
                        editItem.Caja = item.Caja;
                        editItem.Bundle = item.Bundle?.ToUpper();

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

                            return PartialView("_Facturaslin", model);
                        }

                        Session[session] = model;

                        var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
                        var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
                        var portes = 0;

                        var service = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService;
                        Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales);
                    }
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_Facturaslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult FacturasLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<FacturasLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);

            var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
            var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
            var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
            var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
            var portes = 0;

            var service = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService;
            Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales);

            return PartialView("_Facturaslin", model);
        }

        public ActionResult FacturasTotales()
        {
            var model = Session[sessiontotales] as List<FacturasTotalesModel>;
            return PartialView(model);
        }

        // GET: api/Supercuentas/5
        public void FacturasRefresh()
        {
            var model = Session[session] as List<FacturasLinModel>;
            var decimales = model.FirstOrDefault()?.Decimalesmonedas ?? 0;
            var porcentajedescuentopp = HttpContext.Request.Params["porcentajedescuentopp"];
            var porcentajedescuentocomercial = HttpContext.Request.Params["porcentajedescuentocomercial"];
            var fkregimeniva = HttpContext.Request.Params["fkregimeniva"];
            var service = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService;
            var lineas = service.RecalculaLineas(model, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, fkregimeniva, 0, decimales);
            Session[session] = lineas.ToList();
            Session[sessiontotales] = service.Recalculartotales(lineas, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, 0, decimales);
        }

        [ValidateInput(false)]
        public ActionResult VencimientosLin()
        {
            var model = Session[sessionvencimientos] as List<FacturasVencimientosModel>;
            return PartialView("_vencimientoslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult VencimientosLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] FacturasVencimientosModel item, string id)
        {
            var model = Session[sessionvencimientos] as List<FacturasVencimientosModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    var max = model.Any() ? model.Max(f => f.Id) : 0;
                    item.Id = max + 1;
                    item.Diasvencimiento = item.Diasvencimiento;
                    item.Fechavencimiento = item.Fechavencimiento;
                    item.Importevencimiento = item.Importevencimiento;
                    using (var facturasService = new FacturasService(ContextService))
                    {
                        facturasService.GetVencimiento(Session[sessioncabecera] as FacturasModel, model, item);
                    }

                    model.Add(item);
                    Session[sessionvencimientos] = model;
                }
            }
            catch (ValidationException)
            {
                model.Remove(item);
                throw;
            }
            return PartialView("_vencimientoslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult VencimientosLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] FacturasVencimientosModel item, string id)
        {
            var model = Session[sessionvencimientos] as List<FacturasVencimientosModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Diasvencimiento = item.Diasvencimiento;
                    editItem.Fechavencimiento = item.Fechavencimiento;
                    editItem.Importevencimiento = item.Importevencimiento;
                    using (var facturasService = new FacturasService(ContextService))
                        facturasService.GetVencimiento(Session[sessioncabecera] as FacturasModel, model, editItem);
                    Session[sessionvencimientos] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_vencimientoslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult VencimientosLinDelete(string id)
        {
            int intid = int.Parse(id);
            var model = Session[sessionvencimientos] as List<FacturasVencimientosModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[sessionvencimientos] = model;
            return PartialView("_vencimientoslin", model);
        }

        #endregion

        #region Action toolbar

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as FacturasModel;
            var result = base.EditToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            if (objModel.Estado?.Tipoestado < TipoEstado.Finalizado)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-gear",
                    Texto = General.LblContabilizar,
                    Url = Url.Action("Contabilizar", "Movs", new { IdFactura = objModel.Id, NombreTipo = typeof(FacturasModel).ToString(), TipoFactura = objModel.Fktipofactura, returnUrl = Url.Action("Edit", new { id = objModel.Id }) })
                });
                result.Add(new ToolbarSeparatorModel());
            }
            result.Add(CreateComboImprimir(objModel));
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = "javascript:eventAggregator.Publish('Enviarfactura',\'\')"
            });

            
            return result;
        }

        private ToolbarActionComboModel CreateComboEstados(FacturasModel objModel)
        {
            var estadosService = new EstadosService(ContextService);

            var estados = estadosService.GetStates(DocumentoEstado.FacturasVentas, objModel.Tipoestado(ContextService));
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-refresh",
                Texto = General.LblCambiarEstado,
                Url = "#",
                Desactivado = true,
                Items = estados.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("CambiarEstado", "Facturas", new { documentoReferencia = objModel.Id, estadoNuevo = f.Id, returnUrl = Url.Action("Edit", "Facturas", new { id = objModel.Id }) }),
                    Texto = f.Descripcion
                })
            };
        }

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as FacturasModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());

            result.Add(CreateComboImprimir(objModel));
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = "javascript:eventAggregator.Publish('Enviarfactura',\'\')"
            });

            
            return result;
        }

        private ToolbarActionComboModel CreateComboImprimir(FacturasModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.FacturasVentas, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.FacturasVentas, reportId = f }),
                    Texto = f,
                    Target = "_blank"

                })
            };
        }

        #endregion

        public IModelView urlAsiento(IModelView obj)
        {
            var model = obj as FacturasModel;

            if (model.Fkasiento > 0)
                model.urlAsiento = Url.Action("Details", "Movs", new { id = model.Fkasiento });

            return (obj);
        }

        protected override IGestionService createService(IModelView model)
        {

            var result = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }
    }
}