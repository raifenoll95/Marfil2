using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RAseguradoras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Aseguradoras;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class AseguradorasValidation : BaseValidation<Aseguradoras>
    {

        #region CTR

        public AseguradorasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(Aseguradoras model)
        {
            if(string.IsNullOrEmpty(model.fkcuentas))
                throw new ValidationException(General.ErrorCuentaObligatoria);

            if (ValidaCuentaBloqueada(model))
                throw new ValidationException(General.ErrorModificarRegistroBloqueado);

            if (model.fechainicio==null)
                throw new ValidationException(RAseguradoras.ErrorFechaInicio);

            if (model.fechafin == null)
                throw new ValidationException(RAseguradoras.ErrorFechaFin);

            if(model.fechainicio>model.fechafin)
                throw new ValidationException(RAseguradoras.ErrorFechaInicioMayorFechaFin);

            if (!ValidaCuentaTipoTercero(model))
                throw new ValidationException(General.ErrorTipoCuentaIncorrecto);

            ModificaTipoCuenta(model);

           return base.ValidarGrabar(model);
        }

        private void ModificaTipoCuenta(Aseguradoras model)
        {
            var cuenta = _db.Cuentas.FirstOrDefault(
                 f =>
                     f.empresa == model.empresa && f.id == model.fkcuentas &&
                     f.tipocuenta != (int)TiposCuentas.Aseguradoras);
            if (cuenta != null)
            {
                throw new ValidationException(string.Format(General.ErrorCuentaExistente, Funciones.GetEnumByStringValueAttribute((TiposCuentas)cuenta.tipocuenta), model.fkcuentas));
            }
        }

        private bool ValidaCuentaBloqueada(Aseguradoras model)
        {
            var cuentabloqueada = _db.Cuentas.SingleOrDefault(f => f.empresa == model.empresa && f.id == model.fkcuentas)?.bloqueada;
            return cuentabloqueada.HasValue && cuentabloqueada.Value;
        }

        private bool ValidaCuentaTipoTercero(Aseguradoras model)
        {
            var cuenta = _db.Tiposcuentas.SingleOrDefault(f => f.tipos == (int) TiposCuentas.Aseguradoras && f.empresa==model.empresa)?.cuenta;
            if (!string.IsNullOrEmpty(cuenta))
            {
                return model.fkcuentas.StartsWith(cuenta);
            }

            throw new ValidationException(string.Format(General.ErrorTipoCuentaConfiguracion,RAseguradoras.TituloEntidad));
        }
    }
}
