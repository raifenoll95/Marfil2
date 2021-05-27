using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Model.Documentos.DivisionLotes;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.Model;
using Marfil.App.WebMain.Misc;
using Resources;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Newtonsoft.Json;
using System.Text;
using DevExpress.Web.Mvc;
using Marfil.Dom.Persistencia.Model.Documentos.Transformaciones;
using Marfil.Inf.Genericos.Helper;
using System.Net;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;

namespace Marfil.App.WebMain.Controllers
{

    public class DivisionLotesController : GenericController<DivisionLotesModel>
    {
        private const string sessionentrada = "_divisionlotesentrada_";
        private const string sessionsalida = "_divisionlotessalida_";
        private const string sessioncostes = "_divisionlotescostes";

        #region ABSTRACT

        public override bool CanCrear { get; set; }

        public override bool CanEliminar { get; set; }

        public override bool CanModificar { get; set; }

        public override bool IsActivado { get; set; }

        public override string MenuName { get; set; }

        protected override void CargarParametros()
        {

            MenuName = "divisionlotes";
            var permisos = appService.GetPermisosMenu("divisionlotes");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = false;
            CanEliminar = false;
        }

        #endregion

   
        #region CONSTRUCTOR

        public DivisionLotesController(IContextService context) : base(context)
        {
        }

        #endregion


        #region CRUD

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());


