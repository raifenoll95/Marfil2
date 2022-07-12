﻿using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
{
    public class RegistroIvaRepercutidoController : GenericController<RegistroIvaRepercutidoModel>
    {
        private const string session = "_Totaleslin_";
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "registroivarepercutido";
            var permisos = appService.GetPermisosMenu("registroivarepercutido");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public RegistroIvaRepercutidoController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var model = TempData["model"] == null ? Helper.fModel.GetModel<RegistroIvaRepercutidoModel>(ContextService) : TempData["model"] as RegistroIvaRepercutidoModel;

            //Fechas con el día de hoy por defecto
            model.Fecharegistro = DateTime.Today;
            model.Fechafactura = DateTime.Today;
            model.Fechaoperacion = DateTime.Today;
            model.Fechafacturaoriginal = DateTime.Today;

            using (var service = new RegistroIvaRepercutidoService(ContextService))
            {
                ((IToolbar)model).Toolbar = GenerateToolbar(service, TipoOperacion.Alta, model);
            }

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(RegistroIvaRepercutidoModel model)
        {
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
                //model.Context = ContextService;
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
            var newModel = Helper.fModel.GetModel<RegistroIvaRepercutidoModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                if (TempData["model"] != null)
                    return View(TempData["model"]);

                var model = gestionService.get(id);
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
        public override ActionResult EditOperacion(RegistroIvaRepercutidoModel model)
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

        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<RegistroIvaRepercutidoModel>(ContextService);
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

        #region Grid Devexpress

        public ActionResult CustomGridViewEditingPartial(string key, string buttonid)
        {
            var model = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;

            if (buttonid.Equals("btnSplit"))
            {
                var linea = model.Single(f => f.Id == Funciones.Qint(key));
            }
            else
            {
                ViewData["key"] = key;
                ViewData["buttonid"] = buttonid;
            }

            return PartialView("_Albaraneslin", model);
        }

        [ValidateInput(false)]
        public ActionResult TotalesLin()
        {
            var model = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;
            return PartialView("_totales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TotalesLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] RegistroIvaRepercutidoTotalesModel item)
        {
            var model = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;
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



            return PartialView("_totales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TotalesLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] RegistroIvaRepercutidoTotalesModel item)
        {
            var model = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Fktiposiva = item.Fktiposiva;
                    editItem.Porcentajeiva = item.Porcentajeiva;
                    editItem.Brutototal = item.Brutototal;
                    editItem.Baseimponible = item.Baseimponible;
                    editItem.Cuotaiva = item.Cuotaiva;
                    editItem.Porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia;
                    editItem.Importerecargoequivalencia = item.Importerecargoequivalencia;
                    editItem.Baseretencion = item.Baseretencion;
                    editItem.Porcentajeretencion = item.Porcentajeretencion;
                    editItem.Importeretencion = item.Importeretencion;
                    editItem.Subtotal = item.Subtotal;
                    Session[session] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_totales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TotalesLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            return PartialView("_totales", model);
        }

        #endregion
    }
}