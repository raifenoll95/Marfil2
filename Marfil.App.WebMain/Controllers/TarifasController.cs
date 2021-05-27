using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Resources;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class TarifasController : GenericController<TarifasModel>
    {
        private const string session = "_tarifas_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "tarifas";
            var permisos = appService.GetPermisosMenu("tarifas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public TarifasController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            using (var gestionService = FService.Instance.GetService(typeof(TarifasModel), ContextService))
            {
                var model = TempData["model"] == null ? Helper.fModel.GetModel<TarifasModel>(ContextService) : TempData["model"] as TarifasModel;
                Session[session] = model.Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }


        }

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(TarifasModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[session] as List<TarifasLinModel>;
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
            var newModel = Helper.fModel.GetModel<TarifasModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = (TempData["model"] != null) ? TempData["model"]  as TarifasModel: gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }

                Session[session] = ((TarifasModel)model);
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(TarifasModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {

                        var aux = Session[session] as TarifasModel;
                        model.Lineas = aux.Lineas;
                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = obj.get(objExt.primaryKey.First().Name) });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = obj.get(objExt.primaryKey.First().Name) });
            }
        }

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<TarifasModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ReadOnly = true;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bloquear(string id, string returnurl, string motivo, bool operacion)
        {
            if (CanBloquear)
            {
                
                using (var gestionService = FService.Instance.GetService(typeof(TarifasModel),ContextService) as TarifasService)
                {
                    gestionService.Bloquear(id, motivo, ContextService.Id.ToString(), operacion);
                }
            }
            else
            {
                ModelState.AddModelError("", General.LblErrorBloqueoNoPermitido);
            }

            return Redirect(returnurl);
        }

        public ActionResult ExportTo(string exportid, string OutputFormat)
        {
            ActionResult obj;
            var model = Session[session] as TarifasModel;
            var settings = Session["TarifasSettings"]  as GridViewSettings;
            if (settings.Columns["Action"] != null)
                settings.Columns.Remove(settings.Columns["Action"]);

            switch (OutputFormat.ToUpper())
            {
                case "CSV":
                    obj = GridViewExtension.ExportToCsv(settings, model.Lineas, true);
                    break;
                case "PDF":
                    obj = GridViewExtension.ExportToPdf(settings, model.Lineas, true);
                    break;
                case "RTF":
                    obj = GridViewExtension.ExportToRtf(settings, model.Lineas, true);
                    break;
                case "XLS":
                    obj = GridViewExtension.ExportToXls(settings, model.Lineas, true);
                    break;
                case "XLSX":
                    obj = GridViewExtension.ExportToXlsx(settings, model.Lineas, true);
                    break;
                default:
                    obj = RedirectToAction("Index");
                    break;
            }

            var result = obj as FileStreamResult;
            result.FileDownloadName = (model.Descripcion ?? Tarifas.TituloEntidad) + "." + OutputFormat.ToLower();
            return result;
        }

        #region Grid Devexpress


        [ValidateInput(false)]
        public ActionResult TarifasLin()
        {
            var model = Session[session] as TarifasModel;
            return PartialView("_listadotarifas", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TarifasLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] TarifasLinModel item)
        {
            var model = Session[session] as TarifasModel;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Lineas.Any(f => f.Fkarticulos == item.Fkarticulos))
                    {
                        ModelState.AddModelError("Codigovariedad", string.Format(General.ErrorRegistroExistente));
                    }
                    else
                    {
                        if (model.Precioobligatorio && !item.Precio.HasValue && item.Precio.Value == 0.0)
                        {
                            ModelState.AddModelError("Precio", string.Format(General.ErrorCampoObligatorio, Tarifas.Precio));
                            return PartialView("_listadotarifas", model);
                        }

                        var service = FService.Instance.GetService(typeof(UnidadesModel), ContextService);
                        var serviceFamilia = FService.Instance.GetService(typeof(FamiliasproductosModel), ContextService);
                        var serviceEmpresa = FService.Instance.GetService(typeof(EmpresaModel), ContextService);
                        var empresa = serviceEmpresa.get(ContextService.Empresa) as EmpresaModel;

                        var familiaObj = serviceFamilia.get(item.Fkarticulos.Substring(0, 2)) as FamiliasproductosModel;

                        var unidadesObj = service.get(familiaObj.Fkunidadesmedida) as UnidadesModel;
                        item.Unidades = unidadesObj.Codigounidad;
                        item.Precio = Math.Round(item.Precio ?? 0, empresa.Decimalesprecios ?? 2);

                        model.Lineas.Add(item);
                        Session[session] = model;
                    }

                }
            }
            catch (ValidationException)
            {
                model.Lineas.Remove(item);
                throw;
            }



            return PartialView("_listadotarifas", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TarifasLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TarifasLinModel item)
        {
            var model = Session[session] as TarifasModel;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Lineas.Single(f => f.Fkarticulos == item.Fkarticulos);
                    if (model.Precioobligatorio && !item.Precio.HasValue && item.Precio.Value == 0.0)
                    {
                        ModelState.AddModelError("Precio", string.Format(General.ErrorCampoObligatorio, Tarifas.Precio));
                        return PartialView("_listadotarifas", model);
                    }

                    var serviceEmpresa = FService.Instance.GetService(typeof(EmpresaModel), ContextService);
                    var empresa = serviceEmpresa.get(ContextService.Empresa) as EmpresaModel;

                    editItem.Precio = Math.Round(item.Precio ?? 0, empresa.Decimalesprecios ?? 2);
                    editItem.Descuento = item.Descuento;

                    Session[session] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_listadotarifas", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TarifasLinDelete(string Fkarticulos)
        {
            var model = Session[session] as TarifasModel;
            model.Lineas.Remove(model.Lineas.Single(f => f.Fkarticulos == Fkarticulos));
            Session[session] = model;
            return PartialView("_listadotarifas", model);
        }

        public ActionResult getPrecio(string componente, string unidadmedida)
        {
            var servicioTarifas = FService.Instance.GetService(typeof(TarifasModel), ContextService) as TarifasService;
            var precio = servicioTarifas.getPrecioComponentes(componente);
            var serviceArticulos = FService.Instance.GetService(typeof(ArticulosModel), ContextService) as ArticulosService;
            var articuloModel = serviceArticulos.get(componente) as ArticulosModel;
            var metros = 0;

            var preciototal = precio;
            var data = JsonConvert.SerializeObject(preciototal, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}