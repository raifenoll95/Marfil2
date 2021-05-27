using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IGuiascontablesService
    {

    }

    public class GuiascontablesService : GestionService<GuiascontablesModel, Guiascontables>, IGuiascontablesService
    {
        #region CTR

        public GuiascontablesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st= base.GetListIndexModel(t, canEliminar, canModificar, controller);

            st.ExcludedColumns = new[] {"Empresa", "Lineas","Toolbar"};
            return st;
        }
    }
}
