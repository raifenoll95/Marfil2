using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Resources;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using System.Threading;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    //public enum TipoOperacionService
    //{
    //    [StringValue(typeof(RAlbaranesCompras),"TipoOperacionServiceInsertar")]
    //    InsertarRecepcionStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceActualizar")]
    //    ActualizarRecepcionStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceEliminar")]
    //    EliminarRecepcionStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceMovimientoAlmacen")]
    //    MovimientoAlmacen,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceKit")]
    //    MovimientoKit,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceBundle")]
    //    MovimientoBundle,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceRemedir")]
    //    MovimientoRemedir,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceEntregaInsertar")]
    //    InsertarEntregaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceEntregaActualizar")]
    //    ActualizarEntregaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceEntregaEliminar")]
    //    EliminarEntregaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceReservaInsertar")]
    //    InsertarReservaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceReservaActualizar")]
    //    ActualizarReservaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceReservaEliminar")]
    //    EliminarReservaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTraspasosalmacenInsertar")]
    //    InsertarTraspasosalmacen,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTraspasosalmacenActualizar")]
    //    ActualizarTraspasosalmacen,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTraspasosalmacenEliminar")]
    //    EliminarTraspasosalmacen,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionInsertar")]
    //    InsertarTransformacionSalidaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionActualizar")]
    //    ActualizarTransformacionSalidaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionEliminar")]
    //    EliminarTransformacionSalidaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionInsertar")]
    //    InsertarTransformacionEntradaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionActualizar")]
    //    ActualizarTransformacionEntradaStock,
    //    [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionEliminar")]
    //    EliminarTransformacionEntradaStock,
    //    [StringValue(typeof(RAlbaranes), "TipoOperacionServiceTransformacionloteInsertar")]
    //    InsertarTransformacionloteStock,
    //    [StringValue(typeof(RAlbaranes), "TipoOperacionServiceTransformacionloteActualizar")]
    //    ActualizarTransformacionloteStock,
    //    [StringValue(typeof(RAlbaranes), "TipoOperacionServiceActualizarcosteTransformacionloteStock")]
    //    ActualizarcosteTransformacionloteStock,
    //    [StringValue(typeof(RAlbaranes), "TipoOperacionServiceFinalizarTransformacionloteStock")]
    //    FinalizarTransformacionloteStock,
    //    [StringValue(typeof(RAlbaranes), "TipoOperacionServiceInsertarRecepcionStockDevolucion")]
    //    InsertarRecepcionStockDevolucion,
    //    [StringValue(typeof(RAlbaranes), "TipoOperacionServiceActualizarRecepcionStockDevolucion")]
    //    ActualizarRecepcionStockDevolucion,
    //    [StringValue(typeof(RAlbaranes), "TipoOperacionServiceInsertarDevolucionEntregaStock")]
    //    InsertarDevolucionEntregaStock,
    //    [StringValue(typeof(RAlbaranes), "TipoOperacionServiceActualizarEntregaStockDevolucion")]
    //    ActualizarEntregaStockDevolucion,


    //}
    public enum TipoOperacionService
    {
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceInsertar")]
        InsertarRecepcionStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceActualizar")]
        ActualizarRecepcionStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceEliminar")]
        EliminarRecepcionStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceMovimientoAlmacen")]
        MovimientoAlmacen,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceKit")]
        MovimientoKit,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceBundle")]
        MovimientoBundle,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceRemedir")]
        MovimientoRemedir,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceEntregaInsertar")]
        InsertarEntregaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceEntregaActualizar")]
        ActualizarEntregaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceEntregaEliminar")]
        EliminarEntregaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceReservaInsertar")]
        InsertarReservaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceReservaActualizar")]
        ActualizarReservaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceReservaEliminar")]
        EliminarReservaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTraspasosalmacenInsertar")]
        InsertarTraspasosalmacen,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTraspasosalmacenActualizar")]
        ActualizarTraspasosalmacen,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTraspasosalmacenEliminar")]
        EliminarTraspasosalmacen,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionInsertar")]
        InsertarTransformacionSalidaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceDivisionLotesInsertar")]
        InsertarDivisionLotesSalidaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionActualizar")]
        ActualizarTransformacionSalidaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionEliminar")]
        EliminarTransformacionSalidaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceDivisionLotesEliminar")]
        EliminarDivisionLotesSalidaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionInsertar")]
        InsertarTransformacionEntradaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceDivisionLotesInsertar")]
        InsertarDivisionLotesEntradaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionActualizar")]
        ActualizarTransformacionEntradaStock,
        [StringValue(typeof(RAlbaranesCompras), "TipoOperacionServiceTransformacionEliminar")]
        EliminarTransformacionEntradaStock,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceTransformacionloteInsertar")]
        InsertarTransformacionloteStock,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceTransformacionloteActualizar")]
        ActualizarTransformacionloteStock,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceActualizarcosteTransformacionloteStock")]
        ActualizarcosteTransformacionloteStock,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceFinalizarTransformacionloteStock")]
        FinalizarTransformacionloteStock,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceInsertarRecepcionStockDevolucion")]
        InsertarRecepcionStockDevolucion,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceActualizarRecepcionStockDevolucion")]
        ActualizarRecepcionStockDevolucion,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceInsertarDevolucionEntregaStock")]
        InsertarDevolucionEntregaStock,        
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceActualizarEntregaStockDevolucion")]
        ActualizarEntregaStockDevolucion,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceInsertarCostes")]
        InsertarCostes,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceActualizarCostes")]
        EliminarCostes,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceImputacionCostesInsertar")]
        InsertarImputacionCostes,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceSalidaReclamacion")]
        SalidaReclamacion,
        [StringValue(typeof(RAlbaranes), "TipoOperacionServiceEntradaReclamacion")]
        EntradaReclamacion
    }
    

    public enum TipoAlmacenloteService
    {
        [StringValue(typeof(RAlbaranesCompras), "TipoAlmacenLoteMercaderia")]
        Mercaderia,
        [StringValue(typeof(RAlbaranesCompras), "TipoAlmacenLotePropio")]
        Propio,
        [StringValue(typeof(RAlbaranesCompras), "TipoAlmacenLoteGestionado")]
        Gestionado,
    }
    internal class MovimientosstockService
    {
        #region Members

        private readonly MarfilEntities _db;
        private readonly StockactualService _stockactualService;
        private readonly IContextService _context;
        #endregion

        #region CTR

        public MovimientosstockService(IContextService context,MarfilEntities db)
        {
            _db = db;
            _context = context;
            _stockactualService =new StockactualService(_context,_db);
        }

        #endregion

        #region Api

        public void GenerarMovimiento(IStockPieza model, TipoOperacionService tipooperacion, string ubicacionDestino = null)
        {
            using ( var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
               
                //METEMOS EN LA TABLA MOVIMIENTOS DE STOCK DE LA BD EL MOVIMIENTO GENERADO
                GenerarMovimientostock(model, tipooperacion);

                //if (tipooperacion != TipoOperacionService.EntradaReclamacion && tipooperacion != TipoOperacionService.SalidaReclamacion)
                //{
                    //jmm
                    var entrada = model as MovimientosstockModel;

                    if (tipooperacion == TipoOperacionService.MovimientoAlmacen)
                    {
                        entrada.Tipomovimiento = TipoOperacionService.MovimientoAlmacen;
                        entrada.Ubicaciondestino = Convert.ToInt32(ubicacionDestino);
                    }
                    else if (model is IKitStockPieza)
                        entrada.Tipomovimiento = TipoOperacionService.MovimientoKit;
                    else if (model is IBundleStockPieza)
                        entrada.Tipomovimiento = TipoOperacionService.MovimientoBundle;

                    var service = new StockService(_context, _db);
                    service.GestionPieza(entrada);
                //}

                tran.Complete();
            }

            _db.SaveChanges();
        }

        public async System.Threading.Tasks.Task GenerarMovimientoAsync(IStockPieza model, TipoOperacionService tipooperacion, string ubicacionDestino = null)
        {
            using (var transaction = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                //METEMOS EN LA TABLA MOVIMIENTOS DE STOCK DE LA BD EL MOVIMIENTO GENERADO
                GenerarMovimientostock(model, tipooperacion);

                //if (tipooperacion != TipoOperacionService.EntradaReclamacion && tipooperacion != TipoOperacionService.SalidaReclamacion)
                //{
                //jmm
                var entrada = model as MovimientosstockModel;

                if (tipooperacion == TipoOperacionService.MovimientoAlmacen)
                {
                    entrada.Tipomovimiento = TipoOperacionService.MovimientoAlmacen;
                    entrada.Ubicaciondestino = Convert.ToInt32(ubicacionDestino);
                }
                else if (model is IKitStockPieza)
                    entrada.Tipomovimiento = TipoOperacionService.MovimientoKit;
                else if (model is IBundleStockPieza)
                    entrada.Tipomovimiento = TipoOperacionService.MovimientoBundle;

                var service = new StockService(_context, _db);
                await service.GestionPiezaAsync(entrada);
                //}

                //await _db.SaveChangesAsync();

                transaction.Complete();
            }

            await _db.SaveChangesAsync();
        }

        private void GenerarMovimientostock(IStockPieza entrada, TipoOperacionService tipooperacion)
        {
            var model = entrada as MovimientosstockModel;
            if (string.IsNullOrEmpty(model.Lote))
                model.Lote = string.Empty;

            if (model.Tipomovimiento == TipoOperacionService.InsertarTransformacionloteStock ||
                model.Tipomovimiento == TipoOperacionService.ActualizarTransformacionloteStock)
            {
                model.Fkarticulos = _db.Stockhistorico.SingleOrDefault(
                    f =>
                        f.empresa == _context.Empresa &&
                        f.lote == model.Lote && f.loteid == model.Loteid).fkarticulos;
            }

            if (model.Tipodealmacenlote == null &&
                (model.Tipomovimiento == TipoOperacionService.InsertarTransformacionloteStock ||
                model.Tipomovimiento == TipoOperacionService.FinalizarTransformacionloteStock))
            {        
                var stockObj =
                      _db.Stockhistorico.SingleOrDefault(
                          f =>
                              f.empresa == _context.Empresa &&
                              f.lote == model.Lote && f.loteid == model.Loteid);
                model.Tipodealmacenlote = (TipoAlmacenlote?)stockObj.tipoalmacenlote;
            }
            else if (model.Tipodealmacenlote == null)
            {
                var stockObj =
                      _db.Stockhistorico.SingleOrDefault(
                          f =>
                              f.empresa == _context.Empresa &&
                              f.lote == model.Lote && f.loteid == model.Loteid && 
                              f.fkarticulos == model.Fkarticulos); // f.fkalmacenes == _context.Fkalmacen
                model.Tipodealmacenlote = (TipoAlmacenlote?)stockObj.tipoalmacenlote; //?? null;
            }


            //NOS CREAMOS UN ITEM DE MOVIMIENTOS DE STOCK Y LO GUARDAMOS
            var item = _db.Movimientosstock.Create();

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
            item.costeacicionalvariable = model.Costeadicionalvariable;
            item.costeadicionalmaterial = model.Costeadicionalmaterial;
            item.costeadicionalotro = model.Costeadicionalotro;
            item.costeadicionalportes = model.Costeadicionalportes;
            //item.categoriamovimiento = (int)FStockService.Instance.GetCategoriaMovimientos(tipooperacion);
            item.Tipomovimiento = (int)model.Tipomovimiento;
            item.tipoalmacenlote = (int?)model.Tipodealmacenlote;        

            _db.Movimientosstock.Add(item);
        }

       

        #endregion
    }
}
