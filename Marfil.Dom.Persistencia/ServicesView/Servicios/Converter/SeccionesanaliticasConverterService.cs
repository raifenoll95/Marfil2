using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class SeccionesanaliticasConverterService:BaseConverterModel<SeccionesanaliticasModel,Seccionesanaliticas>
    {
        #region CTR
        public SeccionesanaliticasConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #endregion

        #region Existe
        public override bool Exists(string id)
        {
            return _db.Seccionesanaliticas.Any(f => f.empresa == Context.Empresa && f.id == id);
        }
        #endregion

        #region Get
        public override IModelView CreateView(string id)
        {
            var seccionesanaliticas = _db.Seccionesanaliticas.Single(f => f.empresa == Context.Empresa && f.id == id);
            return GetModelView(seccionesanaliticas);
        }
        #endregion


        #region Edit
        public override Seccionesanaliticas EditPersitance(IModelView obj)
        {
            var model = obj as SeccionesanaliticasModel;
            var efobj = _db.Set<Seccionesanaliticas>().Single(f => f.id == model.Id && f.empresa == Empresa);
            foreach (var item in efobj.GetType().GetProperties())
            {

                var yo = item.Name.FirstToUpper();
                var yoyo = obj.GetType().GetProperty(item.Name.FirstToUpper());
                var yoyoyo = obj.GetType().GetProperty(item.Name.FirstToUpper()).PropertyType;

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

        
        public override IModelView GetModelView(Seccionesanaliticas obj)
        {
            var result = base.GetModelView(obj) as SeccionesanaliticasModel;

            return result;
        }
        #endregion
    }
}
