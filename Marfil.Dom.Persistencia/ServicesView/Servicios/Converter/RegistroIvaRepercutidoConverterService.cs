using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Iva;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class RegistroIvaRepercutidoConverterService : BaseConverterModel<RegistroIvaRepercutidoModel, Persistencia.RegistroIVARepercutido>
    {
        public RegistroIvaRepercutidoConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Persistencia.RegistroIVARepercutido>().Select(GetModelView);
        }

        public override IModelView CreateView(string id)
        {
            var idnum = Int32.Parse(id);
            var obj = _db.Set<Persistencia.RegistroIVARepercutido>().Single(f => f.empresa == Empresa && f.id == idnum);
            var result = GetModelView(obj) as RegistroIvaRepercutidoModel;
            return result;
        }

        public override RegistroIVARepercutido CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as RegistroIvaRepercutidoModel;
            var result = _db.RegistroIVARepercutido.Create();

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

            return result;
        }

        public override RegistroIVARepercutido EditPersitance(IModelView obj)
        {
            var viewmodel = obj as RegistroIvaRepercutidoModel;
            var result = _db.RegistroIVARepercutido.Single(f => f.empresa == viewmodel.Empresa && f.id == viewmodel.Id);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(RegistroIvaRepercutidoModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            return result;
        }

        public override IModelView GetModelView(RegistroIVARepercutido viewmodel)
        {
            var result = base.GetModelView(viewmodel) as RegistroIvaRepercutidoModel;

            return result;
        }
    }
}
