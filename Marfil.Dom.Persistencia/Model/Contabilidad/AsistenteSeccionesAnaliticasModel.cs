using System;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System.Web;
using Resources;
using System.Web.Mvc;
using System.Collections.Generic;

namespace Marfil.Dom.Persistencia.Model.Contabilidad
{
    public class ToolbarAsistenteSeccionesAnaliticasModel : ToolbarModel
    {
        public ToolbarAsistenteSeccionesAnaliticasModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = General.LblGenerarSecciones;
        }

        public override string GetCustomTexto()
        {
            return General.LblGenerarSecciones;
        }
    }

    public class AsistenteSeccionesAnaliticasModel : IToolbar
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


        #endregion

        public AsistenteSeccionesAnaliticasModel()
        {
            _toolbar = new ToolbarAsistenteSeccionesAnaliticasModel();
        }
    }
}
