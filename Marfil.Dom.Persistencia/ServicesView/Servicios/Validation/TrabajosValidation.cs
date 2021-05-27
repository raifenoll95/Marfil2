using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RTrabajos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Trabajos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class TrabajosValidation : BaseValidation<Trabajos>
    {
        public TrabajosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Trabajos model)
        {
            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RTrabajos.Descripcion));

            foreach (var linea in model.TrabajosLin)
            {
                var longitud = linea.año.ToString();
                if (longitud.Length != 4)
                {
                    throw new ValidationException(string.Format(RTrabajos.ErrorFormatoAño, RTrabajos.Año));
                }
                if (linea.año < 2000 || linea.año > 2100)
                {
                    throw new ValidationException(string.Format(RTrabajos.ErrorRangoFecha, RTrabajos.Año));
                }
                if (linea.precio < 0 || linea.precio > 9999999.99)
                {
                    throw new ValidationException(string.Format(RTrabajos.ErrorPrecio, RTrabajos.Precio));
                }
            }

            return base.ValidarGrabar(model);
        }
    }
}
