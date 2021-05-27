using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    internal class StockReservasService : IStockService
    {
        private readonly IContextService _context;

        #region Properties

        public MarfilEntities Db { get; set; }

        public TipoOperacionService Tipooperacion { get; set; }

        #endregion

        public StockReservasService(IContextService context)
        {
            _context = context;
        }
        #region Api

        public void GenerarOperacion(IStockPieza entrada)
        {
            var model = entrada as MovimientosstockModel;

            if (!string.IsNullOrEmpty(model.Lote))
            {
                GestionPiezaConLote(model, Tipooperacion);
            }
            else
            {
                GestionPiezaSinLote(model);
            }
            Db.SaveChanges();
        }

        #endregion

        #region Helper

        private Stockactual CrearNuevaPieza(MovimientosstockModel model)
        {
            var item = Db.Stockactual.Create();
            item.empresa = model.Empresa;
            item.fecha = DateTime.Now;
            item.fkalmacenes = model.Fkalmacenes;
            item.fkalmaceneszona = model.Fkalmaceneszona;
            item.fkarticulos = model.Fkarticulos;
            item.lote = model.Lote;
            item.loteid = model.Loteid;
            item.referenciaproveedor = model.Referenciaproveedor;
            item.tag = model.Tag;
            item.fkunidadesmedida = model.Fkunidadesmedida;
            item.largo = model.Largo;
            item.ancho = model.Ancho;
            item.grueso = model.Grueso;
            item.metros = model.Metros;
            item.cantidadtotal = model.Cantidad;
            item.cantidaddisponible = model.Cantidad;
            item.integridadreferencialflag = Guid.NewGuid();

            var historicoitem = Db.Stockhistorico.SingleOrDefault(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote &&
                                f.loteid == model.Loteid)  ?? Db.Stockhistorico.Create();
            historicoitem.empresa = model.Empresa;
            historicoitem.fecha = DateTime.Now;
            historicoitem.fkalmacenes = model.Fkalmacenes;
            historicoitem.fkalmaceneszona = model.Fkalmaceneszona;
            historicoitem.fkarticulos = model.Fkarticulos;
            historicoitem.lote = model.Lote;
            historicoitem.loteid = model.Loteid;
            historicoitem.referenciaproveedor = model.Referenciaproveedor;
            historicoitem.tag = model.Tag;
            historicoitem.fkunidadesmedida = model.Fkunidadesmedida;
            historicoitem.largo = model.Largo;
            historicoitem.ancho = model.Ancho;
            historicoitem.grueso = model.Grueso;
            historicoitem.metros = model.Metros;
            historicoitem.cantidadtotal = model.Cantidad;
            historicoitem.cantidaddisponible = model.Cantidad;
            historicoitem.integridadreferencialflag = Guid.NewGuid();
            Db.Stockhistorico.AddOrUpdate(historicoitem);
            return item;
        }

        private Stockactual EditarPieza(MovimientosstockModel model)
        {
            var item = Db.Stockactual.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote &&
                                f.loteid == model.Loteid);

            var operacion = model.Cantidad > 0 ? 1 : -1;
            item.cantidaddisponible += model.Cantidad;
            item.integridadreferencialflag = Guid.NewGuid();
            //item.metros += model.Metros * operacion;

            var historicoitem = Db.Stockhistorico.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote &&
                                f.loteid == model.Loteid);

            historicoitem.cantidaddisponible = item.cantidaddisponible;
            historicoitem.integridadreferencialflag = Guid.NewGuid();
            //historicoitem.metros =item.metros;

            //Db.Stockhistorico.AddOrUpdate(historicoitem);

            return item;
        }

        private void GestionPiezaSinLote(MovimientosstockModel model)
        {
            var pieza = Db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid && f.fkarticulos == model.Fkarticulos && f.fkalmacenes == model.Fkalmacenes) ?
                          EditarPieza(model) :
                          null;
            var actualizar = true;
            if (pieza.cantidaddisponible <= 0)
            {
                var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, Db);
                var artObj = articulosService.get(model.Fkarticulos) as ArticulosModel;
                actualizar = artObj.Stocknegativoautorizado;
            }

            if (actualizar)
            {
                Db.Stockactual.AddOrUpdate(pieza);
            }
            else
                Db.Stockactual.Remove(pieza);
        }

        private void GestionPiezaConLote(MovimientosstockModel model, TipoOperacionService tipooperacion)
        {

            var pieza = Db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid) ?
                EditarPieza(model) :
                CrearNuevaPieza(model);
            var actualizar = true;
            if (pieza.cantidadtotal <= 0)
            {
                var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, Db);
                var artObj = articulosService.get(model.Fkarticulos) as ArticulosModel;
                actualizar = artObj.Stocknegativoautorizado;
            }

            if (actualizar)
            {
                Db.Stockactual.AddOrUpdate(pieza);
            }
            else
                Db.Stockactual.Remove(pieza);


        }

        #endregion
    }
}
