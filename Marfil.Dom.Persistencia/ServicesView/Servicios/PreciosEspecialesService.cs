using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class PreciosEspecialesService : GestionService<PreciosEspecialesModel, PreciosEspeciales>
    {
        #region CTR

        public PreciosEspecialesService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Fkclientes", "NombreCliente", "Fkarticulo", "DescripcionArticulo", "Fechavalidez", "Precio", "Descuento" };
            var propiedades = Helpers.Helper.getProperties<PreciosEspecialesModel>();

            //model.PrimaryColumnns = new[] { "id" };
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.OrdenColumnas.Add("Fkclientes", 0);
            model.OrdenColumnas.Add("NombreCliente", 1);
            model.OrdenColumnas.Add("Fkarticulo", 2);
            model.OrdenColumnas.Add("DescripcionArticulo", 3);
            model.OrdenColumnas.Add("Fechavalidez", 4);
            model.OrdenColumnas.Add("Precio", 5);
            model.OrdenColumnas.Add("Descuento", 6);

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select i.*,c.descripcion as NombreCliente, a.descripcion as DescripcionArticulo from PreciosEspeciales as i " +
                   " inner join cuentas as c on c.id=i.fkclientes and c.empresa = i.empresa" +
                   " inner join articulos as a on a.id=i.fkarticulo and a.empresa = i.empresa" +
                   " where i.empresa='{0}'", Empresa);
        }

        #endregion

    }
}
