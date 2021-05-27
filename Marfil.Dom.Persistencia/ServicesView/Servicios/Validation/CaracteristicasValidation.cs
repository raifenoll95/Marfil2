using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RCaracteristicas= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Caracteristicas;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class CaracteristicasValidation : BaseValidation<Caracteristicas>
    {
        public CaracteristicasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Caracteristicas model)
        {
            ValidarLineas(model);

            return base.ValidarGrabar(model);
        }

        private void ValidarLineas(Caracteristicas model)
        {
            if (model.CaracteristicasLin.Any(item => string.IsNullOrEmpty(item.descripcion)))
            {
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RCaracteristicas.Descripcion));
            }

            foreach (var item in model.CaracteristicasLin)
                item.id = Funciones.RellenaCod(item.id, 2);
        }
    }
}
