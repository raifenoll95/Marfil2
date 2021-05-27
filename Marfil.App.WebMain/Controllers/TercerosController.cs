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
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class TercerosController : ApiBaseController
    {
        private struct ExistItem
        {
            public bool Existe;
        } 
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get(string id)
        {
            
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var tipocuenta = nvc["tipocuenta"];
            var tipo = (TiposCuentas)Enum.Parse(typeof(TiposCuentas), tipocuenta);
            using (var service = FTercerosService.CreateService(tipo , user.Empresa))
            {
                var existe = service.exists(id);
                
                
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(new ExistItem() {Existe= existe}),Encoding.UTF8, "application/json");
                return response;
            }
        }


    }
}
