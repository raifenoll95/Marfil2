using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Stock;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
   
    internal interface IStockService
    {
        MarfilEntities Db { get;  set; }
        TipoOperacionService Tipooperacion { get; set; }
        
        void GenerarOperacion(IStockPieza movimiento);

    }
}
