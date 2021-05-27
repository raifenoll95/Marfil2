using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class UsuariosApiController : ApiBaseController
    {
        private readonly UsuariosService _service;

        public UsuariosApiController(IContextService context) : base(context)
        {
            _service = FService.Instance.GetService(typeof(UsuariosModel), ContextService) as UsuariosService;
        }

        public HttpResponseMessage Get()
        {
            var list = _service.getAll();
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(list.Select(f=>f as UsuariosModel)), Encoding.UTF8,
                "application/json");
            return response;
        }

        
    }
}