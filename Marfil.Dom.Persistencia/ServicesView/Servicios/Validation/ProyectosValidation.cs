using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class ProyectosValidation : BaseValidation<Proyectos>
    {
        public bool CambiarEstado { get; set; }

        public ProyectosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Proyectos model)
        {
            if (!CambiarEstado)
            {
                ValidarEstado(model);
            }

            return base.ValidarGrabar(model);
        }

        private void ValidarEstado(Proyectos model)
        {
            string message;
            if (!_appService.ValidarEstado(model.fketapa, _db, out message))
                throw new ValidationException(message);
        }

    }
}
