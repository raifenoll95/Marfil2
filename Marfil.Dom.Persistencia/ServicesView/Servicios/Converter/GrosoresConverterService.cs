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
    internal class GrosoresConverterService : BaseConverterModel<GrosoresModel, Grosores>
    {
        public GrosoresConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #region API

        public override IEnumerable<IModelView> GetAll()
        {
            var vector = _db.Grosores.Where(f => f.empresa == Empresa).ToList();
            var result = new List<GrosoresModel>();

            foreach (var item in vector)
            {
                var custom = GetModelView(item) as GrosoresModel;
                result.Add(custom);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Grosores>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Grosores>().Single(f => f.id == id && f.empresa == Empresa);
            var result = GetModelView(obj) as GrosoresModel;
            return result;
        }

         public override Grosores CreatePersitance(IModelView obj)
         {
             var viewmodel = obj as GrosoresModel;
             var result = _db.Grosores.Create();

             foreach (var item in result.GetType().GetProperties())
             {
                 if (typeof(GrosoresModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower()!= "Tipofamilia")
                     item.SetValue(result, viewmodel.get(item.Name));
             }

            return result;
         }

        public override Grosores EditPersitance(IModelView obj)
        {
            var viewmodel = obj as GrosoresModel;
            var result = _db.Grosores.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(GrosoresModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "Tipofamilia")
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            return result;
        }

        

        #endregion
    }
}
