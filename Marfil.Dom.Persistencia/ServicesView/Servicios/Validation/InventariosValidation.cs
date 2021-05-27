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
    internal class InventariosValidation : BaseValidation<Inventarios>
    {
        public InventariosValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Inventarios model)
        {
            model.integridadreferencial = Guid.NewGuid();

            return base.ValidarGrabar(model);
        }
    }
}
