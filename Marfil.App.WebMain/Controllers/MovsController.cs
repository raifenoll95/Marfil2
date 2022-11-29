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
//using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.StateMachine;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;
using System.IO;
using System.Data;
using System.Web.Hosting;
using System.Threading.Tasks;
using Marfil.App.WebMain.Services;
using System.Threading;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Startup;

namespace Marfil.App.WebMain.Controllers
{
    public class MovsController : GenericController<MovsModel>
    {
        private const string session = "_movslin_";
        //private const string sessiontotales = "_movstotales_";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "movs";
            var permisos = appService.GetPermisosMenu("movs");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public MovsController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            try
            {
                //Si ya se ha comenzado el proceso de cierre no se permite crear asientos nuevos
                using (var gestionService = FService.Instance.GetService(typeof(EjerciciosModel), ContextService) as EjerciciosService)
                {
                    var estadoejercicio = gestionService.GetGetEstadoEjercicioAct(ContextService.Ejercicio);
                    if (estadoejercicio != "Abierto")
                    {
                        throw new ValidationException("Ejercicio en proceso de cierre, ya no se pueden añadir asientos");
                    }
                }

                using (var gestionService = FService.Instance.GetService(typeof(MovsModel), ContextService))
                {
                    var model = TempData["model"] as MovsModel ?? Helper.fModel.GetModel<MovsModel>(ContextService);
                    Session[session] = model.Lineas;

                    ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return RedirectToAction("Index", "Movs");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(MovsModel model)
        {
            try
            {
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<MovsModel>(ContextService);
                model.Lineas = Session[session] as List<MovsLinModel>;
                //  model.Totales = Session[sessiontotales] as List<MovsTotalesModel>;
                //        model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
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
            var newModel = Helper.fModel.GetModel<MovsModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as MovsModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((MovsModel)model).Lineas;
                //  Session[sessiontotales] = ((MovsModel)model).Totales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(MovsModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<MovsModel>(ContextService);
                model.Lineas = Session[session] as List<MovsLinModel>;
                //  model.Totales = Session[sessiontotales] as List<MovsTotalesModel>;
                //      model.Criteriosagrupacionlist = newmodel.Criteriosagrupacionlist;
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

            var newModel = Helper.fModel.GetModel<MovsModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[session] = ((MovsModel)model).Lineas;
                //      Session[sessiontotales] = ((MovsModel)model).Totales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }


        public override ActionResult DeleteConfirmed(string id)
        {
            try
            {
                /*var newModel = Helper.fModel.GetModel<MovsModel>(ContextService);
                using (var gestionService = createService(newModel))
                {

                    var model = gestionService.get(id) as MovsModel;
                    if (model.Tipoasiento == "R1" || model.Tipoasiento == "R2" || model.Tipoasiento == "R3" || model.Tipoasiento == "R4" || model.Tipoasiento == "R5")
                    {
                        throw new ValidationException("No se puedo borrar manualmente un asiento de tipo automático que no sea de apertura");
                    }
                }*/
                return base.DeleteConfirmed(id);
            }
            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return RedirectToAction("Index", "Movs");
            }

        }



        #endregion

        #region Grid Devexpress

        public ActionResult CustomGridViewEditingPartial(string key)
        {
            ViewData["key"] = key;
            var model = Session[session] as List<MovsLinModel>;
            return PartialView("_Movslin", model);
        }

        [ValidateInput(false)]
        public ActionResult MovsLin()
        {
            var model = Session[session] as List<MovsLinModel>;
            return PartialView("_Movslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MovsLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] MovsLinModel item)
        {
            var model = Session[session] as List<MovsLinModel> ?? new List<MovsLinModel>();
            try
            {
                if (ModelState.IsValid)
                {
                    var max = model.Any() ? model.Max(f => f.Id) : 0;
                    item.Id = max + 1;
                    var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);

                    var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
                    var serviceCuentas = FService.Instance.GetService(typeof(CuentasModel), ContextService);

                    if (serviceCuentas.exists(item.Fkcuentas))
                    {
                        //var configuracionAplicacion = appService.GetConfiguracion();
                        //if (configuracionAplicacion.ComprasUsarCanal && configuracionAplicacion.ComprasCanalObligatorio && string.IsNullOrEmpty(item.Canal))
                        //{
                        //    ModelState.AddModelError("Canal", string.Format(General.ErrorCampoObligatorio, Movs.Canal));
                        //}
                        //else
                        //{
                        var monedaObj = serviceMonedas.get(moneda) as MonedasModel;

                        item.Esdebe = (short)(item.Haber.HasValue && item.Haber != 0 ? -1 : 1);
                        if (item.Esdebe == -1)
                        {
                            item.Importe = Decimal.Round(item.Haber ?? 0, monedaObj.Decimales);
                        }
                        else
                        {
                            item.Importe = Decimal.Round(item.Debe ?? 0, monedaObj.Decimales);
                        }


                        model.Add(item);

                        Session[session] = model;
                        // var service = FService.Instance.GetService(typeof(MovsModel), ContextService) as MovsService;
                        // Session[sessiontotales] = service.Recalculartotales(model, monedaObj.Decimales);



                        // }

                    }
                    else
                        ModelState.AddModelError("Fkcuentas", Cuentas.ErrorCuentaExistente);
                }
            }
            catch (ValidationException)
            {
                model.Remove(item);
                throw;
            }



            return PartialView("_Movslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MovsLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] MovsLinModel item)
        {
            var model = Session[session] as List<MovsLinModel>;

            try
            {
                if (ModelState.IsValid)
                {
                    var configuracionAplicacion = appService.GetConfiguracion();

                    var editItem = model.Single(f => f.Id == item.Id);
                    var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);
                    var decimalesmonedas = Funciones.Qint(Request.Params["decimalesmonedas"]);

                    var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
                    var monedaObj = serviceMonedas.get(moneda) as MonedasModel;
                    editItem.Decimalesmonedas = decimalesmonedas ?? 0;
                    editItem.Id = item.Id;
                    editItem.Orden = item.Orden;
                    editItem.Fkseccionesanaliticas = item.Fkseccionesanaliticas;
                    editItem.Fkcuentas = item.Fkcuentas;
                    editItem.Codigocomentario = item.Codigocomentario;
                    editItem.Comentario = item.Comentario;


                    editItem.Esdebe = (short)(item.Haber.HasValue && item.Haber != 0 ? -1 : 1);

                    if (editItem.Debe != item.Debe)
                    {
                        editItem.Debe = item.Debe;
                    }

                    if (editItem.Haber != item.Haber)
                    {
                        editItem.Haber = item.Haber;
                    }

                    if (item.Esdebe == -1)
                    {
                        editItem.Importe = Decimal.Round(item.Haber ?? 0, item.Decimalesmonedas ?? 0);
                    }
                    else
                    {
                        editItem.Importe = Decimal.Round(item.Debe ?? 0, item.Decimalesmonedas ?? 0);
                    }

                    //editItem.Esdebe = editItem.Esdebe;
                    //editItem.Importe = Decimal.Round(item.Importe , monedaObj.Decimales);
                    editItem.Decimalesmonedas = item.Decimalesmonedas;

                    editItem.Importemonedadocumento = item.Importemonedadocumento;
                    editItem.Conciliado = item.Conciliado;
                    // var service = FService.Instance.GetService(typeof(MovsModel), ContextService) as MovsService;
                    // Session[sessiontotales] = service.Recalculartotales(model,  monedaObj.Decimales);

                    Session[session] = model;
                }
            }
            catch (ValidationException)
            {
                throw;
            }

            return PartialView("_Movslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult MovsLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<MovsLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;
            var moneda = Funciones.Qnull(Request.Params["fkmonedas"]);

            var serviceMonedas = FService.Instance.GetService(typeof(MonedasModel), ContextService);
            var monedaObj = serviceMonedas.get(moneda) as MonedasModel;

            var service = FService.Instance.GetService(typeof(MovsModel), ContextService) as MovsService;
            //Session[sessiontotales] = service.Recalculartotales(model,  monedaObj.Decimales);

            return PartialView("_Movslin", model);
        }



        // GET: api/Supercuentas/5
        public void MovsRefresh()
        {
            var model = Session[session] as List<MovsLinModel>;
            var decimales = model.FirstOrDefault()?.Decimalesmonedas ?? 0;
            var service = FService.Instance.GetService(typeof(MovsModel), ContextService) as MovsService;

            var lineas = service.RecalculaLineas(model, decimales);
            Session[session] = lineas.ToList();
            // Session[sessiontotales] = service.Recalculartotales(lineas,  decimales);



        }

        #endregion

        #region Action toolbar

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as MovsModel;
            var result = base.EditToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            result.Add(new ToolbarSeparatorModel());
            //          result.Add(CreateComboImprimir(objModel));

            //        result.Add(CreateComboEstados(objModel));
            return result;
        }

        //private ToolbarActionComboModel CreateComboEstados(MovsModel objModel)
        //{
        //    var estadosService = new EstadosService(ContextService);

        //    var estados = estadosService.GetStates(DocumentoEstado.Movs, objModel.Tipoestado(ContextService));
        //    return new ToolbarActionComboModel()
        //    {
        //        Icono = "fa fa-refresh",
        //        Texto = General.LblCambiarEstado,
        //        Url = "#",
        //        Desactivado = true,
        //        Items = estados.Select(f => new ToolbarActionModel()
        //        {
        //            Url = Url.Action("CambiarEstado", "Movs", new { documentoReferencia = objModel.Id, estadoNuevo = f.CampoId, returnUrl = Url.Action("Edit", "Movs", new { id = objModel.Id }) }),
        //            Texto = f.Comentario
        //        })
        //    };
        //}

        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as MovsModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());

            //         result.Add(CreateComboImprimir(objModel));

            return result;
        }

        //private ToolbarActionComboModel CreateComboImprimir(MovsModel objModel)
        //{
        //    objModel.DocumentosImpresion = objModel.GetListFormatos();
        //    return new ToolbarActionComboModel()
        //    {
        //        Icono = "fa fa-print",
        //        Texto = General.LblImprimir,
        //        Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.Movs, reportId = objModel.DocumentosImpresion.Defecto }),
        //        Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
        //        {
        //            Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.Movs, reportId = f }),
        //            Texto = f
        //        }),
        //        Target = "_blank"
        //    };
        //}

        #endregion

        #region AsistenteImportación

        public ActionResult AsistenteMovs()
        {
            return View(new AsistenteMovsModel()
            {

            });
        }

        [HttpPost]
        public ActionResult AsistenteMovs(AsistenteMovsModel model)
        {
            var idPeticion = 0;
            var file = model.Fichero;
            char delimitador = model.Delimitador.ToCharArray()[0];

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName.ToLower().EndsWith(".csv"))
                    {
                        var service = FService.Instance.GetService(typeof(MovsModel), ContextService) as MovsService;
                        StreamReader sr = new StreamReader(file.InputStream, Encoding.UTF8);
                        StringBuilder sb = new StringBuilder();
                        DataTable dt = new DataTable();
                        DataRow dr;
                        string s;
                        int j = 0;

                        dt.Columns.Add("Referencia");
                        dt.Columns.Add("Fecha");
                        dt.Columns.Add("Esdebe");
                        dt.Columns.Add("Fkcuentas");
                        dt.Columns.Add("Importe");
                        dt.Columns.Add("Comentario");
                        dt.Columns.Add("Secc");
                        dt.Columns.Add("TipoAsiento");

                        while (!sr.EndOfStream)
                        {
                            while ((s = sr.ReadLine()) != null)
                            {
                                //Ignorar cabecera                    
                                if (j > 0 || !model.Cabecera)
                                {
                                    string[] str = s.Split(delimitador);
                                    dr = dt.NewRow();

                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        try
                                        {
                                            dr[dt.Columns[i]] = str[i].Replace("\"", string.Empty).ToString() ?? string.Empty;
                                        }
                                        catch (Exception ex)
                                        {
                                            ModelState.AddModelError("File", General.ErrorDelimitadorFormato);
                                            return View("AsistenteMovs", model);
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }
                                j++;
                            }
                        }
                        try
                        {
                            idPeticion = service.CrearPeticionImportacion(ContextService);
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsync(dt, model.Seriecontable.ToString(), idPeticion, token));
                            //service.Importar(dt, model.Seriecontable.ToString(), ContextService);
                            sr.Close();
                        }
                        catch (ValidationException ex)
                        {
                            if (string.IsNullOrEmpty(ex.Message))
                            {
                                TempData["Errors"] = null;
                            }
                            else
                            {
                                TempData["Errors"] = ex.Message;
                            }
                        }

                        //TempData["Success"] = "Importado correctamente!";
                        TempData["Success"] = "Ejecutando, proceso con id = " + idPeticion.ToString() + ", para comprobar su ejecución ir al menú de peticiones asíncronas";
                        return RedirectToAction("AsistenteMovs", "Movs");
                    }
                    else
                    {
                        ModelState.AddModelError("File", General.ErrorFormatoFichero);
                    }
                }
                else
                {
                    ModelState.AddModelError("File", General.ErrorFichero);
                }
            }

            return View("AsistenteMovs", model);
        }

