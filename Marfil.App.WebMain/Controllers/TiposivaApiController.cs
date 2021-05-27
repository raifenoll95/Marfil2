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
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class TiposivaApiController : ApiBaseController
    {
        public TiposivaApiController(IContextService context) : base(context)
        {
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {
            
            using (var service = FService.Instance.GetService(typeof(TiposIvaModel),ContextService) as TiposivaService)
            {

                var list = service.GetAll<TiposIvaModel>().ToList();

                var result = new ResultBusquedas<TiposIvaModel>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true,width=70},
                        new ColumnDefinition() { field = "Nombre", displayName = "Nombre", visible = true},
                        new ColumnDefinition() { field = "PorcentajeIva", displayName = "%", visible = true,width=70},
                        new ColumnDefinition() { field = "PorcentajeRecargoEquivalencia", displayName = "% R.E.", visible = true,width=70 },
                    }
                };

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8,
                    "application/json");
                return response;

            }
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get(string id)
        {
            
            using (var service = FService.Instance.GetService(typeof(TiposIvaModel),ContextService))
            {

                var list = service.get(id);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;

            }
        }
    }
}
