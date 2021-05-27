using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class GruposIvaValidation : BaseValidation<GruposIva>
    {
        public GruposIvaValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }
    }
}
