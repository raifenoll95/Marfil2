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
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class RecepcionStockApiController : ApiBaseController
    {
        public RecepcionStockApiController(IContextService context) : base(context)
        {
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {
            
            using (var service = FService.Instance.GetService(typeof(RecepcionesStockModel),ContextService) as RecepcionStockService)
            {

                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var almacenActual = nvc["Fkalmacen"];
                var list = service.GetAll<RecepcionesStockModel>().Where(f=>f.Fkalmacen== almacenActual);

                var result = new ResultBusquedas<RecepcionesStockModel>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Referencia", displayName = "Referencia", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Fechadocumentocadena", displayName = "Fecha", visible = true},
                        new ColumnDefinition() { field = "Fkproveedores", displayName = "Cód. Proveedor", visible = true},
                        new ColumnDefinition() { field = "Nombreproveedor", displayName = "Proveedor", visible = true},
                        new ColumnDefinition() { field = "Estadodescripcion", displayName = "Estado", visible = true},
                        new ColumnDefinition() { field = "Importebaseimponible", displayName = "Base", visible = true},
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
            
            using (var service = FService.Instance.GetService(typeof(RecepcionesStockModel),ContextService) as RecepcionStockService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var almacenActual = nvc["Fkalmacen"];
                
                var list = service.GetByReferencia(id);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (list.Fkalmacen == almacenActual)
                {
                   
                    response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                        "application/json");
                    return response;
                }

                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
                response.Content = new StringContent("Error", Encoding.UTF8);
                return response;


            }
        }
    }
}
