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
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Inf.Genericos.Helper;
using Newtonsoft.Json;

namespace Marfil.App.WebMain.Controllers
{
    public class StArticulos:IArticulosFiltros
    {
        public string Filtros { get; set; }
        public string Pagina { get; set; }
        public string RegistrosPagina { get; set; }

    }

    public class StListadoStock
    {
        public string Fkalmacen { get; set; }
        public string IdArticulo { get; set; }
    }
    
    public class MobileArticulosApiController : BasicAuthHttpModule
    {
        public MobileArticulosApiController(ILoginService service) : base(service)
        {
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult ListArticulos(StArticulos model)
        {
            using (var service = FService.Instance.GetService(typeof(ArticulosModel),ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos)) as ArticulosService)
            {
                var registrosTotales = 0;
                var list = service.GetArticulosBusquedasMobile(model, out registrosTotales);
                var result = new ResultBusquedasPaginados<ArticulosBusqueda>()
                {
                    values = list,
                    columns = new[]
                    {
                        new ColumnDefinition() { field = "Id", displayName = "Artículo", visible = true},
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true }
                        
                    },
                    RegistrosTotales = registrosTotales,
                    PaginaActual = Funciones.Qint(model.Pagina) ?? 1,
                };
                return Json(result);
            }
        }


        [System.Web.Mvc.HttpPost]
        public ActionResult ListLotes(StListadoStock model)
        {
            var service = new StockactualService(ContextService,MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos));
            var list = service.GetArticulosLotesAgrupados(model.Fkalmacen,ContextService.Empresa,model.IdArticulo);
            var result = new ResultBusquedasPaginados<StockActualMobileModel>()
            {
                values = list,
                columns = new[]
                    {
                        new ColumnDefinition() { field = "Fkarticulos", displayName = "Artículo", visible = true},
                        new ColumnDefinition() { field = "Articulo", displayName = "Descripción", visible = true },
                        new ColumnDefinition() { field = "Lote", displayName = "Lote", visible = true },
                        new ColumnDefinition() { field = "Cantidaddisponible", displayName = "Cantidad", visible = true },
                        new ColumnDefinition() { field = "SMetros", displayName = "Metros", visible = true },
                        new ColumnDefinition() { field = "UM", displayName = "UM", visible = true },

                    },
                RegistrosTotales = list.Count(),
                PaginaActual =  1
            };
            return Json(result);
        }
    }
}
