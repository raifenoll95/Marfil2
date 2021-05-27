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

    public class BusquedaArticulosLotesApiController : ApiBaseController
    {

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
            var flujocadena = nvc["Flujo"];
            var acabadodesde = nvc["FkAcabadoDesde"];
            var acabadohasta = nvc["FkAcabadoHasta"];
            var categoria = TipoCategoria.Ambas;
            if (!string.IsNullOrEmpty(flujocadena))
            {
                if (flujocadena == "0")
                    categoria = TipoCategoria.Ventas;
                else if (flujocadena == "1")
                    categoria = TipoCategoria.Compras;
            }
            if (Funciones.Qbool(solotablas))
            {
                var lote = nvc["Lote"];
                if (!string.IsNullOrEmpty(lote))
                {
                    lotedesde = lote;
                    lotehasta = lote;
                }
            }
            
            
            var service = new StockactualService(ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list = service.GetArticulosLotes(almacen, ContextService.Empresa, articulosdesde, articuloshasta, familiadesde, familiahasta, lotedesde, lotehasta, solotablas, categoria,acabadodesde,acabadohasta);

            var result = new ResultBusquedas<StockActualVistaModel>()
            {
                values = list,
                columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = false, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Fkalmacenes", displayName = "Almacén", visible = false,width=70 },
                        new ColumnDefinition() { field = "Fkarticulos", displayName = "Artículo", visible = true ,filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Fkfamilias", displayName = "Familia", visible = true,width=100},
                        new ColumnDefinition() { field = "Lote", displayName = "Lote", visible = true,width=100},
                        new ColumnDefinition() { field = "Loteid", displayName = "Lote Id", visible = true,width=70 },
                         new ColumnDefinition() { field = "Bundle", displayName = "Bundle", visible = true,width=150 },
                        new ColumnDefinition() { field = "Cantidad", displayName = "Cantidad", visible = true,width=70 },
                        new ColumnDefinition() { field = "Largo", displayName = "Largo", visible = true ,width=70 },
                        new ColumnDefinition() { field = "Ancho", displayName = "Ancho", visible = true,width=70 },
                        new ColumnDefinition() { field = "Grueso", displayName = "Grueso", visible = true,width=70 },
                        new ColumnDefinition() { field = "Metros", displayName = "Metros", visible = true,width=70 },
                       

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

            var service = new StockactualService(ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list= service.GetArticuloPorLoteOCodigo(id,almacen, ContextService.Empresa);

            var response = Request.CreateResponse(list == null ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                "application/json");
            return response;


        }

        public BusquedaArticulosLotesApiController(IContextService context) : base(context)
        {
        }
    }
}
