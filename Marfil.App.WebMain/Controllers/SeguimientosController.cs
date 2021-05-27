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
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using System.Net.Http;
using Marfil.Dom.Persistencia.Model.Terceros;
using Newtonsoft.Json;
using Marfil.Dom.ControlsUI.Email;
using System.Web.Script.Serialization;

namespace Marfil.App.WebMain.Controllers
{
    public class SeguimientosController : GenericController<SeguimientosModel>
    {

        private const string sessioncorreos = "_correos_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "seguimientos";
            var permisos = appService.GetPermisosMenu("seguimientos");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public SeguimientosController(IContextService context) : base(context)
        {

        }

        #endregion


        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newmodel = Helper.fModel.GetModel<SeguimientosModel>(ContextService);

            using (var gestionService = FService.Instance.GetService(typeof(SeguimientosModel), ContextService))
            {

                if (TempData["model"] == null)
                {
                    //model.Id = service.NextId();   
                    Session[sessioncorreos] = newmodel.Correos;
                }
                ((IToolbar)newmodel).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, newmodel);
                return View(newmodel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(SeguimientosModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        //model.Correos = Session[sessioncorreos] as IEnumerable<SeguimientosCorreoModel>;
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
                model.Context = ContextService;
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
            var newModel = Helper.fModel.GetModel<SeguimientosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as SeguimientosModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }

                var service = gestionService as SeguimientosService;

                //Session[sessioncorreos] = ((SeguimientosModel)model).Correos;
                ((SeguimientosModel)model).Correos = service.createLineasCorreos(model as SeguimientosModel) as List<SeguimientosCorreoModel>;

                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(urlPadre(model));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(SeguimientosModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        //model.Correos = Session[sessioncorreos] as IEnumerable<SeguimientosCorreoModel>;
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

                var service = gestionService as SeguimientosService;

                //Session[sessioncorreos] = ((SeguimientosModel)model).Correos;
                ((SeguimientosModel)model).Correos = service.createLineasCorreos(model as SeguimientosModel) as List<SeguimientosCorreoModel>;

                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(urlPadre(model));
            }
        }

        public ActionResult Generar(string id, string referencia, DocumentoEstado tipodocumento, string fkempresa, string fketapa, string fkcontacto)
        {
            var model = Helper.fModel.GetModel<SeguimientosModel>(ContextService);
            model.Origen = referencia;
            model.Tipo = (int)tipodocumento;
            model.Fkempresa = fkempresa;
            model.Fketapa = fketapa;
            model.Fkcontacto = fkcontacto;

            return View("Create", model);
        }

        protected override IEnumerable<IToolbaritem> GenerarNavegadorRegistros(IGestionService service, TipoOperacion operacion, IModelView model)
        {
            var result = new List<IToolbaritem>();
            var seguimientosModel = model as SeguimientosModel;

            string Newurl = "javascript:eventAggregator.Publish('EnviarSeguimiento','" + seguimientosModel.Id + "')";

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = Newurl
            });

            return result;
        }


        public ActionResult recuperarModelo(string id)
        {
            var service = FService.Instance.GetService(typeof(ProspectosModel), ContextService) as ProspectosService;
            var modelo = service.GetProspectoCliente(id);
            var data = JsonConvert.SerializeObject(modelo, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public IModelView urlPadre(IModelView obj)
        {
            var model = obj as SeguimientosModel;

            if (model.Tipo == (int)DocumentoEstado.Oportunidades)
            {
                model.urlPadre = Url.Action("Details", "Oportunidades", new { id = model.idPadre});
            }
            else if (model.Tipo == (int)DocumentoEstado.Proyectos)
            {
                model.urlPadre = Url.Action("Details", "Proyectos", new { id = model.idPadre});
            }
            else if (model.Tipo == (int)DocumentoEstado.Campañas)
            {
                model.urlPadre = Url.Action("Details", "Campañas", new { id = model.idPadre});
            }
            else if (model.Tipo == (int)DocumentoEstado.Incidencias)
            {
                model.urlPadre = Url.Action("Details", "IncidenciasCRM", new { id = model.idPadre});
            }

            switch (model.Fkdocumentorelacionado)
            {
                case "PRE":
                    model.urlDocumentoRelacionado = Url.Action("Details", "Presupuestos", new { id = model.Idrelacionado });
                    break;
                case "PRC":
                    model.urlDocumentoRelacionado = Url.Action("Details", "PresupuestosCompras", new { id = model.Idrelacionado });
                    break;
                case "PED":
                    model.urlDocumentoRelacionado = Url.Action("Details", "Pedidos", new { id = model.Idrelacionado });
                    break;
                case "PEC":
                    model.urlDocumentoRelacionado = Url.Action("Details", "PedidosCompras", new { id = model.Idrelacionado });
                    break;
                case "ALB":
                    model.urlDocumentoRelacionado = Url.Action("Details", "EntregasStock", new { id = model.Idrelacionado });
                    break;
                case "ALC":
                    model.urlDocumentoRelacionado = Url.Action("Details", "RecepcionesStock", new { id = model.Idrelacionado });
                    break;
                case "FRA":
                    model.urlDocumentoRelacionado = Url.Action("Details", "Facturas", new { id = model.Idrelacionado });
                    break;
                case "FRC":
                    model.urlDocumentoRelacionado = Url.Action("Details", "FacturasCompras", new { id = model.Idrelacionado });
                    break;
            }

            return (obj);
        }

        //Rai - Crear los correos enviados
        public ActionResult crearCorreosEnviados(string asunto, string destinatario, int id, string fkorigen)
        {
            SeguimientosCorreoModel correo = new SeguimientosCorreoModel();
            correo.Context = ContextService; 
            correo.Empresa = Empresa;
            correo.Fkorigen = fkorigen;
            correo.Fkseguimientos = id;
            correo.Correo = destinatario;
            correo.Asunto = asunto;
            correo.Fecha = DateTime.Now;
            var seguimientosService = new SeguimientosCorreoService(ContextService);
            seguimientosService.createLineasCorreos(correo);
            var data = JsonConvert.SerializeObject(1, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Rai - recoge la accion del CRM y la fecha del ejercicio y devuelve el coste - costes variables periodo
        public JsonResult costeAccionCRM(string accion, string fechaejercicio)
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            var serviceSeguimientos = FService.Instance.GetService(typeof(SeguimientosModel), ContextService) as SeguimientosService;
            var coste = serviceSeguimientos.getCosteCRM(accion, fechaejercicio);
            var data = JsonConvert.SerializeObject(coste, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

    }
}