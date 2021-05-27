using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.ServicesView;

namespace Marfil.App.WebMain.Controllers
{
    public class SupercuentasController : ApiBaseController
    {
        public SupercuentasController(IContextService context) : base(context)
        {
        }

        // GET: api/Supercuentas
        public HttpResponseMessage Get()
        {
            
            using (var service = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                var result = new ResultBusquedas<CuentasModel>()
                {
                    values = service.GetSuperCuentas(),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Cuentas", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true },
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
            
            using (var service = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {

                var list = service.get(id) as CuentasModel;
                if (list.Nivel == appService.NivelesCuentas(ContextService.Empresa))
                {
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                        "application/json");
                    return response;
                }
                else
                {
                    var response = Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                    return response;
                }
                
            }
        }

        // POST: api/Supercuentas
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Supercuentas/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Supercuentas/5
        public void Delete(int id)
        {
        }
    }
}
