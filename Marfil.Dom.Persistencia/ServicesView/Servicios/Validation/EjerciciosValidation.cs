using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using REjercicios = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Ejercicios;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class EjerciciosValidation : BaseValidation<Ejercicios>
    {
        public EjerciciosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Ejercicios model)
        {
            if (string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REjercicios.Descripcion));

            if (string.IsNullOrEmpty(model.descripcioncorta))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REjercicios.Descripcioncorta));

            if (!model.desde.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REjercicios.Desde));

            if (!model.hasta.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, REjercicios.Hasta));

            if(model.desde>model.hasta)
                throw new ValidationException(string.Format(General.ErrorCampo1MayorCampo2, REjercicios.Desde, REjercicios.Hasta));

            if (_db.Ejercicios.Any(f=>(f.desde <= model.desde) && f.hasta>=model.hasta && f.empresa ==model.empresa && f.id!=model.id)) //esta en un rango existente
                throw new ValidationException(string.Format(General.ErrorRangoExistente));

            

            return base.ValidarGrabar(model);
        }

       
    }
}
