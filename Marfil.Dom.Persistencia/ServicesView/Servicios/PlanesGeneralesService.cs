using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IPlanesGeneralesService
    {

    }

    public class PlanesGeneralesService : GestionService<PlanesGeneralesModel, Planesgenerales>, IPlanesGeneralesService
    {
        #region CTR

        public PlanesGeneralesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion
    }
}
