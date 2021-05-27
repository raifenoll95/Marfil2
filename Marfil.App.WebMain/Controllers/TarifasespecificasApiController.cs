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
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class TarifasespecificasApiController : ApiBaseController
    {
        public TarifasespecificasApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
            
            using (var service = FService.Instance.GetService(typeof(ArticulosModel),ContextService) as ArticulosService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var tipoflujo = nvc["tipoflujo"];
                var articulo = nvc["articulo"];


                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(service.GetTarifas((TipoFlujo)Enum.Parse(typeof(TipoFlujo), tipoflujo), articulo)), Encoding.UTF8, "application/json");
                return response;
            }
        }

       
    }
}
