using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{

    public interface ICaracteristicasService
    {

    }

    public class CaracteristicasService : GestionService<CaracteristicasModel, Caracteristicas>, ICaracteristicasService
    {
        #region CTR

        public CaracteristicasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region List Index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Id","Descripcion" };
            var propiedades = Helpers.Helper.getProperties<CaracteristicasModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select c.id, f.descripcion from caracteristicas as c " +
                                 " inner join familiasproductos as f on f.empresa=c.empresa and f.id=c.id " +
                   " where c.empresa='{0}'",Empresa);
        }

        #endregion
    }
}
