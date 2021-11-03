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
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Terceros;

namespace Marfil.App.WebMain.Controllers
{
    public class CuentasTesoreriaApiController : ApiBaseController
    {
        public CuentasTesoreriaApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

            using (var service = new CuentastesoreriaService(ContextService))
            {
                var listcuentas = service.getCuentasTesoreria();
                var result = new ResultBusquedas<CuentasModel>()
                {
                    values = listcuentas,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Cuenta", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripcion", visible = true }
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
            
            using (var service = new CuentasService(ContextService))
            {
                var model = service.get(id) as CuentasModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                if (!model.Bloqueado)
                {            
                    response.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8,
                        "application/json");
                    
                }
                return response;
            }
        }
      
    }
}
