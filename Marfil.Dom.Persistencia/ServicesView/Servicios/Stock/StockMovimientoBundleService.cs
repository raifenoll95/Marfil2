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
    internal class StockMovimientoBundleService : IStockService
    {
        private readonly IContextService _context;
        
        public MarfilEntities Db { get; set; }
        public TipoOperacionService Tipooperacion { get; set; }

        public StockMovimientoBundleService(IContextService context)
        {
            _context = context;
        }

        public void GenerarOperacion(IStockPieza entrada)
        {
            var model = entrada as MovimientosstockModel;

            var bundleService = FService.Instance.GetService(typeof (BundleModel), _context, Db);
            var bundleobj = bundleService.get(model.Fkarticulos) as BundleModel;
            bundleobj.Fkzonaalmacen = model.Fkalmaceneszona;
            bundleService.edit(bundleobj);
        }
    }
}
