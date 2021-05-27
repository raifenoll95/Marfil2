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
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class StCuentasTerceros
    {
        public string Tipocuenta { get; set; }
        public string CuentaId { get; set; }
    }
    
    public class MobileCuentasTercerosApiController : BasicAuthHttpModule
    {
        public MobileCuentasTercerosApiController(ILoginService service) : base(service)
        {
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult GetTercero(StCuentasTerceros model)
        {
            using (var service = CreateGestionService(model.Tipocuenta))
            {
                var list = service.BuscarTercero(model.CuentaId);
                return Json(list);
            }
        }

        private IMobileTercerosService CreateGestionService(string tipocuenta)
        {
            if (tipocuenta == "0")
            {
                return FService.Instance.GetService(typeof (ClientesModel), ContextService,
                    MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as IMobileTercerosService;
            }
            else if (tipocuenta == "1")
            {
                return FService.Instance.GetService(typeof(ProveedoresModel), ContextService,
                    MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as IMobileTercerosService; 
            }
            else if (tipocuenta == "2")
            {
                return FService.Instance.GetService(typeof(AcreedoresModel), ContextService,
                    MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as IMobileTercerosService; 
            }

            throw new Exception("No existe servicio para el tipo de cuenta " + tipocuenta);
        }
    }
}