            using (var gestionService = FService.Instance.GetService(typeof(DivisionLotesModel), ContextService))
            {
                var model = TempData["model"] as DivisionLotesModel ?? Helper.fModel.GetModel<DivisionLotesModel>(ContextService);
                Session[sessionentrada] = model.LineasEntrada;
                Session[sessionsalida] = model.LineasSalida;
                Session[sessioncostes] = model.Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(DivisionLotesModel model)
        {
            try
            {
                var fmodel = new FModel();
                var newmodel = fmodel.GetModel<DivisionLotesModel>(ContextService);
                model.LineasEntrada = Session[sessionentrada] as List<DivisionLotesentradaLinModel>;
                model.LineasSalida = Session[sessionsalida] as List<DivisionLotessalidaLinModel>;
                model.Costes = Session[sessioncostes] as List<DivisionLotesCostesadicionalesModel>;

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


        #region Details

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var newModel = Helper.fModel.GetModel<DivisionLotesModel>(ContextService);
            using (var gestionService = createService(newModel))
            {

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                Session[sessionentrada] = ((DivisionLotesModel)model).LineasEntrada;
                Session[sessionsalida] = ((DivisionLotesModel)model).LineasSalida;
                Session[sessioncostes] = ((DivisionLotesModel)model).Costes;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                ViewBag.ReadOnly = true;
                return View(model);
            }
        }

        #endregion

        #region METODO COMPROBAR ENTRADAS CORRECTAS LLAMAR VALIDATION

        public void comprobarEntradas(List<DivisionLotesentradaLinModel> entradas, List<DivisionLotessalidaLinModel> salida)
        {
            var fmodel = new FModel();
            var divisionLotesModel = fmodel.GetModel<DivisionLotesModel>(ContextService);
            divisionLotesModel.LineasEntrada = entradas;
            divisionLotesModel.LineasSalida = salida;

            var divisionLotesService = createService(divisionLotesModel) as DivisionLotesService;
            divisionLotesService.entradasCorrectas(divisionLotesModel);
        }

        #endregion

        #region CRUD LINEAS ENTRADA

        [HttpPost, ValidateInput(false)]
        public ActionResult DivisionLotesentradaLinAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] DivisionLotesentradaLinModel item)
        {

            var model = Session[sessionentrada] as List<DivisionLotesentradaLinModel>; //LINEAS DE ENTRADA
            model.Add(item); //AÑADIMOS ELEMENTO
            comprobarEntradas(model, Session[sessionsalida] as List<DivisionLotessalidaLinModel>); //COMPROBAMOS LAS ENTRADAS
            return PartialView("_divisionlotesentradalin", model); //DEVOLVEMOS LA VISTA
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult DivisionLotesentradaLinUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] DivisionLotesentradaLinModel item)
        {
            var model = Session[sessionentrada] as List<DivisionLotesentradaLinModel>; //LINEAS DE ENTRADA

            if (ModelState.IsValid)
            {

                var editItem = model.Single(f => f.Id == item.Id);
                editItem.Largo = item.Largo;
                editItem.Ancho = item.Ancho;
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

            //COMPROBAMOS LAS ENTRADAS EN EL ULTIMO REGISTRO DE ENTRADA
            if(item.Id==model.Count)
            {
                comprobarEntradas(Session[sessionentrada] as List<DivisionLotesentradaLinModel>, Session[sessionsalida] as List<DivisionLotessalidaLinModel>); //COMPROBAMOS LAS ENTRADAS
            }

            return PartialView("_divisionlotesentradalin", Session[sessionentrada] as List<DivisionLotesentradaLinModel>); //DEVOLVEMOS LA VISTA
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult DivisionLotesentradaLinDelete(string id)
        {

            var model = Session[sessionentrada] as List<DivisionLotessalidaLinModel>; //LINEAS DE ENTRADA
            model.Remove(model.Single(f => f.Id.ToString() == id)); //ELIMINAMOS EL CORRESPONDIENTE

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

            Session[sessionentrada] = model;
            comprobarEntradas(Session[sessionentrada] as List<DivisionLotesentradaLinModel>, Session[sessionsalida] as List<DivisionLotessalidaLinModel>); //COMPROBAMOS LAS NUEVAS ENTRADAS
            return PartialView("_divisionlotesentradalin", model);
        }

        #endregion


        [HttpPost, ValidateInput(false)]
        public ActionResult DivisionLotessalidaLinDelete(string id)
        {

            //ELIMINAMOS LA SALIDA
            var modelSalida = Session[sessionsalida] as List<DivisionLotessalidaLinModel>;
            modelSalida.Clear();
            Session[sessionsalida] = modelSalida;

            //ELIMINAMOS LA ENTRADA
            var modelEntrada = Session[sessionentrada] as List<DivisionLotesentradaLinModel>;
            modelEntrada.Clear();
            Session[sessionentrada] = modelEntrada;

            return PartialView("_divisionlotessalidalin", modelSalida);
        }

        [ValidateInput(false)]
        public ActionResult DivisionLotesCostesAdicionales()
        {
            var model = Session[sessioncostes] as List<DivisionLotesCostesadicionalesModel>;
            return PartialView("_divisionlotescostesadicionales", model);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CostesadicionalesNew([ModelBinder(typeof(DevExpressEditorsBinder))] DivisionLotesCostesadicionalesModel item)
        {

            var model = Session[sessioncostes] as List<DivisionLotesCostesadicionalesModel>;
            
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

            return PartialView("_divisionlotescostesadicionales", model);
        }

        #endregion


        #region entradas y salidas

        [ValidateInput(false)]
        public ActionResult DivisionLotesentradaLin()
        {
            var model = Session[sessionentrada] as List<DivisionLotesentradaLinModel>;
            return PartialView("_divisionlotesentradalin", model);
        }

        [ValidateInput(false)]
        public ActionResult DivisionLotessalidaLin()
        {
            var model = Session[sessionsalida] as List<DivisionLotessalidaLinModel>;
            return PartialView("_divisionlotessalidalin", model);
        }

        #endregion


        #region AGREGAR LINEAS DE ENTRADA Y SALIDA

        [HttpPost]
        public ActionResult AgregarLineas(DivisionLotessalidaLinVistaModel model)
        {

            var errormessage = "";
            try
            {
                var listadoSalida = Session[sessionsalida] as List<DivisionLotessalidaLinModel>;
                var listadoEntrada = Session[sessionentrada] as List<DivisionLotesentradaLinModel>;

                //NO PUEDE HABER MAS DE UNA SALIDA
                if (listadoSalida.Count>=1)
                {
                    throw new ValidationException("Ya se ha añadido un lote de salida");
                }

                using (var divisionLotesService = FService.Instance.GetService(typeof(DivisionLotesModel), ContextService) as DivisionLotesService)
                {

                    listadoSalida = divisionLotesService.CrearNuevasLineasSalida(listadoSalida, model); //CREAMOS LA SALIDA
                    Session[sessionsalida] = listadoSalida;

                    //CREAMOS LAS ENTRADAS
                    DivisionLotessalidaLinVistaModel linea = new DivisionLotessalidaLinVistaModel();
                    
                    listadoEntrada = divisionLotesService.CrearDosNuevasLineasEntrada(listadoSalida, listadoEntrada);
                    Session[sessionentrada] = listadoEntrada;
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

        protected override IGestionService createService(IModelView model)
        {
            var result = FService.Instance.GetService(typeof(DivisionLotesModel), ContextService) as DivisionLotesService;
            result.EjercicioId = ContextService.Ejercicio;
            return result;
        }

    }
}