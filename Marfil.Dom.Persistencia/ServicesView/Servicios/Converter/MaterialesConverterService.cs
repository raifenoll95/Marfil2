using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class MaterialesConverterService : BaseConverterModel<MaterialesModel, Materiales>
    {
        public MaterialesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #region API

        public override IEnumerable<IModelView> GetAll()
        {
            //var gruposMateriales = _appService.GetListGrupoMateriales();
            var familiasMateriales = _appService.GetListFamiliaMateriales();
            var vector = _db.Materiales.Where(f => f.empresa == Empresa).ToList();
            var result = new List<MaterialesModel>();

            foreach (var item in vector)
            {
                var custom = GetModelView(item) as MaterialesModel;

                custom.Gruposmateriales = _db.GrupoMateriales.Where(f => f.empresa == Empresa &&  f.cod == item.fkgruposmateriales).Select(f => f.descripcion).SingleOrDefault();
                custom.Familiamateriales =
                    familiasMateriales.FirstOrDefault(f => f.Valor == custom.Fkfamiliamateriales)?.Descripcion;

                result.Add(custom);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Materiales>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Materiales>().Include("MaterialesLin").Single(f => f.id == id && f.empresa == Empresa);
            var result = GetModelView(obj) as MaterialesModel;
            result.Lineas =
               obj.MaterialesLin.ToList().Select(
                   item =>
                       new MaterialesLinModel()
                       {
                           Codigovariedad = item.codigovariedad,
                           Descripcion = item.descripcion,
                           Descripcion2 = item.descripcion2
                       }).ToList();

            return result;
        }

         public override Materiales CreatePersitance(IModelView obj)
         {
             var viewmodel = obj as MaterialesModel;
             var result = _db.Materiales.Create();

             foreach (var item in result.GetType().GetProperties())
             {
                 if (typeof(MaterialesModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower()!= "Tipofamilia")
                     item.SetValue(result, viewmodel.get(item.Name));
             }

            result.MaterialesLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newitem = _db.MaterialesLin.Create();
                newitem.empresa = result.empresa;
                newitem.fkmateriales = result.id;
                newitem.codigovariedad = item.Codigovariedad;
                newitem.descripcion = item.Descripcion;
                newitem.descripcion2 = item.Descripcion2;
                result.MaterialesLin.Add(newitem);
            }
            return result;
         }

        public override Materiales EditPersitance(IModelView obj)
        {
            var viewmodel = obj as MaterialesModel;
            var result = _db.Materiales.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(MaterialesModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "Tipofamilia")
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.MaterialesLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newitem = _db.MaterialesLin.Create();
                newitem.empresa = result.empresa;
                newitem.fkmateriales = result.id;
                newitem.codigovariedad = item.Codigovariedad;
                newitem.descripcion = item.Descripcion;
                newitem.descripcion2 = item.Descripcion2;
                result.MaterialesLin.Add(newitem);
            }

            return result;
        }

        

        #endregion
    }
}
