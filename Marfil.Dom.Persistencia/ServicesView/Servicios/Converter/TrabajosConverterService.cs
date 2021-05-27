using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using System.Data.Entity;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class TrabajosConverterService : BaseConverterModel<TrabajosModel, Trabajos>
    {

        #region CTR

        public TrabajosConverterService(IContextService context,MarfilEntities db) : base(context,db)
        {
            
        }

        #endregion

        #region Api

        public override bool Exists(string id)
        {
            return _db.Set<Trabajos>().Any(f => f.id == id && f.empresa == Context.Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Trabajos>().Include(f => f.TrabajosLin). Single(f => f.id == id && f.empresa == Context.Empresa);
            var result = GetModelView(obj) as TrabajosModel;
            return result;
        }

        public override IModelView GetModelView(Trabajos obj)
        {
            var result = new TrabajosModel
            {
                Empresa = obj.empresa,
                Id = obj.id,
                Descripcion = obj.descripcion,
                Tipotrabajo = (TipoTrabajo)obj.tipotrabajo,
                Tipoimputacion = (TipoImputacion)obj.tipoimputacion,
                Fkacabadoinicial = obj.fkacabadoinicial,
                Fkacabadofinal = obj.fkacabadofinal,
                Fkarticulofacturable = obj.fkarticulofacturable,           
                Precio = (double)obj.precio,
                Lineas = obj.TrabajosLin.Select(f => new TrabajosLinModel() { Empresa = f.empresa, Fktrabajos = f.fktrabajos, Id = f.id, Año = f.año.Value, Precio = f.precio.Value })
            };

            return result;
        }

        //Create
        public override Trabajos CreatePersitance(IModelView obj)
        {
            var objmodel = obj as TrabajosModel;
            var result = _db.Set<Trabajos>().Create();

            result.empresa = objmodel.Empresa;
            result.id = objmodel.Id;
            result.descripcion = objmodel.Descripcion;
            result.fkacabadoinicial = objmodel.Fkacabadoinicial;
            result.fkacabadofinal = objmodel.Fkacabadofinal;
            result.tipotrabajo = (int)objmodel.Tipotrabajo;
            result.tipoimputacion = (int)objmodel.Tipoimputacion;
            result.fkarticulofacturable = objmodel.Fkarticulofacturable;
            result.precio = objmodel.Precio;

            // Añadir las líneas
            result.TrabajosLin.Clear();
            foreach (var item in objmodel.Lineas)
            {
                var newItem = _db.Set<TrabajosLin>().Create();
                newItem.empresa = item.Empresa;
                newItem.fktrabajos = item.Fktrabajos;
                newItem.id = item.Id;
                newItem.año = item.Año;
                newItem.precio = item.Precio;

                result.TrabajosLin.Add(newItem);
            }

            return result;
        }

        //Edit
        public override Trabajos EditPersitance(IModelView obj)
        {
            //var objext = obj as IModelViewExtension;
            //var objmodel = obj as TrabajosModel;
            //var result = _db.Set<Trabajos>().Single(f=>f.empresa == Context.Empresa && f.id==objmodel.Id);

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

            //return result;


            var objmodel = obj as TrabajosModel;
            var result = _db.Set<Trabajos>().Single(f => f.empresa == Context.Empresa && f.id == objmodel.Id);

            result.empresa = objmodel.Empresa;
            result.id = objmodel.Id;
            result.descripcion = objmodel.Descripcion;
            result.fkacabadoinicial = objmodel.Fkacabadoinicial;
            result.fkacabadofinal = objmodel.Fkacabadofinal;
            result.tipotrabajo = (int)objmodel.Tipotrabajo;
            result.tipoimputacion = (int)objmodel.Tipoimputacion;
            result.fkarticulofacturable = objmodel.Fkarticulofacturable;
            result.precio = objmodel.Precio;

            // Añadir las líneas
            result.TrabajosLin.Clear();
            foreach (var item in objmodel.Lineas)
            {
                var newItem = _db.Set<TrabajosLin>().Create();
                newItem.empresa = item.Empresa;
                newItem.fktrabajos = item.Fktrabajos;
                newItem.id = item.Id;
                newItem.año = item.Año;
                newItem.precio = item.Precio;

                result.TrabajosLin.Add(newItem);
            }

            return result;
        }

        #endregion
    }
}
