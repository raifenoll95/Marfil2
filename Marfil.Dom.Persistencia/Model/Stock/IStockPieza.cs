using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Stock
{
    public enum TipoPieza
    {
        Pieza,
        Bundle,
        Kit
    }
    public interface IStockPieza
    {

    }

    public interface IStockPiezaSingle : IStockPieza
    {
        TipoPieza Tipopieza { get; set; }
        bool Lotefraccionable { get; set; }
    }

    public interface IKitStockPieza : IStockPieza
    {
        
    }

    public interface IBundleStockPieza : IStockPieza
    {

    }

   
}
