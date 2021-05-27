using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RAgentes= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Agentes;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class AgentesValidation : BaseValidation<Agentes>
    {
        public AgentesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Agentes model)
        {
            if (string.IsNullOrEmpty(model.fkcuentas))
                throw new ValidationException(RAgentes.ErrorCuentaObligatoria);

           if (ValidaCuentaBloqueada(model))
               throw new ValidationException(General.ErrorModificarRegistroBloqueado);

            if (!ValidaCuentaTipoTercero(model))
                throw new ValidationException(General.ErrorTipoCuentaIncorrecto);

            ModificaTipoCuenta(model);

            return base.ValidarGrabar(model);
        }

        private bool ValidaCuentaBloqueada(Agentes model)
        {
            var cuentabloqueada = _db.Cuentas.SingleOrDefault(f => f.empresa == model.empresa && f.id == model.fkcuentas)?.bloqueada;
            return cuentabloqueada.HasValue && cuentabloqueada.Value;
        }

        private bool ValidaCuentaTipoTercero(Agentes model)
        {
            var cuenta = _db.Tiposcuentas.SingleOrDefault(f => f.tipos == (int)TiposCuentas.Agentes && f.empresa==model.empresa)?.cuenta;
            if (!string.IsNullOrEmpty(cuenta))
            {
                return model.fkcuentas.StartsWith(cuenta);
            }

            throw new ValidationException(string.Format(General.ErrorTipoCuentaConfiguracion, RAgentes.TituloEntidad));
        }

        private void ModificaTipoCuenta(Agentes model)
        {
            var cuenta = _db.Cuentas.FirstOrDefault(
                 f =>
                     f.empresa == model.empresa && f.id == model.fkcuentas &&
                     f.tipocuenta != (int)TiposCuentas.Agentes);
            if (cuenta != null)
            {
                throw new ValidationException(string.Format(General.ErrorCuentaExistente, Funciones.GetEnumByStringValueAttribute((TiposCuentas)cuenta.tipocuenta), model.fkcuentas));
            }
        }
    }
}
