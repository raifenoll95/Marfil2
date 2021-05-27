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
    internal class AcabadosConverterService : BaseConverterModel<AcabadosModel, Acabados>
    {
        public AcabadosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #region API

        public override IEnumerable<IModelView> GetAll()
        {
            var vector = _db.Acabados.Where(f => f.empresa == Empresa).ToList();
            var result = new List<AcabadosModel>();

            foreach (var item in vector)
            {
                var custom = GetModelView(item) as AcabadosModel;
                result.Add(custom);
            }

            return result;
        }

        public override bool Exists(string id)
        {
            return _db.Set<Acabados>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Acabados>().Single(f => f.id == id && f.empresa == Empresa);
            var result = GetModelView(obj) as AcabadosModel;
            return result;
        }

         public override Acabados CreatePersitance(IModelView obj)
         {
             var viewmodel = obj as AcabadosModel;
             var result = _db.Acabados.Create();

             foreach (var item in result.GetType().GetProperties())
             {
                 if (typeof(AcabadosModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower()!= "Tipofamilia")
                     item.SetValue(result, viewmodel.get(item.Name));
             }

            return result;
         }

        public override Acabados EditPersitance(IModelView obj)
        {
            var viewmodel = obj as AcabadosModel;
            var result = _db.Acabados.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(AcabadosModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "Tipofamilia")
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            return result;
        }

        

        #endregion
    }
}
