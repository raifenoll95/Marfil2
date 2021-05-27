using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RSeguimientos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Seguimientos;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class SeguimientosCorreoValidation : BaseValidation<SeguimientosCorreo>
    {
        public bool CambiarEstado { get; set; }

        public SeguimientosCorreoValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(SeguimientosCorreo model)
        {
            //if (!CambiarEstado)
            //{
            //    ValidarEstado(model);
            //}

            return base.ValidarGrabar(model);
        }

        //private void ValidarEstado(SeguimientosCorreo model)
        //{
        //    string message;
        //    if (!_appService.ValidarEstado(model.fketapa, _db, out message))
        //        throw new ValidationException(message);

        //}

    }
}
