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
    class CarteraVencimientosValidation : BaseValidation<Persistencia.CarteraVencimientos>
    {

        public string EjercicioId { get; set; }

        #region CTR
        public CarteraVencimientosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(Persistencia.CarteraVencimientos model)
        {
            return base.ValidarGrabar(model);
        }
    }
}
