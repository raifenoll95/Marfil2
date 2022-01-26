using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCobrosYPagos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CobrosYPagos;
using RConfiguracion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Configuracionaplicacion;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Regularizacion
{
    public class ToolbarAsistenteCierreAperturaModel : ToolbarModel
    {
        public ToolbarAsistenteCierreAperturaModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RCobrosYPagos.CierreApertura;
        }

        public override string GetCustomTexto()
        {
            return RCobrosYPagos.AsistenteRegularizacionCierreApertura;
        }
    }

    public class AsistenteCierreAperturaModel : IToolbar
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

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechacierre")]
        public DateTime Fechacierre { get; set; }
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechaapertura")]
        public DateTime Fechaapertura { get; set; }
        [Required]
        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioCierre")]
        public string ComentarioCierre { get; set; }
        [Required]
        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioApertura")]
        public string ComentarioApertura { get; set; }

        #endregion

        public AsistenteCierreAperturaModel()
        {

        }
        public AsistenteCierreAperturaModel(IContextService context)
        {
            _toolbar = new ToolbarAsistenteCierreAperturaModel();
        }
    }
}
