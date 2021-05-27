using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IGrosoresService
    {

    }

    public class GrosoresService : GestionService<GrosoresModel, Grosores>, IGrosoresService
    {
        #region CTR

        public GrosoresService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion


        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Id", "Descripcion", "Descripcion2", "Descripcionabreviada", "Grosor", "Coficientecortabloques", "Coeficientetelares" };

            var propiedades = Helpers.Helper.getProperties<AcabadosModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return string.Format("select * from Grosores as a where a.empresa='{0}'", Empresa);
        }

    }
}
