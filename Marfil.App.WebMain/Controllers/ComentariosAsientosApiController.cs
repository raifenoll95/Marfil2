using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class ComentariosAsientosApiController : ApiBaseController
    {

        public ComentariosAsientosApiController(IContextService context) : base(context)
        {

        }

        public HttpResponseMessage Get()
        {
            using (var service = FService.Instance.GetService(typeof(BaseTablasVariasModel), ContextService) as TablasVariasService)
            {
                var list = service.GetListComentariosAsientos();
                var result = new ResultBusquedas<TablasVariasGeneralModel>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() {field="Valor",displayName="Codigo",visible=true,
                                                 filter = new  Dom.ControlsUI.Busquedas.Filter() {condition=ColumnDefinition.STARTS_WITH },width=150},
                        new ColumnDefinition() {field="Descripcion",displayName="Descripcion",visible=true ,
                                                 filter = new  Dom.ControlsUI.Busquedas.Filter() {condition=ColumnDefinition.STARTS_WITH } }
                    }
                };

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }


        // GET: api/Supercuentas/5
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get(string id)
        {
            var listcomentariosasientos = WebHelper.GetApplicationHelper().GetListComentariosAsientos().Select(f => new SelectListItem()
            {
                Text = f.Descripcion,
                Value = f.Valor
            }).ToList();
            var comentarioasiento = listcomentariosasientos.Where(f => f.Value == id);
            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(JsonConvert.SerializeObject(comentarioasiento), Encoding.UTF8, "application/json");
            return response;
        }
    }
}