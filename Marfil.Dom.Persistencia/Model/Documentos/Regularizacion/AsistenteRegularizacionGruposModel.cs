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
    public class ToolbarAsistenteRegularizacionGruposModel : ToolbarModel
    {
        public ToolbarAsistenteRegularizacionGruposModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RCobrosYPagos.GenerarRegularizacionExistencias;
        }

        public override string GetCustomTexto()
        {
            return RCobrosYPagos.AsistenteRegularizacionGrupos;
        }
    }
    public class AsistenteRegularizacionGruposModel : IToolbar
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

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fecharegularizacion")]
        public DateTime Fecharegularizacion { get; set; }
        [Display(ResourceType = typeof(RMovs), Name = "Fkseriescontables")]
        public string Fkseriescontables { get; set; }
        [Required]
        [Display(ResourceType = typeof(RConfiguracion), Name = "CuentaPYG")]
        public string CuentaPYG { get; set; }
        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioDebePYG")]
        public string ComentarioDebePYG { get; set; }
        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioHaberPYG")]
        public string ComentarioHaberPYG { get; set; }
        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioCuentasDetalle")]
        public string ComentarioCuentasDetalle { get; set; }

        #endregion

        public AsistenteRegularizacionGruposModel()
        {

        }
        public AsistenteRegularizacionGruposModel(IContextService context)
        {
            _toolbar = new ToolbarAsistenteRegularizacionGruposModel();
        }
    }
}
