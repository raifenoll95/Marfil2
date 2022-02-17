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
            if (!string.IsNullOrEmpty(model.telefono) && model.telefono.Length > 15)
                throw new ValidationException(string.Format("El teléfono tiene más de 15 caracteres", RDireccion.Telefono));

            return base.ValidarGrabar(model);
        }
    }
}
