using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class ProvinciasPaisesApiController : ApiBaseController
    {
        public ProvinciasPaisesApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var codigopais = nvc["codigopais"];


            using (var service = FService.Instance.GetService(typeof(ProvinciasModel), ContextService) as ProvinciasService)
            {
                var result = new ResultBusquedas<ProvinciasModel>()
                {
                    values = service.GetProvinciasPais(codigopais),
                    columns = new[]
                   {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true },
                        new ColumnDefinition() { field = "Nombre", displayName = "Nombre", visible = true }
                    }
                };

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }


        // GET: api/ProvinciasPaisesApi/codigoPais
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get(string id)
        {                        
            using (var service = FService.Instance.GetService(typeof(ProvinciasModel), ContextService) as ProvinciasService)
            {
                var result = service.GetProvinciasPais(id);

                if (result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }


    }
}
