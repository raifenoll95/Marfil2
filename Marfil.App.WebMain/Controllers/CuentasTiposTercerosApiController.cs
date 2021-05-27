using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;

namespace Marfil.App.WebMain.Controllers
{
    public class CuentasTiposTercerosApiController : ApiBaseController
    {
        // GET: api/CuentasTiposTerceros
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get()
        {
           
            using (var service = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var tipocuenta = nvc["tipocuenta"];
                IEnumerable<CuentasBusqueda> items = null;
                try
                {
                    items =service.GetCuentas((TiposCuentas)Funciones.Qint(tipocuenta).Value); // service.GetCuentasClientes((TiposCuentas)Funciones.Qint(tipocuenta).Value) ;
                }
                catch (Exception)
                {
                    
                    
                }

                
                var result = new ResultBusquedas<CuentasBusqueda>()
                {
                    values = items ?? Enumerable.Empty<CuentasBusqueda>(),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Fkcuentas", displayName = "Cuentas", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true },
                    }
                };


                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }

        // GET: api/CuentasClienteApi/5
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get(string id)
        {
            
            using (var service = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var tipocuenta = nvc["tipocuenta"];

                if (Regex.IsMatch(id, ".*\\.\\*"))
                {
                    //get nuevo index
                    var fmodel = new FModel();

                    var vector = id.Split('.');
                    var subcuenta = vector[0];
                    var maxlevel = appService.NivelesCuentas(ContextService.Empresa);
                    subcuenta = subcuenta.PadRight(maxlevel, '0');
                    var nextid = service.GetNextCuenta(ContextService.Empresa, subcuenta);
                    var newCuenta = fmodel.GetModel<CuentasModel>(ContextService);
                    newCuenta.Id = nextid;
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(newCuenta), Encoding.UTF8,
                        "application/json");
                    return response;
                }

                var list = service.get(id) as CuentasModel;
                if (list.Nivel == 0 && !list.Bloqueado)
                {
                    var tipocuentaService = new TiposcuentasService(ContextService);
                    var obj = tipocuentaService.get(tipocuenta) as TiposCuentasModel;
                    if (list.Id.StartsWith(obj.Cuenta))
                    {
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                            "application/json");
                        return response;
                    }


                }

                var final = Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                return final;


            }
        }

        public CuentasTiposTercerosApiController(IContextService context) : base(context)
        {
        }
    }
}
