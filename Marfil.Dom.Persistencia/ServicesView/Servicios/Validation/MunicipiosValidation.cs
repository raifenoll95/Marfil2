using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class MunicipiosValidation : BaseValidation<Municipios>
    {
        public MunicipiosValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Municipios model)
        {
            model.cod = Funciones.RellenaCod(model.cod, 3);
            return base.ValidarGrabar(model);
        }
    }
}
