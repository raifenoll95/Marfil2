using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Interfaces
{
    internal interface IValidationService<in T>
    {
        List<string> WarningList { get; }
        MarfilEntities Db { get; set; }
        bool ValidarGrabar(T model);
        bool ValidarBorrar(T model);
    }
}
