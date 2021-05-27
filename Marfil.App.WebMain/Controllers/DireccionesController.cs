using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.App.WebMain.Controllers
{
    public class DireccionesController : Controller
    {
        
        [ChildActionOnly]
        public ActionResult Direcciones(DireccionesModel model)
        {
            return PartialView("Direcciones",model);
        }
    }
}