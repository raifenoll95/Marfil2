using System;
using System.Collections.Generic;
using System.Globalization;
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
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;

using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.StateMachine;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;
using RAlbaranes =Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using System.Text.RegularExpressions;
using RFamilias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;

namespace Marfil.App.WebMain.Controllers
{
    public class AlbaranesController : GenericController<AlbaranesModel>
    {
        private const string session = "_Albaraneslin_";
        private const string sessiontotales = "_Albaranestotales_";


        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "Albaranes";
            var permisos = appService.GetPermisosMenu("Albaranes");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public AlbaranesController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        public ActionResult Importar(string id, string returnUrl)
        {
            var model = Helper.fModel.GetModel<AlbaranesModel>(ContextService);
            try
            {
                using (var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService)
                {
                    using (var pedidosService = FService.Instance.GetService(typeof(PedidosModel), ContextService) as PedidosService)
                    {
                        model = service.ImportarPedido(pedidosService.get(id) as PedidosModel);
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

            //Creamos el albarán para evitar sobreescribir os datos
            using (var gestionService = createService(model))
            {

                gestionService.create(model);
                TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;

                //Redireccionamos a la ventana de edición
                return RedirectToAction("Edit", "Albaranes", new { id = model.Id });
            }

        }

        #region Importar linea

        [HttpPost]
        public ActionResult ImportarLineas(string lineas, string descuentocomercial, string descuentopp)
        {

            if (!string.IsNullOrEmpty(lineas))
            {
                var data = JsonConvert.DeserializeObject(lineas);
                var model = Session[session] as List<AlbaranesLinModel>;
                var list = data is JArray ? data as JArray : new JArray(new[] { data });
                var decimales = 2;
                var portes = 0;
                var AlbaranesService = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService;
                var maxId = model.Any() ? model.Max(f => f.Id) : 0;
                model.AddRange(AlbaranesService.ImportarLineas(maxId, list.Select(CrearLineaImportar)));

                var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService;
                Session[session] = model;
                Session[sessiontotales] = service.Recalculartotales(model, Funciones.Qdouble(descuentopp) ?? 0, Funciones.Qdouble(descuentocomercial) ?? 0, portes, decimales, true);
            }

            return new EmptyResult();
        }

        private LineaImportarModel CrearLineaImportar(JToken item)
        {
            var result = new LineaImportarModel();
            result.Id = item.Value<int>("Id");
            result.Fkalbaranes = item.Value<int>("Fkalbaranes");
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
            result.Fkdocumento = item.Value<string>("Fkpedidos");
            result.Fkdocumentoid = item.Value<int?>("Id")?.ToString() ?? "";
            result.Fkdocumentoreferencia = item.Value<string>("Fkpedidosreferencia");

            result.Contenedor = item.Value<string>("Contenedor");
            result.Sello = item.Value<string>("Sello");
            var valorcaja = item.Value<string>("Caja");

            if (String.IsNullOrEmpty(valorcaja))
            {
                result.Caja = null;
            }

            else
            {
                result.Caja = Int32.Parse(valorcaja);
            }

            var valorpesoneto = item.Value<string>("Pesoneto");

            if (String.IsNullOrEmpty(valorpesoneto))
            {
                result.Pesoneto = null;
            }

            else
            {
                result.Pesoneto = Convert.ToDouble(valorpesoneto);
            }

            return result;
        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(AlbaranesModel), ContextService))
            {
                var model = TempData["model"] as AlbaranesModel ?? Helper.fModel.GetModel<AlbaranesModel>(ContextService);
                Session[session] = model.Lineas;
                Session[sessiontotales] = model.Totales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(AlbaranesModel model)
        {
            try
            {
                model.Context = ContextService;
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<AlbaranesModel>(ContextService);
                model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
                model.Lineas = Session[session] as List<AlbaranesLinModel>;
                model.Totales = Session[sessiontotales] as List<AlbaranesTotalesModel>;
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
            var newModel = Helper.fModel.GetModel<AlbaranesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as AlbaranesModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((AlbaranesModel)model).Lineas;
                Session[sessiontotales] = ((AlbaranesModel)model).Totales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(AlbaranesModel model)
        {
            model.Context = ContextService;
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            var fmodel = new FModel();
            var newmodel = fmodel.GetModel<AlbaranesModel>(ContextService);
            model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
            model.Lineas = Session[session] as List<AlbaranesLinModel>;
            model.Totales = Session[sessiontotales] as List<AlbaranesTotalesModel>;
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

            var newModel = Helper.fModel.GetModel<AlbaranesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((AlbaranesModel)model).Lineas;
                Session[sessiontotales] = ((AlbaranesModel)model).Totales;
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

            var newModel = Helper.fModel.GetModel<AlbaranesModel>(ContextService);
            using (var gestionService = createService(newModel) as AlbaranesService)
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
        public ActionResult Devolucion(string id, string lineas)
        {
            if (!string.IsNullOrEmpty(lineas))
            {
                using (var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService)
                {
                    var model = service.get(id) as AlbaranesModel;                    
                    model.Id = 0;
                    model.Tipoalbaran = (int)TipoAlbaran.Devolucion;
                    model.Fechadocumento = DateTime.Now;
                    model.Fkmotivosdevolucion = appService.GetListMotivosDevolucion().FirstOrDefault()?.Valor;                    

                    var data = JsonConvert.DeserializeObject(lineas);

                    var list = data is JArray ? data as JArray : new JArray(new[] { data });

                    var maxId = model.Lineas.Any() ? model.Lineas.Max(f => f.Id) : 0;
                    model.Lineas.Clear();

                    var vector = list.Select(CrearLineaImportar).ToList();
                    foreach (var item in vector)
                        item.Cantidad *= -1;
                    model.Lineas.AddRange(service.ImportarLineas(maxId, vector));

                    // No es necesario, no se controla el stock
                    //foreach(var l in model.Lineas)
                    //{
                    //    l.Documentoorigen = model.Referencia;
                    //}

                    var newmodel = Helper.fModel.GetModel<AlbaranesModel>(ContextService);
                    model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
                    model.Totales = service.Recalculartotales(model.Lineas, model.Porcentajedescuentoprontopago ?? 0, model.Porcentajedescuentocomercial ?? 0, 0, model.Decimalesmonedas,true).ToList();
                    TempData["model"] = model;
                    return RedirectToAction("Create");
                }
            }

            TempData["errors"] = "Ocurrió un problema al generar el albarán de devolución";
            return RedirectToAction("Edit", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AsignarContenedores(string id, string lineas)
        {
            if (!string.IsNullOrEmpty(lineas))
            {
                using (var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService)
                {
                    var model = service.get(id) as AlbaranesModel;
                    model.Fkestados = appService.GetConfiguracion()?.Estadoalbaranescomprasinicial ?? string.Empty;
                    model.Fkmotivosdevolucion = appService.GetListMotivosDevolucion().FirstOrDefault()?.Valor;
                    var maxId = model.Lineas.Any() ? model.Lineas.Max(f => f.Id) : 0;
                    var data = JsonConvert.DeserializeObject(lineas);
                    var list = data is JArray ? data as JArray : new JArray(new[] { data });
                    
                    var lineasimputadas = list.Select(CrearLineaImportar).ToList(); //Lineas de clase "Importar"

                    foreach (var linea in model.Lineas)
                    {
                        if (lineasimputadas.Any(f => f.Id == linea.Id))
                        {
                            var modificada = lineasimputadas.Single(f => f.Id == linea.Id);
                            linea.Contenedor = modificada.Contenedor;
                            linea.Sello = modificada.Sello;
                            linea.Pesoneto = modificada.Pesoneto;
                            linea.Caja = modificada.Caja;
                        }
                    }

                    service.edit(model);
                    return RedirectToAction("Index");
                }
            }

            TempData["errors"] = "Ocurrió un problema al generar el albarán de reclamación";
            return RedirectToAction("Edit", new { id = id });
        }

        public ActionResult CambiarEstado(string documentoReferencia, string estadoNuevo, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService)
                {
                    
                    service.EjercicioId = ContextService.Ejercicio;
                    using (var estadosService = new EstadosService(ContextService))
                    {
                        var model = service.get(documentoReferencia) as AlbaranesModel;
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

        #endregion

        #region Grid Devexpress

        public ActionResult CustomGridViewEditingPartial(string key, string buttonid)
        {
            var model = Session[session] as List<AlbaranesLinModel>;

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

            return PartialView("_Albaraneslin", model);
        }

        [ValidateInput(false)]
        public ActionResult AlbaranesLin()
        {
            var model = Session[session] as List<AlbaranesLinModel>;
            return PartialView("_Albaraneslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AlbaranesLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] AlbaranesLinModel item)
        {
            var model = Session[session] as List<AlbaranesLinModel>;
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
                            ModelState.AddModelError("Canal", string.Format(General.ErrorCampoObligatorio, Albaranes.Canal));
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

                               var cantidadRestante= linea.Cantidad - item.Cantidad;
                                if (cantidadRestante <= 0)
                                {
                                    ModelState.AddModelError("Fkarticulos",RAlbaranes.ErrorDividirLineaCantidadCero);
                                    return PartialView("_Albaraneslin", model);
                                }

                                linea.Cantidad = cantidadRestante;
                                using (var albaranesService = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService)
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

                            item.Decimalesmonedas = monedaObj.Decimales;
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

                                return PartialView("_Albaraneslin", model);
                            }

                            model.Add(item);

                            Session[session] = model;
                            var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService;
                            Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales,false);
                        }

                    }
                    else
                        ModelState.AddModelError("Fkarticulos", Articulos.ErrorArticuloInexistente);


                }
            }
            catch (Exception)
            {
                model.Remove(item);
                throw;
            }



            return PartialView("_Albaraneslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AlbaranesLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] AlbaranesLinModel item)
        {
            var model = Session[session] as List<AlbaranesLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var configuracionAplicacion = appService.GetConfiguracion();
                    if (configuracionAplicacion.VentasUsarCanal && configuracionAplicacion.VentasCanalObligatorio &&
                        string.IsNullOrEmpty(item.Canal))
                    {
                        ModelState.AddModelError("Canal",
                            string.Format(General.ErrorCampoObligatorio, Albaranes.Canal));
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
                        editItem.Importe = Math.Round(item.Importe ?? 0, (editItem.Decimalesmonedas??0));
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

                            return PartialView("_Albaraneslin", model);
                        }

                        Session[session] = model;

                        var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
                        var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
                        var portes = 0;

                        var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService;
                        Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales,false);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return PartialView("_Albaraneslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AlbaranesLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<AlbaranesLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);

            var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
            var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
            var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
            var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
            var portes = 0;

            var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService;
            Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales,false);

            return PartialView("_Albaraneslin", model);
        }

        public ActionResult AlbaranesTotales()
        {
            var model = Session[sessiontotales] as List<AlbaranesTotalesModel>;
            return PartialView(model);
        }

        // GET: api/Supercuentas/5
        public void AlbaranesRefresh()
        {
            var model = Session[session] as List<AlbaranesLinModel>;
            var decimales = model.FirstOrDefault()?.Decimalesmonedas ?? 0;
            var porcentajedescuentopp = HttpContext.Request.Params["porcentajedescuentopp"];
            var porcentajedescuentocomercial = HttpContext.Request.Params["porcentajedescuentocomercial"];
            var fkregimeniva = HttpContext.Request.Params["fkregimeniva"];
            var service = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService;
            var lineas = service.RecalculaLineas(model, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, fkregimeniva, 0, decimales);
            Session[session] = lineas.ToList();
            Session[sessiontotales] = service.Recalculartotales(lineas, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, 0, decimales,true);
        }

        #endregion

        #region Action toolbar

        protected override ToolbarModel GenerateToolbar(IGestionService service, TipoOperacion operacion, dynamic model = null)
        {
            var result = base.GenerateToolbar(service, operacion, model as object);

            result.Titulo = RAlbaranes.AlbaranTaller;

            return result;
        }

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as AlbaranesModel;
            var result = base.EditToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-copy",
                Texto = General.LblClonar,
                Url = Url.Action("Clonar", new { id = objModel.Id })
            });

            if (objModel.Tipoalbaran < (int)TipoAlbaran.Devolucion)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-share",
                    Texto = Albaranes.LblGestionDevoluciones,
                    Url = "javascript:CallGenerarDevolucion();"
                });
            }

            result.Add(new ToolbarSeparatorModel());

            if (objModel.Tipoalbaran < (int)TipoAlbaran.Reclamacion)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-archive",
                    Texto = RAlbaranes.LblGestionContenedores,
                    Url = "javascript:CallGenerarContenedores();"
                });

