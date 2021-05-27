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
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class DireccionesContactoController : ApiBaseController
    {
        public DireccionesContactoController(IContextService context) : base(context)
        {
        }

        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get()
        {
           

            using (var service = FService.Instance.GetService(typeof(DireccionesLinModel),ContextService) as DireccionesService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var fkentidad = nvc["fkentidad"];
                var tipocuenta = Funciones.Qint(nvc["tipotercero"]) ?? 0;
                var result = new ResultBusquedas<DireccionesLinModel>()
                {
                    values = service.GetDirecciones(ContextService.Empresa, tipocuenta, fkentidad).ToList(),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = false, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Título", visible = true,width = 200},
                        new ColumnDefinition() { field = "Direccion", displayName = "Dirección", visible = true ,width = 300},
                        new ColumnDefinition() { field = "Poblacion", displayName = "Población", visible = true },
                        new ColumnDefinition() { field = "Cp", displayName = "Cp", visible = true },
                        new ColumnDefinition() { field = "Telefono", displayName = "Teléfono", visible = true },
                        new ColumnDefinition() { field = "Telefonomovil", displayName = "Móvil", visible = true },
                        new ColumnDefinition() { field = "Email", displayName = "Email", visible = true ,width=200 }
                    }
                };

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }

        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get(int id)
        {

            using (var service = FService.Instance.GetService(typeof(DireccionesLinModel),ContextService) as DireccionesService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                
                var entidad = nvc["fkentidad"];
                var tipocuenta = Funciones.Qint(nvc["tipotercero"]) ?? 0;
                var list = service.get(DireccionesLinModel.CreateId(tipocuenta, entidad, id)) as DireccionesLinModel;

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");

                return response;
            }

        }
    }
}

