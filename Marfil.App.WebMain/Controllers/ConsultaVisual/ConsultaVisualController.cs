using Marfil.Dom.Persistencia.Model.Stock;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using System;
using Marfil.Dom.Persistencia.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace Marfil.App.WebMain.Controllers
{
    public class ConsultaVisualController : GenericController<ConsultaVisualFullModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {

            MenuName = "consultavisual";
            var permisos = appService.GetPermisosMenu("consultavisual");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        #region ctr

        public ConsultaVisualController(IContextService context) : base(context)
        {

        }

        #endregion

        //Pantalla Almacenes
        [ValidateInput(false)]
        public ActionResult Almacenes() {
            var model = Helper.fModel.GetModel<ConsultaVisualFullModel>(ContextService) as ConsultaVisualFullModel;
            var empresa = appService.GetCurrentEmpresa();
            model.Empresa = empresa.Id;
            model.DescEmpresa = empresa.Nombre;
            return View(model);
        }

        //Pantalla Familias
        [System.Web.Http.HttpPost]
        [ValidateInput(false)]
        public ActionResult Familias(String seleccion)
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            ConsultaVisualFullModel modelo = serializer1.Deserialize<ConsultaVisualFullModel>(seleccion);
            return View(modelo);
        }

        //Pantalla Productos
        public ActionResult GruposMateriales(String seleccion) {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            ConsultaVisualFullModel modelo = serializer1.Deserialize<ConsultaVisualFullModel>(seleccion);
            return View(modelo);
        }

        //Pantalla Productos
        public ActionResult Materiales(String seleccion) {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            ConsultaVisualFullModel modelo = serializer1.Deserialize<ConsultaVisualFullModel>(seleccion);
            return View(modelo);
        }

        //Pantalla Producto Final
        public ActionResult Lotes(String seleccion) {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            ConsultaVisualFullModel modelo = serializer1.Deserialize<ConsultaVisualFullModel>(seleccion);
            return View(modelo);
        }


        //GET: api/Supercuentas/5
        public ActionResult getAlmacenes(string seleccion)
        {
            using (var servicioAlmacenes = FService.Instance.GetService(typeof(ConsultaVisualModel), ContextService) as ConsultaVisualService)
            {
                JavaScriptSerializer serializer1 = new JavaScriptSerializer();
                ConsultaVisualFullModel modelo = serializer1.Deserialize<ConsultaVisualFullModel>(seleccion);
                var listadoAlmacenes = servicioAlmacenes.obtenerAlmacenes(modelo);
                var data = JsonConvert.SerializeObject(listadoAlmacenes, Formatting.Indented);
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }


        //Obtener los Familias de Stock de ese amlacen
        public JsonResult getFamilias(string seleccion)
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            ConsultaVisualFullModel modelo = serializer1.Deserialize<ConsultaVisualFullModel>(seleccion);
            var servicioStockActual = FService.Instance.GetService(typeof(ConsultaVisualModel), ContextService) as ConsultaVisualService;
            var listadoFamilias = servicioStockActual.getFamilias(modelo);
            var data = JsonConvert.SerializeObject(listadoFamilias, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        //Grupos de materiales
        public JsonResult getGruposMateriales(string seleccion)
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            ConsultaVisualFullModel modelo = serializer1.Deserialize<ConsultaVisualFullModel>(seleccion);
            var servicioStockActual = FService.Instance.GetService(typeof(ConsultaVisualModel), ContextService) as ConsultaVisualService;
            var listadoGrupoMateriales = servicioStockActual.getGruposMateriales(modelo);
            var data = JsonConvert.SerializeObject(listadoGrupoMateriales, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Materiales stock
        public JsonResult getMateriales(string seleccion)
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            ConsultaVisualFullModel modelo = serializer1.Deserialize<ConsultaVisualFullModel>(seleccion);
            var servicioStockActual = FService.Instance.GetService(typeof(ConsultaVisualModel), ContextService) as ConsultaVisualService;
            var listadoMateriales = servicioStockActual.getMateriales(modelo);
            var data = JsonConvert.SerializeObject(listadoMateriales, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //Materiales stock
        public JsonResult getMaterial(string seleccion)
        {
            JavaScriptSerializer serializer1 = new JavaScriptSerializer();
            ConsultaVisualFullModel modelo = serializer1.Deserialize<ConsultaVisualFullModel>(seleccion);
            var servicioStockActual = FService.Instance.GetService(typeof(ConsultaVisualModel), ContextService) as ConsultaVisualService;
            var listadoArticulos = servicioStockActual.getMaterialEspecifico(modelo);
            var data = JsonConvert.SerializeObject(listadoArticulos, Formatting.Indented);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}