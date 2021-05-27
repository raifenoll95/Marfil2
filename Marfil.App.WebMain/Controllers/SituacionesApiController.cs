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

namespace Marfil.App.WebMain.Controllers
{
    public class SituacionesApiController : ApiBaseController
    {
        public SituacionesApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var situaciones = nvc["situaciones"];

            using (var service = new SituacionesTesoreriaService(ContextService))
            {
                var listsituaciones = appService.GetListSituaciones();
                var result = new ResultBusquedas<SituacionesTesoreriaModel>()
                {
                    values = !String.IsNullOrEmpty(situaciones) ? service.getAll().Select(f => (SituacionesTesoreriaModel)f).ToList().Where(f => f.Valorinicialcobros == false && f.Valorinicialpagos == false) :
                        service.getAll().Select(f=>(SituacionesTesoreriaModel)f).ToList(),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Cod", displayName = "Cod", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
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
            
            using (var service = new SituacionesTesoreriaService(ContextService))
            {
                var list = service.get(id) as SituacionesTesoreriaModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
               

            }
        }
    }
}
