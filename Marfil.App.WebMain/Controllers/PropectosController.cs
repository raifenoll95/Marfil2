using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.ConvertirProspectosClientes;
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
    public class ProspectosController : GenericController<ProspectosModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "prospectos";
            var permisos = appService.GetPermisosMenu("prospectos");



            using (var serviceTiposcuentas = FService.Instance.GetService(typeof(TiposCuentasModel), ContextService))
            {
                ViewBag.ExisteTipoCuenta = serviceTiposcuentas.exists(((int)TiposCuentas.Prospectos).ToString());
            }


            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear && ViewBag.ExisteTipoCuenta;
            CanModificar = permisos.CanModificar && ViewBag.ExisteTipoCuenta;
            CanEliminar = permisos.CanEliminar && ViewBag.ExisteTipoCuenta;
            CanBloquear = permisos.CanBloquear && ViewBag.ExisteTipoCuenta;
        }

        #region CTR

        public ProspectosController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Create 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(ProspectosModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var modelview = Helper.fModel.GetModel<ProspectosModel>(ContextService);

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

        #region Exists

        [Authorize]
        public override ActionResult Exists(string id)
        {
            
            var modelview = Helper.fModel.GetModel<ProspectosModel>(ContextService);
            using (var service = createService(modelview) as ProspectosService)
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

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Convertirprospectoencliente(ConvertirProspectoClienteModel model)
        {
            var mierror = string.Empty;
            try
            {
                using (var prospectosService = FService.Instance.GetService(typeof(ProspectosModel), ContextService) as  ProspectosService)
                {
                    prospectosService.ConvertirProspectoEnCliente(model);
                }
                HttpContext.Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(model);
            }
            catch (Exception ex)
            {
                mierror = ex.Message;
            }
            HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json(mierror);
        }
    }
}