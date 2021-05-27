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
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class KitConverterService : BaseConverterModel<KitModel, Kit>
    {

        #region CTR

        public KitConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        #region Api

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Kit.Where(f => f.empresa == Empresa).ToList().Select(GetModelView);}

        public override bool Exists(string id)
        {
            var intid = Funciones.Qint(id);
            return _db.Set<Kit>().Any(f => f.id == intid && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            var intid = Funciones.Qint(id);
            var obj = _db.Set<Kit>().Include("KitLin").Single(f => f.id == intid && f.empresa == Empresa);
            var result = GetModelView(obj) as KitModel;

            result.Lineas = obj.KitLin.GroupJoin(_db.Stockactual,f=>f.empresa+f.lote+f.loteid,j=> j.empresa + j.lote+j.loteid,(kitlin,stock)=> new { Kitlin= kitlin, Stock= stock}).Select(f => new KitLinModel()
            {
                Id=f.Kitlin.id,
                Fkalmacenes = f.Kitlin.fkalmacenes,
                Lote= f.Kitlin.lote,
                Loteid = f.Kitlin.loteid,
                Fkarticulos = f.Kitlin.fkarticulos,
                Descripcion = f.Kitlin.descripcion,
                Coste= f.Kitlin.coste,
                Cantidad = f.Kitlin.cantidad,
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

        public override Kit CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as KitModel;
            var result = _db.Kit.Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(KitModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.KitLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.KitLin.Create();
                newItem.empresa = viewmodel.Empresa;
                newItem.fkkit = viewmodel.Id;
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

                result.KitLin.Add(newItem);
            }

            return result;
        }

        public override Kit EditPersitance(IModelView obj)
        {
            var viewmodel = obj as KitModel;
            var result = _db.Kit.Include("KitLin").Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(KitModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()))
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.KitLin.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.KitLin.Create();
                newItem.empresa = viewmodel.Empresa;
                newItem.fkkit = viewmodel.Id;
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

                result.KitLin.Add(newItem);
            }

            return result;
        }

        

        #endregion
    }
}
