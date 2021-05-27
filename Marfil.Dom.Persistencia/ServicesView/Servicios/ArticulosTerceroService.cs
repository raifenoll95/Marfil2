using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{



    public class ArticulosTerceroService : GestionService<ArticulosTerceroModel, Articulos>
    {

        #region constructor
        public ArticulosTerceroService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
        }
        #endregion

        #region index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            model.List = model.List.OfType<ArticulosTerceroModel>();
            var propiedadesVisibles = new[] { "CodArticulo", "CodTercero", "CodArticuloTercero", "Descripcion" };
            var propiedades = Helpers.Helper.getProperties<ArticulosModel>();
            model.PrimaryColumnns = new[] { "CodTercero" };
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * from ArticulosTercero where empresa='{0}'", Empresa);
        }

        #endregion
    }
}
