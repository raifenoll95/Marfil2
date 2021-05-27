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
using System.Web.UI.WebControls;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class StMovimientosalmacen
    {
        public string Lote { get; set; }
        public string Fkalmacen { get; set; }
        public string Fkzonalmacen { get; set; }
    }

    
    public class MobileMovimientoalmacenApiController : BasicAuthHttpModule
    {
        

        #region Members

        
        #endregion

        #region CTR

        public MobileMovimientoalmacenApiController(ILoginService service) : base(service)
        {
            
        }

        #endregion

        [System.Web.Mvc.HttpPost]
        public ActionResult MovimientosAlmacen(StMovimientosalmacen model)
        {
            var service= new MovimientosalmacenService(ContextService);
            service.GenerarMovimientoAlmacen(model.Lote, model.Fkalmacen, model.Fkzonalmacen);
            return Content("OK");
        }

    }
}
