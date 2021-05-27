using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class UnidadesConverterService : BaseConverterModel<UnidadesModel, Unidades>
    {
        public UnidadesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Set<Unidades>().Select(GetModelView).ToList();
        }

        public override bool Exists(string id)
        {
            return _db.Set<Unidades>().Any(f => f.id == id);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Unidades>().Single(f => f.id == id);
            return GetModelView(obj);
        }

        public override Unidades EditPersitance(IModelView obj)
        {
            var objext = obj as IModelViewExtension;
            var viewmodel = obj as UnidadesModel;
            var result = _db.Set<Unidades>().Single(f => f.id == viewmodel.Id);

            foreach (var item in result.GetType().GetProperties())
            {
                item.SetValue(result, obj.get(item.Name));
            }

            return result;
        }

        public override IModelView GetModelView(Unidades obj)
        {
            return new UnidadesModel()
            {
                Codigounidad = obj.codigounidad,
                Decimalestotales = obj.decimalestotales,
                Descripcion = obj.descripcion,
                Descripcion2 = obj.descripcion2,
                Formula = (TipoStockFormulas)obj.formula,
                Id = obj.id,
                Textocorto = obj.textocorto,
                Textocorto2 = obj.textocorto2,
                Tiposmovimientostock =(TiposStockMovimientos)obj.tiposmovimientostock,
                Tipostock = (TiposStock)obj.tipostock,
                Tipototal = (TipoStockTotal)obj.tipototal

            };
        }
    }
}
