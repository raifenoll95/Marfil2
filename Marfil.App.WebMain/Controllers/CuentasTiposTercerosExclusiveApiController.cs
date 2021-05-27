using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;

namespace Marfil.App.WebMain.Controllers
{
    public class CuentasTiposTercerosExclusiveApiController : ApiBaseController
    {
        public CuentasTiposTercerosExclusiveApiController(IContextService context) : base(context)
        {
        }
        // GET: api/CuentasTiposTerceros
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get()
        {
           
            using (var service = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);

                var primeracarga = true;
                if(nvc[Constantes.Primeracarga]!=null)
                    primeracarga = Funciones.Qbool(nvc[Constantes.Primeracarga]);
                var tipocuenta = nvc["tipocuenta"];
                if (tipocuenta == null)
                    tipocuenta = "99";

                var list = service.GetCuentas((TiposCuentas)Funciones.Qint(tipocuenta).Value).ToList();

                //Rai -- Esto no sé que hace aquí, Bloqueado en false bien porque solo se muestran las activas, pero primeracarga??? no se que es eso

                //var cuentas = prueba.Where(f => primeracarga && f.Bloqueado == false);

                var result = new ResultBusquedas<CuentasBusqueda>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Cuentas", visible = true, filter = new  Filter() { condition = ColumnDefinition.STARTS_WITH }},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true },
                    }
                };


                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;
            }
        }

        // GET: api/CuentasClienteApi/5
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get(string id)
        {
          
            using (var service = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var tipocuenta = nvc["tipocuenta"];
                var permitebloqueados = Funciones.Qbool(nvc["Permitebloqueados"]);
                var primeracarga = Funciones.Qbool(nvc[Constantes.Primeracarga]);
                if (Regex.IsMatch(id, ".*\\.\\*"))
                {
                    //get nuevo index
                    var fmodel = new FModel();

                    var vector = id.Split('.');
                    var subcuenta = vector[0];
                    var maxlevel = appService.NivelesCuentas(ContextService.Empresa);
                    subcuenta = subcuenta.PadRight(maxlevel, '0');
                    var nextid = service.GetNextCuenta(ContextService.Empresa, subcuenta);
                    var newCuenta = fmodel.GetModel<CuentasModel>(ContextService);
                    newCuenta.Id = nextid;
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(newCuenta), Encoding.UTF8,
                        "application/json");
                    return response;
                }

                var list = service.get(id) as CuentasModel;
                if (list.Nivel == 0 && (primeracarga || (!list.Bloqueado || permitebloqueados)))
                {
                    var inttipocuenta = Funciones.Qint(tipocuenta);
                    var tipocuentaService = new TiposcuentasService(ContextService);
                    if (tipocuenta == null)
                    {
                        tipocuenta = tipocuentaService.GetTipoTercero(id).SingleOrDefault().ToString();
                        inttipocuenta = Funciones.Qint(tipocuenta);
                    }                        
                    var obj = tipocuentaService.get(tipocuenta) as TiposCuentasModel;                      
                    if (list.Id.StartsWith(obj.Cuenta) && list.Tiposcuentas==inttipocuenta)
                    {
                        var response = Request.CreateResponse(HttpStatusCode.OK);
                        response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                            "application/json");
                        return response;
                    }


                }

                var final = Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                return final;


            }
        }

       
    }
}
