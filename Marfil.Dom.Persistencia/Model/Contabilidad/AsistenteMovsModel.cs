using System;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System.Web;
using Resources;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;

namespace Marfil.Dom.Persistencia.Model.Contabilidad.Movs
{
    public class ToolbarAsistenteMovsModel: ToolbarModel
    {
        public ToolbarAsistenteMovsModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = General.LblGenerarDiarioContable;
        }

        public override string GetCustomTexto()
        {            
            return General.LblGenerarDiarioContable;
        }
    }

    public class AsistenteMovsModel: IToolbar
    {
        #region Members

        private ToolbarModel _toolbar;

        #endregion

        #region Properties

        public ToolbarModel Toolbar
        {
            get { return _toolbar; }
            set { _toolbar = value; }
        }

        [Display(ResourceType = typeof(General), Name = "LblFichero")]
        public HttpPostedFileBase Fichero { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblDelimitador")]
        public string Delimitador { get; set; }

        [Display(ResourceType = typeof(General), Name = "LblCabecera")]
        public bool Cabecera { get; set; }
    
        [Display(ResourceType = typeof(RMovs), Name = "Fkseriescontables")]
        public string Seriecontable { get; set; }

        #endregion

        public AsistenteMovsModel()
        {
            _toolbar = new ToolbarAsistenteMovsModel();
        }
    }
}
