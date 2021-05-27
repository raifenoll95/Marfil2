using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class CostesVariablesPeriodoValidation : BaseValidation<CostesVariablesPeriodo>
    {
        #region constructor
        public CostesVariablesPeriodoValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }
        #endregion
    }
}
