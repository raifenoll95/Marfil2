using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RFicheros = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Ficheros;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class FicherosValidation : BaseValidation<Ficheros>
    {
        public FicherosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool ValidarGrabar(Ficheros model)
        {

            model.nombre = model.nombre.Replace("%", "");//quitamos este caracter para que nos casque con la web

            if (string.IsNullOrEmpty(model.nombre))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFicheros.Nombre));

            if (string.IsNullOrEmpty(model.tipo))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RFicheros.Tipo));

            return base.ValidarGrabar(model);
        }
    }
}
