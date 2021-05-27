using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class DireccionesValidation : BaseValidation<Direcciones>
    {
        #region CTR

        public DireccionesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(Direcciones model)
        {
            if(string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RDireccion.Descripcion));

            return base.ValidarGrabar(model);
        }
    }
}
