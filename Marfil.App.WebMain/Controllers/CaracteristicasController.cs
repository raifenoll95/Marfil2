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
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class CaracteristicasController : GenericController<CaracteristicasModel>
    {
        private const string session = "_Caracteristicas_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "caracteristicas";
            var permisos = appService.GetPermisosMenu("caracteristicas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public CaracteristicasController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            using (var gestionService = FService.Instance.GetService(typeof(CaracteristicasModel), ContextService))
            {
                var model = TempData["model"] == null ? Helper.fModel.GetModel<CaracteristicasModel>(ContextService) : TempData["model"] as CaracteristicasModel;
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
        public override ActionResult CreateOperacion(CaracteristicasModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[session] as List<CaracteristicasLinModel>;
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
            var newModel = Helper.fModel.GetModel<CaracteristicasModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null? TempData["model"] as CaracteristicasModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((CaracteristicasModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(CaracteristicasModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {

                        model.Lineas = Session[session] as List<CaracteristicasLinModel>;
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

            var newModel = Helper.fModel.GetModel<CaracteristicasModel>(ContextService);
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
        public ActionResult CaracteristicasLin()
        {
            var model = Session[session] as List<CaracteristicasLinModel>;
            return PartialView("_Caracteristicaslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CaracteristicasLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] CaracteristicasLinModel item)
        {
            var model = Session[session] as List<CaracteristicasLinModel>;
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
                        item.Id = Funciones.RellenaCod(item.Id, 2);

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

            return PartialView("_Caracteristicaslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CaracteristicasLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] CaracteristicasLinModel item)
        {
            var model = Session[session] as List<CaracteristicasLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Descripcion = item.Descripcion;
                    editItem.Descripcion2 = item.Descripcion2;
                    editItem.Descripcionabreviada = item.Descripcionabreviada;
                    Session[session] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_Caracteristicaslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CaracteristicasLinDelete(string id)
        {
            var model = Session[session] as List<CaracteristicasLinModel>;
            model.Remove(model.Single(f => f.Id == id));
            Session[session] = model;
            return PartialView("_Caracteristicaslin", model);
        }

        #endregion

        #region Helper



        #endregion
    }
}