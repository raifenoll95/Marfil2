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

namespace Marfil.App.WebMain.Controllers
{
    public class SeriesController : GenericController<SeriesModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "series";
            var permisos = appService.GetPermisosMenu("series");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public SeriesController(IContextService context) : base(context)
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
                
                using (var service = FService.Instance.GetService(typeof(SeriesModel),ContextService) as SeriesService)
                {
                    service.Bloquear(id, motivo, ContextService.Id.ToString(), operacion);
                }
            }
            else
            {
                ModelState.AddModelError("", General.LblErrorBloqueoNoPermitido);
            }
            return RedirectToAction("Edit",new {id= id});


        }
    }
}