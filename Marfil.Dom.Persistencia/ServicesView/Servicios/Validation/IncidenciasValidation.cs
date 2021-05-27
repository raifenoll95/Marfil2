using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RIncidencias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Incidencias;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class IncidenciasValidation : BaseValidation<Incidencias>
    {
        public IncidenciasValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Incidencias model)
        {
            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RIncidencias.Descripcion));

            return base.ValidarGrabar(model);
        }
    }
}
