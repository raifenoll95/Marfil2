using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.Stock;
using Resources;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class LotesController : BaseController
    {
        private readonly ILotesService _lotesService;

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "lotes";
            var permisos = appService.GetPermisosMenu("lotes");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public LotesController(IContextService context,ILotesService lotesService):base(context)
        {
            _lotesService = lotesService;
            CargarParametros();
        }

        #endregion

        #region API

        public ActionResult Index()
        {
            
            var model = new ListadoLotesModel()
            {
                Empresa = ContextService.Empresa
                
            };
            model.Toolbar.Acciones = HelpItem();
            return View(model);
        }

       

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Lotes(ListadoLotesModel model)
        {
            var aux = model as IToolbar;
            var helper = new UrlHelper(HttpContext.Request.RequestContext);
            aux.Toolbar = new ToolbarModel();
            aux.Toolbar.Acciones = HelpItem();
            aux.Toolbar.Titulo =General.Lotes;
            aux.Toolbar.Acciones = HelpItem();
            aux.Toolbar.CustomAction = true;
            aux.Toolbar.CustomActionName = helper.Action("Index");
            model.Context = ContextService;
            Session["_lotes_"] = model;
            return View(model);
        }

        public async Task<ActionResult> Detalle(string id)
        {
            return View(await _lotesService.GetAsync(id));
        }

        public ActionResult DataBindingToLargeDatabasePartial()
        {
            var model = Session["_lotes_"] as ListadoLotesModel;
            model.Context = ContextService;
            return PartialView("_resultados", model);
        }
        #endregion

        public ActionResult Imagenes()
        {        
            var model = new LotesModel();
            model.Toolbar.Titulo = General.SubirImagen;
            model.Toolbar.Acciones = HelpItem();
            return View(model);
        }

        [HttpPost]
        public ActionResult Imagenes(LotesModel model)
        {
            if (!string.IsNullOrEmpty(model.Lote))
            {
                try
                {
                    var lotesService = new LotesService(ContextService);
                    lotesService.SubirImagenLote(model);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("Error", e.Message);
                }
            }
            else
            {
                ModelState.AddModelError("Error", "Introduce un número de lote");
            }
            model.Toolbar.Titulo = General.SubirImagen;
            model.Toolbar.Acciones = HelpItem();
            return View("Imagenes", model);
        }
    }
}