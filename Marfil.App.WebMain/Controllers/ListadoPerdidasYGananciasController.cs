using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.App.WebMain.Controllers
{
    public class ListadoPerdidasYGananciasController : ListadosController<ListadoPerdidasYGanancias>
    {
        public ListadoPerdidasYGananciasController(IContextService context) : base(context)
        {
        }
    }
}