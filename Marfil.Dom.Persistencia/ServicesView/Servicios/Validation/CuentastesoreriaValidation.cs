using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Inf.Genericos.Helper;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class CuentastesoreriaValidation : BaseValidation<Cuentastesoreria>
    {
        public CuentastesoreriaValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Cuentastesoreria model)
        {
            if (string.IsNullOrEmpty(model.fkcuentas))
                throw new ValidationException(string.Format(General.ErrorCuentaObligatoria));

            if (ValidaCuentaBloqueada(model))
                throw new ValidationException(General.ErrorModificarRegistroBloqueado);

            if (!ValidaCuentaTipoTercero(model))
                throw new ValidationException(General.ErrorTipoCuentaIncorrecto);

            ModificaTipoCuenta(model);

            return base.ValidarGrabar(model);
        }

        private void ModificaTipoCuenta(Cuentastesoreria model)
        {
            var cuenta = _db.Cuentas.FirstOrDefault(
                 f =>
                     f.empresa == model.empresa && f.id == model.fkcuentas &&
                     f.tipocuenta != (int)TiposCuentas.Cuentastesoreria);
            if (cuenta != null)
            {
                throw new ValidationException(string.Format(General.ErrorCuentaExistente, Funciones.GetEnumByStringValueAttribute((TiposCuentas)cuenta.tipocuenta), model.fkcuentas));
            }
        }

        private bool ValidaCuentaBloqueada(Cuentastesoreria model)
        {
            var cuentabloqueada = _db.Cuentas.SingleOrDefault(f => f.empresa == model.empresa && f.id == model.fkcuentas)?.bloqueada;
            return cuentabloqueada.HasValue && cuentabloqueada.Value;
        }

        private bool ValidaCuentaTipoTercero(Cuentastesoreria model)
        {
            var cuenta = _db.Tiposcuentas.SingleOrDefault(f => f.tipos == (int)TiposCuentas.Cuentastesoreria && f.empresa == model.empresa)?.cuenta;
            if (!string.IsNullOrEmpty(cuenta))
            {
                return model.fkcuentas.StartsWith(cuenta);
            }

            throw new ValidationException(General.ErrorTipoCuentaConfiguracion);
        }
    }
}
