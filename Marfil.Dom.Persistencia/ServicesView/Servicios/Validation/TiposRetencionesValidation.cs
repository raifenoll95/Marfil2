using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class TiposRetencionesValidation : BaseValidation<Tiposretenciones>
    {
        public TiposRetencionesValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }
    }
}
