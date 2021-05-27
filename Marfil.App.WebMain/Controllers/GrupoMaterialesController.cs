using System;
using System.Linq;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.Documentos.GrupoMateriales;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model;
using Marfil.App.WebMain.Misc;
using System.Net;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class GrupoMaterialesController : GenericController<GrupoMaterialesModel>
    {
        #region constructor

        public GrupoMaterialesController(IContextService context) : base(context)
        {
        }

        #endregion

        #region ABSTRACT

        public override bool CanCrear { get; set; }

        public override bool CanEliminar { get; set; }

        public override bool CanModificar { get; set; }

        public override bool IsActivado { get; set; }

        public override string MenuName { get; set; }

        protected override void CargarParametros()
        {

            MenuName = "grupomateriales";
            var permisos = appService.GetPermisosMenu("grupomateriales");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #endregion

        #region crud
        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(GrupoMaterialesModel), ContextService))
            {
                var model = TempData["model"] as GrupoMaterialesModel ?? Helper.fModel.GetModel<GrupoMaterialesModel>(ContextService);
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }
        }

        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<GrupoMaterialesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(GrupoMaterialesModel model)
        {
            try
            {
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<GrupoMaterialesModel>(ContextService);

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        gestionService.create(model);
                        TempData[Constantes.VariableMensajeExito] = Resources.General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }

                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
            catch (IntegridadReferencialException ex2)
            {
                TempData["errors"] = ex2.Message;
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

        
        public override ActionResult Edit(String id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newModel = Helper.fModel.GetModel<GrupoMaterialesModel>(ContextService);

            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as GrupoMaterialesModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(GrupoMaterialesModel model)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");

                    }
                }

                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Edit", new { cod = model.Cod });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { cod = model.Cod });
            }

        }

        #endregion
    }
}