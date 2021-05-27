using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados
{
    [Serializable]
   public  class TablasVariasModosPagoModel: TablasVariasGeneralModel
    {
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Efectivo")]
        public bool Efectivo { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Remesable")]
        public bool Remesable { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "MandatoRequerido")]
        public bool MandatoRequerido { get; set; }
    }
}
