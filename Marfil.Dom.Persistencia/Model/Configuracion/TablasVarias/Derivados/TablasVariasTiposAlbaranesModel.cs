using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTablasVarias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias;
namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados
{
    public class TablasVariasTiposAlbaranesModel: TablasVariasGeneralModel
    {
        public bool Defecto { get; set; }
        public bool Pidemotivo { get; set; }

        [Display(ResourceType = typeof(RTablasVarias), Name = "SalidasV")]
        public bool Salidas { get; set; }

        [Display(ResourceType = typeof(RTablasVarias), Name = "EntradasV")]
        public bool Entradas { get; set; }

        [Display(ResourceType = typeof(RTablasVarias), Name = "CosteAdq")]
        public bool CosteAdq { get; set; }

        public int EnumInterno { get; set; }
    }
}
