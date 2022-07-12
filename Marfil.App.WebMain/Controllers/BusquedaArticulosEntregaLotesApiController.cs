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
    public class BusquedaArticulosEntregaLotesApiController : ApiBaseController
    {
        public BusquedaArticulosEntregaLotesApiController(IContextService context) : base(context)
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
            var acabadodesde = nvc["FkAcabadoDesde"];
            var acabadohasta = nvc["FkAcabadoHasta"];
            var lotedesde = nvc["LoteDesde"];
            var lotehasta = nvc["LoteHasta"];
            var flujocadena = nvc["Flujo"];
            var categoria = TipoCategoria.Ambas;
            if (!string.IsNullOrEmpty(flujocadena))
            {
                if (flujocadena == "0")
                    categoria = TipoCategoria.Ventas;
                else if (flujocadena == "1")
                    categoria = TipoCategoria.Compras;
            }
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
            
            
            var service = new StockactualService(ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list = service.GetArticulosLotes(almacen, ContextService.Empresa, articulosdesde, articuloshasta, familiadesde, familiahasta, lotedesde, lotehasta, solotablas, categoria,acabadodesde,acabadohasta);

            var result = new ResultBusquedas<StockActualVistaModel>()
            {
                values = list,
                columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = false, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH, }},
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
            var lotedesde = id;
            var lotehasta = id;
            //var categoria = TipoCategoria.Ventas;
            var solotablas = Funciones.Qbool(nvc["Solotablas"]);

            var service = new StockactualService(ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list= service.GetArticuloEntradaPorLoteOCodigo(id,almacen, ContextService.Empresa).Where(f=>((SalidabusquedaentregaslotesarticulosModel)f).Cantidad>0).Select(f=>(SalidabusquedaentregaslotesarticulosModel)f);
            //var list = service.GetArticulosLotes(almacen, ContextService.Empresa, articulosdesde, articuloshasta, familiadesde, familiahasta, lotedesde, lotehasta, solotablas, categoria, null, null);
            var result = new ResultBusquedas<SalidabusquedaentregaslotesarticulosModel>()
            {
                values = list,
                columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = false, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Fkalmacenes", displayName = "Almacén", visible = false,width=70 },
                        new ColumnDefinition() { field = "Fkarticulos", displayName = "Artículo", visible = true ,filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Lote", displayName = "Lote", visible = true,width=100},
                        new ColumnDefinition() { field = "Loteid", displayName = "Lote Id", visible = true,width=70 },
                        new ColumnDefinition() { field = "Bundle", displayName = "Bundle", visible = true,width=150 },
                        new ColumnDefinition() { field = "Cantidad", displayName = "Cantidad", visible = true,width=70 },
                        new ColumnDefinition() { field = "SLargo", displayName = "Largo", visible = true ,width=70 },
                        new ColumnDefinition() { field = "SAncho", displayName = "Ancho", visible = true,width=70 },
                        new ColumnDefinition() { field = "SGrueso", displayName = "Grueso", visible = true,width=70 },
                        new ColumnDefinition() { field = "SMetros", displayName = "Metros", visible = true,width=70 },


                    }
            };
            var response = Request.CreateResponse(list == null ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8,
                "application/json");
            return response;


        }

        [Route("api/BusquedaArticulosEntregaLotesApi/GetHitorico")]
        public HttpResponseMessage GetHitorico()
        {

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var almacen = nvc["Fkalmacen"];
            var articulosdesde = nvc["FkarticulosDesde"];
            var articuloshasta = nvc["FkarticulosHasta"];
            var familiadesde = nvc["FkfamiliaDesde"];
            var familiahasta = nvc["FkfamiliaHasta"];
            var acabadodesde = nvc["FkAcabadoDesde"];
            var acabadohasta = nvc["FkAcabadoHasta"];
            var lotedesde = nvc["Id"];
            var lotehasta = nvc["Id"];
            var flujocadena = nvc["Flujo"];
            var categoria = TipoCategoria.Ambas;
            if (!string.IsNullOrEmpty(flujocadena))
            {
                if (flujocadena == "0")
                    categoria = TipoCategoria.Ventas;
                else if (flujocadena == "1")
                    categoria = TipoCategoria.Compras;
            }
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
            var list = service.GetArticulosLotesHistorico(almacen, ContextService.Empresa, articulosdesde, articuloshasta, familiadesde, familiahasta, lotedesde, lotehasta, solotablas, categoria, acabadodesde, acabadohasta);

            var result = new ResultBusquedas<StockActualVistaModelHistorico>()
            {
                values = list,
                columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = false, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH, }},
                        new ColumnDefinition() { field = "Fkalmacenes", displayName = "Almacén", visible = false,width=70 },
                        new ColumnDefinition() { field = "Fkarticulos", displayName = "Artículo", visible = true ,filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Fkfamilias", displayName = "Familia", visible = true,width=100},
                        new ColumnDefinition() { field = "Lote", displayName = "Lote", visible = true,width=100},
                        new ColumnDefinition() { field = "Loteid", displayName = "Lote Id", visible = true,width=70 },
                        new ColumnDefinition() { field = "Bundle", displayName = "Bundle", visible = true,width=150 },
                        new ColumnDefinition() { field = "Cantidadentrada", displayName = "Cantidad", visible = true,width=70 },
                        new ColumnDefinition() { field = "Largoentrada", displayName = "Largo", visible = true ,width=70 },
                        new ColumnDefinition() { field = "Anchoentrada", displayName = "Ancho", visible = true,width=70 },
                        new ColumnDefinition() { field = "Gruesoentrada", displayName = "Grueso", visible = true,width=70 },
                        new ColumnDefinition() { field = "MetrosEntrada", displayName = "Metros", visible = true,width=70 },


                    }
            };

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8,
                "application/json");
            return response;



        }

        [Route("api/BusquedaArticulosEntregaLotesApi/GetArticulosLotes")]
        public HttpResponseMessage GetArticulosLotes()
        {


            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var almacen = nvc["Fkalmacen"];
            var articulosdesde = nvc["FkarticulosDesde"];
            var articuloshasta = nvc["FkarticulosHasta"];
            var familiadesde = nvc["FkfamiliaDesde"];
            var familiahasta = nvc["FkfamiliaHasta"];
            var acabadodesde = nvc["FkAcabadoDesde"];
            var acabadohasta = nvc["FkAcabadoHasta"];
            var lotedesde = nvc["Id"];
            var lotehasta = nvc["Id"];
            var flujocadena = nvc["Flujo"];
            var categoria = TipoCategoria.Ambas;
            if (!string.IsNullOrEmpty(flujocadena))
            {
                if (flujocadena == "0")
                    categoria = TipoCategoria.Ventas;
                else if (flujocadena == "1")
                    categoria = TipoCategoria.Compras;
            }
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
            var list = service.GetArticulosLotes(almacen, ContextService.Empresa, articulosdesde, articuloshasta, familiadesde, familiahasta, lotedesde, lotehasta, solotablas, categoria, acabadodesde, acabadohasta);

            var result = new ResultBusquedas<StockActualVistaModel>()
            {
                values = list,
                columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = false, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH, }},
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

    }
}
