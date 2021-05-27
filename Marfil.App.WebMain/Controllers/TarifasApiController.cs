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
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class TarifasApiController : ApiBaseController
    {
        public TarifasApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get(string id)
        {
            
            using (var service =FService.Instance.GetService(typeof(TarifasModel),ContextService))
            {
                var tipoflujo = (TipoFlujo)Enum.Parse(typeof (TipoFlujo), id);
                var result =
                    service.getAll()
                        .OfType<TarifasModel>()
                        .Where(f => f.Tipotarifa == TipoTarifa.Sistema && f.Tipoflujo == tipoflujo);


                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }

       
    }
}
