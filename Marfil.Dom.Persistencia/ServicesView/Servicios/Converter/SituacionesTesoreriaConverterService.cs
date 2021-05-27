using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class SituacionesTesoreriaConverterService : BaseConverterModel<SituacionesTesoreriaModel, SituacionesTesoreria>
    {
        public SituacionesTesoreriaConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        //public override IEnumerable<IModelView> GetAll()
        //{
        //    return _db.Set<SituacionesTesoreria>().Select(GetModelView);
        //}

        public override IModelView CreateView(string cod)
        {
            var obj = _db.Set<SituacionesTesoreria>().Single(f => f.cod == cod);
            var result = GetModelView(obj) as SituacionesTesoreriaModel;
            return result;
        }

        public override SituacionesTesoreria CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as SituacionesTesoreriaModel;
            var result = _db.Set<SituacionesTesoreria>().Create();
            result.cod = viewmodel.Cod;
            result.descripcion = viewmodel.Descripcion;
            result.descripcion2 = viewmodel.Descripcion2;          
            result.valorinicialcobros = viewmodel.Valorinicialcobros;
            result.valorinicialpagos = viewmodel.Valorinicialpagos;
            result.editable = viewmodel.Editable;
            result.prevision = (int)viewmodel.Prevision;
            result.remesable = viewmodel.Remesable;
            result.riesgo = (int)viewmodel.Riesgo;
            return result;
        }

        public override SituacionesTesoreria EditPersitance(IModelView obj)
        {
            var viewmodel = obj as SituacionesTesoreriaModel;
            var result = _db.SituacionesTesoreria.Where(f => f.cod == viewmodel.Cod).Single();
            result.cod = viewmodel.Cod;
            result.descripcion = viewmodel.Descripcion;
            result.descripcion2 = viewmodel.Descripcion2;
            result.valorinicialcobros = viewmodel.Valorinicialcobros;
            result.valorinicialpagos = viewmodel.Valorinicialpagos;
            result.editable = viewmodel.Editable;
            result.prevision = (int)viewmodel.Prevision;
            result.remesable = viewmodel.Remesable;
            result.riesgo = (int)viewmodel.Riesgo;
            return result;
        }

        public override IModelView GetModelView(SituacionesTesoreria obj)
        {
            var result = new SituacionesTesoreriaModel
            {
                Cod = obj.cod,
                Descripcion = obj.descripcion,
                Descripcion2 = obj.descripcion2,
                Valorinicialcobros = obj.valorinicialcobros.Value,
                Valorinicialpagos = obj.valorinicialpagos.Value,
                Prevision = (Marfil.Dom.Persistencia.Model.FicherosGenerales.TipoPrevision)obj.prevision,
                Editable = obj.editable.Value,
                Remesable = obj.remesable.Value,
                Riesgo = (Marfil.Dom.Persistencia.Model.FicherosGenerales.TipoRiesgo)obj.riesgo           
            };
           
            return result;

        }
    }
}
