using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class BundleConverterService : BaseConverterModel<BundleModel, Bundle>
    {

        #region CTR

        public BundleConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        #region Api

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Bundle.Where(f => f.empresa == Empresa).ToList().Select(GetModelView);}

        public override bool Exists(string id)
        {
            var vector = id.Split(';');
            var lote = vector[0];
            var identificador = vector[1];
            return _db.Set<Bundle>().Any(f => f.lote ==  lote && f.id == identificador && f.empresa == Empresa);
        }

        public override IModelView GetModelView(Bundle obj)
        {
            var model= base.GetModelView(obj) as BundleModel;
            model.Codigo = obj.id;
            return model;
        }

        public override IModelView CreateView(string id)
        {
            var vector = id.Split(';');
            var lote = vector[0];
            var identificador = vector[1];
            var obj = _db.Set<Bundle>().Include("BundleLin").Single(f =>f.lote==lote && f.id == identificador && f.empresa == Empresa);
            var result = GetModelView(obj) as BundleModel;

            result.Lineas = obj.BundleLin.GroupJoin(_db.Stockactual, f => f.empresa + f.lote + f.loteid, j => j.empresa + j.lote + j.loteid, (kitlin, stock) => new { Kitlin = kitlin, Stock = stock }).Select(f => new BundleLinModel()
            {
                Id = f.Kitlin.id,
                Fkalmacenes = f.Kitlin.fkalmacenes,
                Lote = f.Kitlin.lote,
                Loteid = f.Kitlin.loteid,
                Fkarticulos = f.Kitlin.fkarticulos,
                Descripcion = f.Kitlin.descripcion,
                Coste = f.Kitlin.coste,
                Cantidad = (int?) f.Kitlin.cantidad,
                Largo = f.Stock.Any() ? f.Stock.First().largo : f.Kitlin.largo,
                Ancho = f.Stock.Any() ? f.Stock.First().ancho : f.Kitlin.ancho,
                Grueso = f.Stock.Any() ? f.Stock.First().grueso : f.Kitlin.grueso,
                Metros = f.Stock.Any() ? f.Stock.First().metros : f.Kitlin.metros,
                Fkunidades = f.Kitlin.fkunidades,
                Decimalesprecio = f.Kitlin.decimalesprecio,
                Decimalesunidades = f.Kitlin.decimalesunidades
            }).ToList();
           
            
            
            return result;
        }

        public override Bundle CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as BundleModel;
            var result = _db.Bundle.Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(BundleModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }
            result.id = viewmodel.Codigo;
            result.BundleLin.Clear();
            if(viewmodel.Lineas!=null)
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.BundleLin.Create();
                newItem.empresa = viewmodel.Empresa;
                newItem.fkbundle = viewmodel.Codigo;
                newItem.id = item.Id;
                newItem.fkalmacenes = result.fkalmacen;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.loteid = item.Loteid;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.coste = item.Coste ??0 ;
                newItem.cantidad = item.Cantidad ?? 0;
                newItem.largo = item.Largo??0;
                newItem.ancho = item.Ancho ?? 0;
                newItem.grueso = item.Grueso ?? 0;
                newItem.metros = item.Metros ?? 0;
                newItem.fkunidades = item.Fkunidades;
                newItem.decimalesunidades = item.Decimalesunidades ?? 0;
                newItem.decimalesprecio = item.Decimalesprecio ?? 0;

                result.BundleLin.Add(newItem);
            }

            return result;
        }

        public override Bundle EditPersitance(IModelView obj)
        {
            var viewmodel = obj as BundleModel;
            var result = _db.Bundle.Include("BundleLin").Single(f =>f.lote==viewmodel.Lote && f.id == viewmodel.Codigo && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(BundleModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }
           
            result.BundleLin.Clear();
            if (viewmodel.Lineas != null)
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.BundleLin.Create();
                newItem.empresa = viewmodel.Empresa;
                newItem.fkbundle = viewmodel.Codigo;
                newItem.id = item.Id;
                newItem.fkalmacenes = item.Fkalmacenes;
                newItem.descripcion = item.Descripcion;
                newItem.lote = item.Lote;
                newItem.loteid = item.Loteid;
                newItem.fkarticulos = item.Fkarticulos;
                newItem.cantidad = item.Cantidad ?? 0;
                newItem.coste = item.Coste ?? 0;
                newItem.largo = item.Largo ?? 0;
                newItem.ancho = item.Ancho ?? 0;
                newItem.grueso = item.Grueso ?? 0;
                newItem.metros = item.Metros ?? 0;
                newItem.fkunidades = item.Fkunidades;
                newItem.decimalesunidades = item.Decimalesunidades ?? 0;
                newItem.decimalesprecio = item.Decimalesprecio ?? 0;

                result.BundleLin.Add(newItem);
            }

            return result;
        }

        

        #endregion
    }
}
