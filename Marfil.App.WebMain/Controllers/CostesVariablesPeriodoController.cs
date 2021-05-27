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
//using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Resources;
using Marfil.Dom.Persistencia.Model.CRM;

namespace Marfil.App.WebMain.Controllers
{
    public class CostesVariablesPeriodoController : GenericController<CostesVariablesPeriodoModel>, ICostesVariablesPeriodoService
    {

        private const string session = "_Costesvariablesperiodo_";

        #region constructor
        public CostesVariablesPeriodoController(IContextService context) : base(context)
        {
        }
        #endregion

        #region seguridad controlador
        public override bool CanCrear { get; set; }

        public override bool CanEliminar { get; set; }

        public override bool CanModificar { get; set; }

        public override bool IsActivado { get; set; }

        public override string MenuName { get; set; }

        protected override void CargarParametros()
        {

            MenuName = "CostesVariablesPeriodo";
            var permisos = appService.GetPermisosMenu("CostesVariablesPeriodo");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }
        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            using (var gestionService = FService.Instance.GetService(typeof(CostesVariablesPeriodoModel), ContextService))
            {
                var model = TempData["model"] == null ? Helper.fModel.GetModel<CostesVariablesPeriodoModel>(ContextService) : TempData["model"] as CostesVariablesPeriodoModel;
                Session[session] = model._costes;

                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(CostesVariablesPeriodoModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        model._costes = Session[session] as List<CostesVariablesPeriodoLinModel>;

                        /*
                        int count = 1;

                        foreach(var linea in model._costes)
                        {
                            linea.Id = count;
                            count++;
                        }
                        */

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

        //Sobreescribir el details
        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<CostesVariablesPeriodoModel>(ContextService);
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

        //Sobreescribir el Edit
        public override ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newModel = Helper.fModel.GetModel<CostesVariablesPeriodoModel>(ContextService);

            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as CostesVariablesPeriodoModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((CostesVariablesPeriodoModel)model)._costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(CostesVariablesPeriodoModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {

                        model._costes = Session[session] as List<CostesVariablesPeriodoLinModel>;
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

        #region Grid devexpress

        [ValidateInput(false)]
        public ActionResult CostesVariablesPeriodoLin()
        {
            if (Session["valorescostesvariablesperiodo"] == null)
            {

                var list = WebHelper.GetApplicationHelper().GetCRMValores().Select(f => new SelectListItem() { Value = f.Descripcion, Text = f.Valor + "-" + f.Descripcion }).ToList();
                Session["valorescostesvariablesperiodo"] = list;
            }

            var model = Session[session] as List<CostesVariablesPeriodoLinModel>;
            return PartialView("_costesvariablesperiodolin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesVariablesPeriodoLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] CostesVariablesPeriodoLinModel item)
        {
            var model = Session[session] as List<CostesVariablesPeriodoLinModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Any(f => f.Descripcion == item.Descripcion))
                    {
                        ModelState.AddModelError("Descripcion", string.Format(General.ErrorRegistroExistente));
                    }
                    else
                    {

                        //int total = model.Count();
                        //Ir actualizando los ID en caso de que sufran cambios (eliminacion de lineas)
                        if (model.Count() > 1)
                        {
                            int count = 1;
                            foreach (var linea in model)
                            {

                                linea.Id = count;
                                count++;
                            }

                            item.Id = count;
                        }

                        else
                        {
                            item.Id = model.Count() + 1;

                        }

                        item.Tablavaria = "2130";
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

            return PartialView("_costesvariablesperiodolin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesVariablesPeriodoLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] CostesVariablesPeriodoLinModel item)
        {
            var model = Session[session] as List<CostesVariablesPeriodoLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Tablavaria = "2130";
                    editItem.Descripcion = item.Descripcion;
                    editItem.Precio = item.Precio;
                    Session[session] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_costesvariablesperiodolin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesVariablesPeriodoLinDelete(string id)
        {
            var model = Session[session] as List<CostesVariablesPeriodoLinModel>;
            model.Remove(model.Single(f => f.Id.ToString() == id));

            //Ir actualizando los ID en caso de que sufran cambios (eliminacion de lineas)
            if (model.Count() >= 1)
            {
                int count = 1;
                foreach (var linea in model)
                {

                    linea.Id = count;
                    count++;
                }
            }

            else
            {
                model[0].Id = 1;

            }

            Session[session] = model;
            return PartialView("_costesvariablesperiodolin", model);
        }
        #endregion
    }
}
