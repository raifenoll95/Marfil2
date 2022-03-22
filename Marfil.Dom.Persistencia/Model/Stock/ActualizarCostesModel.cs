using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RActualizacionCostes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ActualizacionCostes;

namespace Marfil.Dom.Persistencia.Model.Stock
{
    public class ToolbarActualizarCostesModel : ToolbarModel
    {
        public ToolbarActualizarCostesModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RActualizacionCostes.ActualizarCostes;
            CustomAction = true;
            CustomActionName = "ActualizarCostes";
        }

        public override string GetCustomTexto()
        {
            return RActualizacionCostes.ActualizarCostes;
        }
    }

    public class ActualizarCostesModel : IToolbar
    {
        #region Members

        private List<string> _series = new List<string>();
        private ToolbarModel _toolbar;

        #endregion

        public string TituloPagina
        {
            get { return RActualizacionCostes.ActualizarCostes; }
        }

        [Required]
        [Display(ResourceType = typeof(RActualizacionCostes), Name = "SeriesRecepciones")]
        public List<string> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        [Display(ResourceType = typeof(RActualizacionCostes), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RActualizacionCostes), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RActualizacionCostes), Name = "ArticulosDesde")]
        public string ArticulosDesde { get; set; }

        [Display(ResourceType = typeof(RActualizacionCostes), Name = "ArticulosHasta")]
        public string ArticulosHasta { get; set; }

        public ToolbarModel Toolbar
        {
            get { return _toolbar; }
            set { _toolbar = value; }
        }

        public ActualizarCostesModel()
        {

        }
        public ActualizarCostesModel(IContextService context)
        {
            _toolbar = new ToolbarActualizarCostesModel();
        }
      
    }
}
