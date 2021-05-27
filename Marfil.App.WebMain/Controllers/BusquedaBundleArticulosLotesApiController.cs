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

    public class BusquedaBundleArticulosLotesApiController : ApiBaseController
    {
        public BusquedaBundleArticulosLotesApiController(IContextService context) : base(context)
        {
        }
        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {

            
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var almacen = nvc["Fkalmacen"];
            
        
           
          
            
            
            var service = new StockactualService(ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list = service.GetBundleArticulosLotes(almacen, ContextService.Empresa);

            var result = new ResultBusquedas<StockActualVistaModel>()
            {
                values = list,
                columns = new[]
                    {
                        new ColumnDefinition() { field = "Lote", displayName = "Lote", visible = true,width=150},
                        new ColumnDefinition() { field = "Fkalmacenes", displayName = "Almacén", visible = false,width=70 },
                        new ColumnDefinition() { field = "Fkarticulos", displayName = "Artículo", visible = true ,filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                       
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
            var solotablas = Funciones.Qbool(nvc["Solotablas"]);

            var service = new StockactualService(ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list= service.GetArticuloLote(almacen, ContextService.Empresa, "", "", "", "", "", "", solotablas,id);

            var response = Request.CreateResponse(list == null ? HttpStatusCode.InternalServerError : HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                "application/json");
            return response;


        }

       
    }
}
