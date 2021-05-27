using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using RObras =Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Obras;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class ObrasConverterService : BaseConverterModel<ObrasModel, Obras>
    {
        public ObrasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool Exists(string id)
        {
            var idInt = int.Parse(id);
            return _db.Set<Obras>().Any(f => f.id == idInt && f.empresa == Empresa);
        }

        public override IEnumerable<IModelView> GetAll()
        {
            var list= _db.Obras.Where(f => f.empresa == Empresa).ToList().Select(f => GetModelView(f) as ObrasModel).ToList();

            foreach (var item in list)
            {
                item.Nombrecliente = _db.Cuentas.SingleOrDefault(f => f.empresa == Empresa && f.id == item.Fkclientes)?.descripcion;
            }

            return list;
        }

        public override IModelView CreateView(string id)
        {
            var idInt = int.Parse(id);
            var obj = _db.Set<Obras>().Single(f => f.id == idInt && f.empresa == Empresa);
            return GetModelView(obj) as ObrasModel;
        }

        public override Obras EditPersitance(IModelView obj)
        {
            var model = obj as ObrasModel;
            var efobj = _db.Set<Obras>().Single(f => f.id == model.Id && f.empresa == Empresa);
            foreach (var item in efobj.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
                {
                    item.SetValue(efobj, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(efobj, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(efobj, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            return efobj;
        }

        public override IModelView GetModelView(Obras obj)
        {
            var result= base.GetModelView(obj) as ObrasModel;
            
            return result;
        }
    }
}
