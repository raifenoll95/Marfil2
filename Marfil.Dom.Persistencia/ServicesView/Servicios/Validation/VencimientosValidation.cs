using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RFormasPago = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Formaspago;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class VencimientosValidation : BaseValidation<Persistencia.Vencimientos>
    {
        #region CTR

        public VencimientosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(Persistencia.Vencimientos model)
        {
            return base.ValidarGrabar(model);
        }
    }
}
