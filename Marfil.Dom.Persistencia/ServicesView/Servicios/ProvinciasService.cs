using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IProvinciasService
    {

    }

    public class ProvinciasService : GestionService<ProvinciasModel, Provincias>, IProvinciasService
    {
        #region CTR

        public ProvinciasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            model.ExcludedColumns = new [] { "CustomId", "Codigopais","Toolbar" };
            model.PrimaryColumnns = new[] {"CustomId"};
            model.ColumnasCombo.Add("DescripcionPais",_appService.GetListPaises().Select(f=> new Tuple<string, string>(f.Descripcion,f.Descripcion)));
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return "select p.*,pa.descripcion as DescripcionPais, concat(p.codigopais,'-',p.id) as CustomId from provincias as p " +
                   " inner join paises as pa on pa.valor=p.codigopais";
        }

        #endregion

        public IEnumerable<ProvinciasModel> GetProvinciasPais(string codigopais)
        {
            return _db.Set<Provincias>().Where(f => f.codigopais == codigopais).ToList().Select(f=>_converterModel.GetModelView(f) as ProvinciasModel);
        }
    }
}
