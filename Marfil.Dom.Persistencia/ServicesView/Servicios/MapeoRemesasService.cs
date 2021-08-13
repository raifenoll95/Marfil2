using Marfil.Dom.Persistencia.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    class MapeoRemesasService : GestionService<MapeoRemesasModel, MapeoRemesas>
    {
        public MapeoRemesasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }
        public IEnumerable<MapeoRemesasModel> GetEtiquetasCampos()
        {
            return _db.Set<MapeoRemesas>().ToList().Select(f => _converterModel.GetModelView(f) as MapeoRemesasModel);
        }

    }
}
