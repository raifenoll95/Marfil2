using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class IncidenciasCRMValidation : BaseValidation<IncidenciasCRM>
    {
        public bool CambiarEstado { get; set; }

        public IncidenciasCRMValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(IncidenciasCRM model)
        {
            if (!CambiarEstado)
            {
                ValidarEstado(model);
            }

            return base.ValidarGrabar(model);
        }

        private void ValidarEstado(IncidenciasCRM model)
        {
            string message;
            if (!_appService.ValidarEstado(model.fketapa, _db, out message))
                throw new ValidationException(message);
        }

    }
}
