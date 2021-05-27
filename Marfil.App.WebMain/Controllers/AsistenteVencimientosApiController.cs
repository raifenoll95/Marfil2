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
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;

namespace Marfil.App.WebMain.Controllers
{
    public class AsistenteVencimientosApiController : ApiBaseController
    {
        public AsistenteVencimientosApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
          
            using (var service = FService.Instance.GetService(typeof(VencimientosModel),ContextService) as VencimientosService)
            {
                    var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    var tipoasignacion = nvc["Tipoasignacion"];
                    var circuitotesoreria =  nvc["Circuitotesoreria"];
                    var fkcuentas = nvc["Fkcuentas"];
                    var importe = nvc["Importe"];

                    var result = new ResultBusquedas<VencimientosModel>()
                    {
                        values = service.getVencimientos(tipoasignacion, circuitotesoreria, fkcuentas, importe),
                        columns = new[]
                        {
                            new ColumnDefinition() { field = "Id", displayName = "Id", visible = false},
                            new ColumnDefinition() { field = "Referencia", displayName = "Referencia", visible = true ,width=150},
                            new ColumnDefinition() { field = "Traza", displayName = "Doc.", visible = true ,width=150},
                            new ColumnDefinition() { field = "FechaStrfactura", displayName = "Fecha Factura", visible = true,width=200},
                            new ColumnDefinition() { field = "FechaStrvencimiento", displayName = "Fecha Vencimiento", visible = true,width = 200},
                            new ColumnDefinition() { field = "Importegiro", displayName = "Importe", visible = true,width = 100},
                            new ColumnDefinition() { field = "FormaPago", displayName = "Forma de pago", visible = true,width = 450}
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
            using (var service = FService.Instance.GetService(typeof(VencimientosModel),ContextService) as VencimientosService)
            { 
                var list = service.get(id) as VencimientosModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }
    }
}
