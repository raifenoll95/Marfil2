using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.ControlsUI.Campoidentificador
{
    public class Configuracion
    {
        public string AnchoTexto { get; set; }
    }

    public class AltaEnlazadaSettings
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ReturnUrl { get; set; }
    }

    public class SettingsVerificacionPk
    {
        public string UrlVerificacion { get; set; }
        public string UrlEdicion { get; set; }
    }

    public class CampoidentificadorModel
    {
        public string DisplayName { get; set; }
        public string Titulo { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string CampoIdentificador { get; set; }
        public string CampoDescripcion { get; set; }
        public IEnumerable<string> ColumnasExcuidas { get; set; }
        public string Valor { get; set; }
        public bool Obligatorio { get; set; }
        public bool SoloLectura { get; set; }
        public bool Autofocus { get; set; }
        public string Mascara { get; set; }
        public string Params { get; set; }
        public string Rellenacod { get; set; }
        public bool EditarEnPagina { get; set; }

        public bool PermiteBuscar
        {
            get { return _permiteBuscar; }
            set { _permiteBuscar = value; }
        }

        public bool PermiteExistentes
        {
            get { return _permiteExistentes; }
            set { _permiteExistentes = value; }
        }

        private Configuracion _configuracion = new Configuracion() { AnchoTexto = "col-md-2 col-xs-2" };


        public Configuracion Configuracion
        {
            get { return _configuracion; }
            set { _configuracion = value; }
        }
        private SettingsVerificacionPk _settingsVerificacion = new SettingsVerificacionPk();
        private bool _permiteExistentes = true;
        private bool _permiteBuscar =true;


        public SettingsVerificacionPk SettingsVerificacion
        {
            get { return _settingsVerificacion; }
            set { _settingsVerificacion = value; }
        }

     
    }
}
