using Marfil.Dom.Persistencia.Model.Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model;
using Marfil.App.WebMain.Misc;
using Resources;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using DevExpress.Web.Mvc;
using Marfil.Inf.Genericos.Helper;
using RImputacionCostes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ImputacionCostes;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.App.WebMain.Controllers
{
    public class ImputacionCostesController : GenericController<ImputacionCostesModel>
    {

        private const string sessionlotes = "_imputacionlotes_";
        private const string sessioncostes = "_imputacioncostes";

        #region ABSTRACT

        public override bool CanCrear { get; set; }
        public override bool CanEliminar { get; set; }
        public override bool CanModificar { get; set; }
        public override bool IsActivado { get; set; }
        public override string MenuName { get; set; }

        protected override void CargarParametros()
        {

            MenuName = "imputacioncostes";
            var permisos = appService.GetPermisosMenu("imputacioncostes");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #endregion

        #region ctr
        public ImputacionCostesController(IContextService context) : base(context)
        {
        }
        #endregion


        #region crud

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<ImputacionCostesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[sessionlotes] = ((ImputacionCostesModel)model).LineasLotes;
                Session[sessioncostes] = ((ImputacionCostesModel)model).LineasCostes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            using (var gestionService = FService.Instance.GetService(typeof(ImputacionCostesModel), ContextService))
            {
                var model = TempData["model"] as ImputacionCostesModel ?? Helper.fModel.GetModel<ImputacionCostesModel>(ContextService);
                Session[sessionlotes] = model.LineasLotes;
                Session[sessioncostes] = model.LineasCostes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(ImputacionCostesModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        model.LineasLotes = Session[sessionlotes] as List<ImputacionCostesLinModel>;
                        model.LineasCostes = Session[sessioncostes] as List<ImputacionCostesCostesadicionalesModel>;
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
            var newModel = Helper.fModel.GetModel<ImputacionCostesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {
                var model = TempData["model"] != null ? TempData["model"] as ImputacionCostesModel : gestionService.get(id);
                Session[sessionlotes] = ((ImputacionCostesModel)model).LineasLotes;
                Session[sessioncostes] = ((ImputacionCostesModel)model).LineasCostes;                 
                
                //Redireccion al indice
                try
                {
                    ((ImputacionCostesService)gestionService).ValidarEstado(((ImputacionCostesModel)model));
                }
                catch(Exception ex)
                { 
                    TempData["errors"] = ex.Message;
                    TempData["model"] = model;
                    ModelState.AddModelError("", TempData["errors"].ToString());
                    return RedirectToAction("Index");
                }         

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
        public override ActionResult EditOperacion(ImputacionCostesModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            var fmodel = new FModel();
            var newmodel = fmodel.GetModel<ImputacionCostesModel>(ContextService);
            model.LineasLotes = Session[sessionlotes] as List<ImputacionCostesLinModel>;
            model.LineasCostes = Session[sessioncostes] as List<ImputacionCostesCostesadicionalesModel>;

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


        #region agregar lineas controller nueva linea registro

        [HttpPost]
        public ActionResult AgregarLineas(ImputacionCostesLinVistaModel model)
        {

            var errormessage = "";
            try
            {
                var listadoLotes = Session[sessionlotes] as List<ImputacionCostesLinModel>;

                using (var imputacionCostesService = FService.Instance.GetService(typeof(ImputacionCostesModel), ContextService) as ImputacionCostesService)
                {

                    listadoLotes = imputacionCostesService.CrearLineasLotes(listadoLotes.ToList(), model); //CREAMOS LA SALIDA
                    Session[sessionlotes] = listadoLotes as List<ImputacionCostesLinModel>;
                }

                return Content(JsonConvert.SerializeObject(model), "application/json", Encoding.UTF8);
            }
            catch (Exception ex)
            {
                errormessage = ex.Message;
            }


            return Content("{\"error\":\"" + errormessage + "\"}", "application/json", Encoding.UTF8);
        }

        #endregion


        #region lineas lotes y costes grid

        //-----------------------LOTES-------------------------

        [ValidateInput(false)]
        public ActionResult ImputacionCostesLotesLin()
        {
            var model = Session[sessionlotes] as List<ImputacionCostesLinModel>;
            return PartialView("_imputacionloteslin", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ImputacionCostesLotesLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] ImputacionCostesLinModel item)
        {
            var model = Session[sessionlotes] as List<ImputacionCostesLinModel>; //LINEAS DE ENTRADA
            item.Id = model.Count() + 1; //0+1=1
            model.Add(item);
            Session[sessionlotes] = model;
            return PartialView("_imputacionloteslin", model); //DEVOLVEMOS LA VISTA
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ImputacionCostesLotesLinDelete(string id)
        {
            var model = Session[sessionlotes] as List<ImputacionCostesLinModel>;
            var intid = int.Parse(id);
            //var linea = model.Single(f => f.Id == intid);
            
            model.Remove(model.Single(f => f.Id == intid));
            Session[sessionlotes] = model;

            return PartialView("_imputacionloteslin", model);
        }

        //-----------------COSTES-------------

        [ValidateInput(false)]
        public ActionResult ImputacionCostesCostesLin()
        {
            var model = Session[sessioncostes] as List<ImputacionCostesCostesadicionalesModel>;
            return PartialView("_imputacioncostescostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ImputacionCostesCostesLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] ImputacionCostesCostesadicionalesModel item)
        {
            var model = Session[sessioncostes] as List<ImputacionCostesCostesadicionalesModel>; //LINEAS DE ENTRADA
            item.Id = model.Count() + 1; //0+1=1       
            model.Add(item);
            Session[sessioncostes] = model;
            return PartialView("_imputacioncostescostesadicionales", model); //DEVOLVEMOS LA VISTA
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ImputacionCostesCostesLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] ImputacionCostesCostesadicionalesModel item)
        {
            var model = Session[sessioncostes] as List<ImputacionCostesCostesadicionalesModel>;


            if (ModelState.IsValid)
            {

                var editItem = model.Single(f => f.Id == item.Id);
                var decimalesunidades = Funciones.Qint(Request.Params["decimalesunidades"]);
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

            return PartialView("_imputacioncostescostesadicionales", model);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult ImputacionCostesCostesLinDelete(string id)
        {
            var intid = int.Parse(id);
            var model = Session[sessioncostes] as List<ImputacionCostesCostesadicionalesModel>;
            model.Remove(model.Single(f => f.Id == intid));

            //Ir actualizando los ID en caso de que sufran cambios (eliminacion de lineas)
            if (model.Count() >= 1)
            {
                int count = 1;
                foreach (var linea in model)
                {
                    linea.Id = count;
                    count++;
                }
            }

            else
            {
                model[0].Id = 1;

            }
            
            Session[sessioncostes] = model;
            return PartialView("_imputacioncostescostesadicionales", model);
        }

        #endregion

        #region toolbar finalizar y estados

        //Generamos un nuevo toolbar
        protected override ToolbarModel GenerateToolbar(IGestionService service, TipoOperacion operacion, dynamic model = null)
        {
            var result = base.GenerateToolbar(service, operacion, model as object);
            result.Titulo = RImputacionCostes.TituloEntidad;

            return result;
        }


        //En Details el toolbar tiene solo la opcion de imprimir
        protected override IEnumerable<IToolbaritem> VerToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as ImputacionCostesModel;
            var result = base.VerToolbar(service, model).ToList();
            result.Add(new ToolbarSeparatorModel());
            result.Add(CreateComboImprimir(objModel));

            return result;
        }

        //En editar tiene imprimir y si el estado es introducido, tiene la opcion de finalizar el documento
        protected override IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var objModel = model as ImputacionCostesModel;
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

        //Boton imprimir
        private ToolbarActionComboModel CreateComboImprimir(ImputacionCostesModel objModel)
        {
            objModel.DocumentosImpresion = objModel.GetListFormatos();
            return new ToolbarActionComboModel()
            {
                Icono = "fa fa-print",
                Texto = General.LblImprimir,
                Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.ImputacionCostes, reportId = objModel.DocumentosImpresion.Defecto }),
                Target = "_blank",
                Items = objModel.DocumentosImpresion.Lineas.Select(f => new ToolbarActionModel()
                {
                    Url = Url.Action("Visualizar", "Designer", new { primaryKey = objModel.Referencia, tipo = TipoDocumentos.ImputacionCostes, reportId = f }),
                    Texto = f,
                    Target = "_blank"
                })
            };
        }

        //Boton Finalizar
        private ToolbarActionModel CreateComboEstados(ImputacionCostesModel objModel)
        {
            var estadosService = new EstadosService(ContextService);

            var estados = estadosService.GetStates(DocumentoEstado.ImputacionCostes, TipoEstado.Curso);
            var estadoFinalizado = estados.First(f => f.Tipoestado == TipoEstado.Finalizado);
            return new ToolbarActionModel()
            {
                Icono = "fa fa-gear",
                Texto = General.LblFinalizar,
                Url = Url.Action("CambiarEstado", "ImputacionCostes", 
                    new { documentoReferencia = objModel.Id, estadoNuevo = estadoFinalizado.CampoId,
                        returnUrl = Url.Action("Edit", "ImputacionCostes", new { id = objModel.Id }) })
            };
        }

        //Cambiamos de introducido a finalizado
        public ActionResult CambiarEstado(string documentoReferencia, string estadoNuevo, string returnUrl)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(ImputacionCostesModel), ContextService) as ImputacionCosteservice)
                {

                    //service.EjercicioId = ContextService.Ejercicio;
                    using (var estadosService = new EstadosService(ContextService))
                    {
                        var newModel = Helper.fModel.GetModel<ImputacionCostesModel>(ContextService);
                        var gestionService = createService(newModel);
                        var model = gestionService.get(documentoReferencia) as ImputacionCostesModel;

                        var listestados = estadosService.GetStates(DocumentoEstado.ImputacionCostes); //Lista de estados
                        var estadoFinalizado = listestados.First(f => f.Tipoestado == TipoEstado.Finalizado);
                        model.Fkestados = estadoFinalizado.CampoId;

                        //llamamos al servicio para que setee el estado
                        gestionService.edit(model);
                        TempData[Constantes.VariableMensajeExito] = "Imputación de costes terminada con éxito";
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

    }
}