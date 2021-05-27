using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
{
    public class ChangeCultureController : Controller
    {
        // GET: ChangeCulture
        public ActionResult Index(string id,string returnUrl)
        {
            var langCookie = new HttpCookie("lang", id)
            {
                Expires = DateTime.MaxValue
            };
            Response.AppendCookie(langCookie);
            return Redirect(returnUrl);
        }
    }
}