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
    class FormasPagoValidation : BaseValidation<FormasPago>
    {
        #region CTR

        public FormasPagoValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(FormasPago model)
        {
            if(string.IsNullOrEmpty(model.nombre))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFormasPago.Nombre));

            if (model.bloqueada.HasValue && model.bloqueada.Value)
            {
                throw new ValidationException(General.ErrorModificarRegistroBloqueado);
            }

            var total = model.FormasPagoLin.Sum(f => f.porcentajerecargo);
            if (total != 0 && total != 100)
                throw new ValidationException(RFormasPago.ErrorSumaPorcentajes);

            if (total == 0)
            {
                var count = model.FormasPagoLin.Count;
                var intervalos = Math.Round(100.0 / count, 2);
                var i = 0;
                var acum = 0.0;
                foreach (var item in model.FormasPagoLin)
                {
                    item.porcentajerecargo = i == count - 1 ? 100.0 - acum : intervalos;//los intervalos no son siempre exacto, con esto nos aseguramos que la suma siempre sea 100
                    acum += intervalos;
                    i++;
                }
            }
            if(string.IsNullOrEmpty(model.fkgruposformaspago))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFormasPago.FkGruposformaspago));

            return base.ValidarGrabar(model);
        }
    }
}
