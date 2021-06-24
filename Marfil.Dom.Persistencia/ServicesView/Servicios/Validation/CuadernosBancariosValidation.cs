using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class CuadernosBancariosValidation : BaseValidation<CuadernosBancarios>
    {
        public CuadernosBancariosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }
    }
}
