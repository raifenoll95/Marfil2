using Marfil.Dom.Persistencia.Model.Documentos.GrupoMateriales;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class GrupoMaterialesConverterService : BaseConverterModel<GrupoMaterialesModel, GrupoMateriales>
    {
        #region
        public GrupoMaterialesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        //Get All
        public override IEnumerable<IModelView> GetAll()
        {
            var result = _db.Set<GrupoMateriales>().Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as GrupoMaterialesModel).ToList();
            return result;
        }

        //Exist
        public override bool Exists(string id)
        {
            return _db.GrupoMateriales.Any(f => f.empresa == Context.Empresa && f.cod == id);
        }

        //Obtiene el modelo a partir del codigo
        public override IModelView CreateView(string codigo)
        {
            var obj = _db.Set<GrupoMateriales>().Where(f => f.empresa == Empresa && f.cod == codigo).Single();
            var result = GetModelView(obj) as GrupoMaterialesModel;
            return result;
        }

        public override GrupoMateriales CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as GrupoMaterialesModel;
            var result = _db.Set<GrupoMateriales>().Create();

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

            result.cod = viewmodel.Cod;
            result.empresa = Empresa;
            result.descripcion = viewmodel.Descripcion;
            result.inglesDescripcion = viewmodel.InglesDescripcion;
    
            return result;
        }

        public override GrupoMateriales EditPersitance(IModelView obj)
        {
            var viewmodel = obj as GrupoMaterialesModel;
            var result = _db.GrupoMateriales.Where(f => f.empresa == viewmodel.Empresa && f.cod == viewmodel.Cod).Single();
            //todo asignar
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

            //todo asignar contador y referencia
            result.cod = viewmodel.Cod;
            result.descripcion = viewmodel.Descripcion;
            result.inglesDescripcion = viewmodel.InglesDescripcion;

            return result;
        }

        #endregion
    }
}

