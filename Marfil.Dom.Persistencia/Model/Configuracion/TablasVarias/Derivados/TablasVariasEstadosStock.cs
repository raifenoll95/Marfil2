using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados
{
    public class TablasVariasEstadosStock : TablasVariasGeneralModel
    {
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "EnAlmacen")]
        public bool EnAlmacen { get; set; }
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.TablasVarias), Name = "Disponible")]
        public bool Dispobible { get; set; }
    }
}
