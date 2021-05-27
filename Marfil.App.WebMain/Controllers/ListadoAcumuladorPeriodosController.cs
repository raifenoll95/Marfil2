using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.App.WebMain.Controllers
{
    public class ListadoAcumuladorPeriodosController : ListadosController<ListadoAcumuladorPeriodos>
    {
        public ListadoAcumuladorPeriodosController(IContextService context) : base(context)
        {

        }
    }
}