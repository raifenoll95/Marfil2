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
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;

namespace Marfil.App.WebMain.Controllers
{
    public class VariedadesApiController : ApiBaseController
    {
        public VariedadesApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
            
            using (var service = FService.Instance.GetService(typeof(MaterialesModel),ContextService) as MaterialesService)
            {
                
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var articulos  = nvc["fkarticulos"];
                var material = ArticulosService.GetCodigoMaterial(articulos);
                var model= service.get(material) as MaterialesModel;

                var result = new ResultBusquedas<MaterialesLinModel>()
                {
                    values = model.Lineas,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Codigovariedad", displayName = "Id", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
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
            
            using (var service = FService.Instance.GetService(typeof(MaterialesModel),ContextService))
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var articulos = nvc["fkarticulos"];
                var material = ArticulosService.GetCodigoMaterial(articulos);
                var model = service.get(material) as MaterialesModel;
                var resultado = model.Lineas.FirstOrDefault(f => f.Codigovariedad == id);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(resultado), Encoding.UTF8,
                    "application/json");
                return response;
               

            }
        }
    }
}
