using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RAlmacenes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Almacenes;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class AlmacenesValidation : BaseValidation<Almacenes>
    {
        public AlmacenesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Almacenes model)
        {
            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RAlmacenes.Descripcion));

            return base.ValidarGrabar(model);
        }
    }
}
