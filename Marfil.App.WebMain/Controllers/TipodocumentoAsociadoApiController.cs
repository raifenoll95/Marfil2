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
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class TipodocumentoAsociadoApiController : ApiBaseController
    {
        public TipodocumentoAsociadoApiController(IContextService context) : base(context)
        {
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get(string id)
        {
            var result = "";
            switch (id)
            {
                case "PRE":
                    result = "PED";
                    break;
                case "PED":
                    result = "ALB";
                    break;

                case "ALB":
                case "SAV":
                    result = "FRA";
                    break;

                case "FRA":
                    break;


                case "PRC":
                    result = "PEC";
                    break;
                case "PEC":
                    result = "ALC";
                    break;

                case "ALC":
                case "ENV":
                    result = "FRC";
                    break;

                case "FRC":
                    break;
                case "RES":
                    result = "ALB";
                    break;
                case "COR":
                    break;
                case "TRA":
                    break;
            }

            
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8,
                "application/json");
            return response;



        }
    }
}
