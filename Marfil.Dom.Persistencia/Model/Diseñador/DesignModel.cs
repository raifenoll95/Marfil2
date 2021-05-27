using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;


namespace Marfil.Dom.Persistencia.Model.Diseñador
{
    public enum TipoPrivacidadDocumento
    {
        Publico,
        Privado
    }

    public enum TipoReport
    {
        Report,
        Subreport
    }

    public class DesignModel
    {
        public bool Nuevo { get; set; }
        public string ReturnUrl { get; set; }
        public string Url { get; set; }
        public Guid UsuarioId { get; set; }
        public string Usuarionombre { get; set; }
        public TipoDocumentoImpresion Tipodocumento { get; set; }
        public TipoReport Tiporeport { get; set; }
        public TipoPrivacidadDocumento Tipoprivacidad { get; set; }
        public string Name { get; set; }
        public byte[] Report { get; set; }
        public object DataSource { get; set; }
        public Dictionary<string, object> Parameters { get; set; }
    }
}
