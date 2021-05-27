using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Marfil.App.WebMain.Misc;
using Marfil.App.WebMain.Services;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Login;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Threading;

namespace Marfil.App.WebMain.Controllers
{
    public class StartupController : Controller
    {

        private readonly string _dominio;

        public StartupController()
        {
            _dominio = System.Web.HttpContext.Current.Request.Url.DnsSafeHost;
        }

        #region Usuarios admin

        public ActionResult Admin(string id)
        {
            return View(new RootModel() { DataBase = id });
        }

        [HttpPost]
        public ActionResult Admin(RootModel model)
        {

            if (ModelState.IsValid)
            {
                var context = new ContextService();
                context.BaseDatos = model.DataBase;
                using (var service = new StartupService(context,model.DataBase))
                {
                    //Create user password
                    service.CreateAdmin(model.Password);
                    using (var lservice = new LoginService())
                    {
                        HttpCookie cookie;
                        lservice.Login(_dominio,ApplicationHelper.UsuariosAdministrador, model.Password, out cookie, ApplicationHelper.EmpresaMock,string.Empty,string.Empty);

                        Response.Cookies.Add(cookie);
                    }
                    return RedirectToAction("DatosDefecto", new { id = model.DataBase });
                }
            }
            return View(model);
        }

        #endregion

        #region Cargar Datos por defecto
        
        public ActionResult DatosDefecto(string id)
        {
            var context = new ContextService();
            context.BaseDatos = id;
            using (var service = new StartupService(context,id))
            {
                return View(service.GetDatosDefecto());
            }
        }

        [HttpPost]
        public ActionResult DatosDefecto(string id, IEnumerable<DatosDefectoItemModel> model)
        {
            var context = new ContextService();
            context.BaseDatos = id;
            using (var service = new StartupService(context, id))
            {
                var serviceConfiguracion = new ConfiguracionService(context);
                serviceConfiguracion.SetCargaDatos(1);

                if (model != null)
                    //service.CreateDatosDefecto(model);
                    HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsync(id, model, token, context));

                

                return RedirectToAction("Empresa", new { id });
            }
        }

        private async Task GetAsync(string id, IEnumerable<DatosDefectoItemModel> entidades, CancellationToken cancellationToken, ContextService _context)
        {
            var context = _context;
            context.BaseDatos = id;
            var serviceConfiguracion = new ConfiguracionService(context);
            //serviceConfiguracion.SetCargaDatos(1);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = new StartupService(context, id))
                {
                    await Task.Run(() => service.CreateDatosDefecto(entidades));
                    serviceConfiguracion.SetCargaDatos(2);
                    return;
                }

            }
            catch (TaskCanceledException tce)
            {                                
                serviceConfiguracion.SetCargaDatos(-1);

            }
            catch (Exception ex)
            {                
                serviceConfiguracion.SetCargaDatos(-1);
            }
        }

        #endregion

        #region Empresa

        public ActionResult Empresa(string id)
        {
            var context = new ContextService();
            context.BaseDatos = id;
            using (var service = new StartupService(context, id))
            {
                var serviceConfiguracion = new ConfiguracionService(context, service.Db);
                var estadoImportacion = serviceConfiguracion.GetCargaDatos();
                EmpresaModel model;

                if (estadoImportacion == 2)
                {
                    ViewBag.database = id;
                    model = Helper.fModel.GetModel<EmpresaModel>(context, service.Db);
                    model.EstadoImportacion = estadoImportacion;
                }
                else
                {
                    model = new EmpresaModel();
                    model.EstadoImportacion = estadoImportacion;
                }
                

                return View(model);
            }

        }

        [HttpPost]
        public ActionResult Empresa(string database, EmpresaModel model)
        {
            ViewBag.database = database;
            if (ModelState.IsValid)
            {
                var context = new ContextService();
                context.BaseDatos = database;
                using (var service = new StartupService(context, database))
                {
                    try
                    {
                        if (model != null)
                        {
                            var nModel = Helper.fModel.GetModel<EmpresaModel>(context,service.Db);
                            model.Paises = nModel.Paises;
                            model.PlanesGenerales = nModel.PlanesGenerales;
                            var aux = Helper.fModel.GetModel<EmpresaModel>(context,service.Db);
                            model.PlanesGenerales = aux.PlanesGenerales;
                            model.Paises = aux.Paises;
                            model.LstTarifasVentas = aux.LstTarifasVentas;
                            model.LstTarifasCompras = aux.LstTarifasCompras;
                            service.CreateEmpresa(model);
                            using (var loginService = new LoginService())
                            {
                                HttpCookie securityCookie;
                                FormsAuthentication.SignOut();
                                loginService.Forzardesconexion(database,ApplicationHelper.UsuariosAdministrador);
                                loginService.SetEmpresaUserAdmin(_dominio,database, model.Id,string.Empty,string.Empty,Guid.NewGuid(), out securityCookie);
                                Response.Cookies.Add(securityCookie);
                            }
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        


                    }

                }
            }
          
           
            return View(model);
        }

        #endregion

        #region Helpers

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}