using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DevExpress.Web.Mvc;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Listados;
using System.IO;
using System.Net;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Newtonsoft.Json;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public  class ListadosController<T> : Controller
    {
        private string _formatocookieId = "{0}-{1}";
        private readonly IContextService _context;
        public ListadosController(IContextService context)
        {
            _context = context;
        }

        protected IContextService Context { get { return _context; } }

        #region Listados

        public ActionResult Listado(string id)
        {

            return View(GetModelListado(id));
        }

        protected virtual IEnumerable<IToolbaritem> HazTuToolbar()
        {
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Listado(T model)
        {
            if (ModelState.IsValid)
            {
                var listmodel = model as ListadosModel;
                
                Session["datalistados"] = model;
                //TODO EL: Revisar esto porque casca cuando usamos muchas cookies
                //SetModelListado(model);
                return RedirectToAction("Result");
            }

            return View(model);
        }

        #endregion

        #region Resultado

        public virtual ActionResult Result()
        {
            if (Session["datalistados"] == null)
                return RedirectToAction("Index", "Home");

            var service = new ListadosService();
            var model = Session["datalistados"] as ListadosModel;
            model.Context = _context;
            var listadomodel = service.Listar(model);
            Session[listadomodel.IdListado + "_resultado"] = listadomodel;

            
            using (var db = MarfilEntities.ConnectToSqlServer(_context.BaseDatos))
            {
                using (var servicePreferencias = new PreferenciasUsuarioService(db))
                {
                    var result = servicePreferencias.GePreferencia(TiposPreferencias.ConfiguracionListado, _context.Id, listadomodel.IdListado, "Defecto") as PreferenciaConfiguracionListado;
                    if (result != null)
                    {
                        Session[listadomodel.IdListado + "UserSettings"] = result.SettingsDevexpress;
                    }
                }
            }

            return View(Session[listadomodel.IdListado + "_resultado"] as ListadoResultado);
        }

        public ActionResult ResultSimple(string id)
        {
            return View(Session[id + "_resultado"] as ListadoResultado);
        }

        #endregion

        #region Grid

        [ValidateInput(false)]
        public ActionResult _listResult(string id)
        {
            return PartialView("_listResult", Session[id + "_resultado"] as ListadoResultado);
        }

        #endregion

        #region ExportTo

        public ActionResult ExportTo(string id, string OutputFormat)
        {
            ActionResult obj;
            var model = Session[id + "_resultado"] as ListadoResultado;
          
            var settings = Session[id + "Settings"] as GridViewSettings;
            if (settings.Columns["Action"] != null)
                settings.Columns.Remove(settings.Columns["Action"]);

            switch (OutputFormat.ToUpper())
            {
                case "CSV":
                    obj = GridViewExtension.ExportToCsv(settings, model.Listado, true);
                    break;
                case "PDF":
                    obj = GridViewExtension.ExportToPdf(settings, model.Listado, true);
                    break;
                case "RTF":
                    obj = GridViewExtension.ExportToRtf(settings, model.Listado, true);
                    break;
                case "XLS":
                    obj = GridViewExtension.ExportToXls(settings, model.Listado, true);
                    break;
                case "XLSX":
                    obj = GridViewExtension.ExportToXlsx(settings, model.Listado, true);
                    break;
                default:
                    obj = RedirectToAction("Index", "Home");
                    break;
            }

            var result = obj as FileStreamResult;
            result.FileDownloadName = (model.TituloListado ?? "ListadosMarfil") + "." + OutputFormat.ToLower();
            return result;
        }



        #endregion

        #region SaveSettings

        public ActionResult SaveSettings(string id)
        {
            if (Session[id + "UserSettings"] != null)
            {

                var nombre = HttpContext.Request.Params["nombre"];
                var settings = Session[id + "UserSettings"];
              
                using (var db = MarfilEntities.ConnectToSqlServer(_context.BaseDatos))
                {
                    using (var service = new PreferenciasUsuarioService(db))
                    {
                        service.SetPreferencia(TiposPreferencias.ConfiguracionListado, _context.Id, id, nombre, new PreferenciaConfiguracionListado() { SettingsDevexpress = settings.ToString() });
                    }
                }
                return new EmptyResult();
            }

            throw new ValidationException("Ups! no se pudieron guardar los settings");
        }

        #endregion

        #region Generar grafica

        public ActionResult GenerarGrafica()
        {
            var model = Session["datalistados"] as ListadosModel;
            TempData["configuracion"] = model;
            return RedirectToAction("Create", "Configuraciongraficas", new {id = model.WebIdListado});
        }
        #endregion

        #region Helpers

        private IListados GetModelListado(string id)
        {
           
            var model= FListadosModel.Instance.GetModel(_context, id, _context.Empresa, _context.Ejercicio);

            var helper = new UrlHelper(HttpContext.Request.RequestContext);
            var aux = model as IToolbar;
            aux.Toolbar = new ToolbarListadosModel();
            aux.Toolbar.Titulo = model.TituloListado;
            //aux.Toolbar.Acciones = HelpItem();
            List<IToolbaritem> lista = new List<IToolbaritem>();
            var accionespropias = HazTuToolbar();
            if(accionespropias != null && accionespropias.Count() > 0)
            {
                lista.AddRange(accionespropias);                            
            }
            lista.InsertRange(0, HelpItem());
            aux.Toolbar.Acciones = lista;
            aux.Toolbar.CustomAction = true;
            aux.Toolbar.CustomActionName = helper.Action("Listado", new {id});

            return model;
            /*
            TODO EL: Revisar esto porque casca cuando guardamos muchas cookies
            var cookie = HttpContext.Request.Cookies.Get(string.Format(_formatocookieId, id, custom.Empresa));

            return cookie == null
                ? FListadosModel.Instance.GetModel(id, custom.Empresa)
                : JsonConvert.DeserializeObject<T>(cookie.Value) as IListados;*/
        }

        protected IEnumerable<IToolbaritem> HelpItem()
        {
            var appService = new ApplicationHelper(_context);
            var ayudaModel = appService.GetAyudaModel();
            var action = ControllerContext.RouteData.Values["action"].ToString();
            var controller = ControllerContext.RouteData.Values["controller"].ToString();
            var item = ayudaModel.List.FirstOrDefault(f => f.Controller == controller && f.Action == action);
            if (item == null)
                item = ayudaModel.List.FirstOrDefault(f => f.Controller == controller && string.IsNullOrEmpty(f.Action));

            var result = new List<IToolbaritem>();
            if (item != null)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-question-circle fa-1-5x green",
                    OcultarTextoSiempre = true,
                    Target = "_blank",
                    Url = item.Url,
                    Texto = General.LblAyuda
                });
                result.Add(new ToolbarSeparatorModel());
            }


            return result;

        }
        /*
         TODO EL: Revisar esto porque casca cuando guardamos muchas cookies
        private void SetModelListado(T model)
        {

            var custom = HttpContext.User as ICustomContextService.
            var auxmodel = model as IListados;
            var cookieid = string.Format(_formatocookieId, auxmodel.IdListado, custom.Empresa);
            var cookie = HttpContext.Request.Cookies.Get(cookieid);
            if (cookie != null)
            {
                HttpContext.Response.Cookies.Remove(cookieid);
            }

            var json = JsonConvert.SerializeObject(model);
            var userCookie = new HttpCookie(cookieid, json);
            userCookie.Expires = DateTime.Now.AddDays(365);
            HttpContext.Response.Cookies.Add(userCookie);
        }*/

        #endregion
    }
}