using Marfil.App.WebMain.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Net.Http;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.ControlsUI.Busquedas;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.App.WebMain.Controllers
{
    public class TercerosApiController : ApiBaseController
    {
        public TercerosApiController(IContextService context) : base(context)
        {
        }

        // GET: api/Supercuentas/5
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get()
        {

            using(var service = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService)
            {

                var terceros = service.GetCuentasTercerosArticulos();
                var result = new ResultBusquedas<CuentasModel>()
                {
                    values = terceros,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Cuenta", visible = true,
                            filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }, width=150},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripcion", visible = true,
                            filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH } }
                    }
                };

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
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
    }
}