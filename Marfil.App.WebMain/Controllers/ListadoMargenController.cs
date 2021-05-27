using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.App.WebMain.Misc;

namespace Marfil.App.WebMain.Controllers
{
    public class ListadoMargenController : BaseController
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "listadomargen";
            var permisos = appService.GetPermisosMenu("listadomargen");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        public ListadoMargenController(IContextService context) : base(context)
        {
        }

        public ActionResult ListadoMargen()
        {
            /*return View("ListadosBalancePedidos");*/
            return View(new ListadoMargen(ContextService) { });
        }
    }
}