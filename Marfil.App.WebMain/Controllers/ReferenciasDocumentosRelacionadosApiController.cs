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
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.App.WebMain.Controllers
{
    public class ReferenciasDocumentosRelacionadosApiController : ApiBaseController
    {
        public ReferenciasDocumentosRelacionadosApiController(IContextService context) : base(context)
        {
        }

        // GET: api/ReferenciasDocumentosRelacionadosApi
        public HttpResponseMessage Get()
        {

            using (var service = FService.Instance.GetService(typeof(SeguimientosModel), ContextService) as SeguimientosService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                var primeracarga = true;
                if (nvc[Constantes.Primeracarga] != null)
                    primeracarga = Funciones.Qbool(nvc[Constantes.Primeracarga]);

                var tipodocumento = nvc["tipodocumento"];
                var tercero = nvc["tercero"];          

                var result = new ResultBusquedas<SeguimientosModel>()
                {                    
                    values = service.getDocumentosRelacionados(tipodocumento, tercero),
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Referencia", displayName = "Referencia", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Fecha", displayName = "Fecha documento", visible = true },
                        new ColumnDefinition() { field = "Baseimponible", displayName = "Base imponible", visible = true },
                    }
                };


                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }


        //public HttpResponseMessage Get(string id)
        //{
            
        //    using (var service = FService.Instance.GetService(typeof(AlbaranesComprasModel),ContextService) as AlbaranesComprasService)
        //    {
        //        service.EjercicioId = ContextService.Ejercicio;

        //        var documento = service.GetDocumentosCompras(id).Single();
        //        var response = Request.CreateResponse(HttpStatusCode.OK);
        //        response.Content = new StringContent(JsonConvert.SerializeObject(documento), Encoding.UTF8,
        //            "application/json");
        //        return response;

        //    }
        //}
    }
}
