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
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    
    public class MobileArticulosLotesApiController : BasicAuthHttpModule
    {
        public MobileArticulosLotesApiController(ILoginService service) : base(service)
        {
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult MovimientosAlmacen(StArticulosLotes model)
        {
            var almacen = model.Fkalmacen;
            var referencialote = model.Referencialote;
            var service = new StockactualService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list = service.GetArticuloPorLoteOCodigo(referencialote, almacen, ContextService.Empresa);
            return Json(list);
        }

       
    }
}
