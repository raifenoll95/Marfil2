using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class CampañasValidation : BaseValidation<Campañas>
    {
        public bool CambiarEstado { get; set; }

        public CampañasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Campañas model)
        {
            if (!CambiarEstado)
            {
                ValidarEstado(model);
            }

            return base.ValidarGrabar(model);
        }

        private void ValidarEstado(Campañas model)
        {
            string message;
            if (!_appService.ValidarEstado(model.fketapa, _db, out message))
                throw new ValidationException(message);
        }

    }
}
