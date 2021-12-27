using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotesnave;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Marfil.Inf.Genericos.Helper;
using System.Net;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Resources;
using Marfil.Dom.Persistencia;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace Marfil.App.WebMain.Controllers
{
    public class TransformacioneslotesnaveController : GenericController<TransformacioneslotesnaveModel>
    {
        private const string session = "_Transformacioneslotesnave_";
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "Transformacioneslotesnave";
            var permisos = appService.GetPermisosMenu("Transformacioneslotesnave");
            IsActivado = permisos.IsActivado;
            CanCrear = false;
            CanModificar = permisos.CanModificar;
            CanEliminar = false;
        }

        #region CTR

        public TransformacioneslotesnaveController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Create

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(TransformacioneslotesnaveModel), ContextService))
            {
                var model = TempData["model"] as TransformacioneslotesnaveModel ?? Helper.fModel.GetModel<TransformacioneslotesnaveModel>(ContextService);
                var serviceConfiguracion = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService;
                model.PuedeReclasificarMaterial = serviceConfiguracion.GetModel().Materialentradaigualsalida;

                Session[session] = model.Lineas;
                //Session[sessioncostes] = model.Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(TransformacioneslotesnaveModel model)
        {
            try
            {
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<TransformacioneslotesnaveModel>(ContextService);

                model.Lineas = Session[session] as List<TransformacioneslotesnaveLinModel>;
                //model.Costes = Session[sessioncostes] as List<TransformacioneslotesCostesadicionalesModel>;
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
            var newModel = Helper.fModel.GetModel<TransformacioneslotesnaveModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as TransformacioneslotesnaveModel : gestionService.get(id) as TransformacioneslotesnaveModel;
                Session["id"] = model.Id; 
                var serviceConfiguracion = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService;
                model.PuedeReclasificarMaterial = serviceConfiguracion.GetModel().Materialentradaigualsalida;

                if (model == null)
                {
                    return HttpNotFound();
                }

                Session[session] = ((TransformacioneslotesnaveModel)model).Lineas;
                //Session[sessioncostes] = ((TransformacioneslotesnaveModel)model).Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(TransformacioneslotesnaveModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            var fmodel = new FModel();
            var newmodel = fmodel.GetModel<TransformacioneslotesnaveModel>(ContextService);

            model.Lineas = Session[session] as List<TransformacioneslotesnaveLinModel>;
            //model.Costes = Session[sessioncostes] as List<TransformacioneslotesCostesadicionalesModel>;
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

            var newModel = Helper.fModel.GetModel<TransformacioneslotesnaveModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id) as TransformacioneslotesnaveModel;
                var serviceConfiguracion = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService;
                model.PuedeReclasificarMaterial = serviceConfiguracion.GetModel().Materialentradaigualsalida;

                if (model == null)
                {
                    return HttpNotFound();
                }

                Session[session] = ((TransformacioneslotesnaveModel)model).Lineas;
                //Session[sessioncostes] = ((TransformacioneslotesnaveModel)model).Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }

        #endregion

        [ValidateInput(false)]
        public ActionResult TransformacioneslotesnaveLin()
        {
            var model = Session[session] as List<TransformacioneslotesnaveLinModel>;
            return PartialView("_Transformacioneslotesnavelin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransformacioneslotesnaveLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TransformacioneslotesnaveLinModel item)
        {
            var model = Session[session] as List<TransformacioneslotesnaveLinModel>;


            if (ModelState.IsValid)
            {

                var editItem = model.Single(f => f.Id == item.Id);
                var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
                editItem.Decimalesmedidas = decimalesunidades ?? 0;
                editItem.Ancho = item.Ancho;
                editItem.Largo = item.Largo;
                editItem.Grueso = item.Grueso;
                editItem.Canal = item.Canal;
                editItem.Cantidad = item.Cantidad;
                editItem.Fkarticulos = item.Fkarticulos;
                editItem.Descripcion = item.Descripcion;
                editItem.Metros = item.Metros;
                editItem.Lote = item.Lote;
                editItem.Tabla = item.Tabla;
                editItem.Revision = item.Revision?.ToUpper();
                editItem.Orden = item.Orden;
                editItem.Terminado = item.Terminado;
                editItem.Flagidentifier = Guid.NewGuid();
                Session[session] = model;
            }


            return PartialView("_Transformacioneslotesnavelin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Terminar(int key)
        {
            var model = Session[session] as List<TransformacioneslotesnaveLinModel>;
            var transformacionId = int.Parse(Session["id"].ToString());
            var editItem = model.Single(f => f.Id == key);

            editItem.Terminado = true;

            Session[session] = model;

            using(var service = new TransformacioneslotesnaveService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)))
            {
                service.TerminarBD(key, transformacionId, editItem);
            }

            return PartialView("_Transformacioneslotesnavelin", model);
        }

        public string Imprimir(int key)
        {
            var model = Session[session] as List<TransformacioneslotesnaveLinModel>;
            var transformacionId = int.Parse(Session["id"].ToString());
            var editItem = model.Single(f => f.Id == key);

            var data = JsonConvert.SerializeObject(editItem);

            return data;
        }
    }
}