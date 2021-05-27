using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RAcabados = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Acabados;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class ConfiguraciongraficasValidation : BaseValidation<Configuraciongraficas>
    {
        public ConfiguraciongraficasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }
    }
}
