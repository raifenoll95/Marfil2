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
    public class ToolbarAsistenteAperturaProvisionalModel : ToolbarModel
    {
        public ToolbarAsistenteAperturaProvisionalModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RCobrosYPagos.AperturaProvisional;
        }

        public override string GetCustomTexto()
        {
            return RCobrosYPagos.AsistenteAperturaProvisional;
        }
    }

    public class AsistenteAperturaProvisionalModel : IToolbar
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

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechaapertura")]
        public DateTime Fechaapertura { get; set; }
        [Required]
        [Display(ResourceType = typeof(RConfiguracion), Name = "CuentaDesde")]
        public string CuentaDesde { get; set; }
        [Required]
        [Display(ResourceType = typeof(RConfiguracion), Name = "CuentaHasta")]
        public string CuentaHasta { get; set; }
        [Required]
        [Display(ResourceType = typeof(RConfiguracion), Name = "CuentaReapertura")]
        public string CuentaReapertura { get; set; }
        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioAperturaProvisional")]
        public string ComentarioAperturaProvisional { get; set; }

        #endregion

        public AsistenteAperturaProvisionalModel()
        {

        }
        public AsistenteAperturaProvisionalModel(IContextService context)
        {
            _toolbar = new ToolbarAsistenteAperturaProvisionalModel();
        }
    }
}
