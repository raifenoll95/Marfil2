using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class GrupoMaterialesValidation : BaseValidation<GrupoMateriales>
    {
        #region ctr
        public GrupoMaterialesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }
        #endregion

        public string EjercicioId { get; set; }

        public override bool ValidarGrabar(GrupoMateriales model)
        {
            
            return true;

        }
    }
}
