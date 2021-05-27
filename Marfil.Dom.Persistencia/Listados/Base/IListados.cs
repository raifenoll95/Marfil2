using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Listados.Base
{
    public interface IListados
    {
        IContextService Context { get; set; }
        ApplicationHelper AppService { get; set; }
        string TituloListado { get; }
        string IdListado { get; }
        string Empresa { get; set; }
        List<string> Condiciones { get; set; }
        string WebIdListado { get; }
        string Select { get; }
    }
}
