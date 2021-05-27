using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.StateMachine;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Newtonsoft.Json;
using Resources;
using System.Text.RegularExpressions;
using RFamilias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;

namespace Marfil.App.WebMain.Controllers
{
    public class PresupuestosController : GenericController<PresupuestosModel>
    {
        private const string session = "_presupuestoslin_";
        private const string sessiontotales = "_presupuestostotales_";
        private const string sessioncomponentes = "_presupuestoscomponentes";


        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "presupuestos";
            var permisos = appService.GetPermisosMenu("presupuestos");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public PresupuestosController(IContextService context) : base(context)
        {

        }

        #endregion

        public ActionResult Imprimir()
        {
            return View();
        }

        #region Api

        public override ActionResult Create()
        {
           if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            using (var gestionService = FService.Instance.GetService(typeof(PresupuestosModel), ContextService))
            {
                var model = TempData["model"] as PresupuestosModel ?? Helper.fModel.GetModel<PresupuestosModel>(ContextService);
                Session[session] = model.Lineas;
                Session[sessiontotales] = model.Totales;
                Session[sessioncomponentes] = model.Componentes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }      
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(PresupuestosModel model)
        {
            try
            {
                model.Context = ContextService;
                model.Lineas = Session[session] as List<PresupuestosLinModel>;
                model.Totales = Session[sessiontotales] as List<PresupuestosTotalesModel>;
                model.Componentes = Session[sessioncomponentes] as List<PresupuestosComponentesLinModel>;
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

        public override ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            var newModel = Helper.fModel.GetModel<PresupuestosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"]as PresupuestosModel : gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((PresupuestosModel)model).Lineas;
                Session[sessiontotales] = ((PresupuestosModel)model).Totales;
                Session[sessioncomponentes] = ((PresupuestosModel)model).Componentes.OrderBy(f => f.Idlineaarticulo).ToList();
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
               
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(PresupuestosModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                model.Context = ContextService;
                model.Lineas = Session[session] as List<PresupuestosLinModel>;
                model.Totales = Session[sessiontotales] as List<PresupuestosTotalesModel>;
                model.Componentes = Session[sessioncomponentes] as List<PresupuestosComponentesLinModel>;
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

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<PresupuestosModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                
                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((PresupuestosModel)model).Lineas;
                Session[sessiontotales] = ((PresupuestosModel)model).Totales;
                Session[sessioncomponentes] = ((PresupuestosModel)model).Componentes.OrderBy(f => f.Idlineaarticulo).ToList();

                ViewBag.ReadOnly = true;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                return View(model);
            }
        }

        public ActionResult Clonar(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var newModel = Helper.fModel.GetModel<PresupuestosModel>(ContextService);
            using (var gestionService = createService(newModel) as PresupuestosService)
            {
                try
                {

                    var cloned = gestionService.Clonar(id);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoClonar;
                    return RedirectToAction("Edit", new { id = cloned.Id });
                }
                catch (Exception ex)
                {
                    TempData["errors"] = ex.Message;
                    return RedirectToAction("Edit", new { id = id });
                }

            }
        }

        public ActionResult Fabricar(int id)
        {
            var asistenteModel = new PresupuestosAsistenteModel(ContextService);
            asistenteModel.Id = id;
            return View(asistenteModel);
        }

        public ActionResult CambiarEstado(string documentoReferencia, string estadoNuevo,string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(PresupuestosModel), ContextService) as  PresupuestosService)
                {
                    
                    service.EjercicioId = ContextService.Ejercicio;
                    using (var estadosService = new EstadosService(ContextService))
                    {
                        var model = service.get(documentoReferencia) as PresupuestosModel;
                        var nuevoEstado = estadosService.get(estadoNuevo) as EstadosModel;
                        var cambiarEstadoService = new MachineStateService();
                        cambiarEstadoService.SetState(service, model, nuevoEstado);
                    }
                }
            }
            catch(Exception ex)
            {
                TempData["errors"] = ex.Message;
            }
            return Redirect(returnUrl);
        }

        #endregion

