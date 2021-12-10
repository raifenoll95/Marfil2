using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Resources;
using RGuias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.GuiasBalances;
using RGuiasLin = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.GuiasBalancesLineas;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class GuiasBalancesValidation : BaseValidation<GuiasBalances>
    {
        public GuiasBalancesValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(GuiasBalances model)
        {
            if ( model.guiaId == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RGuias.Guia));
            if (model.informeId == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RGuias.Informe));

            foreach (var item in model.GuiasBalancesLineas)
            {
                ValidarLineas(item);
            }           

            return base.ValidarGrabar(model);
        }

        private void ValidarLineas(GuiasBalancesLineas model)
        {
            if (model.guiaId == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RGuiasLin.Guia));
            if (model.informeId == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RGuiasLin.Informe));
            if (model.guiasBalancesId == null)
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RGuiasLin.GuiasBalancesId));
        }

        public override bool ValidarBorrar(GuiasBalances model)
        {
            //todo check if it used in a serie
            return base.ValidarBorrar(model);
        }
    }
}
