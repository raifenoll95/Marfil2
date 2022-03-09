using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marfil.App.WebMain.Controllers
{
    public class ListadoCuentasNoAsignadasFuncionalController : ListadosController<ListadoCuentasNoAsignadasFuncional>
    {
        public ListadoCuentasNoAsignadasFuncionalController(IContextService context) : base(context)
        {
        }

    }
}