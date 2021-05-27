using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
{
    public class BancosMandatosController : Controller
    {
        [ChildActionOnly]
        public ActionResult BancosMandatos(BancosMandatosModel model)
        {
            return PartialView("BancosMandatos", model);
        }
    }
}