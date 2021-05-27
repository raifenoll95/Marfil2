using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class CarpetasConverterService : BaseConverterModel<CarpetasModel, Carpetas>
    {
        public CarpetasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool Exists(string id)
        {
            var idInt = new Guid(id);
            return _db.Set<Carpetas>().Any(f => f.id == idInt && f.empresa==Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var guidid = new Guid(id);
            var obj = _db.Carpetas.Single(f => f.empresa == Empresa && f.id == guidid);
            return GetModelView(obj);
        }

        public override Carpetas EditPersitance(IModelView obj)
        {
            var model = obj as CarpetasModel;
            var result = _db.Carpetas.Single(f => f.empresa == Empresa && f.id == model.Id);

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
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
