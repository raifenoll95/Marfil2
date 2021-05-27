using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model.Graficaslistados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Login;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IContextService _context;

        public HomeController(IContextService context)
        {
            _context = context;
           
        }

        public ActionResult Index()
        {

            //Rai- errores permisos niveles
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var model = new PanelcontrolModel();
            try
            {
                using (var db = MarfilEntities.ConnectToSqlServer(_context.BaseDatos))
                {
                    using (var service = new PreferenciasUsuarioService(db))
                    {
                        var serviceGraficas= new ConfiguraciongraficasService(_context,db);
                        var preferencias = service.GePreferencia(TiposPreferencias.PanelControlDefecto, _context.Id, "1", "Defecto") as PreferenciaPanelControlDefecto;
                        
                        model.Paneles = preferencias != null ? preferencias.GetPanelesControl(_context.Empresa).Select(f=>serviceGraficas.get(f) as ConfiguraciongraficasModel).ToList() : Enumerable.Empty<ConfiguraciongraficasModel>().ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                
            }
            return View(model);
        }

        //Rai
        public ActionResult CambioEmpresa(string id)
        {
            var model = new PanelcontrolModel();
            try
            {
                using (var db = MarfilEntities.ConnectToSqlServer(_context.BaseDatos))
                {
                    using (var service = new PreferenciasUsuarioService(db))
                    {
                        LoginService _serviceLogin = new LoginService();
                        var dominio = System.Web.HttpContext.Current.Request.Url.DnsSafeHost;

                        if (_serviceLogin.puedeCambiarEmpresa(_context.BaseDatos, _context.Usuario, id, dominio))
                        {
                            return RedirectToAction("Index", "CambioEmpresa", new { id = id });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["errors"] = ex.Message;

            }
            return RedirectToAction("Index");
        }

        [ValidateInput(false)]
        public ActionResult _listIndex(string id)
        {
            return PartialView("_listIndex", Session[id] as ListIndexModel);
        }

        [ChildActionOnly]
        public ActionResult _lateralHeader()
        {
            var appService = new ApplicationHelper(_context);
            return PartialView(appService.CreateMenuAplicacionJavascriptUsuario(WebHelper.DevexpressAA));
        }


        public ActionResult ExportTo(string exportid,string OutputFormat)
        {
            ActionResult obj;
            var model = Session[exportid] as ListIndexModel;
            var settings = Session[exportid + "Settings"] as GridViewSettings;
            if(settings.Columns["Action"]!=null)
                settings.Columns.Remove(settings.Columns["Action"]);
            
            switch (OutputFormat.ToUpper())
            {
                case "CSV":
                    obj= GridViewExtension.ExportToCsv(settings, model.List,true);
                    break;
                case "PDF":
                    obj = GridViewExtension.ExportToPdf(settings, model.List, true);
                    break;
                case "RTF":
                    obj = GridViewExtension.ExportToRtf(settings, model.List, true);
                    break;
                case "XLS":
                    obj = GridViewExtension.ExportToXls(settings, model.List, true);
                    break;
                case "XLSX":
                    obj = GridViewExtension.ExportToXlsx(settings, model.List, true);
                    break;
                default:
                    obj = RedirectToAction("Index");
                    break;
            }

            var result = obj as FileStreamResult;
            result.FileDownloadName =( model.Entidad?? "ExportMarfil" ) + "." + OutputFormat.ToLower();
            return result;
        }
    }
}