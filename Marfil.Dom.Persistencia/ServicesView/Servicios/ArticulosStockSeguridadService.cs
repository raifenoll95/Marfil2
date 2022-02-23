using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class ArticulosStockSeguridadService : GestionService<ArticulosStockSeguridadModel, Articulos>
    {

        #region constructor
        public ArticulosStockSeguridadService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
        }
        #endregion

        #region index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            model.List = model.List.OfType<ArticulosStockSeguridadModel>();
            var propiedadesVisibles = new[] { "Empresa", "Id", "Codarticulo", "Codalmacen", "Descripcionalmacen", "Stockseguridad", "Stockminimo", "Stockmaximo" };
            var propiedades = Helpers.Helper.getProperties<ArticulosModel>();
            //model.PrimaryColumnns = new[] { "Id" };
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * from ArticulosStockSeguridad where empresa='{0}'", Empresa);
        }

        #endregion
    }
}
