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
    public interface IMunicipiosService
    {

    }

    public class MunicipiosService : GestionService<MunicipiosModel, Municipios>, IMunicipiosService
    {
        #region CTR

        public MunicipiosService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        #region ListIndexModel

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            model.ExcludedColumns = new[] { "CustomId", "Codigopais", "Codigoprovincia", "Cod", "Context", "Toolbar"};
            model.PrimaryColumnns = new[] { "CustomId" };
            model.ColumnasCombo.Add("DescripcionPais", _appService.GetListPaises().Select(f => new Tuple<string, string>(f.Descripcion, f.Descripcion)));
            return model;
        }

        public override string GetSelectPrincipal()
        {
            return "select m.*,pa.descripcion as DescripcionPais, pr.nombre as DescripcionProvincia, concat(m.codigopais,'-',m.codigoprovincia,'-',m.cod) as CustomId from municipios as m " +
                   " inner join paises as pa on pa.valor=m.codigopais" +
                   " inner join provincias as pr on pr.id=m.codigoprovincia";
        }

        #endregion
        
        public int obtenerUltimoId(MunicipiosModel model)
        {
            int n = 0;
            if(_db.Municipios.Any(f => f.codigopais == model.Codigopais && f.codigoprovincia == model.Codigoprovincia)) {
                n = Int32.Parse(_db.Municipios.Where(f => f.codigopais == model.Codigopais && f.codigoprovincia == model.Codigoprovincia).Select(f => f.cod).ToList().Max());
            }
            return n;
        }

        public IEnumerable<MunicipiosModel> GetMunicipiosProvincia(string codigoprovincia)
        {
            return _db.Set<Municipios>().Where(f => f.codigoprovincia == codigoprovincia).ToList().Select(f => _converterModel.GetModelView(f) as MunicipiosModel);
        }

        public IEnumerable<MunicipiosModel> GetMunicipioNombre(string nombre)
        {
            return _db.Set<Municipios>().Where(f => f.nombre == nombre).ToList().Select(f => _converterModel.GetModelView(f) as MunicipiosModel);
        }
    }
}
