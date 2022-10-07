using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class RegistroIvaRepercutidoValidation : BaseValidation<Persistencia.RegistroIVARepercutido>
    {
        #region CTR

        public RegistroIvaRepercutidoValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(RegistroIVARepercutido model)
        {

            ValidarFechaoperacion(model);

            return base.ValidarGrabar(model);
        }

        public bool ValidarFechaoperacion(RegistroIVARepercutido model)
        {
            if (model.fechaoperacion > model.fechafactura)
            {
                throw new ValidationException("La fecha de operación no puede ser mayor a la fecha de factura.");
            }

            return true;
        }
    }
}
