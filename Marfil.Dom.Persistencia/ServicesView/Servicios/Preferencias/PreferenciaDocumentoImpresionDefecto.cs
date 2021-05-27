using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias
{
   
    [Serializable]
    public class PreferenciaDocumentoImpresionDefecto : ITipoPreferencia
    {
        public const string Id = "DocumentoImpresionDefecto";
        public const string Nombre = "Defecto";

        public TiposPreferencias Tipo => TiposPreferencias.DocumentoImpresionDefecto;

        public Guid Usuario { get; set; }

        public TipoDocumentoImpresion Tipodocumento { get; set; }

        public string Name { get; set; }

        
    }
}
