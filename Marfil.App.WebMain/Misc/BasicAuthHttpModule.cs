using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Marfil.App.WebMain.Services;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Net.Http;
using System.Web.Http;
using System.Net;

namespace Marfil.App.WebMain.Misc
{
    public class ErrorJson
    {
        public string Error { get; set; }

        public ErrorJson(string error)
        {
            Error = error;
        }
    }



    public class BasicAuthHttpModule : Controller
    {
        #region Members

        private readonly ILoginService _service;

        #endregion

        #region Properties

        public IContextService ContextService { get; set; }
        private readonly string _dominio;
        #endregion

        #region CTR

        public BasicAuthHttpModule(ILoginService service)
        {
            try
            {
                _service = service;
                var request = System.Web.HttpContext.Current.Request;
                _dominio = request?.Url?.DnsSafeHost;
                GetParametrosConexion();
            }
            catch (Exception ex)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotAcceptable)
                {
                    Content = new StringContent(ex.Message),
                    ReasonPhrase = ex.Message
                };
                throw new HttpResponseException(resp);
            }
        }

        #endregion


        [HandleError()]
        public ActionResult SomeError()
        {
            return Json(new ErrorJson("test error"));

        }

        //private bool CheckPassword(string username, string password, out HttpCookie security)
        //{
        //    return _service.Login(_dominio, username, password, out security);
        //}

        //private void AuthenticateUser(string dominio, string usuario,string password, Guid idConexion)
        //{
        //    HttpCookie security;
        //    if (_service.checkPassword(dominio, usuario, password))
        //    {
        //        if(!_service.Existeusuarioactivo(usuario, idConexion))
        //        {
        //            throw new Exception("La sesión caducó");
        //        }
        //        var authTicket = FormsAuthentication.Decrypt(security.Value);
        //        var serializer = new JavaScriptSerializer();
        //        var serializeModel = serializer.Deserialize<SecurityTicket>(authTicket.UserData);
        //        var principal = new CustomPrincipal(authTicket.Name)
        //        {
        //            Id = serializeModel.Id,
        //            RoleId = serializeModel.RoleId,
        //            Usuario = serializeModel.Usuario,
        //            BaseDatos = serializeModel.BaseDatos,
        //            Roles = serializeModel.Roles,
        //            Empresa = serializeModel.Empresa,
        //            Ejercicio = serializeModel.Ejercicio,
        //            Fkalmacen = serializeModel.Fkalmacen
        //        };

        //        System.Web.HttpContext.Current.User = principal;

        //        ContextService = new ContextService()
        //        {
        //            Id = serializeModel.Id,
        //            RoleId = serializeModel.RoleId,
        //            Usuario = serializeModel.Usuario,
        //            BaseDatos = serializeModel.BaseDatos,
        //            Empresa = serializeModel.Empresa,
        //            Ejercicio = serializeModel.Ejercicio,
        //            Fkalmacen = serializeModel.Fkalmacen
        //        };

        //    }
        //    else
        //    {
        //        throw new Exception("Usuario o contaseña incorrectos");
        //    }
        //}


        private void AuthenticateUser(string dominio, string usuario, string password, Guid idConexion)
        {
            string basedatos;
            if (_service.checkPassword(dominio, usuario, password, out basedatos))
            {

                if (!_service.Existeusuarioactivo(basedatos, idConexion))
                {
                    throw new Exception("La sesión caducó");
                }
                var principal = _service.getUserActivo(dominio, usuario, password, idConexion);

                System.Web.HttpContext.Current.User = principal;

                ContextService = new ContextService()
                {
                    Id = principal.Id,
                    RoleId = principal.RoleId,
                    Usuario = principal.Usuario,
                    BaseDatos = principal.BaseDatos,
                    Empresa = principal.Empresa,
                    Ejercicio = principal.Ejercicio,
                    Fkalmacen = principal.Fkalmacen
                };

            }
            else
            {
                throw new Exception("Usuario o contaseña incorrectos");
            }
        }

        private void GetParametrosConexion()
        {
            var request = System.Web.HttpContext.Current.Request;

            AuthenticateUser(request.Headers["Dominio"], request.Headers["Usuario"], request.Headers["Password"], Guid.Parse(request.Headers["IdConn"]));

        }

        public void Dispose()
        {
        }
    }
}