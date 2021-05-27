using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Reservasstock;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;

using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.StateMachine;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;
using Articulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using RReservasstock = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Reservas;
using System.Text.RegularExpressions;
using RFamilias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;

namespace Marfil.App.WebMain.Controllers
{
    public class ReservasstockController : GenericController<ReservasstockModel>
    {
        private const string session = "_Reservasstocklin_";
        private const string sessiontotales = "_Reservasstocktotales_";


        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "Reservasstock";
            var permisos = appService.GetPermisosMenu("Reservasstock");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public ReservasstockController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        #region Create

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(ReservasstockModel), ContextService))
            {
                var model = TempData["model"] as ReservasstockModel ?? Helper.fModel.GetModel<ReservasstockModel>(ContextService);
                Session[session] = model.Lineas;
                Session[sessiontotales] = model.Totales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }






        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(ReservasstockModel model)
        {
            try
            {
                model.Context = ContextService;
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<ReservasstockModel>(ContextService);
                model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
                model.Lineas = Session[session] as List<ReservasstockLinModel>;
                model.Totales = Session[sessiontotales] as List<ReservasstockTotalesModel>;
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
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
        }

        #endregion

        #region Edit

        public override ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newModel = Helper.fModel.GetModel<ReservasstockModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as ReservasstockModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((ReservasstockModel)model).Lineas;
                Session[sessiontotales] = ((ReservasstockModel)model).Totales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(ReservasstockModel model)
        {
            model.Context = ContextService;
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            var fmodel = new FModel();
            var newmodel = fmodel.GetModel<ReservasstockModel>(ContextService);
            model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
            model.Lineas = Session[session] as List<ReservasstockLinModel>;
            model.Totales = Session[sessiontotales] as List<ReservasstockTotalesModel>;
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

                model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = model.Id });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = model.Id });
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

            var newModel = Helper.fModel.GetModel<ReservasstockModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((ReservasstockModel)model).Lineas;
                Session[sessiontotales] = ((ReservasstockModel)model).Totales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }

        #endregion

        #region Cambiar estado

        public ActionResult CambiarEstado(string documentoReferencia, string estadoNuevo, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(ReservasstockModel), ContextService) as ReservasstockService)
                {
                    
                    service.EjercicioId = ContextService.Ejercicio;
                    using (var estadosService = new EstadosService(ContextService))
                    {
                        var model = service.get(documentoReferencia) as ReservasstockModel;
                        var nuevoEstado = estadosService.get(estadoNuevo) as EstadosModel;
                        var cambiarEstadoService = new MachineStateService();
                        cambiarEstadoService.SetState(service, model, nuevoEstado);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;
            }
            return Redirect(returnUrl);
        }

        #endregion

        #endregion

        #region Grid Devexpress

        [HttpPost]
        public ActionResult AgregarLineas(ReservasstockLinVistaModel model)
        {
            var errormessage = "";
            try
            {
                var listado = Session[session] as List<ReservasstockLinModel>;
                using (var ReservasstockService = FService.Instance.GetService(typeof(ReservasstockModel), ContextService) as ReservasstockService)
                {

                    listado = ReservasstockService.CrearNuevasLineas(listado, model);
                    Session[session] = listado;
                    var service = FService.Instance.GetService(typeof(ReservasstockModel), ContextService) as ReservasstockService;
                    Session[sessiontotales] = service.Recalculartotales(listado, Funciones.Qdouble(model.Descuentoprontopago) ?? 0, Funciones.Qdouble(model.Descuentocomercial) ?? 0, Funciones.Qdouble(model.Portes) ?? 0, model.Decimalesmonedas);
                }

                return Content(JsonConvert.SerializeObject(model), "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
            }


            return Content("{\"error\":\"" + errormessage + "\"}", "application/json", Encoding.UTF8);
        }



        [ValidateInput(false)]
        public ActionResult ReservasstockLin()
        {
            var model = Session[session] as List<ReservasstockLinModel>;
            return PartialView("_Reservasstocklin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ReservasstockLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] ReservasstockLinModel item)
        {
            var model = Session[session] as List<ReservasstockLinModel>;




            return PartialView("_Reservasstocklin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ReservasstockLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] ReservasstockLinModel item)
        {
            var model = Session[session] as List<ReservasstockLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var configuracionAplicacion = appService.GetConfiguracion();
                    if (configuracionAplicacion.VentasUsarCanal && configuracionAplicacion.VentasCanalObligatorio &&
                        string.IsNullOrEmpty(item.Canal))
                    {
                        ModelState.AddModelError("Canal",
                            string.Format(General.ErrorCampoObligatorio, RReservasstock.Canal));
                    }
                    else
                    {
                        var editItem = model.Single(f => f.Id == item.Id);
                        var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);
                        var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
                        var decimalesmonedas = Funciones.Qint(Request.Params["decimalesmonedas"]);

                        var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
                        var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
                        editItem.Decimalesmedidas = decimalesunidades ?? 0;
                        editItem.Decimalesmonedas = decimalesmonedas ?? 0;
                        editItem.Ancho = item.Ancho;
                        editItem.Largo = item.Largo;
                        editItem.Grueso = item.Grueso;
                        editItem.Canal = item.Canal;
                        editItem.Cantidad = item.Cantidad;
                        editItem.Cantidadpedida = item.Cantidadpedida;
                        editItem.Fkarticulos = item.Fkarticulos;
                        editItem.Descripcion = item.Descripcion;
                        editItem.Metros = item.Metros;
                        editItem.Cuotaiva = item.Cuotaiva;
                        editItem.Cuotarecargoequivalencia = item.Cuotarecargoequivalencia;
                        editItem.Fktiposiva = item.Fktiposiva;
                        editItem.Porcentajeiva = item.Porcentajeiva;
                        editItem.Porcentajerecargoequivalencia = item.Porcentajerecargoequivalencia;
                        editItem.Importe = Math.Round(item.Importe ?? 0, (editItem.Decimalesmonedas ?? 0));
                        editItem.Importedescuento = item.Importedescuento;
                        editItem.Lote = item.Lote;
                        editItem.Precio = Math.Round(item.Precio ?? 0, decimalesmonedas ?? 0);
                        editItem.Precioanterior = item.Precioanterior;
                        editItem.Porcentajedescuento = item.Porcentajedescuento;
                        editItem.Tabla = item.Tabla;
                        editItem.Revision = item.Revision?.ToUpper();
                        editItem.Caja = item.Caja;
                        editItem.Bundle = item.Bundle?.ToUpper();
                        editItem.Orden = item.Orden;
                        editItem.Flagidentifier = Guid.NewGuid();

                        // Validar dimensiones artículo
                        try
                        {
                            var familiasProductosService = FService.Instance.GetService(typeof(FamiliasproductosModel), ContextService) as FamiliasproductosService;
                            familiasProductosService.ValidarDimensiones(item.Fkarticulos, item.Largo, item.Ancho, item.Grueso);
                        }
                        catch (ValidationException ex)
                        {
                            Regex rgx = new Regex(@"\{.*\}");

                            if (Regex.IsMatch(ex.Message, rgx.Replace(RFamilias.ErrorLargo, ".*") + ".*"))
                                ModelState.AddModelError("SLargo", Regex.Match(ex.Message, rgx.Replace(RFamilias.ErrorLargo, ".*")).Value);
                            if (Regex.IsMatch(ex.Message, ".*" + rgx.Replace(RFamilias.ErrorAncho, ".*") + ".*"))
                                ModelState.AddModelError("SAncho", Regex.Match(ex.Message, rgx.Replace(RFamilias.ErrorAncho, ".*")).Value);
                            if (Regex.IsMatch(ex.Message, ".*" + rgx.Replace(RFamilias.ErrorGrueso, ".*") + "."))
                                ModelState.AddModelError("SGrueso", Regex.Match(ex.Message, rgx.Replace(RFamilias.ErrorGrueso, ".*")).Value);

                            return PartialView("_Reservasstocklin", model);
                        }

                        Session[session] = model;

                        var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
                        var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
                        var portes = 0;

                        var service = FService.Instance.GetService(typeof(ReservasstockModel), ContextService) as ReservasstockService;
                        Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales);
                    }
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_Reservasstocklin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ReservasstockLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<ReservasstockLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);

            var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
            var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
            var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
            var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
            var portes = 0;

            var service = FService.Instance.GetService(typeof(ReservasstockModel), ContextService) as ReservasstockService;
            Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales);

            return PartialView("_Reservasstocklin", model);
        }

        public ActionResult ReservasstockTotales()
        {
            var model = Session[sessiontotales] as List<ReservasstockTotalesModel>;
            return PartialView(model);
        }

        // GET: api/Supercuentas/5
        public void ReservasstockRefresh()
        {
            var model = Session[session] as List<ReservasstockLinModel>;
            var decimales = model.FirstOrDefault()?.Decimalesmonedas ?? 0;
            var porcentajedescuentopp = HttpContext.Request.Params["porcentajedescuentopp"];
            var porcentajedescuentocomercial = HttpContext.Request.Params["porcentajedescuentocomercial"];
            var fkregimeniva = HttpContext.Request.Params["fkregimeniva"];
            var service = FService.Instance.GetService(typeof(ReservasstockModel), ContextService) as ReservasstockService;
            var lineas = service.RecalculaLineas(model, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, fkregimeniva, 0, decimales);
            Session[session] = lineas.ToList();
            Session[sessiontotales] = service.Recalculartotales(lineas, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, 0, decimales);
        }

        #endregion

        #region Action toolbar

        protected override ToolbarModel GenerateToolbar(IGestionService service, TipoOperacion operacion, dynamic model = null)
        {
            var result = base.GenerateToolbar(service, operacion, model as object);
            result.Titulo = RReservasstock.TituloEntidad;
            return result;
        }

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as ReservasstockModel;

            var result = base.EditToolbar(service, model).ToList();

            result.Add(new ToolbarSeparatorModel());

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-gear",
                Texto = General.LblEntregar,
                Url = Url.Action("GenerarEntrega", "EntregasStock", new { id = objModel.Id })
            });

            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = "javascript:eventAggregator.Publish('Enviaralbaran',\'\')"
            });
            if (objModel.Estado.Tipoestado != TipoEstado.Finalizado && objModel.Estado.Tipoestado != TipoEstado.Anulado)
            {
                result.Add(new ToolbarSeparatorModel());
                result.Add(CreateComboEstados(objModel));
            }
            return result;
        }

        private ToolbarActionComboModel CreateComboEstados(ReservasstockModel objModel)
        {

            var estadosService = new EstadosService(ContextService);

            var estados = estadosService.GetStates(DocumentoEstado.Reservasstock, objModel.Tipoestado(ContextService));
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-refresh",
                Texto = General.LblCambiarEstado,
                Url = "#",
                Desactivado = true,
                Items = estados.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("CambiarEstado", "Reservasstock", new { documentoReferencia = objModel.Id, estadoNuevo = f.CampoId, returnUrl = Url.Action("Edit", "Reservasstock", new { id = objModel.Id }) }),
                    Texto = f.Descripcion
                })
            };


        }

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as ReservasstockModel;
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

        private ToolbarActionComboModel CreateComboImprimir(ReservasstockModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentoImpresion.Reservasstock, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentoImpresion.Reservasstock, reportId = f }),
                    Texto = f,
                    Target = "_blank"

                })
            };
        }


        #endregion

        protected override IGestionService createService(IModelView model)
        {
            
            var result = FService.Instance.GetService(typeof(ReservasstockModel), ContextService) as ReservasstockService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }
    }
}