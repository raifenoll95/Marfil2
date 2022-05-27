using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class RegistroIvaRepercutidoValidation : BaseValidation<Persistencia.RegistroIVARepercutido>
    {
        #region CTR

        public RegistroIvaRepercutidoValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion
    }
}
