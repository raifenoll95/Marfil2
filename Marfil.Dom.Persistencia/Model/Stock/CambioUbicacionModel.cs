using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using RMovimientosalmacen=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movimientosalmacen;

namespace Marfil.Dom.Persistencia.Model.Stock
{
    public class CambioUbicacionModel : IToolbar
    {
        #region Members

        private ToolbarModel _toolbar;

        #endregion

        public string TituloPagina
        {
            get { return "Marfil - " + RMovimientosalmacen.CambioUbicacion; }
        }

        [Required]
        [Display(ResourceType = typeof(RMovimientosalmacen), Name = "Lote")]
        public string Lote { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMovimientosalmacen), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RMovimientosalmacen), Name = "Fkzonaalmacen")]
        public string Fkzonaalmacen { get; set; }

        public ToolbarModel Toolbar
        {
            get { return _toolbar; }
            set { _toolbar = value; }
        }

        public CambioUbicacionModel()
        {
            
        
            _toolbar = new ToolbarCambioUbicacionModel();
        
    }
    }
}
