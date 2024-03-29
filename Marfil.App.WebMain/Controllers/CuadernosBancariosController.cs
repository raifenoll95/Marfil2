﻿using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
{
    public class CuadernosBancariosController : GenericController<CuadernosBancariosModel>
    {
        private const string session = "_cuadernosBancarios_";

        #region CTR
        public CuadernosBancariosController(IContextService context) : base(context)
        {
        }
        #endregion

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "Cuadernos Bancarios";
            var permisos = appService.GetPermisosMenu("Cuadernos Bancarios");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var model = TempData["model"] == null ? Helper.fModel.GetModel<CuadernosBancariosModel>(ContextService) : TempData["model"] as CuadernosBancariosModel;
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
        public override ActionResult CreateOperacion(CuadernosBancariosModel model)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    using (var gestionService = createService(model))
                    {
                        model.Lineas = Session[session] as List<CuadernosBancariosLinModel>;
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
            var newModel = Helper.fModel.GetModel<CuadernosBancariosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {



                var model = TempData["model"] != null ? TempData["model"] as CuadernosBancariosModel : gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((CuadernosBancariosModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                //var modelo = model as CuadernosBancariosModel;
                //modelo.Lineas = modelo.Lineas.FindAll(f => f.Registro == modelo.TipoRegistro.ToString());
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(CuadernosBancariosModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        using (var cuadernosService = new CuadernosBancariosServices(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)))
                        {

                            model.Lineas = Session[session] as List<CuadernosBancariosLinModel>;
                            gestionService.edit(model);
                            //Elimina lineas con idCab = null
                            cuadernosService.DeleteAllLin();
                            TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                            return RedirectToAction("Index");
                        }
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

            var newModel = Helper.fModel.GetModel<CuadernosBancariosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ReadOnly = true;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                var modelo = model as CuadernosBancariosModel;
                modelo.Lineas = modelo.Lineas.FindAll(f => f.Registro == modelo.TipoRegistro.ToString());
                return View(modelo);
            }
        }

        public override ActionResult DeleteConfirmed(string id)
        {
            try
            {
                var modelview = Helper.fModel.GetModel<CuadernosBancariosModel>(ContextService);
                using (var gestionService = createService(modelview as IModelView))
                {
                    using (var cuadernosService = new CuadernosBancariosServices(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)))
                    {
                        var model = gestionService.get(id);               
                        gestionService.delete(model);
                        //Elimina lineas con idCab = null
                        cuadernosService.DeleteAllLin();

                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;
                return RedirectToAction("Delete", new { id = id });
            }
        }

        #region Grid Devexpress
        [HttpPost, ValidateInput(false)]
        public void CuadernosBancariosLinSession(string registro)
        {
            Session["tipoRegistro"] = registro;
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CuadernosBancariosLin(string registro, string formato)
        {
            var model = Session[session] as List<CuadernosBancariosLinModel>;

            if (registro != null && registro != "")
            {
                var tipoRegistro = "";
                switch (registro)
                {
                    case "0":
                        tipoRegistro = "Cabecera";
                        break;
                    case "1":
                        tipoRegistro = "Detalle";
                        break;
                    case "2":
                        tipoRegistro = "Total";
                        break;
                }

                var actRegistro = Session["actRegistro"] as string;
                Session["tipoRegistro"] = tipoRegistro;

                if (Session["Lineas" + tipoRegistro] != null)
                {
                    //Guardamos las anteriores                  
                    Session["Lineas" + actRegistro] = model.FindAll(f => f.Registro == actRegistro.ToString());
                    //Mostramos las guardadas
                    model = Session["Lineas" + tipoRegistro] as List<CuadernosBancariosLinModel>;
                }
                else
                {
                    //Guardamos las anteriores
                    foreach (var item in model)
                    {
                        if (item.Registro == "") { item.Registro = actRegistro; }                      
                    }
                    Session["Lineas" + actRegistro] = model.FindAll(f => f.Registro == actRegistro.ToString());

                    //Mostrtamos vacías
                    model = new List<CuadernosBancariosLinModel>();
                }

                Session["actRegistro"] = tipoRegistro;
            } else
            {
                model = Session["Lineas" + Session["tipoRegistro"]] as List<CuadernosBancariosLinModel>;
            }

            if (formato != null && formato != "")
            {
                Session["Formato"] = formato == "0" ? "Fijo" : "Variable";
            }         
            return PartialView("_cuadernosbancarioslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CuadernosBancariosLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] CuadernosBancariosLinModel item)
        {
            var actRegistro = Session["actRegistro"];
            var tipoRegistro = Session["tipoRegistro"];

            var model = Session[session] as List<CuadernosBancariosLinModel>;


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
                        item.Registro = tipoRegistro != null ? tipoRegistro.ToString() : "";
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

            if(tipoRegistro != null && tipoRegistro.ToString() != "")
            {
                var modelFiltro = model.FindAll(f => f.Registro == tipoRegistro.ToString());
                return PartialView("_cuadernosbancarioslin", modelFiltro);
            }
            else
            {
                return PartialView("_cuadernosbancarioslin", model);
            }

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CuadernosBancariosLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] CuadernosBancariosLinModel item)
        {
            var model = Session[session] as List<CuadernosBancariosLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Orden = item.Orden;
                    editItem.Posicion = item.Posicion;
                    editItem.Longitud = item.Longitud;
                    editItem.TipoCampo = item.TipoCampo;
                    editItem.EtiquetaIni = item.EtiquetaIni;
                    editItem.Campo = item.Campo;
                    editItem.Obligatorio = item.Obligatorio;
                    editItem.EtiquetaFin = item.EtiquetaFin;
                    editItem.Condicion = item.Condicion;
                    editItem.DescripcionLin = item.DescripcionLin;
                    Session[session] = model;
                    model = model.FindAll(m => m.Registro == editItem.Registro);
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_cuadernosbancarioslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CuadernosBancariosLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<CuadernosBancariosLinModel>;          

            try
            {
                using (var gestionService = new CuadernosBancariosServices(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)))
                {
                    var registro = gestionService.DeleteLin(id);

                    if(registro == "")
                    {
                        var regDelete = model.Find(m => m.Id == intid);
                        registro = regDelete.Registro;
                    }

                    model.Remove(model.Single(f => f.Id == intid));
                    Session[session] = model;

                    return PartialView("_cuadernosbancarioslin", model.FindAll(m => m.Registro == registro));
                }
            }
            catch (ValidationException)
            {
                throw;
            }

        }

        #endregion
    }
}