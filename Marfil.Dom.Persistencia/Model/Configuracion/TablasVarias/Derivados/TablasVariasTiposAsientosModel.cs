using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados
{
    public class TablasVariasTiposAsientosModel : TablasVariasGeneralModel
    {
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Defecto")]
        public bool Defecto { get; set; }
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "MovAutomatico")]
        public bool MovAutomatico { get; set; }
    }
}

