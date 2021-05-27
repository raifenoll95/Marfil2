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
    public class ArticulosDocumentosApiController : ApiBaseController
    {
        public ArticulosDocumentosApiController(IContextService context) : base(context)
        {
        }
        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {
            
            using (var service = FService.Instance.GetService(typeof(ArticulosModel), ContextService) as ArticulosService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var flujocadena = HttpUtility.ParseQueryString(Request.RequestUri.Query)["flujo"];
                var acabadodesde = nvc["FkAcabadoDesde"];
                var acabadohasta = nvc["FkAcabadoHasta"];
                var material = nvc["Materialsalida"];
                var categoria = TipoCategoria.Ambas;
                if (!string.IsNullOrEmpty(flujocadena))
                {
                    if (flujocadena == "0")
                        categoria = TipoCategoria.Ventas;
                    else if (flujocadena=="1")
                        categoria=TipoCategoria.Compras;
                }
                var list = service.GetArticulosBusquedas(categoria);

                if (!string.IsNullOrEmpty(acabadodesde))
                {
                    list = list.Where(f => string.CompareOrdinal(f.Acabados,acabadodesde)>=0);
                }

                if (!string.IsNullOrEmpty(acabadohasta))
                {
                    list = list.Where(f => string.CompareOrdinal(f.Acabados, acabadohasta) <= 0);
                }

                if (!string.IsNullOrEmpty(material))
                {
                    list = list.Where(f => f.Materiales== material);
                }
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
                        new ColumnDefinition() { field = "Descripcion", displayName = "Desc. Artículo", visible = true,width=300}
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
                var flujocadena = HttpUtility.ParseQueryString(Request.RequestUri.Query)["flujo"];
                var fkcuentas = HttpUtility.ParseQueryString(Request.RequestUri.Query)["fkcuentas"];
                var fkmonedas = HttpUtility.ParseQueryString(Request.RequestUri.Query)["fkmonedas"];
                var fkregimeniva = HttpUtility.ParseQueryString(Request.RequestUri.Query)["fkregimeniva"];
                var flujo =string.IsNullOrEmpty(flujocadena)? TipoFlujo.Venta: (TipoFlujo) Enum.Parse(typeof (TipoFlujo), flujocadena);
                var acabadodesde = HttpUtility.ParseQueryString(Request.RequestUri.Query)["FkAcabadoDesde"];
                var acabadohasta = HttpUtility.ParseQueryString(Request.RequestUri.Query)["FkAcabadoHasta"];
                var material = HttpUtility.ParseQueryString(Request.RequestUri.Query)["Materialsalida"];

                var list = service.GetArticulo(id, fkcuentas, fkmonedas, fkregimeniva, flujo);
                if (!string.IsNullOrEmpty(acabadodesde))
                {
                    if (string.CompareOrdinal(ArticulosService.GetCodigoAcabado(list.Id), acabadodesde) < 0)
                    {
                        throw new Exception("Articulo no encontrado");
                    }
                }

                if (!string.IsNullOrEmpty(acabadohasta))
                {
                    if (string.CompareOrdinal(ArticulosService.GetCodigoAcabado(list.Id), acabadohasta) > 0)
                    {
                        throw new Exception("Articulo no encontrado");
                    }
                }

                if (!string.IsNullOrEmpty(material))
                {
                    if (material != ArticulosService.GetCodigoMaterial(list.Id))
                    {
                        throw new Exception("Articulo no encontrado");
                    }
                }
                var response = Request.CreateResponse(list==null? HttpStatusCode.InternalServerError :HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;

            }
        }
    }
}
