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
using Marfil.Dom.Persistencia.Model.Documentos.GrupoMateriales;

namespace Marfil.App.WebMain.Controllers
{
    public class GrupoMaterialesApiController : ApiBaseController
    {
        public GrupoMaterialesApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {

            using (var service = FService.Instance.GetService(typeof(GrupoMaterialesModel), ContextService))
            {

                //var a = service.getAll().Select(f => (GrupoMaterialesModel)f);
                var result = new ResultBusquedas<GrupoMaterialesModel>()
                {
                    values = service.getAll().Select(f => (GrupoMaterialesModel)f),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Cod", displayName = "Cod", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripcion", visible = true },
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

            using (var service = FService.Instance.GetService(typeof(GrupoMaterialesModel), ContextService))
            {

                var list = service.get(id) as GrupoMaterialesModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;


            }
        }
    }
}
