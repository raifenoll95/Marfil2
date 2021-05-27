using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class CriteriosagrupacionConverterService : BaseConverterModel<CriteriosagrupacionModel, Criteriosagrupacion>
    {
        public CriteriosagrupacionConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Criteriosagrupacion>().Where(f =>f.id == id).Include(f => f.CriteriosagrupacionLin).ToList().Single();
           
            var result = GetModelView(obj) as CriteriosagrupacionModel;
           
            //Lineas
            result.Lineas = obj.CriteriosagrupacionLin.ToList().Select(f => new CriteriosagrupacionLinModel()
            {
                Id=f.id,
                Campo = f.campo,
                Orden = f.orden ?? f.id
            }).ToList();

            return result;
        }

        public override Criteriosagrupacion CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as CriteriosagrupacionModel;
            var result = _db.Set<Criteriosagrupacion>().Create();
           
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
            
            foreach (var item in viewmodel.Lineas)
            {
                var newItem= _db.Set<CriteriosagrupacionLin>().Create();
                newItem.fkcriteriosagrupacion = viewmodel.Id;
                newItem.id = item.Id;
                newItem.campo = (int)item.Campo;
                newItem.orden = item.Orden??0;
                result.CriteriosagrupacionLin.Add(newItem);
            }

            return result;
        }

        public override Criteriosagrupacion EditPersitance(IModelView obj)
        {
            var viewmodel = obj as CriteriosagrupacionModel;
            var result = _db.Criteriosagrupacion.Where(f =>f.id==viewmodel.Id).Include(b => b.CriteriosagrupacionLin).ToList().Single();
            
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
            
            
            result.CriteriosagrupacionLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.Set<CriteriosagrupacionLin>().Create();
                newItem.fkcriteriosagrupacion = viewmodel.Id;
                newItem.id = item.Id;
                newItem.campo = (int)item.Campo;
                newItem.orden = item.Orden ?? 0;
                result.CriteriosagrupacionLin.Add(newItem);
                
            }

           
            return result;
        }
    }
}
