using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

using System.Net.Http;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Newtonsoft.Json.Linq;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.ServicesView;

namespace Marfil.App.WebMain.Controllers
{
    public class TablasVariasApiController : ApiBaseController
    {
        private readonly GestionService<BaseTablasVariasModel, Tablasvarias> _service ;

        public TablasVariasApiController(IContextService context) : base(context)
        {
            _service = FService.Instance.GetService(typeof(BaseTablasVariasModel), context) as TablasVariasService;
        }


        // PUT api/<controller>/5
        public void Put(int id, [FromBody]JObject value)
        {
            try
            {
                var item = _service.get(id.ToString()) as BaseTablasVariasModel;
                var serializer1 = new JavaScriptSerializer();
                item.Lineas = new List<dynamic>();
                JArray jsonObject = value["value"] as JArray;
                foreach (var child in jsonObject.Children())
                {
                    if (child.Count() > 1)
                        item.Lineas.Add(serializer1.Deserialize(child.ToString(), Helper.GetTypeFromFullName(item.Clase)));
                }

                _service.edit(item);
            }
            catch (Exception ex)
            {
                HttpResponseMessage message =new HttpResponseMessage(HttpStatusCode.InternalServerError);
                message.Content = new StringContent(ex.Message);
                throw new HttpResponseException(message);
            }
              
            
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}