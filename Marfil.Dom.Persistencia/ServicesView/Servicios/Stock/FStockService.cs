using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Stock;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    internal class FStockService
    {
        #region Singleton

        private static FStockService _instance;
        public static FStockService Instance
        {
            get
            {
                if (_instance == null)
                    _instance =new FStockService();

                return _instance;
            }
        }

        #endregion

        #region Members

        private readonly Dictionary<TipoOperacionService, Type> _dictionary;
        private readonly Dictionary<TipoOperacionService, CategoriaMovimientos> _dictionaryCategoria;
        #endregion

        private FStockService()
        {
            _dictionary=new Dictionary<TipoOperacionService, Type>();
            _dictionaryCategoria = new Dictionary<TipoOperacionService, CategoriaMovimientos>();
            #region StockCrudService
            //RecepcionStock
            _dictionary.Add(TipoOperacionService.InsertarRecepcionStock,typeof(StockCrudService));
            _dictionary.Add(TipoOperacionService.ActualizarRecepcionStock, typeof( StockCrudService));
            _dictionary.Add(TipoOperacionService.EliminarRecepcionStock, typeof( StockCrudService));
            //EntregaStock
            _dictionary.Add(TipoOperacionService.InsertarEntregaStock, typeof(StockCrudService));
            _dictionary.Add(TipoOperacionService.ActualizarEntregaStock, typeof(StockCrudService));
            _dictionary.Add(TipoOperacionService.EliminarEntregaStock, typeof(StockCrudService));
            //transformaciones
            _dictionary.Add(TipoOperacionService.InsertarTransformacionEntradaStock, typeof(StockCrudService));
            _dictionary.Add(TipoOperacionService.ActualizarTransformacionEntradaStock, typeof(StockCrudService));
            _dictionary.Add(TipoOperacionService.EliminarTransformacionEntradaStock, typeof(StockCrudService));
            _dictionary.Add(TipoOperacionService.InsertarTransformacionSalidaStock, typeof(StockCrudService));
            _dictionary.Add(TipoOperacionService.ActualizarTransformacionSalidaStock, typeof(StockCrudService));
            _dictionary.Add(TipoOperacionService.EliminarTransformacionSalidaStock, typeof(StockCrudService));

            #endregion

            #region StockDevolucionRecepcionStockService
            _dictionary.Add(TipoOperacionService.InsertarRecepcionStockDevolucion, typeof(StockDevolucionRecepcionStockService));
            _dictionary.Add(TipoOperacionService.ActualizarRecepcionStockDevolucion, typeof(StockDevolucionRecepcionStockService));


            #endregion

            #region StockMovimientoAlmacenService
            _dictionary.Add(TipoOperacionService.MovimientoAlmacen, typeof( StockMovimientoAlmacenService));
            _dictionary.Add(TipoOperacionService.MovimientoKit, typeof( StockMovimientoKitService));
            _dictionary.Add(TipoOperacionService.MovimientoBundle, typeof( StockMovimientoBundleService));
            _dictionary.Add(TipoOperacionService.MovimientoRemedir, typeof( StockMovimientoRemedirService));

            #endregion

            #region StockReservasService
            _dictionary.Add(TipoOperacionService.InsertarReservaStock, typeof( StockReservasService));
            _dictionary.Add(TipoOperacionService.ActualizarReservaStock, typeof( StockReservasService));
            _dictionary.Add(TipoOperacionService.EliminarReservaStock, typeof( StockReservasService));

            #endregion

            #region StockTraspasosalmacenService
            _dictionary.Add(TipoOperacionService.InsertarTraspasosalmacen, typeof( StockTraspasosalmacenService));
            _dictionary.Add(TipoOperacionService.ActualizarTraspasosalmacen, typeof(StockTraspasosalmacenModificarCostesService));
            _dictionary.Add(TipoOperacionService.EliminarTraspasosalmacen, typeof( StockTraspasosalmacenService));

            #endregion

            #region StockTranformacionesencursolotesService

            _dictionary.Add(TipoOperacionService.InsertarTransformacionloteStock, typeof(StockTranformacionesencursolotesService));
            _dictionary.Add(TipoOperacionService.ActualizarTransformacionloteStock, typeof(StockTranformacionesencursolotesService));
            
            #endregion

            #region StockTransformacioneslotesModificarCostesService
            _dictionary.Add(TipoOperacionService.ActualizarcosteTransformacionloteStock, typeof(StockTransformacioneslotesModificarCostesService));
            #endregion

            #region StockTranformacioneslotesfinalizadosService
            _dictionary.Add(TipoOperacionService.FinalizarTransformacionloteStock, typeof(StockTranformacioneslotesfinalizadosService));

            #endregion

            #region StockDevolucionEntregaStockService
            _dictionary.Add(TipoOperacionService.InsertarDevolucionEntregaStock, typeof(StockDevolucionEntregaStockService));
            _dictionary.Add(TipoOperacionService.ActualizarEntregaStockDevolucion, typeof(StockDevolucionEntregaStockService));
            #endregion
            
            //CategoriaMovientos
            _dictionaryCategoria.Add(TipoOperacionService.InsertarRecepcionStock, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.ActualizarRecepcionStock, CategoriaMovimientos.Secudario);
            _dictionaryCategoria.Add(TipoOperacionService.EliminarRecepcionStock, CategoriaMovimientos.Principal);


            #region RecepcionStockDevolucion
            _dictionaryCategoria.Add(TipoOperacionService.InsertarRecepcionStockDevolucion, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.ActualizarRecepcionStockDevolucion, CategoriaMovimientos.Secudario);
            #endregion

            #region EntregasStockDevolucion
            _dictionaryCategoria.Add(TipoOperacionService.InsertarDevolucionEntregaStock, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.ActualizarEntregaStockDevolucion, CategoriaMovimientos.Secudario);
            #endregion

            _dictionaryCategoria.Add(TipoOperacionService.MovimientoAlmacen, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.MovimientoKit, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.MovimientoBundle, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.MovimientoRemedir, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.InsertarEntregaStock, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.ActualizarEntregaStock, CategoriaMovimientos.Secudario);
            _dictionaryCategoria.Add(TipoOperacionService.EliminarEntregaStock, CategoriaMovimientos.Principal);

            _dictionaryCategoria.Add(TipoOperacionService.InsertarReservaStock, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.ActualizarReservaStock, CategoriaMovimientos.Secudario);
            _dictionaryCategoria.Add(TipoOperacionService.EliminarReservaStock, CategoriaMovimientos.Principal);

            _dictionaryCategoria.Add(TipoOperacionService.InsertarTraspasosalmacen, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.ActualizarTraspasosalmacen, CategoriaMovimientos.Secudario);
            _dictionaryCategoria.Add(TipoOperacionService.EliminarTraspasosalmacen, CategoriaMovimientos.Principal);

            _dictionaryCategoria.Add(TipoOperacionService.InsertarTransformacionEntradaStock, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.ActualizarTransformacionEntradaStock, CategoriaMovimientos.Secudario);
            _dictionaryCategoria.Add(TipoOperacionService.EliminarTransformacionEntradaStock, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.InsertarTransformacionSalidaStock, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.ActualizarTransformacionSalidaStock, CategoriaMovimientos.Secudario);
            _dictionaryCategoria.Add(TipoOperacionService.EliminarTransformacionSalidaStock, CategoriaMovimientos.Principal);

            _dictionaryCategoria.Add(TipoOperacionService.InsertarTransformacionloteStock, CategoriaMovimientos.Principal);
            _dictionaryCategoria.Add(TipoOperacionService.ActualizarcosteTransformacionloteStock, CategoriaMovimientos.Secudario);
            _dictionaryCategoria.Add(TipoOperacionService.ActualizarTransformacionloteStock, CategoriaMovimientos.Secudario);
            _dictionaryCategoria.Add(TipoOperacionService.FinalizarTransformacionloteStock, CategoriaMovimientos.Principal);
            
        }

        public IStockService GenerarServicio(IContextService context,TipoOperacionService operacion,MarfilEntities db)
        {
            if (_dictionary.ContainsKey(operacion))
            {
                
                var type = _dictionary[operacion];
                var ctor = type.GetConstructor(new[] { typeof(IContextService) });
                var result = ctor.Invoke(new object[] { context }) as IStockService;
                result.Db = db;
                result.Tipooperacion = operacion;
                return result;
            }

            throw new ValidationException("No existe la operacion de stock");
        }

        public CategoriaMovimientos GetCategoriaMovimientos(TipoOperacionService operacion)
        {
             return _dictionaryCategoria[operacion];
        }
    }
}
