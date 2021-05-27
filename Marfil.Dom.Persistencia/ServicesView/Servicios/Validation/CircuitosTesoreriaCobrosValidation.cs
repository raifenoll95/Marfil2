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
    class CircuitosTesoreriaCobrosValidation : BaseValidation<Persistencia.CircuitosTesoreriaCobros>
    {
        #region CTR

        public CircuitosTesoreriaCobrosValidation(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        public override bool ValidarGrabar(Persistencia.CircuitosTesoreriaCobros model)
        {
            bool descuadrado = false;

            if((String.IsNullOrEmpty(model.cuentacargo1) && model.importecuentacargo1 != 0) || (!String.IsNullOrEmpty(model.cuentacargo1) && model.importecuentacargo1 == 0))
            {
                descuadrado = true;
            }
            if ((String.IsNullOrEmpty(model.cuentacargo2) && model.importecuentacargo2 != 0) || (!String.IsNullOrEmpty(model.cuentacargo2) && model.importecuentacargo2 == 0))
            {
                descuadrado = true;
            }
            if ((String.IsNullOrEmpty(model.cuentacargorel) && model.importecuentacargorel != 0) || (!String.IsNullOrEmpty(model.cuentacargorel) && model.importecuentacargorel == 0))
            {
                descuadrado = true;
            }
            if ((String.IsNullOrEmpty(model.cuentaabono1) && model.importecuentaabono1 != 0) || (!String.IsNullOrEmpty(model.cuentaabono1) && model.importecuentaabono1 == 0))
            {
                descuadrado = true;
            }
            if ((String.IsNullOrEmpty(model.cuentaabono2) && model.importecuentaabono2 != 0) || (!String.IsNullOrEmpty(model.cuentaabono2) && model.importecuentaabono2 == 0))
            {
                descuadrado = true;
            }
            if ((String.IsNullOrEmpty(model.cuentaabonorel) && model.importecuentaabonorel != 0) || (!String.IsNullOrEmpty(model.cuentaabonorel) && model.importecuentaabonorel == 0))
            {
                descuadrado = true;
            }

            if (descuadrado)
            {
                throw new ValidationException("Una cuenta no vacía debe tener asignado un importe asignado, y viceversa");
            }

            return base.ValidarGrabar(model);
        }
    }
}
