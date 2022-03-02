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
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Converter
{
    internal class AlmacenesConverterService : BaseConverterModel<AlmacenesModel, Almacenes>
    {

        #region CTR

        public AlmacenesConverterService(IContextService context, MarfilEntities db) : base(context, db)
        {
            
        }

        #endregion

        #region Api

        public override IEnumerable<IModelView> GetAll()
        {
            return _db.Almacenes.Where(f => f.empresa == Empresa).ToList().Select(GetModelView);
        }

        public override bool Exists(string id)
        {
            return _db.Set<Almacenes>().Any(f => f.id == id && f.empresa == Empresa);
        }

        public override IModelView CreateView(string id)
        {
            //TODO - ANG - Cambio para que no se pare en debug, comprobar que funciona
            var obj = _db.Set<Almacenes>().SingleOrDefault(f => f.id == id && f.empresa == Empresa);
            if (obj == null) return null;

            var result = GetModelView(obj) as AlmacenesModel;

            var fmodel=new FModel();
            var direccionesService = FService.Instance.GetService(typeof (DireccionesLinModel), Context, _db) as DireccionesService;

            result.Direcciones = fmodel.GetModel<DireccionesModel>(Context);
            result.Direcciones.Direcciones = direccionesService.GetDirecciones(Empresa, ApplicationHelper.ALMACENDIRECCIONINT, result.Id);
            result.Direcciones.Id = Guid.NewGuid();
            result.Direcciones.Tipotercero = (int)TiposCuentas.Acreedores;
                        
            result.Lineas =
                _db.Set<AlmacenesZona>().Where(f => f.fkalmacenes == id && f.empresa == Empresa).Select(
                    f =>
                        new AlmacenesZonasModel()
                        {
                            Id = f.id,
                            Coordenadas = f.coordenadas,
                            Descripcion = f.descripcion,
                            Fktipoubicacion = f.fktipoubicacion
                        }).ToList();

            result.ArticulosStockSeguridad = 
                obj.ArticulosStockSeguridad.Where(f => f.codalmacen == id && f.empresa == Empresa).ToList().Select(f => new ArticulosStockSeguridadModel()
            {
                Id = f.id,
                Codalmacen = f.codalmacen,
                Descripcionalmacen = f.descripcionalmacen,
                Codarticulo = f.codarticulo,
                Descripcionarticulo = f.descripcionarticulo,
                Stockseguridad = (TipoStockSeguridad)f.stockseguridad,
                Stockminimo = (double)f.stockminimo,
                Stockmaximo = (double)f.stockmaximo
            }).ToList();

            return result;
        }

        public override Almacenes CreatePersitance(IModelView obj)
        {
            var viewmodel = obj as AlmacenesModel;
            var result = _db.Almacenes.Create();

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(AlmacenesModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "articulosstockseguridad")
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.AlmacenesZona.Clear();
            
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.AlmacenesZona.Create();
                newItem.empresa = viewmodel.Empresa;
                newItem.fkalmacenes = viewmodel.Id;
                newItem.id = item.Id;
                newItem.descripcion = item.Descripcion;
                newItem.coordenadas = item.Coordenadas;
                newItem.fktipoubicacion = item.Fktipoubicacion;
                result.AlmacenesZona.Add(newItem);
            }

            return result;
        }

        public override Almacenes EditPersitance(IModelView obj)
        {
            var viewmodel = obj as AlmacenesModel;
            var result = _db.Almacenes.Include("AlmacenesZona").Single(f => f.id == viewmodel.Id && f.empresa == viewmodel.Empresa);

            foreach (var item in result.GetType().GetProperties())
            {
                if (typeof(AlmacenesModel).GetProperties().Any(f => f.Name.ToLower() == item.Name.ToLower()) && item.Name.ToLower() != "articulosstockseguridad")
                    item.SetValue(result, viewmodel.get(item.Name));
            }

            result.AlmacenesZona.Clear();
            foreach (var item in viewmodel.Lineas)
            {
                var newItem = _db.AlmacenesZona.Create();
                newItem.id = item.Id;
                newItem.descripcion = item.Descripcion;
                newItem.coordenadas = item.Coordenadas;
                newItem.fktipoubicacion = item.Fktipoubicacion;
                result.AlmacenesZona.Add(newItem);
            }

            return result;
        }

        #endregion
    }
}
