using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Interfaces;
using DevExpress.Web.Mvc;
using System.Net;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class CriteriosagrupacionController : GenericController<CriteriosagrupacionModel>
    {
        private const string session = "_Criteriosagrupacionlin_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "mantCriteriosagrupacion";
            var permisos = appService.GetPermisosMenu("mantCriteriosagrupacion");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public CriteriosagrupacionController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var model = TempData["model"] == null ? Helper.fModel.GetModel<CriteriosagrupacionModel>(ContextService) : TempData["model"] as CriteriosagrupacionModel;
            using (var service = new CriteriosagrupacionService(ContextService))
            {
                if (TempData["model"] == null)
                {
                    Session[session] = model.Lineas;
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(service, TipoOperacion.Alta, model);
            }

            return View(model);

        }

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(CriteriosagrupacionModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[session] as List<CriteriosagrupacionLinModel>;
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
            var newModel = Helper.fModel.GetModel<CriteriosagrupacionModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                if (TempData["model"] != null)
                    return View(TempData["model"]);

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((CriteriosagrupacionModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(CriteriosagrupacionModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {

                        model.Lineas = Session[session] as List<CriteriosagrupacionLinModel>;
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

            var newModel = Helper.fModel.GetModel<CriteriosagrupacionModel>(ContextService);
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
        public ActionResult CriteriosagrupacionLin()
        {
            var model = Session[session] as IEnumerable<CriteriosagrupacionLinModel>;
            return PartialView("_criteriosagrupacionlin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CriteriosagrupacionLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] CriteriosagrupacionLinModel item)
        {
            var model = Session[session] as List<CriteriosagrupacionLinModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    var max = model.Any() ? model.Max(f => f.Id) : 0;
                    item.Id = max + 1;
                    model.Add(item);

                    Session[session] = model;
                }
            }
            catch (ValidationException)
            {
                model.Remove(item);
                throw;
            }

            return PartialView("_criteriosagrupacionlin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CriteriosagrupacionLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] CriteriosagrupacionLinModel item)
        {
            var model = Session[session] as List<CriteriosagrupacionLinModel>;

            if (ModelState.IsValid)
            {
                var editItem = model.Single(f => f.Id == item.Id);
                editItem.Campo = item.Campo;
                editItem.Orden = item.Orden;
                if (model.Count(f => f.Campo == editItem.Campo) > 1)
                    throw new ValidationException("No se puede repetir el campo de la agrupación");
                Session[session] = model;
            }

            return PartialView("_criteriosagrupacionlin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CriteriosagrupacionLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<CriteriosagrupacionLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            return PartialView("_criteriosagrupacionlin", model);
        }

        #endregion

        #region Helper



        #endregion

    }
}