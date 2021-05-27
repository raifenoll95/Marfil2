using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Stock;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    internal class StockMovimientoAlmacenService : IStockService
    {
        private readonly IContextService _context;
        public MarfilEntities Db { get; set; }
        public TipoOperacionService Tipooperacion { get; set; }

        public StockMovimientoAlmacenService(IContextService context)
        {
            _context = context;
        }

        public void GenerarOperacion(IStockPieza entrada)
        {
            var model = entrada as MovimientosstockModel;
            
            ValidarPerteneceKitOBundle(model);

            var pieza =
                Db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid && f.fkarticulos==model.Fkarticulos && f.fkalmacenes==model.Fkalmacenes)
                    ? EditarPieza(model)
                    : null;

            if(pieza != null)
            {
                Db.Stockactual.AddOrUpdate(pieza);
                Db.SaveChanges();
            }
            else
                throw new ValidationException(string.Format("No se ha encontrado la pieza {0}{1} en el stock",string.IsNullOrEmpty(model.Lote) ? model.Fkarticulos : model.Lote, model.Loteid));
        }

        private void ValidarPerteneceKitOBundle(MovimientosstockModel model)
        {
            if (Db.Kit.Include("KitLin").Any(f => (f.estado == (int) EstadoKit.EnProceso || f.estado == (int) EstadoKit.Montado) &&  f.empresa == model.Empresa && f.KitLin.Any(j=>j.lote == model.Lote && j.loteid==model.Loteid && j.empresa==model.Empresa)))
            {
                var kitobj =
                    Db.Kit.Include("KitLin")
                        .First(
                            f =>
                                (f.estado == (int) EstadoKit.EnProceso || f.estado == (int) EstadoKit.Montado) &&
                                f.empresa == model.Empresa &&
                                f.KitLin.Any(
                                    j => j.lote == model.Lote && j.loteid == model.Loteid && j.empresa == model.Empresa));
                throw new ValidationException(string.Format("No se puede mover la pieza porque pertenece al Kit {0}", kitobj.referencia));
            }

            if (Db.Bundle.Include("BundleLin").Any(f =>  f.empresa == model.Empresa && f.BundleLin.Any(j => j.lote == model.Lote && j.loteid == model.Loteid && j.empresa == model.Empresa)))
            {
                var bundleobj =
                    Db.Bundle.Include("BundleLin")
                        .First(
                            f =>
                                f.empresa == model.Empresa &&
                                f.BundleLin.Any(
                                    j => j.lote == model.Lote && j.loteid == model.Loteid && j.empresa == model.Empresa));
                throw new ValidationException(string.Format("No se puede mover la pieza porque pertenece al bundle {0} {1}", bundleobj.lote, bundleobj.id));
            }
        }

        private Stockactual EditarPieza(MovimientosstockModel model)
        {
            var item = Db.Stockactual.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote &&
                                f.loteid == model.Loteid);

            item.fkalmaceneszona = model.Fkalmaceneszona;

            var historicoitem = Db.Stockhistorico.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote &&
                                f.loteid == model.Loteid);
            
            historicoitem.fkalmaceneszona = item.fkalmaceneszona;
            
           // Db.Stockhistorico.AddOrUpdate(historicoitem);
            return item;
        }

        
    }
}
