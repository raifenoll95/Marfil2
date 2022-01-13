using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCobrosYPagos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CobrosYPagos;
using RConfiguracion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Configuracionaplicacion;

namespace Marfil.Dom.Persistencia.Model.Documentos.RegularizacionExistencias
{
    public class ToolbarAsistenteRegularizacionExistenciasModel : ToolbarModel
    {
        public ToolbarAsistenteRegularizacionExistenciasModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RCobrosYPagos.GenerarRegularizacionExistencias;
        }

        public override string GetCustomTexto()
        {
            return RCobrosYPagos.AsistenteRegularizacionExistencias;
        }
    }
    public class AsistenteRegularizacionExistenciasModel : IToolbar
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
        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioExistenciasIniciales")]
        public string ComentarioExistenciasIniciales { get; set; }
        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioExistenciasFinales")]
        public string ComentarioExistenciasFinales { get; set; }

        #endregion

        public AsistenteRegularizacionExistenciasModel()
        {
        
        }
        public AsistenteRegularizacionExistenciasModel(IContextService context)
        {
            _toolbar = new ToolbarAsistenteRegularizacionExistenciasModel();
        }
    }
}

