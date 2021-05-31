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
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class UltimoprecioApiController : ApiBaseController
    {
        public UltimoprecioApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {

            using (var service = new UltimospreciosService(ContextService))
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var cuenta = nvc["fkcuenta"];
                var articulo = nvc["articulo"];
                var tipo = nvc["tipodocumento"];
                var tipodocumento = (TipoDocumentos)Enum.Parse(typeof(TipoDocumentos), tipo);
                var result = new UltimospreciosModel()
                {
                    Especificos = new ResultBusquedas<UltimoprecioEspecificoModel>()
                    {
                        values = service.GetUltimosPrecios(articulo, cuenta, tipodocumento).OrderByDescending(f => f.Fecha),
                        columns = new[]
                        {
                            new ColumnDefinition()
                            {
                                field = "Referenciadocumento",
                                displayName = "Documento",
                                visible = true,
                                filter = new Filter() {condition = ColumnDefinition.STARTS_WITH}
                            },
                            new ColumnDefinition() {field = "Fecha", displayName = "Fecha", visible = true, type="date",filter= new Filter() {type = "date"} },
                            new ColumnDefinition() {field = "DtoCial", displayName = "DtoCial", visible = true},
                            new ColumnDefinition() {field = "DtoPP", displayName = "DtoPP", visible = true},
                            new ColumnDefinition() {field = "Cantidad", displayName = "Cantidad", visible = true},
                            new ColumnDefinition() {field = "Metros", displayName = "Metros", visible = true},
                            new ColumnDefinition() {field = "Precio", displayName = "Precio", visible = true},
                            new ColumnDefinition() {field = "DtoLin", displayName = "DtoLin", visible = true},
                            new ColumnDefinition() {field = "Moneda", displayName = "Moneda", visible = true}
                        }
                    },
                    SistemaVenta = new ResultBusquedas<UltimopreciosistemaModel>()
                    {
                        values = service.GetPreciosSistema(articulo, TipoFlujo.Venta, cuenta),
                        columns = new[]
                        {
                             new ColumnDefinition() {field = "Tarifa", displayName = "Tarifa", visible = true},
                            new ColumnDefinition() {field = "Precio", displayName = "Precio", visible = true}
                        }
                    },
                    SistemaCompra = new ResultBusquedas<UltimopreciosistemaModel>()
                    {
                        values = service.GetPreciosSistema(articulo, TipoFlujo.Compra, cuenta),
                        columns = new[]
                        {
                             new ColumnDefinition() {field = "Tarifa", displayName = "Tarifa", visible = true},
                            new ColumnDefinition() {field = "Precio", displayName = "Precio", visible = true}
                        }
                    },
                    SistemaEspecial = new ResultBusquedas<UltimopreciosistemaModel>()
                    {
                        values = service.GetPreciosEspeciales(articulo, cuenta),
                        columns = new[]
                        {
                             new ColumnDefinition() {field = "Tarifa", displayName = "Tarifa", visible = true},
                            new ColumnDefinition() {field = "Precio", displayName = "Precio", visible = true}
                        }
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

            using (var service = FService.Instance.GetService(typeof(AcabadosModel), ContextService) as AcabadosService)
            {
                service.Empresa = ContextService.Empresa;
                var list = service.get(id) as UltimoprecioEspecificoModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;


            }
        }
    }
}
