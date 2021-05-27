using System.Web.Http.Results;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Interfaces;

using System.Reflection;
using System.Web.Script.Serialization;
//using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;

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
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;




namespace Marfil.App.WebMain.Controllers
{
    public class DescripcionesContablesApiController : ApiBaseController
    {
        private readonly GestionService<BaseTablasVariasModel, Tablasvarias> _service;

        public DescripcionesContablesApiController(IContextService context) : base(context)
        {
            _service = FService.Instance.GetService(typeof(BaseTablasVariasModel), context) as TablasVariasService;
        }



    // GET: api/Supercuentas/5
    public HttpResponseMessage Get(string id)
    {

        using (var service =   
            FService.Instance.GetService(typeof(BaseTablasVariasModel), ContextService) as TablasVariasService)
        {

            var list =    service.GetDescripcionAsiento(id);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                "application/json");
            return response;

        }
    }

}
}



