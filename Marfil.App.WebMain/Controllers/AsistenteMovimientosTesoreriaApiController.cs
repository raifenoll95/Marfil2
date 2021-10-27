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
    public class AsistenteMovimientosTesoreriaApiController : ApiBaseController
    {
        public AsistenteMovimientosTesoreriaApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
          
            using (var service = FService.Instance.GetService(typeof(VencimientosModel),ContextService) as VencimientosService)
            {
                    var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                    var tipoasignacion = nvc["Tipoasignacion"];
                    var circuitotesoreria =  nvc["Circuitotesoreria"];
                    var fkmodospago = nvc["Fkmodospago"];
                    var TerceroDesde = nvc["TerceroDesde"];
                    var TerceroHasta = nvc["TerceroHasta"];
                    var FechaDesde = nvc["FechaDesde"];
                    var FechaHasta = nvc["FechaHasta"];
                    var ReferenciaRemesa = nvc["ReferenciaRemesa"];

                var result = new ResultBusquedas<GridAsistenteMovimientosTesoreriaModel>()
                    {
                        values = service.getVencimientosMovimientosTesoreria(tipoasignacion, circuitotesoreria, fkmodospago, TerceroDesde, TerceroHasta, FechaDesde, FechaHasta, ReferenciaRemesa),
                        columns = new[]
                        {
                            new ColumnDefinition() { field = "Id", displayName = "Id", visible = false},
                            new ColumnDefinition() { field = "Referencia", displayName = "Referencia", visible = true,width=120},
                            new ColumnDefinition() { field = "Fkcuentas", displayName = "Cuenta", visible = true,width=120},
                            new ColumnDefinition() { field = "FkcuentasDescripcion", displayName = "Nombre", visible = true,width=250},
                            new ColumnDefinition() { field = "Traza", displayName = "Doc.", visible = true,width=150},
                            new ColumnDefinition() { field = "Fechavencimiento", displayName = "Fecha Vencimiento", visible = true,width = 180},
                            new ColumnDefinition() { field = "Importegiro", displayName = "Importe", visible = true,width = 100},
                            new ColumnDefinition() { field = "Situacion", displayName = "Situación", visible = true,width = 100},
                            new ColumnDefinition() { field = "FkcuentaTesoreria", displayName = "Cta. Tesorería", visible = true,width = 120},
                            new ColumnDefinition() { field = "Fkformaspago", displayName = "Forma de pago", visible = true,width = 200}
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
                var list = service.get(id) as GridAsistenteMovimientosTesoreriaModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }
    }
}
