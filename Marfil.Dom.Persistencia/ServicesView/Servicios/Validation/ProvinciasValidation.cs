using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class ProvinciasValidation : BaseValidation<Provincias>
    {
        public ProvinciasValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Provincias model)
        {
            model.id = Funciones.RellenaCod(model.id, 2);
            return base.ValidarGrabar(model);
        }
    }
}
