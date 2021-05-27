using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Interfaces
{
    public interface IMovimientosAlmacen
    {
        void GenerarMovimientoAlmacen(string lote, string fkalmacen, string fkzona);
    }
}
