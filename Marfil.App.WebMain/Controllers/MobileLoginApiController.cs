using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI.WebControls;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Login;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class StMobileLogin
    {
        public string Dominio { get; set; }
        public string Usuario { get; set; }
        public string Password { get; set; }
        public string Basedatos { get; set; }
    }

    public class MobileLoginApiController : ApiBaseController
    {
        public MobileLoginApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Post(StMobileLogin model)
        {

            using (var loginService = new LoginService())
            {
                SecurityTicket usuario;
                HttpResponseMessage responseError;
                try
                {
                    if (loginService.Login(model.Dominio, model.Usuario, model.Password, out usuario))
                    {
                        if (usuario != null)
                        {
                            var serializer = new JavaScriptSerializer();
                            var userData = serializer.Serialize(usuario);
                            var authTicket = new FormsAuthenticationTicket(
                                     1,
                                        model.Usuario,
                                     DateTime.Now,
                                     DateTime.Now.AddMinutes(120),//meter en settings
                                     false,
                                     userData);

                            var encTicket = FormsAuthentication.Encrypt(authTicket);
                            var securityCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                            securityCookie.Expires = DateTime.Now.AddMinutes(120);
                            HttpContext.Current.Response.Cookies.Add(securityCookie);

                        }
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(JsonConvert.SerializeObject(usuario), Encoding.UTF8, "application/json");

                        return response;
                    }
                    responseError = Request.CreateResponse(HttpStatusCode.NotAcceptable);
                }
                catch (Exception ex)
                {
                    responseError = Request.CreateResponse(HttpStatusCode.NotAcceptable);
                    responseError.Content = new StringContent(ex.Message, Encoding.UTF8, "application/json");
                }
                return responseError;
            }
        }

        
    }
}