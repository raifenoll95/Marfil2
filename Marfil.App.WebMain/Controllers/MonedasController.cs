using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.App.WebMain.Controllers
{
    public class MonedasController : GenericController<MonedasModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "monedas";
            var permisos = appService.GetPermisosMenu("monedas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public MonedasController(IContextService context) : base(context)
        {

        }

        #endregion


        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            using (var service = new MonedasService(ContextService))
            {
                var model = TempData["model"] == null ? Helper.fModel.GetModel<MonedasModel>(ContextService) : TempData["model"] as MonedasModel;

                if (TempData["model"] == null)
                {
                    model.Id = service.NextId();
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(service, TipoOperacion.Alta, model);

                return View(model);
            }
        }
    }
}