using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    internal class StockTraspasosalmacenService : IStockService
    {
        private IContextService _context;

        public MarfilEntities Db { get; set; }
        public TipoOperacionService Tipooperacion { get; set; }

        public StockTraspasosalmacenService(IContextService context)
        {
            _context = context;
        }

        public void GenerarOperacion(IStockPieza entrada)
        {
            var serializer = new Serializer<TraspasosalmacenDiarioStockSerializable>();
            var model = entrada as MovimientosstockModel;
            var trazabilidad = serializer.SetXml(model.Documentomovimiento);
            var stockCrud = new StockCrudService(_context);
            stockCrud.Db = Db;
            stockCrud.Tipooperacion = Tipooperacion;
            var almacenorigen = trazabilidad.Fkalmacenorigen;
            var almacendestino = trazabilidad.Fkalmacendestino;
            
            model.Fkalmacenes = almacenorigen;
            model.Cantidad *= -1;
            stockCrud.GenerarOperacion(model);
            EliminarDeHistorico(model);
            GenerarMovimientostock(model,TipoOperacionService.ActualizarTraspasosalmacen);
            

            model.Fkalmacenes = almacendestino;
            model.Cantidad *= -1;
            stockCrud.GenerarOperacion(model);
           
        }

        private void EliminarDeHistorico(MovimientosstockModel model)
        {
            var pieza =
                Db.Stockhistorico.Single(
                    f =>
                        f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid &&
                        f.fkalmacenes == model.Fkalmacenes);
            Db.Stockhistorico.Remove(pieza);
            Db.SaveChanges();
        }

        private void GenerarMovimientostock(IStockPieza entrada, TipoOperacionService tipooperacion)
        {
            var model = entrada as MovimientosstockModel;
            var item = Db.Movimientosstock.Create();
            item.empresa = model.Empresa;
            item.fecha = DateTime.Now;
            item.fkalmacenes = model.Fkalmacenes;
            item.fkarticulos = model.Fkarticulos;
            item.referenciaproveedor = model.Referenciaproveedor;
            item.fkcontadorlote = model.Fkcontadorlote;
            item.lote = model.Lote;
            item.loteid = model.Loteid;
            item.tag = model.Tag;
            item.fkunidadesmedida = model.Fkunidadesmedida;
            item.cantidad = model.Cantidad;
            item.largo = model.Largo;
            item.ancho = model.Ancho;
            item.grueso = model.Grueso;
            item.fkalmaceneszona = model.Fkalmaceneszona;
            item.fkcalificacioncomercial = model.Fkcalificacioncomercial;
            item.fktipograno = model.Fktipograno;
            item.fktonomaterial = model.Fktonomaterial;
            item.fkincidenciasmaterial = model.Fkincidenciasmaterial;
            item.documentomovimiento = model.Documentomovimiento;
            item.tipooperacion = (int?)tipooperacion;
            item.integridadreferencialflag = Guid.NewGuid();
            item.fkusuarios = model.Fkusuarios;
            item.fkvariedades = model.Fkvariedades;
            item.categoriamovimiento = (int)FStockService.Instance.GetCategoriaMovimientos(tipooperacion);
            Db.Movimientosstock.Add(item);
        }
        
    }
}
