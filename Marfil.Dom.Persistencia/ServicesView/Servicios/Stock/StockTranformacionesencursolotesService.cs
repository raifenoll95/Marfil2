using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes;
using Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    internal class StockTranformacionesencursolotesService : IStockService
    {
        private IContextService _context;

        public MarfilEntities Db { get; set; }
        public TipoOperacionService Tipooperacion { get; set; }

        public StockTranformacionesencursolotesService(IContextService context)
        {
            _context = context;
        }

        public void GenerarOperacion(IStockPieza entrada)
        {
            var model = entrada as MovimientosstockModel;

            var stockCrud = new StockCrudService(_context);
            stockCrud.Db = Db;
            stockCrud.Tipooperacion = Tipooperacion;

            stockCrud.GenerarOperacion(model);
        }

        
    }
}