        private async Task GetAsync(DataTable dt, string serie, int idPeticion, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = FService.Instance.GetService(typeof(MovsModel), ContextService) as MovsService)
                {
                    await Task.Run(() => service.Importar(dt, serie, idPeticion, ContextService));
                    return;
                }

            }
            catch (TaskCanceledException tce)
            {

            }
            catch (Exception ex)
            {
                using (var service = FService.Instance.GetService(typeof(PeticionesAsincronasModel), ContextService) as PeticionesAsincronasService)
                {
                    service.CambiarEstado(EstadoPeticion.Error, idPeticion, ex.Message);
                }
            }
        }

        #endregion

        public ActionResult Contabilizar(int IdFactura, string NombreTipo, string TipoFactura, string returnUrl)
        {
            Type tipo = Helper.GetTypeFromFullName(NombreTipo);
            var model = Helper.fModel.GetModel<MovsModel>(ContextService);
            try
            {
                using (var serviceFactura = FService.Instance.GetService(tipo, ContextService))
                using (var service = FService.Instance.GetService(typeof(Dom.Persistencia.Model.Contabilidad.Movs.MovsModel), ContextService) as MovsService)
                {
                    serviceFactura.Empresa = ContextService.Empresa;
                    var Factura = serviceFactura.get(IdFactura.ToString()) as IDocument;

                    //debe tener indicada la factura el tipo de factura
                    if (TipoFactura == null || TipoFactura == "")
                    {
                        throw new ValidationException("La factura debe tener indicada el campo Tipo Factura de la pestaña Comerciales");
                    }

                    service.EjercicioId = ContextService.Ejercicio;
                    model = service.Contabilizar(Factura);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                }
            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;
                return Redirect(returnUrl);
            }
            return RedirectToAction("Details", "Movs", new { id = model.Id });
        }

        protected override IGestionService createService(IModelView model)
        {

            var result = FService.Instance.GetService(typeof(MovsModel), ContextService) as MovsService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }
    }
}