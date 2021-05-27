using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
{
    public class ScriptsController : Controller
    {
        // GET: Scripts
        public ActionResult campoverificacion(string id,string api)
        {
            Response.ContentType = "text/javascript";
            ViewBag.id = id ?? "";
            ViewBag.Api = api ?? "";
            
            return View();
        }
    }
}