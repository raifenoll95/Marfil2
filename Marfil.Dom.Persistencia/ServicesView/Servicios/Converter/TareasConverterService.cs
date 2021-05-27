using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos;
using System.Data.Entity;
using System.Collections;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class TareasConverterService : BaseConverterModel<TareasModel, Tareas>
    {

        #region CTR

        public TareasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {

        }

        #endregion

        #region Api

        public override bool Exists(string id)
        {
            return _db.Set<Tareas>().Any(f => f.id == id && f.empresa == Context.Empresa);           
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Tareas>().Include(f => f.TareasLin).Single(f => f.id == id && f.empresa == Context.Empresa);            
            var result = GetModelView(obj) as TareasModel;            

            return result;
        }

        public override IModelView GetModelView(Tareas obj)
        {
            var result = new TareasModel
            { 
                Empresa = obj.empresa,
                Id = obj.id,               
                Descripcion = obj.descripcion,
                SeccionProduccion = obj.seccionproduccion,
                Imputacion = (Imputacion)obj.imputacion,
                Capacidad = (double)obj.capacidad,
                Precio = (double)obj.precio,
                Unidad = (Unidad)obj.unidad,                             
                Lineas = obj.TareasLin.Select(f => new TareasLinModel() { Empresa = f.empresa, Fktareas = f.fktareas, Id = f.id, Año = f.año.Value, Precio = f.precio.Value })
            };

            return result;
        }

        public override Tareas CreatePersitance(IModelView obj)
        {
            var objmodel = obj as TareasModel;
            var result = _db.Set<Tareas>().Create();

            result.empresa = objmodel.Empresa;
            result.id = objmodel.Id;
            result.descripcion = objmodel.Descripcion;
            result.seccionproduccion = objmodel.SeccionProduccion;
            result.imputacion = (int)objmodel.Imputacion;
            result.capacidad = objmodel.Capacidad;
            result.precio = objmodel.Precio;
            result.unidad = (int)objmodel.Unidad;

            // Añadir las líneas
            result.TareasLin.Clear();
            foreach (var item in objmodel.Lineas)
            {
                var newItem = _db.Set<TareasLin>().Create();
                newItem.empresa = item.Empresa;
                newItem.fktareas = item.Fktareas;
                newItem.id = item.Id;
                newItem.año = item.Año;
                newItem.precio = item.Precio;

                result.TareasLin.Add(newItem);
            }

            return result;
        }


        public override Tareas EditPersitance(IModelView obj)
        {
            //var objext = obj as IModelViewExtension;
            //var objmodel = obj as TareasModel;
            //var result = _db.Set<Tareas>().Single(f => f.empresa == Context.Empresa && f.id == objmodel.Id);

            //foreach (var item in result.GetType().GetProperties())
            //{
            //    if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
            //        obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
            //        typeof(ICollection<>))
            //    {
            //        item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
            //    }
            //    else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
            //    {
            //        item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
            //    }
            //    else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
            //    {
            //        item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
            //    }
            //}

            var objmodel = obj as TareasModel;
            var result = _db.Set<Tareas>().Single(f => f.empresa == Context.Empresa && f.id == objmodel.Id);

            result.empresa = objmodel.Empresa;
            result.id = objmodel.Id;
            result.descripcion = objmodel.Descripcion;
            result.seccionproduccion = objmodel.SeccionProduccion;
            result.imputacion = (int)objmodel.Imputacion;
            result.capacidad = objmodel.Capacidad;
            result.precio = objmodel.Precio;
            result.unidad = (int)objmodel.Unidad;

            // Añadir las líneas
            result.TareasLin.Clear();
            foreach (var item in objmodel.Lineas)
            {
                var newItem = _db.Set<TareasLin>().Create();
                newItem.empresa = item.Empresa;
                newItem.fktareas = item.Fktareas;
                newItem.id = item.Id;
                newItem.año = item.Año;
                newItem.precio = item.Precio;
                                      
                result.TareasLin.Add(newItem);
            }

            return result;
        }

        #endregion
    }
}
