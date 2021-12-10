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
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;
using Marfil.Dom.Persistencia;

namespace Marfil.App.WebMain.Controllers
{
    public class GuiasBalancesController : GenericController<GuiasBalancesModel>
    {
        // GET: GuiasBalances
        public GuiasBalancesController(IContextService context) : base(context)
        {
        }
        private const string session = "_guiasBalances_";
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

            var model = TempData["model"] == null ? Helper.fModel.GetModel<GuiasBalancesModel>(ContextService) : TempData["model"] as GuiasBalancesModel;
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

        public override ActionResult CreateOperacion(GuiasBalancesModel model)
        {
            try
            {
                using (var gestionService = createService(model))
                {
                    using (var guiasBalancesService = new GuiasBalancesService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)))
                    {
                        if (ModelState.IsValid)
                        {
                            model.Lineas = Session[session] as List<GuiasBalancesLineasModel>;
                            gestionService.create(model);
                            //Elimina lineas con idCab = null
                            guiasBalancesService.DeleteAllLin();
                            TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                            return RedirectToAction("Index");
                        }
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
                Session[session] = ((GuiasBalancesModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                //SetListGuiasBalancesLineas(((GuiasBalancesModel)model).Lineas);
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
                        using (var guiasBalancesService = new GuiasBalancesService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)))
                        {
                            model.Lineas = Session[session] as List<GuiasBalancesLineasModel>;
                            gestionService.edit(model);
                            //Elimina lineas con idCab = null
                            guiasBalancesService.DeleteAllLin();
                            TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                            return RedirectToAction("Index");
                        }
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                //return base.EditOperacion(model);
                return RedirectToAction("Edit", new { id = new { id = obj.get(objExt.primaryKey.First().Name) } });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = new { id = obj.get(objExt.primaryKey.First().Name) } });
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
                Session[session] = newModel.Lineas;
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

        #region Guias de Balances Lineas
        [ValidateInput(false)]
        public ActionResult GuiasBalancesLineas()
        {
            var modelLineas = Session[session] as List<GuiasBalancesLineasModel>;
            return PartialView("_lineasGridView", modelLineas);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuiasBalancesLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] GuiasBalancesLineasModel item)
        {
            var model = Session[session] as List<GuiasBalancesLineasModel>;
            try
            {
                if (ModelState.IsValid && !model.Any(f => item.Id == f.Id))
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
            return PartialView("_lineasGridView", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuiasBalancesLineasUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] GuiasBalancesLineasModel item)
        {
            var model = Session[session] as List<GuiasBalancesLineasModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    var EditItem = model.Single(s => s.Id == item.Id);
                    //EditItem.GuiaId = item.GuiaId;
                    //EditItem.GuiasBalancesId = item.GuiasBalancesId;
                    EditItem.Id = item.Id;
                    //EditItem.InformeId = item.InformeId;
                    //EditItem.Orden = item.Orden;
                    EditItem.Cuenta = item.Cuenta;
                    EditItem.Signo = item.Signo;
                    EditItem.Signoea = item.Signoea;

                    Session[session] = model;
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return PartialView("_lineasGridView", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuiasBalancesLineasDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<GuiasBalancesLineasModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;

            return PartialView("_lineasGridView", model);
        }

        #endregion

        [HttpGet]
        public bool CuentasNoAsignadas()
        {
            var cuentas = false;
            using (var service = FService.Instance.GetService(typeof(GuiasBalancesModel), ContextService) as GuiasBalancesService)
            {
                cuentas = service.HayCuentasNoAsignadas();
            }

            return cuentas;
        }
    }
}