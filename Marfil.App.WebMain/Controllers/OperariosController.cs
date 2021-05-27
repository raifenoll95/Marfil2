using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class OperariosController : GenericController<OperariosModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "operarios";
            var permisos = appService.GetPermisosMenu("operarios");

           
                using (var serviceTiposcuentas = FService.Instance.GetService(typeof(TiposCuentasModel), ContextService))
                {
                    ViewBag.ExisteTipoCuenta = serviceTiposcuentas.exists(((int)TiposCuentas.Operarios).ToString());
                }
            

            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear && ViewBag.ExisteTipoCuenta;
            CanModificar = permisos.CanModificar && ViewBag.ExisteTipoCuenta;
            CanEliminar = permisos.CanEliminar && ViewBag.ExisteTipoCuenta;
            CanBloquear = permisos.CanBloquear && ViewBag.ExisteTipoCuenta;
        }

        #region CTR

        public OperariosController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Exists

        [Authorize]
        public override ActionResult Exists(string id)
        {
          
            var modelview = Helper.fModel.GetModel<OperariosModel>(ContextService);
            using (var service = createService(modelview) as OperariosService)
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