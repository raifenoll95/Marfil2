using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos.Helper;
using Resources;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    internal class StockDevolucionEntregaStockService : IStockService
    {

        private readonly IContextService _context;
        #region Properties

        public MarfilEntities Db { get; set; }

        public TipoOperacionService Tipooperacion { get; set; }

        #endregion

        public StockDevolucionEntregaStockService(IContextService context)
        {
            _context = context;
        }

        #region Api

        public void GenerarOperacion(IStockPieza entrada)
        {
            var model = entrada as MovimientosstockModel;
            
            if (!string.IsNullOrEmpty(model.Lote))
            {
                GestionPiezaConLote(model, Tipooperacion);
            }
            else
            {
                GestionPiezaSinLote(model);
            }
            Db.SaveChanges();
        }

        #endregion

        #region Helper

        private Stockactual CrearNuevaPieza(MovimientosstockModel model)
        {
            var item = Db.Stockactual.Create();
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
            item.metros =Math.Abs(model.Metros);
            item.cantidadtotal = model.Cantidad;
            item.cantidaddisponible = model.Cantidad;
            item.integridadreferencialflag = Guid.NewGuid();
            item.costeacicionalvariable = model.Costeadicionalvariable;
            item.costeadicionalmaterial = model.Costeadicionalmaterial;
            item.costeadicionalotro = model.Costeadicionalotro;
            item.costeadicionalportes = model.Costeadicionalportes;
            item.pesonetolote = model.Pesoneto;
            var historicoitem = Db.Stockhistorico.SingleOrDefault(f =>
                       f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote &&
                               f.loteid == model.Loteid) ?? Db.Stockhistorico.Create();
            historicoitem.empresa = model.Empresa;
            historicoitem.fecha = DateTime.Now;
            historicoitem.fkalmacenes = model.Fkalmacenes;
            historicoitem.fkalmaceneszona = model.Fkalmaceneszona;
            historicoitem.fkarticulos = model.Fkarticulos;
            historicoitem.lote = model.Lote;
            historicoitem.loteid = model.Loteid;
            historicoitem.referenciaproveedor = model.Referenciaproveedor;
            historicoitem.tag = model.Tag;
            historicoitem.fkunidadesmedida = model.Fkunidadesmedida;
            historicoitem.largo = model.Largo;
            historicoitem.ancho = model.Ancho;
            historicoitem.grueso = model.Grueso;
            historicoitem.metros = model.Metros;
            historicoitem.cantidadtotal = model.Cantidad;
            historicoitem.cantidaddisponible = model.Cantidad;
            //historicoitem.cantidadsalida += model.Cantidadsalida * -1;
            //historicoitem.metrossalida += model.Metrossalida;
            historicoitem.pesonetolote = model.Pesoneto;
            historicoitem.integridadreferencialflag = Guid.NewGuid();
            historicoitem.fkcarpetas = GenerarCarpetaAsociada(model, model.Empresa, Db);
            if (Tipooperacion == TipoOperacionService.InsertarRecepcionStock ||
                Tipooperacion == TipoOperacionService.ActualizarRecepcionStock ||
                Tipooperacion == TipoOperacionService.EliminarRecepcionStock ||
                Tipooperacion == TipoOperacionService.InsertarTransformacionEntradaStock ||
                Tipooperacion == TipoOperacionService.ActualizarTransformacionEntradaStock ||
                Tipooperacion == TipoOperacionService.EliminarTransformacionEntradaStock ||
                Tipooperacion == TipoOperacionService.InsertarTraspasosalmacen ||
                Tipooperacion == TipoOperacionService.ActualizarTraspasosalmacen ||
                Tipooperacion == TipoOperacionService.EliminarTraspasosalmacen)
            {
                var operacion = (model.Cantidad < 0) ? -1 : 1;
                historicoitem.costeacicionalvariable = (historicoitem.costeacicionalvariable??0) + model.Costeadicionalvariable * operacion;
                historicoitem.costeadicionalmaterial = (historicoitem.costeadicionalmaterial ?? 0) + model.Costeadicionalmaterial  * operacion;
                historicoitem.costeadicionalotro = (historicoitem.costeadicionalotro ?? 0) + model.Costeadicionalotro  * operacion;
                historicoitem.costeadicionalportes = (historicoitem.costeadicionalportes ?? 0) + model.Costeadicionalportes * operacion;
            }
            Db.Stockhistorico.AddOrUpdate(historicoitem);

            return item;
        }

        private bool PermiteRealizarDevolucion(MovimientosstockModel model)
        {
            var tablaid = Funciones.Qint(model.Loteid);
            //if (Db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid))
            //  throw new Exception("La pieza ya existe en el stock");            

            var serializer = new Serializer<AlbaranesDiarioStockSerializable>();            
            var documentoOrig = serializer.SetXml(model.Documentomovimiento).Linea.documentoorigen;

            var albaranOrig = Db.Albaranes.Include("AlbaranesLin").Where(f => f.empresa == model.Empresa && f.referencia == documentoOrig).SingleOrDefault();

            var cantidadVendida = albaranOrig.AlbaranesLin.Where(f => f.fkarticulos == model.Fkarticulos && f.lote == model.Lote && f.tabla.ToString() == model.Loteid).Sum(f => f.cantidad);

            var cantidadDevuelta = Db.AlbaranesLin.Where(f => f.empresa == model.Empresa && f.fkarticulos == model.Fkarticulos && f.lote == model.Lote && f.tabla.ToString() == model.Loteid 
                                                            && f.documentoorigen == documentoOrig).Sum(f => f.cantidad);

            if (cantidadVendida + cantidadDevuelta < 0)
                throw new Exception("La cantidad a devolver supera la cantidad vendida");
            

            return true;
        }

        private bool PiezaUtilizada(MovimientosstockModel model)
        {
            return Tipooperacion == TipoOperacionService.ActualizarRecepcionStock && Db.Movimientosstock.Any(
                f => f.empresa == model.Empresa && f.tipooperacion == (int) TipoOperacionService.MovimientoRemedir && f.lote==model.Lote && f.loteid== model.Loteid);
        }

        private Stockactual EditarPieza(MovimientosstockModel model)
        {
            var item = Db.Stockactual.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.lote == model.Lote &&
                                f.loteid == model.Loteid);
            
            var operacion = (model.Cantidad < 0) ? -1:1;
            var piezautilizada =  PiezaUtilizada(model);
            item.cantidadtotal += model.Cantidad;
            item.cantidaddisponible += model.Cantidad;
            item.integridadreferencialflag = Guid.NewGuid();

            if (!piezautilizada)
            {
                if (operacion > 0)
                {
                    item.largo = model.Largo;
                    item.ancho = model.Ancho;
                    item.grueso = model.Grueso;
                    item.metros = model.Metros;
                }
                
            }
            else
                item.metros += model.Metros* operacion;
            item.costeacicionalvariable += model.Costeadicionalvariable* operacion;
            item.costeadicionalmaterial += model.Costeadicionalmaterial * operacion;
            item.costeadicionalotro += model.Costeadicionalotro* operacion;
            item.costeadicionalportes += model.Costeadicionalportes*operacion;
            item.pesonetolote = model.Pesoneto;
            var historicoitem = Db.Stockhistorico.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.lote == model.Lote &&
                                f.loteid == model.Loteid);
           
            //historicoitem.metros = item.metros;
            historicoitem.cantidadtotal = item.cantidadtotal;
            historicoitem.cantidaddisponible = item.cantidaddisponible;
            //historicoitem.cantidadsalida += model.Cantidadsalida * -1;
            //historicoitem.metrossalida += model.Metrossalida;
            historicoitem.integridadreferencialflag = Guid.NewGuid();
            historicoitem.pesonetolote = model.Pesoneto;
            if (Tipooperacion == TipoOperacionService.InsertarRecepcionStock ||
                Tipooperacion == TipoOperacionService.ActualizarRecepcionStock ||
                Tipooperacion == TipoOperacionService.EliminarRecepcionStock ||
                Tipooperacion == TipoOperacionService.InsertarTransformacionEntradaStock ||
                Tipooperacion == TipoOperacionService.ActualizarTransformacionEntradaStock ||
                Tipooperacion == TipoOperacionService.ActualizarTransformacionEntradaStock ||
                Tipooperacion == TipoOperacionService.InsertarTraspasosalmacen ||
                Tipooperacion == TipoOperacionService.ActualizarTraspasosalmacen ||
                Tipooperacion == TipoOperacionService.EliminarTraspasosalmacen)
            {
                if (!piezautilizada)
                {
                    if (operacion > 0)
                    {
                        historicoitem.largo = model.Largo;
                        historicoitem.ancho = model.Ancho;
                        historicoitem.grueso = model.Grueso;
                        historicoitem.metros = model.Metros;
                    }
                    
                }
                historicoitem.costeacicionalvariable += model.Costeadicionalvariable * operacion;
                historicoitem.costeadicionalmaterial += model.Costeadicionalmaterial * operacion;
                historicoitem.costeadicionalotro += model.Costeadicionalotro * operacion;
                historicoitem.costeadicionalportes += model.Costeadicionalportes * operacion;
            }

            return item;
        }

        private Stockactual EditarPiezaSinLote(MovimientosstockModel model)
        {
            var item = Db.Stockactual.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos );

            item.cantidadtotal += model.Cantidad;
            item.cantidaddisponible += model.Cantidad;
            item.integridadreferencialflag = Guid.NewGuid();
            item.metros += model.Metros;
            item.costeacicionalvariable = model.Costeadicionalvariable;
            item.costeadicionalmaterial = model.Costeadicionalmaterial;
            item.costeadicionalotro = model.Costeadicionalotro;
            item.costeadicionalportes = model.Costeadicionalportes;

            var historicoitem = Db.Stockhistorico.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos );

            historicoitem.metros = item.metros;
            historicoitem.cantidadtotal = item.cantidadtotal;
            historicoitem.cantidaddisponible = item.cantidaddisponible;            
            historicoitem.integridadreferencialflag = Guid.NewGuid();
            historicoitem.costeacicionalvariable = model.Costeadicionalvariable;
            historicoitem.costeadicionalmaterial = model.Costeadicionalmaterial;
            historicoitem.costeadicionalotro = model.Costeadicionalotro;
            historicoitem.costeadicionalportes = model.Costeadicionalportes;

            return item;
        }

        private void GestionPiezaSinLote(MovimientosstockModel model)
        {
            var pieza = Db.Stockactual.Any(f => f.empresa == model.Empresa && f.fkarticulos == model.Fkarticulos && f.fkalmacenes == model.Fkalmacenes) ?
                          EditarPiezaSinLote(model) :
                          CrearNuevaPieza(model);
            var actualizar = true;
            if (pieza.cantidaddisponible <= 0)
            {
                var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, Db);
                var artObj = articulosService.get(model.Fkarticulos) as ArticulosModel;
                actualizar = artObj.Stocknegativoautorizado;
            }

            if (actualizar)
            {
                Db.Stockactual.AddOrUpdate(pieza);
            }
            else
                Db.Stockactual.Remove(pieza);
        }

        private void GestionPiezaConLote(MovimientosstockModel model, TipoOperacionService tipooperacion)
        {

            PermiteRealizarDevolucion(model);
            if (tipooperacion == TipoOperacionService.InsertarRecepcionStock)
            {
                if (
                    Db.Stockactual.Any(
                        f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid))
                {
                    throw new ValidationException(string.Format(General.ErrorGenerarMovimientoStock, model.Lote, model.Loteid));
                }

                Db.Stockactual.Add(CrearNuevaPieza(model));
            }
            else
            {
                var pieza = Db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid) ?
                    EditarPieza(model) :
                    CrearNuevaPieza(model);
                var actualizar = true;
                if (pieza.cantidaddisponible <= 0)
                {
                    var articulosService = FService.Instance.GetService(typeof(ArticulosModel), _context, Db);
                    var artObj = articulosService.get(model.Fkarticulos) as ArticulosModel;
                    actualizar = artObj.Stocknegativoautorizado;
                }

                if (actualizar)
                {
                    Db.Stockactual.AddOrUpdate(pieza);
                }
                else if (Db.Stockactual.Any(f => f.empresa == model.Empresa && f.lote == model.Lote && f.loteid == model.Loteid))
                {
                    Db.Stockactual.Remove(pieza);
                }
                else 
                    throw new ValidationException(string.Format("No se puede modificar la pieza {0}{1} porque ya no está en el stock",model.Lote,Funciones.RellenaCod(model.Loteid,3)));
                    
            }

        }

        private Guid? GenerarCarpetaAsociada(MovimientosstockModel model,  string empresa, MarfilEntities db)
        {
            var carpetasService = FService.Instance.GetService(typeof(CarpetasModel), _context, db) as CarpetasService;
            

            if (!carpetasService.ExisteCarpeta(Path.Combine(ConfigurationManager.AppSettings["FileManagerNodoRaiz"],
            "Lotes", "Imagenes",string.Format("{0}{1}",model.Lote,Funciones.Qnull(model.Loteid)))))
            {
                var carpeta = carpetasService.GenerarCarpetaAsociada("Lotes", "Imagenes", string.Format("{0}{1}", model.Lote, Funciones.Qnull(model.Loteid)));
                return carpeta.Id;
            }
            return null;
        }

        #endregion
    }
}
