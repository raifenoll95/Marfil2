using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Graficaslistados;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class GraficasController : BaseController
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "graficas";
            IsActivado = true;
            CanCrear = true;
            CanModificar = true;
            CanEliminar = true;
        }

        #region CTR

        public GraficasController(IContextService context) : base(context)
        {

        }

        #endregion

        public ActionResult Index(string id)
        {
            using (var service = FService.Instance.GetService(typeof(ConfiguraciongraficasModel), ContextService) as ConfiguraciongraficasService)
            {
                var model = service.CargarGrafica(id) as GraficasModel;
                model.Toolbar = new ToolbarModel()
                {
                    Titulo = model.Titulo,
                    Operacion = TipoOperacion.Index,
                    Acciones = HelpItem()
                };

                return View(model);
            }
        }

        [HttpPost]
        public ActionResult Datos(string id)
        {
            using (var service = FService.Instance.GetService(typeof(ConfiguraciongraficasModel), ContextService) as ConfiguraciongraficasService)
            {
                var model = service.CargarGrafica(id) as GraficasModel;

                return Json(JsonConvert.SerializeObject(model.Datos, Formatting.Indented));
            }
        }

        [ChildActionOnly]
        public ActionResult Panel(string id)
        {
            using (var service = FService.Instance.GetService(typeof(ConfiguraciongraficasModel), ContextService) as ConfiguraciongraficasService)
            {
                var model = service.CargarGrafica(id);
               

                return PartialView(model);
            }
        }
    }
}