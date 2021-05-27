using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DevExpress.XtraSpreadsheet.Layout;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
namespace Marfil.App.WebMain.Controllers
{
    public class CambiosController: GenericController<MonedasModel>
    {
        private const string sessionname = "cambiomonedas";
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "cambiosmonedas";
            var permisos = appService.GetPermisosMenu("cambiosmonedas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public CambiosController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Listado ContextService.

        public override ActionResult Index()
        {
            var modelview = Helper.fModel.GetModel<MonedasModel>(ContextService);
            using (var gestionService = createService(modelview))
            {
                

                var model = gestionService.GetListIndexModel(typeof(MonedasModel), CanEliminar, CanModificar, ControllerContext.RouteData.Values["controller"].ToString());
                model.List = model.List.Where(f => ((MonedasModel) f).Activado);
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Index, model);
                
                Session[model.VarSessionName] = model;
                return View(model);
            }
        }

        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var modelview = Helper.fModel.GetModel<MonedasModel>(ContextService);
            using (var gestionService = createService(modelview))
            {
                
                var model = gestionService.get(id) as MonedasModel;
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[sessionname] = model.Log;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                return View(model);
            }
        }

        public override ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var modelview = Helper.fModel.GetModel<MonedasModel>(ContextService);
            using (var gestionService = createService(modelview))
            {
                var model = (TempData["model"] != null) ? TempData["model"] as MonedasModel : gestionService.get(id) as MonedasModel;

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[sessionname] = model.Log;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        #endregion

        #region GRID

        [ValidateInput(false)]
        public ActionResult CambiosLog()
        {
            var model = Session[sessionname] as IEnumerable<CambioMonedasLogModel>;
            return PartialView("CambiosLog", model);
        }

        #endregion
    }
}