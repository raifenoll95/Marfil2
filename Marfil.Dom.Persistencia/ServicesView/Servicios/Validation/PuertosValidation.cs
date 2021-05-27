using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RPuertos= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Puertos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class PuertosValidation : BaseValidation<Puertos>
    {
        public PuertosValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Puertos model)
        {
            model.id = Funciones.RellenaCod(model.id, 4);

            if(string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPuertos.Descripcion));

            return base.ValidarGrabar(model);
        }
    }
}
