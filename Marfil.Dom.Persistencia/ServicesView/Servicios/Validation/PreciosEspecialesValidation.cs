using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RPreciosEspeciales = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PreciosEspeciales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class PreciosEspecialesValidation : BaseValidation<PreciosEspeciales>
    {
        #region CTR

        public PreciosEspecialesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(PreciosEspeciales model)
        {
            if (string.IsNullOrEmpty(model.fkclientes))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPreciosEspeciales.Fkclientes));

            if (string.IsNullOrEmpty(model.fkarticulo))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPreciosEspeciales.Fkarticulo));

            if (!model.fechavalidez.HasValue)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPreciosEspeciales.Fechavalidez));

            if (double.IsNaN(model.precio))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RPreciosEspeciales.Precio));

            if (_db.PreciosEspeciales.Any(f => f.fkclientes == model.fkclientes && f.fkarticulo == model.fkarticulo && f.id != model.id && f.empresa == model.empresa))
                throw new ValidationException("Ya existe un precio especial para este cliente y este artículo. Solo se permite un único registro por cliente y articulo.");

            return base.ValidarGrabar(model);
        }
    }
}
