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
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.App.WebMain.Controllers
{
    public class MonedasApiController : ApiBaseController
    {
        public MonedasApiController(IContextService context) : base(context)
        {
        }

        // GET: api/MonedasApi
        public HttpResponseMessage Get()
        {
           
           
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                bool mostrarTodas;
                bool.TryParse(nvc["todas"], out mostrarTodas);
                
                using (var service = FService.Instance.GetService(typeof (MonedasModel), ContextService))
                {
                    var result = new ResultBusquedas<MonedasModel>()
                    {
                        values = mostrarTodas? service.getAll().Select(f => (MonedasModel)f) : service.getAll().Where(f=> ((MonedasModel)f).Activado).Select(f=>(MonedasModel)f),
                        columns = new[]
                       {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true },
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true },
                        new ColumnDefinition() { field = "Abreviatura", displayName = "Abraviatura", visible = true }
                    }
                    };

                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                    return response;
                }
            
        }

        // GET: api/MonedasApi/5
        public HttpResponseMessage Get(string id)
        {
           
            
            using (var service = FService.Instance.GetService(typeof(MonedasModel),ContextService))
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                bool mostrarTodas;
                bool.TryParse(nvc["todas"], out mostrarTodas);
                var list = service.get(id) as MonedasModel;
                if (list != null && (mostrarTodas || list.Activado))
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                        "application/json");
                    return response;
                }
                else
                {
                    var response = Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                    return response;
                }
            }
        }
    }
}
