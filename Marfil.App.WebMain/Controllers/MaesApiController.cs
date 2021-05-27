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
using Marfil.Dom.Persistencia.Model.Contabilidad.Maes;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class MaesApiController : ApiBaseController
    {

        public MaesApiController(IContextService context) : base(context)
        {

        }

        // GET: api/Supercuentas/5
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get(string id)
        {
            using (var service = FService.Instance.GetService(typeof(MaesModel), ContextService) as MaesService)
            {
                service.Empresa = ContextService.Empresa;
                service.EjercicioId = ContextService.Ejercicio;

                var list = service.get(id) as MaesModel;
                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }
    }
}