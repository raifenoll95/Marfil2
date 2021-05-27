using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using DevExpress.Web.Mvc;
using System.Net;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.ServicesView;
using Resources;


namespace Marfil.App.WebMain.Controllers
{
    public class TareasController : GenericController<TareasModel>
    {
        private const string SessionLin = "_tareaslin_";        
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "tareas";
            var permisos = appService.GetPermisosMenu("tareas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public TareasController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var model = TempData["model"] == null ? Helper.fModel.GetModel<TareasModel>(ContextService) : TempData["model"] as TareasModel;
            using (var service = new TareasService(ContextService))
            {
                if (TempData["model"] == null)
                {
                    //model.Id = service.NextId();   
                    Session[SessionLin] = model.Lineas;
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(service, TipoOperacion.Alta, model);
            }

            return View(model);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(TareasModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[SessionLin] as IEnumerable<TareasLinModel>;
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
            var newModel = Helper.fModel.GetModel<TareasModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = TempData["model"] != null ? TempData["model"] as TareasModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[SessionLin] = ((TareasModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(TareasModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[SessionLin] as IEnumerable<TareasLinModel>;
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

        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<TareasModel>(ContextService);
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
        public ActionResult TareasLin()
        {
            var model = Session[SessionLin] as IEnumerable<TareasLinModel>;
            return PartialView("TareasLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TareasLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] TareasLinModel item)
        {
            var model = Session[SessionLin] as List<TareasLinModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    var max = model.Any() ? model.Max(f => f.Id) : 0;                    
                    item.Id = max + 1;
                    model.Add(item);

                    Session[SessionLin] = model;

                }
            }
            catch (ValidationException)
            {
                model.Remove(item);
                throw;
            }

            return PartialView("TareasLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TareasLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TareasLinModel item)
        {
            var model = Session[SessionLin] as List<TareasLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Año = item.Año;
                    editItem.Precio = item.Precio;

                    Session[SessionLin] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("TareasLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TareasLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[SessionLin] as List<TareasLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[SessionLin] = model;
            return PartialView("TareasLin", model);
        }

        #endregion
    }
}