using Marfil.Dom.Persistencia.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class TiposFacturasIvaValidation : BaseValidation<Persistencia.TiposFacturas>
    {
        #region CTR

        public TiposFacturasIvaValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(Persistencia.TiposFacturas model)
        {
            bool descuadrado = false;

            if ((String.IsNullOrEmpty(model.cuentacargo1) && model.importecuentacargo1 != 0) || (!String.IsNullOrEmpty(model.cuentacargo1) && model.importecuentacargo1 == 0))
            {
                descuadrado = true;
            }
            if ((String.IsNullOrEmpty(model.cuentacargo2) && model.importecuentacargo2 != 0 && model.tipocuenta == 3) || (!String.IsNullOrEmpty(model.cuentacargo2) && model.importecuentacargo2 == 0 && model.tipocuenta == 3))
            {
                descuadrado = true;
            }
            if ((String.IsNullOrEmpty(model.cuentacargo3) && model.importecuentacargo3 != 0 && model.tipocuenta3 == 3) || (!String.IsNullOrEmpty(model.cuentacargo3) && model.importecuentacargo3 == 0 && model.tipocuenta3 == 3))
            {
                descuadrado = true;
            }
            if ((String.IsNullOrEmpty(model.cuentaabono1) && model.importecuentaabono1 != 0) || (!String.IsNullOrEmpty(model.cuentaabono1) && model.importecuentaabono1 == 0))
            {
                descuadrado = true;
            }
            if ((String.IsNullOrEmpty(model.cuentaabono2) && model.importecuentaabono2 != 0 && model.tipoabono2 == 3) || (!String.IsNullOrEmpty(model.cuentaabono2) && model.importecuentaabono2 == 0 && model.tipoabono2 == 3))
            {
                descuadrado = true;
            }
            if ((String.IsNullOrEmpty(model.cuentaabono3) && model.importecuentaabono3 != 0 && model.tipoabono3 == 3) || (!String.IsNullOrEmpty(model.cuentaabono3) && model.importecuentaabono3 == 0 && model.tipoabono2 == 3))
            {
                descuadrado = true;
            }

            if (descuadrado)
            {
                throw new ValidationException("Una cuenta no vacía debe tener asignado un importe asignado, y viceversa");
            }

            if (model.tipofacturadefecto.Value && model.empresa == Context.Empresa) {
                if (_db.TiposFacturas.Where(f=> f.empresa == Context.Empresa && f.tipocircuito == model.tipocircuito && f.tipofacturadefecto == true && f.id != model.id).FirstOrDefault() != null)
                {
                    throw new ValidationException("Ya existe un tipo de factura marcado por defecto para el mismo que tipo que ha intentado guardar, solo puede existir uno");
                }
            }

            return base.ValidarGrabar(model);
        }
    }
}
