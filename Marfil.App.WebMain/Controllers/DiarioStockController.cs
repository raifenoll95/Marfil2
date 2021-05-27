using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class DiarioStockController : BaseController
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "diariostock";
            var permisos = appService.GetPermisosMenu("diariostock");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public DiarioStockController(IContextService context) :base(context)
        {
            CargarParametros();
        }

        #endregion

        public ActionResult Index()
        {
            var fmodel = new FModel();
            var model = fmodel.GetModel<DiarioStockModel>(ContextService);

            var aux = model as IToolbar;
            var helper = new UrlHelper(HttpContext.Request.RequestContext);
            aux.Toolbar = new ToolbarListadosModel();
            aux.Toolbar.Titulo = "Diario de stock";
            aux.Toolbar.Acciones = HelpItem();
            aux.Toolbar.CustomAction = true;
            aux.Toolbar.CustomActionName = helper.Action("Index");

            return View(model);
        }

        [HttpPost]
        public ActionResult Diario(DiarioStockModel model)
        {
            var aux = model as IToolbar;
            var helper = ContextService.GetUrlHelper();
            aux.Toolbar = new ToolbarListadosModel();
            aux.Toolbar.Titulo = "Diario de stock";
            aux.Toolbar.Acciones = HelpItem();
            aux.Toolbar.CustomAction = true;
            aux.Toolbar.CustomActionName = helper.Action("Index");
            model.Context = ContextService;
            Session["_diario_"] = model;
            return View(model);
        }

        
        public ActionResult DataBindingToLargeDatabasePartial()
        {
            var model = Session["_diario_"] as DiarioStockModel;
            model.Context = ContextService;
            return PartialView("_resultados",model);
        }
    }
}