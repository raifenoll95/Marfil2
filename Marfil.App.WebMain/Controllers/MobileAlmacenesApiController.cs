using System;
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
using System.Web.UI.WebControls;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Mobile;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;



namespace Marfil.App.WebMain.Controllers
{
    public class StAlmacenes : BaseParam
    {
        public string Id { get; set; }
    }
    
    public class MobileAlmacenesApiController : ApiBaseController
    {
        public MobileAlmacenesApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Post(StAlmacenes model)
        {
            var mierror = "";
            try
            {
              
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    if (string.IsNullOrEmpty(model.Id))
                    {
                        return CrearListado(response,model);
                    }
                    else
                        return CrearAlmacen(response, model);
                
                   
                
            }
            catch (Exception ex)
            {
                mierror = ex.Message;
            }

            var responseError = Request.CreateResponse(HttpStatusCode.InternalServerError);
            responseError.Content= new StringContent(mierror, Encoding.UTF8, "application/json");
            return responseError;
        }


        private HttpResponseMessage CrearListado(HttpResponseMessage input, StAlmacenes model)
        {
            using (
                var service = FService.Instance.GetService(typeof (AlmacenesModel), ContextService,MarfilEntities.ConnectToSqlServer(model.Basedatos)) as AlmacenesService)
            {

                var list = service.GetAll<AlmacenesModel>();

                var result = new ResultBusquedas<AlmacenesModel>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition()
                        {
                            field = "Id",
                            displayName = "Id",
                            visible = true,
                            filter = new Filter() {condition = ColumnDefinition.STARTS_WITH}
                        },
                        new ColumnDefinition()
                        {
                            field = "Descripcion",
                            displayName = "Descripción",
                            visible = true,
                            filter = new Filter() {condition = ColumnDefinition.STARTS_WITH}
                        },

                    }
                };


                input.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8,
                    "application/json");
                return input;
            }
        }

        private HttpResponseMessage CrearAlmacen(HttpResponseMessage input, StAlmacenes model)
        {
            using (var service = FService.Instance.GetService(typeof(AlmacenesModel), ContextService, MarfilEntities.ConnectToSqlServer(model.Basedatos)) as AlmacenesService)
            {

                var list = service.get(model.Id);

                input.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return input;

            }
        }

    }
}
