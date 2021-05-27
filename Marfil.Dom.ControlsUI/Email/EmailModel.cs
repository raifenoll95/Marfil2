using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.ControlsUI.Email
{
    public enum TipoFicherosEmail
    {
        Local,
        Url
    }
    [Serializable]
    public class FicherosEmailModel
    {
        public TipoFicherosEmail Tipo { get; set; }
        public string Nombre { get; set; }
        public string Url { get; set; }

    }
    [Serializable]
    public class EmailModel
    {
        private List<FicherosEmailModel> _ficheros = new List<FicherosEmailModel>();

        public int Id{ get; set; }
        public int Tipo { get; set; }

        public string Tituloformulario { get; set; }
        public string Destinatario { get; set; }
        public string Destinatarioerror { get; set; }

        public bool PermiteCc { get; set; }
        public string PermiteBcc { get; set; }

        public string Asunto { get; set; }
        public string Asuntoerror { get; set; }

        public string Remitente { get; set; }
        public string Remitenteerror { get; set; }

        public string DestinatarioCc { get; set; }
        public string DestinatarioCcerror { get; set; }

        public string DestinatarioBcc { get; set; }
        public string DestinatarioBccerror { get; set; }

        public string Contenido { get; set; }

        public List<FicherosEmailModel> Ficheros
        {
            get { return _ficheros; }
            set { _ficheros = value; }
        }
    }
}
