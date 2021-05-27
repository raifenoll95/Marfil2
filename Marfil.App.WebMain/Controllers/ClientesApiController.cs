using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class ClientesApiController : BasicAuthHttpModule
    {
        struct StResultadoPost
        {
            public string Error { get; set; }
        }
        public ClientesApiController(ILoginService service) : base(service)
        {
        }

        [System.Web.Http.HttpPost]
        public ActionResult Crear(ClientesModel model)
        {

            var mierror = "";

            try
            {
                using (var gestionService = FService.Instance.GetService(typeof(ClientesModel), Principal.Empresa))
                {
                    gestionService.create(model);
                    return new EmptyResult();
                }

            }
            catch (Exception ex)
            {

                mierror = ex.Message;

            }


            return Json(new StResultadoPost() { Error = mierror });
        }

        [System.Web.Http.HttpPost]
        public ActionResult Borrar(string id)
        {

            var mierror = "";

            try
            {
                using (var gestionService = FService.Instance.GetService(typeof(ClientesModel), Principal.Empresa))
                {
                    gestionService.delete(gestionService.get(id));
                }

                return new EmptyResult();
            }
            catch (Exception ex)
            {

                mierror = ex.Message;

            }


            return Json(new StResultadoPost() { Error = mierror });
        }

        [HttpPost]
        public ActionResult Leer(string id)
        {
            var mierror = "";

            try
            {
                using (var gestionService = FService.Instance.GetService(typeof(ClientesModel), Principal.Empresa) as ClientesService)
                {
                    var model = gestionService.get(id) as ClientesModel;
                    var result = new ContentResult();
                    result.Content = JsonConvert.SerializeObject(model);
                    return result;
                }
            
            }
            catch (Exception ex)
            {

                mierror = ex.Message;

            }


            return Json(new StResultadoPost() { Error = mierror });
        }
    }
}
