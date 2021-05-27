using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class ProveedoresController : GenericController<ProveedoresModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "proveedores";
            var permisos = appService.GetPermisosMenu("proveedores");


            using (var serviceTiposcuentas = FService.Instance.GetService(typeof(TiposCuentasModel), ContextService))
            {
                ViewBag.ExisteTipoCuenta = serviceTiposcuentas.exists(((int)TiposCuentas.Proveedores).ToString());
            }


            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear && ViewBag.ExisteTipoCuenta;
            CanModificar = permisos.CanModificar && ViewBag.ExisteTipoCuenta;
            CanEliminar = permisos.CanEliminar && ViewBag.ExisteTipoCuenta;
            CanBloquear = permisos.CanBloquear && ViewBag.ExisteTipoCuenta;
        }

        #region CTR

        public ProveedoresController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Create 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(ProveedoresModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var modelview = Helper.fModel.GetModel<ProveedoresModel>(ContextService);
                    
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

        #endregion

        #region Edit

        public override ActionResult Edit(string id)
        {
            return base.Edit(id);
        }

        #endregion

        #region Exists

        [Authorize]
        public override ActionResult Exists(string id)
        {
            
            var modelview = Helper.fModel.GetModel<ProveedoresModel>(ContextService);
            using (var service = createService(modelview) as ProveedoresService)
            {
                var tipocuenta = Request.Params["tipocuenta"];
                if (string.IsNullOrEmpty(tipocuenta))
                {
                    var obj = service.exists(id);
                    return Content(JsonConvert.SerializeObject(new ExistItem() { Existe = obj }), "application/json");
                }
                else
                {
                    var obj = service.GetCompañia((TiposCuentas)Funciones.Qint(tipocuenta), id);
                    return Content(JsonConvert.SerializeObject(new ExistItem() { Existe = obj.Existe, Valido = obj.Valido }), "application/json");
                }


            }
        }

        #endregion
    }
}