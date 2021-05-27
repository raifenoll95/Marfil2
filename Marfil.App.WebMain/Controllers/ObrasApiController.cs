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
    public class ObrasApiController : ApiBaseController
    {
        public ObrasApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
          
            using (var service = FService.Instance.GetService(typeof(ObrasModel),ContextService))
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var fkentidad = nvc["cliente"];
                var list = service.getAll().Select(f => (ObrasModel) f);
                if (!string.IsNullOrEmpty(fkentidad))
                    list = list.Where(f => f.Fkclientes == fkentidad);

                var result = new ResultBusquedas<ObrasModel>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Fkclientes", displayName = "Cliente", visible = true },
                        new ColumnDefinition() { field = "Nombrecliente", displayName = "Nombre", visible = true },
                        new ColumnDefinition() { field = "Nombreobra", displayName = "Nombre de obra", visible = true },
                        new ColumnDefinition() { field = "Poblacion", displayName = "Población", visible = true },
                        new ColumnDefinition() { field = "Fechainiciocadena", displayName = "Fecha de inicio", visible = true },
                        new ColumnDefinition() { field = "Fechafincadena", displayName = "Fecha de fin", visible = true }

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
          
            using (var service = FService.Instance.GetService(typeof(ObrasModel),ContextService))
            {

                var list = service.get(id) as ObrasModel;
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var fkentidad = nvc["cliente"];
                if (!string.IsNullOrEmpty(fkentidad))
                {
                    if (list.Fkclientes != fkentidad)
                        return Request.CreateResponse(HttpStatusCode.BadRequest);
                }
                
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
               

            }
        }
    }
}
