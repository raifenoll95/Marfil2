using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class RemesasConverterService : BaseConverterModel<RemesasModel, Remesas>
    {
        public RemesasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IModelView CreateView(string id)
        {
            var identificador = Funciones.Qint(id);
            var obj = _db.Set<Remesas>().Where(f => f.empresa == Empresa && f.id == identificador).Single();
            var result = GetModelView(obj) as RemesasModel;


            return result;
        }
    }
}
