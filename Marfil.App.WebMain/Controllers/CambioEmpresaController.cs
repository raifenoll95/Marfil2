using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Login;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class CambioEmpresaController :Controller
    {
        #region Members

        private readonly ILoginService _serviceLogin;
        private readonly IContextService _contextService;
        private readonly string _dominio;
        #endregion

        #region CTR

        public CambioEmpresaController(ILoginService service,IContextService context)
        {
            _serviceLogin = service;
            _contextService = context;
            _dominio = System.Web.HttpContext.Current.Request.Url.DnsSafeHost;
        }

        #endregion


        //Rai
        #region Cambiar Empresa
        public ActionResult Index(string id)
        {
            HttpCookie cookie;
            _serviceLogin.SetEmpresaUser(_dominio, _contextService.BaseDatos, _contextService.Usuario, id, string.Empty, string.Empty, _contextService.Idconexion, out cookie);
            Response.Cookies.Add(cookie);
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}