using Marfil.Dom.Persistencia.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RBancosMandatos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.BancosMandatos;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class BancosMandatosValidation : BaseValidation<BancosMandatos>
    {
        public BancosMandatosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(BancosMandatos model)
        {
            //validar bancos
            if (string.IsNullOrEmpty(model.fkcuentas))
                throw new ValidationException(General.ErrorCuentaObligatoria);
            if(string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RBancosMandatos.Descripcion));
            if(string.IsNullOrEmpty(model.fkpaises))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RBancosMandatos.Fkpaises));
            if (string.IsNullOrEmpty(model.iban))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RBancosMandatos.Iban));
            if (string.IsNullOrEmpty(model.bic))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RBancosMandatos.Bic));
            if (!ModeloNegocioFunciones.ValidateIban(model.iban))
                throw new ValidationException(string.Format(General.ErrorFormatoCampo, RBancosMandatos.Iban));

            if (!ModeloNegocioFunciones.ValidateBic(model.bic))
                throw new ValidationException(string.Format(General.ErrorFormatoCampo, RBancosMandatos.Bic));
            
            if (!string.IsNullOrEmpty(model.idacreedor) && !ModeloNegocioFunciones.ValidateIdAcreedor(model.idacreedor))
            {
                throw new ValidationException(string.Format(General.ErrorIdAcreedor, model.idacreedor));
            }

            //validar mandatos
            return base.ValidarGrabar(model);
        }
    }
}
