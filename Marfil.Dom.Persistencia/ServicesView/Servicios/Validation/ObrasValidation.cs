using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RObras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Obras;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class ObrasValidation : BaseValidation<Obras>
    {
        #region CTR

        public ObrasValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        #endregion

        public override bool ValidarGrabar(Obras model)
        {
            if (string.IsNullOrEmpty(model.nombreobra))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RObras.Nombreobra));

            if (string.IsNullOrEmpty(model.fkclientes))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RObras.Fkclientes));

            if(model.fechainicio.HasValue && model.fechafin.HasValue && model.fechainicio>model.fechafin)
                throw new ValidationException(string.Format(General.ErrorCampo1MayorCampo2, RObras.Fechainicio,RObras.Fechafin));

            return base.ValidarGrabar(model);
        }
    }
}
