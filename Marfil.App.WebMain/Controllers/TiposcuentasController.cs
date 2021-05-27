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
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class TiposcuentasController : GenericController<TiposCuentasModel>
    {
        #region Member

        private readonly string session = "tiposcuentaslin";

        #endregion

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "tiposcuentas";
            var permisos = appService.GetPermisosMenu("tiposcuentas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public TiposcuentasController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Create

        public override ActionResult Create()
        {

            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var model = TempData["model"] == null ? Helper.fModel.GetModel<TiposCuentasModel>(ContextService) : TempData["model"] as TiposCuentasModel;
            using (var service = createService(model))
            {
                if (TempData["model"] == null)
                {
                    Session[session] = model;
                    
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
        public override ActionResult CreateOperacion(TiposCuentasModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        var auxModel = Session[session] as TiposCuentasModel;
                        model.Lineas = auxModel.Lineas;
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
            var newModel = Helper.fModel.GetModel<TiposCuentasModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null? TempData["model"] as TiposCuentasModel : gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }

                Session[session] = model;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(TiposCuentasModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {

                        var auxModel = Session[session] as TiposCuentasModel;
                        model.Lineas = auxModel.Lineas;
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

                return RedirectToAction("Edit", new { id = (int)model.Tipos });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = (int)model.Tipos });
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
            var newModel = Helper.fModel.GetModel<TiposCuentasModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                ViewBag.ReadOnly = true;
                var paises = gestionService.get(id);
                if (paises == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ReadOnly = true;
                ((IToolbar)paises).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, paises);

                return View(paises);
            }
        }

        #endregion

        #region Grid Devexpress


        [ValidateInput(false)]
        public ActionResult TiposcuentasLin()
        {

            var model = Session[session] as TiposCuentasModel;
            using(var cuentasService=new CuentasService(ContextService))
            Session["lineas"] = cuentasService.GetCuentasModel(Empresa, appService.NivelesCuentas(model.Empresa));
            return PartialView("TiposcuentasLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TiposcuentasLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] TiposCuentasLinModel item)
        {

            var model = Session[session] as TiposCuentasModel;
            try
            {
                if (ModelState.IsValid)
                {
                    if (!model.Lineas.Any(f => f.Cuenta == item.Cuenta))
                    {
                        model.Lineas.Add(item);
                        Session[session] = model;
                    }
                    else
                        ModelState.AddModelError("Cuenta", "Ya existe la cuenta");
                }
            }
            catch (ValidationException)
            {
                model.Lineas.Remove(item);
                throw;
            }



            return PartialView("TiposcuentasLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TiposcuentasLinDelete(string Cuenta)
        {
            var model = Session[session] as TiposCuentasModel;
            model.Lineas.Remove(model.Lineas.Single(f => f.Cuenta == Cuenta));
            Session[session] = model;
            return PartialView("TiposcuentasLin", model);
        }

        #endregion

        #region Helper

        private void UpdateLineas(TiposCuentasModel model)
        {
            foreach (var item in model.Lineas)
            {
                item.Empresa = model.Empresa;
                item.Tipo = model.Tipos;
            }
        }

        protected override IGestionService createService(IModelView model)
        {
            return new TiposcuentasService(ContextService);
        }

        #endregion
    }
}