                result.Add(new ToolbarSeparatorModel());
            }


            if (!string.IsNullOrEmpty(objModel.Fkseriefactura) && objModel.Estado?.Tipoestado<TipoEstado.Finalizado)
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
            if (objModel.Estado.Tipoestado != TipoEstado.Finalizado && objModel.Estado.Tipoestado != TipoEstado.Anulado)
            {
                result.Add(new ToolbarSeparatorModel());
                result.Add(CreateComboEstados(objModel));
            }
            return result;
        }

        private ToolbarActionComboModel CreateComboEstados(AlbaranesModel objModel)
        {
            var estadosService = new EstadosService(ContextService);

            var estados = estadosService.GetStates(DocumentoEstado.AlbaranesVentas, objModel.Tipoestado(ContextService));
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-refresh",
                Texto = General.LblCambiarEstado,
                Url = "#",
                Desactivado = true,
                Items = estados.Where(f => f.Tipomovimiento != TipoMovimiento.Automatico).Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("CambiarEstado", "Albaranes", new { documentoReferencia = objModel.Id, estadoNuevo = f.CampoId, returnUrl = Url.Action("Edit", "Albaranes", new { id = objModel.Id }) }),
                    Texto = f.Descripcion
                })
            };
        }

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as AlbaranesModel;
            var result = base.VerToolbar(service, model).ToList();
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

        private ToolbarActionComboModel CreateComboImprimir(AlbaranesModel objModel)
        {
             objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.AlbaranesVentas, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.AlbaranesVentas, reportId = f }),
                    Texto = f,
                    Target = "_blank"

                })
            };
        }

        #endregion

        protected override IGestionService createService(IModelView model)
        {
            
            var result = FService.Instance.GetService(typeof(AlbaranesModel), ContextService) as AlbaranesService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }

    }
}