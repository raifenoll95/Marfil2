using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using DevExpress.Web.Mvc;
using System.Net;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.ServicesView;
using Resources;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.App.WebMain.Controllers
{
    public class OportunidadesController : GenericController<OportunidadesModel>
    {        
        private const string sessionSeguimientos = "_seguimientos_";
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "oportunidades";
            var permisos = appService.GetPermisosMenu("oportunidades");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public OportunidadesController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            using (var gestionService = FService.Instance.GetService(typeof(OportunidadesModel), ContextService))
            {
                var newmodel = Helper.fModel.GetModel<OportunidadesModel>(ContextService);

                Session[sessionSeguimientos] = ((OportunidadesModel)newmodel).Seguimientos;
                ((IToolbar)newmodel).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, newmodel);
                return View(newmodel);
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
            var newModel = Helper.fModel.GetModel<OportunidadesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as OportunidadesModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[sessionSeguimientos] = ((OportunidadesModel)model).Seguimientos;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<OportunidadesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[sessionSeguimientos] = ((OportunidadesModel)model).Seguimientos;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as OportunidadesModel;
            var result = base.EditToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());

            if (!objModel.Cerrado)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-gear",
                    Texto = General.LblGenerarSeguimiento,
                    Url = Url.Action("Generar", "Seguimientos", new
                    {
                        id = objModel.Id,
                        referencia = objModel.Referencia,
                        tipodocumento = DocumentoEstado.Oportunidades,
                        fkempresa = objModel.Fkempresa,
                        fkcontacto = objModel.Fkcontacto,
                        fketapa = objModel.Fketapa
                    })
                });

                result.Add(new ToolbarSeparatorModel());
            }
            
            return result;
        }


        public ActionResult GridViewSeguimientos(string key)
        {
            ViewData["key"] = key;
            var model = Session[sessionSeguimientos] as List<SeguimientosModel>;            
            return PartialView("_seguimientoslin", model);
        }

        [ValidateInput(false)]
        public ActionResult Seguimientos()
        {
            var model = Session[sessionSeguimientos] as List<SeguimientosModel>;
            return PartialView("_seguimientoslin", model);
        }

        [ChildActionOnly]
        public ActionResult Oportunidades(List<OportunidadesModel> oportunidades)
        {
            return PartialView("Oportunidades", oportunidades);
        }
    }
}