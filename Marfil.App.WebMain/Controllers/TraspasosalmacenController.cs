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
using Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen;
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
using RTraspasosalmacen = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Traspasosalmacen;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;

namespace Marfil.App.WebMain.Controllers
{
    public class TraspasosalmacenController : GenericController<TraspasosalmacenModel>
    {
        private const string session = "_Traspasosalmacenlin_";
        private const string sessioncostes = "_Traspasosalmacencostes_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "traspasosalmacen";
            var permisos = appService.GetPermisosMenu("traspasosalmacen");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public TraspasosalmacenController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        #region Create

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(TraspasosalmacenModel), ContextService))
            {
                var model = TempData["model"] as TraspasosalmacenModel ?? Helper.fModel.GetModel<TraspasosalmacenModel>(ContextService);
                Session[session] = model.Lineas;
                Session[sessioncostes] = model.Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(TraspasosalmacenModel model)
        {
            try
            {
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<TraspasosalmacenModel>(ContextService);
               
                model.Lineas = Session[session] as List<TraspasosalmacenLinModel>;
                model.Costes = Session[sessioncostes] as List<TraspasosalmacenCostesadicionalesModel>;
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

        #endregion

        #region Edit

        public override ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newModel = Helper.fModel.GetModel<TraspasosalmacenModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as TraspasosalmacenModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((TraspasosalmacenModel)model).Lineas;
                
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(TraspasosalmacenModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            var fmodel = new FModel();
            var newmodel = fmodel.GetModel<TraspasosalmacenModel>(ContextService);
            
            model.Lineas = Session[session] as List<TraspasosalmacenLinModel>;
            
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

            
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = model.Id });
            }
            catch (Exception ex)
            {

                TempData["errors"] = ex.Message;
                model.Context = ContextService;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = model.Id });
            }
        }
        #endregion

        #region Details

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newModel = Helper.fModel.GetModel<TraspasosalmacenModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((TraspasosalmacenModel)model).Lineas;
                Session[sessioncostes] = ((TraspasosalmacenModel)model).Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }

        #endregion

        #region Clonar

        public ActionResult Clonar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var newModel = Helper.fModel.GetModel<TraspasosalmacenModel>(ContextService);
            using (var gestionService = createService(newModel) as TraspasosalmacenService)
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

        #endregion

        #region Cambiar estado

        public ActionResult CambiarEstado(string documentoReferencia, string estadoNuevo, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(TraspasosalmacenModel), ContextService) as TraspasosalmacenService)
                {
                    
                    service.EjercicioId = ContextService.Ejercicio;
                    using (var estadosService = new EstadosService(ContextService))
                    {
                        var model = service.get(documentoReferencia) as TraspasosalmacenModel;
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

        #region Cambiar Costes

        public ActionResult ForzarCambiarCostes(string documentoReferencia, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(TraspasosalmacenModel), ContextService) as TraspasosalmacenService)
                {
                    service.EjercicioId = ContextService.Ejercicio;
                    var model = service.get(documentoReferencia) as TraspasosalmacenModel;
                    model.Costes = Session[sessioncostes] as List<TraspasosalmacenCostesadicionalesModel>;
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

        #endregion

        #region Grid Devexpress

        [HttpPost]
        public ActionResult AgregarLineas(TraspasosalmacenLinVistaModel model)
        {
            var errormessage = "";
            try
            {
                var listado = Session[session] as List<TraspasosalmacenLinModel>;
                using (var TraspasosalmacenService = FService.Instance.GetService(typeof(TraspasosalmacenModel), ContextService) as TraspasosalmacenService)
                {

                    listado = TraspasosalmacenService.CrearNuevasLineas(listado, model);
                    Session[session] = listado;
                    var service = FService.Instance.GetService(typeof(TraspasosalmacenModel), ContextService) as TraspasosalmacenService;
                    
                }

                return Content(JsonConvert.SerializeObject(model), "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
            }


            return Content("{\"error\":\"" + errormessage + "\"}", "application/json", Encoding.UTF8);
        }

        

        [ValidateInput(false)]
        public ActionResult TraspasosalmacenLin()
        {
            var model = Session[session] as List<TraspasosalmacenLinModel>;
            return PartialView("_Traspasosalmacenlin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TraspasosalmacenLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] TraspasosalmacenLinModel item)
        {
            var model = Session[session] as List<TraspasosalmacenLinModel>;
            return PartialView("_Traspasosalmacenlin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TraspasosalmacenLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TraspasosalmacenLinModel item)
        {
            var model = Session[session] as List<TraspasosalmacenLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var configuracionAplicacion = appService.GetConfiguracion();
                    if (configuracionAplicacion.VentasUsarCanal && configuracionAplicacion.VentasCanalObligatorio &&
                        string.IsNullOrEmpty(item.Canal))
                    {
                        ModelState.AddModelError("Canal",
                            string.Format(General.ErrorCampoObligatorio, Traspasosalmacen.Canal));
                    }
                    else
                    {
                        var editItem = model.Single(f => f.Id == item.Id);
                        var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);
                        var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
                        var decimalesmonedas = Funciones.Qint(Request.Params["decimalesmonedas"]);

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
                        editItem.Importe = Math.Round(item.Importe ?? 0, (editItem.Decimalesmonedas ?? 0));
                        editItem.Importedescuento = item.Importedescuento;
                        editItem.Lote = item.Lote;
                        editItem.Precio = Math.Round(item.Precio ?? 0, decimalesmonedas ?? 0);
                        editItem.Precioanterior = item.Precioanterior;
                        editItem.Porcentajedescuento = item.Porcentajedescuento;
                        editItem.Tabla = item.Tabla;
                        editItem.Revision = item.Revision?.ToUpper();
                        editItem.Caja = item.Caja;
                        editItem.Bundle = item.Bundle?.ToUpper();
                        editItem.Orden = item.Orden;
                        Session[session] = model;

                        var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
                        var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
                        var portes = 0;

                        var service = FService.Instance.GetService(typeof(TraspasosalmacenModel), ContextService) as TraspasosalmacenService;
                        
                    }
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_Traspasosalmacenlin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TraspasosalmacenLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<TraspasosalmacenLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;

            return PartialView("_Traspasosalmacenlin", model);
        }

        [ValidateInput(false)]
        public ActionResult TraspasosalmacenCostesAdicionales()
        {
            var model = Session[sessioncostes] as List<TraspasosalmacenCostesadicionalesModel>;
            return PartialView("_Traspasosalmacencostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesNew([ModelBinder(typeof(DevExpressEditorsBinder))] TraspasosalmacenCostesadicionalesModel item)
        {
            var model = Session[sessioncostes] as List<TraspasosalmacenCostesadicionalesModel>;
            if (ModelState.IsValid)
            {
                var max = model.Any() ? model.Max(f => f.Id) : 0;
                item.Id = max + 1;
                if (item.Tipodocumento == TipoCosteAdicional.Documento &&
                    string.IsNullOrEmpty(item.Referenciadocumento))
                {
                    ModelState.AddModelError("Referenciadocumento", "El campo es obligatorio");
                }
                else
                {
                    if (item.Tipodocumento == TipoCosteAdicional.Importefijo)
                        item.Referenciadocumento = string.Empty;

                    item.Total = Math.Round((double)(item.Importe * (item.Porcentaje / 100.0)), 2);

                    model.Add(item);
                    Session[sessioncostes] = model;
                }
            }

            return PartialView("_Traspasosalmacencostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TraspasosalmacenCostesadicionalesModel item)
        {
            var model = Session[sessioncostes] as List<TraspasosalmacenCostesadicionalesModel>;

            if (ModelState.IsValid)
            {
                var editItem = model.Single(f => f.Id == item.Id);
                if (item.Tipodocumento == TipoCosteAdicional.Documento && string.IsNullOrEmpty(item.Referenciadocumento))
                {
                    ModelState.AddModelError("Referenciadocumento", "El campo es obligatorio");
                }
                {
                    if (item.Tipodocumento == TipoCosteAdicional.Importefijo)
                        item.Referenciadocumento = string.Empty;
                    item.Total = Math.Round((double)(item.Importe * (item.Porcentaje / 100.0)), 2);

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

            return PartialView("_Traspasosalmacencostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[sessioncostes] as List<TraspasosalmacenCostesadicionalesModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[sessioncostes] = model;


            return PartialView("_Traspasosalmacencostesadicionales", model);
        }

        // GET: api/Supercuentas/5
        public void TraspasosalmacenRefresh()
        {
            var model = Session[session] as List<TraspasosalmacenLinModel>;
            var decimales = model.FirstOrDefault()?.Decimalesmonedas ?? 0;
            var porcentajedescuentopp = HttpContext.Request.Params["porcentajedescuentopp"];
            var porcentajedescuentocomercial = HttpContext.Request.Params["porcentajedescuentocomercial"];
            var fkregimeniva = HttpContext.Request.Params["fkregimeniva"];
            var service = FService.Instance.GetService(typeof(TraspasosalmacenModel), ContextService) as TraspasosalmacenService;
            var lineas = service.RecalculaLineas(model, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, fkregimeniva, 0, decimales);
            Session[session] = lineas.ToList();
            
        }

        #endregion

        #region Action toolbar

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as TraspasosalmacenModel;

            var result = base.EditToolbar(service, model).ToList();

            result.Add(new ToolbarSeparatorModel());
           

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

        private ToolbarActionComboModel CreateComboEstados(TraspasosalmacenModel objModel)
        {
            var estadosService = new EstadosService(ContextService);

            var estados = estadosService.GetStates(DocumentoEstado.Traspasosalmacen, objModel.Tipoestado(ContextService));
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-refresh",
                Texto = General.LblCambiarEstado,
                Url = "#",
                Desactivado = true,
                Items = estados.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("CambiarEstado", "Traspasosalmacen", new { documentoReferencia = objModel.Id, estadoNuevo = f.CampoId, returnUrl = Url.Action("Edit", "Traspasosalmacen", new { id = objModel.Id }) }),
                    Texto = f.Descripcion
                })
            };
        }

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as TraspasosalmacenModel;
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

        private ToolbarActionComboModel CreateComboImprimir(TraspasosalmacenModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.Traspasosalmacen, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.Traspasosalmacen, reportId = f }),
                    Texto = f,
                    Target = "_blank"

                })
            };
        }


        #endregion

        #region Helper

        protected override IGestionService createService(IModelView model)
        {
            
            var result = FService.Instance.GetService(typeof(TraspasosalmacenModel), ContextService) as TraspasosalmacenService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }

        #endregion
    }
}