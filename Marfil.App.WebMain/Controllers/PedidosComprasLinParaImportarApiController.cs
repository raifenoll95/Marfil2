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
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;

namespace Marfil.App.WebMain.Controllers
{
    public class PedidosComprasLinParaImportarApiController : ApiBaseController
    {
        public PedidosComprasLinParaImportarApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
           
            using (var service = FService.Instance.GetService(typeof(PedidosComprasModel),ContextService) as PedidosComprasService)
            {
                using (var seriesService = FService.Instance.GetService(typeof (SeriesModel), ContextService) as SeriesService)
                {
                    var serieslist = seriesService.GetSeriesTipoDocumento(TipoDocumento.PedidosCompras).Select(f=>new ComboListItem() {label =f.Id,value=f.Id}).ToList();
                

                    var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var cliente =  nvc["proveedor"];
                var Pedidosdesde = nvc["PedidosDesde"];
                var Pedidoshasta = nvc["PedidosHasta"];

                var result = new ResultBusquedas<PedidosComprasLinImportarModel>()
                {
                    values = service.BuscarLineasPedidosCompras(cliente, Pedidosdesde, Pedidoshasta),
                    columns = new[]
                    {
                         
                        new ColumnDefinition() { field = "FkPedidosComprasreferencia", displayName = "Pedido", visible = true ,width=150},
                        new ColumnDefinition() { field = "Fkarticulos", displayName = "Cod. Artículo", visible = true,width=150 },
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true,width = 200},
                         new ColumnDefinition() { field = "Cantidad", displayName = "Cantidad", visible = true,width = 100,enableCellEdit = true, type = "number"},
                        new ColumnDefinition() { field = "Lote", displayName = "Lote", visible = true,width = 200, enableCellEdit = true },
                        new ColumnDefinition() { field = "Caja", displayName = "Caja", visible = true,width = 100, enableCellEdit = true ,type = "number"},
                        new ColumnDefinition() { field = "SLargo", displayName = "Largo", visible = true,width = 100 },
                        new ColumnDefinition() { field = "SAncho", displayName = "Ancho", visible = true,width = 100 },
                        new ColumnDefinition() { field = "SGrueso", displayName = "Grueso", visible = true,width = 100 },
                        new ColumnDefinition() { field = "SMetros", displayName = "Metros", visible = true,width = 100 },
                        new ColumnDefinition() { field = "SPrecio", displayName = "Precio", visible = true ,width = 100},
                        new ColumnDefinition() { field = "Porcetanjedescuento", displayName = "%  Desc.", visible = true ,width = 100},
                        new ColumnDefinition() { field = "SImporte", displayName = "Subtotal", visible = true ,width = 100}

                    }
                };

                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                    return response;
                }
            }
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get(string id)
        {
          
            using (var service = FService.Instance.GetService(typeof(PedidosComprasModel),ContextService) as PedidosComprasService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var cliente = nvc["proveedor"];
                var list = service.GetByReferencia(id);

                if (!string.IsNullOrEmpty(cliente) && list.Fkproveedores != cliente)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }
    }
}
