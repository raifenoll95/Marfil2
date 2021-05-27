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
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Mobile;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;
using Filter = System.Web.Mvc.Filter;


namespace Marfil.App.WebMain.Controllers
{
    public class StCuentas : BaseParam, ICuentasFiltros
    {
        public string Id { get; set; }
        public string Filtros { get; set; }
        public string Pagina { get; set; }
        public string RegistrosPagina { get; set; }
        public string Tipocuenta { get; set; }
    }

    

   
    public class MobileCuentasApiController : BasicAuthHttpModule
    {
        
        public MobileCuentasApiController(ILoginService service) : base(service)
        {
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Buscar(StCuentas model)
        {
            using (var service = FService.Instance.GetService(typeof(CuentasModel), ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as CuentasService)
            {

                int registrosTotales = 0;
                IEnumerable<CuentasBusqueda> items = null;
                try
                {
                    
                    items = service.GetCuentas(model,out registrosTotales); 
                }
                catch (Exception ex)
                {
                    return Json(new ErrorJson(ex.Message));
                }

                var result = new ResultBusquedasPaginados<CuentasBusqueda>()
                {
                    values = items,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Fkcuentas", displayName = "Cuentas", visible = true},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true },
                        new ColumnDefinition() { field = "Razonsocial", displayName = "Razón", visible = true },
                        new ColumnDefinition() { field = "Nif", displayName = "Nif", visible = true },
                        new ColumnDefinition() { field = "Pais", displayName = "País", visible = true },
                        new ColumnDefinition() { field = "Provincia", displayName = "Provincia", visible = true },
                        new ColumnDefinition() { field = "Poblacion", displayName = "Población", visible = true },
                    },
                    RegistrosTotales = registrosTotales,
                    PaginaActual = Funciones.Qint(model.Pagina)??1,
                };


                
                return Json(result);
            }
        }

    }
}
