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
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Resources;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;

namespace Marfil.App.WebMain.Controllers
{
    public class BundleController : GenericController<BundleModel>
    {
        private const string session = "_Bundle_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "bundle";
            var permisos = appService.GetPermisosMenu("bundle");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public BundleController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        [HttpPost]
        public ActionResult AgregarArticulosLotesApi(IEnumerable<StockActualVistaModel> listado)
        {
            var mierror = string.Empty;
            try
            {
                var BundleService = FService.Instance.GetService(typeof(BundleModel), ContextService) as BundleService;
                var lineasActuales = Session[session] as List<BundleLinModel>;

                Session[session] = BundleService.AgregarLineasBundle(lineasActuales, listado);

                return new EmptyResult();
            }
            catch (Exception ex)
            {
                mierror = ex.Message;
            }
            Response.StatusCode = 200;
            return Content("{\"error\":\"" + mierror + "\"}", "application/json", Encoding.UTF8);
        }

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var model = TempData["model"] == null ? Helper.fModel.GetModel<BundleModel>(ContextService) : TempData["model"] as BundleModel;
            using (var gestionService = createService(model))
            {
                if (TempData["model"] == null)
                {
                    Session[session] = model.Lineas;
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }
        }

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(BundleModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[session] as List<BundleLinModel>;
                        gestionService.create(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                   .SelectMany(x => x.Errors)
                   .Select(x => x.ErrorMessage));
                model.Lineas = Session[session] as List<BundleLinModel>;
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                model.Lineas = Session[session] as List<BundleLinModel>;
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
            var newModel = Helper.fModel.GetModel<BundleModel>(ContextService);
            using (var gestionService = createService(newModel))
            {



                var model = TempData["model"] != null ? TempData["model"] as BundleModel : gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((BundleModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(BundleModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {

                        model.Lineas = Session[session] as List<BundleLinModel>;
                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                model.Lineas = Session[session] as List<BundleLinModel>;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = model.CampoId });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                model.Lineas = Session[session] as List<BundleLinModel>;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = model.CampoId });
            }
        }

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<BundleModel>(ContextService);
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

        #endregion

        #region Toolbar

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var result = base.VerToolbar(service, model).ToList();
            var viewmodel = model as BundleModel;
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(viewmodel));

            return result;
        }

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var result = base.EditToolbar(service, model).ToList();
            var viewmodel = model as BundleModel;
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(viewmodel));
            return result;
        }

        private ToolbarActionComboModel CreateComboImprimir(BundleModel objModel)
        {
            objModel.DocumentosImpresion = WebHelper.GetListFormatos(TipoDocumentoImpresion.Bundle, string.Format("{0}{1}",objModel.Lote,objModel.Codigo));
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = string.Format("{0}{1}", objModel.Lote, objModel.Codigo), tipo = TipoDocumentoImpresion.Bundle, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = string.Format("{0}{1}", objModel.Lote, objModel.Codigo), tipo = TipoDocumentoImpresion.Bundle, reportId = f }),
                    Texto = f,
                    Target = "_blank"

                })
            };
        }

        #endregion

        #region Grid Devexpress


        [ValidateInput(false)]
        public ActionResult BundleLin()
        {
            var model = Session[session] as List<BundleLinModel>;
            return PartialView("_BundleLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BundleLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] BundleLinModel item)
        {
            var model = Session[session] as List<BundleLinModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Any(f => f.Id == item.Id))
                    {
                        ModelState.AddModelError("Id", string.Format(General.ErrorRegistroExistente));
                    }
                    else
                    {
                        var max = model.Any() ? model.Max(f => f.Id) + 1 : 1;
                        item.Id = max;
                        model.Add(item);
                        Session[session] = model;
                    }

                }
            }
            catch (ValidationException)
            {
                model.Remove(item);
                throw;
            }



            return PartialView("_BundleLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BundleLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] BundleLinModel item)
        {
            var model = Session[session] as List<BundleLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Descripcion = item.Descripcion;
                    
                    Session[session] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_BundleLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BundleLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<BundleLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            return PartialView("_BundleLin", model);
        }

        #endregion
    }
}