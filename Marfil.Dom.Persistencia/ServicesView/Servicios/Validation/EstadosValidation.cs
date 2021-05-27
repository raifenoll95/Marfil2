using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using REstados= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Estados;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class EstadosValidation : BaseValidation<Estados>
    {
        public EstadosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Estados model)
        {

            if(!model.tipoestado.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REstados.Tipoestado));

            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REstados.Descripcion));

            return base.ValidarGrabar(model);
        }
    }
}
