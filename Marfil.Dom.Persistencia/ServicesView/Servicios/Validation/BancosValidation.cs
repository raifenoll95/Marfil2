using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RBancos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Bancos;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class BancosValidation : BaseValidation<Bancos>
    {
        #region CTR

        public BancosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(Bancos model)
        {
            if(string.IsNullOrEmpty(model.id))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RBancos.Id));

            if (string.IsNullOrEmpty(model.nombre))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RBancos.Nombre));

            if (string.IsNullOrEmpty(model.bic))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RBancos.Bic));

            if (!ModeloNegocioFunciones.ValidateBic(model.bic))
                throw new ValidationException(string.Format(General.ErrorFormatoCampo, RBancos.Bic));

            return base.ValidarGrabar(model);
        }
    }
}
