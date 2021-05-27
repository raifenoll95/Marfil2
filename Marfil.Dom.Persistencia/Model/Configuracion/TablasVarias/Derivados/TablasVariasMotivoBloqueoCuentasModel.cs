using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados
{
    
        [Serializable]
        public class TablasVariasMotivoBloqueoCuentasModel : TablasVariasGeneralModel
        {
            [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "BloqueoRGPD")]
            public bool BloqueoRGPD { get; set; }
        }
    
}
