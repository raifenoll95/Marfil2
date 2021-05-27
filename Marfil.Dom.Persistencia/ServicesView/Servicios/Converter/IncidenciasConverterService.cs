using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class IncidenciasConverterService : BaseConverterModel<IncidenciasModel, Incidencias>
    {
        #region CTR

        public IncidenciasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        #region Api

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Incidencias.Where(f => f.empresa == Empresa).ToList().Select(GetModelView);
        }

        public override bool Exists(string id)
        {
            return _db.Set<Incidencias>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Incidencias>().Single(f => f.id == id && f.empresa == Empresa);
            return GetModelView(obj) as IncidenciasModel;
        }

        public override Incidencias EditPersitance(IModelView obj)
        {
            var viewmodel = obj as IncidenciasModel;
            var result = _db.Incidencias.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);
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


        #endregion
    }
}

