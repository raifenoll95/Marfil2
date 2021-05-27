using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class StArticulosLotes
    {
        public string Referencialote { get; set; }
        public string Fkalmacen { get; set; }
    }
    public class ArticulosLotesApiController : ApiBaseController
    {
        public ArticulosLotesApiController(IContextService context) : base(context)
        {
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {
            
           
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var almacen = nvc["Fkalmacen"];
            var referencialote = nvc["Referencialote"];
            var service = new StockactualService(ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list = service.GetArticuloPorLoteOCodigo(referencialote, almacen, ContextService.Empresa);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                "application/json");
            return response;
               

            
        }

       
    }
}
