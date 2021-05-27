using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IIncidenciasService
    {

    }

    public class IncidenciasService : GestionService<IncidenciasModel, Incidencias>, IIncidenciasService
    {
        
        #region CTR

        public IncidenciasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndex

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st= base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.ExcludedColumns = new[] {"Empresa","Toolbar","Fkgrupo"};
            return st;
        }

        #endregion
    }
}
