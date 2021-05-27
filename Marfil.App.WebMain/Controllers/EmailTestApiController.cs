using System;
using System.Collections.Generic;
using System.Globalization;
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
using Marfil.Dom.Persistencia.ServicesView.Servicios.Mailing;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    
    public class EmailTestApiController : ApiBaseController
    {
        public EmailTestApiController(IContextService context) : base(context)
        {
        }

        // GET: api/Supercuentas/5


        // GET: api/Supercuentas/5
        public HttpResponseMessage Get()
        {
            var emailService = new EmailsService(ContextService);
            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var email = nvc["email"];
            var usuario = nvc["usuario"];
            var password = nvc["password"];
            var smtp = nvc["smtp"];
            var puerto =Funciones.Qint(nvc["puerto"])??0;
            var ssl = Funciones.Qbool(nvc["ssl"]);
            var nombre = nvc["nombre"];
            var asunto = General.AsuntoEmailTest;
            var cuerpo = string.Format(General.CuerpoEmailTest,DateTime.Now.ToString(CultureInfo.InvariantCulture));
            var mierror = "";
            emailService.EnviarEmail(email,"", nombre,email,usuario,password,smtp, puerto,asunto,cuerpo,true,null,null,ssl,null,ref mierror,0,0);
            
            var response =string.IsNullOrEmpty(mierror) ?  Request.CreateResponse(HttpStatusCode.OK): Request.CreateResponse(HttpStatusCode.InternalServerError);
            response.Content = new StringContent(mierror, Encoding.UTF8,
                "application/json");
            return response;
        }
    }
}
