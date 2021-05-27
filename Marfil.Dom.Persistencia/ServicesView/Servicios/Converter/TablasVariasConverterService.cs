using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using System.Data.Entity;
using System.Reflection;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{

    class TablasVariasConverterService<TModelLin, TEntityLin> : BaseConverterModel<BaseTablasVariasModel, Tablasvarias> where TEntityLin : class where TModelLin : class
    {
        public TablasVariasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Tablasvarias>().Include(f => f.TablasvariasLin).ToList().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Tablasvarias>().Where(f => f.id.ToString() == id).Include(f => f.TablasvariasLin).Single();
            return GetModelView(obj);
        }

        public override IModelView GetModelView(Tablasvarias obj)
        {
            var result = new BaseTablasVariasModel()
            {
                Id = obj.id,
                Nombre = obj.nombre,
                Clase = obj.clase,
                Tipo = obj.tipo.HasValue ? (TipoTablaVaria)obj.tipo : TipoTablaVaria.Otros,
                Noeditable = obj.noeditable ?? false
            };
            var tipo = Helper.GetTypeFromFullName(obj.clase);
            // No target, no arguments
            var serializerType = typeof(TablasVariasLinConverterService<>);
            var genericType = serializerType.MakeGenericType(tipo);
            var serializer = Activator.CreateInstance(genericType,Context,_db);
            var methodInfo = genericType.GetMethod("GetModelView");
            result.Lineas = new List<dynamic>();
            foreach (var item in obj.TablasvariasLin)
            {
                (result.Lineas as List<dynamic>).Add(methodInfo.Invoke(serializer, new[] { item }));
            }

            return result;
        }

        public override Tablasvarias CreatePersitance(IModelView obj)
        {
            var result = _db.Tablasvarias.Create();
            var objext = obj as BaseTablasVariasModel;

            result.TablasvariasLin.Clear();
            var serializerType = typeof(Serializer<>);
            var genericType = serializerType.MakeGenericType(Helper.GetTypeFromFullName(objext.Clase));
            var serializer = Activator.CreateInstance(genericType);
            var methodInfo = genericType.GetMethod("GetXml");
            result.id = objext.Id;
            result.clase = objext.Clase;
            result.nombre = objext.Nombre;
            result.tipo = (int)objext.Tipo;
            result.noeditable = objext.Noeditable;
            foreach (var item in objext.Lineas)
            {
                var newItem = _db.TablasvariasLin.Create();
                newItem.id = Guid.NewGuid();
                newItem.xml = methodInfo.Invoke(serializer, new[] { item }) as string;
                result.TablasvariasLin.Add(newItem);
            }

            return result;
        }

        public override Tablasvarias EditPersitance(IModelView obj)
        {
            var objext = obj as BaseTablasVariasModel;
            var result = _db.Tablasvarias.Where(f => f.id == objext.Id).Include(f => f.TablasvariasLin).ToList().Single();

            result.TablasvariasLin.Clear();
            var serializerType = typeof(Serializer<>);
            var genericType = serializerType.MakeGenericType(Helper.GetTypeFromFullName(result.clase));
            var serializer = Activator.CreateInstance(genericType);
            var methodInfo = genericType.GetMethod("GetXml");
            //result.tipo = (int)objext.Tipo; Esta linea asigna el tipo Otros(0) siempre sea cual sea el tipo que le llegue.
            result.noeditable = objext.Noeditable;

            foreach (var item in objext.Lineas)
            {
                var newItem = _db.TablasvariasLin.Create();
                newItem.id = Guid.NewGuid();
                newItem.xml = methodInfo.Invoke(serializer, new[] { item }) as string;
                result.TablasvariasLin.Add(newItem);
            }

            return result;
        }
    }
}
