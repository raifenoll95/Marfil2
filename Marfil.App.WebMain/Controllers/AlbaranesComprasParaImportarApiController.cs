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
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;

namespace Marfil.App.WebMain.Controllers
{
    public class AlbaranesComprasParaImportarApiController : ApiBaseController
    {
        public AlbaranesComprasParaImportarApiController(IContextService context) : base(context)
        {
        }

        public HttpResponseMessage Get()
        {
           
            using (var service = FService.Instance.GetService(typeof(AlbaranesComprasModel),ContextService) as AlbaranesComprasService)
            {

                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var cliente = nvc["cliente"];
                var fkalbaraninicio = nvc["fkalbaraninicio"];
                var fkalbaranfin = nvc["fkalbaranfin"];

                //Todo EL: Crear un nuevo metodo en AlbaranesComprasservice que nos devuelva los AlbaranesCompras que se pueden importar
                var listAlbaranesCompras = service.BuscarAlbaranesComprasImportar("ENV");
                var estadosService = new EstadosService(ContextService);
                var estados = estadosService.GetStates(DocumentoEstado.AlbaranesCompras);
                var result = new ResultBusquedas<AlbaranesComprasModel>()
                {
                    values = string.IsNullOrEmpty(cliente) ? listAlbaranesCompras : listAlbaranesCompras.Where(f => f.Fkproveedores == cliente),
                    columns = new[]
                {
                        new ColumnDefinition() { field = "Fkseries", displayName = "Serie", visible = true },
                        new ColumnDefinition() { field = "Referencia", displayName = "Referencia", visible = true },
                        new ColumnDefinition() { field = "Fkproveedor", displayName = "Proveedor", visible = true },
                        new ColumnDefinition() { field = "Fechadocumentocadena", displayName = "Fecha", visible = true },
                        new ColumnDefinition() { field = "Importebaseimponible", displayName = "Base", visible = true },
                        /*new ColumnDefinition() { field = "Porcentajedescuentocomercialcadena", displayName = "%Dto. Cial.", visible = true },
                        new ColumnDefinition() { field = "Porcentajedescuentoprontopagocadena", displayName = "%Dto. P.P.", visible = true },
                        new ColumnDefinition() { field = "Obradescripcion", displayName = "Obra", visible = true },
                        new ColumnDefinition() { field = "Estadodescripcion", displayName = "Estado", visible = true,filter=new Filter() {type="select",selectOptions=estados.Select(f=>new ComboListItem() { label=f.Descripcion,value=f.Descripcion}).ToList()} },*/

                    }
                };

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
                return response;


            }
        }

        // GET: api/Supercuentas/5
        public HttpResponseMessage Get(string id)
        {
            
            using (var service = FService.Instance.GetService(typeof(AlbaranesComprasModel), ContextService) as AlbaranesComprasService)
            {
                var nvc = HttpUtility.ParseQueryString(Request.RequestUri.Query);
                var cliente = nvc["cliente"];
                var list = service.GetByReferencia(id);
                //Todo EL: Crear un nuevo metodo en AlbaranesComprasservice que nos devuelva un presupuesto, con 2 parametros de entrada: cliente y referencia
                if (!string.IsNullOrEmpty(cliente) && list.Fkproveedores != cliente)
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError);
                }

                var response = Request.CreateResponse(HttpStatusCode.OK);
                response.Content = new StringContent(JsonConvert.SerializeObject(list), Encoding.UTF8,
                    "application/json");
                return response;
            }
        }

       
    }
}
