using System.Collections.Generic;

namespace Marfil.Dom.ControlsUI.CampoVerificacion
{
    public class Configuracion
    {
        public string AnchoTexto { get; set; }
        public bool OcultarTexto { get; set; }
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

    public class CampoverificacionModel
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
        public string Params { get; set; }
        public string Longitud { get; set; }
        public string Tipo { get; set; }
        public string ControlesAsociados { get; set; }
        public string Columnabloqueados { get; set; }
        public bool ExcludeSubmit { get; set; }
        private Configuracion _configuracion=new Configuracion() { AnchoTexto = "col-md-2 col-xs-2"};
        

        public Configuracion Configuracion
        {
            get { return _configuracion; }
            set { _configuracion = value; }
        }
        private SettingsVerificacionPk _settingsVerificacion = new SettingsVerificacionPk();
        

        public SettingsVerificacionPk SettingsVerificacion
        {
            get { return _settingsVerificacion; }
            set { _settingsVerificacion = value; }
        }

        public bool AltaEnlazada { get; set; }

        private AltaEnlazadaSettings _altaSettings = new AltaEnlazadaSettings();
        public AltaEnlazadaSettings AltaSettings
        {
            get { return _altaSettings; }
            set { _altaSettings = value; }
        }
    }
}