using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Iva;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IGruposIvaService
    {

    }

    public class GruposIvaService : GestionService<GruposIvaModel, GruposIva>, IGruposIvaService
    {

        #region CTR

        public GruposIvaService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var excluded = new[] {"Empresa", "Lineas","Toolbar"};
            model.ExcludedColumns = excluded;

            return model;
        }

        public IEnumerable<GruposIvaModel> GetGruposWithTipoIva(string id)
        {
           return _db.GruposIva.Where(f => f.empresa == Empresa)
                .Include(f => f.GruposIvaLin)
                .Where(
                    j =>
                        j.GruposIvaLin.Any(
                            f =>
                                f.empresa == j.empresa && (f.fktiposivaconrecargo == id || f.fktiposivasinrecargo == id))).ToList().Select(f=>_converterModel.GetModelView(f) as GruposIvaModel);
        }
    }
}
