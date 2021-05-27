using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class TiposIvaValidation : BaseValidation<TiposIva>
    {
        public TiposIvaValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

       
    }
}
