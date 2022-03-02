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
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class AlmacenesController : GenericController<AlmacenesModel>
    {
        private const string session = "_almacenes_";
        private const string SessionArticulosStockSeguridad = "_articulosstockseguridad";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "almacenes";
            var permisos = appService.GetPermisosMenu("almacenes");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public AlmacenesController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var model = TempData["model"] == null ? Helper.fModel.GetModel<AlmacenesModel>(ContextService) : TempData["model"] as AlmacenesModel;
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

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(AlmacenesModel model)
        {

            try
            {

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[session] as List<AlmacenesZonasModel>;
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
            var newModel = Helper.fModel.GetModel<AlmacenesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {



                var model = TempData["model"] != null ? TempData["model"] as AlmacenesModel : gestionService.get(id) as AlmacenesModel;
                if (model == null)
                {
                    return HttpNotFound();
                }

                Session[session] = ((AlmacenesModel)model).Lineas;
                Session[SessionArticulosStockSeguridad] = ((AlmacenesModel)model).ArticulosStockSeguridad;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(AlmacenesModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {

                        model.Lineas = Session[session] as List<AlmacenesZonasModel>;
                        model.ArticulosStockSeguridad = Session[SessionArticulosStockSeguridad] as List<ArticulosStockSeguridadModel>;
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

            var newModel = Helper.fModel.GetModel<AlmacenesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[SessionArticulosStockSeguridad] = ((AlmacenesModel)model).ArticulosStockSeguridad;
                ViewBag.ReadOnly = true;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        #endregion

        #region Grid Devexpress


        [ValidateInput(false)]
        public ActionResult AlmacenesLin()
        {
            var model = Session[session] as List<AlmacenesZonasModel>;
            return PartialView("_zonas", model);
        }

        [ValidateInput(false)]
        public ActionResult ArticulosStockSeguridad()
        {
            var model = Session[SessionArticulosStockSeguridad] as List<ArticulosStockSeguridadModel>;
            return PartialView("_Movsarticulosstockseguridadalmacen", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AlmacenesLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] AlmacenesZonasModel item)
        {
            var model = Session[session] as List<AlmacenesZonasModel>;
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
                        var max = model.Any() ? model.Max(f => f.Id) + 1 : 1;
                        item.Id = max;
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



            return PartialView("_zonas", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AlmacenesLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] AlmacenesZonasModel item)
        {
            var model = Session[session] as List<AlmacenesZonasModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Coordenadas = item.Coordenadas;
                    editItem.Descripcion = item.Descripcion;
                    editItem.Fktipoubicacion = item.Fktipoubicacion;
                    Session[session] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_zonas", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult AlmacenesLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<AlmacenesZonasModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            return PartialView("_zonas", model);
        }

        #endregion
    }
}