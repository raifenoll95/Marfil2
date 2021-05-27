using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using Newtonsoft.Json;


using Marfil.Dom.Persistencia.ServicesView.Servicios.Vencimientos;

namespace Marfil.App.WebMain.Controllers
{
    public class VencimientosApiController : ApiBaseController
    {
        public VencimientosApiController(IContextService context) : base(context)
        {
        }

        // GET: api/CuentasClienteApi/5
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get(string id)
        {
            //using (var service = FService.Instance.GetService(typeof(FacturasModel), ContextService) as FacturasService)
            //{
                //                int myid = int.Parse(id);
                //              var model = new List<FacturasVencimientosModel>();
                //var item = model.Single(f => f.Id == myid);

                // parametros proporcionados
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var dias = nvc["dias"];
            var fechadocumento = nvc["fechadocumento"];
            var fkformaspago = nvc["fkformaspago"];


            // sacamos dia1 y dia2 de la forma de pago



            var list = FacturasService.GetFechavencimiento(DateTime.Parse(fechadocumento), int.Parse(dias), 15, 30, true);//diapago1, diapago2, excluirfestivos);


                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
              //  }
        }
    }
}
