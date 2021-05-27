using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class OportunidadesValidation : BaseValidation<Oportunidades>
    {
        public bool CambiarEstado { get; set; }

        public OportunidadesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Oportunidades model)
        {
            if (!CambiarEstado)
            {
                ValidarEstado(model);
            }

            return base.ValidarGrabar(model);
        }

        private void ValidarEstado(Oportunidades model)
        {
            string message;
            if (!_appService.ValidarEstado(model.fketapa, _db, out message))
                throw new ValidationException(message);
        }

    }
}
