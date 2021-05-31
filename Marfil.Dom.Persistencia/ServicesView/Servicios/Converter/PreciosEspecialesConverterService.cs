using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Marfil.Inf.Genericos;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class PreciosEspecialesConverterService : BaseConverterModel<PreciosEspecialesModel, PreciosEspeciales>
    {
        public PreciosEspecialesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override PreciosEspeciales CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as PreciosEspecialesModel;
            var result = _db.Set<PreciosEspeciales>().Create();

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

        public override IModelView CreateView(string id)
        {
            var idparse = int.Parse(id);
            var result = _db.PreciosEspeciales.Single(f => f.empresa == Context.Empresa && f.id == idparse);

            return GetModelView(result);
        }

        public override PreciosEspeciales EditPersitance(IModelView obj)
        {
            var viewmodel = obj as PreciosEspecialesModel;
            var result = _db.Set<PreciosEspeciales>().Single(f => f.empresa == Context.Empresa && f.id == viewmodel.Id);

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

        public override bool Exists(string id)
        {
            var idparse = int.Parse(id);
            return _db.PreciosEspeciales.Any(f => f.empresa == Context.Empresa && f.id == idparse);
        }

        public override IModelView GetModelView(PreciosEspeciales obj)
        {
            var result = base.GetModelView(obj) as PreciosEspecialesModel;

            return result;
        }
    }
}
