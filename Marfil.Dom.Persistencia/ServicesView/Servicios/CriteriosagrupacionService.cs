using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ICriteriosagrupacionService
    {

    }

    public class CriteriosagrupacionService : GestionService<CriteriosagrupacionModel, Criteriosagrupacion>, ICriteriosagrupacionService
    {
        #region CTR

        public CriteriosagrupacionService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Nombre", "Ordenaralbaranesvista" };
            var propiedades = Helpers.Helper.getProperties<CriteriosagrupacionModel>();
            st.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            return st;
        }
    }
}
