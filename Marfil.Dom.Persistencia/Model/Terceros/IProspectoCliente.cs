using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.Model.Terceros
{
    
    public interface IProspectoCliente
    {
        string Fkcuentas { get; }
        string Descripcion { get;  }
        string Direccion { get;  }
        string Poblacion { get; }
        string Cp { get;}
        string Pais { get;  }
        string Provincia { get;  }
        string Telefono { get;  }
        string Fax { get;  }
        string Email { get;  }
        string Nif { get;  }
        double Descuentocomercial { get; }
        double Descuentoprontopago { get;  }
        string Fktransportistahabitual { get;  }
        string Fkregimeniva { get;  }
        string Fkincoterm { get; }
        string Fkunidadnegocio { get; }
        int? Moneda { get; }
        PuertoscontrolModel Fkpuertos { get; }
        bool Bloqueado { get; }
        bool EsProspecto { get; }
        string Fktarifas { get; }
    }

    public interface IProveedorAcreedor
    {
        string Fkcuentas { get; }
        string Descripcion { get; }
        string Direccion { get; }
        string Poblacion { get; }
        string Cp { get; }
        string Pais { get; }
        string Provincia { get; }
        string Telefono { get; }
        string Fax { get; }
        string Email { get; }
        string Nif { get; }
        double Descuentocomercial { get; }
        double Descuentoprontopago { get; }
        string Fktransportistahabitual { get; }
        string Fkregimeniva { get; }
        string Fkincoterm { get; }
        string Fkunidadnegocio { get; }
        int Fkmonedas { get; }
        PuertoscontrolModel Fkpuertos { get; }
        bool Bloqueado { get; }
        string Tarifa { get; }
    }
}
