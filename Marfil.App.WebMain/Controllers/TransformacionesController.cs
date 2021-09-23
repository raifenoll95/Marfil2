using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Transformaciones;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Transformaciones;
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
using RTransformaciones = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformaciones;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using System.Data.Entity.Validation;

namespace Marfil.App.WebMain.Controllers
{


    public class TransformacionesController : GenericController<TransformacionesModel>
    {
        private const string sessionentrada = "_transformacionesentrada_";
        private const string sessionsalida = "_transformacionessalida_";
        private const string sessioncostes = "_transformacionescostes";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "transformaciones";
            var permisos = appService.GetPermisosMenu("transformaciones");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public TransformacionesController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        #region Create

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(TransformacionesModel), ContextService))
            {
                var model = TempData["model"] as TransformacionesModel ?? Helper.fModel.GetModel<TransformacionesModel>(ContextService);
                Session[sessionentrada] = model.Lineasentrada;
                Session[sessionsalida] = model.Lineassalida;
                Session[sessioncostes] = model.Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(TransformacionesModel model)
        {
            try
            {
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<TransformacionesModel>(ContextService);
                model.Lineasentrada = Session[sessionentrada] as List<TransformacionesentradaLinModel>;
                model.Lineassalida = Session[sessionsalida] as List<TransformacionessalidaLinModel>;
                model.Costes = Session[sessioncostes] as List<TransformacionesCostesadicionalesModel>;
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
            catch (IntegridadReferencialException ex2)
            {
                TempData["errors"] = ex2.Message;
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
            var newModel = Helper.fModel.GetModel<TransformacionesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as TransformacionesModel : gestionService.get(id);

                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[sessionentrada] = ((TransformacionesModel)model).Lineasentrada;
                Session[sessionsalida] = ((TransformacionesModel)model).Lineassalida;
                Session[sessioncostes] = ((TransformacionesModel)model).Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(TransformacionesModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            var fmodel = new FModel();
            var newmodel = fmodel.GetModel<TransformacionesModel>(ContextService);
            model.Lineasentrada = Session[sessionentrada] as List<TransformacionesentradaLinModel>;
            model.Lineassalida = Session[sessionsalida] as List<TransformacionessalidaLinModel>;
            model.Costes = Session[sessioncostes] as List<TransformacionesCostesadicionalesModel>;
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

            var newModel = Helper.fModel.GetModel<TransformacionesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[sessionentrada] = ((TransformacionesModel)model).Lineasentrada;
                Session[sessionsalida] = ((TransformacionesModel)model).Lineassalida;
                Session[sessioncostes] = ((TransformacionesModel)model).Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }

        #endregion

        #region Cambiar costes

        public ActionResult ForzarCambiarCostes(string documentoReferencia, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(TransformacionesModel), ContextService) as ImputacionCosteservice)
                {
                    service.EjercicioId = ContextService.Ejercicio;
                    var model = service.get(documentoReferencia) as TransformacionesModel;

                    List<TransformacionesCostesadicionalesModel> costesoriginales = model.Costes;

                    model.Costes = Session[sessioncostes] as List<TransformacionesCostesadicionalesModel>;
                    




                    service.ModificarCostes(model, costesoriginales);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;
            }
            return Redirect(returnUrl);
        }

        #endregion

        #region Cambiar estado

        public async System.Threading.Tasks.Task<ActionResult> CambiarEstado(string documentoReferencia, string estadoNuevo, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(TransformacionesModel), ContextService) as ImputacionCosteservice)
                {

                    service.EjercicioId = ContextService.Ejercicio;
                    using (var estadosService = new EstadosService(ContextService))
                    {
                        var model = service.get(documentoReferencia) as TransformacionesModel;
                        var nuevoEstado = estadosService.get(estadoNuevo) as EstadosModel;
                        var cambiarEstadoService = new MachineStateService();
                        //Condición para evitar timeout
                        if (model.Lineassalida.Count >=100)
                        {
                            TempData["errors"]= "Tiene " + model.Lineassalida.Count + " registros de salida y se permiten un máximo de 100. Divida los registros en 2 o más transformaciones.";
                            //await cambiarEstadoService.SetStateAsync(service, model, nuevoEstado);
                        }
                        else
                        {
                            cambiarEstadoService.SetState(service, model, nuevoEstado);
                            TempData[Constantes.VariableMensajeExito] = "Transformación terminada con éxito";
                        }                       
                        
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

        #region Entrada stock

        public struct StContadoresLotes
        {
            public string IdLote { get; set; }
            public string Texto { get; set; }
        }

        [HttpPost]
        public ActionResult NuevaLinea(TransformacionesentradaLinVistaModel model)
        {
            var errormessage = "";
            try
            {
                var listado = Session[sessionentrada] as List<TransformacionesentradaLinModel>;
                using (var TransformacionesService = FService.Instance.GetService(typeof(TransformacionesModel), ContextService) as ImputacionCosteservice)
                {

                    var lineas = TransformacionesService.CrearNuevasLineasEntrada(listado, model);
                    var maxid = listado.Any() ? listado.Max(f => f.Id) : 0;
                    foreach (var item in lineas)
                        item.Id = ++maxid;

                    listado.AddRange(lineas);
                    Session[sessionentrada] = listado;
                    var service = FService.Instance.GetService(typeof(TransformacionesModel), ContextService) as ImputacionCosteservice;
                }

                return Content(JsonConvert.SerializeObject(model), "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
            }


            return Content("{\"error\":\"" + errormessage + "\"}", "application/json", Encoding.UTF8);
        }

        [HttpPost]
        public ActionResult GetLotesAutomaticos(string fkarticulo)
        {
            var errormessage = "";
            try
            {
                var listado = Session[sessionentrada] as List<TransformacionesentradaLinModel>;

                return Content(JsonConvert.SerializeObject(listado.Where(f => f.Nueva && !string.IsNullOrEmpty(f.Loteautomaticoid) && ArticulosService.GetCodigoFamilia(f.Fkarticulos) == ArticulosService.GetCodigoFamilia(fkarticulo) && ArticulosService.GetCodigoMaterial(f.Fkarticulos) == ArticulosService.GetCodigoMaterial(fkarticulo)).GroupBy(f => f.Loteautomaticoid).Select(f => new StContadoresLotes() { IdLote = f.Key, Texto = listado.First(j => j.Loteautomaticoid == f.Key).Lote }).ToList()), "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
                Response.StatusCode = 500;
            }

            return Content(errormessage);
        }

        [ValidateInput(false)]
        public ActionResult TransformacionesentradaLin()
        {
            var model = Session[sessionentrada] as List<TransformacionesentradaLinModel>;
            return PartialView("_transformacionesentradalin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransformacionesentradaLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TransformacionesentradaLinModel item)
        {
            var model = Session[sessionentrada] as List<TransformacionesentradaLinModel>;

            if (ModelState.IsValid)
            {
                var editItem = model.Single(f => f.Id == item.Id);
                var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
                editItem.Decimalesmedidas = decimalesunidades ?? 0;
                editItem.Ancho = item.Ancho;
                editItem.Largo = item.Largo;
                editItem.Grueso = item.Grueso;
                editItem.Canal = item.Canal;
                editItem.Cantidad = item.Cantidad;
                editItem.Fkarticulos = item.Fkarticulos;
                editItem.Descripcion = item.Descripcion;
                editItem.Metros = item.Metros;
                editItem.Lote = item.Lote;
                editItem.Tabla = item.Tabla;
                editItem.Revision = item.Revision?.ToUpper();
                editItem.Orden = item.Orden;
                Session[sessionentrada] = model;
            }

            return PartialView("_transformacionesentradalin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransformacionesentradaLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[sessionentrada] as List<TransformacionesentradaLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[sessionentrada] = model;

            return PartialView("_transformacionesentradalin", model);
        }

        #endregion

        #region Salida stock

        [HttpPost]
        public ActionResult AgregarLineas(TransformacionessalidaLinVistaModel model)
        {
            var errormessage = "";
            try
            {
                var listado = Session[sessionsalida] as List<TransformacionessalidaLinModel>;
                using (var transformacionesService = FService.Instance.GetService(typeof(TransformacionesModel), ContextService) as ImputacionCosteservice)
                {

                    listado = transformacionesService.CrearNuevasLineasSalida(listado, model);
                    Session[sessionsalida] = listado;
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
        public ActionResult TransformacionessalidaLin()
        {
            var model = Session[sessionsalida] as List<TransformacionessalidaLinModel>;
            return PartialView("_transformacionessalidalin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransformacionessalidaLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TransformacionessalidaLinModel item)
        {
            var model = Session[sessionsalida] as List<TransformacionessalidaLinModel>;

            
                if (ModelState.IsValid)
                {
                    
                        var editItem = model.Single(f => f.Id == item.Id);
                        var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
                        editItem.Decimalesmedidas = decimalesunidades ?? 0;
                        editItem.Ancho = item.Ancho;
                        editItem.Largo = item.Largo;
                        editItem.Grueso = item.Grueso;
                        editItem.Canal = item.Canal;
                        editItem.Cantidad = item.Cantidad;
                        editItem.Fkarticulos = item.Fkarticulos;
                        editItem.Descripcion = item.Descripcion;
                        editItem.Metros = item.Metros;
                        editItem.Lote = item.Lote;
                        editItem.Tabla = item.Tabla;
                        editItem.Revision = item.Revision?.ToUpper();
                        editItem.Orden = item.Orden;
                        editItem.Flagidentifier = Guid.NewGuid();
                        Session[sessionsalida] = model;
                }
            

            return PartialView("_transformacionessalidalin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransformacionessalidaLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[sessionsalida] as List<TransformacionessalidaLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[sessionsalida] = model;

            return PartialView("_transformacionessalidalin", model);
        }

        #endregion

        #region Costes adicionales

        [ValidateInput(false)]
        public ActionResult TransformacionesCostesAdicionales()
        {
            var model = Session[sessioncostes] as List<TransformacionesCostesadicionalesModel>;
            return PartialView("_transformacionescostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesNew([ModelBinder(typeof(DevExpressEditorsBinder))] TransformacionesCostesadicionalesModel item)
        {
            var model = Session[sessioncostes] as List<TransformacionesCostesadicionalesModel>;
            if (ModelState.IsValid)
            {
                var max = model.Any() ? model.Max(f => f.Id) : 0;
                item.Id = max + 1;
                if (item.Tipodocumento == TipoCosteAdicional.Documento &&
                    string.IsNullOrEmpty(item.Referenciadocumento))
                {
                    ModelState.AddModelError("Referenciadocumento", "El campo es obligatorio");
                }
                else
                {
                    if (item.Tipodocumento == TipoCosteAdicional.Importefijo)
                    {
                        item.Referenciadocumento = string.Empty;

                        item.Total = Math.Round((double)(item.Importe * (item.Porcentaje / 100.0)), 2);

                        model.Add(item);
                        Session[sessioncostes] = model;
                    }
                    else
                    //        {
                    //            // xm2 xm3
                    //            item.Referenciadocumento = string.Empty;
                    //            item.Total = 0;
                    //            model.Add(item);
                    //            Session[sessioncostes] = model;
                    //        }
                    //    }
                    //}
                    {
                        item.Referenciadocumento = string.Empty;

                        // Código para calcular el coste adicional cuando tipoDocumento es xm2 o xm3
                        var tipoDocumento = item.Tipodocumento;
                        var codUnidadMedida = "-1";

                        if (tipoDocumento == TipoCosteAdicional.Costexm2)
                        {
                            codUnidadMedida = "02";
                        }
                        else if (tipoDocumento == TipoCosteAdicional.Costexm3)
                        {
                            codUnidadMedida = "03";
                        }

                        var lineas = Session[sessionentrada] as List<TransformacionesentradaLinModel>;
                        var totalMetros = 0.0d;
                        foreach (var l in lineas)
                        {
                            var unidadMedida = l.Fkunidades;
                            if (unidadMedida.Equals(codUnidadMedida))
                            {
                                totalMetros += Math.Round((double)l.Metros, 3);
                            }
                        }
                        //item.Total = Math.Round((double)item.Importe * totalMetros, 2);
                        item.Total = Math.Round((double)(item.Importe * (item.Porcentaje / 100.0)), 2);
                        model.Add(item);
                        Session[sessioncostes] = model;
                    }
                }
            }

            return PartialView("_transformacionescostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TransformacionesCostesadicionalesModel item)
        {
            var model = Session[sessioncostes] as List<TransformacionesCostesadicionalesModel>;

            if (ModelState.IsValid)
            {
                var editItem = model.Single(f => f.Id == item.Id);
                if (item.Tipodocumento == TipoCosteAdicional.Documento && string.IsNullOrEmpty(item.Referenciadocumento))
                {
                    ModelState.AddModelError("Referenciadocumento", "El campo es obligatorio");
                }
                {
                    if (item.Tipodocumento == TipoCosteAdicional.Importefijo)
                    {
                        item.Referenciadocumento = string.Empty;
                        item.Total = Math.Round((double)(item.Importe * (item.Porcentaje / 100.0)), 2);
                    }

                    if (item.Tipodocumento == TipoCosteAdicional.Costexm2 || item.Tipodocumento == TipoCosteAdicional.Costexm3)
                    {
                        item.Referenciadocumento = string.Empty;
                        item.Total = 0;
                    }


                    editItem.Tipodocumento = item.Tipodocumento;
                    editItem.Referenciadocumento = item.Referenciadocumento;
                    editItem.Importe = item.Importe;
                    editItem.Porcentaje = item.Porcentaje;
                    editItem.Total = item.Total;
                    editItem.Tipocoste = item.Tipocoste;
                    editItem.Tiporeparto = item.Tiporeparto;
                    editItem.Notas = item.Notas;

                    Session[sessioncostes] = model;


                }



            }

            return PartialView("_transformacionescostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[sessioncostes] as List<TransformacionesCostesadicionalesModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[sessioncostes] = model;


            return PartialView("_transformacionescostesadicionales", model);
        }

        #endregion

        

        #endregion

        #region Action toolbar

        protected override ToolbarModel GenerateToolbar(IGestionService service, TipoOperacion operacion, dynamic model = null)
        {
            var result = base.GenerateToolbar(service, operacion, model as object);
            result.Titulo = RTransformaciones.TituloEntidad;

            return result;
        }

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as TransformacionesModel;
            var result = base.EditToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));
            if (objModel.Estado.Tipoestado < TipoEstado.Finalizado)
            {
                result.Add(new ToolbarSeparatorModel());
                result.Add(CreateComboEstados(objModel));
            }
            return result;
        }



        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as TransformacionesModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));

            return result;
        }

        private ToolbarActionComboModel CreateComboImprimir(TransformacionesModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.Transformaciones, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.Transformaciones, reportId = f }),
                    Texto = f,
                    Target = "_blank"
                })
            };
        }

        private ToolbarActionModel CreateComboEstados(TransformacionesModel objModel)
        {
            var estadosService = new EstadosService(ContextService);

            var estados = estadosService.GetStates(DocumentoEstado.Transformaciones, TipoEstado.Curso);
            var estadoFinalizado = estados.First(f => f.Tipoestado == TipoEstado.Finalizado);
            return new ToolbarActionModel()
            {
                Icono = "fa fa-gear",
                Texto = General.LblFinalizar,
                Url = Url.Action("CambiarEstado", "Transformaciones", new { documentoReferencia = objModel.Id, estadoNuevo = estadoFinalizado.CampoId, returnUrl = Url.Action("Edit", "Transformaciones", new { id = objModel.Id }) })
            };
        }

        #endregion

        protected override IGestionService createService(IModelView model)
        {
            
            var result = FService.Instance.GetService(typeof(TransformacionesModel), ContextService) as ImputacionCosteservice;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }
    }
}