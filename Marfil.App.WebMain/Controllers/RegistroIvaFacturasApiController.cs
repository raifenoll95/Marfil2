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
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;

namespace Marfil.App.WebMain.Controllers
{
    public class RegistroIvaFacturasApiController : ApiBaseController
    {
        public RegistroIvaFacturasApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {

            using (var service = FService.Instance.GetService(typeof(FacturasModel), ContextService))
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var cliente = nvc["cliente"];

                var result = new ResultBusquedas<FacturasModel>()
                {
                    values = service.getAll().Select(f => (FacturasModel)f).Where(f => f.Fkclientes == cliente),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Id", visible = true/*, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }*/},
                        new ColumnDefinition() { field = "Referencia", displayName = "Referencia", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH } },
                        new ColumnDefinition() { field = "Fechadocumento", displayName = "Fechadocumento", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH } },
                        new ColumnDefinition() { field = "Fktipofactura", displayName = "Fktipofactura", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH } },
                        new ColumnDefinition() { field = "Importetotaldoc", displayName = "Importetotaldoc", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH } }

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

            using (var service = FService.Instance.GetService(typeof(MaterialesModel), ContextService))
            {

                var list = service.get(id) as MaterialesModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;


            }
        }
    }
}
