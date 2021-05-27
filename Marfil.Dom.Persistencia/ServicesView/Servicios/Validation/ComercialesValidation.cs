using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RComerciales= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Comerciales;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class ComercialesValidation : BaseValidation<Comerciales>
    {
        public ComercialesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Comerciales model)
        {
            if (string.IsNullOrEmpty(model.fkcuentas))
                throw new ValidationException(RComerciales.ErrorCuentaObligatoria);

           if (ValidaCuentaBloqueada(model))
               throw new ValidationException(General.ErrorModificarRegistroBloqueado);

            if (!ValidaCuentaTipoTercero(model))
                throw new ValidationException(General.ErrorTipoCuentaIncorrecto);

            ModificaTipoCuenta(model);

            return base.ValidarGrabar(model);
        }

        private bool ValidaCuentaBloqueada(Comerciales model)
        {
            var cuentabloqueada = _db.Cuentas.SingleOrDefault(f => f.empresa == model.empresa && f.id == model.fkcuentas)?.bloqueada;
            return cuentabloqueada.HasValue && cuentabloqueada.Value;
        }

        private bool ValidaCuentaTipoTercero(Comerciales model)
        {
            var cuenta = _db.Tiposcuentas.SingleOrDefault(f => f.tipos == (int)TiposCuentas.Comerciales && f.empresa== model.empresa)?.cuenta;
            if (!string.IsNullOrEmpty(cuenta))
            {
                return model.fkcuentas.StartsWith(cuenta);
            }

            throw new ValidationException(string.Format(General.ErrorTipoCuentaConfiguracion, RComerciales.TituloEntidad));
        }

        private void ModificaTipoCuenta(Comerciales model)
        {
            var cuenta = _db.Cuentas.FirstOrDefault(
                f =>
                    f.empresa == model.empresa && f.id == model.fkcuentas &&
                    f.tipocuenta != (int)TiposCuentas.Comerciales);
            if (cuenta != null)
            {
                throw new ValidationException(string.Format(General.ErrorCuentaExistente, Funciones.GetEnumByStringValueAttribute((TiposCuentas)cuenta.tipocuenta), model.fkcuentas));
            }
        }
    }
}
