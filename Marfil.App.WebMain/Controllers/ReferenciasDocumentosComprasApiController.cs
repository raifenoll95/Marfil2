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
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class ReferenciasDocumentosComprasApiController : ApiBaseController
    {
        public ReferenciasDocumentosComprasApiController(IContextService context) : base(context)
        {
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {
            
            using (var service = FService.Instance.GetService(typeof(AlbaranesComprasModel),ContextService) as AlbaranesComprasService)
            {
                service.EjercicioId = ContextService.Ejercicio;
                var list = service.GetDocumentosCompras().OrderBy(f=>f.Fecha);

                var result = new ResultBusquedas<StDocumentosCompras>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Tipo", displayName = "Tipo", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Referencia", displayName = "Referencia", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH } },
                        new ColumnDefinition() { field = "Fechadocumento", displayName = "Fecha", visible = true},
                        new ColumnDefinition() { field = "Proveedor", displayName = "Proveedor", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Base", displayName = "Base", visible = true}
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
            
            using (var service = FService.Instance.GetService(typeof(AlbaranesComprasModel),ContextService) as AlbaranesComprasService)
            {
                service.EjercicioId = ContextService.Ejercicio;

                var documento = service.GetDocumentosCompras(id).Single();
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(documento), Encoding.UTF8,
                    "application/json");
                return response;

            }
        }
    }
}
