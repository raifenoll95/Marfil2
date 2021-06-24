using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class CuadernosBancariosServices : GestionService<CuadernosBancariosModel, CuadernosBancarios>
    {
        #region CTR

        public CuadernosBancariosServices(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Clave", "Descripcion", "Banco", "TipoFormato"};
            var propiedades = Helpers.Helper.getProperties<CuadernosBancariosModel>();

            //model.PrimaryColumnns = new[] { "id" };
            model.ExcludedColumns = propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();
            model.OrdenColumnas.Add("Clave", 0);
            model.OrdenColumnas.Add("Descripcion", 1);
            model.OrdenColumnas.Add("Banco", 2);
            model.OrdenColumnas.Add("TipoFormato", 3);

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select clave,descripcion,banco,tipoFormato " +
                "from CuadernosBancarios " +
                "where empresa='{0}'", Empresa);
        }

        #endregion
    }
}
