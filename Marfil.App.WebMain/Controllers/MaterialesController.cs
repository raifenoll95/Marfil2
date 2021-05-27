using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using Marfil.Dom.ControlsUI.Toolbar;

namespace Marfil.App.WebMain.Controllers
{
    public class MaterialesController : GenericController<MaterialesModel>
    {
        private const string session = "_materiales_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "materiales";
            var permisos = appService.GetPermisosMenu("materiales");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public MaterialesController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            using (var gestionService = FService.Instance.GetService(typeof(MaterialesModel), ContextService))
            {
                var model = TempData["model"] == null ? Helper.fModel.GetModel<MaterialesModel>(ContextService) : TempData["model"] as MaterialesModel;
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
        public override ActionResult CreateOperacion(MaterialesModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    
                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[session] as List<MaterialesLinModel>;
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
            var newModel = Helper.fModel.GetModel<MaterialesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as MaterialesModel: gestionService.get(id);
                
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((MaterialesModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(MaterialesModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        
                        model.Lineas = Session[session] as List<MaterialesLinModel>;
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

            var newModel = Helper.fModel.GetModel<MaterialesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                
                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ReadOnly = true;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                return View(model);
            }
        }

        #region Grid Devexpress


        [ValidateInput(false)]
        public ActionResult MaterialesLin()
        {
            var model = Session[session] as List<MaterialesLinModel>;
            return PartialView("_materialeslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MaterialesLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] MaterialesLinModel item)
        {
            var model = Session[session] as List<MaterialesLinModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Any(f => f.Codigovariedad == item.Codigovariedad))
                    {
                        ModelState.AddModelError("Codigovariedad",string.Format(General.ErrorRegistroExistente));
                    }
                    else
                    {
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



            return PartialView("_materialeslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MaterialesLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] MaterialesLinModel item)
        {
            var model = Session[session] as List<MaterialesLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Codigovariedad == item.Codigovariedad);
                    editItem.Descripcion = item.Descripcion;
                    editItem.Descripcion2 = item.Descripcion2;

                    Session[session] = model;


                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_materialeslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MaterialesLinDelete(string Codigovariedad)
        {
            var model = Session[session] as List<MaterialesLinModel>;
            model.Remove(model.Single(f => f.Codigovariedad == Codigovariedad));
            Session[session] = model;
            return PartialView("_materialeslin", model);
        }

        #endregion

        #region Helper



        #endregion
    }
}