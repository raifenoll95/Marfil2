using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    internal class StockMovimientoRemedirService : IStockService
    {
        private readonly IContextService _context;

        public MarfilEntities Db { get; set; }
        public TipoOperacionService Tipooperacion { get; set; }

        public StockMovimientoRemedirService(IContextService context)
        {
            _context = context;
        }

        public void GenerarOperacion(IStockPieza entrada)
        {
            var model = entrada as MovimientosstockModel;
            var pieza =
                Db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid && f.fkarticulos == model.Fkarticulos && f.fkalmacenes == model.Fkalmacenes)
                    ? EditarPieza(entrada)
                    : null;

            if (pieza != null)
            {
                Db.Stockactual.AddOrUpdate(pieza);
                Db.SaveChanges();
            }
            else
                throw new ValidationException(string.Format("No se ha encontrado la pieza {0}{1} en el stock", string.IsNullOrEmpty(model.Lote) ? model.Fkarticulos : model.Lote, model.Loteid));

        }

        private Stockactual EditarPieza(IStockPieza entrada)
        {
            var model = entrada as MovimientosstockModel;
            
            var item = Db.Stockactual.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote &&
                                f.loteid == model.Loteid);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, Db) as ArticulosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, Db) as UnidadesService;
            var articuloObj = articulosService.GetArticulo(model.Fkarticulos);
            var unidadesObj = unidadesService.get(model.Fkunidadesmedida) as UnidadesModel;

            if(articuloObj.Permitemodificarlargo)
                item.largo = model.Largo;
            if (articuloObj.Permitemodificarancho)
                item.ancho = model.Ancho;
            if (articuloObj.Permitemodificargrueso)
                item.grueso = model.Grueso;
            
            var metros = UnidadesService.CalculaResultado(unidadesObj, model.Cantidad, model.Largo, model.Ancho, model.Grueso,item.metros??0);
            item.metros = metros;

            if (model.Pesoneto.HasValue)
            {
                item.pesonetolote = (model.Pesoneto ?? 0)*(item.metros ?? 0);
            }
            
            item.referenciaproveedor = model.Referenciaproveedor;
            item.fkalmaceneszona = model.Fkalmaceneszona;
            item.fkincidenciasmaterial = model.Fkincidenciasmaterial;
            item.fkcalificacioncomercial = model.Fkcalificacioncomercial;
            item.fktipograno = model.Fktipograno;
            item.fktonomaterial = model.Fktonomaterial;
            item.fkvariedades = model.Fkvariedades;

            var historicoitem = Db.Stockhistorico.Single(f =>
                       f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote &&
                               f.loteid == model.Loteid);

            if (model.Pesoneto.HasValue)
            {
                historicoitem.pesonetolote = item.pesonetolote;
            }
            historicoitem.largo = item.largo;
            historicoitem.ancho = item.ancho;
            historicoitem.grueso = item.grueso;
            historicoitem.metros = item.metros;
            historicoitem.referenciaproveedor = item.referenciaproveedor;
            historicoitem.fkalmaceneszona = item.fkalmaceneszona;
            historicoitem.fkincidenciasmaterial = item.fkincidenciasmaterial;
            historicoitem.fkcalificacioncomercial = item.fkcalificacioncomercial;
            historicoitem.fktipograno = item.fktipograno;
            historicoitem.fktonomaterial = item.fktonomaterial;
            historicoitem.fkvariedades = item.fkvariedades;
            //Db.Stockhistorico.AddOrUpdate(historicoitem);

            return item;
        }
    }
}
