using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;

using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using System.Threading;
using System.Resources;
using System.Reflection;
using System.IO;
using System.Globalization;
using Marfil.App.WebMain.Misc.DataAnnotations;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Web.SessionState;
using DevExpress.Data.Filtering;
using Marfil.App.WebMain.Misc.Designer;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Inf.Genericos.Helper;
using System.Transactions;
using System.Web.UI;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Login;

namespace Marfil.App.WebMain
{
    public class MvcApplication : System.Web.HttpApplication
    {


        void Application_Error(Object sender, EventArgs e)
        {
            /*
            Exception ex = Server.GetLastError().GetBaseException();

            if (ex is Marfil.Dom.Persistencia.Helpers.ValidationException)
                return;
            if ((ex as HttpException)?.GetHttpCode() == 404)
                return;

            WebHelper.Log.AddLog(ex);
            */

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            WebHelper.Log = new LogService();

            ModelBinders.Binders.Add(typeof(decimal), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(decimal?), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(double), new DecimalModelBinder());
            ModelBinders.Binders.Add(typeof(double?), new DecimalModelBinder());
            CriteriaOperator.RegisterCustomFunction(new AccentFilter());
            //Data annotations
            DataAnnotationsModelValidatorProvider.RegisterAdapter(typeof(RequiredAttribute), typeof(CustomRequiredAttributeAdapter));
            DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension.RegisterExtensionGlobal(new CustomReportStorageWebExtension());
            ModelBinders.Binders.DefaultBinder = new DevExpress.Web.Mvc.DevExpressEditorsBinder();

        }

        private void UnobtrusiveGlobalization()
        {
            string name = string.Format("Local{0}", "_" + Thread.CurrentThread.CurrentUICulture.Name);

            if (Helper.GetTypeFromFullName("Resources." + name) != null)
            {
                ClientDataTypeModelValidatorProvider.ResourceClassKey = name;
                DefaultModelBinder.ResourceClassKey = name;
            }
            else
            {
                ClientDataTypeModelValidatorProvider.ResourceClassKey = "Local";
                DefaultModelBinder.ResourceClassKey = "Local";
            }


        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var langCookie = HttpContext.Current.Request.Cookies["lang"];
            CultureInfo ci;
            if (langCookie == null)
            {
                var langName = "es";
                ci = new CultureInfo(langName);

                //Try to get values from Accept lang HTTP header
                if (HttpContext.Current.Request.UserLanguages != null && HttpContext.Current.Request.UserLanguages.Length != 0)
                {
                    //Gets accepted list 
                    langName = HttpContext.Current.Request.UserLanguages[0].Substring(0, 2);
                }

                langCookie = new HttpCookie("lang", langName)
                {
                    Expires = DateTime.MaxValue
                };


                HttpContext.Current.Response.AppendCookie(langCookie);

            }
            else
            {
                ci = new CultureInfo(langCookie.Value);
            }

            Thread.CurrentThread.CurrentUICulture = ci;
            Thread.CurrentThread.CurrentCulture = ci;
        }

        void Application_BeginRequest(object sender, EventArgs e)
        {

            try
            {
                var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {
                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    var serializer = new JavaScriptSerializer();
                    var serializeModel = serializer.Deserialize<SecurityTicket>(authTicket.UserData);
                }

            }
            catch (Exception)
            {

                throw;
            }

        }

        

        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            //lo convierto a Invariant porque sino DevExpress casca!! 
            //Devexpess no utiliza las culturas especificas como es-ES. Siempre hay que convertirlo: es-ES -> es
            var newCultureInvariant = new CultureInfo(Thread.CurrentThread.CurrentUICulture.Name.Split('-')[0]);
            Thread.CurrentThread.CurrentCulture = newCultureInvariant;
            Thread.CurrentThread.CurrentUICulture = newCultureInvariant;
            UnobtrusiveGlobalization();

            if (Funciones.Qbool(ConfigurationManager.AppSettings["Modopruebaslogin"]))
            {
                HttpContext.Current.User = new CustomPrincipal(ApplicationHelper.UsuariosAdministrador)
                {
                    Id = Guid.Empty,
                    Idconexion = Guid.Empty,
                    RoleId = Guid.Empty,
                    Usuario = ApplicationHelper.UsuariosAdministrador,
                    BaseDatos = "marfil",
                    Roles = new List<string>(),
                    Empresa = "0001",
                    Ejercicio = "2",
                    Fkalmacen = "0010",
                    Azureblob = ""
                };
            }
            else
            {
                var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null)
                {

                    var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                    var serializer = new JavaScriptSerializer();
                    var serializeModel = serializer.Deserialize<SecurityTicket>(authTicket.UserData);
                    HttpContext.Current.User = new CustomPrincipal(authTicket.Name)
                    {
                        Id = serializeModel.Id,
                        Idconexion = serializeModel.Idconexion,
                        RoleId = serializeModel.RoleId,
                        Usuario = serializeModel.Usuario,
                        BaseDatos = serializeModel.BaseDatos,
                        Roles = serializeModel.Roles,
                        Empresa = serializeModel.Empresa,
                        Ejercicio = serializeModel.Ejercicio,
                        Fkalmacen = serializeModel.Fkalmacen,
                        Tipolicencia = serializeModel.Tipolicencia,
                        Azureblob=serializeModel.Azureblob
                        
                    };

                    using (var loginservice = new LoginService())
                    {
                        if (!loginservice.Existeusuarioactivo(serializeModel.BaseDatos, serializeModel.Idconexion))
                        {
                            //Forzamos a echar de la aplicacion
                            FormsAuthentication.SignOut();
                            Response.RedirectToRoute("Default");
                        }
                    }
                }
            }


        }
    }
}
