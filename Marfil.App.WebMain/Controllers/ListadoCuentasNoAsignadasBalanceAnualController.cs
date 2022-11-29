using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Listados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marfil.App.WebMain.Controllers
{
    public class ListadoCuentasNoAsignadasBalanceAnualController : ListadosController<ListadoCuentasNoAsignadasBalanceAnual>
    {
        public ListadoCuentasNoAsignadasBalanceAnualController(IContextService context) : base(context)
        {
        }
    }
}