using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RMateriales= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Materiales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class MaterialesValidation : BaseValidation<Materiales>
    {
        public MaterialesValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Materiales model)
        {
            if(string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RMateriales.Descripcion));

            return base.ValidarGrabar(model);
        }
    }
}
