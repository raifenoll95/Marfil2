using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Interfaces
{
   

    internal interface ILineasDocumentosBusquedaMovil
    {
        string Fkarticulos { get; }

        string Descripcion { get;  }

        double? Cantidad { get; }

        string SLargo { get; }

        string SAncho { get; }

        string SGrueso { get;  }

        string SMetros { get;  }

        string SPrecio { get;  }

        double? Porcentajedescuento { get;  }

        string SImporte { get; }

    }

    internal interface ITotalesDocumentosBusquedaMovil
    {
        double? Porcentajeiva { get; set; }
        string SBrutototal { get; set; }

        string SBaseimponible { get; set; }
        string SCuotaiva { get; set; }
        string SImporterecargoequivalencia { get; set; }
        string SSubtotal { get; set; }
    }
}
