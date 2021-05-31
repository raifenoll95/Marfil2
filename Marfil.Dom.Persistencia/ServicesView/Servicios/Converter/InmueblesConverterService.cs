using Marfil.Dom.Persistencia.Model.Configuracion.Inmueble;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Marfil.Inf.Genericos;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class InmueblesConverterService : BaseConverterModel<InmueblesModel, Inmuebles>
    {
        public InmueblesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override Inmuebles CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as InmueblesModel;
            var result = _db.Set<Inmuebles>().Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) && item.Name.ToLower() != "situacion")
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
            result.situacion = (int)viewmodel.Situacion;

            return result;
        }

        public override IModelView CreateView(string id)
        {
            var result = _db.Inmuebles.Single(f => f.empresa == Context.Empresa && f.id == id);

            return GetModelView(result);
        }

        public override Inmuebles EditPersitance(IModelView obj)
        {
            var viewmodel = obj as InmueblesModel;
            var result = _db.Set<Inmuebles>().Single(f => f.empresa == Context.Empresa && f.id == viewmodel.Id);

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) && item.Name.ToLower() != "situacion")
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
            result.situacion = (int)viewmodel.Situacion;

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Inmuebles.Any(f => f.empresa == Context.Empresa && f.id == id);
        }

        public override IModelView GetModelView(Inmuebles obj)
        {
            var result = base.GetModelView(obj) as InmueblesModel;

            result.Situacion = obj.situacion.HasValue ? (InmueblesModel.TipoSituacion)obj.situacion : InmueblesModel.TipoSituacion.ConEspaña;

            return result;
        }
    }
}
