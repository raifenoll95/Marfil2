using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Inventarios;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Resources;
using System.Net;

namespace Marfil.App.WebMain.Controllers
{
    public class InventariosController : GenericController<InventariosModel>
    {
        private const string session = "_inventarioslin_";
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "inventarios";
            var permisos = appService.GetPermisosMenu("inventarios");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public InventariosController(IContextService context) : base(context)
        {

        }

        #endregion

        #region API

        #region Create

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(InventariosModel), ContextService))
            {
                var model = TempData["model"] as InventariosModel ?? Helper.fModel.GetModel<InventariosModel>(ContextService);
                Session[session] = model.Lineas;

                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(InventariosModel model)
        {
            try
            {
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<InventariosModel>(ContextService);

                model.Lineas = Session[session] as List<InventariosLinModel>;
                if (model.Lineas.Count == 0)
                {
                    TempData["errors"] =   Resources.General.ErrorDocumentoSinLineas;  
                    return RedirectToAction("Create");
                }
                else
                {
                    if (ModelState.IsValid)
                    {

                        using (var gestionService = createService(model))
                        {

                            gestionService.create(model);
                            TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {

                        TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                           .SelectMany(x => x.Errors)
                           .Select(x => x.ErrorMessage));
                        TempData["model"] = model;
                        return RedirectToAction("Create");
                    }
                }
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
        }

        #endregion

        #region Details

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<InventariosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((InventariosModel)model).Lineas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }

        #endregion

        #region Calcular lineas inventario

        [HttpPost]
        public ActionResult CalcularLineasInventario(InventariosModel model)
        {
            using (var service = FService.Instance.GetService(typeof(InventariosModel), ContextService) as InventariosService)
            {
                Session[session] = service.CalcularListadoInventarios(model);
            }
            return new EmptyResult();
        }

        #endregion

        #endregion

        #region Grid Devexpress

        [ValidateInput(false)]
        public ActionResult InventariosLin()
        {
            var model = Session[session] as List<InventariosLinModel>;
            return PartialView("_Inventarioslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult InventariosLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<InventariosLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            

            return PartialView("_Inventarioslin", model);
        }
       

        #endregion

        #region Action toolbar

        protected override ToolbarModel GenerateToolbar(IGestionService service, TipoOperacion operacion, dynamic model = null)
        {
            var result = base.GenerateToolbar(service, operacion, model as object);

            result.Titulo = Inventarios.TituloEntidad;

            return result;
        }

       

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as InventariosModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());

            result.Add(CreateComboImprimir(objModel));
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = "javascript:eventAggregator.Publish('Enviaralbaran',\'\')"
            });
            return result;
        }

        private ToolbarActionComboModel CreateComboImprimir(InventariosModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.Inventarios, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.Inventarios, reportId = f }),
                    Texto = f,
                    Target = "_blank"

                })
            };
        }

        #endregion
    }
}