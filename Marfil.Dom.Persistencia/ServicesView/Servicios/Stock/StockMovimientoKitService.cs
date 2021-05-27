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
    internal class StockMovimientoKitService : IStockService
    {
        private IContextService _context;

        public MarfilEntities Db { get; set; }
        public TipoOperacionService Tipooperacion { get; set; }

        public StockMovimientoKitService(IContextService context)
        {
            _context = context;
        }

        public void GenerarOperacion(IStockPieza entrada)
        {
            var model = entrada as MovimientosstockModel;

            var kitService = FService.Instance.GetService(typeof (KitModel), _context, Db);
            var kitobj = kitService.get(model.Fkarticulos) as KitModel;
            kitobj.Fkzonalamacen = model.Fkalmaceneszona;
            kitService.edit(kitobj);
        }
    }
}
