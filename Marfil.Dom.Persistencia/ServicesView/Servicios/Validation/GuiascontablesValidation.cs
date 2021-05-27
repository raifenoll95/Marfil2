using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RGuiascontables= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Guiascontables;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class GuiascontablesValidation : BaseValidation<Guiascontables>
    {
        public GuiascontablesValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Guiascontables model)
        {
            if(string.IsNullOrEmpty(model.descripcion))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RGuiascontables.Id));

            var duplicates = model.GuiascontablesLin.GroupBy(s => s.fkregimeniva)
            .SelectMany(grp => grp.Skip(1));
            if(duplicates.Any())
                throw new ValidationException(RGuiascontables.ErrorRegimenIvaDuplicado);

            return base.ValidarGrabar(model);
        }
    }
}
