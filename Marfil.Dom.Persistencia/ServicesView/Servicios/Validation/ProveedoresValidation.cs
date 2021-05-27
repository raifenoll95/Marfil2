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
using RProveedores= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class ProveedoresValidation : BaseValidation<Proveedores>
    {
        public ProveedoresValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Proveedores model)
        {
            if (string.IsNullOrEmpty(model.fkcuentas))
                throw new ValidationException(string.Format(General.ErrorCuentaObligatoria));

            if (ValidaCuentaBloqueada(model))
                throw new ValidationException(General.ErrorModificarRegistroBloqueado);

            if (!ValidaCuentaTipoTercero(model))
                throw new ValidationException(General.ErrorTipoCuentaIncorrecto);

            if(string.IsNullOrEmpty(model.fkidiomas))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio,RProveedores.Fkidiomas));

            if (string.IsNullOrEmpty(model.fkregimeniva))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RProveedores.Fkregimeniva));

            if (!ValidaFromatoPeriodosPagos(model.periodonopagodesde))
                throw new ValidationException(string.Format(General.ErrorFormatoCampo, RProveedores.Periodonopagodesde));

            if (!ValidaFromatoPeriodosPagos(model.periodonopagohasta))
                throw new ValidationException(string.Format(General.ErrorFormatoCampo, RProveedores.Periodonopagohasta));

            ModificaTipoCuenta(model);

            return base.ValidarGrabar(model);
        }

        private bool ValidaFromatoPeriodosPagos(string periodo)
        {
            var result = true;

            if (!string.IsNullOrEmpty(periodo))
            {
                result = false;
                var expresion=new Regex("^[0-9][0-9]/[0-9][0-9]$");
                if (expresion.IsMatch(periodo))
                {
                    DateTime dummyfecha;
                    result = DateTime.TryParse(periodo + "/2015", out dummyfecha);//2015 porque no es bisiesto y asi usamos febrero solo con 29
                }
            }

            return result;
        }

        private void ModificaTipoCuenta(Proveedores model)
        {
            var cuenta = _db.Cuentas.FirstOrDefault(
                f =>
                    f.empresa == model.empresa && f.id == model.fkcuentas &&
                    f.tipocuenta != (int)TiposCuentas.Proveedores);
            if (cuenta != null)
            {
                throw new ValidationException(string.Format(General.ErrorCuentaExistente, Funciones.GetEnumByStringValueAttribute((TiposCuentas)cuenta.tipocuenta), model.fkcuentas));
            }
        }

        private bool ValidaCuentaBloqueada(Proveedores model)
        {
            var cuentabloqueada = _db.Cuentas.SingleOrDefault(f => f.empresa == model.empresa && f.id == model.fkcuentas)?.bloqueada;
            return cuentabloqueada.HasValue && cuentabloqueada.Value;
        }

        private bool ValidaCuentaTipoTercero(Proveedores model)
        {
            var cuenta = _db.Tiposcuentas.SingleOrDefault(f => f.tipos == (int)TiposCuentas.Proveedores && f.empresa==model.empresa)?.cuenta;
            if (!string.IsNullOrEmpty(cuenta))
            {
                return model.fkcuentas.StartsWith(cuenta);
            }

            throw new ValidationException(General.ErrorTipoCuentaConfiguracion);
        }
    }
}
