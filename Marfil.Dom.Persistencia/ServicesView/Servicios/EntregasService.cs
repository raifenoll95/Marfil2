using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.BusquedasMovil;
using Marfil.Inf.Genericos.Helper;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.Reservasstock;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;
using Marfil.Inf.Genericos;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using static DevExpress.CodeParser.CodeStyle.Formatting.Rules;
using System.Data.Entity.Migrations;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IEntregasService
    {

    }

    public class EntregasService : AlbaranesService, IAgregarLineaDocumentoMovile, IEntregasService
    {
        #region CTR

        public EntregasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {
            Modo = ModoAlbaran.Constock;
        }

        #endregion

        #region List index

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var st = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            st.List = st.List.OfType<AlbaranesModel>().OrderByDescending(f => f.Fechadocumento).ThenByDescending(f => f.Referencia);
            return st;
        }

        public override string GetSelectPrincipal()
        {
            //return string.Format("select * from albaranes where empresa='{0}'{1}",Empresa," and modo=" + (int)ModoAlbaran.Constock );
            return string.Format("select * from albaranes where empresa='{0}'{1}{2}", Empresa, " and modo=" + (int)ModoAlbaran.Constock, " and tipoalbaran<>" + (int)TipoAlbaran.VariosAlmacen);
        }

        #endregion

        #region Create

        //Rai - me dice si la cantidad de las lineas introducidas no supera la cantidad disponible de stock
        //Método modificado para el stock de seguridad
        public bool StockDisponible(EntregasStockModel model)
        {
            var haystockdisponible = true;
            var articulos = model.Lineas.GroupBy(f => f.Fkarticulos).ToList(); //Agrupamos por articulos
            foreach (var grupo in articulos)
            {
                var articulo = grupo.First().Fkarticulos;

                if (_db.Stockactual.Where(f => f.empresa == Empresa && f.fkarticulos == articulo && f.fkalmacenes == model.Fkalmacen).GroupBy(f => f.fkarticulos).FirstOrDefault() != null)
                {
                    var stockactual = _db.Stockactual.Where(f => f.empresa == Empresa && f.fkarticulos == articulo && f.fkalmacenes == model.Fkalmacen).GroupBy(f => f.fkarticulos).FirstOrDefault();
                    var stockdisponible = 0d;
                    var descalmacen = "";

                    //Compruebo que existe stock de seguridad por almacen, si no, estandar del artículo
                    var stockminimo = 0d;
                    var stockmaximo = 0d;
                    var stockseguridad = 0;

                    if (_db.ArticulosStockSeguridad.Where(f => f.empresa == Empresa && f.codalmacen == model.Fkalmacen && f.codarticulo == articulo).FirstOrDefault() != null)
                    {
                        stockseguridad = (int)_db.ArticulosStockSeguridad.Where(f => f.empresa == Empresa && f.codalmacen == model.Fkalmacen && f.codarticulo == articulo).FirstOrDefault().stockseguridad;
                        stockminimo = (double)_db.ArticulosStockSeguridad.Where(f => f.empresa == Empresa && f.codalmacen == model.Fkalmacen && f.codarticulo == articulo).FirstOrDefault().stockminimo;
                        stockmaximo = (double)_db.ArticulosStockSeguridad.Where(f => f.empresa == Empresa && f.codalmacen == model.Fkalmacen && f.codarticulo == articulo).FirstOrDefault().stockmaximo;
                        descalmacen = _db.ArticulosStockSeguridad.Where(f => f.empresa == Empresa && f.codalmacen == model.Fkalmacen && f.codarticulo == articulo).FirstOrDefault().descripcionalmacen;
                    }
                    else
                    {
                        stockseguridad = (int)_db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().stockseguridad;
                        stockminimo = (double)(_db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().stockminimo == null ? 0 : _db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().stockminimo);
                        stockmaximo = (double)(_db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().stockmaximo == null ? 0 : _db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().stockmaximo);
                        descalmacen = _db.Almacenes.Where(f => f.empresa == Empresa && f.id == model.Fkalmacen).FirstOrDefault().descripcion;
                    }

                    //Si no hay stock mínimo, no hacemos comprobaciones y pasamos al siguiente artículo, entendemos que no las quieren.
                    if (stockminimo == 0)
                    {
                        continue;
                    }

                    if (stockseguridad == 1)
                    {
                        stockdisponible = stockactual.Sum(f => f.cantidadtotal);
                    }
                    else
                    {
                        stockdisponible = (double)stockactual.Sum(f => f.metros);
                    }

                    //El stock restante después de la entrega es menor que el stock de seguridad del artículo
                    if (stockdisponible < stockminimo)
                    {
                        //Inserto la línea de log del stock de seguridad
                        var log = _db.LogStockSeguridad.Create();
                        log.id = _db.LogStockSeguridad.Any() ? _db.LogStockSeguridad.Max(f => f.id) + 1 : 1;
                        log.empresa = Empresa;
                        log.codarticulo = articulo;
                        log.descripcionarticulo = _db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().descripcion;
                        log.fecharotura = DateTime.Today;
                        log.fecha = (DateTime)model.Fechadocumento;
                        log.documento = model.Referencia;
                        log.codigounidad = "";
                        log.stockseguridad = stockseguridad;
                        log.stockactual = stockdisponible;
                        log.stockminimo = stockminimo;
                        log.pedidooptimo = (double)(stockmaximo - stockdisponible);
                        log.almacen = model.Fkalmacen + " - " + descalmacen;

                        _db.LogStockSeguridad.AddOrUpdate(log);
                        _db.SaveChanges();

                        haystockdisponible = false;
                    }
                }
                else //Ya no hay stock
                {
                    //Compruebo que existe stock de seguridad por almacen, si no, estandar del artículo
                    var stockminimo = 0d;
                    var stockmaximo = 0d;
                    var stockseguridad = 0;
                    var descalmacen = "";

                    if (_db.ArticulosStockSeguridad.Where(f => f.empresa == Empresa && f.codalmacen == model.Fkalmacen && f.codarticulo == articulo).FirstOrDefault() != null)
                    {
                        stockseguridad = (int)_db.ArticulosStockSeguridad.Where(f => f.empresa == Empresa && f.codalmacen == model.Fkalmacen && f.codarticulo == articulo).FirstOrDefault().stockseguridad;
                        stockminimo = (double)_db.ArticulosStockSeguridad.Where(f => f.empresa == Empresa && f.codalmacen == model.Fkalmacen && f.codarticulo == articulo).FirstOrDefault().stockminimo;
                        stockmaximo = (double)_db.ArticulosStockSeguridad.Where(f => f.empresa == Empresa && f.codalmacen == model.Fkalmacen && f.codarticulo == articulo).FirstOrDefault().stockmaximo;
                        descalmacen = _db.ArticulosStockSeguridad.Where(f => f.empresa == Empresa && f.codalmacen == model.Fkalmacen && f.codarticulo == articulo).FirstOrDefault().descripcionalmacen;
                    }
                    else
                    {
                        stockseguridad = (int)_db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().stockseguridad;
                        stockminimo = (double)(_db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().stockminimo == null ? 0 : _db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().stockminimo);
                        stockmaximo = (double)(_db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().stockmaximo == null ? 0 : _db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().stockmaximo);
                        descalmacen = _db.Almacenes.Where(f => f.empresa == Empresa && f.id == model.Fkalmacen).FirstOrDefault().descripcion;
                    }

                    //Si no hay stock mínimo, no hacemos comprobaciones y pasamos al siguiente artículo, entendemos que no las quieren.
                    if (stockminimo == 0)
                    {
                        continue;
                    }

                    //Inserto la línea de log del stock de seguridad. Como no hay stock, el valor es 0
                    var log = _db.LogStockSeguridad.Create();
                    log.id = _db.LogStockSeguridad.Any() ? _db.LogStockSeguridad.Max(f => f.id) + 1 : 1;
                    log.empresa = Empresa;
                    log.codarticulo = articulo;
                    log.descripcionarticulo = _db.Articulos.Where(f => f.empresa == Empresa && f.id == articulo).FirstOrDefault().descripcion;
                    log.fecharotura = DateTime.Today;
                    log.fecha = (DateTime)model.Fechadocumento;
                    log.documento = model.Referencia;
                    log.codigounidad = "";
                    log.stockseguridad = stockseguridad;
                    log.stockactual = 0;
                    log.stockminimo = stockminimo;
                    log.pedidooptimo = (double)(stockmaximo - 0);
                    log.almacen = model.Fkalmacen + " - " + descalmacen;

                    _db.LogStockSeguridad.AddOrUpdate(log);
                    _db.SaveChanges();

                    haystockdisponible = false;
                }
            }
            return haystockdisponible;
        }

        public override void create(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as AlbaranesModel;

                base.create(obj);

                //EntregarStock(obj as AlbaranesModel);
                if (model.Tipoalbaran == (int)TipoAlbaran.Devolucion)
                    GenerarMovimientosLineas(model.Lineas, model, TipoOperacionService.InsertarDevolucionEntregaStock);

                else if (model.Tipoalbaran == (int)TipoAlbaran.Reclamacion)
                {
                    var antiguas = model.Lineas.Where(f => f.Cantidad < 0).ToList();
                    var nuevas = model.Lineas.Where(f => f.Cantidad > 0).ToList();

                    GenerarMovimientosLineas(antiguas, model, TipoOperacionService.InsertarDevolucionEntregaStock);
                    GenerarMovimientosLineas(nuevas, model, TipoOperacionService.InsertarEntregaStock);

                    //Cuando creamos el albaran de reclamacion, necesitamos introducir en las lineas del albaran original el id y la referencia del reclamado
                    var original = get(model.idOriginalReclamado.ToString()) as AlbaranesModel;
                    foreach (var linea in original.Lineas)
                    {
                        linea.Fkreclamado = model.Id;
                        linea.Fkreclamadoreferencia = model.Referencia;
                    }

                    //base.edit(original);

                    // 09/04/2021 -- TIENE QUE MODIFICARSE EL ALBARAN ORIGINAL (PARA RELLENAR LOS CAMPOS FKRECLAMADO Y FKRECLAMADOREFERENCIA)
                    // pero no tiene que hacer comprobaciones de que el albaran original esté o no FINALIZADO.

                    var objPersistance = _converterModel.EditPersitance(original);
                    if (_validationService.ValidarGrabar(objPersistance))
                    {
                        _db.Entry(objPersistance).State = EntityState.Modified;
                        try
                        {
                            _db.SaveChanges();
                        }
                        catch (DbUpdateException ex)
                        {
                            throw;
                        }
                    }
                }

                else
                    GenerarMovimientosLineas(model.Lineas, model, TipoOperacionService.InsertarEntregaStock);

                _db.SaveChanges();
                tran.Complete();
            }
                
        }

        //private void EntregarStock(AlbaranesModel nuevo)
        //{
        //    foreach (var item in nuevo.Lineas)
        //        item.Cantidad *= -1;
        //    OperarStock(nuevo, TipoOperacionStock.Salida,TipoOperacionService.InsertarEntregaStock);
        //}

        #endregion

        #region Edit

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var original = get(Funciones.Qnull(obj.get("id"))) as AlbaranesModel;
                var editado = obj as AlbaranesModel;

                if(editado.Estado.Tipoestado == TipoEstado.Anulado || editado.Estado.Tipoestado == TipoEstado.Caducado || editado.Estado.Tipoestado == TipoEstado.Finalizado)
                {
                    throw new ValidationException("No se pueden modificar documentos en estado finalizado");
                }

                else
                {
                    base.edit(obj);
                    //ActualizarStock(original, editado);
                    GenerarMovimientosLineas(original.Lineas, original, TipoOperacionService.EliminarEntregaStock);
                    GenerarMovimientosLineas(editado.Lineas, editado, TipoOperacionService.InsertarEntregaStock);

                    tran.Complete();
                }                
            }
        }

        //private void ActualizarStock(AlbaranesModel original, AlbaranesModel nuevo)
        //{
        //    var list = new List<AlbaranesLinModel>();

        //    var lineasModificadas = nuevo.Lineas.Where(f => !original.Lineas.Any(j => j.Flagidentifier == f.Flagidentifier)).ToList();
        //    foreach (var item in lineasModificadas)
        //        item.Cantidad *= -1;

        //    var lineasEliminadas = original.Lineas.Where(f => !nuevo.Lineas.Where(j => !lineasModificadas.Any(h => h.Flagidentifier == f.Flagidentifier)).Any(j => j.Flagidentifier == f.Flagidentifier)).ToList();

        //    lineasModificadas.ForEach(f => f.Nueva = true);

        //    list = lineasEliminadas.Union(lineasModificadas ).ToList(); // primero las eliminadas para que entre el stock

        //    nuevo.Tipoalbaranenum = TipoAlbaran.Habitual;
        //    GenerarMovimientosLineas(list, nuevo, TipoOperacionStock.Actualizacion);

        //}


        #endregion

        #region Delete

        public override void delete(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                base.delete(obj);
                var model = obj as AlbaranesModel;
                //model.Tipoalbaranenum=TipoAlbaran.Habitual;
                //EliminarStock(model);
                if (model.Tipoalbaran == (int)TipoAlbaran.Devolucion)
                    GenerarMovimientosLineas(model.Lineas, model, TipoOperacionService.ActualizarEntregaStockDevolucion);
                else
                    GenerarMovimientosLineas(model.Lineas, model, TipoOperacionService.EliminarEntregaStock);
                tran.Complete();
            }
        }

        //private void EliminarStock(AlbaranesModel nuevo)
        //{
        //    OperarStock(nuevo, TipoOperacionStock.Salida, TipoOperacionService.EliminarEntregaStock);
        //}

        #endregion

        #region Importar pedido

        public override AlbaranesModel ImportarPedido(PedidosModel presupuesto)
        {
            var result = Helper.fModel.GetModel<AlbaranesModel>(_context);
            result.Importado = true;
            ImportarCabecera(presupuesto, result);
            base.EstablecerSerie(presupuesto.Fkseries, result);
            result.Fkalmacen = _context.Fkalmacen;
            result.Fkpedidos = presupuesto.Referencia;
            return result;
        }

        public AlbaranesModel CrearAlbaranImputacionMateriales(PedidosModel presupuesto)
        {
            var result = Helper.fModel.GetModel<AlbaranesModel>(_context);
            result.Importado = true;
            ImportarCabecera(presupuesto, result);
            result.Fkseries = _db.Series.Where(f => f.empresa == Empresa && f.salidasvarias == true).Select(f => f.id).SingleOrDefault();
            result.Tipoalbaran = (int)TipoAlbaran.ImputacionMateriales;            
            result.Fkclientes = _db.Empresas.Where(f => presupuesto.Empresa == f.id).Select(f => f.fkCuentaSalidasVariasAlmacen).SingleOrDefault();                
            result.Fkalmacen = _context.Fkalmacen;
            result.Fkpedidos = presupuesto.Referencia;
            return result;
        }

        #endregion

        #region Generar entrega

        public AlbaranesModel GenerarEntrega(string entregaid)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var validationService = _validationService as AlbaranesValidation;
                var serviceReserva = FService.Instance.GetService(typeof(ReservasstockModel), _context, _db) as ReservasstockService;
                var reservaModel = serviceReserva.get(entregaid) as ReservasstockModel;
                
                var modelo = GenerarModelo(reservaModel);

                serviceReserva.ConsumirReserva(entregaid);
                validationService.FlagActualizarCantidadesFacturadas = true;
                create(modelo);
                validationService.FlagActualizarCantidadesFacturadas = false;
                var resultado = GetByReferencia(modelo.Referencia);
                tran.Complete();
                return resultado;
            }
                
        }

        private EntregasStockModel GenerarModelo(ReservasstockModel reservaModel)
        {
            var resultado = Helper.fModel.GetModel<EntregasStockModel>(_context);

            
            EstablecerSerie(reservaModel.Fkseries, resultado);
            //cabecera
            var properties = typeof(EntregasStockModel).GetProperties();
            foreach (var p in properties)
            {
                if (p.CanWrite && p.Name != "Lineas"
                    && p.Name != "Totales"
                    && p.Name != "Galeria"
                    && p.Name != "Fkseries"
                    && p.Name != "Fkejercicios"
                    && p.Name != "Fechadocumento"
                    && p.Name != "Fechavalidez"
                    && p.Name != "Criteriosagrupacionlist"
                    && p.Name != "Fkcriteriosagrupacion")
                {
                    p.SetValue(resultado, reservaModel.get(p.Name));
                }
            }
            resultado.Tipodeportes = null;
            var propertieslin = typeof(AlbaranesLinVistaModel).GetProperties();
            var propertiesreserva = typeof(ReservasstockLinModel).GetProperties();
            foreach (var linea in reservaModel.Lineas)
            {
                var item = new AlbaranesLinVistaModel();

                foreach (var p in propertieslin)
                {
                    if (p.CanWrite && propertiesreserva.Any(f => f.Name == p.Name))
                    {

                        var preserva = propertiesreserva.Single(f => f.Name == p.Name);

                        p.SetValue(item, preserva.GetValue(linea));
                    }
                }
                item.Fkalmacen = resultado.Fkalmacen;
                item.Fkmonedas = resultado.Fkmonedas.ToString();
                item.Fkcuenta = resultado.Fkclientes;
                item.Descuentoprontopago = resultado.Porcentajedescuentoprontopago?.ToString() ?? "0";
                item.Descuentocomercial = resultado.Porcentajedescuentocomercial?.ToString() ?? "0";
                item.Lineas = new List<MovimientosstockModel>();
                item.Lineas.Add(new MovimientosstockModel()
                {
                    Fkarticulos = linea.Fkarticulos,
                    Descripcion = linea.Descripcion,
                    Lote = linea.Lote,
                    Loteid = linea.Tabla.ToString(),
                    Cantidad = linea.Cantidad ?? 0,
                    Largo = linea.Largo ?? 0,
                    Ancho = linea.Ancho ?? 0,
                    Grueso = linea.Grueso ?? 0,
                    Fkunidadesmedida = linea.Fkunidades,
                    Metros = linea.Metros ?? 0,
                    Bundle = linea.Bundle,
                    Decimalesmedidas = linea.Decimalesmedidas
                });
                resultado.Lineas= CrearNuevasLineas(resultado.Lineas, item);
                
            }

            foreach (var elem in resultado.Lineas)
            {
                elem.Fkpedidosreferencia = reservaModel.Referencia;
                elem.Fkpedidosid = reservaModel.Id;
                elem.Fkpedidos = reservaModel.Id;
            }

            Recalculartotales(resultado.Lineas, resultado.Porcentajedescuentoprontopago ?? 0, resultado.Porcentajedescuentocomercial ?? 0, resultado.Costeportes ?? 0, resultado.Decimalesmonedas,true);
            resultado.Modo=ModoAlbaran.Constock;
            return resultado;
        }

        private void EstablecerSerie(string fkseries, EntregasStockModel result)
        {
            var service = FService.Instance.GetService(typeof(SeriesModel), _context);

            var serieObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.Reservas), fkseries)) as SeriesModel;
            var serieAsociada = serieObj.Fkseriesasociada;
            if (!string.IsNullOrEmpty(serieAsociada))
            {
                var serieasociadaObj = service.get(string.Format("{0}-{1}", SeriesService.GetSerieCodigo(TipoDocumento.AlbaranesVentas), serieAsociada)) as SeriesModel;
                result.Fkseries = serieasociadaObj.Id;
            }
            else
                throw new ValidationException(Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Presupuestos.ErrorNoExisteSerieAsociada);
        }

        #endregion

        public IEnumerable<AlbaranesLinModel> ImportarLineasReclamadas(IEnumerable<ILineaImportar> duplicadas, int maxId)
        {
            List<AlbaranesLinModel> lineas = new List<AlbaranesLinModel>();
            var id = maxId++;

            foreach (var linea in duplicadas)
            {
                lineas.Add(new AlbaranesLinModel
                {
                    Cantidadpedida = 0,
                    Id = id,
                    Ancho = linea.Ancho,
                    Canal = linea.Canal,
                    Cantidad = linea.Cantidad,
                    Cuotaiva = linea.Cuotaiva,
                    Cuotarecargoequivalencia = linea.Cuotarecargoequivalencia,
                    Decimalesmedidas = linea.Decimalesmedidas,
                    Decimalesmonedas = linea.Decimalesmonedas,
                    Descripcion = linea.Descripcion,
                    Fkregimeniva = linea.Fkregimeniva,
                    Fkunidades = linea.Fkunidades,
                    Metros = linea.Metros,
                    Precio = linea.Precio,
                    Fkarticulos = linea.Fkarticulos,
                    Fktiposiva = linea.Fktiposiva,
                    Grueso = linea.Grueso,
                    Importe = linea.Importe,
                    Importedescuento = linea.Importedescuento,
                    Largo = linea.Largo,
                    Lote = linea.Lote,
                    Notas = linea.Notas,
                    Porcentajedescuento = linea.Porcentajedescuento,
                    Porcentajeiva = linea.Porcentajeiva,
                    Porcentajerecargoequivalencia = linea.Porcentajerecargoequivalencia,
                    Precioanterior = linea.Precioanterior,
                    Revision = linea.Revision,
                    Tabla = linea.Tabla,
                    Bundle = linea.Bundle,
                    Fkpedidos = Funciones.Qint(linea.Fkdocumento),
                    Fkpedidosid = Funciones.Qint(linea.Fkdocumentoid),
                    Fkpedidosreferencia = linea.Fkdocumentoreferencia,
                    Contenedor = linea.Contenedor,
                    Sello = linea.Sello,
                    Caja = linea.Caja,
                    Pesoneto = linea.Pesoneto

                });
                id++;
            }

            return lineas;
        }

        #region Buscar documento


        public override IEnumerable<DocumentosBusqueda> Buscar(IDocumentosFiltros filtros, out int registrostotales)
        {
            var service = new BuscarDocumentosEntregasstockService(_db, Empresa);
            return service.Buscar(filtros, out registrostotales);
        }

        public override IEnumerable<IItemResultadoMovile> BuscarDocumento(string referencia)
        {
            var service = new BuscarDocumentosEntregasstockService(_db, Empresa);
            return service.Get<AlbaranesModel, AlbaranesLinModel, AlbaranesTotalesModel>(this, referencia);
        }

        #endregion

        #region Agregar linea mobile api

        public AgregarLineaDocumentosModel AgregarLinea(string referencia, string lote)
        {
            return OperarLinea(referencia, lote, true);
        }

        public AgregarLineaDocumentosModel EliminarLinea(string referencia, string lote)
        {
            return OperarLinea(referencia, lote, false);
        }

        private AgregarLineaDocumentosModel OperarLinea(string referencia, string lote, bool agregar)
        {
            var result = new AgregarLineaDocumentosModel();
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = GetByReferencia(referencia);
                model = get(model.Id.ToString()) as AlbaranesModel;
                
                result.Referencia = model.Referencia;
                result.Fecha = model.Fechadocumentocadena;
                model.Lineas = (agregar
                    ? CrearNuevasLineas(model.Lineas, GenerarModeloLin(model, lote))
                    : EliminarLineas(model.Lineas, lote));

                edit(model);

                result.Lineas = model.Lineas
                    .Select(f => new AgregarLineaDocumentosLinModel()
                    {
                        Lote = string.Format("{0}{1}", f.Lote, Funciones.RellenaCod(f.Tabla?.ToString() ?? "0", 3)),
                        Largo = f.SLargo,
                        Ancho = f.SAncho,
                        Grueso = f.SGrueso,
                        Cantidad = f.Cantidad.ToString(),
                        Descripcion = f.Descripcion,
                        Fkarticulos = f.Fkarticulos,
                        Metros = f.SMetros
                    }).ToList();

                tran.Complete();
            }
               

            return result;
        }

        private List<AlbaranesLinModel> EliminarLineas(List<AlbaranesLinModel> lineas, string lote)
        {
            var item =
                lineas.SingleOrDefault(f => string.Format("{0}{1}", f.Lote, Funciones.RellenaCod(f.Tabla?.ToString()??"0", 3)) == lote);
            if (item != null)
            {
                lineas.Remove(item);
            }

            return lineas;

        }

        private AlbaranesLinVistaModel GenerarModeloLin(AlbaranesModel albaranObj,string lote)
        {

            var serviceStock = new StockactualService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof (ArticulosModel), _context, _db) as ArticulosService;
            var familiasService = FService.Instance.GetService(typeof(FamiliasproductosModel), _context, _db) as FamiliasproductosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
            var obj = serviceStock.GetArticuloPorLoteOCodigo(lote, albaranObj.Fkalmacen, albaranObj.Empresa) as MovimientosstockModel;
            if(obj!=null)
            {
                var fkarticulos = obj.Fkarticulos;
                var articulosObj = articulosService.GetArticulo(fkarticulos, albaranObj.Fkclientes,
                    albaranObj.Fkmonedas.ToString(), albaranObj.Fkregimeniva, TipoFlujo.Venta);
                var familiaObj = familiasService.get(ArticulosService.GetCodigoFamilia(fkarticulos)) as FamiliasproductosModel;
                var unidadesObj = unidadesService.get(familiaObj.Fkunidadesmedida) as UnidadesModel;
                var metros = UnidadesService.CalculaResultado(unidadesObj, obj.Cantidad, obj.Largo, obj.Ancho, obj.Grueso, obj.Metros);
                obj.Metros = metros;
                return new AlbaranesLinVistaModel()
                {
                    Modificarmedidas = false,
                    Lote = lote,
                    Decimalesmonedas = albaranObj.Decimalesmonedas,
                    Descuentocomercial = albaranObj.Porcentajedescuentocomercialcadena,
                    Descuentoprontopago = albaranObj.Porcentajedescuentoprontopagocadena,
                    Fkcuenta = albaranObj.Fkclientes,
                    Fkmonedas = albaranObj.Fkmonedas.ToString(),
                    Flujo = TipoFlujo.Venta,
                    Fkregimeniva = albaranObj.Fkregimeniva,
                    Portes = albaranObj.Costeportes.ToString(),
                    Fkalmacen = albaranObj.Fkalmacen,
                    Descuento = 0,
                    Precio = articulosObj.Precio ?? 0,
                    Fkarticulos = fkarticulos,
                    Lineas = new List<MovimientosstockModel>(new [] { obj })
                };
            }

            return new AlbaranesLinVistaModel()
            {
                Fkmonedas = albaranObj.Fkmonedas.ToString()
            };
        }

        #endregion

        #region Saldar pedido

        public void SaldarPedidos( OperacionSaldarPedidosModel model,AlbaranesModel entrega)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                    entrega.Pedidosaldado = true;
                    SaldarLineaPedidos(model);
                    edit(entrega);
                    _db.SaveChanges();
                    tran.Complete();
                
            }
        }

        private void SaldarLineaPedidos(OperacionSaldarPedidosModel model)
        {
            var pedidosService = FService.Instance.GetService(typeof (PedidosModel), _context, _db) as PedidosService;
            var pedidoreferenciaObj = pedidosService.GetByReferencia(model.Fkpedidos);
            var pedidoobj = pedidosService.get(pedidoreferenciaObj.Id.ToString()) as PedidosModel;

            

            var agrupacionArticulos = pedidoobj.Lineas.GroupBy(f =>  f.Fkarticulos );
            foreach (var item in agrupacionArticulos)
                AsignarCantidadesPedidas(item,pedidoobj, model);


            pedidosService.edit(pedidoobj);

        }

        private void AsignarCantidadesPedidas(IGrouping<string, PedidosLinModel> item, PedidosModel pedido, OperacionSaldarPedidosModel model)
        {
            if (pedido.Lineas.Count(f => f.Fkarticulos == item.Key) > 1)
            {
                var agrupacionArticulos =
                    pedido.Lineas.Where(f => f.Fkarticulos == item.Key)
                        .GroupBy(f => new {f.Fkarticulos, f.Largo, f.Ancho, f.Grueso});

                foreach (var lineaarticulo in agrupacionArticulos)
                    AsignarPorArticuloMedidas(lineaarticulo.Key.Fkarticulos, lineaarticulo.Key.Largo,
                        lineaarticulo.Key.Ancho, lineaarticulo.Key.Grueso, pedido, model);
                
            }
            else
            {
                AsignarPorArticulo(item.Key, pedido, model);
            }
        }

        private void AsignarPorArticuloMedidas(string fkarticulos, double? largo, double? ancho, double? grueso, PedidosModel pedido, OperacionSaldarPedidosModel model)
        {
            var linea = model.Lineas.Single(f => f.Codigo == fkarticulos && Funciones.Qdouble(f.Largo.Replace(".","").Replace(",", ".")) ==largo && Funciones.Qdouble(f.Ancho.Replace(".", "").Replace(",", ".")) == ancho && Funciones.Qdouble(f.Grueso.Replace(".", "").Replace(",", ".")) == grueso);
            var cantidadpedida = linea.Cantidadpedida - linea.Cantidadpendiente;
            foreach (var item in pedido.Lineas)
            {
                if (item.Fkarticulos == fkarticulos 
                    && item.Largo==largo
                    && item.Ancho == ancho
                    && item.Grueso == grueso)
                {
                    var cantidadpendiente = (item.Cantidad ?? 0) - (item.Cantidadpedida ?? 0);
                    var auxcantidad = cantidadpendiente >= cantidadpedida ? cantidadpedida : cantidadpendiente;
                    item.Cantidadpedida = (item.Cantidadpedida ?? 0) + auxcantidad;

                    if (item.Cantidadpedida < 0)
                        item.Cantidadpedida = 0;
                    if (item.Cantidadpedida > item.Cantidad)
                        item.Cantidadpedida = item.Cantidad;

                    cantidadpedida -= auxcantidad;
                }
            }
        }

        private void AsignarPorArticulo(string fkarticulo, PedidosModel pedido, OperacionSaldarPedidosModel model)
        {
            var linea = model.Lineas.Single(f => f.Codigo == fkarticulo);
            
            var cantidadpedida =linea.Cantidadpedida - linea.Cantidadpendiente;
            foreach (var item in pedido.Lineas)
            {
                if (item.Fkarticulos == fkarticulo)
                {
                    var cantidadpendiente = (item.Cantidad ?? 0) - (item.Cantidadpedida ?? 0);
                    var auxcantidad = cantidadpendiente >= cantidadpedida ? cantidadpedida : cantidadpendiente;
                    item.Cantidadpedida = (item.Cantidadpedida ??0 ) + auxcantidad;
                    if (item.Cantidadpedida < 0)
                        item.Cantidadpedida = 0;
                    if (item.Cantidadpedida > item.Cantidad)
                        item.Cantidadpedida = item.Cantidad;
                    cantidadpedida -= auxcantidad;
                }
            }
        }


        public IEnumerable<SaldarPedidosModel> GetLineasSaldarPedidos(string pedidoreferencia, List<AlbaranesLinModel> lineas)
        {
            var result = new List<SaldarPedidosModel>();
            var pedidosService = FService.Instance.GetService(typeof (PedidosModel), _context,_db) as PedidosService;
            var pedidosreferenciaObj = pedidosService.GetByReferencia(pedidoreferencia);
            var pedidosObj = pedidosService.get(pedidosreferenciaObj.Id.ToString()) as PedidosModel;

            var agrupacionLineas = pedidosObj.Lineas.GroupBy(f => f.Fkarticulos);
            
            
            foreach (var item in agrupacionLineas)
            {
                if (pedidosObj.Lineas.Count(f => f.Fkarticulos == item.Key)>1)
                {
                    var lineasArticulos = pedidosObj.Lineas.Where(f => f.Fkarticulos == item.Key).GroupBy(f=>new  {f.Fkarticulos,f.Largo,f.Ancho,f.Grueso,f.Decimalesmedidas});
                    foreach (var art in lineasArticulos)
                    {
                        var selectedArts = pedidosObj.Lineas.Where(f => f.Fkarticulos == art.Key.Fkarticulos && Funciones.Qdouble(f.Largo) == art.Key.Largo && Funciones.Qdouble(f.Ancho) == art.Key.Ancho && Funciones.Qdouble(f.Grueso) == art.Key.Grueso).ToList();
                        var cantidadtotal = selectedArts.Sum(f => (f.Cantidad ?? 0));
                        var cantidadpedida = selectedArts.Sum(f => (f.Cantidad ?? 0) - (f.Cantidadpedida ?? 0));
                        var metrosalbaran = (lineas.Where(f => f.Fkarticulos == item.Key && Funciones.Qdouble(f.Largo) == art.Key.Largo && Funciones.Qdouble(f.Ancho) == art.Key.Ancho && Funciones.Qdouble(f.Grueso) == art.Key.Grueso).Sum(f => f.Metros) ?? 0);
                      
                        var metrospedidos = selectedArts.Sum(f => f.Metros) ?? 0;
                        metrospedidos = metrospedidos * (cantidadpedida / cantidadtotal);
                        result.Add(new SaldarPedidosModel()
                        {
                            Id = Guid.NewGuid(),
                            Codigo = art.Key.Fkarticulos,
                            Cantidadpedida = cantidadpedida,
                            Largo = art.Key.Largo?.ToString("N" + (art.Key.Decimalesmedidas ?? 2)),
                            Ancho = art.Key.Ancho?.ToString("N" + (art.Key.Decimalesmedidas ?? 2)),
                            Grueso = art.Key.Grueso?.ToString("N" + (art.Key.Decimalesmedidas ?? 2)),
                            Metros = metrospedidos.ToString("N" + (art.Key.Decimalesmedidas ?? 2)),
                            Cantidadalbaran = lineas.Where(f => f.Fkarticulos == art.Key.Fkarticulos && Funciones.Qdouble(f.Largo) == art.Key.Largo && Funciones.Qdouble(f.Ancho) == art.Key.Ancho && Funciones.Qdouble(f.Grueso) == art.Key.Grueso).Sum(f => f.Cantidad) ?? 0,
                            Metrosalbaran = (metrosalbaran).ToString("N" + art.Key.Decimalesmedidas),
                            Cantidadpendiente =Math.Round(metrospedidos>0? cantidadpedida - (cantidadpedida * metrosalbaran / metrospedidos):0,2),
                        });
                    }
                }
                else
                {
                    var cantidadtotal = item.Sum(f => f.Cantidad ?? 0);
                    var cantidadpedida = item.Sum(f => (f.Cantidad ?? 0) -(f.Cantidadpedida??0));
                    var metrosalbaran = (lineas.Where(f =>f.Fkarticulos == item.Key).Sum(f => f.Metros) ?? 0);
                    var metrospedidos = pedidosObj.Lineas.Where(f => f.Fkarticulos == item.Key).Sum(f => f.Metros) ?? 0;
                    metrospedidos = metrospedidos * (cantidadpedida / cantidadtotal);
                    var linea = pedidosObj.Lineas.Single(f => f.Fkarticulos == item.Key);

                    result.Add(new SaldarPedidosModel()
                    {
                        Id = Guid.NewGuid(),
                        Codigo = item.Key,
                        Cantidadpedida = cantidadpedida,
                        Largo = linea.Largo?.ToString("N" + (linea.Decimalesmedidas ?? 2)),
                        Ancho = linea.Ancho?.ToString("N" + (linea.Decimalesmedidas ?? 2)),
                        Grueso = linea.Grueso?.ToString("N" + (linea.Decimalesmedidas ?? 2)),
                        Metros = metrospedidos.ToString("N" + (linea.Decimalesmedidas ?? 2)),
                        Cantidadalbaran = lineas.Where(f => f.Fkarticulos == item.Key).Sum(f => f.Cantidad) ?? 0,
                        Metrosalbaran = (metrosalbaran ).ToString("N"+ linea.Decimalesmedidas),
                        Cantidadpendiente = Math.Round(metrospedidos > 0 ? cantidadpedida - (cantidadpedida * metrosalbaran / metrospedidos):0,2),
                    });
                }
                   
            }

            return result;
        } 

        #endregion  

        #region Helpers

        //private void OperarStock(AlbaranesModel nuevo, TipoOperacionStock operacion, TipoOperacionService serviciotipo)
        //{
        //   GenerarMovimientosLineas(nuevo.Lineas, nuevo, operacion);
        //}


        private void GenerarMovimientosLineas(IEnumerable<AlbaranesLinModel> lineas, AlbaranesModel nuevo, TipoOperacionService movimiento)
        {
            var movimientosStockService = new MovimientosstockService(_context, _db);
            var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
            var serializer = new Serializer<AlbaranesDiarioStockSerializable>();
            var vectorArticulos = new Hashtable();

            //jmm
            var operacion = 1;
            if (movimiento == TipoOperacionService.InsertarEntregaStock || movimiento == TipoOperacionService.InsertarDevolucionEntregaStock)
                operacion = -1;

            foreach (var linea in lineas)
            {
                ArticulosModel articuloObj;
                if (vectorArticulos.ContainsKey(linea.Fkarticulos))
                    articuloObj = vectorArticulos[linea.Fkarticulos] as ArticulosModel;
                else
                {
                    articuloObj = articulosService.get(linea.Fkarticulos) as ArticulosModel;
                    vectorArticulos.Add(linea.Fkarticulos, articuloObj);
                }

                var aux = Funciones.ConverterGeneric<AlbaranesLinSerialized>(linea);                

                if (articuloObj?.Gestionstock ?? false)
                {
                    var model = new MovimientosstockModel
                    {
                        Empresa = nuevo.Empresa,
                        Fkalmacenes = nuevo.Fkalmacen,
                        Fkalmaceneszona = Funciones.Qint(nuevo.Fkzonas),
                        Fkarticulos = linea.Fkarticulos,
                        Referenciaproveedor = "",
                        Lote = linea.Lote,
                        Loteid = (linea.Tabla ?? 0).ToString(),
                        Tag = "",
                        Fkunidadesmedida = linea.Fkunidades,                        
                        Largo = linea.Largo ?? 0,
                        Ancho = linea.Ancho ?? 0,
                        Grueso = linea.Grueso ?? 0,
                        
                        Documentomovimiento = serializer.GetXml(
                            new AlbaranesDiarioStockSerializable
                            {
                                Id = nuevo.Id,
                                Referencia = nuevo.Referencia,
                                Fechadocumento = nuevo.Fechadocumento,
                                Codigocliente = nuevo.Fkclientes,
                                Linea = aux
                            }),
                        Fkusuarios = Usuarioid,
                        //Tipooperacion = operacion,
                        Tipodealmacenlote = nuevo.Tipodealmacenlote,        


                        Cantidad = !String.Equals(linea.Fkunidades, "08") ? (linea.Cantidad ?? 0) * operacion : 0, ///*-1,
                        Metros = (linea.Metros ?? 0) * operacion,

                        //Pesoneto = ((articuloObj.Kilosud ?? 0) * linea.Metros) * operacion,
                        Costeadicionalmaterial = linea.Costeadicionalmaterial * operacion,
                        Costeadicionalotro = linea.Costeadicionalotro * operacion,
                        Costeadicionalvariable = linea.Costeadicionalvariable * operacion,
                        Costeadicionalportes = linea.Costeadicionalportes * operacion,

                        Tipomovimiento = movimiento
                    };

                    ////cantidad
                    //if(nuevo.Tipoalbaran == (int)TipoAlbaran.Reclamacion && linea.Cantidad < 0)
                    //{
                    //    model.Cantidad = !String.Equals(linea.Fkunidades, "08") ? (linea.Cantidad ?? 0) * operacion : 0;
                    //}

                    //else {
                    //    model.Cantidad = linea.Cantidad.Value;
                    //}

                    ////metros
                    //if (nuevo.Tipoalbaran == (int)TipoAlbaran.Reclamacion && linea.Cantidad < 0)
                    //{
                    //    model.Metros = (linea.Metros ?? 0) * operacion;
                    //}

                    //else
                    //{
                    //    model.Metros = linea.Metros.Value;
                    //}

                    var operacionServicio = linea.Nueva
                        ? TipoOperacionService.InsertarEntregaStock
                        : TipoOperacionService.ActualizarEntregaStock;
                    if (nuevo.Tipoalbaranenum == TipoAlbaran.Devolucion)
                    {
                        operacionServicio = linea.Nueva
                        ? TipoOperacionService.InsertarDevolucionEntregaStock
                        : TipoOperacionService.ActualizarEntregaStockDevolucion;
                    }

                    if (nuevo.Tipoalbaranenum == TipoAlbaran.Reclamacion && linea.Cantidad < 0)
                    {
                        operacionServicio = TipoOperacionService.InsertarDevolucionEntregaStock;

                    }
                    if (nuevo.Tipoalbaranenum == TipoAlbaran.Reclamacion && linea.Cantidad > 0)
                    {
                        operacionServicio = TipoOperacionService.InsertarEntregaStock;

                    }

                    movimientosStockService.GenerarMovimiento(model, operacionServicio);
                }

            }
        }
        #endregion


    }
}
