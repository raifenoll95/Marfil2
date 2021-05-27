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
using Marfil.Dom.Persistencia.Model.Stock;

namespace Marfil.App.WebMain.Controllers
{
    public class TiposAlbaranesApiController : ApiBaseController
    {
        public TiposAlbaranesApiController(IContextService context) : base(context)
        {
        }

  
        // GET: api/TareasApi/ALB
        [System.Web.Mvc.Authorize]        
        public HttpResponseMessage Get(string id)
        {
            ApplicationHelper app = new ApplicationHelper(ContextService);
            var tiposAlbaranes = app.GetListTiposAlbaranes();

            var result = app.GetListTiposAlbaranes().Where(f => !f.Salidas && !f.Entradas);
            result.OrderBy(f => f.Defecto);

            var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos);

            var tipoSalidasVarias = db.Series.Where(f => f.empresa == ContextService.Empresa && f.salidasvarias == true).Select(f => f.id).SingleOrDefault();
            var tipoEntradasVarias = db.Series.Where(f => f.empresa == ContextService.Empresa && f.entradasvarias == true).Select(f => f.id).SingleOrDefault();

            if (id == tipoSalidasVarias)
            {
                result = tiposAlbaranes.Where(f => f.Salidas == true);
            }
            else if (id == tipoEntradasVarias)
            {
                result = tiposAlbaranes.Where(f => f.Entradas == true);
            }   

            var response = Request.CreateResponse(HttpStatusCode.OK);

            response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
            return response;
        }          

    }
}