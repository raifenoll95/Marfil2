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
    internal class ContadoresLotesConverterService : BaseConverterModel<ContadoresLotesModel, ContadoresLotes>
    {
        public ContadoresLotesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
        }

        #region API

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.ContadoresLotes.Where(f => f.empresa == Empresa).ToList().Select(f=>GetModelView(f));

        }

        public override bool Exists(string id)
        {
            return _db.Set<ContadoresLotes>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var obj = _db.Set<ContadoresLotes>().Include("ContadoresLotesLin").Single(f => f.id == id && f.empresa == Empresa);
            var result = GetModelView(obj) as ContadoresLotesModel;
            result.Lineas =
               obj.ContadoresLotesLin.ToList().Select(
                   item =>
                       new ContadoresLotesLinModel()
                       {
                           Id = item.id,
                           Valor = item.valor,
                           Longitud = item.longitud?? 0,
                           Tiposegmento = item.tiposegmento.HasValue? (TiposLoteSegmentos)item.tiposegmento.Value: TiposLoteSegmentos.Constante
                       }).ToList();

            return result;
        }

        public override ContadoresLotes CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as ContadoresLotesModel;
            var result = _db.ContadoresLotes.Create();
            
            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(ContadoresLotesModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "tipoinicio" && item.Name.ToLower() != "tipocontador")
                    item.SetValue(result, viewmodel.get(item.Name));
            }
            result.tipocontador = (int)viewmodel.Tipocontador;
            result.tipoinicio = (int)viewmodel.Tipoinicio;

            result.ContadoresLotesLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newitem = _db.ContadoresLotesLin.Create();
                newitem.empresa = result.empresa;
                newitem.fkcontadores = result.id;
                newitem.id = item.Id;
                newitem.valor = item.Valor;
                newitem.longitud = item.Longitud;
                newitem.tiposegmento = (int)item.Tiposegmento;
                result.ContadoresLotesLin.Add(newitem);
            }
            return result;
        }

        public override ContadoresLotes EditPersitance(IModelView obj)
        {
            var viewmodel = obj as ContadoresLotesModel;
            var result = _db.ContadoresLotes.Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(ContadoresLotesModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "tipoinicio" && item.Name.ToLower() != "tipocontador")
                    item.SetValue(result, viewmodel.get(item.Name));
            }
            result.tipocontador = (int) viewmodel.Tipocontador;
            result.tipoinicio = (int)viewmodel.Tipoinicio;
            result.ContadoresLotesLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newitem = _db.ContadoresLotesLin.Create();
                newitem.empresa = result.empresa;
                newitem.fkcontadores = result.id;
                newitem.id = item.Id;
                newitem.valor = item.Valor;
                newitem.longitud = item.Longitud;
                newitem.tiposegmento = (int)item.Tiposegmento;
                result.ContadoresLotesLin.Add(newitem);
            }

            return result;
        }

        public override IModelView GetModelView(ContadoresLotes obj)
        {
            var result= base.GetModelView(obj) as ContadoresLotesModel;
            result.Tipocontador = obj.tipocontador.HasValue ? (TipoLoteContador)obj.tipocontador.Value: TipoLoteContador.Multidocumento;
            return result;
        }

        #endregion
    }
}
