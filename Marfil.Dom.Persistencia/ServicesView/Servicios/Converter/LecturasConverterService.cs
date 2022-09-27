using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class LecturasConverterService : BaseConverterModel<LecturasModel, Lecturas>
    {
        public LecturasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IModelView CreateView(string id)
        {
            var service = new LecturasService(Context);

            var usuarioid = service.GetUsuarioid();

            var obj = _db.Set<Lecturas>().Where(f => f.empresa == Empresa && f.identificador == id && f.usuario == usuarioid).FirstOrDefault();

            var result = GetModelView(obj) as LecturasModel;

            return result;
        }

        public override Lecturas EditPersitance(IModelView obj)
        {
            var viewmodel = obj as LecturasModel;
            var result = _db.Lecturas.Where(f => f.empresa == viewmodel.Empresa && f.identificador == viewmodel.Identificador).FirstOrDefault();

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            return result;
        }
    }
}
