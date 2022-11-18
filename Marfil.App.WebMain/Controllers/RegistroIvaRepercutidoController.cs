using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
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
            //model.Fechafacturaoriginal = DateTime.Today;
            //model.Fechaalta = DateTime.Now;

            //Fkseriescontables por defecto
            var serviceEjercicios = new EjerciciosService(ContextService);
            model.Fkseriescontables = serviceEjercicios.GetSerieRepercutido();

            Session[session] = model.Totales;
            //Session[sumatotales] = model.Sumatotales;
            //Se arrastran los datos de la pestaña Factura, ya no es necesario
            //Session[rectificadas] = model.Rectificadas;

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
                //Se arrastran los datos de la pestaña Factura, ya no es necesario
                //model.Rectificadas = Session[rectificadas] as List<RegistroIvaRepercutidoRectificadasModel>;

                //Auditoría
                model.Fechaaltaregistro = DateTime.Now;
                model.Fkusuarioalta = ContextService.Id;

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
                //Se arrastran los datos de la pestaña Factura, ya no es necesario
                //Session[rectificadas] = ((RegistroIvaRepercutidoModel)model).Rectificadas;
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
            //Se arrastran los datos de la pestaña Factura, ya no es necesario
            //model.Rectificadas = Session[rectificadas] as List<RegistroIvaRepercutidoRectificadasModel>;

            //Auditoría
            model.Fechamodificacionregistro = DateTime.Now;
            model.Fkusuariomodificacion = ContextService.Id;

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
                //Se arrastran los datos de la pestaña Factura, ya no es necesario
                //Session[rectificadas] = ((RegistroIvaRepercutidoModel)model).Rectificadas;
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

                        ValidarTipoFacturaGrid(item);

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
                    editItem.Siioperacion = item.Siioperacion;
                    editItem.Importearticulos = item.Importearticulos;
                    editItem.Importetai = item.Importetai;
                    editItem.Decimalesmonedas = 2;

                    if (string.IsNullOrEmpty(editItem.Cuentaventas))
                    {
                        throw new ValidationException("Debe existir una cuenta de venta");
                    }

                    ValidarTipoFacturaGrid(item);

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

        public string GetPorcentajeRetencion(string tipo)
        {
            var service = new TiposRetencionesService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var porcentajeretencion = service.GetPorcentaje(tipo);
            var inmueble = service.RequiereInmueble(tipo);

            var data = new
            {
                porcentajeretencion = porcentajeretencion,
                inmueble = inmueble
            };
            return JsonConvert.SerializeObject(data);
        }

        public string GetIvaTercero(string cuenta, string regimen)
        {
            var service = new TiposRetencionesService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));          
            var tipoiva = service.GetIvaTercero(cuenta);
            var tipoivaporcentaje = service.GetPorcentajeIvaTercero(tipoiva);
            var tipoivaporcentajerecargo = service.GetPorcentajeRecargoTercero(tipoiva);

            Session["tipoivatercero"] = tipoiva;
            Session["porcentajetipoivatercero"] = tipoivaporcentaje;
            Session["porcentajerecargotercero"] = tipoivaporcentajerecargo;

            var cuentaService = new CuentasService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var cuentaModel = cuentaService.get(cuenta) as CuentasModel;//Queremos la cuenta para cargar automáticamente campos relacionados

            var regimenService = new RegimenivaService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var configuracionService = new ConfiguracionService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var regimentercero = "";
            var clavetipofactura = "";
            var regimenespecial = "";
            var tipooperacion = "0";//Factura
            var cuentaDescripcion = "";
            var prefijos = configuracionService.GetPrefijosPrestacionServicios().Split(',');
            var clienteVarios = configuracionService.GetClientesVarios();
            var tiporetencionescliente = service.GetTipoRetencionCliente(cuenta);
            var essujetanoexenta = false;
            var essujetaexenta = false;
            var esnosujeta = false;
            var invsujetopasivo = "0";
            var identificacion = 0;//Nif
            var nif = "";
            var tiponif = "";
            var pais = "ES";

            //Si es cliente Varios, datos de la cuenta vacíos
            if (cuenta == clienteVarios)
            {
                clavetipofactura = "F2";//FACTURA SIMPLIFICADA
            }
            else
            {
                if (string.IsNullOrEmpty(regimen))
                {
                    regimentercero = service.GetRegimenivaTercero(cuenta);
                    clavetipofactura = regimenService.GetClaveTipoFacturaEmitida(regimentercero);
                    regimenespecial = regimenService.GetRegimenEspecialEmitida(regimentercero);
                    if (prefijos.Contains(cuenta.Substring(0, 3)))
                    {
                        tipooperacion = "2";//Prestación de servicios
                    }
                    else
                    {
                        if (GetOperacionUE(regimentercero))
                        {
                            tipooperacion = "1";//Entrega de bienes
                        }
                        else if (GetExportacion(regimentercero))
                        {
                            tipooperacion = "1";//Entrega de bienes
                        }
                    }

                    /*if (EsSujetaNoExenta(regimentercero))
                    {
                        essujetanoexenta = true;
                        if (EsInvSujetoPasivo(regimentercero))
                        {
                            invsujetopasivo = "1";
                        }
                    }
                    if (EsSujetaExenta(regimentercero))
                    {
                        essujetaexenta = true;
                    }
                    if (EsNoSujeta(regimentercero))
                    {
                        esnosujeta = true;
                    }*/

                }
                else
                {
                    clavetipofactura = regimenService.GetClaveTipoFacturaEmitida(regimen);
                    regimenespecial = regimenService.GetRegimenEspecialEmitida(regimen);
                    if (prefijos.Contains(cuenta.Substring(0, 3)))
                    {
                        tipooperacion = "2";//Prestación de servicios
                    }
                    else
                    {
                        if (GetOperacionUE(regimen))
                        {
                            tipooperacion = "1";//Entrega de bienes
                        }
                        else if (GetExportacion(regimen))
                        {
                            tipooperacion = "1";//Entrega de bienes
                        }
                    }

                    /*if (EsSujetaNoExenta(regimen))
                    {
                        essujetanoexenta = true;
                        if (EsInvSujetoPasivo(regimen))
                        {
                            invsujetopasivo = "1";
                        }
                    }
                    if (EsSujetaExenta(regimen))
                    {
                        essujetaexenta = true;
                    }
                    if (EsNoSujeta(regimen))
                    {
                        esnosujeta = true;
                    }*/

                }


                if (string.IsNullOrEmpty(cuentaModel.Nif.Nif))
                {
                    identificacion = 1;//Otro
                }
                else
                {
                    nif = cuentaModel.Nif.Nif.Length > 9 ? cuentaModel.Nif.Nif.Substring(2) : cuentaModel.Nif.Nif;
                }


                if (!string.IsNullOrEmpty(cuentaModel.Nif.TipoNif))
                {
                    tiponif = cuentaModel.Nif.TipoNif;
                }


                if (!string.IsNullOrEmpty(cuentaModel.FkPais) && cuentaModel.FkPais != "070")
                {
                    var listpaises = WebHelper.GetApplicationHelper().GetListPaises().ToList();

                    pais = listpaises.Where(f => f.Valor == cuentaModel.FkPais).SingleOrDefault().CodigoIsoAlfa2;
                }
            }

            var data = new
            {
                regimentercero = regimentercero,
                cuentaDescripcion = cuentaDescripcion,
                identificacion = identificacion,
                tiporetencionescliente = tiporetencionescliente,
                nif = nif,
                tiponif = tiponif,
                pais = pais,
                clavetipofactura = clavetipofactura,
                regimenespecial = regimenespecial,
                tipooperacion = tipooperacion,
                essujetanoexenta = essujetanoexenta,
                essujetaexenta = essujetaexenta,
                esnosujeta = esnosujeta,
                invsujetopasivo = invsujetopasivo
            };
            return JsonConvert.SerializeObject(data);
        }

        public string GetRegimenIva(string tipo)
        {
            var service = new TiposFacturasIvaService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var regimen = service.GetRegimenivaRepercutido(ContextService.Empresa, tipo);
            var esbieninversion = GetBienInversion(tipo);

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


            var data = new
            {
                regimen = regimen,
                esbieninversion = esbieninversion
            };
            return JsonConvert.SerializeObject(data);

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

        public bool GetExportacion(string regimen)
        {
            if (string.IsNullOrEmpty(regimen))
            {
                return false;
            }

            var service = new RegimenivaService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var esExportacion = service.GetExportacion(ContextService.Empresa, regimen);

            return esExportacion;
        }

        public bool GetBienInversion(string tipofactura)
        {
            if (string.IsNullOrEmpty(tipofactura))
            {
                return false;
            }

            var service = new RegimenivaService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var esBieninversion = service.GetBienInversion(ContextService.Empresa, tipofactura);

            return esBieninversion;
        }

        public bool EsSujetaNoExenta(string regimen)
        {
            if (string.IsNullOrEmpty(regimen))
            {
                return false;
            }

            var service = new RegimenivaService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var EsSujetaNoExenta = service.EsSujetaNoExenta(ContextService.Empresa, regimen);

            return EsSujetaNoExenta;
        }

        public bool EsSujetaExenta(string regimen)
        {
            if (string.IsNullOrEmpty(regimen))
            {
                return false;
            }

            var service = new RegimenivaService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var EsSujetaExenta = service.EsSujetaExenta(ContextService.Empresa, regimen);

            return EsSujetaExenta;
        }

        public bool EsNoSujeta(string regimen)
        {
            if (string.IsNullOrEmpty(regimen))
            {
                return false;
            }

            var service = new RegimenivaService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var EsNoSujeta = service.EsNoSujeta(ContextService.Empresa, regimen);

            return EsNoSujeta;
        }

        public bool EsInvSujetoPasivo(string regimen)
        {
            if (string.IsNullOrEmpty(regimen))
            {
                return false;
            }

            var service = new RegimenivaService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var invsujetopasivo = service.EsInvSujetoPasivo(ContextService.Empresa, regimen);

            return invsujetopasivo;
        }

        public string GetDatosFacturaRectificativa(string idfactura)
        {
            var service = new FacturasService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var facturamodel = service.get(idfactura) as FacturasModel;

            return JsonConvert.SerializeObject(facturamodel);

        }

        private void ValidarTipoFacturaGrid(RegistroIvaRepercutidoTotalesModel item)
        {
            var tipofactura = item.Idtipofactura;
            var serviceCuentas = new CuentasService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list = serviceCuentas.GetCuentasContablesNivel(0);
            var abono = serviceCuentas.GetCuentaAbono1(1, tipofactura);
            list = list.Where(f => f.Id.StartsWith(abono));

            if (!list.Any(f => f.Id == item.Cuentaventas))
            {
                throw new ValidationException("La cuenta de venta " + item.Cuentaventas + " no es válida para el tipo de factura indicado");
            }
        }
        #endregion
    }
}