using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using DevExpress.Web.Mvc;
using System.Net;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.ServicesView;
using Resources;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.App.WebMain.Controllers
{
    public class CampañasController : GenericController<CampañasModel>
    {

        private const string sessionterceros = "_terceros_";
        private const string sessionSeguimientos = "_seguimientos_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "campañas";
            var permisos = appService.GetPermisosMenu("campañas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public CampañasController(IContextService context) : base(context)
        {

        }

        #endregion

        #region CRUD

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newmodel = Helper.fModel.GetModel<CampañasModel>(ContextService);

            using (var gestionService = FService.Instance.GetService(typeof(CampañasModel), ContextService))
            {
               
                if (TempData["model"] == null)
                {
                    //model.Id = service.NextId();   
                    Session[sessionSeguimientos] = newmodel.Seguimientos;
                    Session[sessionterceros] = newmodel.Campañas;
                }
                ((IToolbar)newmodel).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, newmodel);
                return View(newmodel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(CampañasModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        model.Seguimientos = Session[sessionSeguimientos] as IEnumerable<SeguimientosModel>;
                        model.Campañas = Session[sessionterceros] as IEnumerable<CampañasTerceroModel>;
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
            var newModel = Helper.fModel.GetModel<CampañasModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = TempData["model"] != null ? TempData["model"] as CampañasModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[sessionSeguimientos] = ((CampañasModel)model).Seguimientos;
                Session[sessionterceros] = ((CampañasModel)model).Campañas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(CampañasModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        model.Seguimientos = Session[sessionSeguimientos] as IEnumerable<SeguimientosModel>;
                        model.Campañas = Session[sessionterceros] as IEnumerable<CampañasTerceroModel>;
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

        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<CampañasModel>(ContextService);
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

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as CampañasModel;
            var result = base.EditToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());

            if (!objModel.Cerrado)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-gear",
                    Texto = General.LblGenerarSeguimiento,
                    Url = Url.Action("Generar", "Seguimientos", new
                    {
                        id = objModel.Id,
                        referencia = objModel.Referencia,
                        tipodocumento = DocumentoEstado.Campañas,
                        fketapa = objModel.Fketapa,
                        
                    })
                });

                result.Add(new ToolbarSeparatorModel());
            }

            return result;
        }


        public ActionResult GridViewSeguimientos(string key)
        {
            ViewData["key"] = key;
            var model = Session[sessionSeguimientos] as List<SeguimientosModel>;            
            return PartialView("_seguimientoslin", model);
        }

        [ValidateInput(false)]
        public ActionResult Seguimientos()
        {
            var model = Session[sessionSeguimientos] as List<SeguimientosModel>;
            return PartialView("_seguimientoslin", model);
        }

        #region terceros

        [ValidateInput(false)]
        public ActionResult CampañasTercero()
        {
            var model = Session[sessionterceros] as List<CampañasTerceroModel>;
            return PartialView("terceroslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CampañasTerceroAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] CampañasTerceroModel item)
        {

            var model = Session[sessionterceros] as List<CampañasTerceroModel>;

            //No se pueden repetir codigos de terceros
            if (model.Count >= 1)
            {
                foreach (var tercero in model)
                {
                    if (tercero.Codtercero == item.Codtercero)
                    {
                        throw new ValidationException("Ya existe un registro con el código de tercero: " + tercero.Codtercero);
                    }
                }
            }

            item.Id = model.Count() + 1; //0+1=1

            //Añadimos el item al model
            try
            {
                if (ModelState.IsValid)
                {
                    model.Add(item);
                    Session[sessionterceros] = model;
                }
            }
            catch (ValidationException)
            {

                throw;
            }

            return PartialView("terceroslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CampañasTerceroUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] CampañasTerceroModel item)
        {

            var model = Session[sessionterceros] as List<CampañasTerceroModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id); //Sacamos la cuenta del tercero a editar
                    editItem.Codtercero = editItem.Codtercero;
                    editItem.Descripciontercero = item.Descripciontercero;
                    editItem.Poblacion = item.Poblacion;
                    editItem.Fkprovincia = item.Fkprovincia;
                    editItem.Fkpais = item.Fkpais;
                    editItem.Email = item.Email;
                    editItem.Telefono = item.Telefono;
                    Session[sessionterceros] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("terceroslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CampañasTerceroDelete(string id)
        {
            var model = Session[sessionterceros] as List<CampañasTerceroModel>;
            model.Remove(model.Single(f => f.Id.ToString() == id));

            //Ir actualizando los ID en caso de que sufran cambios (eliminacion de lineas) EN CASO DE QUE QUEDEN LINEAS
            if (model.Count() >= 1)
            {
                int count = 1;
                foreach (var linea in model)
                {

                    linea.Id = count;
                    count++;
                }
            }

            Session[sessionterceros] = model;
            return PartialView("terceroslin", model);
        }

        #endregion
    }
}