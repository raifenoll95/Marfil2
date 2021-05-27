using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using RFicheros =Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Ficheros;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class FicherosConverterService : BaseConverterModel<FicherosGaleriaModel, Ficheros>
    {
        public FicherosConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override bool Exists(string id)
        {
            var idInt = new Guid(id);
            return _db.Set<Ficheros>().Any(f => f.id == idInt && f.empresa==Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var guidid=new Guid(id);
            var obj = _db.Ficheros.Single(f => f.empresa == Empresa && f.id == guidid);
            return GetModelView(obj);
        }

        public override Ficheros EditPersitance(IModelView obj)
        {
            var model = obj as FicherosGaleriaModel;
            var result = _db.Ficheros.Single(f => f.empresa == Empresa && f.id == model.Id);

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>))
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
    }
}
