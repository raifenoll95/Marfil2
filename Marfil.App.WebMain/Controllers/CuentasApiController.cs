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
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;



namespace Marfil.App.WebMain.Controllers
{
    //public class StCuentas : BaseParam, ICuentasFiltros
    //{
    //    public string Id { get; set; }
    //    public string Filtros { get; set; }
    //    public string Pagina { get; set; }
    //    public string RegistrosPagina { get; set; }
    //    public string Tipocuenta { get; set; }
    //}

    //public class ErrorJson
    //{
    //    public string Error { get; set; }

    //    public ErrorJson(string error)
    //    {
    //        Error = error;
    //    }
    //}


    public class CuentasApiController :  ApiBaseController // BasicAuthHttpModule

        
        //public class TiposcuentasController : GenericController<TiposCuentasModel>
    {
        public CuentasApiController(IContextService context) : base(context)
        {
        }

        // GET: api/Supercuentas/5
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get()
        {

            using (var service = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService)
            {
                var nivel = HttpUtility.ParseQueryString(Request.RequestUri.Query)["nivel"];
                int intnivel = 0;
                if (!string.IsNullOrEmpty(nivel))
                    intnivel = int.Parse(nivel); 
                var list = service.GetCuentasContablesNivel(intnivel);

                var result = new ResultBusquedas<CuentasModel>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Cuenta", visible = true,
                            filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }, width=150},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripcion", visible = true,
                            filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH } }
                        
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

            using (var service = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService)
            {

                var list = service.get(id);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;

            }
        }




        //public CuentasApiController(ILoginService service) : base(service)
        //{
        //}

        //[System.Web.Mvc.HttpPost]
        //public ActionResult Buscar(StCuentas model)
        //{
        //    using (var service = FService.Instance.GetService(typeof(CuentasModel), ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as CuentasService)
        //    {

        //        int registrosTotales = 0;
        //        IEnumerable<CuentasBusqueda> items = null;
        //        try
        //        {

        //            items = service.GetCuentas(model, out registrosTotales);
        //        }
        //        catch (Exception ex)
        //        {
        //            return Json(new ErrorJson(ex.Message));
        //        }

        //        var result = new ResultBusquedasPaginados<CuentasBusqueda>()
        //        {
        //            values = items,
        //            columns = new[]
        //            {
        //                new ColumnDefinition() { field = "Fkcuentas", displayName = "Cuentas", visible = true},
        //                new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true },
        //                new ColumnDefinition() { field = "Razonsocial", displayName = "Razón", visible = true },
        //                new ColumnDefinition() { field = "Nif", displayName = "Nif", visible = true },
        //                new ColumnDefinition() { field = "Pais", displayName = "País", visible = true },
        //                new ColumnDefinition() { field = "Provincia", displayName = "Provincia", visible = true },
        //                new ColumnDefinition() { field = "Poblacion", displayName = "Población", visible = true },
        //            },
        //            RegistrosTotales = registrosTotales,
        //            PaginaActual = Funciones.Qint(model.Pagina) ?? 1,
        //        };



        //        return Json(result);
        //    }
        //}

    }
}
