using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class PlanesGeneralesValidation : BaseValidation<Planesgenerales>
    {
        public PlanesGeneralesValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Planesgenerales model)
        {
            if (string.IsNullOrEmpty(model.nombre))
                throw new ValidationException("El campo Nombre es obligatorio");

            if (string.IsNullOrEmpty(model.fichero))
                throw new ValidationException("El campo Fichero es obligatorio");

            return base.ValidarGrabar(model);
        }
    }
}
