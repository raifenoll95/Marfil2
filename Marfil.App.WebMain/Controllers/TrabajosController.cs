using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using DevExpress.Web.Mvc;
using System.Net;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.App.WebMain.Misc;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class TrabajosController : GenericController<TrabajosModel>
    {
        private const string SessionLin = "_trabajoslin_";
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "trabajos";
            var permisos = appService.GetPermisosMenu("trabajos");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public TrabajosController(IContextService context) : base(context)
        {

        }

        #endregion


        #region CRUD

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var model = TempData["model"] == null ? Helper.fModel.GetModel<TrabajosModel>(ContextService) : TempData["model"] as TrabajosModel;
            using (var service = new TrabajosService(ContextService))
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
        public override ActionResult CreateOperacion(TrabajosModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[SessionLin] as IEnumerable<TrabajosLinModel>;
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
            var newModel = Helper.fModel.GetModel<TrabajosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = TempData["model"] != null ? TempData["model"] as TrabajosModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[SessionLin] = ((TrabajosModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(TrabajosModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[SessionLin] as IEnumerable<TrabajosLinModel>;
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

            var newModel = Helper.fModel.GetModel<TrabajosModel>(ContextService);
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

        #endregion

        #region Grid Devexpress

        [ValidateInput(false)]
        public ActionResult TrabajosLin()
        {
            var model = Session[SessionLin] as IEnumerable<TrabajosLinModel>;
            return PartialView("TrabajosLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TrabajosLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] TrabajosLinModel item)
        {
            var model = Session[SessionLin] as List<TrabajosLinModel> ?? new List<TrabajosLinModel>();
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

            return PartialView("TrabajosLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TrabajosLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TrabajosLinModel item)
        {
            var model = Session[SessionLin] as List<TrabajosLinModel>;

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

            return PartialView("TrabajosLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TrabajosLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[SessionLin] as List<TrabajosLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[SessionLin] = model;
            return PartialView("TrabajosLin", model);
        }

        #endregion
    }
}