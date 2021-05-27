using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public class MovimientosalmacenService: IMovimientosAlmacen,IDisposable
    {
        private readonly IContextService _context;
        private readonly MarfilEntities _db;
        public MovimientosalmacenService(IContextService context)
        {
            _context = context;
            _db= MarfilEntities.ConnectToSqlServer(context.BaseDatos);
        }

        public void GenerarMovimientoAlmacen(string lote, string fkalmacen, string fkzona)
        {
            var service= new MovimientosstockService(_context,_db);
            var stockactualService = new StockactualService(_context, _db);
            service.GenerarMovimiento(stockactualService.GetArticuloPorLoteOCodigo(lote,fkalmacen,_context.Empresa), TipoOperacionService.MovimientoAlmacen, fkzona);
        }

     

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
