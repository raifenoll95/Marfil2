using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class BaseConverterModel<TView,TPersistance> : IConverterModelService<TView,TPersistance> where TPersistance : class where TView : class
    {
      
        public string Empresa { get; set; }
        
        [XmlIgnore]
        protected IContextService Context { get; set; }
        [XmlIgnore]
        protected ApplicationHelper _appService;
        [XmlIgnore]
        protected MarfilEntities _db;
        

        public BaseConverterModel(IContextService context,MarfilEntities db)
        {
            _db = db;
            Context = context;
            _appService= new ApplicationHelper(context);
        }

        public virtual IEnumerable<IModelView> GetAll()
        {
            return _db.Set<TPersistance>().ToList().Select(GetModelView);
        }

        public virtual IModelView CreateView(string  id)
        {
            
            var ctor = typeof(TView).GetConstructor(new[] { typeof(IContextService) });
            var objModel = ctor.Invoke(new object[] { Context });
            var objExt = objModel as IModelViewExtension;
            var obj = _db.Set<TPersistance>().Find(objExt.generateId(id));
            return GetModelView(obj);
        }

        public virtual bool Exists(string id)
        {
            var ctor = typeof(TView).GetConstructor(new[] { typeof(IContextService) });
            var objModel = ctor.Invoke(new object[] { Context });
            var objExt = objModel as IModelViewExtension;
            return _db.Set<TPersistance>().Find(objExt.generateId(id))!=null;
        }

        public virtual IModelView GetModelView(TPersistance obj)
        {
            var fmodel=new FModel();
            var instance = fmodel.GetModel<TView>(Context,_db) as IModelView;
            if(obj != null)
            {
                var objProperties = obj.GetType().GetProperties();
                foreach (var item in objProperties)
                {
                    if (obj.GetType().GetProperty(item.Name).PropertyType.IsGenericType &&
                        obj.GetType().GetProperty(item.Name).PropertyType.GetGenericTypeDefinition() !=
                        typeof(ICollection<>))
                    {
                        instance.set(item.Name.FirstToUpper(), obj.GetType().GetProperty(item.Name)?.GetValue(obj, null));
                    }
                    else if (obj.GetType().GetProperty(item.Name).PropertyType.IsEnum)
                    {
                        instance.set(item.Name.FirstToUpper(), (int)obj.GetType().GetProperty(item.Name)?.GetValue(obj, null));
                    }
                    else if (!obj.GetType().GetProperty(item.Name).PropertyType.IsGenericType)
                    {
                        instance.set(item.Name.FirstToUpper(), obj.GetType().GetProperty(item.Name)?.GetValue(obj, null));
                    }
                }
            }
            return instance;
        }

        public virtual TPersistance CreatePersitance(IModelView obj)
        {
            var result = _db.Set<TPersistance>().Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) )
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            return result;
        }

        public virtual TPersistance EditPersitance(IModelView obj)
        {
            var objext = obj as IModelViewExtension;
            var result = _db.Set<TPersistance>().Find(obj.get(objext.primaryKey.First().Name));

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            return result;
        }

        public virtual void AsignaId(TPersistance objPer, ref IModelView objView)
        {
            var objext = objView as IModelViewExtension;
            var prop = objPer.GetType().GetProperty(objext?.primaryKey.FirstOrDefault()?.Name ?? string.Empty) ??
                        objPer.GetType().GetProperty(objext?.primaryKey.FirstOrDefault()?.Name.FirstToUpper() ?? string.Empty) ??
                        objPer.GetType().GetProperty(objext?.primaryKey.FirstOrDefault()?.Name.ToLower() ?? string.Empty);

            if (prop != null && prop.GetValue(objPer) != null)
                objView.set(objext.primaryKey.First().Name, prop.GetValue(objPer));
        }

    }
}
