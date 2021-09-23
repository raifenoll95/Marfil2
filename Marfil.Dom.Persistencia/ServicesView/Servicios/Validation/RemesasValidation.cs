using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class RemesasValidation : BaseValidation<Remesas>
    {
        #region CTR
        public RemesasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(Remesas model)
        {
            return base.ValidarGrabar(model);
        }
    }
}
