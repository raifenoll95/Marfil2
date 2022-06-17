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
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;

namespace Marfil.App.WebMain.Controllers
{
    public class CircuitosTesoreriaApiController : ApiBaseController
    {
        public CircuitosTesoreriaApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var tipoasignacion = nvc["tipoasignacion"];
            var soloinicialescobropago = nvc["soloinicialescobropago"];

            using (var service = new CircuitosTesoreriaCobrosService(ContextService))
            {
                var listsituaciones = appService.GetListSituaciones();
                var result = new ResultBusquedas<CircuitoTesoreriaCobrosModel>()
                {
                    values = soloinicialescobropago == "1" ? service.GetCircuitosTesoreria(tipoasignacion, true) : service.GetCircuitosTesoreria(tipoasignacion, false),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripcion", visible = true }
                    }
                };

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get(string id)
        {
            
            using (var service = new CircuitosTesoreriaCobrosService(ContextService))
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var tipoasignacion = nvc["tipoasignacion"];
                var soloinicialescobropago = nvc["soloinicialescobropago"];
                var idparse = int.Parse(id);

                //var list = service.get(id) as CircuitoTesoreriaCobrosModel;
                var registros = soloinicialescobropago == "1" ? service.GetCircuitosTesoreria(tipoasignacion, true).ToList() : service.GetCircuitosTesoreria(tipoasignacion, false).ToList();
                var list = registros.Single(x => x.Id == idparse);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
               

            }
        }
    }
}
