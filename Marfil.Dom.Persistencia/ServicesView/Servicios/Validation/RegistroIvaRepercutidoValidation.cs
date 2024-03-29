﻿using System;
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

            ValidarFechas(model);
            ValidarCuentacliente(model);
            ValidarTipoFacturaGrid(model);
            ValidarTipoRetencion(model);
            ValidarCtaTesoreria(model);

            return base.ValidarGrabar(model);
        }

        private void ValidarTipoFacturaGrid(RegistroIVARepercutido model)
        {
            var tipofactura = model.tipofactura;
            var serviceCuentas = new CuentasService(Context, MarfilEntities.ConnectToSqlServer(Context.BaseDatos));
            var list = serviceCuentas.GetCuentasContablesNivel(0);
            var abono = serviceCuentas.GetCuentaAbono1(1, tipofactura);
            list = list.Where(f => f.Id.StartsWith(abono));

            foreach (var item in model.RegistroIVARepercutidoTotales)
            {
                if (item.idtipofactura != tipofactura)
                {
                    throw new ValidationException("El tipo de factura de una de las líneas no corresponde con la indicada.");
                }

                if (!list.Any(f => f.Id == item.cuentaventas))
                {
                    throw new ValidationException("La cuenta de venta " + item.cuentaventas + " no es válida para el tipo de factura indicado");
                }
            }
        }

        private void ValidarCuentacliente(RegistroIVARepercutido model)
        {
            var tipofactura = model.tipofactura;
            var cuentaCliente = model.cuentacliente;
            var serviceCuentas = new CuentasService(Context, MarfilEntities.ConnectToSqlServer(Context.BaseDatos));

            var list = serviceCuentas.GetCuentasContablesNivel(0);
            var cargo = serviceCuentas.GetCuentaCargo1(1, tipofactura);
            list = list.Where(f => f.Id.StartsWith(cargo));

            if (!list.Any(f => f.Id == cuentaCliente))
            {
                throw new ValidationException("La cuenta cliente " + cuentaCliente + " no es válida para el tipo de factura indicado");
            }

        }

        public bool ValidarFechas(RegistroIVARepercutido model)
        {
            if (model.fechaoperacion > model.fechafactura || model.fechaoperacion > model.fecharegistro)
            {
                throw new ValidationException("La fecha de operación no puede ser mayor que la fecha de factura o que la fecha de registro.");
            }
            else if (model.fechafactura > model.fecharegistro)
            {
                throw new ValidationException("La fecha de factura no puede ser mayor a la fecha de registro.");
            }

            return true;
        }

        private void ValidarTipoRetencion(RegistroIVARepercutido model)
        {
            var tipofactura = model.tipofactura;
            var service = new TiposFacturasIvaService(Context, MarfilEntities.ConnectToSqlServer(Context.BaseDatos));

            var requiereIRPF = service.RequiereIRPF(Context.Empresa, tipofactura);

            if (requiereIRPF && string.IsNullOrEmpty(model.fktiporetencion))
            {
                throw new ValidationException("No se ha indicado el tipo de retención y la configuración del tipo de factura seleccionado lo requiere");
            }

        }

        private void ValidarCtaTesoreria(RegistroIVARepercutido model)
        {
            if (model.contabilizar.Value && string.IsNullOrEmpty(model.fkcuentastesoreria))
            {
                throw new ValidationException("Se debe indicar la Cuenta de tesorería si el registro se va a contabilizar");
            }

        }

        private void ValidarPorcentajeRetencion(RegistroIVARepercutido model)
        {
            if (!string.IsNullOrEmpty(model.fktiporetencion) && model.porcentajeretencion == null)
            {
                throw new ValidationException("Se debe indicar un porcentaje de retención si se ha seleccionado un tipo de retención");
            }

        }
    }
}
