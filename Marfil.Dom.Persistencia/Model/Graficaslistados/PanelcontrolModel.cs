using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model.Interfaces;
using RConfiguraciongraficos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Configuraciongraficas;
namespace Marfil.Dom.Persistencia.Model.Graficaslistados
{
   

    public class PanelcontrolModel 
    {
        #region Properties

        public List<ConfiguraciongraficasModel> Paneles { get; set; }

        #endregion
    }
}
