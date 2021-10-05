using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class TransformacioneslotesnaveValidation : BaseValidation<Transformacioneslotesnave>
    {
        public TransformacioneslotesnaveValidation(IContextService context, MarfilEntities db) : base(context, db)
        {

        }
    }
}
