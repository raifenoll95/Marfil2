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
                IEnumerable<CuentasModel> list = GetListaCuentas(service);

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
                id = id.TrimStart('0');//Por si hay ceros a la izq.
                IEnumerable<CuentasModel> cuentas = GetListaCuentas(service);
                var list = service.get(id);

                if (!cuentas.Any(f => f.Id == id))
                {
                    throw new ValidationException("La cuenta " + id + " no es válida");
                }
      
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;

            }
        }

        private IEnumerable<CuentasModel> GetListaCuentas(CuentasService service)
        {
            var nivel = HttpUtility.ParseQueryString(Request.RequestUri.Query)["nivel"];
            int intnivel = 0;
            if (!string.IsNullOrEmpty(nivel))
                intnivel = int.Parse(nivel);

            var list = service.GetCuentasContablesNivel(intnivel);

            //Quitamos cuentas que realmente no están habilitadas como tercero
            //list = list.Where(f => f.Descripcion2 != null);

            var tipofacturaiva = HttpUtility.ParseQueryString(Request.RequestUri.Query)["tipofacturaiva"];
            var inttipofacturaiva = 2;
            if (!string.IsNullOrEmpty(tipofacturaiva))
            {
                inttipofacturaiva = int.Parse(tipofacturaiva);
            }

            var cuenta = HttpUtility.ParseQueryString(Request.RequestUri.Query)["cuenta"];
            var idtipofactura = HttpUtility.ParseQueryString(Request.RequestUri.Query)["idfacturaiva"];
            var regimeniva = HttpUtility.ParseQueryString(Request.RequestUri.Query)["regimeniva"];
          
            if (inttipofacturaiva == 0)//Soportado
            {
                if (cuenta == "cliente")
                {
                    var cargo = service.GetCuentaCargo1(inttipofacturaiva, idtipofactura);
                    list = list.Where(f => f.Id.StartsWith(cargo));

                }
                else if (cuenta == "ventas")
                {
                    var abono = service.GetCuentaAbono1(inttipofacturaiva, idtipofactura);
                    list = list.Where(f => f.Id.StartsWith(abono));
                }
            }
            else if (inttipofacturaiva == 1)//Repercutido
            {
                if (cuenta == "cliente")
                {
                    var cargo = service.GetCuentaCargo1(inttipofacturaiva, idtipofactura);
                    list = list.Where(f => f.Id.StartsWith(cargo));
                }
                else if (cuenta == "ventas")
                {
                    var abono = service.GetCuentaAbono1(inttipofacturaiva, idtipofactura);
                    list = list.Where(f => f.Id.StartsWith(abono));
                }
            }

            var listaRemove = new List<CuentasModel>();
            var listaFinal = new List<CuentasModel>();
            var lista = list.ToList();

            if (!string.IsNullOrEmpty(regimeniva))
            {
                foreach (var item in list)
                {
                    var serviceRetenciones = new TiposRetencionesService(ContextService, MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
                    var regimentercero = serviceRetenciones.GetRegimenivaTercero(item.Id);
                    var tipofacturatercero = serviceRetenciones.GetTipoFacturaCliente(item.Id);

                    if (regimentercero != regimeniva || tipofacturatercero != idtipofactura)
                    {
                        lista.RemoveAll(f => f.Id == item.Id);
                    }
                }
            }



            return lista;
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
