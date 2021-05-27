using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IContadoresService
    {

    }

    public class ContadoresService : GestionService<ContadoresModel, Contadores>, IContadoresService
    {
        #region CTR

        public ContadoresService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st= base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.ExcludedColumns = new[] {"Empresa", "Lineas","Toolbar","Tipocontador"};
            return st;
        }
    }
}
