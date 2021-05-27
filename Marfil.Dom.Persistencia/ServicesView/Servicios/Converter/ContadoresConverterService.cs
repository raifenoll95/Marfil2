using Marfil.Dom.Persistencia.Model.Configuracion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class ContadoresConverterService : BaseConverterModel<ContadoresModel, Contadores>
    {
        public ContadoresConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #region API

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Contadores.Where(f => f.empresa == Empresa).ToList().Select(f=>GetModelView(f));

        }

        public override bool Exists(string id)
        {
            return _db.Set<Contadores>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<Contadores>().Include("ContadoresLin").Single(f => f.id == id && f.empresa == Empresa);
            var result = GetModelView(obj) as ContadoresModel;
            result.Lineas =
               obj.ContadoresLin.ToList().Select(
                   item =>
                       new ContadoresLinModel()
                       {
                           Id = item.id,
                           Valor = item.valor,
                           Longitud = item.longitud?? 0,
                           Tiposegmento = item.tiposegmento.HasValue? (TiposSegmentos)item.tiposegmento.Value:TiposSegmentos.Constante
                       }).ToList();

            return result;
        }

        public override Contadores CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as ContadoresModel;
            var result = _db.Contadores.Create();
            
            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(ContadoresModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "tipoinicio" && item.Name.ToLower() != "tipocontador")
                    item.SetValue(result, viewmodel.get(item.Name));
            }
            result.tipocontador = (int)viewmodel.Tipocontador;
            result.tipoinicio = (int)viewmodel.Tipoinicio;

            result.ContadoresLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newitem = _db.ContadoresLin.Create();
                newitem.empresa = result.empresa;
                newitem.fkcontadores = result.id;
                newitem.id = item.Id;
                newitem.valor = item.Valor;
                newitem.longitud = item.Longitud;
                newitem.tiposegmento = (int)item.Tiposegmento;
                result.ContadoresLin.Add(newitem);
            }
            return result;
        }

        public override Contadores EditPersitance(IModelView obj)
        {
            var viewmodel = obj as ContadoresModel;
            var result = _db.Contadores.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(ContadoresModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "tipoinicio" && item.Name.ToLower() != "tipocontador")
                    item.SetValue(result, viewmodel.get(item.Name));
            }
            result.tipocontador = (int) viewmodel.Tipocontador;
            result.tipoinicio = (int)viewmodel.Tipoinicio;
            result.ContadoresLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newitem = _db.ContadoresLin.Create();
                newitem.empresa = result.empresa;
                newitem.fkcontadores = result.id;
                newitem.id = item.Id;
                newitem.valor = item.Valor;
                newitem.longitud = item.Longitud;
                newitem.tiposegmento = (int)item.Tiposegmento;
                result.ContadoresLin.Add(newitem);
            }

            return result;
        }

        public override IModelView GetModelView(Contadores obj)
        {
            var result= base.GetModelView(obj) as ContadoresModel;
            result.Tipocontador = obj.tipocontador.HasValue ? (TipoContador)obj.tipocontador.Value: TipoContador.Multidocumento;
            return result;
        }

        #endregion
    }
}
