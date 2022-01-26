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
    public class ToolbarAsistenteRervertirEstadoEjercicioModel : ToolbarModel
    {
        public ToolbarAsistenteRervertirEstadoEjercicioModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RCobrosYPagos.RevertirEstado;
        }

        public override string GetCustomTexto()
        {
            return RCobrosYPagos.AsistenteRevertirEjercicio;
        }
    }

    public class AsistenteRervertirEstadoEjercicioModel : IToolbar
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

        [Required]
        [Display(ResourceType = typeof(RConfiguracion), Name = "EstadoEjercicioRevertir")]
        public string EstadoEjercicioRevertir { get; set; }

        #endregion

        public AsistenteRervertirEstadoEjercicioModel()
        {

        }
        public AsistenteRervertirEstadoEjercicioModel(IContextService context)
        {
            _toolbar = new ToolbarAsistenteRervertirEstadoEjercicioModel();
        }
    }
}
