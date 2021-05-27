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
    public class CuentasTiposComercialesExclusiveApiController : ApiBaseController
    {
        public CuentasTiposComercialesExclusiveApiController(IContextService context) : base(context)
        {
        }
        // GET: api/CuentasTiposTerceros
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get()
        {
           
            using (var service = FService.Instance.GetService(typeof(ComercialesModel),ContextService) as ComercialesService)
            {
                var result = new ResultBusquedas<ComercialesModel>()
                {
                    values = service.getAll().Select(f => (ComercialesModel)f),
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
            
            using (var service = FService.Instance.GetService(typeof(ComercialesModel),ContextService) as ComercialesService)
            {
                var list = service.get(id) as ComercialesModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }

      
    }
}
