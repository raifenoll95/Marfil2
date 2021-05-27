using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    internal class SeccionesAnaliticasValidation:BaseValidation<Seccionesanaliticas>
    {
        #region CTR
        public SeccionesAnaliticasValidation(IContextService context, MarfilEntities db) : base(context, db)
        {

        }
        #endregion


        #region Validar Grabar
        public override bool ValidarGrabar(Seccionesanaliticas model)
        {
            return base.ValidarGrabar(model); 
        }
        #endregion

        #region Validar Borrar
        public override bool ValidarBorrar(Seccionesanaliticas model)
        {
            return base.ValidarBorrar(model);
        }
        #endregion



    }
}
