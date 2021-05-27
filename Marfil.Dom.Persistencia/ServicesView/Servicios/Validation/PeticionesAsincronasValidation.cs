using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class PeticionesAsincronasValidation : BaseValidation<PeticionesAsincronas>
    {
        public PeticionesAsincronasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(PeticionesAsincronas model)
        {
            return base.ValidarGrabar(model);
        }

    }
}
