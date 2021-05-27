using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RCarpetas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Ficheros;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class CarpetasValidation : BaseValidation<Carpetas>
    {
        public CarpetasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Carpetas model)
        {
            if (string.IsNullOrEmpty(model.nombre))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RCarpetas.Nombre));

            return base.ValidarGrabar(model);
        }
    }
}
