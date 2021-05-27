using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Resources;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class SeriesContablesController : GenericController<SeriesContablesModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "seriescontables";
            var permisos = appService.GetPermisosMenu("series");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public SeriesContablesController(IContextService context) : base(context)
        {

        }

        #endregion

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bloquear(string id, string returnurl, string motivo, bool operacion)
        {
            if (CanBloquear)
            {

                using (var service = FService.Instance.GetService(typeof(SeriesModel), ContextService) as SeriesService)
                {
                    service.Bloquear(id, motivo, ContextService.Id.ToString(), operacion);
                }
            }
            else
            {
                ModelState.AddModelError("", General.LblErrorBloqueoNoPermitido);
            }
            return RedirectToAction("Edit", new { id = id });

        }


        public ActionResult obtenerSerieContable(string tipo)
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            var servicioSerieContable = FService.Instance.GetService(typeof(SeriesContablesModel), ContextService) as SeriesContablesService;
            var SerieModel = servicioSerieContable.getSerie(tipo)[0] as SeriesContablesModel;
            var data = JsonConvert.SerializeObject(SerieModel, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}