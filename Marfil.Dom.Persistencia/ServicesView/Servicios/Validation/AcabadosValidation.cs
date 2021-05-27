using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RAcabados = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Acabados;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class AcabadosValidation : BaseValidation<Acabados>
    {
        public AcabadosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Acabados model)
        {
            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAcabados.Descripcion));

            return base.ValidarGrabar(model);
        }
    }
}
