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
using System.Data.Entity.Validation;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    internal class StockCrudService: IStockService
    {

        private readonly IContextService _context;
        #region Properties

        public MarfilEntities Db { get; set; }

        public TipoOperacionService Tipooperacion { get; set; }

        #endregion

        public StockCrudService(IContextService context)
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
        #region HelperPieza
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
            item.metros = model.Metros;
            item.cantidadtotal = model.Cantidad;
            item.cantidaddisponible = model.Cantidad;
            item.integridadreferencialflag = Guid.NewGuid();
            item.costeacicionalvariable = model.Costeadicionalvariable;
            item.costeadicionalmaterial = model.Costeadicionalmaterial;
            item.costeadicionalotro = model.Costeadicionalotro;
            item.costeadicionalportes = model.Costeadicionalportes;
            item.pesonetolote = model.Pesoneto;

            item.tipoalmacenlote = (int?)model.Tipodealmacenlote;

            StockhistoricoAddOrUpdate(model);

            return item;
        }
        private Stockactual EditarPieza(MovimientosstockModel model)
        {
            var item = Db.Stockactual.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.lote == model.Lote &&
                                f.loteid == model.Loteid);

            var operacion = (model.Cantidad < 0) ? -1 : 1;
            var piezautilizada = PiezaUtilizada(model);
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
                item.metros += model.Metros * operacion;


            // costes
            item.costeacicionalvariable = model.Costeadicionalvariable * operacion;
            item.costeadicionalmaterial = model.Costeadicionalmaterial * operacion;
            item.costeadicionalotro = model.Costeadicionalotro * operacion;
            item.costeadicionalportes = model.Costeadicionalportes * operacion;

            item.pesonetolote = model.Pesoneto;

            item.tipoalmacenlote = (int?)model.Tipodealmacenlote;

            //if (Tipooperacion == TipoOperacionService.ActualizarTransformacionEntradaStock)
            //{
                StockhistoricoAddOrUpdate(model);
            //}
            
            return item;
        }
        private void GestionPiezaConLote(MovimientosstockModel model, TipoOperacionService tipooperacion)
        {
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
                    throw new ValidationException(string.Format("No se puede modificar la pieza {0}{1} porque ya no está en el stock", model.Lote, Funciones.RellenaCod(model.Loteid, 3)));

            }

        }

        #endregion HelperPieza

        #region HelperPiezaSinLote

        private Stockactual EditarPiezaSinLote(MovimientosstockModel model)
        {
            var item = Db.Stockactual.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos);

            item.cantidadtotal += model.Cantidad;
            item.cantidaddisponible += model.Cantidad;
            item.integridadreferencialflag = Guid.NewGuid();
            item.metros += model.Metros;
            item.costeacicionalvariable = model.Costeadicionalvariable;
            item.costeadicionalmaterial = model.Costeadicionalmaterial;
            item.costeadicionalotro = model.Costeadicionalotro;
            item.costeadicionalportes = model.Costeadicionalportes;

            var historicoitem = Db.Stockhistorico.Single(f =>
                        f.empresa == model.Empresa && f.fkalmacenes == model.Fkalmacenes && f.fkarticulos == model.Fkarticulos);

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

        #endregion HelperPiezaSinLote

        private void StockhistoricoAddOrUpdate(MovimientosstockModel model)
        {
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
            historicoitem.cantidadtotal += model.Cantidad;
            historicoitem.cantidaddisponible += model.Cantidad;
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

                //costes historico
                historicoitem.costeacicionalvariable = model.Costeadicionalvariable * operacion;
                historicoitem.costeadicionalmaterial = model.Costeadicionalmaterial * operacion;
                historicoitem.costeadicionalotro = model.Costeadicionalotro * operacion;
                historicoitem.costeadicionalportes = model.Costeadicionalportes * operacion;
            }

            //jmm
            //if (model.Referenciaentrada != null && Tipooperacion == TipoOperacionService.InsertarRecepcionStock)
            //{
            //    historicoitem.codigoproveedor = model.Codigoproveedor;
            //    historicoitem.fechaentrada = model.Fechaentrada;
            //    historicoitem.precioentrada = model.Precioentrada;
            //    historicoitem.referenciaentrada = model.Referenciaentrada;
            //    historicoitem.codigodocumentoentrada = model.Codigodocumentoentrada;
            //    historicoitem.cantidadentrada = (historicoitem.cantidadentrada ?? 0) + model.Cantidadentrada;
            //    historicoitem.largoentrada = model.Largoentrada;
            //    historicoitem.anchoentrada = model.Anchoentrada;
            //    historicoitem.gruesoentrada = model.Gruesoentrada;
            //    historicoitem.metrosentrada = (historicoitem.metrosentrada ?? 0) + model.Metrosentrada;
            //    historicoitem.netocompra = model.Netocompra;
            //    historicoitem.preciovaloracion = model.Preciovaloracion;
            //}
            //else if (model.Referenciaentrada != null && Tipooperacion == TipoOperacionService.ActualizarRecepcionStock)
            //{
            //    if (model.Metrosentrada < 0)
            //    {
            //        //borrar devolución                    
            //    }
            //    else
            //    {
            //        //borrar recepción
            //        historicoitem.referenciaentrada = null;
            //        historicoitem.codigodocumentoentrada = null;                    
            //    }                
            //    historicoitem.cantidadentrada += model.Cantidadentrada ?? 0;
            //    historicoitem.metrosentrada += (model.Metrosentrada ?? 0) * -1;
            //}

            //else if (model.Referenciasalida != null && Tipooperacion == TipoOperacionService.InsertarEntregaStock)
            //{
            //    historicoitem.codigocliente = model.Codigocliente;
            //    historicoitem.fechasalida = model.Fechasalida;
            //    historicoitem.preciosalida = model.Preciosalida;
            //    historicoitem.referenciasalida = model.Referenciasalida;
            //    historicoitem.codigodocumentosalida = model.Codigodocumentosalida;
            //    historicoitem.cantidadsalida = (historicoitem.cantidadsalida ?? 0) + (model.Cantidadsalida * -1);
            //    historicoitem.largosalida = model.Largosalida;
            //    historicoitem.anchosalida = model.Anchosalida;
            //    historicoitem.gruesosalida = model.Gruesosalida;
            //    historicoitem.metrossalida = (historicoitem.metrossalida ?? 0) + model.Metros;
            //}
            //else if (model.Referenciasalida != null && Tipooperacion == TipoOperacionService.ActualizarEntregaStock )
            //{
            //    if (model.Cantidad < 0)
            //    {
            //        //borrar devolución
                                        
            //    }
            //    else
            //    {
            //        //borrar entrega   
            //        int? loteid = Convert.ToInt32(model.Loteid);
            //        var albaranAnterior = Db.AlbaranesLin.Where(f => f.empresa == model.Empresa && f.lote == model.Lote && f.tabla == loteid)
            //            .Select(f => f.fkalbaranes).ToList().LastOrDefault();

            //        if (albaranAnterior > 0) {
            //            historicoitem.referenciasalida = Db.Albaranes.Where(f => f.empresa == model.Empresa && f.id == albaranAnterior).Select(f => f.referencia).SingleOrDefault();
            //            historicoitem.codigodocumentosalida = albaranAnterior;
            //        }
            //        else
            //        {
            //            historicoitem.referenciasalida = null;
            //            historicoitem.codigodocumentosalida = null;
            //        }                   
                    
            //    }                                
            //    historicoitem.cantidadsalida += (model.Cantidadsalida ?? 0) * -1;
            //    historicoitem.metrossalida += (model.Metrossalida ?? 0) * -1;
            //}

            historicoitem.tipoalmacenlote = (int?)model.Tipodealmacenlote;

            Db.Stockhistorico.AddOrUpdate(historicoitem);
        }



        private bool PiezaUtilizada(MovimientosstockModel model)
        {
            return Tipooperacion == TipoOperacionService.ActualizarRecepcionStock && Db.Movimientosstock.Any(
                f => f.empresa == model.Empresa && f.tipooperacion == (int) TipoOperacionService.MovimientoRemedir && f.lote==model.Lote && f.loteid== model.Loteid);
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

        #endregion Helper
    }
}
