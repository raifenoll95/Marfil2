using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    class CuadernosBancariosConverterServices : BaseConverterModel<CuadernosBancariosModel,CuadernosBancarios>
    {
        public CuadernosBancariosConverterServices(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        public override CuadernosBancarios CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as CuadernosBancariosModel;
            var result = _db.Set<CuadernosBancarios>().Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) && item.Name.ToLower() != "formato" && item.Name.ToLower() != "tipoCampo")
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
            result.formato = (byte)viewmodel.Formato;
            result.tipoCampo = (byte)viewmodel.TipoCampo;

            return result;
        }
        public override IModelView CreateView(string id)
        {
            var result = _db.CuadernosBancarios.Single(f => f.empresa == Context.Empresa && f.id == int.Parse(id));

            return GetModelView(result);
        }
        public override CuadernosBancarios EditPersitance(IModelView obj)
        {
            var viewmodel = obj as CuadernosBancariosModel;
            var result = _db.Set<CuadernosBancarios>().Single(f => f.empresa == Context.Empresa && f.id == viewmodel.Id);

            foreach (var item in result.GetType().GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)) && item.Name.ToLower() != "formato" && item.Name.ToLower() != "tipoCampo")
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
            result.formato = (byte)viewmodel.Formato;
            result.tipoCampo = (byte)viewmodel.TipoCampo;

            return result;
        }
        public override bool Exists(string id)
        {
            return _db.CuadernosBancarios.Any(f => f.empresa == Context.Empresa && f.id == int.Parse(id));
        }

        public override IModelView GetModelView(CuadernosBancarios obj)
        {
            var result = base.GetModelView(obj) as CuadernosBancariosModel;

            result.Formato = obj.formato.HasValue ? (CuadernosBancariosModel.TipoFormatos)obj.formato : CuadernosBancariosModel.TipoFormatos.Fijo;
            result.TipoCampo = obj.tipoCampo.HasValue ? (CuadernosBancariosModel.TipoCampos)obj.tipoCampo : CuadernosBancariosModel.TipoCampos.Alfanumerico;

            return result;
        }

    }
}
