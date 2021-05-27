using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class CaracteristicasConverterService : BaseConverterModel<CaracteristicasModel, Caracteristicas>
    {
        public CaracteristicasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #region API

        public override IEnumerable<IModelView> GetAll()
        {
            var result=new List<CaracteristicasModel>();
            var list= _db.Caracteristicas.Where(f => f.empresa == Empresa).ToList();
            foreach(var item in list)
            {
                result.Add(GetModelView(item) as CaracteristicasModel);
            }
            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Caracteristicas>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Caracteristicas>().Include("CaracteristicasLin").SingleOrDefault(f => f.id == id && f.empresa == Empresa);
            //var obj = _db.Set<Caracteristicas>().Where(f => f.empresa == Empresa && f.id == id).Single();
            var result = GetModelView(obj) as CaracteristicasModel;
            result.Lineas =
               obj.CaracteristicasLin.ToList().Select(
                   item =>
                       new CaracteristicasLinModel()
                       {
                           Id= item.id.ToString(),
                           Descripcion = item.descripcion,
                           Descripcion2 = item.descripcion2,
                           Descripcionabreviada = item.descripcionabreviada
                           
                       }).ToList();

            return result;
        }

         public override Caracteristicas CreatePersitance(IModelView obj)
         {
             var viewmodel = obj as CaracteristicasModel;
             var result = _db.Caracteristicas.Create();

             foreach (var item in result.GetType().GetProperties())
             {
                 if (typeof(CaracteristicasModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower()!= "Tipofamilia")
                     item.SetValue(result, viewmodel.get(item.Name));
             }

            result.CaracteristicasLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newitem = _db.CaracteristicasLin.Create();
                newitem.empresa = result.empresa;
                newitem.fkcaracteristicas = result.id;
                newitem.id = item.Id;
                newitem.descripcion = item.Descripcion;
                newitem.descripcion2 = item.Descripcion2;
                newitem.descripcionabreviada = item.Descripcionabreviada;
                result.CaracteristicasLin.Add(newitem);
            }
            return result;
         }

        public override Caracteristicas EditPersitance(IModelView obj)
        {
            var viewmodel = obj as CaracteristicasModel;
            var result = _db.Caracteristicas.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(CaracteristicasModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "Tipofamilia")
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.CaracteristicasLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newitem = _db.CaracteristicasLin.Create();
                newitem.empresa = result.empresa;
                newitem.fkcaracteristicas = result.id;
                newitem.id =item.Id;
                newitem.descripcion = item.Descripcion;
                newitem.descripcion2 = item.Descripcion2;
                newitem.descripcionabreviada = item.Descripcionabreviada;
                
                result.CaracteristicasLin.Add(newitem);
            }

            return result;
        }

        public override IModelView GetModelView(Caracteristicas obj)
        {
            var result= base.GetModelView(obj) as CaracteristicasModel;

            var familiaService = FService.Instance.GetService(typeof (FamiliasproductosModel), Context, _db);
            var familiamodel = familiaService.get(obj.id) as FamiliasproductosModel;
            result.Descripcion = familiamodel?.Descripcion;
            return result;
        }

        #endregion
    }
}
