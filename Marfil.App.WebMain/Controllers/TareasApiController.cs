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
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Stock;

namespace Marfil.App.WebMain.Controllers
{
    public class TareasApiController : ApiBaseController
    {
        public TareasApiController(IContextService context) : base(context)
        {
        }

        // GET: api/TareasApi
        public HttpResponseMessage Get()
        {

            using (var service = FService.Instance.GetService(typeof(TareasModel), ContextService) as TareasService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                var primeracarga = true;
                if (nvc[Constantes.Primeracarga] != null)
                    primeracarga = Funciones.Qbool(nvc[Constantes.Primeracarga]);
                //var tipocuenta = nvc["tipocuenta"];

                var result = new ResultBusquedas<TareasModel>()
                {
                    //values = service.GetCuentas((TiposCuentas)Funciones.Qint(tipocuenta).Value).Where(f => primeracarga),
                    values = service.getAllTareas(),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id Tarea", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true },
                    }
                };


                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }

        // GET: api/TareasApi/0001
        [System.Web.Mvc.Authorize]        
        public HttpResponseMessage Get(string id)
        {

            using (var service = FService.Instance.GetService(typeof(TareasModel), ContextService) as TareasService)
            {

                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var primeracarga = Funciones.Qbool(nvc[Constantes.Primeracarga]);

                var result = service.getTarea(id);                     
                if(result == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound);
                }
                var response = Request.CreateResponse(HttpStatusCode.OK);

                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;                                   
            }
        }
   
        // GET: api/TareasApi/0001/2010
        [System.Web.Mvc.Authorize]
        [Route("api/TareasApi/{id}/{año}")]
        public HttpResponseMessage Get(string id, string año)
        {

            using (var service = FService.Instance.GetService(typeof(TareasModel), ContextService) as TareasService)
            {

                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var primeracarga = Funciones.Qbool(nvc[Constantes.Primeracarga]);

                var result = service.getPrecio(id, año);

                var response = Request.CreateResponse(HttpStatusCode.OK);

                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }

    }
}