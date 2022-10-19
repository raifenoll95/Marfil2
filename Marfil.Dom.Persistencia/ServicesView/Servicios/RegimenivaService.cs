using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Iva;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IRegimenivaService
    {

    }

    public class RegimenivaService : GestionService<RegimenIvaModel, RegimenIva>, IRegimenivaService
    {


        #region CTR

        public RegimenivaService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var obj = new RegimenIvaModel();
            var excluded =
                obj.getProperties()
                    .Where(f => f.property.Name != "Descripcion" && f.property.Name != "Id")
                    .Select(f => f.property.Name).ToList();
            model.ExcludedColumns = excluded;

            return model;
        }

        public bool GetOperacionUE(string empresa, string regimen)
        {
            return _db.RegimenIva.Where(f => f.empresa == empresa && f.id == regimen).FirstOrDefault().operacionue.Value;
        }
    }
}
