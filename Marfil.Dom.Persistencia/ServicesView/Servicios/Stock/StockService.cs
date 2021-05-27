using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Stock;
using System.Data.Entity.Migrations;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Inf.Genericos.Helper;
using Resources;
using System.Data.Entity.Validation;
using System.IO;
using System.Configuration;
using Marfil.Inf.Genericos;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen;
using Marfil.Dom.Persistencia.Model.Documentos.Transformaciones;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes;
using Marfil.Dom.Persistencia.Model.Documentos.DivisionLotes;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    class StockService
    {

        private readonly MarfilEntities _db;
        private readonly IContextService _context;
        private readonly int decimalesmonedas;
        private readonly int decimalesmedidas;

        #region CTR

        public StockService(IContextService context, MarfilEntities db)
        {
            _db = db;
            _context = context;
            decimalesmonedas = 2;
            decimalesmedidas = 3;            
        }

        #endregion

        public void GestionPieza(MovimientosstockModel model)
        {
            if (model.Tipomovimiento == TipoOperacionService.InsertarRecepcionStock && !string.IsNullOrEmpty(model.Lote))
            {
                if (_db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid))
                {
                    throw new ValidationException(string.Format(General.ErrorGenerarMovimientoStock, model.Lote, model.Loteid));
                }
                _db.Stockactual.Add(CrearNuevaPieza(model));
            }
            else if (model.Tipomovimiento == TipoOperacionService.InsertarRecepcionStock)
            {
                //Sin gestión de lote
                if(_db.Stockactual.Any(f => f.empresa == model.Empresa && f.fkarticulos == model.Fkarticulos))
                    _db.Stockactual.AddOrUpdate(EditarPieza(model));
                else
                    _db.Stockactual.Add(CrearNuevaPieza(model));
            }
            else if (model.Tipomovimiento == TipoOperacionService.MovimientoRemedir ||
                    model.Tipomovimiento == TipoOperacionService.MovimientoAlmacen)
            {
                if (model.Tipomovimiento == TipoOperacionService.MovimientoAlmacen)
                    ValidarPerteneceKitOBundle(model);

                var pieza = _db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid &&
                    f.fkarticulos == model.Fkarticulos && f.fkalmacenes == model.Fkalmacenes) ? 
                    EditarPieza(model) : 
                    null;
                
                if (pieza != null)
                {
                    _db.Stockactual.AddOrUpdate(pieza);
                    _db.SaveChanges();
                }
                else
                    throw new ValidationException(string.Format("No se ha encontrado la pieza {0}{1} en el stock", 
                        string.IsNullOrEmpty(model.Lote) ? model.Fkarticulos : model.Lote, model.Loteid));
            }
            else if (model.Tipomovimiento == TipoOperacionService.EliminarCostes ||
                    model.Tipomovimiento == TipoOperacionService.InsertarCostes)
            {
                ModificarCostes(model);
            }
            else if (model.Tipomovimiento == TipoOperacionService.MovimientoBundle)
            {
                MovimientoBundle(model);
            }
            else if (model.Tipomovimiento == TipoOperacionService.MovimientoKit)
            {
                MovimientoKit(model);
            }
            else if (model.Tipomovimiento == TipoOperacionService.InsertarTraspasosalmacen || 
                    model.Tipomovimiento == TipoOperacionService.ActualizarTraspasosalmacen ||
                    model.Tipomovimiento == TipoOperacionService.EliminarTraspasosalmacen)
            {
                TraspasoAlmacen(model);
            }
            else if (model.Tipomovimiento == TipoOperacionService.ActualizarRecepcionStock)
            {
                ActualizarLineasRecepcion(model);
            }
            else
            {
                if (model.Tipomovimiento == TipoOperacionService.InsertarDevolucionEntregaStock ||
                    model.Tipomovimiento == TipoOperacionService.InsertarRecepcionStockDevolucion)
                {
                    PermiteRealizarDevolucion(model);
                }


                //SI LA PIEZA ESTA EN STOCK ACTUAL, LA ACTUALIZAS en el historico, Y SI NO LA CREAS en el historico (EN LAS TRANSFORMACIONES AL FINALZIAR SE CREA LA PIEZA)
                var pieza = _db.Stockactual.Any(f => f.empresa == model.Empresa &&  f.lote == model.Lote && f.loteid == model.Loteid &&
                    f.fkarticulos == model.Fkarticulos && f.fkalmacenes == model.Fkalmacenes) ?
                    EditarPieza(model) :
                    CrearNuevaPieza(model);

                var actualizar = true;

                if (pieza.cantidadtotal <= 0)//(pieza.cantidaddisponible <= 0)
                {
                    var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db);
                    var artObj = articulosService.get(model.Fkarticulos) as ArticulosModel;
                    actualizar = artObj.Stocknegativoautorizado;
                }

                //ENTRA CUANDO FINALIZAS LA TRANSFORMACION
                if (actualizar)
                {
                    _db.Stockactual.AddOrUpdate(pieza);
                }
                //ENTRA CUANDO CREAS LA TRANSFORMACION
                else if (_db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid && 
                        f.fkarticulos == model.Fkarticulos)) //almacen?
                {
                    _db.Stockactual.Remove(pieza);
                }
                else
                    throw new ValidationException(string.Format("No se puede modificar la pieza {0}{1} porque ya no está en el stock", 
                        model.Lote, Funciones.RellenaCod(model.Loteid, 3)));
            }
        }

        public void ActualizarLineasRecepcion(MovimientosstockModel model)
        {
            StockhistoricoAddOrUpdate(model);
            var item = _db.Stockactual.SingleOrDefault(f => f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos &&
                    f.lote == model.Lote && f.loteid == model.Loteid);

            if (item != null)
            {
                var piezautilizada = PiezaUtilizada(model);

                if (!piezautilizada)
                {
                    var operacion = (model.Cantidad < 0) ? -1 : 1;
                    item.metros += model.Metros;
                    if (operacion > 0)
                    {
                        item.largo = model.Largo;
                        item.ancho = model.Ancho;
                        item.grueso = model.Grueso;
                        item.metros = model.Metros;
                    }
                    _db.Stockactual.AddOrUpdate(item);
                }

            }

        }

        public Stockactual CrearNuevaPieza(MovimientosstockModel model)
        {
            var item = _db.Stockactual.Create();
            item.empresa = model.Empresa;
            item.fecha = DateTime.Now;
            item.fkalmacenes = model.Fkalmacenes;
            item.fkalmaceneszona = model.Fkalmaceneszona;
            item.fkarticulos = model.Fkarticulos;
            item.lote = model.Lote;
            item.loteid = model.Loteid;
            item.referenciaproveedor = model.Referenciaproveedor;
            item.tag = model.Tag;
            item.fkunidadesmedida = model.Fkunidadesmedida;
            item.largo = model.Largo;
            item.ancho = model.Ancho;
            item.grueso = model.Grueso;
            item.metros = model.Metros;
            item.cantidadtotal = model.Cantidad;
            item.cantidaddisponible = model.Cantidad;
            item.integridadreferencialflag = Guid.NewGuid();

            //No tiene sentido los costes adicionales en el stock actual
            //item.costeacicionalvariable = model.Costeadicionalvariable;
            //item.costeadicionalmaterial = model.Costeadicionalmaterial;
            //item.costeadicionalotro = model.Costeadicionalotro;
            //item.costeadicionalportes = model.Costeadicionalportes;
            item.pesonetolote = model.Pesoneto;

            item.tipoalmacenlote = (int?)model.Tipodealmacenlote;

            StockhistoricoAddOrUpdate(model);

            return item;
        }

        private Stockactual EditarPieza(MovimientosstockModel model)
        {
            var item = _db.Stockactual.Single(f => f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos &&
                    f.lote == model.Lote && f.loteid == model.Loteid);

            if (model.Tipomovimiento == TipoOperacionService.MovimientoRemedir)
            {
                var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, _db) as ArticulosService;
                var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context, _db) as UnidadesService;
                var articuloObj = articulosService.GetArticulo(model.Fkarticulos);
                var unidadesObj = unidadesService.get(model.Fkunidadesmedida) as UnidadesModel;

                if (articuloObj.Permitemodificarlargo)
                    item.largo = model.Largo;
                if (articuloObj.Permitemodificarancho)
                    item.ancho = model.Ancho;
                if (articuloObj.Permitemodificargrueso)
                    item.grueso = model.Grueso;

                var metros = UnidadesService.CalculaResultado(unidadesObj, model.Cantidad, item.largo, item.ancho, item.grueso, item.metros ?? 0);
                item.metros = metros;

                model.Metros = metros;

                if (model.Pesoneto.HasValue)
                {
                    item.pesonetolote = (model.Pesoneto ?? 0) * (item.metros ?? 0);
                    model.Pesoneto = item.pesonetolote;
                }
                
                item.referenciaproveedor = model.Referenciaproveedor;
                if (model.Fkalmaceneszona > 0)
                    item.fkalmaceneszona = model.Fkalmaceneszona;
                item.fkincidenciasmaterial = model.Fkincidenciasmaterial;
                item.fkcalificacioncomercial = model.Fkcalificacioncomercial;
                item.fktipograno = model.Fktipograno;
                item.fktonomaterial = model.Fktonomaterial;
                item.fkvariedades = model.Fkvariedades;                
            }
            else if (model.Tipomovimiento == TipoOperacionService.MovimientoAlmacen)
            {       
                item.fkalmaceneszona = model.Ubicaciondestino;

                var historicoitem = _db.Stockhistorico.SingleOrDefault(f =>
                    f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote &&
                    f.loteid == model.Loteid) ?? _db.Stockhistorico.Create();

                historicoitem.fkalmaceneszona = model.Ubicaciondestino;

                return item;
            }
            else if (model.Tipomovimiento == TipoOperacionService.InsertarReservaStock ||
                    model.Tipomovimiento == TipoOperacionService.EliminarReservaStock)
            {
                item.cantidaddisponible += model.Cantidad;
            }

            else
            {
                var operacion = (model.Cantidad < 0) ? -1 : 1;
                var piezautilizada = PiezaUtilizada(model);
                item.cantidadtotal += model.Cantidad;
                item.cantidaddisponible += model.Cantidad;
                item.integridadreferencialflag = Guid.NewGuid();

                if (!piezautilizada)
                {
                    item.metros += model.Metros;
                    if (operacion > 0)
                    {
                        item.largo = model.Largo;
                        item.ancho = model.Ancho;
                        item.grueso = model.Grueso;
                        item.metros = model.Metros;
                    }
                }
                else
                    item.metros += model.Metros * operacion;

                //No tiene sentido los costes adicionales en el stock actual
                //item.costeacicionalvariable = (item.costeacicionalvariable ?? 0) + model.Costeadicionalvariable;
                //item.costeadicionalmaterial = (item.costeadicionalmaterial ?? 0) + model.Costeadicionalmaterial;
                //item.costeadicionalotro = (item.costeadicionalotro ?? 0) + model.Costeadicionalotro;
                //item.costeadicionalportes = (item.costeadicionalportes ?? 0) + model.Costeadicionalportes;

                item.pesonetolote += model.Pesoneto;
                item.tipoalmacenlote = (int?)model.Tipodealmacenlote;
            }

            StockhistoricoAddOrUpdate(model);
            return item;
        }

        private void StockhistoricoAddOrUpdate(MovimientosstockModel model)
        {

            Stockhistorico historicoitem = null;

            //Cuando finalizas una transformacion
            if(model.Tipomovimiento == TipoOperacionService.FinalizarTransformacionloteStock)
            {
                //En teoria siempre deberia de haber en el stock historico
                if(_db.Stockhistorico.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid))
                {
                    //Borramos la que ya habia
                    var piezaAntigua = _db.Stockhistorico.SingleOrDefault(f => f.empresa == model.Empresa && f.lote == model.Lote &&
                    f.loteid == model.Loteid);
                    _db.Stockhistorico.Remove(piezaAntigua); //Eliminamos la antigua
                    historicoitem = _db.Stockhistorico.Create(); //Creamos una nueva
                }                
            }

            else
            {
                historicoitem = _db.Stockhistorico.SingleOrDefault(f =>
                    f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote &&
                    f.loteid == model.Loteid) ?? _db.Stockhistorico.Create();
            }

            historicoitem.empresa = model.Empresa;
            historicoitem.fecha = DateTime.Now;
            historicoitem.fkalmacenes = model.Fkalmacenes;
            if (model.Fkalmaceneszona > 0 && model.Tipomovimiento != TipoOperacionService.EliminarRecepcionStock)
                historicoitem.fkalmaceneszona = model.Fkalmaceneszona;
            historicoitem.fkarticulos = model.Fkarticulos;
            historicoitem.lote = model.Lote;
            historicoitem.loteid = model.Loteid;            
            historicoitem.tag = model.Tag;
            historicoitem.fkunidadesmedida = model.Fkunidadesmedida;
            if (historicoitem.largo == (historicoitem.largoentrada ?? 0))
                historicoitem.largo = model.Largo;
            if (historicoitem.ancho == (historicoitem.anchoentrada ?? 0))
                historicoitem.ancho = model.Ancho;
            if (historicoitem.grueso == (historicoitem.gruesoentrada ?? 0))
                historicoitem.grueso = model.Grueso;

            if (model.Tipomovimiento == TipoOperacionService.MovimientoRemedir)
            {
                if (model.Pesoneto.HasValue)
                {
                    historicoitem.pesonetolote = model.Pesoneto;
                }

                historicoitem.largo = model.Largo;
                historicoitem.ancho = model.Ancho;
                historicoitem.grueso = model.Grueso;
                historicoitem.metros = Math.Round(model.Metros, decimalesmedidas);
                historicoitem.referenciaproveedor = model.Referenciaproveedor;
                historicoitem.fkincidenciasmaterial = model.Fkincidenciasmaterial;
                historicoitem.fkcalificacioncomercial = model.Fkcalificacioncomercial;
                historicoitem.fktipograno = model.Fktipograno;
                historicoitem.fktonomaterial = model.Fktonomaterial;
                historicoitem.fkvariedades = model.Fkvariedades;
            }

            else if (model.Tipomovimiento == TipoOperacionService.InsertarReservaStock ||
                    model.Tipomovimiento == TipoOperacionService.EliminarReservaStock)
            {
                historicoitem.cantidaddisponible += model.Cantidad;
            }


            else                 
            {
                historicoitem.metros = Math.Round((historicoitem.metros ?? 0) + model.Metros, decimalesmedidas);
                historicoitem.cantidadtotal += model.Cantidad;
                historicoitem.cantidaddisponible += model.Cantidad;
                historicoitem.pesonetolote = (historicoitem.pesonetolote ?? 0) + model.Pesoneto;               
            }

            historicoitem.integridadreferencialflag = Guid.NewGuid();
            historicoitem.fkcarpetas = GenerarCarpetaAsociada(model, model.Empresa, _db);

            //if (Tipooperacion == TipoOperacionService.InsertarRecepcionStock ||
            //    Tipooperacion == TipoOperacionService.ActualizarRecepcionStock ||
            //    Tipooperacion == TipoOperacionService.EliminarRecepcionStock ||
            //    Tipooperacion == TipoOperacionService.InsertarTransformacionEntradaStock ||
            //    Tipooperacion == TipoOperacionService.ActualizarTransformacionEntradaStock ||
            //    Tipooperacion == TipoOperacionService.EliminarTransformacionEntradaStock ||
            //    Tipooperacion == TipoOperacionService.InsertarTraspasosalmacen ||
            //    Tipooperacion == TipoOperacionService.ActualizarTraspasosalmacen ||
            //    Tipooperacion == TipoOperacionService.EliminarTraspasosalmacen)
            //{
            //    var operacion = (model.Cantidad < 0) ? -1 : 1;

            //    //costes historico
            //    historicoitem.costeacicionalvariable = model.Costeadicionalvariable * operacion;
            //    historicoitem.costeadicionalmaterial = model.Costeadicionalmaterial * operacion;
            //    historicoitem.costeadicionalotro = model.Costeadicionalotro * operacion;
            //    historicoitem.costeadicionalportes = model.Costeadicionalportes * operacion;
            //}

            if (model.Tipomovimiento == TipoOperacionService.InsertarRecepcionStock ||
                model.Tipomovimiento == TipoOperacionService.EliminarRecepcionStock ||
                model.Tipomovimiento == TipoOperacionService.InsertarTransformacionEntradaStock ||
                model.Tipomovimiento == TipoOperacionService.InsertarDivisionLotesEntradaStock ||
                model.Tipomovimiento == TipoOperacionService.InsertarTraspasosalmacen ||
                model.Tipomovimiento == TipoOperacionService.FinalizarTransformacionloteStock)
            {
                historicoitem.costeadicionalmaterial = Math.Round((double)((historicoitem.costeadicionalmaterial ?? 0) + model.Costeadicionalmaterial ?? 0), decimalesmonedas);
                historicoitem.costeadicionalportes = Math.Round((double)((historicoitem.costeadicionalportes ?? 0) + model.Costeadicionalportes ?? 0), decimalesmonedas);
                historicoitem.costeadicionalotro = Math.Round((double)((historicoitem.costeadicionalotro ?? 0) + model.Costeadicionalotro ?? 0), decimalesmonedas);
                historicoitem.costeacicionalvariable = Math.Round((double)((historicoitem.costeacicionalvariable ?? 0) + model.Costeadicionalvariable ?? 0), decimalesmonedas);
            }

            //jmm            
            if (model.Tipomovimiento == TipoOperacionService.InsertarRecepcionStock ||
                model.Tipomovimiento == TipoOperacionService.InsertarTransformacionEntradaStock ||
                model.Tipomovimiento == TipoOperacionService.InsertarDivisionLotesEntradaStock || 
                model.Tipomovimiento == TipoOperacionService.FinalizarTransformacionloteStock)
            {
                dynamic xml;
                if (model.Tipomovimiento == TipoOperacionService.InsertarRecepcionStock)
                {
                    var serializer = new Serializer<AlbaranesComprasDiarioStockSerializable>();
                    xml = serializer.SetXml(model.Documentomovimiento);                                                           
                    historicoitem.netocompra = (historicoitem.netocompra ?? 0) + xml.Linea.importe;
                    historicoitem.preciovaloracion = xml.Linea.precio;
                }
                else if (model.Tipomovimiento == TipoOperacionService.InsertarTransformacionEntradaStock)
                {
                    var serializer = new Serializer<TransformacionesentradaDiarioStockSerializable>();
                    xml = serializer.SetXml(model.Documentomovimiento);
                    historicoitem.netocompra = xml.Linea.precio;
                    historicoitem.preciovaloracion = Math.Round((xml.Linea.precio / model.Metros), 2);
                }
                else if (model.Tipomovimiento == TipoOperacionService.InsertarDivisionLotesEntradaStock)
                {
                    var serializer = new Serializer<DivisionLotesEntradaSerializable>();
                    xml = serializer.SetXml(model.Documentomovimiento);
                    historicoitem.netocompra = xml.Linea.precio;
                    historicoitem.preciovaloracion = Math.Round((xml.Linea.precio / model.Metros), 2);
                }
                else
                {
                    var serializer = new Serializer<TransformacioneslotesDiarioStockSerializable>();
                    xml = serializer.SetXml(model.Documentomovimiento);
                    historicoitem.netocompra = xml.Linea.precio;
                    historicoitem.preciovaloracion = Math.Round((xml.Linea.precio / model.Metros), 2);
                }
                    
                historicoitem.codigoproveedor = xml.Codigoproveedor;
                historicoitem.fechaentrada = xml.Fechadocumento;
                historicoitem.precioentrada = xml.Linea.precio;
                historicoitem.referenciaentrada = xml.Referencia;
                historicoitem.codigodocumentoentrada = xml.Id;
                historicoitem.cantidadentrada = (historicoitem.cantidadentrada ?? 0) + model.Cantidad;
                historicoitem.largoentrada = xml.Linea.largo;
                historicoitem.anchoentrada = xml.Linea.ancho;
                historicoitem.gruesoentrada = xml.Linea.grueso;
                historicoitem.metrosentrada = Math.Round((historicoitem.metrosentrada ?? 0) + model.Metros, decimalesmedidas);                                             
            }
            else if (model.Tipomovimiento == TipoOperacionService.EliminarRecepcionStock)
            {
                var serializer = new Serializer<AlbaranesComprasDiarioStockSerializable>();
                var xml = serializer.SetXml(model.Documentomovimiento);
                historicoitem.netocompra += xml.Linea.importe * -1;

                historicoitem.referenciaentrada = null;
                historicoitem.codigodocumentoentrada = null;
                historicoitem.cantidadentrada += model.Cantidad;
                historicoitem.metrosentrada = Math.Round((historicoitem.metrosentrada ?? 0) + model.Metros, decimalesmedidas);                
            }
            else if (model.Tipomovimiento == TipoOperacionService.InsertarRecepcionStockDevolucion ||
                    model.Tipomovimiento == TipoOperacionService.ActualizarRecepcionStockDevolucion)
            {
                var serializer = new Serializer<AlbaranesComprasDiarioStockSerializable>();
                var xml = serializer.SetXml(model.Documentomovimiento);

                // Bloque o tabla es lote unitario, en lote unitario no se resta el netocompra al devolver porque queda en 0     
                var codsFamilias = _db.Familiasproductos.Where(f => f.empresa == model.Empresa && (f.tipofamilia == (int)TipoFamilia.Bloque || f.tipofamilia == (int)TipoFamilia.Tabla)).Select(f => f.id).ToList();
                if (!codsFamilias.Any(f => f == model.Fkarticulos.Substring(0, 2)))
                {
                    historicoitem.netocompra += xml.Linea.importe;
                    historicoitem.metrosentrada = Math.Round((historicoitem.metrosentrada ?? 0) + model.Metros, decimalesmedidas);
                }
                historicoitem.cantidadentrada += model.Cantidad;                               
            }                        
            // TODO: Creo que eliminar y actualizar van junto a EliminarEntregas
            else if (model.Tipomovimiento == TipoOperacionService.InsertarEntregaStock ||
                    model.Tipomovimiento == TipoOperacionService.InsertarTransformacionSalidaStock ||
                    model.Tipomovimiento == TipoOperacionService.InsertarDivisionLotesSalidaStock ||
                    model.Tipomovimiento == TipoOperacionService.EliminarTransformacionSalidaStock ||
                    model.Tipomovimiento == TipoOperacionService.InsertarTransformacionloteStock ||
                    model.Tipomovimiento == TipoOperacionService.ActualizarTransformacionloteStock)
            {
                dynamic xml;
                if (model.Tipomovimiento == TipoOperacionService.InsertarEntregaStock)
                {
                    var serializer = new Serializer<AlbaranesDiarioStockSerializable>();
                    xml = serializer.SetXml(model.Documentomovimiento);
                    historicoitem.codigocliente = xml.Codigocliente;
                }                                
                else if (model.Tipomovimiento == TipoOperacionService.InsertarTransformacionSalidaStock ||
                        model.Tipomovimiento == TipoOperacionService.EliminarTransformacionSalidaStock)
                {
                    var serializer = new Serializer<TransformacionessalidaDiarioStockSerializable>();
                    xml = serializer.SetXml(model.Documentomovimiento);
                }
                else if (model.Tipomovimiento == TipoOperacionService.InsertarDivisionLotesSalidaStock)
                {
                    var serializer = new Serializer<DivisionLotesSalidaSerializable>();
                    xml = serializer.SetXml(model.Documentomovimiento);
                }
                else /*(model.Tipomovimiento == TipoOperacionService.InsertarTransformacionloteStock ||
                        model.Tipomovimiento == TipoOperacionService.ActualizarTransformacionloteStock)*/
                {
                    var serializer = new Serializer<TransformacioneslotesDiarioStockSerializable>();
                    xml = serializer.SetXml(model.Documentomovimiento);
                }

                historicoitem.fechasalida = xml.Fechadocumento;
                historicoitem.preciosalida = xml.Linea.precio;
                historicoitem.referenciasalida = xml.Referencia;
                historicoitem.codigodocumentosalida = xml.Id;
                historicoitem.cantidadsalida = (historicoitem.cantidadsalida ?? 0) + (model.Cantidad * -1); // -1 para que se muestren en positivo
                historicoitem.largosalida = xml.Linea.largo;
                historicoitem.anchosalida = xml.Linea.ancho;
                historicoitem.gruesosalida = xml.Linea.grueso;
                historicoitem.metrossalida = Math.Round((historicoitem.metrossalida ?? 0) + (model.Metros * -1), decimalesmedidas);
            }
            else if (model.Tipomovimiento == TipoOperacionService.EliminarEntregaStock)
            {
                int? loteid = Convert.ToInt32(model.Loteid);
                var albaranAnterior = _db.AlbaranesLin.Where(f => f.empresa == model.Empresa && f.lote == model.Lote && f.tabla == loteid)
                    .Select(f => f.fkalbaranes).ToList().LastOrDefault();

                if (albaranAnterior > 0)
                {
                    historicoitem.referenciasalida = _db.Albaranes.Where(f => f.empresa == model.Empresa && f.id == albaranAnterior)
                    .Select(f => f.referencia).SingleOrDefault();
                    historicoitem.codigodocumentosalida = albaranAnterior;
                }
                else
                {
                    historicoitem.referenciasalida = null;
                    historicoitem.codigodocumentosalida = null;
                }
     
                historicoitem.cantidadsalida += model.Cantidad * -1;
                historicoitem.metrossalida = Math.Round((historicoitem.metrossalida ?? 0) + (model.Metros * -1), decimalesmedidas);
            }           
            else if (model.Tipomovimiento == TipoOperacionService.InsertarDevolucionEntregaStock ||
                    model.Tipomovimiento == TipoOperacionService.ActualizarEntregaStockDevolucion)
            {
                historicoitem.cantidadsalida += model.Cantidad * -1;
                historicoitem.metrossalida = Math.Round((historicoitem.metrossalida ?? 0) + (model.Metros * -1), decimalesmedidas);
            }


            historicoitem.tipoalmacenlote = (int?)model.Tipodealmacenlote;

            _db.Stockhistorico.AddOrUpdate(historicoitem);
        }

        public void TraspasoAlmacen(MovimientosstockModel model)
        {            
            //var serializer = new Serializer<TraspasosalmacenDiarioStockSerializable>();
            //var trazabilidad = serializer.SetXml(model.Documentomovimiento);            

            //if (model.Tipomovimiento == TipoOperacionService.ActualizarTraspasosalmacen || 
            //    model.Tipomovimiento == TipoOperacionService.EliminarTraspasosalmacen)
            //    almacenorigen = model.Fkalmacenes;
                       
            var item = _db.Stockactual.SingleOrDefault(f =>
                f.empresa == model.Empresa && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote && f.loteid == model.Loteid);

            _db.Stockactual.Remove(item);
            _db.SaveChanges();

            var historicoitem = _db.Stockhistorico.SingleOrDefault(f =>
                f.empresa == model.Empresa && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote && f.loteid == model.Loteid);

            if (model.Tipomovimiento == TipoOperacionService.InsertarTraspasosalmacen)
            {
                item.fkalmacenes = model.Fkalmacenes;
                item.fkalmaceneszona = model.Fkalmaceneszona;
                historicoitem.fkalmacenes = model.Fkalmacenes;
                historicoitem.fkalmaceneszona = model.Fkalmaceneszona;
            }

            if (item != null)
            {
                item.costeadicionalmaterial = Math.Round((double)((item.costeadicionalmaterial ?? 0) + model.Costeadicionalmaterial), decimalesmonedas);
                item.costeadicionalportes = Math.Round((double)((item.costeadicionalportes ?? 0) + model.Costeadicionalportes), decimalesmonedas);
                item.costeadicionalotro = Math.Round((double)((item.costeadicionalotro ?? 0) + model.Costeadicionalotro), decimalesmonedas);
                item.costeacicionalvariable = Math.Round((double)((item.costeacicionalvariable ?? 0) + model.Costeadicionalvariable), decimalesmonedas);

                _db.Stockactual.AddOrUpdate(item);                
            }

            historicoitem.costeadicionalmaterial = Math.Round((double)((historicoitem.costeadicionalmaterial ?? 0) + model.Costeadicionalmaterial), decimalesmonedas);
            historicoitem.costeadicionalportes = Math.Round((double)((historicoitem.costeadicionalportes ?? 0) + model.Costeadicionalportes), decimalesmonedas);
            historicoitem.costeadicionalotro = Math.Round((double)((historicoitem.costeadicionalotro ?? 0) + model.Costeadicionalotro), decimalesmonedas);
            historicoitem.costeacicionalvariable = Math.Round((double)((historicoitem.costeacicionalvariable ?? 0) + model.Costeadicionalvariable), decimalesmonedas);

            _db.Stockhistorico.AddOrUpdate(historicoitem);
        }

        public void ModificarCostes(MovimientosstockModel model)
        {
            var item = _db.Stockactual.SingleOrDefault(f =>
                f.empresa == model.Empresa && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote && f.loteid == model.Loteid);

            var historicoitem = _db.Stockhistorico.SingleOrDefault(f =>
                f.empresa == model.Empresa && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote && f.loteid == model.Loteid);

            if (item != null)
            {
                item.costeadicionalmaterial = Math.Round((double)((item.costeadicionalmaterial ?? 0) + model.Costeadicionalmaterial), decimalesmonedas);
                item.costeadicionalportes = Math.Round((double)((item.costeadicionalportes ?? 0) + model.Costeadicionalportes), decimalesmonedas);
                item.costeadicionalotro = Math.Round((double)((item.costeadicionalotro ?? 0) + model.Costeadicionalotro), decimalesmonedas);
                item.costeacicionalvariable = Math.Round((double)((item.costeacicionalvariable ?? 0) + model.Costeadicionalvariable), decimalesmonedas);

                _db.Stockactual.AddOrUpdate(item);
            }

            historicoitem.costeadicionalmaterial = Math.Round((double)((historicoitem.costeadicionalmaterial ?? 0) + model.Costeadicionalmaterial), decimalesmonedas);
            historicoitem.costeadicionalportes = Math.Round((double)((historicoitem.costeadicionalportes ?? 0) + model.Costeadicionalportes), decimalesmonedas);
            historicoitem.costeadicionalotro = Math.Round((double)((historicoitem.costeadicionalotro ?? 0) + model.Costeadicionalotro), decimalesmonedas);
            historicoitem.costeacicionalvariable = Math.Round((double)((historicoitem.costeacicionalvariable ?? 0) + model.Costeadicionalvariable), decimalesmonedas);

            _db.Stockhistorico.AddOrUpdate(historicoitem);
        }

        public void MovimientoBundle(MovimientosstockModel model)
        {
            var bundleService = FService.Instance.GetService(typeof(BundleModel), _context, _db);
            var bundleobj = bundleService.get(model.Fkarticulos) as BundleModel;
            bundleobj.Fkzonaalmacen = model.Fkalmaceneszona;
            bundleService.edit(bundleobj);
        }

        public void MovimientoKit(MovimientosstockModel model)
        {
            var kitService = FService.Instance.GetService(typeof(KitModel), _context, _db);
            var kitobj = kitService.get(model.Fkarticulos) as KitModel;
            kitobj.Fkzonalamacen = model.Fkalmaceneszona;
            kitService.edit(kitobj);
        }

        private bool PermiteRealizarDevolucion(MovimientosstockModel model)
        {
            var tablaid = Funciones.Qint(model.Loteid);

            if (model.Tipomovimiento == TipoOperacionService.InsertarRecepcionStockDevolucion)
            {
                if (!_db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid))
                    throw new Exception("La pieza no existe en el stock");

                if (_db.Transformacionesloteslin.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.tabla == tablaid))
                    throw new Exception("La pieza no se puede devolver porque ha pasado por un proceso de transformación de acabados");

                if (_db.Transformacionessalidalin.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.tabla == tablaid))
                    throw new Exception("La pieza no se puede devolver porque ha pasado por un proceso de transformación");

                if (_db.Stockactual.Where(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == tablaid.ToString())
                    .Select(f => f.cantidaddisponible).SingleOrDefault() < (model.Cantidad * -1)) // * -1 porque la cantidad al devolver viene en negativo
                    throw new Exception("La cantidad a devolver supera la cantidad disponible");
            }
            else if (model.Tipomovimiento == TipoOperacionService.InsertarDevolucionEntregaStock)
            {
                //if (Db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid))
                //  throw new Exception("La pieza ya existe en el stock");            

                var serializer = new Serializer<AlbaranesDiarioStockSerializable>();
                var documentoOrig = serializer.SetXml(model.Documentomovimiento).Linea.documentoorigen;

                var albaranOrig = _db.Albaranes.Include("AlbaranesLin").Where(f => f.empresa == model.Empresa && f.referencia == documentoOrig).SingleOrDefault();

                var cantidadVendida = albaranOrig.AlbaranesLin.Where(f => f.fkarticulos == model.Fkarticulos && f.lote == model.Lote && f.tabla.ToString() == model.Loteid).Sum(f => f.cantidad);

                var cantidadDevuelta = _db.AlbaranesLin.Where(f => f.empresa == model.Empresa && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote && f.tabla.ToString() == model.Loteid
                                                                && f.documentoorigen == documentoOrig).Sum(f => f.cantidad);

                if (cantidadVendida + cantidadDevuelta < 0)
                    throw new Exception("La cantidad a devolver supera la cantidad vendida");
            }

            return true;
        }

        private void ValidarPerteneceKitOBundle(MovimientosstockModel model)
        {
            if (_db.Kit.Include("KitLin").Any(f => (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado) && f.empresa == model.Empresa && f.KitLin.Any(j => j.lote == model.Lote && j.loteid == model.Loteid && j.empresa == model.Empresa)))
            {
                var kitobj =
                    _db.Kit.Include("KitLin")
                        .First(
                            f =>
                                (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado) &&
                                f.empresa == model.Empresa &&
                                f.KitLin.Any(
                                    j => j.lote == model.Lote && j.loteid == model.Loteid && j.empresa == model.Empresa));
                throw new ValidationException(string.Format("No se puede mover la pieza porque pertenece al Kit {0}", kitobj.referencia));
            }

            if (_db.Bundle.Include("BundleLin").Any(f => f.empresa == model.Empresa && f.BundleLin.Any(j => j.lote == model.Lote && j.loteid == model.Loteid && j.empresa == model.Empresa)))
            {
                var bundleobj =
                    _db.Bundle.Include("BundleLin")
                        .First(
                            f =>
                                f.empresa == model.Empresa &&
                                f.BundleLin.Any(
                                    j => j.lote == model.Lote && j.loteid == model.Loteid && j.empresa == model.Empresa));
                throw new ValidationException(string.Format("No se puede mover la pieza porque pertenece al bundle {0} {1}", bundleobj.lote, bundleobj.id));
            }
        }

        private bool PiezaUtilizada(MovimientosstockModel model)
        {
            return model.Tipomovimiento == TipoOperacionService.ActualizarRecepcionStock && _db.Movimientosstock.Any(
                f => f.empresa == model.Empresa && f.tipooperacion == (int)TipoOperacionService.MovimientoRemedir && f.lote == model.Lote && f.loteid == model.Loteid);
        }

        private Guid? GenerarCarpetaAsociada(MovimientosstockModel model, string empresa, MarfilEntities db)
        {
            var carpetasService = FService.Instance.GetService(typeof(CarpetasModel), _context, db) as CarpetasService;

            var Loteid = Funciones.Qnull(model.Loteid).PadLeft(3, '0');

            if (!carpetasService.ExisteCarpeta(Path.Combine(ConfigurationManager.AppSettings["FileManagerNodoRaiz"],
            "Lotes", "Imagenes", string.Format("{0}{1}", model.Lote, Loteid))))
            {
                var carpeta = carpetasService.GenerarCarpetaAsociada("Lotes", "Imagenes", string.Format("{0}{1}", model.Lote, Loteid));
                return carpeta.Id;
            }else
            {
                var ruta = carpetasService.GenerateRutaCarpeta("Lotes", "Imagenes", string.Format("{0}{1}", model.Lote, Loteid));
                var carpeta = carpetasService.GetCarpeta(ruta);                
                return carpeta.Id;
            }
        }

    }
}
