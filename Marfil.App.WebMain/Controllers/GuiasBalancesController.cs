using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using DevExpress.Web.Mvc;
using Marfil.Dom.Persistencia.ServicesView;
using Resources;
using Marfil.App.WebMain.Misc;

namespace Marfil.App.WebMain.Controllers
{
    public class GuiasBalancesController : GenericController<GuiasBalancesModel>
    {
        // GET: GuiasBalances
        public GuiasBalancesController(IContextService context) : base(context)
        {
        }

        string SessionGuiasBalancesLineas = "_SGuiasBalancesLineas";
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "GuiasBalances";
            var permisos = appService.GetPermisosMenu(MenuName);
            IsActivado = true;
            CanCrear = true;
            CanModificar = true;
            CanEliminar = true;
        }
        #region Create
        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            using (var gestionService = FService.Instance.GetService(typeof(GuiasBalancesModel), ContextService))
            {
                var modelview = Helper.fModel.GetModel<GuiasBalancesModel>(ContextService) as IModelView;
                if (TempData["model"] != null)
                    modelview = TempData["model"] as IModelView;
                ((IToolbar)modelview).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, modelview);
                return View(modelview);
            }
        }
        public override ActionResult CreateOperacion(GuiasBalancesModel model)
        {
            try
            {
                var a = 3;

                var modelview = Helper.fModel.GetModel<GuiasBalancesModel>(ContextService);
                model.GuiasBalancesLineas = GetListGuiasBalancesLineas.ToList();

                using (var gestionService = createService(modelview))
                {
                    ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                    if (ModelState.IsValid)
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

            var newModel = Helper.fModel.GetModel<GuiasBalancesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as GuiasBalancesModel : gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }

                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                SetListGuiasBalancesLineas(((GuiasBalancesModel)model).GuiasBalancesLineas);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(GuiasBalancesModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        model.GuiasBalancesLineas = GetListGuiasBalancesLineas.ToList();
                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                //return base.EditOperacion(model);
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
        #endregion
        #region Details
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<GuiasBalancesModel>(ContextService);
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
        #region Delete
        public override ActionResult Delete(string id)
        {
            return base.Delete(id);
        }

        public override ActionResult DeleteConfirmed(string id)
        {
            return base.DeleteConfirmed(id);
        }
        #endregion

        #region Gias de Balances Lineas
        [ValidateInput(false)]
        public ActionResult GuiasBalancesLineas() => PartialView("_GuiasBalancesLineasGrid", GetListGuiasBalancesLineas);

        [HttpPost, ValidateInput(false)]
        public ActionResult GuiasBalancesLineasAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] GuiasBalancesLineasModel item)
        {
            var items = GetListGuiasBalancesLineas;
            item.Id = items.Count() + 1;
            try
            {
                if (ModelState.IsValid)
                {
                    items.Add(item);
                    SetListGuiasBalancesLineas(items);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return PartialView("_GuiasBalancesLineasGrid", items);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuiasBalancesLineasUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] GuiasBalancesLineasModel item)
        {
            var items = GetListGuiasBalancesLineas;
            try
            {
                if (ModelState.IsValid)
                {
                    var EditItem = items.Single(s => s.Id == item.Id);
                    EditItem.GuiaId = item.GuiaId;
                    EditItem.GuiasBalancesId = item.GuiasBalancesId;
                    EditItem.Id = item.Id;
                    EditItem.InformeId = item.InformeId;
                    EditItem.orden = item.orden;
                    EditItem.signo = item.signo;
                    EditItem.signoea = item.signoea;
                    SetListGuiasBalancesLineas(items);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return PartialView("_GuiasBalancesLineasGrid", items);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuiasBalancesLineasDelete(int id)
        {
            var items = GetListGuiasBalancesLineas;
            items.Remove(items.Single(s => s.Id == id));

            //Ir actualizando los ID en caso de que sufran cambios (eliminacion de lineas) EN CASO DE QUE QUEDEN LINEAS
            if (items.Count() >= 1)
            {
                int count = 1;
                foreach (var linea in items)
                {

                    linea.Id = count;
                    count++;
                }
            }

            return PartialView("_GuiasBalancesLineasGrid", items);
        }

        void SetListGuiasBalancesLineas(List<GuiasBalancesLineasModel> guiasBalancesLineas) => Session[SessionGuiasBalancesLineas] = guiasBalancesLineas;
        List<GuiasBalancesLineasModel> GetListGuiasBalancesLineas => Session[SessionGuiasBalancesLineas] as List<GuiasBalancesLineasModel>;
        #endregion
    }
}