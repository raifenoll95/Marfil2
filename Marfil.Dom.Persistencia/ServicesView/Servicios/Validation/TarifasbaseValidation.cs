using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class TarifasbaseValidation : BaseValidation<Tarifasbase>
    {
        public TarifasbaseValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }
    }
}
