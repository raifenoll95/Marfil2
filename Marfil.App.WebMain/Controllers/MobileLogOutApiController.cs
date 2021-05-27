using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
{
    public class MobileLogOutApiController : BasicAuthHttpModule
    {
        private readonly ILoginService _service;
        #region CTR

        public MobileLogOutApiController(ILoginService service) : base(service)
        {
            _service = service;
        }

        #endregion

        [System.Web.Mvc.HttpPost]
        public ActionResult Logout()
        {
            _service.Logout((ICustomPrincipal)System.Web.HttpContext.Current.User);

            return Content("OK");
        }
    }
}