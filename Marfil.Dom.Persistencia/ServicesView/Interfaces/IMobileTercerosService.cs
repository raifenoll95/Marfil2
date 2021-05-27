using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.BusquedaTerceros;

namespace Marfil.Dom.Persistencia.ServicesView.Interfaces
{
    public interface IMobileTercerosService:IDisposable
    {
        IEnumerable<IItemResultadoMovile> BuscarTercero(string cuenta);
    }
}
