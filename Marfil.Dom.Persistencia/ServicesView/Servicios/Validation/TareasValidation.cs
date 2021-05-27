using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RTareas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Tareas;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class TareasValidation : BaseValidation<Tareas>
    {
        public TareasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Tareas model)
        {
            if (string.IsNullOrEmpty(model.id))
            {
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RTareas.Id));
            }
            if (string.IsNullOrEmpty(model.descripcion))
            {
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RTareas.Descripcion));
            }
            if (model.capacidad < 0 || model.capacidad > 9999.99)
            {
                throw new ValidationException(string.Format(RTareas.ErrorRangoCapacidad, RTareas.Capacidad));
            }
            if (model.precio < 0 || model.precio > 9999999.99) // 9,999,999.99
            {
                throw new ValidationException(string.Format(RTareas.ErrorPrecio, RTareas.Precio));
            }
            foreach (var linea in model.TareasLin)
            {
                var longitud = linea.año.ToString();
                if (longitud.Length != 4)
                {
                    throw new ValidationException(string.Format(RTareas.ErrorFormatoAño, RTareas.Año));
                }
                if (linea.año < 2000 || linea.año > 2100)
                {
                    throw new ValidationException(string.Format(RTareas.ErrorRangoFecha, RTareas.Año));
                }
                if (linea.precio < 0 || linea.precio > 9999999.99)
                {
                    throw new ValidationException(string.Format(RTareas.ErrorPrecio, RTareas.Precio));
                }
            }

            return base.ValidarGrabar(model);
        }
    }
}
