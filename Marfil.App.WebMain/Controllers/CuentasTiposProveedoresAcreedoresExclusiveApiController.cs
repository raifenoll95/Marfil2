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
    public class CuentasTiposProveedoresAcreedoresExclusiveApiController : ApiBaseController
    {
        public CuentasTiposProveedoresAcreedoresExclusiveApiController(IContextService context) : base(context)
        {
        }
        // GET: api/CuentasTiposTerceros
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get()
        {
           
            using (var service = FService.Instance.GetService(typeof(ProveedoresModel),ContextService) as ProveedoresService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var primeracarga = Funciones.Qbool(nvc[Constantes.Primeracarga]);

                var result = new ResultBusquedas<CuentasBusqueda>()
                {
                    values = service.GetProveedoresAndAcreedores(primeracarga) ,//service.getAll().Where(f=> primeracarga || !((ClientesModel)f).Bloqueado).Select(f => (ClientesModel)f),
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
           
            using (var service = FService.Instance.GetService(typeof(ProveedoresModel),ContextService) as ProveedoresService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var primeracarga = Funciones.Qbool(nvc[Constantes.Primeracarga]);
                var list = service.GetProveedoresAcreedores(id);
                if (primeracarga || !list.Bloqueado)
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                        "application/json");
                    return response;
                }
                else return Request.CreateResponse(HttpStatusCode.BadRequest);
                
            }
        }

        
    }
}
