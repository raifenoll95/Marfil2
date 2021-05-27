using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DevExpress.Web.Mvc;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class TablasVariasController : GenericController<BaseTablasVariasModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "tablasvarias";
            var permisos = appService.GetPermisosMenu("tablasvarias");
            IsActivado = permisos.IsActivado;
            CanCrear = false;
            CanModificar = permisos.CanModificar;
            CanEliminar = false;
        }

        #region CTR

        public TablasVariasController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Edit(string id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CleanFiltros();
            var modelview = Helper.fModel.GetModel<BaseTablasVariasModel>(ContextService);
            using (var gestionService = createService(modelview as IModelView))
            {
                
              
                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }


                var aux = model as BaseTablasVariasModel;
                Session["_tablasvariaslin_"] = aux;
                ((IToolbar)aux).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                ((IToolbar) aux).Toolbar.Titulo = aux.Nombre;
                return View(aux);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BaseTablasVariasModel model)
        {
            
            try
            {
                if (ModelState.IsValid)
                {
                    var modelMemory = Session["_tablasvariaslin_"] as BaseTablasVariasModel;
                    model.Lineas = modelMemory.Lineas;
                    
                    using (var gestionService = createService(model))
                    {
                        CanCrear = !model.Noeditable;
                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
               
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);//añadir lenguaje settings
                return View();
            }
        }

        public override ActionResult Details(string id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CleanFiltros();
            var modelview = Helper.fModel.GetModel<BaseTablasVariasModel>(ContextService);
            using (var gestionService = createService(modelview as IModelView))
            {
                
                var tablavaria = gestionService.get(id);
                if (tablavaria == null)
                {
                    return HttpNotFound();
                }
                
                var aux = tablavaria as BaseTablasVariasModel;
                CanCrear = false;
                CanModificar = !aux.Noeditable;
                Session["_tablasvariaslinReadOnly_"] = aux;
                ((IToolbar)tablavaria).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, tablavaria);
                ((IToolbar)tablavaria).Toolbar.Titulo = aux.Nombre;
                return View(tablavaria);
            }
        }

        #region Grid Devexpress
        [ValidateInput(false)]
        public ActionResult _customListIndex(string id)
        {
            return PartialView("_customListIndex", Session[id] as ListIndexModel);
        }
        [ValidateInput(false)]
        public ActionResult TablasvariasLinView()
        {
            var model = Session["_tablasvariaslinReadOnly_"] as BaseTablasVariasModel;
            return PartialView("TablasvariasLinView", model);
        }

        [ValidateInput(false)]
        public ActionResult TablasvariasLin()
        {
            var model = Session["_tablasvariaslin_"] as BaseTablasVariasModel;
            return PartialView("TablasvariasLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TablasvariasLinAddNew([ModelBinder(typeof(CustomModelBinder))] dynamic item)
        {
            var model = Session["_tablasvariaslin_"] as BaseTablasVariasModel;
            try
            {
                if (ModelState.IsValid)
                {
                    model.Lineas.Add(item);
                    var validation = item as ICanValidate;
                    if (validation.ValidateModel(model.Lineas))
                        Session["_tablasvariaslin_"] = model;

                }
            }
            catch (ValidationException)
            {
                model.Lineas.Remove(item);
                throw;
            }



            return PartialView("TablasvariasLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TablasvariasLinUpdate([ModelBinder(typeof(CustomModelBinder))] dynamic item)
        {
            var model = Session["_tablasvariaslin_"] as BaseTablasVariasModel;
            var previous = model.Lineas.Single(f => f.Valor == (item as IModelView).get("Valor").ToString());
            try
            {
                if (ModelState.IsValid)
                {
                    model.Lineas.Remove(model.Lineas.Single(f => f.Valor == (item as IModelView).get("Valor").ToString()));
                    model.Lineas.Add(item);
                    var validation = item as ICanValidate;
                    if (validation.ValidateModel(model.Lineas))
                        Session["_tablasvariaslin_"] = model;


                }
            }
            catch (ValidationException)
            {
                model.Lineas.Remove(item);
                model.Lineas.Add(previous);

                throw;
            }

            return PartialView("TablasvariasLin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TablasvariasLinDelete(string valor)
        {
            valor = valor.Replace("\"","");
            var model = Session["_tablasvariaslin_"] as BaseTablasVariasModel;
            model.Lineas.Remove(model.Lineas.Single(f => f.Valor == valor));
            Session["_tablasvariaslin_"] = model;
            return PartialView("TablasvariasLin", model);
        }

        public ActionResult ExportTo(string exportid, string OutputFormat)
        {
            ActionResult obj;
            var model = Session[exportid] as ListIndexModel;
            var settings = Session[exportid + "Settings"] as GridViewSettings;
            if (settings.Columns["Action"] != null)
                settings.Columns.Remove(settings.Columns["Action"]);

            switch (OutputFormat.ToUpper())
            {
                case "CSV":
                    obj = GridViewExtension.ExportToCsv(settings, model.List, true);
                    break;
                case "PDF":
                    obj = GridViewExtension.ExportToPdf(settings, model.List, true);
                    break;
                case "RTF":
                    obj = GridViewExtension.ExportToRtf(settings, model.List, true);
                    break;
                case "XLS":
                    obj = GridViewExtension.ExportToXls(settings, model.List, true);
                    break;
                case "XLSX":
                    obj = GridViewExtension.ExportToXlsx(settings, model.List, true);
                    break;
                default:
                    obj = RedirectToAction("Index");
                    break;
            }

            var result = obj as FileStreamResult;
            result.FileDownloadName = (model.Entidad ?? "ExportMarfil") + "." + OutputFormat.ToLower();
            return result;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ExportDetailsTo(string OutputFormat)
        {
            ActionResult obj;
            var model = Session["_tablasvariaslin_"] as BaseTablasVariasModel; ;
            var settings = Session["TablavariaSettings"] as GridViewSettings;
            if (settings.Columns["Action"] != null)
                settings.Columns.Remove(settings.Columns["Action"]);

            switch (OutputFormat.ToUpper())
            {
                case "CSV":
                    obj = GridViewExtension.ExportToCsv(settings, model.Lineas, true);
                    break;
                case "PDF":
                    obj = GridViewExtension.ExportToPdf(settings, model.Lineas, true);
                    break;
                case "RTF":
                    obj = GridViewExtension.ExportToRtf(settings, model.Lineas, true);
                    break;
                case "XLS":
                    obj = GridViewExtension.ExportToXls(settings, model.Lineas, true);
                    break;
                case "XLSX":
                    obj = GridViewExtension.ExportToXlsx(settings, model.Lineas, true);
                    break;
                default:
                    obj = RedirectToAction("Index");
                    break;
            }

            var result = obj as FileStreamResult;
            result.FileDownloadName = (model.DisplayName ?? "ExportMarfil") + "." + OutputFormat.ToLower();
            return result;
        }


        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var list= base.EditToolbar(service, model).ToList();
            var nuevoregistro = list.OfType<IToolbarActionModel>().Single(f => f.Texto == General.BtnNuevoRegistro);
            var borrarregistro = list.OfType<IToolbarActionModel>().Single(f => f.Texto == General.LblBorrar);
            list.Remove(nuevoregistro);
            list.Remove(borrarregistro);
            return list;
        }

        #region Helper

        private void CleanFiltros()
        {
            Session["ValorFilter"] = null;
            Session["DescripcionFilter"] = null;
            Session["Descripcion2Filter"] = null;
        }
        #endregion
    }

    #endregion


}
