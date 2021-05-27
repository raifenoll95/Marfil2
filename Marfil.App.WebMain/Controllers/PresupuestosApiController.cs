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
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;

namespace Marfil.App.WebMain.Controllers
{
    public class PresupuestosApiController : ApiBaseController
    {
        public PresupuestosApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
          
            using (var service = FService.Instance.GetService(typeof(PresupuestosModel),ContextService) as PresupuestosService)
            {
                var result = new ResultBusquedas<PresupuestosModel>()
                {
                    values = service.getAll().Select(f=>(PresupuestosModel)f),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Referencia", displayName = "Referencia", visible = true },
                        new ColumnDefinition() { field = "Fecha", displayName = "Fecha", visible = true },
                        new ColumnDefinition() { field = "Fkclientes", displayName = "Cod. Cliente", visible = true },
                        new ColumnDefinition() { field = "Nombrecliente", displayName = "Cliente", visible = true }
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
           
            using (var service = FService.Instance.GetService(typeof(PresupuestosModel),ContextService) as PresupuestosService)
            {
                var list = service.GetByReferencia(id);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }
    }
}
