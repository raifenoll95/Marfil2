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
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Newtonsoft.Json;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class BusquedaArticulosLotesRemedirApiController : ApiBaseController
    {
        public BusquedaArticulosLotesRemedirApiController(IContextService context) : base(context)
        {
        }
        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {

            
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var fkalmacen = nvc["Fkalmacen"];
            var fkproveedores = nvc["Fkproveedores"];
            var desderecepcionstock = nvc["Desderecepcionstock"];
            var hastarecepcionstock = nvc["Hastarecepcionstock"];
            var fechadesde = nvc["Fechadesde"];
            var fechahasta = nvc["Fechahasta"];
            var lotedesde = nvc["Desdelote"];
            var lotehasta = nvc["Hastalote"];
            
            var service = new StockactualService(ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list = service.GetArticulosLotesRemedir(fkalmacen, ContextService.Empresa, fkproveedores, desderecepcionstock, hastarecepcionstock, fechadesde, fechahasta, lotedesde, lotehasta);

            var result = new ResultBusquedas<StockActualRemedirModel>()
            {
                values = list,
                columns = new[]
                    {
                        new ColumnDefinition() { field = "Fkalmacenes", displayName = "Almacén", visible = false,width=70 },
                        new ColumnDefinition() { field = "Fkarticulos", displayName = "Artículo", visible = true ,filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH },width=100},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH },width=200},
                        new ColumnDefinition() { field = "Lote", displayName = "Lote", visible = true,width=100},
                        new ColumnDefinition() { field = "Loteid", displayName = "Lote Id", visible = true,width=70 },
                        new ColumnDefinition() { field = "Bundle", displayName = "Bundle", visible = true,width=70 },
                        new ColumnDefinition() { field = "Cantidaddisponible", displayName = "Cantidad", visible = true,width=70 },
                        new ColumnDefinition() { field = "SLargo", displayName = "Largo", visible = true ,width=70 },
                        new ColumnDefinition() { field = "SAncho", displayName = "Ancho", visible = true,width=70 },
                        new ColumnDefinition() { field = "SGrueso", displayName = "Grueso", visible = true,width=70 },
                        new ColumnDefinition() { field = "SMetros", displayName = "Metros", visible = true,width=70 },
                        new ColumnDefinition() { field = "UM", displayName = "UM", visible = true,width=70 },
                        new ColumnDefinition() { field = "Cc", displayName = "C. comercial", visible = true,width=70 },
                        new ColumnDefinition() { field = "Tono", displayName = "Tono", visible = true,width=70 },
                        new ColumnDefinition() { field = "Grano", displayName = "Grano", visible = true,width=70 },
                        new ColumnDefinition() { field = "Zona", displayName = "Zona", visible = true,width=70 },
                        new ColumnDefinition() { field = "Loteproveedor", displayName = "Lote proveedor", visible = true,width=120 },


                    }
            };

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8,
                "application/json");
            return response;



        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get(string id)
        {
            


            var service = FService.Instance.GetService(typeof (ArticulosModel), ContextService) as ArticulosService;
            var list = service.GetArticulo(id);
            

            var response = Request.CreateResponse(list == null ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                "application/json");
            return response;


        }

       
    }
}