        #region Action toolbar

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as PresupuestosModel;
            var result= base.EditToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            if (objModel.Estado?.Tipoestado < TipoEstado.Finalizado)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-gear",
                    Texto = General.LblGenerarPedido,
                    Url = Url.Action("Importar", "Pedidos", new { id = objModel.Id, returnUrl = Url.Action("Edit", new { id = objModel.Id }) })
                });
            } 

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-copy",
                Texto = General.LblClonar,
                Url = Url.Action("Clonar", new { id = objModel.Id })
            });

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-cubes",
                Texto = General.Presupuestar,
                Url = Url.Action("Fabricar", new { id = objModel.Id })
            });

            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));
            result.Add(new ToolbarActionModel()
            {
                Icono="fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = "javascript:eventAggregator.Publish('Enviarpresupuesto',\'\')"
            });
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboEstados(objModel));
            return result;
        }

        private ToolbarActionComboModel CreateComboEstados(PresupuestosModel objModel)
        {
            var estadosService=new EstadosService(ContextService);

            var estados= estadosService.GetStates(DocumentoEstado.PresupuestosVentas, objModel.Tipoestado(ContextService));
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-refresh",
                Texto = General.LblCambiarEstado,
                Url = "#",
                Desactivado = true,
                Items = estados.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("CambiarEstado", "Presupuestos", new { documentoReferencia = objModel.Id, estadoNuevo = f.CampoId, returnUrl = Url.Action("Edit", "Presupuestos", new { id = objModel.Id }) }),
                    Texto = f.Descripcion
                })
            };
        }

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as PresupuestosModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());

            result.Add(CreateComboImprimir(objModel));
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-envelope-o",
                OcultarTextoSiempre = true,
                Texto = General.LblEnviaremail,
                Url = "javascript:eventAggregator.Publish('Enviarpresupuesto',\'\')"
            });
            return result;
        }

        private ToolbarActionComboModel CreateComboImprimir(PresupuestosModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.PresupuestosVentas, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f=>new ToolbarActionModel()
                {
                  Url= Url.Action("Visualizar","Designer", new {primaryKey = objModel.Referencia, tipo = TipoDocumentos.PresupuestosVentas,reportId=f}),
                  Texto = f,
                  Target="_blank"
                })
            };
        }

        #endregion

        #region Grid Devexpress

        public ActionResult CustomGridViewEditingPartial(string key)
        {
            ViewData["key"] = key;
            var model = Session[session] as List<PresupuestosLinModel>;
            return PartialView("_presupuestoslin", model);
        }

        [ValidateInput(false)]
        public ActionResult PresupuestosLin()
        {
            var model = Session[session] as List<PresupuestosLinModel>;
            return PartialView("_presupuestoslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PresupuestosLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] PresupuestosLinModel item)
        {
            var model = Session[session] as List<PresupuestosLinModel>;
            try
            {
                if (ModelState.IsValid)
                {
                    var max = model.Any() ? model.Max(f => f.Id) : 0;
                    item.Id = max + 1;
                    item.Intaux = item.Id;
                    item.Integridadreferenciaflag = Guid.NewGuid();

                    var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);
                    var serviceEmpresa = FService.Instance.GetService(typeof(EmpresaModel), ContextService);
                    var empresa = serviceEmpresa.get(ContextService.Empresa) as EmpresaModel;

                    var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
                    var serviceArticulos = FService.Instance.GetService(typeof(ArticulosModel), ContextService);

                    if (serviceArticulos.exists(item.Fkarticulos))
                    {
                        var configuracionAplicacion = appService.GetConfiguracion();
                        if (configuracionAplicacion.VentasUsarCanal && configuracionAplicacion.VentasCanalObligatorio && string.IsNullOrEmpty(item.Canal))
                        {
                            ModelState.AddModelError("Canal", string.Format(General.ErrorCampoObligatorio, Presupuestos.Canal));
                        }
                        else
                        {
                            var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
                            var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
                            var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
                            var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
                            var portes = 0;

                            item.Decimalesmonedas = monedaObj.Decimales;
                            item.Importe = Math.Round(item.Importe ?? 0, monedaObj.Decimales);
                            item.Precio = Math.Round(item.Precio ?? 0, empresa.Decimalesprecios ?? 2);
                            item.Decimalesmedidas = decimalesunidades ?? 0;
                            item.Revision = item.Revision?.ToUpper();

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

                                return PartialView("_presupuestoslin", model);
                            }


                            model.Add(item);

                            Session[session] = model;
                            var service = FService.Instance.GetService(typeof(PresupuestosModel), ContextService) as PresupuestosService;
                            Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales);
                        }

                    }
                    else
                        ModelState.AddModelError("Fkarticulos", Articulos.ErrorArticuloInexistente);


                }
            }
            catch (ValidationException)
            {
                model.Remove(item);
                throw;
            }



            return PartialView("_presupuestoslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PresupuestosLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] PresupuestosLinModel item)
        {
            var model = Session[session] as List<PresupuestosLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var configuracionAplicacion = appService.GetConfiguracion();
                    if (configuracionAplicacion.VentasUsarCanal && configuracionAplicacion.VentasCanalObligatorio &&
                        string.IsNullOrEmpty(item.Canal))
                    {
                        ModelState.AddModelError("Canal",
                            string.Format(General.ErrorCampoObligatorio, Presupuestos.Canal));
                    }
                    else
                    {
                        var editItem = model.Single(f => f.Id == item.Id);
                        var integridad = editItem.Integridadreferenciaflag;
                        var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);
                        var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
                        var decimalesmonedas = Funciones.Qint(Request.Params["decimalesmonedas"]);
                        var serviceEmpresa = FService.Instance.GetService(typeof(EmpresaModel), ContextService);
                        var empresa = serviceEmpresa.get(ContextService.Empresa) as EmpresaModel;

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
                        editItem.Fkunidades = item.Fkunidades;
                        editItem.Importe = Math.Round(item.Importe ?? 0, monedaObj.Decimales);
                        editItem.Importedescuento = item.Importedescuento;
                        editItem.Lote = item.Lote;
                        editItem.Precio = Math.Round(item.Precio ?? 0, empresa.Decimalesprecios ?? 2);
                        editItem.Precioanterior = item.Precioanterior;
                        editItem.Porcentajedescuento = item.Porcentajedescuento;
                        editItem.Tabla = item.Tabla;
                        editItem.Revision = item.Revision?.ToUpper();
                        editItem.Orden = item.Orden;
                        editItem.Integridadreferenciaflag = integridad;
                        editItem.Intaux = item.Id;

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

                            return PartialView("_presupuestoslin", model);
                        }

                        Session[session] = model;

                        var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
                        var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
                        var portes = 0;

                        var service = FService.Instance.GetService(typeof(PresupuestosModel), ContextService) as PresupuestosService;
                        Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales);
                    }
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_presupuestoslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PresupuestosLinDelete( string id)
        {
            var model = Session[session] as List<PresupuestosLinModel>;
            var idint = int.Parse(id);
            var linea = model.Single(f => f.Id == idint);

            //Eliminar los componentes asociados
            var componentes = Session[sessioncomponentes] as List<PresupuestosComponentesLinModel>;
            componentes.RemoveAll(f => f.Integridadreferenciaflag == linea.Integridadreferenciaflag);

            model.Remove(linea);
            Session[sessioncomponentes] = componentes;
            Session[session] = model;
            var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);

            var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
            var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
            var descuentopp = Funciones.Qdouble(Request.Params["descuentopp"]) ?? 0;
            var descuentocomercial = Funciones.Qdouble(Request.Params["descuentocomercial"]) ?? 0;
            var portes = 0;

            var service = FService.Instance.GetService(typeof(PresupuestosModel), ContextService) as PresupuestosService;
            Session[sessiontotales] = service.Recalculartotales(model, descuentopp, descuentocomercial, portes, monedaObj.Decimales);

            return PartialView("_presupuestoslin", model);
        }

        public ActionResult PresupuestosTotales()
        {
            var model = Session[sessiontotales] as List<PresupuestosTotalesModel>;
            return PartialView(model);
        }

        // GET: api/Supercuentas/5
        public void PresupuestosRefresh()
        {
            var model = Session[session] as List<PresupuestosLinModel>;
            var decimales = model.FirstOrDefault()?.Decimalesmonedas ?? 0;
            var porcentajedescuentopp = HttpContext.Request.Params["porcentajedescuentopp"];
            var porcentajedescuentocomercial = HttpContext.Request.Params["porcentajedescuentocomercial"];
            var fkregimeniva = HttpContext.Request.Params["fkregimeniva"];
            var service = FService.Instance.GetService(typeof(PresupuestosModel), ContextService) as PresupuestosService;
            var lineas = service.RecalculaLineas(model, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, fkregimeniva, 0, decimales);
            Session[session] = lineas.ToList();
            Session[sessiontotales] = service.Recalculartotales(lineas, Funciones.Qdouble(porcentajedescuentopp) ?? 0, Funciones.Qdouble(porcentajedescuentocomercial) ?? 0, 0, decimales);

        }

        #endregion

        protected override IGestionService createService(IModelView model)
        {
            var result = FService.Instance.GetService(typeof(PresupuestosModel), ContextService) as PresupuestosService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }


        [HttpPost]
        public ActionResult Presupuestar(IEnumerable<PresupuestosComponentesLinModel> componentes, string PresupuestoId, string integridadreferencial, string idArticulo)
        {
            try
            {
                using (var gestionService = FService.Instance.GetService(typeof(PresupuestosModel), ContextService))
                {
                    var model = gestionService.get(PresupuestoId) as PresupuestosModel;
                    var articuloid = Int32.Parse(idArticulo);

                    //Eliminamos los componentes de ese articulo
                    model.Componentes.RemoveAll(f => f.Idlineaarticulo == articuloid);
                    int id = 0;

                    foreach (var componente in componentes)
                    {
                        componente.Empresa = Empresa;
                        componente.Id = id;
                        componente.Idlineaarticulo = articuloid;
                        componente.Integridadreferenciaflag = new Guid(integridadreferencial); 
                        id = id + 1;
                    }

                    model.Componentes.AddRange(componentes);                 

                    foreach(var articulo in model.Lineas)
                    {
                        if(articulo.Id == articuloid)
                        {
                            articulo.Precio = Math.Round((componentes.Sum(f => f.Precio.Value) / articulo.Metros.Value), model.Decimalesmonedas);
                        }
                    }

                    gestionService.edit(model);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;     
                }
            }

            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
            }

            return RedirectToAction("Index");
        }

        #region componentes

        [ValidateInput(false)]
        public ActionResult PresupuestosComponentesLin()
        {
            var model = Session[sessioncomponentes] as List<PresupuestosComponentesLinModel>;
            return PartialView("_componenteslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PresupuestosComponentesLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] PresupuestosComponentesLinModel item)
        {
            var model = Session[sessioncomponentes] as List<PresupuestosComponentesLinModel>;
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
                        model.Add(item);
                        Session[sessioncomponentes] = model;
                    }
                }
            }
            catch (ValidationException)
            {
                model.Remove(item);
                throw;
            }

            return PartialView("_componenteslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PresupuestosComponentesLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] PresupuestosComponentesLinModel item)
        {
            var model = Session[sessioncomponentes] as List<PresupuestosComponentesLinModel>;

            if (ModelState.IsValid)
            {
                var editItem = model.Single(f => f.Id == item.Id);
                editItem.Fkpresupuestos = item.Fkpresupuestos;
                editItem.IdComponente = item.IdComponente;
                editItem.Integridadreferenciaflag = item.Integridadreferenciaflag;
                editItem.Descripcioncomponente = item.Descripcioncomponente;
                editItem.Piezas = item.Piezas;
                editItem.Largo = item.Largo;
                editItem.Ancho = item.Ancho;
                editItem.Grueso = item.Grueso;
                editItem.Merma = item.Merma;
                editItem.Precio = item.Precio;
                Session[sessioncomponentes] = model;
            }

            return PartialView("_componenteslin", Session[sessioncomponentes] as List<PresupuestosComponentesLinModel>);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult PresupuestosComponentesLinDelete(string id)
        {
            var model = Session[sessioncomponentes] as List<PresupuestosComponentesLinModel>;
            model.Remove(model.Single(f => f.Id.ToString() == id));

            if (model.Count() >= 1)
            {
                int count = 0;
                foreach (var linea in model)
                {
                    linea.Id = count;
                    count++;
                }
            }

            Session[sessioncomponentes] = model;
            return PartialView("_componenteslin", model);
        }

        #endregion
    }
}