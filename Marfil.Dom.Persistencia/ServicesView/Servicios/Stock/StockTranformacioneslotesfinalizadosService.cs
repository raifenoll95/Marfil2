using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes;
using Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    internal class StockTranformacioneslotesfinalizadosService : IStockService
    {
        private IContextService _context;

        public MarfilEntities Db { get; set; }
        public TipoOperacionService Tipooperacion { get; set; }

        public StockTranformacioneslotesfinalizadosService(IContextService context)
        {
            _context = context;
        }

        public void GenerarOperacion(IStockPieza entrada)
        {
            var ms = entrada as MovimientosstockModel;
            var serializer = new Serializer<TransformacioneslotesDiarioStockSerializable>();
            var diario = serializer.SetXml(ms.Documentomovimiento);
            var historicoStock = Db.Stockhistorico.SingleOrDefault(
                    f => f.empresa == ms.Empresa && f.lote == ms.Lote && f.loteid == ms.Loteid && f.fkarticulos == ms.Fkarticulos);
            var unidadesService = FService.Instance.GetService(typeof (UnidadesModel), _context, Db) as UnidadesService;
            var unidadesModel = unidadesService.get(ms.Fkunidadesmedida) as UnidadesModel;
            if (historicoStock != null)
            {

                var nuevapiezaStock = Db.Stockactual.Create();
                var properties = nuevapiezaStock.GetType().GetProperties();
                foreach (var item in properties)
                    item.SetValue(nuevapiezaStock, historicoStock.GetType().GetProperty(item.Name).GetValue(historicoStock));
                nuevapiezaStock.largo = ms.Largo;
                nuevapiezaStock.ancho = ms.Ancho;
                nuevapiezaStock.grueso = ms.Grueso;
                nuevapiezaStock.metros = ms.Metros;
                nuevapiezaStock.costeacicionalvariable = (nuevapiezaStock.costeacicionalvariable ?? 0) + (ms.Costeadicionalvariable ?? 0);
                nuevapiezaStock.costeadicionalmaterial = (nuevapiezaStock.costeadicionalmaterial ?? 0) + (ms.Costeadicionalmaterial ?? 0);
                nuevapiezaStock.costeadicionalotro = (nuevapiezaStock.costeadicionalotro ?? 0) + (ms.Costeadicionalotro ?? 0);
                nuevapiezaStock.costeadicionalportes = (nuevapiezaStock.costeadicionalportes ?? 0) + (ms.Costeadicionalportes ?? 0);
                nuevapiezaStock.cantidaddisponible = ms.Cantidad;
                nuevapiezaStock.cantidadtotal = ms.Cantidad;
                nuevapiezaStock.metros = UnidadesService.CalculaResultado(unidadesModel, Math.Abs(ms.Cantidad),
                    nuevapiezaStock.largo, nuevapiezaStock.ancho, nuevapiezaStock.grueso, nuevapiezaStock.metros ?? 0);
                nuevapiezaStock.fkarticulos = diario.Fkarticulosnuevo;
                Db.Stockactual.Add(nuevapiezaStock);
            }

            if (historicoStock != null)
            {

                var nuevapiezaStock = Db.Stockhistorico.Create();
                var properties = historicoStock.GetType().GetProperties();
                foreach (var item in properties)
                    item.SetValue(nuevapiezaStock, historicoStock.GetType().GetProperty(item.Name).GetValue(historicoStock));
                nuevapiezaStock.largo = ms.Largo;
                nuevapiezaStock.ancho = ms.Ancho;
                nuevapiezaStock.grueso = ms.Grueso;
                nuevapiezaStock.metros = ms.Metros;
                nuevapiezaStock.fkarticulos = diario.Fkarticulosnuevo;
                nuevapiezaStock.costeacicionalvariable = (nuevapiezaStock.costeacicionalvariable ?? 0) + (ms.Costeadicionalvariable ?? 0);
                nuevapiezaStock.costeadicionalmaterial = (nuevapiezaStock.costeadicionalmaterial ?? 0) + (ms.Costeadicionalmaterial ?? 0);
                nuevapiezaStock.costeadicionalotro = (nuevapiezaStock.costeadicionalotro ?? 0) + (ms.Costeadicionalotro ?? 0);
                nuevapiezaStock.costeadicionalportes = (nuevapiezaStock.costeadicionalportes ?? 0) + (ms.Costeadicionalportes ?? 0);
                nuevapiezaStock.cantidaddisponible = ms.Cantidad;
                nuevapiezaStock.cantidadtotal = ms.Cantidad;
                nuevapiezaStock.metros = UnidadesService.CalculaResultado(unidadesModel, Math.Abs(ms.Cantidad),
                   nuevapiezaStock.largo, nuevapiezaStock.ancho, nuevapiezaStock.grueso, nuevapiezaStock.metros ?? 0);

                Db.Stockhistorico.Remove(historicoStock);
                Db.Stockhistorico.Add(nuevapiezaStock);
            }
        }
    }
}
