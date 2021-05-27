using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Model.Ficheros;
using System.Configuration;
using System.IO;
using DevExpress.DataAccess.Sql;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface ILotesService
    {
        Task<LotesModel> GetAsync(string id);
        LotesModel Get(string id);
    }

    public class LotesService : IDisposable, ILotesService
    {
        #region Members

        private readonly MarfilEntities _db;
        private readonly IContextService _context;
        
        #endregion

        #region CTR

        public LotesService(IContextService service)
        {
            _db = MarfilEntities.ConnectToSqlServer(service.BaseDatos);
            _context = service;
            
        }

        #endregion

        #region Api

        public async Task<LotesModel> GetAsync(string id)
        {
            return await Task.Run(() => Get(id));
        }


        public LotesModel GetByReferencia(string referencialote)
        {
            var obj = _db.Stockhistorico.Single(f => f.lote+f.loteid ==referencialote && f.empresa == _context.Empresa);
            return Get(obj.id.ToString());
        }

        public LotesModel Get(string id)
        {
            var intid = int.Parse(id);
            var obj = _db.Stockhistorico.Single(f => f.id == intid && f.empresa == _context.Empresa);
            var unidades = _db.Unidades.Single(f => f.id == obj.fkunidadesmedida);
            var loteid = Funciones.Qint(obj.loteid);

            var estadoActual = EstadoStock.SinStock;
            if (_db.Stockactual.Any(
                    f =>
                        f.cantidaddisponible > 0 && f.empresa == obj.empresa && f.lote == obj.lote &&
                        f.loteid == obj.loteid && f.fkarticulos == obj.fkarticulos))
            {
                estadoActual = EstadoStock.EnStock;
            }
            else if (_db.ReservasstockLin.Include("Reservasstock").Any(f => f.empresa == obj.empresa && f.lote == obj.lote && f.tabla == loteid && _db.Estados.Any(j => (j.documento + "-" + j.id) == f.Reservasstock.fkestados && j.tipoestado < (int)TipoEstado.Finalizado)))
            {
                estadoActual = EstadoStock.Reservado;
            }
            else if (
                _db.Transformacionesloteslin.Include("Transformacioneslotes")
                    .Any(
                        f =>
                            f.empresa == obj.empresa && f.lote == obj.lote && f.tabla == loteid &&
                            _db.Estados.Any(
                                j =>
                                    (j.documento + "-" + j.id) == f.Transformacioneslotes.fkestados &&
                                    j.tipoestado < (int) TipoEstado.Finalizado)))
            {
                estadoActual = EstadoStock.Transformacion;
            }
                var result = new LotesModel();
            result.Estado = estadoActual;
            result.Fkvariedades = obj.fkvariedades;
            result.Loteproveedor = obj.referenciaproveedor;
            result.Fkalmacenes = obj.fkalmacenes;
            result.NombreAlmacen = _db.Almacenes.Where(f => f.empresa == obj.empresa && f.id == obj.fkalmacenes).Select(f => f.descripcion).SingleOrDefault();
            result.Zona = obj.fkalmaceneszona?.ToString();
            result.Fkcalificacioncomercial = obj.fkcalificacioncomercial;
            result.Fkalmaceneszona = obj.fkalmaceneszona?.ToString() ?? string.Empty;
            result.Fkincidenciasmaterial = obj.fkincidenciasmaterial;
            result.Fktipograno = obj.fktipograno;
            result.Fktonomaterial = obj.fktonomaterial;
            result.Lotereferencia = string.Format("{0}{1}", obj.lote, Funciones.RellenaCod(obj.loteid, 3));
            result.Tipodealmacenlote = (TipoAlmacenlote?)obj.tipoalmacenlote;
            result.Fkcarpetas = obj.fkcarpetas;
            result.Decimales = unidades.decimalestotales;
            result.CantidadProduccion = obj.cantidaddisponible;
            result.LargoProduccion = obj.largo;
            result.AnchoProduccion = obj.ancho;
            result.GruesoProduccion = obj.grueso;
            var modelUnidades = new UnidadesModel();
            modelUnidades.Formula = (TipoStockFormulas)unidades.formula;
            var metros = obj.metros??0;
            result.MetrosProduccion = obj.cantidaddisponible ==0 ? UnidadesService.CalculaResultado(modelUnidades, 1 ,obj.largo,obj.ancho,obj.grueso, metros):obj.metros;            
            result.Unidades = unidades.codigounidad;            
            result.Formula = unidades.formula;
            result.Costeadicionalmaterial = obj.costeadicionalmaterial;
            result.Costeadicionalportes = obj.costeadicionalportes;
            result.Costeadicionalotro = obj.costeadicionalotro;
            result.Costeadicionalvariable = obj.costeacicionalvariable;
            result.Pesoneto = obj.pesonetolote;

            // jmm
            result.Codigoproveedor = obj.codigoproveedor;
            result.Fechaentrada = obj.fechaentrada;
            result.Precioentrada = obj.precioentrada;
            result.Referenciaentrada = obj.referenciaentrada;
            result.Codigodocumentoentrada = obj.codigodocumentoentrada.ToString();
            result.CantidadEntrada = obj.cantidadentrada;
            result.LargoEntrada = obj.largoentrada;
            result.AnchoEntrada = obj.anchoentrada;
            result.GruesoEntrada = obj.gruesoentrada;
            result.MetrosEntrada = obj.metrosentrada;
            result.Netocompra = obj.netocompra;
            result.Preciovaloracion = obj.preciovaloracion;
            result.Codigocliente = obj.codigocliente;
            result.Fechasalida = obj.fechasalida;
            result.Preciosalida = obj.preciosalida;
            result.Referenciasalida = obj.referenciasalida;
            result.Codigodocumentosalida = obj.codigodocumentosalida.ToString();
            result.CantidadSalida = obj.cantidadsalida;
            result.LargoSalida = obj.largosalida;
            result.AnchoSalida = obj.anchosalida;
            result.GruesoSalida = obj.gruesosalida;
            result.MetrosSalida = obj.metrossalida;                                    
            result.CantidadProduccion = obj.cantidaddisponible;
            result.MetrosDisponibles = UnidadesService.CalculaResultado(modelUnidades, obj.cantidaddisponible, obj.largo, obj.ancho, obj.grueso, metros);

            //articulo descripcion
            result.Fkarticulos = obj.fkarticulos;

            //entrada
            //var entradaObj = GetModeloEntrada(obj.lote, loteid);
            //if (entradaObj != null)
            //{
            //    result.Codigoproveedor = entradaObj.Cuenta;
            //    result.Fechaentrada = entradaObj.Fecha;
            //    result.Precioentrada = entradaObj.Precio;
            //    result.Referenciaentrada = entradaObj.Referencia;
            //    result.Codigodocumentoentrada = entradaObj.Codigodocumento;
            //    result.CantidadEntrada = entradaObj.Cantidad;
            //    result.LargoEntrada = entradaObj.Largo;
            //    result.AnchoEntrada = entradaObj.Ancho;
            //    result.GruesoEntrada = entradaObj.Grueso;
            //    result.MetrosEntrada = entradaObj.Metros;
            //    result.Netocompra = entradaObj.Precio;                
            //}

            ////salida
            //var salidaObj = GetModeloSalida(obj.lote,loteid);
            //if (salidaObj != null)
            //{
            //    result.Codigocliente = salidaObj.Cuenta;
            //    result.Fechasalida = salidaObj.Fecha;
            //    result.Preciosalida = salidaObj.Precio;
            //    result.Referenciasalida = salidaObj.Referencia;
            //    result.Codigodocumentosalida = salidaObj.Codigodocumento;
            //    result.CantidadSalida = salidaObj.Cantidad;
            //    result.LargoSalida = salidaObj.Largo;
            //    result.AnchoSalida = salidaObj.Ancho;
            //    result.GruesoSalida = salidaObj.Grueso;
            //    result.MetrosSalida = salidaObj.Metros;
            //}

            result.Documentosrelacionados = GetDocumentosRelacionados(obj.lote,obj.loteid,obj.fkarticulos);

            return result;
        }
        
        #endregion

        #region Helper

        #region GetModeloEntradaSalida Lote+Loteid
        private LineasLotes GetModeloEntrada(string lote, int? loteid)
        {
            LineasLotes result=null;

            var entradaObj = _db.AlbaranesComprasLin.Include("AlbaranesCompras").FirstOrDefault(f => f.lote == lote && f.tabla == loteid && f.empresa == _context.Empresa);
            if (entradaObj != null)
            {
                result = new LineasLotes
                {
                    Cuenta = entradaObj.AlbaranesCompras.fkproveedores,
                    Fecha = entradaObj.AlbaranesCompras.fechadocumento,
                    Precio = entradaObj.importe,
                    Referencia = entradaObj.AlbaranesCompras.referencia,
                    Codigodocumento = entradaObj.AlbaranesCompras.id.ToString(),
                    Cantidad = entradaObj.cantidad,
                    Largo = entradaObj.largo,
                    Ancho = entradaObj.ancho,
                    Grueso = entradaObj.grueso,
                    Metros = entradaObj.metros
                    //Preciovaloracion = entradaObj.preciovaloracion
                };
            }
            else
            {
                var transformacionObj = _db.Transformacionesentradalin.Include("Transformaciones").FirstOrDefault(f => f.lote == lote && f.tabla == loteid && f.empresa == _context.Empresa);
                if (transformacionObj != null)
                {
                    result = new LineasLotes
                    {
                        Cuenta = transformacionObj.Transformaciones.fkproveedores,
                        Fecha = transformacionObj.Transformaciones.fechadocumento,
                        Precio = transformacionObj.precio,
                        Referencia = transformacionObj.Transformaciones.referencia,
                        Codigodocumento = transformacionObj.Transformaciones.id.ToString(),
                        Cantidad = transformacionObj.cantidad,
                        Largo = transformacionObj.largo,
                        Ancho = transformacionObj.ancho,
                        Grueso = transformacionObj.grueso,
                        Metros = transformacionObj.metros
                    };
                }
            }

            return result;
        }

        private LineasLotes GetModeloSalida(string lote, int? loteid)
        {
            LineasLotes result = null;

            var salidaObj = _db.AlbaranesLin.Include("Albaranes").FirstOrDefault(f => f.lote == lote && f.tabla == loteid && f.empresa == _context.Empresa);
            if (salidaObj != null)
            {
                result = new LineasLotes
                {
                    Cuenta = salidaObj.Albaranes.fkclientes,
                    Fecha = salidaObj.Albaranes.fechadocumento,
                    Precio = salidaObj.importe,
                    Referencia = salidaObj.Albaranes.referencia,
                    Codigodocumento = salidaObj.Albaranes.id.ToString(),
                    Largo = salidaObj.largo,
                    Ancho = salidaObj.ancho,
                    Grueso = salidaObj.grueso,
                    Metros = salidaObj.metros
                };
                var sumacantidad = _db.AlbaranesLin.Where(f => f.lote == lote && f.tabla == loteid && f.empresa == _context.Empresa).Sum(f => f.cantidad);
                result.Cantidad = sumacantidad;

            }
            else
            {
                var transformacionObj = _db.Transformacionessalidalin.Include("Transformaciones").FirstOrDefault(f => f.lote == lote && f.tabla == loteid && f.empresa == _context.Empresa);
                if (transformacionObj != null)
                {
                    result = new LineasLotes
                    {
                        Cuenta = transformacionObj.Transformaciones.fkproveedores,
                        Fecha = transformacionObj.Transformaciones.fechadocumento,
                        Precio = 0,
                        Referencia = transformacionObj.Transformaciones.referencia,
                        Codigodocumento = transformacionObj.Transformaciones.id.ToString(),
                        Cantidad = transformacionObj.cantidad,
                        Largo = transformacionObj.largo,
                        Ancho = transformacionObj.ancho,
                        Grueso = transformacionObj.grueso,
                        Metros = transformacionObj.metros
                    };
                }
            }

            return result;
        }
        #endregion GetModeloEntradaSalida Lote+Loteid
     
        /// <summary>
        /// lote -> Lote seleccionado
        /// loteid -> id lote
        /// fkarticulo -> id articulo (gestion de stock pero sin gestión de lote)
        /// </summary>
        /// <param name="lote"></param>
        /// <param name="loteid"></param>
        /// <param name="fkarticulo"></param>
        /// <returns></returns>
        private List<StLotesDocumentosRelacionados> GetDocumentosRelacionados(string lote, string loteid, string fkarticulo)
        {
            var result =
                _db.Diariostock.Where(f => f.empresa == _context.Empresa && f.lote == lote && f.loteid == loteid && f.fkarticulos == fkarticulo)
                    .ToList();
           return result.Select(f=>new StLotesDocumentosRelacionados { Referencia = f.Documentoreferencia, Tipodocumento =(TipoOperacionService?)f.tipooperacion }).Distinct().ToList();
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            _db?.Dispose();
        }

        #endregion

        public LotesModel SubirImagenLote(LotesModel model)
        {
            Stockhistorico pieza = null;
            string lote;
            string loteid;

            CalcularPartesLote(model.Lote, out lote, out loteid);

            try
            {
                pieza = _db.Stockhistorico.First(f => f.empresa == _context.Empresa && f.lote == lote && f.loteid == loteid);

                var carpetasService = FService.Instance.GetService(typeof(CarpetasModel), _context, _db) as CarpetasService;
                var ruta = carpetasService.GenerateRutaCarpeta("Lotes", "Imagenes", model.Lote);
                model.Fkcarpetas = carpetasService.GetCarpeta(ruta)?.Id;
                model.Fkarticulos = pieza.fkarticulos;
                model.Descripcion = _db.Articulos.Where(f => f.empresa == _context.Empresa && f.id == pieza.fkarticulos).Select(f => f.descripcion).SingleOrDefault();
                model.LargoProduccion = pieza.largo;
                model.AnchoProduccion = pieza.ancho;
                model.GruesoProduccion = pieza.grueso;
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("La pieza {0} no existe", model.Lote));
            }              
            
            return model;
        }

        public static void CalcularPartesLote(string referencialote, out string lote, out string loteid)
        {
            lote = "";
            loteid = "";

            if (!string.IsNullOrEmpty(referencialote) && referencialote.Length > 3)
            {
                lote = referencialote.Substring(0, referencialote.Length - 3);
                loteid = referencialote.Substring(referencialote.Length - 3);
                loteid = Funciones.Qint(loteid)?.ToString() ?? string.Empty;
            }

        }
    }
}
