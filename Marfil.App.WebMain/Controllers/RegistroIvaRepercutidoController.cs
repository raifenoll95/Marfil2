using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
{
    public class RegistroIvaRepercutidoController : GenericController<RegistroIvaRepercutidoModel>
    {
        private const string session = "_Totaleslin_";
        private const string rectificadas = "_Rectificadaslin_";
        //private const string sumatotales = "_Sumatotales_";
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "registroivarepercutido";
            var permisos = appService.GetPermisosMenu("registroivarepercutido");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public RegistroIvaRepercutidoController(IContextService context) : base(context)
        {

        }

        #endregion

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var model = TempData["model"] == null ? Helper.fModel.GetModel<RegistroIvaRepercutidoModel>(ContextService) : TempData["model"] as RegistroIvaRepercutidoModel;

            //Fechas con el día de hoy por defecto
            model.Fecharegistro = DateTime.Today;
            model.Fechafactura = DateTime.Today;
            model.Fechaoperacion = DateTime.Today;
            model.Fechafacturaoriginal = DateTime.Today;
            model.Fechaalta = DateTime.Now;

            //Fkseriescontables por defecto
            var serviceEjercicios = new EjerciciosService(ContextService);
            model.Fkseriescontables = serviceEjercicios.GetSerieRepercutido();

            Session[session] = model.Totales;
            //Session[sumatotales] = model.Sumatotales;
            Session[rectificadas] = model.Rectificadas;

            using (var service = new RegistroIvaRepercutidoService(ContextService))
            {
                ((IToolbar)model).Toolbar = GenerateToolbar(service, TipoOperacion.Alta, model);
            }

            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(RegistroIvaRepercutidoModel model)
        {
            try
            {
                model.Context = ContextService;
                model.Totales = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;
                //model.Sumatotales = Session[sumatotales] as RegistroIvaRepercutidoSumaTotalesModel;
                model.Rectificadas = Session[rectificadas] as List<RegistroIvaRepercutidoRectificadasModel>;

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
            var newModel = Helper.fModel.GetModel<RegistroIvaRepercutidoModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                if (TempData["model"] != null)
                    return View(TempData["model"]);

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((RegistroIvaRepercutidoModel)model).Totales;
                /*if (((RegistroIvaRepercutidoModel)model).Sumatotales == null)
                {
                    ((RegistroIvaRepercutidoModel)model).Sumatotales = new RegistroIvaRepercutidoSumaTotalesModel();
                }
                Session[sumatotales] = ((RegistroIvaRepercutidoModel)model).Sumatotales;*/
                Session[rectificadas] = ((RegistroIvaRepercutidoModel)model).Rectificadas;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(RegistroIvaRepercutidoModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            model.Totales = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;
            //model.Sumatotales = Session[sumatotales] as RegistroIvaRepercutidoSumaTotalesModel;
            model.Rectificadas = Session[rectificadas] as List<RegistroIvaRepercutidoRectificadasModel>;
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

        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<RegistroIvaRepercutidoModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((RegistroIvaRepercutidoModel)model).Totales;
                //Session[sumatotales] = ((RegistroIvaRepercutidoModel)model).Sumatotales;
                Session[rectificadas] = ((RegistroIvaRepercutidoModel)model).Rectificadas;
                ViewBag.ReadOnly = true;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                return View(model);
            }
        }

        #region Grid Devexpress

        public ActionResult CustomGridViewEditingPartial(string key, string buttonid)
        {
            var model = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;

            if (buttonid.Equals("btnSplit"))
            {
                var linea = model.Single(f => f.Id == Funciones.Qint(key));
            }
            else
            {
                ViewData["key"] = key;
                ViewData["buttonid"] = buttonid;
            }

            return PartialView("_totales", model);
        }

        [ValidateInput(false)]
        public ActionResult TotalesLin()
        {
            var model = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;
            return PartialView("_totales", model);
        }

        [ValidateInput(false)]
        public ActionResult RectificadasLin()
        {
            var model = Session[rectificadas] as List<RegistroIvaRepercutidoRectificadasModel>;
            return PartialView("_rectificadas", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TotalesLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] RegistroIvaRepercutidoTotalesModel item)
        {
            var model = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Any(f => f.Id == item.Id))
                    {
                        ModelState.AddModelError("Id", string.Format(General.ErrorRegistroExistente));
                    }
                    else
                    {
                        var max = model.Any() ? model.Max(f => f.Id) + 1 : 1;
                        item.Id = max;
                        item.Decimalesmonedas = 2;

                        if (string.IsNullOrEmpty(item.Cuentaventas))
                        {
                            throw new ValidationException("Debe existir una cuenta de venta");
                        }

                        model.Add(item);

                        Session[session] = model;

                        /*Total de las bases imponibles
                        var service = FService.Instance.GetService(typeof(RegistroIvaRepercutidoModel), ContextService) as RegistroIvaRepercutidoService;
                        Session[sumatotales] = service.Recalculartotales(model);*/

                    }

                }
            }
            catch (ValidationException)
            {
                model.Remove(item);
                throw;
            }



            return PartialView("_totales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RectificadasLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] RegistroIvaRepercutidoRectificadasModel item)
        {
            var model = Session[rectificadas] as List<RegistroIvaRepercutidoRectificadasModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Any(f => f.Id == item.Id))
                    {
                        ModelState.AddModelError("Id", string.Format(General.ErrorRegistroExistente));
                    }
                    else
                    {
                        var max = model.Any() ? model.Max(f => f.Id) + 1 : 1;
                        item.Id = max;
                        if (item.Id > 3)
                        {
                            throw new ValidationException("Solo se permiten 3 rectificadas");
                        }
                        model.Add(item);
                        Session[rectificadas] = model;
                    }

                }
            }
            catch (ValidationException)
            {
                model.Remove(item);
                throw;
            }



            return PartialView("_rectificadas", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TotalesLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] RegistroIvaRepercutidoTotalesModel item)
        {
            var model = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Idtipofactura = item.Idtipofactura;
                    editItem.Cuentaventas = item.Cuentaventas;
                    editItem.Fktiposiva = item.Fktiposiva;
                    editItem.Porcentajeiva = item.Porcentajeiva;
                    editItem.Baseimponible = item.Baseimponible;
                    editItem.Cuotaiva = item.Cuotaiva;
                    editItem.Porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia;
                    editItem.Importerecargoequivalencia = item.Importerecargoequivalencia;
                    editItem.Subtotal = item.Subtotal;
                    editItem.Decimalesmonedas = 3;

                    if (string.IsNullOrEmpty(editItem.Cuentaventas))
                    {
                        throw new ValidationException("Debe existir una cuenta de venta");
                    }

                    Session[session] = model;

                    /*Total de las bases imponibles
                    var service = FService.Instance.GetService(typeof(RegistroIvaRepercutidoModel), ContextService) as RegistroIvaRepercutidoService;
                    Session[sumatotales] = service.Recalculartotales(model);*/
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_totales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RectificadasLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] RegistroIvaRepercutidoRectificadasModel item)
        {
            var model = Session[rectificadas] as List<RegistroIvaRepercutidoRectificadasModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var editItem = model.Single(f => f.Id == item.Id);
                    editItem.Facturaemisor = item.Facturaemisor;
                    editItem.Fechaexpedemisor = item.Fechaexpedemisor;
                    Session[rectificadas] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_rectificadas", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TotalesLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<RegistroIvaRepercutidoTotalesModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;

            /*Total de las bases imponibles
            var service = FService.Instance.GetService(typeof(RegistroIvaRepercutidoModel), ContextService) as RegistroIvaRepercutidoService;
            Session[sumatotales] = service.Recalculartotales(model);*/

            return PartialView("_totales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult RectificadosLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[rectificadas] as List<RegistroIvaRepercutidoRectificadasModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[rectificadas] = model;
            return PartialView("_rectificadas", model);
        }
        #endregion

        #region Helper

        public string ModificarPeriodo(string Fechafactura, string Fechaoperacion)
        {
            var listperiodo = WebHelper.GetApplicationHelper().GetListPeriodoRegistroIva().Select(f => new SelectListItem()
            {
                Text = f.Text,
                Value = f.Value,

            }).ToList();

            var Fechafacturaparse = DateTime.ParseExact(Fechafactura, "dd/MM/yyyy", new CultureInfo("es-ES"));
            var Fechaoperacionparse = DateTime.ParseExact(Fechaoperacion, "dd/MM/yyyy", new CultureInfo("es-ES"));

            var service = new RegistroIvaRepercutidoService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));

            var periodo = service.ModificarPeriodo(listperiodo, Fechafacturaparse, Fechaoperacionparse);

            return periodo;
        }

        public double GetPorcentajeRetencion(string tipo)
        {
            var service = new TiposRetencionesService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            return service.GetPorcentaje(tipo);
        }

        public string GetIvaTercero(string cuenta)
        {
            var service = new TiposRetencionesService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var tipoiva = service.GetIvaTercero(cuenta);
            var tipoivaporcentaje = service.GetPorcentajeIvaTercero(tipoiva);
            var tipoivaporcentajerecargo = service.GetPorcentajeRecargoTercero(tipoiva);

            Session["tipoivatercero"] = tipoiva;
            Session["porcentajetipoivatercero"] = tipoivaporcentaje;
            Session["porcentajerecargotercero"] = tipoivaporcentajerecargo;

            return tipoiva;
        }

        public string GetRegimenIva(string tipo)
        {
            var service = new TiposFacturasIvaService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var regimen = service.GetRegimenivaRepercutido(ContextService.Empresa, tipo);

            Session["idtipofactura"] = tipo;

            var serviceCuentas = new CuentasService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list = serviceCuentas.GetCuentasContablesNivel(0);
            var abono = serviceCuentas.GetCuentaAbono1(1, tipo);
            list = list.Where(f => f.Id.StartsWith(abono));

            if (list.Count() == 1)
            {
                Session["cuentaventas"] = list.FirstOrDefault().Id;
            }
            else
            {
                Session["cuentaventas"] = null;
            }

            return regimen;
        }

        public bool GetOperacionUE(string regimen)
        {           
            if (string.IsNullOrEmpty(regimen))
            {
                return false;
            }

            var service = new RegimenivaService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var esOperacionUE = service.GetOperacionUE(ContextService.Empresa, regimen);

            return esOperacionUE;
        }

        public string GetDatosFacturaRectificativa(string idfactura)
        {
            var service = new FacturasService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var facturamodel = service.get(idfactura) as FacturasModel;

            return JsonConvert.SerializeObject(facturamodel);

        }
        #endregion
    }
}