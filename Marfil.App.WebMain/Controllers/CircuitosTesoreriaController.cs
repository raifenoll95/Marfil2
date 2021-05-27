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
using System.Web.Script.Serialization;
using Marfil.Dom.Persistencia.ServicesView;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class CircuitosTesoreriaController : GenericController<CircuitoTesoreriaCobrosModel>
    {

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "circuitotesoreriacobros";
            var permisos = appService.GetPermisosMenu("circuitotesoreriacobros");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public CircuitosTesoreriaController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var model = TempData["model"] == null ?Helper.fModel.GetModel<CircuitoTesoreriaCobrosModel>(ContextService) : TempData["model"] as CircuitoTesoreriaCobrosModel;
            using (var service = new CircuitosTesoreriaCobrosService(ContextService))
            {
                ((IToolbar)model).Toolbar = GenerateToolbar(service, TipoOperacion.Alta, model);
            }

            return View(model);

        }

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(CircuitoTesoreriaCobrosModel model)
        {
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
            var newModel = Helper.fModel.GetModel<CircuitoTesoreriaCobrosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                if (TempData["model"] != null)
                    return View(TempData["model"]);

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(CircuitoTesoreriaCobrosModel model)
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

            var newModel = Helper.fModel.GetModel<CircuitoTesoreriaCobrosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ReadOnly = true;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                return View(model);
            }
        }

        public ActionResult obtenerDatosAsistenteMovimientosTesoreria(string circuito)
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            var servicioCircuitosTesoreria = FService.Instance.GetService(typeof(CircuitoTesoreriaCobrosModel), ContextService) as CircuitosTesoreriaCobrosService;

            string data = new JavaScriptSerializer().Serialize(new
            {
                actualizar = servicioCircuitosTesoreria.ActualizarFechaPago(circuito),
                datosdocumento = servicioCircuitosTesoreria.SolicitarDatosDocumento(circuito),
                importecargo2 = servicioCircuitosTesoreria.ImporteCargo2(circuito),
                importeabono2 = servicioCircuitosTesoreria.ImporteAbono2(circuito),
                codigomanual = servicioCircuitosTesoreria.CodigoManual(circuito),
                cobrador = servicioCircuitosTesoreria.ExisteCobrador(circuito),
                remesa = servicioCircuitosTesoreria.Remesa(circuito)
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult obtenerCuentaCargo(string circuito)
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            var servicioCircuitosTesoreria = FService.Instance.GetService(typeof(CircuitoTesoreriaCobrosModel), ContextService) as CircuitosTesoreriaCobrosService;
            var CuentaModel = servicioCircuitosTesoreria.Cuentacargo2(circuito) as CuentasModel;
            var data = JsonConvert.SerializeObject(CuentaModel, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult obtenerCuentaAbono(string circuito)
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            var servicioCircuitosTesoreria = FService.Instance.GetService(typeof(CircuitoTesoreriaCobrosModel), ContextService) as CircuitosTesoreriaCobrosService;
            var CuentaModel = servicioCircuitosTesoreria.Cuentaabono2(circuito) as CuentasModel;
            var data = JsonConvert.SerializeObject(CuentaModel, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}