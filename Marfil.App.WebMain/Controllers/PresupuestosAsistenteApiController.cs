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
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;

namespace Marfil.App.WebMain.Controllers
{
    public class PresupuestosAsistenteApiController : ApiBaseController
    {
        public PresupuestosAsistenteApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
          
            using (var service = FService.Instance.GetService(typeof(PresupuestosModel), ContextService) as PresupuestosService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var id = nvc["PresupuestoId"];
                var articulos = service.articulosComponentes(id);

                    var result = new ResultBusquedas<PresupuestosLinModel>()
                    {
                        values = articulos,
                        columns = new[]
                        {               
                            new ColumnDefinition() { field = "Fkarticulos", displayName = "Cód. Artículo", visible = true ,width=150},
                            new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true ,width=320},
                            new ColumnDefinition() { field = "Id", displayName = "Artículo", visible = true, width=100},
                            new ColumnDefinition() { field = "Cantidad", displayName = "Cantidad", visible = true ,width=100},
                            new ColumnDefinition() { field = "Lote", displayName = "Lote", visible = true ,width=100},
                            new ColumnDefinition() { field = "SLargo", displayName = "Largo", visible = true ,width=100},
                            new ColumnDefinition() { field = "SAncho", displayName = "Ancho", visible = true ,width=100},
                            new ColumnDefinition() { field = "SGrueso", displayName = "Grueso", visible = true ,width=100},
                            new ColumnDefinition() { field = "SMetros", displayName = "Metros", visible = true ,width=100},
                            new ColumnDefinition() { field = "SPrecio", displayName = "Precio", visible = true ,width=100},
                            new ColumnDefinition() { field = "Integridadreferenciaflag", displayName = "Integridadreferenciaflag", visible = false, width=1}
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
            using (var service = FService.Instance.GetService(typeof(PresupuestosLinModel),ContextService) as PresupuestosService)
            { 
                var list = service.get(id) as PresupuestosLinModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }
    }
}
