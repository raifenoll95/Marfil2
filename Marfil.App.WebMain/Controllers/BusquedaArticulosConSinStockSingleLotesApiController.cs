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
    public class BusquedaArticulosConSinStockSingleLotesApiController : ApiBaseController
    {
        public BusquedaArticulosConSinStockSingleLotesApiController(IContextService context) : base(context)
        {
        }
        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {


            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var almacen = nvc["Fkalmacen"];
            var articulosdesde = nvc["FkarticulosDesde"];
            var articuloshasta = nvc["FkarticulosHasta"];
            var familiadesde = nvc["FkfamiliaDesde"];
            var familiahasta = nvc["FkfamiliaHasta"];
            var lotedesde = nvc["LoteDesde"];
            var lotehasta = nvc["LoteHasta"];
            var solotablas = Funciones.Qbool(nvc["Solotablas"]);
            if (Funciones.Qbool(solotablas))
            {
                var lote = nvc["Lote"];
                if (!string.IsNullOrEmpty(lote))
                {
                    lotedesde = lote;
                    lotehasta = lote;
                }
            }


            var service = new StockactualService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            //Le pasamos la empresa, el almacen y el articulo del que queremos buscar
            var list = service.GetArticulosLotesAgrupadosHistorico(almacen, ContextService.Empresa, articulosdesde);

            var result = new ResultBusquedas<StockActualMobileModel>()
            {
                values = list,
                columns = new[]
                    {
                        new ColumnDefinition() { field = "Fkarticulos", displayName = "Artículo", visible = true ,filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Articulo", displayName = "Descripción", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Lote", displayName = "Lote", visible = true,width=100},
                        new ColumnDefinition() { field = "Familia", displayName = "Familia", visible = true,width=100},
                        new ColumnDefinition() { field = "Familiamaterial", displayName = "Familia M.", visible = true,width=100},
                        new ColumnDefinition() { field = "Material", displayName = "Material", visible = true,width=100},
                        new ColumnDefinition() { field = "Cantidaddisponible", displayName = "Cantidaddisponible", visible = true,width=70 },
                        new ColumnDefinition() { field = "SMetros", displayName = "SMetros", visible = true,width=150 },
                        new ColumnDefinition() { field = "UM", displayName = "UM", visible = true,width=70 },
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
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var almacen = nvc["Fkalmacen"];
            var articulosdesde = nvc["FkarticulosDesde"];
            var articuloshasta = nvc["FkarticulosHasta"];
            var familiadesde = nvc["FkfamiliaDesde"];
            var familiahasta = nvc["FkfamiliaHasta"];
            var lotedesde = nvc["LoteDesde"];
            var lotehasta = nvc["LoteHasta"];
            var solotablas = Funciones.Qbool(nvc["Solotablas"]);

            var service = new StockactualService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list = service.GetArticuloPorLoteEntradaSingleOCodigoHistorico(id, almacen, ContextService.Empresa);

            var response = Request.CreateResponse(list == null ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                "application/json");
            return response;


        }


    }
}

