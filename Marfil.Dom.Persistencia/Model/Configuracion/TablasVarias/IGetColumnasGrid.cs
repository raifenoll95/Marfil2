using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Busquedas;

namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias
{
    public interface IGetColumnasGrid
    {
        IEnumerable<ColumnDefinition> GetColumnDefinitions();
    }
}
