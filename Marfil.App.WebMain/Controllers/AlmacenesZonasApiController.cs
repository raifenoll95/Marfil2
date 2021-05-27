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
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class AlmacenesZonasApiController : ApiBaseController
    {
        public AlmacenesZonasApiController(IContextService context) : base(context)
        {
        }
        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var almacen = nvc["fkalmacen"];
            
            using (var service = FService.Instance.GetService(typeof(AlmacenesModel),ContextService) as AlmacenesService)
            {
                var almacenModel = service.get(almacen) as AlmacenesModel;

                var list = almacenModel?.Lineas;

                var result = new ResultBusquedas<AlmacenesZonasModel>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Coordenadas", displayName = "Coordenadas", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},

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
            
            using (var service = FService.Instance.GetService(typeof(AlmacenesModel),ContextService) as AlmacenesService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var almacen = nvc["fkalmacen"];
                var list = service.get(almacen) as AlmacenesModel;

                var intId = Funciones.Qint(id)??0;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list.Lineas.First(f=>f.Id == intId)), Encoding.UTF8,
                    "application/json");
                return response;

            }
        }
    }
}
