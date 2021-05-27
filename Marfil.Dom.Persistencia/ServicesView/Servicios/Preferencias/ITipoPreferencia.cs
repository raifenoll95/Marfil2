using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias
{
    public enum TiposPreferencias
    {
        EmpresaDefecto,
        ConfiguracionListado,
        DocumentoImpresionDefecto,
        EjercicioDefecto,
        AlmacenDefecto,
        PanelControlDefecto,
        DiarioContableImpresionDefecto

    }

    public interface ITipoPreferencia
    {
        TiposPreferencias Tipo { get; }
    }
}
