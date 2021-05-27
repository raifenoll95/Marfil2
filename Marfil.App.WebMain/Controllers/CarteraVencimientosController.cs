using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Linq;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System.Net;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using System.Collections.Generic;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class CarteraVencimientosController : GenericController<CarteraVencimientosModel>
    {
        //private const string session = "_formaspagolin_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "carteravencimientos";
            var permisos = appService.GetPermisosMenu("carteravencimientos");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public CarteraVencimientosController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var model = TempData["model"] == null ?Helper.fModel.GetModel<CarteraVencimientosModel>(ContextService) : TempData["model"] as CarteraVencimientosModel;
            using (var service = new CarteraVencimientosService(ContextService))
            {
                ((IToolbar)model).Toolbar = GenerateToolbar(service, TipoOperacion.Alta, model);
            }

            model.Usuario = ContextService.Usuario;
            model.Fecha = DateTime.Now;
            return View(model);

        }

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(CarteraVencimientosModel model)
        {
            model.Fecha = DateTime.Now;
            try
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
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                   .SelectMany(x => x.Errors)
                   .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                //model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Create");
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
            var newModel = Helper.fModel.GetModel<CarteraVencimientosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                if (TempData["model"] != null)
                    return View(TempData["model"]);

                var model = gestionService.get(id);
                ((CarteraVencimientosModel)model).Context = newModel.Context;

                if (model == null)
                {
                    return HttpNotFound();
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);

                foreach (var vencimiento in ((CarteraVencimientosModel)model).LineasPrevisiones)
                {
                    vencimiento.urlDocumento = ((CarteraVencimientosModel)model).Tipovencimiento == TipoVencimiento.Cobros ? Url.Action("Details", "Vencimientos", new { id = vencimiento.Id }) : Url.Action("Details", "VencimientosCompra", new { id = vencimiento.Id });
                }
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(CarteraVencimientosModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = obj.get(objExt.primaryKey.First().Name) });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = obj.get(objExt.primaryKey.First().Name) });
            }
        }

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<CarteraVencimientosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                ((CarteraVencimientosModel)model).Context = newModel.Context;

                if (model == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ReadOnly = true;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);

                foreach(var vencimiento in ((CarteraVencimientosModel)model).LineasPrevisiones)
                {
                    vencimiento.urlDocumento = ((CarteraVencimientosModel)model).Tipovencimiento == TipoVencimiento.Cobros ? Url.Action("Details", "Vencimientos", new { id = vencimiento.Id }) : Url.Action("Details", "VencimientosCompra", new { id = vencimiento.Id });
                }
                return View(model);
            }
        }

        public ActionResult AsistenteAsignacionCartera(string id)
        {
            return View(new AsistenteAsignacionModel(ContextService));
        }

        #region imprimir

        protected override ToolbarModel GenerateToolbar(IGestionService service, TipoOperacion operacion, dynamic model = null)
        {
            var result = base.GenerateToolbar(service, operacion, model as object);
            result.Titulo = "Cartera";
            return result;
        }

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            CarteraVencimientosModel objModel = model as CarteraVencimientosModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));
            return result;
        }

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            CarteraVencimientosModel objModel = model as CarteraVencimientosModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));
            return result;
        }

        private ToolbarActionComboModel CreateComboImprimir(CarteraVencimientosModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.CarteraVencimientos, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.CarteraVencimientos, reportId = f }),
                    Texto = f,
                    Target = "_blank"
                })
            };
        }
        #endregion

        protected override IGestionService createService(IModelView model)
        {
            var result = FService.Instance.GetService(typeof(CarteraVencimientosModel), ContextService) as CarteraVencimientosService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }
    }
}