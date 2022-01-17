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
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;

namespace Marfil.App.WebMain.Controllers
{
    public class AsistenteRegularizacionGruposApiController : ApiBaseController
    {
        public AsistenteRegularizacionGruposApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {

            using (var service = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                var result = new ResultBusquedas<CuentasRegularizacionGruposModel>()
                {
                    values = service.BuscarCuentasGrupos6y7(),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Cuentagrupos", displayName = "Cuenta", visible = true, width=150},
                        new ColumnDefinition() { field = "SaldoDeudor", displayName = "Saldo Deudor", visible = true, width=150, type = "number"},
                        new ColumnDefinition() { field = "SaldoAcreedor", displayName = "Saldo Acreedor", visible = true, width=150, type = "number"}
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
                var list = service.get(id) as CuentasModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }
    }
}