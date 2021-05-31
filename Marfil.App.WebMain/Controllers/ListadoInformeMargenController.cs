using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.App.WebMain.Controllers
{
    public class ListadoInformeMargenController : ListadosController<ListadoInformeMargen>
    {
        public ListadoInformeMargenController(IContextService context) : base(context)
        {
        }
    }
}