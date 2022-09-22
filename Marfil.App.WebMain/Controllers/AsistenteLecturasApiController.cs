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
using Marfil.Dom.Persistencia.Model;

namespace Marfil.App.WebMain.Controllers
{
    public class AsistenteLecturasApiController : ApiBaseController
    {
        public AsistenteLecturasApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {

            using (var service = FService.Instance.GetService(typeof(LecturasModel), ContextService) as LecturasService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                var result = new ResultBusquedas<LecturasAsistenteModel>()
                {
                    values = service.BuscarLecturas(),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Identificador", displayName = "Identificador", visible = true, width=150},
                        new ColumnDefinition() { field = "Fechaformat", displayName = "Fecha", visible = true, width=150},
                        new ColumnDefinition() { field = "Numregistros", displayName = "Registros", visible = true, width=150, type = "number"},

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
            using (var service = FService.Instance.GetService(typeof(LecturasModel), ContextService) as LecturasService)
            {
                var list = service.get(id) as LecturasModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }

    }
}