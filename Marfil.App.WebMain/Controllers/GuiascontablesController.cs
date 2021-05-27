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
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class CuentasCombo
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
    }
    public class GuiascontablesController : GenericController<GuiascontablesModel>
    {
        private const string SessionLin = "_guiascontableslin_";
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "guiascontables";
            var permisos = appService.GetPermisosMenu("guiascontables");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public GuiascontablesController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Create

        public override ActionResult Create()
        {

            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var model = TempData["model"] == null ? Helper.fModel.GetModel<GuiascontablesModel>(ContextService): TempData["model"] as GuiascontablesModel;
            using (var service = createService(model))
            {
                if (TempData["model"] == null)
                {
                    Session[SessionLin] = model.Lineas;
                }
                 ((IToolbar)model).Toolbar = GenerateToolbar(service, TipoOperacion.Alta, model);
                return View(model);

            }

        }

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(GuiascontablesModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        var auxModel = Session[SessionLin] as List<GuiascontablesLinModel>;
                        model.Lineas = auxModel;
                        UpdateLineas(model);
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
            var newModel = Helper.fModel.GetModel<GuiascontablesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as GuiascontablesModel : gestionService.get(id);
                
                if (model == null)
                {
                    return HttpNotFound();
                }

                Session[SessionLin] = ((GuiascontablesModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(GuiascontablesModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {

                        var auxModel = Session[SessionLin] as List<GuiascontablesLinModel>;
                        model.Lineas = auxModel;
                        UpdateLineas(model);
                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;

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

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var newModel = Helper.fModel.GetModel<GuiascontablesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                ViewBag.ReadOnly = true;
                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[SessionLin] = newModel.Lineas;
                ViewBag.ReadOnly = true;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                return View(model);
            }
        }

        #endregion


        #region Grid Devexpress

        [ValidateInput(false)]
        public ActionResult GuiascontablesLin()
        {
            var fService = FService.Instance;
            if (Session["cuentasguiascontables"] == null)
            {
                var cuentasService = fService.GetService(typeof(CuentasModel), ContextService) as CuentasService;
                var list = cuentasService.GetCuentasClientes().Select(f => new CuentasCombo() { Id = f.Id, Descripcion = f.Id + "-" + f.Descripcion }).ToList();
                list.Insert(0, new CuentasCombo());
                Session["cuentasguiascontables"] = list;
            }
            if (Session["regimeniva"] == null)
            {
                var regimenivaService = fService.GetService(typeof(RegimenIvaModel), ContextService);
                Session["regimeniva"] = regimenivaService.getAll().Select(f => new CuentasCombo { Id = ((RegimenIvaModel)f).Id, Descripcion = ((RegimenIvaModel)f).Id + "-" + ((RegimenIvaModel)f).Descripcion });
            }

            var model = Session[SessionLin] as IList<GuiascontablesLinModel>;
            return PartialView("_guiascontableslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuiascontablesLinAddNew([ModelBinder(typeof(CustomDevExpressEditorsBinder))] GuiascontablesLinModel item)
        {
            var model = Session[SessionLin] as IList<GuiascontablesLinModel>;
            try
            {
                if (ModelState.IsValid && !model.Any(f => item.Fkregimeniva == f.Fkregimeniva))
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



            return PartialView("_guiascontableslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuiascontablesLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] GuiascontablesLinModel item)
        {
            var model = Session[SessionLin] as IList<GuiascontablesLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Fkcuentascompras = item.Fkcuentascompras;
                    editItem.Fkcuentasventas = item.Fkcuentasventas;
                    editItem.Fkcuentasdevolucioncompras = item.Fkcuentasdevolucioncompras;
                    editItem.Fkcuentasdevolucionventas = item.Fkcuentasdevolucionventas;

                    Session[SessionLin] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_guiascontableslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GuiascontablesLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[SessionLin] as IList<GuiascontablesLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[SessionLin] = model;
            return PartialView("_guiascontableslin", model);
        }

        #endregion

        #region Helper

        private void UpdateLineas(GuiascontablesModel model)
        {
            foreach (var item in model.Lineas)
            {
                item.Empresa = model.Empresa;
                item.Fkguiascontables = model.Id;
            }
        }

        #endregion
    }
}