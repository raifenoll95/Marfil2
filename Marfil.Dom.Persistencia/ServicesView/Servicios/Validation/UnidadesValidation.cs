using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class UnidadesValidation :BaseValidation<Unidades>
    {
        #region CTR

        public UnidadesValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        #endregion

        public override bool ValidarGrabar(Unidades model)
        {
            if(string.IsNullOrEmpty(model.id))
                throw new Exception(Resources.Unobtrusive.PropertyValueRequired);

            if (string.IsNullOrEmpty(model.codigounidad))
                throw new Exception(Resources.Unobtrusive.PropertyValueRequired);

            if (string.IsNullOrEmpty(model.textocorto))
                throw new Exception(Resources.Unobtrusive.PropertyValueRequired);

            return base.ValidarGrabar(model);
        }

        public override bool ValidarBorrar(Unidades model)
        {
            if(_db.Familiasproductos.Any(f=>f.fkunidadesmedida==model.id && f.empresa==Context.Empresa))
                throw new ValidationException(General.ErrorBorradoUnidadesUsadas);

            return base.ValidarBorrar(model);
        }
    }
}
