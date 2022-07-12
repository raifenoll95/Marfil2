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
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class ArticulosApiController : ApiBaseController
    {
        public ArticulosApiController(IContextService context) : base(context)
        {
        }
        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {
            
            using (var service = FService.Instance.GetService(typeof(ArticulosModel),ContextService) as ArticulosService)
            {
                var flujocadena = HttpUtility.ParseQueryString(Request.RequestUri.Query)["flujo"];
                var categoria = TipoCategoria.Ambas;
                if (!string.IsNullOrEmpty(flujocadena))
                {
                    if (flujocadena == "0")
                        categoria = TipoCategoria.Ventas;
                    else if (flujocadena == "1")
                        categoria = TipoCategoria.Compras;
                    else if (flujocadena == "3")
                        categoria = (TipoCategoria)3; //Muestra los artículos de todas las categorías.
                }
                var list = service.GetArticulosBusquedas(categoria);

                var result = new ResultBusquedas<ArticulosModel>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Referencia", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Familia", displayName = "Cod.Familia", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH } ,width=70},
                        new ColumnDefinition() { field = "FamiliaDescripcion", displayName = "Familia", visible = true},
                        new ColumnDefinition() { field = "Materiales", displayName = "Cod.Material", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH },width=70 },
                        new ColumnDefinition() { field = "MaterialesDescripcion", displayName = "Material", visible = true},
                        new ColumnDefinition() { field = "Caracteristicas", displayName = "Cod.Caracteristica", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH },width=70 },
                        new ColumnDefinition() { field = "CaracteristicasDescripcion", displayName = "Caracteristica", visible = true},
                        new ColumnDefinition() { field = "Grosores", displayName = "Cod.Grosor", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH },width=70 },
                        new ColumnDefinition() { field = "GrosoresDescripcion", displayName = "Grosor", visible = true},
                        new ColumnDefinition() { field = "Acabados", displayName = "Cod.Acabado", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH },width=70 },
                        new ColumnDefinition() { field = "AcabadosDescripcion", displayName = "Acabado", visible = true},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Desc. Artículo", visible = true,width=300},


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
            
            using (var service = FService.Instance.GetService(typeof(ArticulosModel),ContextService) as ArticulosService)
            {

                var list = service.get(id);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;

            }
        }

       
    }
}
