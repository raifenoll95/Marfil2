using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ITarifasbaseService
    {

    }

    public class TarifasbaseService : GestionService<TarifasbaseModel, Tarifasbase>, ITarifasbaseService
    {
        #region CTR

        public TarifasbaseService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion
    }
}
