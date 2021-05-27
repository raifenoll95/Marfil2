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

namespace Marfil.App.WebMain.Controllers
{
    public class ArticulosComponentesApiController : ApiBaseController
    {
        public ArticulosComponentesApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var articulo = nvc["articulo"];

            using (var service = new ArticulosService(ContextService))
            {
                var articuloModel = service.get(articulo) as ArticulosModel;
                var result = new ResultBusquedas<ArticulosComponentesModel>()
                {
                    values = articuloModel.ArticulosComponentes,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "IdComponente", displayName = "IdComponente", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "FkArticulo", displayName = "Componente", visible = true },
                        new ColumnDefinition() { field = "DescripcionComponente", displayName = "Descripcion", visible = true },
                        new ColumnDefinition() { field = "Piezas", displayName = "Piezas", visible = true },
                        new ColumnDefinition() { field = "Largo", displayName = "Largo", visible = true },
                        new ColumnDefinition() { field = "Ancho", displayName = "Ancho", visible = true },
                        new ColumnDefinition() { field = "Grueso", displayName = "Grueso", visible = true },
                        new ColumnDefinition() { field = "Merma", displayName = "Merma", visible = true }
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
            using (var service = new ArticulosService(ContextService))
            {

                var list = service.get(id) as ArticulosComponentesModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }
    }
}
