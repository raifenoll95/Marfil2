using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView;

namespace Marfil.App.WebMain.Controllers
{
    public class AcabadosController : GenericController<AcabadosModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "acabados";
            var permisos = appService.GetPermisosMenu("acabados");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public AcabadosController(IContextService context) : base(context)
        {

        }

        #endregion

        public ActionResult comprobarAcabadoBruto()
        {
            string responseView = "";
            string acabado = "";
            var servicioAcabado = FService.Instance.GetService(typeof(AcabadosModel), ContextService) as AcabadosService;

            //Comprobamos si existe un acabado en Bruto en la BD
            if(servicioAcabado.comprobarAcabadosEnBruto())
            {
                responseView = "okBruto";
                acabado = servicioAcabado.nombreAcabado();
            }

            var data = new { status = "ok", responseView = responseView, acabado = acabado};
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}