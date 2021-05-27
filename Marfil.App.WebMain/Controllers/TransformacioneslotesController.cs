using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using DevExpress.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes;

using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.StateMachine;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using Resources;
using RTransformacioneslotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformacioneslotes;

namespace Marfil.App.WebMain.Controllers
{


    public class TransformacioneslotesController : GenericController<TransformacioneslotesModel>
    {
        
        private const string session = "_Transformacioneslotes_";
        private const string sessioncostes = "_Transformacioneslotescostes";

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "Transformacioneslotes";
            var permisos = appService.GetPermisosMenu("Transformacioneslotes");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region CTR

        public TransformacioneslotesController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        #region Create

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(TransformacioneslotesModel), ContextService))
            {
                var model = TempData["model"] as TransformacioneslotesModel ?? Helper.fModel.GetModel<TransformacioneslotesModel>(ContextService);
                var serviceConfiguracion = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService;
                model.PuedeReclasificarMaterial = serviceConfiguracion.GetModel().Materialentradaigualsalida;

                Session[session] = model.Lineas;
                Session[sessioncostes] = model.Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(TransformacioneslotesModel model)
        {
            try
            {
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<TransformacioneslotesModel>(ContextService);
                
                model.Lineas = Session[session] as List<TransformacioneslotesLinModel>;
                model.Costes = Session[sessioncostes] as List<TransformacioneslotesCostesadicionalesModel>;
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
            var newModel = Helper.fModel.GetModel<TransformacioneslotesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as TransformacioneslotesModel : gestionService.get(id) as TransformacioneslotesModel;
                var serviceConfiguracion = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService;
                model.PuedeReclasificarMaterial = serviceConfiguracion.GetModel().Materialentradaigualsalida;

                if (model == null)
                {
                    return HttpNotFound();
                }
                
                Session[session] = ((TransformacioneslotesModel)model).Lineas;
                Session[sessioncostes] = ((TransformacioneslotesModel)model).Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(TransformacioneslotesModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            var fmodel = new FModel();
            var newmodel = fmodel.GetModel<TransformacioneslotesModel>(ContextService);
            
            model.Lineas = Session[session] as List<TransformacioneslotesLinModel>;
            model.Costes = Session[sessioncostes] as List<TransformacioneslotesCostesadicionalesModel>;
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

            var newModel = Helper.fModel.GetModel<TransformacioneslotesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id) as TransformacioneslotesModel;
                var serviceConfiguracion = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService;
                model.PuedeReclasificarMaterial = serviceConfiguracion.GetModel().Materialentradaigualsalida;

                if (model == null)
                {
                    return HttpNotFound();
                }
                
                Session[session] = ((TransformacioneslotesModel)model).Lineas;
                Session[sessioncostes] = ((TransformacioneslotesModel)model).Costes;
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
                using (var service = FService.Instance.GetService(typeof(TransformacioneslotesModel), ContextService) as TransformacioneslotesService)
                {
                    service.EjercicioId = ContextService.Ejercicio;
                    var model = service.get(documentoReferencia) as TransformacioneslotesModel;
                    model.Costes = Session[sessioncostes] as List<TransformacioneslotesCostesadicionalesModel>;
                    service.ModificarCostes(model);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                }
            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;
            }
            return Redirect(returnUrl);
        }

        #endregion

        public ActionResult CambiarEstado(string documentoReferencia, string estadoNuevo, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(TransformacioneslotesModel), ContextService) as TransformacioneslotesService)
                {

                    service.EjercicioId = ContextService.Ejercicio;
                    using (var estadosService = new EstadosService(ContextService))
                    {
                        var model = service.get(documentoReferencia) as TransformacioneslotesModel;
                        var nuevoEstado = estadosService.get(estadoNuevo) as EstadosModel;
                        var cambiarEstadoService = new MachineStateService();
                        cambiarEstadoService.SetState(service, model, nuevoEstado);
                        if(nuevoEstado.Tipoestado==TipoEstado.Finalizado)
                            TempData[Constantes.VariableMensajeExito] = "Transformación terminada con éxito";
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

        #region Grid Devexpress

        #region  stock

        [HttpPost]
        public ActionResult AgregarLineas(TransformacioneslotesLinVistaModel model)
        {
            var errormessage = "";
            try
            {
                var listado = Session[session] as List<TransformacioneslotesLinModel>;
                using (var TransformacioneslotesService = FService.Instance.GetService(typeof(TransformacioneslotesModel), ContextService) as TransformacioneslotesService)
                {

                    listado = TransformacioneslotesService.CrearNuevasLineas(listado, model);
                    Session[session] = listado;
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
        public ActionResult TransformacioneslotesLin()
        {
            var model = Session[session] as List<TransformacioneslotesLinModel>;
            return PartialView("_Transformacionesloteslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransformacioneslotesLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TransformacioneslotesLinModel item)
        {
            var model = Session[session] as List<TransformacioneslotesLinModel>;

            
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
                        Session[session] = model;
                }
            

            return PartialView("_Transformacionesloteslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult TransformacioneslotesLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[session] as List<TransformacioneslotesLinModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[session] = model;

            return PartialView("_Transformacionesloteslin", model);
        }

        #endregion

        #region Costes adicionales

        [ValidateInput(false)]
        public ActionResult TransformacioneslotesCostesAdicionales()
        {
            var model = Session[sessioncostes] as List<TransformacioneslotesCostesadicionalesModel>;
            return PartialView("_Transformacioneslotescostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesNew([ModelBinder(typeof(DevExpressEditorsBinder))] TransformacioneslotesCostesadicionalesModel item)
        {
            var model = Session[sessioncostes] as List<TransformacioneslotesCostesadicionalesModel>;
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

                        var lineas = Session[session] as List<TransformacioneslotesLinModel>;
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

            return PartialView("_Transformacioneslotescostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] TransformacioneslotesCostesadicionalesModel item)
        {
            var model = Session[sessioncostes] as List<TransformacioneslotesCostesadicionalesModel>;

            if (ModelState.IsValid)
            {
                var editItem = model.Single(f => f.Id == item.Id);
                if (item.Tipodocumento == TipoCosteAdicional.Documento && string.IsNullOrEmpty(item.Referenciadocumento))
                {
                    ModelState.AddModelError("Referenciadocumento", "El campo es obligatorio");
                }
                {
                    if (item.Tipodocumento == TipoCosteAdicional.Importefijo)
                        item.Referenciadocumento = string.Empty;
                    item.Total = Math.Round((double)(item.Importe * (item.Porcentaje / 100.0)), 2);

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

            return PartialView("_Transformacioneslotescostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[sessioncostes] as List<TransformacioneslotesCostesadicionalesModel>;
            model.Remove(model.Single(f => f.Id == intid));
            Session[sessioncostes] = model;


            return PartialView("_Transformacioneslotescostesadicionales", model);
        }

        #endregion

        

        #endregion

        #region Action toolbar

        protected override ToolbarModel GenerateToolbar(IGestionService service, TipoOperacion operacion, dynamic model = null)
        {
            var result = base.GenerateToolbar(service, operacion, model as object);
            result.Titulo = RTransformacioneslotes.TituloEntidad;

            return result;
        }

        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as TransformacioneslotesModel;
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
            var objModel = model as TransformacioneslotesModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));

            return result;
        }

        private ToolbarActionComboModel CreateComboImprimir(TransformacioneslotesModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.TransformacionesAcabados, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.TransformacionesAcabados, reportId = f }),
                    Texto = f,
                    Target = "_blank"
                })
            };
        }

        private ToolbarActionModel CreateComboEstados(TransformacioneslotesModel objModel)
        {
            var estadosService = new EstadosService(ContextService);

            var estados = estadosService.GetStates(DocumentoEstado.Transformacioneslotes,TipoEstado.Curso);
            var estadoFinalizado = estados.First(f => f.Tipoestado ==TipoEstado.Finalizado);
            return new ToolbarActionModel()
            {
                Icono = "fa fa-gear",
                Texto = General.LblFinalizar,
                Url = Url.Action("CambiarEstado", "Transformacioneslotes", new { documentoReferencia = objModel.Id, estadoNuevo = estadoFinalizado.CampoId, returnUrl = Url.Action("Edit", "Transformacioneslotes", new { id = objModel.Id }) })
            };
        }
        #endregion

        protected override IGestionService createService(IModelView model)
        {
            
            var result = FService.Instance.GetService(typeof(TransformacioneslotesModel), ContextService) as TransformacioneslotesService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }
    }
}