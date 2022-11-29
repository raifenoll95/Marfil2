using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marfil.App.WebMain.Controllers
{
    public class ListadosStockValoradoTarifaController : ListadosController<ListadosStockValoradoTarifa>
    {
        public ListadosStockValoradoTarifaController(IContextService context) : base(context)
        {
        }
    }
}