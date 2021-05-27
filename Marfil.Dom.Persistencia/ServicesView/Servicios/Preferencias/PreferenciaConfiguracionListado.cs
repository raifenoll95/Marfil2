using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias
{
    public interface IPreferenciaConfiguracionListado
    {
        string SettingsDevexpress { get; set; }
    }

    [Serializable]
    public class PreferenciaConfiguracionListado : ITipoPreferencia, IPreferenciaConfiguracionListado
    {
        public TiposPreferencias Tipo => TiposPreferencias.ConfiguracionListado;

        public string SettingsDevexpress { get; set; }
    }
}
