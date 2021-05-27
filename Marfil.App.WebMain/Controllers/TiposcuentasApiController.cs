using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class TiposcuentasApi
    {
        public string Text { get; set; }    
        public int Value { get; set; }
    }

    public class TiposcuentasApiController : ApiBaseController
    {
        public TiposcuentasApiController(IContextService context) : base(context)
        {
        }

        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get(string id)
        {
            

            using (var service = FService.Instance.GetService(typeof(TiposCuentasModel),ContextService) as TiposcuentasService)
            {
                var list = service.GetTiposCuentasFromCuentaCliente(id).Select(f=>new TiposcuentasApi() {Text= Funciones.GetEnumByStringValueAttribute(f), Value=(int)f});

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }
    }
}
