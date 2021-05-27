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
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class GruposIvaController : GenericController<GruposIvaModel>
    {
        private const string SessionLin = "_gruposivalin_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "gruposiva";
            var permisos = appService.GetPermisosMenu("gruposiva");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public GruposIvaController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Create

        public override ActionResult Create()
        {

            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var model = TempData["model"] == null ? Helper.fModel.GetModel<GruposIvaModel>(ContextService) : TempData["model"] as GruposIvaModel;
            using (var service = createService(model))
            {
                Session[SessionLin] = model.Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(service, TipoOperacion.Alta, model);
                return View(model);
            }

            

        }

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(GruposIvaModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        var auxModel = Session[SessionLin] as List<GruposIvaLinModel>;
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
            var newModel = Helper.fModel.GetModel<GruposIvaModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = TempData["model"] != null ? TempData["model"] as GruposIvaModel : gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }

                Session[SessionLin] = ((GruposIvaModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(GruposIvaModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {

                        var auxModel = Session[SessionLin] as List<GruposIvaLinModel>;
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
            var newModel = Helper.fModel.GetModel<GruposIvaModel>(ContextService);
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
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        #endregion

        #region Grid Devexpress

        [ValidateInput(false)]
        public ActionResult GruposIvaLin()
        {
            var fService = FService.Instance;
            var tiposivaService = fService.GetService(typeof(TiposIvaModel), ContextService);
            Session["tiposiva"] = tiposivaService.getAll().Select(f => (TiposIvaModel)f);
            var model = Session[SessionLin] as IList<GruposIvaLinModel>;
            return PartialView("GruposIvaLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GruposIvaLinAddNew([ModelBinder(typeof(CustomDevExpressEditorsBinder))] GruposIvaLinModel item)
        {
            var model = Session[SessionLin] as IList<GruposIvaLinModel>;
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



            return PartialView("GruposIvaLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GruposIvaLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] GruposIvaLinModel item)
        {
            var model = Session[SessionLin] as IList<GruposIvaLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Desde = item.Desde;
                    editItem.FkTiposIvaConRecargo = item.FkTiposIvaConRecargo;
                    editItem.FkTiposIvaSinRecargo = item.FkTiposIvaSinRecargo;
                    editItem.FkTiposIvaExentoIva = item.FkTiposIvaExentoIva;

                    Session[SessionLin] = model;


                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("GruposIvaLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GruposIvaLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[SessionLin] as IList<GruposIvaLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[SessionLin] = model;
            return PartialView("GruposIvaLin", model);
        }

        #endregion

        #region Helper

        private void UpdateLineas(GruposIvaModel model)
        {
            foreach (var item in model.Lineas)
            {
                item.Empresa = model.Empresa;
                item.FkGruposIva = model.Id;
            }
        }

        #endregion
    }
}