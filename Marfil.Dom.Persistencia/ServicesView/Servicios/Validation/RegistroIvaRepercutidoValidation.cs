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
            ValidarTipoFacturaGrid(model);

            return base.ValidarGrabar(model);
        }

        private void ValidarTipoFacturaGrid(RegistroIVARepercutido model)
        {
            var tipofactura = model.tipofactura;

            foreach (var item in model.RegistroIVARepercutidoTotales)
            {
                if (item.idtipofactura != tipofactura)
                {
                    throw new ValidationException("El tipo de factura de una de las líneas no corresponde con la indicada.");
                }
            }
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
