using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Web.Mvc;
using System.Text;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;

namespace Marfil.App.WebMain.Controllers
{
    public class ExportarController : GenericController<Exportar>
    {           
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "exportar";
            var permisos = appService.GetPermisosMenu("exportar");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        public ExportarController(IContextService context) : base(context)
        {
            
        }

        public override ActionResult Index()
        {
            Exportar model = new Exportar();
            return View("Exportar", model);
        }

        public ActionResult Exportar()
        {
            Exportar model = new Exportar();
            return View("Exportar", model);
        }

        public ActionResult ExportarCuentas()
        {
            var cuentasService = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService;
            cuentasService.Exportar();
            return View("Exportar");
        }

        public ActionResult ExportarApuntes()
        {
            var movsService = FService.Instance.GetService(typeof(MovsModel), ContextService) as MovsService;
            movsService.Exportar();
            return View("Exportar");
        }

        public ActionResult ExportarIVA()
        {
            var movsService = FService.Instance.GetService(typeof(MovsModel), ContextService) as MovsService;
            movsService.ExportarIVA();
            return View("Exportar");
        }

        public ActionResult ExportarPrevisiones()
        {
            var movsService = FService.Instance.GetService(typeof(MovsModel), ContextService) as MovsService;
            movsService.ExportarPrevisiones();
            return View("Exportar");
        }

    }
}