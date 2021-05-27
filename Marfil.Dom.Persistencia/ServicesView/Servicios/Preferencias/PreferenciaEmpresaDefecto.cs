using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias
{
    public interface IPreferenciaEmpresaDefecto
    {
        string Empresa { get; set; }
    }

    [Serializable]
    public class PreferenciaEmpresaDefecto : ITipoPreferencia, IPreferenciaEmpresaDefecto
    {
        public const string Id = "EmpresaDefecto";
        public const string Nombre = "Defecto";

        public TiposPreferencias Tipo => TiposPreferencias.EmpresaDefecto;

        public string Empresa { get; set; }
    }
}
