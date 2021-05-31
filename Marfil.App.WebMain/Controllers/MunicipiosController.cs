using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.App.WebMain.Misc;
using Resources;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.ControlsUI.Toolbar;
using System.Net;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class MunicipiosController : GenericController<MunicipiosModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "municipios";
            var permisos = appService.GetPermisosMenu("municipios");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public MunicipiosController(IContextService context) : base(context)
        {

        }

        #endregion

        #region CRUD

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newmodel = Helper.fModel.GetModel<MunicipiosModel>(ContextService);

            using (var gestionService = FService.Instance.GetService(typeof(MunicipiosModel), ContextService))
            {
                ((IToolbar)newmodel).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, newmodel);
                return View(newmodel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(MunicipiosModel model)
        {
            var service = createService(model) as MunicipiosService;
            model.Cod = (service.obtenerUltimoId(model) + 1).ToString();

            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
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

        public override ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newModel = Helper.fModel.GetModel<MunicipiosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = TempData["model"] != null ? TempData["model"] as MunicipiosModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        //Rai. Esto es un poco una chapuza, este mantenimiento esta un poco mal implementado. Hay que hacer esto del split porque ela clave primaria del
        //Municipio esta formado por codpais-codprovincia-id. Cuando se llama al edit, esta cogiendo como id el codpais-codprovincia-id, lo cual creo que esta bien
        //porque es la clave primaria. El problema esta cuando viene aqui al editOperacion, el programa se piensa que el id es codpais-codprovincia-id
        //y la clave primaria pasa a ser codpais-codprovincia-codpais-codprovincia-id.   Un lio!!!!   (Se deberia volver a implementar esto)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(MunicipiosModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;

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

        public JsonResult GetMunicipio(string nombre)
        {
            using (var servicioMunicipios = FService.Instance.GetService(typeof(MunicipiosModel), ContextService) as MunicipiosService)
            {
                JavaScriptSerializer serializer1 = new JavaScriptSerializer();
                String N = serializer1.Deserialize<String>(nombre);
                var municipio = servicioMunicipios.GetMunicipioNombre(N).FirstOrDefault();
                var data = JsonConvert.SerializeObject(municipio, Formatting.Indented);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetMunicipios(string id)
        {
            using (var servicioMunicipios = FService.Instance.GetService(typeof(MunicipiosModel), ContextService) as MunicipiosService)
            {
                JavaScriptSerializer serializer1 = new JavaScriptSerializer();
                String N = serializer1.Deserialize<String>(id);
                var municipio = servicioMunicipios.GetMunicipiosProvincia(N);
                var data = JsonConvert.SerializeObject(municipio, Formatting.Indented);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }
}