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
using Marfil.Dom.Persistencia.ServicesView;

namespace Marfil.App.WebMain.Controllers
{
    public class CuentasTiposTercerosSemiExclusiveApiController : ApiBaseController
    {
        public CuentasTiposTercerosSemiExclusiveApiController(IContextService context) : base(context)
        {
        }

        // GET: api/CuentasTiposTerceros
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get()
        {

            var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
            var tipocuenta = nvc["tipocuenta"];
            IEnumerable<CuentasModel> items = null;

            if (tipocuenta == "3")//Cuentas de tesorería junto con cuentas de caja
            {
                using (var service = new CuentastesoreriaService(ContextService))
                {

                    try
                    {
                        items = service.getCuentasTesoreria();
                    }
                    catch (Exception)
                    {


                    }

                    var result = new ResultBusquedas<CuentasModel>()
                    {
                        values = items ?? Enumerable.Empty<CuentasModel>(),
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
            else
            {
                using (var service = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService)
                {
                    try
                    {
                        var inttipocuenta = (TiposCuentas)Funciones.Qint(tipocuenta).Value;
                        var totalitems = service.GetCuentasClientes(inttipocuenta);
                        items = totalitems.Where(f => (f.Tiposcuentas == (int)inttipocuenta || (f.Tiposcuentas == 0 || !f.Tiposcuentas.HasValue)) && f.Bloqueado == false);
                    }
                    catch (Exception)
                    {


                    }

                    var result = new ResultBusquedas<CuentasModel>()
                    {
                        values = items ?? Enumerable.Empty<CuentasModel>(),
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

        }

        // GET: api/CuentasClienteApi/5
        [System.Web.Mvc.Authorize]
        public HttpResponseMessage Get(string id)
        {

            using (var service = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var tipocuenta = nvc["tipocuenta"];

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
                if (list.Nivel == 0 && !list.Bloqueado)
                {
                    //var tipocuentaService = new TiposcuentasService();
                    //var obj = tipocuentaService.get(tipocuenta) as TiposCuentasModel;
                    //var inttipocuenta = (TiposCuentas)Funciones.Qint(tipocuenta).Value;
                    //if (list.Id.StartsWith(obj.Cuenta) && (list.Tiposcuentas == (int)inttipocuenta || (list.Tiposcuentas == 0 || !list.Tiposcuentas.HasValue)))
                    //{
                    var response = Request.CreateResponse(HttpStatusCode.OK);
                    response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                        "application/json");
                    return response;
                    //}


                }

                var final = Request.CreateResponse(HttpStatusCode.ExpectationFailed);
                return final;


            }
        }
    }
}
