using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Transactions;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Inf.Genericos.Helper;
using Resources;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Stock
{
    public class StockactualService
    {
        private readonly IContextService _context;
        #region Members

        private readonly MarfilEntities _db;

        #endregion

        #region CTR

        public StockactualService(IContextService context,MarfilEntities db)
        {
            _db = db;
            _context = context;
        }

        #endregion

        #region Api

        #region Generar movimiento

        public void GenerarMovimiento(IStockPieza model, TipoOperacionService tipooperacion)
        {
            if(model is IKitStockPieza)
                tipooperacion=TipoOperacionService.MovimientoKit;
            else if(model is IBundleStockPieza)
                tipooperacion=TipoOperacionService.MovimientoBundle;
            

            var service = FStockService.Instance.GenerarServicio(_context,tipooperacion, _db);
            service.GenerarOperacion(model);
        }

        #endregion

        public static void CalcularPartesLote(string referencialote, out string lote, out string loteid)
        {
            lote = "";
            loteid = "";

            if (!string.IsNullOrEmpty(referencialote) && referencialote.Length > 3)
            {
                lote = referencialote.Substring(0,referencialote.Length - 3);
                loteid = referencialote.Substring(referencialote.Length - 3);
                loteid = Funciones.Qint(loteid)?.ToString() ?? string.Empty;
            }
            
        }

        #region Busqueda articulos lotes agrupado STOCK ACTUAL

        public IEnumerable<StockActualMobileModel> GetArticulosLotesAgrupados(string fkalmacen, string empresa, string articulo)
        {
            return _db.Database.SqlQuery<StockActualMobileModel>(CadenaAgrupadoArticulosLote(),new SqlParameter("empresa",empresa),new SqlParameter("articulos",articulo), new SqlParameter("fkalmacen", fkalmacen)).ToList();
        }

        //Le pasas el articulo que has puesto y te devuelve los lotes de stock que tienen ese articulo (agrupados)
        private string CadenaAgrupadoArticulosLote()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" select s.fkarticulos as [Fkarticulos],a.descripcion as [Articulo],s.lote as [Lote],fp.descripcion as [Familia],ml.descripcion as [Material],fm.Descripcion as [Familiamaterial], sum(s.cantidaddisponible) as [Cantidaddisponible],sum(s.metros) as [Metros],u.codigounidad as [UM],u.decimalestotales as [_decimales] from stockactual as s ");
            sb.AppendFormat(" inner join articulos as a on a.id = s.fkarticulos and a.empresa= s.empresa ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.id = substring(s.fkarticulos, 0, 3) and fp.empresa= a.empresa ");
            sb.AppendFormat(" inner join unidades as u on u.id = fp.fkunidadesmedida ");
            sb.AppendFormat(" left join materiales as ml on ml.id = substring(s.fkarticulos, 3, 3) and ml.empresa= a.empresa ");
            sb.AppendFormat(" left join Familiamateriales  as fm on fm.valor=ml.fkfamiliamateriales ");
            sb.AppendFormat(" where s.empresa=@empresa and s.fkarticulos=@articulos and s.fkalmacenes =@fkalmacen");
            sb.AppendFormat(" group by s.fkarticulos, a.descripcion,fp.descripcion,ml.descripcion,fm.descripcion, s.lote,u.codigounidad,u.decimalestotales ");
            return sb.ToString();
        }

        #endregion

        #region Busqueda articulos lotes agrupado STOCK HISTORICO

        public IEnumerable<StockActualMobileModel> GetArticulosLotesAgrupadosHistorico(string fkalmacen, string empresa, string articulo)
        {
            return _db.Database.SqlQuery<StockActualMobileModel>(CadenaAgrupadoArticulosLoteHistorico(), new SqlParameter("empresa", empresa), new SqlParameter("articulos", articulo), new SqlParameter("fkalmacen", fkalmacen)).ToList();
        }

        //Le pasas el articulo que has puesto y te devuelve los lotes de stock historico que tienen ese articulo (agrupados)
        private string CadenaAgrupadoArticulosLoteHistorico()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" select s.fkarticulos as [Fkarticulos],a.descripcion as [Articulo],s.lote as [Lote],fp.descripcion as [Familia],ml.descripcion as [Material],fm.Descripcion as [Familiamaterial], sum(s.cantidaddisponible) as [Cantidaddisponible],sum(s.metros) as [Metros],u.codigounidad as [UM],u.decimalestotales as [_decimales] from stockhistorico as s ");
            sb.AppendFormat(" inner join articulos as a on a.id = s.fkarticulos and a.empresa= s.empresa ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.id = substring(s.fkarticulos, 0, 3) and fp.empresa= a.empresa ");
            sb.AppendFormat(" inner join unidades as u on u.id = fp.fkunidadesmedida ");
            sb.AppendFormat(" left join materiales as ml on ml.id = substring(s.fkarticulos, 3, 3) and ml.empresa= a.empresa ");
            sb.AppendFormat(" left join Familiamateriales  as fm on fm.valor=ml.fkfamiliamateriales ");
            sb.AppendFormat(" where s.empresa=@empresa and s.fkarticulos=@articulos and s.fkalmacenes =@fkalmacen");
            sb.AppendFormat(" group by s.fkarticulos, a.descripcion,fp.descripcion,ml.descripcion,fm.descripcion, s.lote,u.codigounidad,u.decimalestotales ");
            return sb.ToString();
        }

        //Devuelve los lotes de ese articulo con ese lote stock historico
        public IEnumerable<StockActualVistaModelHistorico> GetArticulosLotesHistorico(string fkalmacen, string empresa, string articulodesde, string articulohasta, string familiadesde, string familiahasta, string lotedesde, string lotehasta, bool solotablas, TipoCategoria flujo, string acabadodesde = "", string acabadohasta = "")
        {
            return _db.Database.SqlQuery<StockActualVistaModelHistorico>(GetCadenaBusquedaLotesHsitorico(fkalmacen, empresa, articulodesde, articulohasta, familiadesde, familiahasta, lotedesde, lotehasta, solotablas, flujo, acabadodesde, acabadohasta)).ToList();
        }

        private string GetCadenaBusquedaLotesHsitorico(string fkalmacen, string empresa, string articulodesde, string articulohasta,
            string familiadesde, string familiahasta, string lotedesde, string lotehasta, bool solotablas, TipoCategoria flujo, string acabadodesde, string acabadohasta)
        {
            var a = string.Format("select s.*, s.metrosentrada as [MetrosEntrada],s.cantidaddisponible as [Cantidad],u.decimalestotales as [Decimalesmedidas],a.descripcionabreviada as [Descripcion],fp.id as [Fkfamilias],concat(bl.fkbundlelote,bl.fkbundle) as [Bundle],REPLACE(STR(s.loteid, 3), SPACE(1), '0') as Loteidentificador   from stockhistorico as s " +
                                 " inner join articulos as a on a.empresa= s.empresa and a.id=s.fkarticulos and (a.categoria=0 or a.categoria=" + (int)flujo + ")" +
                                 " inner join familiasproductos as fp on fp.empresa= s.empresa and fp.id=substring(s.fkarticulos,0,3) {3}" +
                                 " inner join unidades as u on u.id= s.fkunidadesmedida" +
                                 " left join bundlelin as bl on bl.empresa= s.empresa and bl.lote= s.lote and bl.loteid=s.loteid and bl.fkalmacenes=s.fkalmacenes and bl.fkarticulos= s.fkarticulos " +
                                 " where isnull(s.lote,'') <> '' and s.empresa='{0}' and s.fkalmacenes='{1}' {2} order by  s.lote asc,REPLACE(STR(s.loteid, 3), SPACE(1), '0') asc", empresa, fkalmacen, CrearFiltrosArticulos(articulodesde, articulohasta, lotedesde, lotehasta, acabadodesde, acabadohasta), CrearFiltrosFamilias(familiadesde, familiahasta, solotablas));
            return a;
        }
        #endregion

        #region Get articuloLote

        //Devuelve los lotes de ese articulo con ese lote stock
        public StockActualVistaModel GetArticuloLote(string fkalmacen, string empresa, string articulodesde, string articulohasta, string familiadesde, string familiahasta, string lotedesde, string lotehasta, bool solotablas,string lote)
        {
            return _db.Database.SqlQuery<StockActualVistaModel>(GetCadenaBusquedaLote(fkalmacen, empresa, articulodesde, articulohasta, familiadesde, familiahasta, lotedesde, lotehasta, solotablas,lote)).FirstOrDefault();
        }
        
        private string GetCadenaBusquedaLote(string fkalmacen, string empresa, string articulodesde, string articulohasta,
            string familiadesde, string familiahasta, string lotedesde, string lotehasta, bool solotablas,string lote)
        {
            return string.Format("select top 1 s.*,s.cantidaddisponible as [Cantidad],u.decimalestotales as [Decimalesmedidas],a.descripcionabreviada as [Descripcion],fp.id as [Fkfamilias],b.descripcion as [Bundle]  from stockactual as s " +
                                 " inner join articulos as a on a.empresa= s.empresa and a.id=s.fkarticulos" +
                                 " inner join familiasproductos as fp on fp.empresa= s.empresa and fp.id=substring(s.fkarticulos,0,3) {3}" +
                                 " inner join unidades as u on u.id= s.fkunidadesmedida" +
                                 " left join bundlelin as bl on bl.empresa= s.empresa and bl.lote= s.lote and bl.loteid=s.loteid and bl.fkalmacenes=s.fkalmacenes and bl.fkarticulos= s.fkarticulos " +
                                 " left join bundle as b on b.empresa= bl.empresa and b.id= bl.fkbundle" +
                                 " where s.empresa='{0}' and s.fkalmacenes='{1}' and s.lote='{4}' {2}", empresa, fkalmacen, CrearFiltrosArticulos(articulodesde, articulohasta, lotedesde, lotehasta,"",""), CrearFiltrosFamilias(familiadesde, familiahasta, solotablas),lote);
        }

        #endregion

        #region Busqueda articulos bundle lotes

        public IEnumerable<StockActualVistaModel> GetBundleArticulosLotes(string fkalmacen, string empresa)
        {
            return _db.Database.SqlQuery<StockActualVistaModel>(GetCadenaBusquedaLotes(empresa,fkalmacen)).ToList();
        }

        private string GetCadenaBusquedaLotes(string empresa,string fkalmacen)
        {
            return string.Format("select distinct  s.fkalmacenes as [Fkalmacenes], s.fkarticulos as [Fkarticulos],a.descripcionabreviada as [Descripcion], s.lote as [Lote]  from stockactual as s " +
                                 " inner join articulos as a on a.empresa= s.empresa and a.id=s.fkarticulos" +
                                 " inner join familiasproductos as fp on fp.empresa= s.empresa and fp.id=substring(s.fkarticulos,0,3) and fp.tipofamilia={2}" +
                                 " inner join unidades as u on u.id= s.fkunidadesmedida" +
                                 " left join bundlelin as bl on bl.empresa= s.empresa and bl.lote= s.lote and bl.loteid=s.loteid and bl.fkalmacenes=s.fkalmacenes and bl.fkarticulos= s.fkarticulos " +
                                 " where s.empresa='{0}' and s.fkalmacenes='{1}' order by  s.lote asc", empresa, fkalmacen,(int)TipoFamilia.Tabla);
        }

        #endregion

        #region Busqueda ArticulosLotes

        public IEnumerable<StockActualVistaModel> GetArticulosLotes(string fkalmacen, string empresa,string articulodesde,string articulohasta,string familiadesde,string familiahasta,string lotedesde,string lotehasta,bool solotablas,TipoCategoria flujo,string acabadodesde="",string acabadohasta="")
        {
            return _db.Database.SqlQuery<StockActualVistaModel>(GetCadenaBusquedaLotes(fkalmacen,empresa,articulodesde, articulohasta,familiadesde,familiahasta,  lotedesde,  lotehasta, solotablas,  flujo,acabadodesde,acabadohasta)).ToList();
        }

        private string GetCadenaBusquedaLotes(string fkalmacen, string empresa, string articulodesde, string articulohasta,
            string familiadesde, string familiahasta, string lotedesde, string lotehasta, bool solotablas, TipoCategoria flujo,string acabadodesde,string acabadohasta)
        {
            return string.Format("select s.*,s.cantidaddisponible as [Cantidad],u.decimalestotales as [Decimalesmedidas],a.descripcionabreviada as [Descripcion],fp.id as [Fkfamilias],concat(bl.fkbundlelote,bl.fkbundle) as [Bundle],REPLACE(STR(s.loteid, 3), SPACE(1), '0') as Loteidentificador   from stockactual as s " +
                                 " inner join articulos as a on a.empresa= s.empresa and a.id=s.fkarticulos and (a.categoria=0 or a.categoria="+ (int)flujo +")" +
                                 " inner join familiasproductos as fp on fp.empresa= s.empresa and fp.id=substring(s.fkarticulos,0,3) {3}" +
                                 " inner join unidades as u on u.id= s.fkunidadesmedida" +
                                 " left join bundlelin as bl on bl.empresa= s.empresa and bl.lote= s.lote and bl.loteid=s.loteid and bl.fkalmacenes=s.fkalmacenes and bl.fkarticulos= s.fkarticulos " +
                                 " where isnull(s.lote,'') <> '' and s.empresa='{0}' and s.fkalmacenes='{1}' {2} order by  s.lote asc,REPLACE(STR(s.loteid, 3), SPACE(1), '0') asc", empresa,fkalmacen,CrearFiltrosArticulos(articulodesde, articulohasta,lotedesde,lotehasta,acabadodesde,acabadohasta), CrearFiltrosFamilias(familiadesde, familiahasta,solotablas));
        }

        private string CrearFiltrosArticulos(string articulodesde, string articulohasta,string lotedesde,string lotehasta,string acabadodesde,string acabadohasta)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(articulodesde))
            {
                sb.AppendFormat(" AND s.fkarticulos>='{0}' ", articulodesde);
            }

            if (!string.IsNullOrEmpty(articulohasta))
            {
                sb.AppendFormat(" AND s.fkarticulos<='{0}' ", articulohasta);
            }

            if (!string.IsNullOrEmpty(acabadodesde))
            {
                sb.AppendFormat(" AND Substring(s.fkarticulos,10,2) >='{0}' ", acabadodesde);
            }

            if (!string.IsNullOrEmpty(acabadohasta))
            {
                sb.AppendFormat(" AND Substring(s.fkarticulos,10,2) <='{0}' ", acabadohasta);
            }

            if (!string.IsNullOrEmpty(lotedesde))
            {
                sb.AppendFormat(" AND s.lote>='{0}' ", lotedesde);
            }
            if (!string.IsNullOrEmpty(lotehasta))
            {
                sb.AppendFormat(" AND s.lote<='{0}' ", lotehasta);
            }
            return sb.ToString();
        }

        private string CrearFiltrosFamilias(string familiadesde, string familiahasta,bool solotablas)
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(familiadesde))
            {
                sb.AppendFormat(" AND fp.id >= '{0}' ", familiadesde);
            }

            if (!string.IsNullOrEmpty(familiahasta))
            {
                sb.AppendFormat(" AND fp.id <= '{0}' ", familiahasta);
            }

            if (solotablas)
            {
                sb.AppendFormat(" AND fp.tipofamilia={0} ", (int) TipoFamilia.Tabla);
            }

            return sb.ToString();
        }

        #endregion

        #region Busqueda lote para entregas

        public IEnumerable<IStockPieza> GetArticuloEntradaPorLoteOCodigo(string referencialote, string fkalmacen, string empresa)
        {
            IEnumerable<object> pieza = null;
            Kit kitobj = null;
            Bundle bundleobj = null;
           

            if (!string.IsNullOrEmpty(referencialote) && _db.Stockactual.Any(f =>
                f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                f.lote == referencialote))
            {
               pieza = _db.Stockactual.Where(f =>
                    f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                    f.lote == referencialote).GroupJoin(_db.BundleLin,f=>f.lote +f.loteid,g=>g.lote + g.loteid,(f,g)=>new {Pieza = f,Bundle =g}).ToList().OrderBy(f=>Funciones.Qint(f.Pieza.loteid));
            }
            else if (_db.Stockactual.Any(f =>
                 f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                 f.fkarticulos == referencialote))
            {
                pieza = _db.Stockactual.Where(f =>
                f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                f.fkarticulos == referencialote).GroupJoin(_db.BundleLin, f => f.lote + f.loteid, g => g.lote + g.loteid, (f, g) => new { Pieza = f, Bundle = g }).ToList().OrderBy(f => Funciones.Qint(f.Pieza.loteid));
            }
            else if (_db.Kit.Any(f => f.empresa == empresa && f.fkalmacen == fkalmacen && f.referencia == referencialote && (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado)))
            {
                kitobj = _db.Kit.Include("KitLin").Single(f => f.empresa == empresa && f.fkalmacen == fkalmacen && f.referencia == referencialote && f.empresa == empresa && (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado));
            }
            else if (_db.Bundle.Any(f => (f.lote + f.id) == referencialote && f.empresa == empresa))
            {

                bundleobj = _db.Bundle.Include("BundleLin").Single(f => f.empresa == empresa && f.fkalmacen == fkalmacen && (f.lote + f.id) == referencialote && f.empresa == empresa);
            }
            var articuloService = FService.Instance.GetService(typeof(ArticulosModel), _context) as ArticulosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context) as UnidadesService;
            var almacenesService = FService.Instance.GetService(typeof(AlmacenesModel), _context) as AlmacenesService;
            if (pieza != null)
            {
                
                var result = new List<SalidabusquedaentregaslotesarticulosModel>();
                foreach (dynamic elem in pieza)
                {
                    var elempieza = elem.Pieza as Stockactual;
                    var bundle = elem.Bundle as IEnumerable<BundleLin>;
                    var articuloObj = articuloService.GetArticulo(elempieza.fkarticulos);
                    var unidadesObj = unidadesService.get(elempieza.fkunidadesmedida) as UnidadesModel;
                    var item = new SalidabusquedaentregaslotesarticulosModel();
                    if (elempieza.fkalmaceneszona !=null)
                    {
                        var almacenModel = almacenesService.get(elempieza.fkalmacenes) as AlmacenesModel;

                        item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (elempieza.fkalmaceneszona ?? 0)).Descripcion;
                    }

                    item.Empresa = empresa;
                    item.Fecha = DateTime.Now;
                    item.Fkalmacenes = fkalmacen;
                    item.Fkarticulos = elempieza.fkarticulos;
                    item.Fkalmaceneszona = elempieza.fkalmaceneszona;
                    item.Referenciaproveedor = elempieza.referenciaproveedor;
                    item.Lote = elempieza.lote;
                    item.Loteid = elempieza.loteid;
                    item.Bundle = bundle.FirstOrDefault()?.fkbundle??string.Empty;
                    item.Tag = elempieza.tag;
                    item.Fkunidadesmedida = elempieza.fkunidadesmedida;
                    item.Cantidad = elempieza.cantidaddisponible;
                    item.Largo = elempieza.largo;
                    item.Ancho = elempieza.ancho;
                    item.Grueso = elempieza.grueso;
                    item.Metros = elempieza.metros??0;
                    item.Descripcion = articuloObj.Descripcion;
                    item.Decimalesmedidas = unidadesObj.Decimalestotales;
                    item.Documentomovimiento = "";
                    item.Tipooperacion = TipoOperacionStock.MovimientoStock;
                    item.Fkcalificacioncomercial = elempieza.fkcalificacioncomercial;
                    item.Fktipograno = elempieza.fktipograno;
                    item.Fktonomaterial = elempieza.fktonomaterial;
                    item.Fkincidenciasmaterial = elempieza.fkincidenciasmaterial;
                    item.Fkvariedades = elempieza.fkvariedades;
                    item.Lotefraccionable = articuloObj.Lotefraccionable;
                    result.Add(item);
                }
                return result;
                
                
            }
            else if (kitobj != null)
            {
                var list = kitobj.KitLin.Join(_db.Stockactual, f => f.lote + f.loteid, j => j.lote + j.loteid, (a, b) => new { Linea = a, Stock = b }).OrderBy(f => Funciones.Qint(f.Stock.loteid));

                var result = new List<SalidabusquedaentregaslotesarticulosModel>();
                foreach (var elem in list)
                {
                    var articuloObj = articuloService.GetArticulo(elem.Linea.fkarticulos);
                    var unidadesObj = unidadesService.get(elem.Linea.fkunidades) as UnidadesModel;
                    var item = new SalidabusquedaentregaslotesarticulosModel();
                    if (kitobj.fkzonalamacen.HasValue)
                    {
                        var almacenModel = almacenesService.get(kitobj.fkalmacen) as AlmacenesModel;

                        item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (kitobj.fkzonalamacen ?? 0)).Descripcion;
                    }

                    item.Empresa = empresa;
                    item.Fecha = DateTime.Now;
                    item.Fkalmacenes = fkalmacen;
                    item.Fkarticulos = elem.Linea.fkarticulos;
                    item.Fkalmaceneszona = kitobj.fkzonalamacen;
                    item.Referenciaproveedor = "";
                    item.Lote = elem.Linea.lote;
                    item.Loteid = elem.Linea.loteid;
                    item.Tag ="";
                    item.Fkunidadesmedida = elem.Linea.fkunidades;
                    item.Cantidad = elem.Linea.cantidad;
                    item.Largo = elem.Stock.largo;
                    item.Ancho = elem.Stock.ancho;
                    item.Grueso = elem.Stock.grueso;
                    item.Descripcion = articuloObj.Descripcion;
                    item.Decimalesmedidas = unidadesObj.Decimalestotales;
                    item.Documentomovimiento = "";
                    
                    result.Add(item);
                }
                return result;
            }
            else if (bundleobj != null)
            {
                var list = bundleobj.BundleLin.Join(_db.Stockactual,f=>f.lote+f.loteid,j=>j.lote+j.loteid,(a,b)=>new { Linea= a, Stock = b} ).OrderBy(f => Funciones.Qint(f.Stock.loteid));

                var result = new List<SalidabusquedaentregaslotesarticulosModel>();
                foreach (var elem in list)
                {
                    var articuloObj = articuloService.GetArticulo(elem.Linea.fkarticulos);
                    var unidadesObj = unidadesService.get(elem.Linea.fkunidades) as UnidadesModel;
                    var item = new SalidabusquedaentregaslotesarticulosModel();
                    if (bundleobj.fkzonaalmacen.HasValue)
                    {
                        var almacenModel = almacenesService.get(bundleobj.fkalmacen) as AlmacenesModel;

                        item.Fkzonaalmacedescripcion =
                            almacenModel.Lineas.FirstOrDefault(f => f.Id == (bundleobj.fkzonaalmacen ?? 0)).Descripcion;
                    }

                    item.Empresa = empresa;
                    item.Fecha = DateTime.Now;
                    item.Fkalmacenes = fkalmacen;
                    item.Fkarticulos = elem.Linea.fkarticulos;
                    item.Fkalmaceneszona = bundleobj.fkzonaalmacen;
                    item.Referenciaproveedor = "";
                    item.Lote = elem.Linea.lote;
                    item.Loteid = elem.Linea.loteid;
                    item.Bundle = elem.Linea.fkbundle;
                    item.Tag = "";
                    item.Fkunidadesmedida = elem.Linea.fkunidades;
                    item.Cantidad = elem.Linea.cantidad ?? 0;
                    item.Largo = elem.Stock.largo;
                    item.Ancho = elem.Stock.ancho ;
                    item.Grueso = elem.Stock.grueso;
                    item.Metros = elem.Stock.metros ?? 0;
                    item.Descripcion = articuloObj.Descripcion;
                    item.Decimalesmedidas = unidadesObj.Decimalestotales;
                    item.Documentomovimiento = "";

                    result.Add(item);
                }
                return result;
            }
            
            

            throw new ValidationException(string.Format("La pieza {0} no existe", referencialote));
        }


        //
        public IStockPiezaSingle GetArticuloPorLoteEntradaSingleOCodigo(string referencialote, string fkalmacen, string empresa)
        {
            Stockactual pieza = null;
            Kit kitobj = null;
            Bundle bundleobj = null;

            if (!string.IsNullOrEmpty(referencialote) && _db.Stockactual.Any(f =>
                f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                f.lote == referencialote))
            {
                pieza = _db.Stockactual.First(f =>
                    f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                    f.lote == referencialote);
            }
            else if (_db.Stockactual.Any(f =>
                 f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                 f.fkarticulos == referencialote))
            {
                pieza = _db.Stockactual.Single(f =>
                f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                f.fkarticulos == referencialote);
            }
            else if (_db.Kit.Any(f => f.empresa == empresa && f.fkalmacen == fkalmacen && f.referencia == referencialote && (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado)))
            {
                kitobj = _db.Kit.Include("KitLin").Single(f => f.empresa == empresa && f.fkalmacen == fkalmacen && f.referencia == referencialote && f.empresa == empresa && (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado));
            }
            else if (_db.Bundle.Any(f => (f.lote + f.id) == referencialote && f.empresa == empresa))
            {

                bundleobj = _db.Bundle.Include("BundleLin").Single(f => f.empresa == empresa && f.fkalmacen == fkalmacen && (f.lote + f.id) == referencialote && f.empresa == empresa);
            }
            var articuloService = FService.Instance.GetService(typeof(ArticulosModel), _context) as ArticulosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context) as UnidadesService;
            var almacenesService = FService.Instance.GetService(typeof(AlmacenesModel), _context) as AlmacenesService;
            if (pieza != null)
            {

                var articuloObj = articuloService.GetArticulo(pieza.fkarticulos);
                var unidadesObj = unidadesService.get(pieza.fkunidadesmedida) as UnidadesModel;
                var item = new StockPiezaSingle();
                if (pieza.fkalmaceneszona.HasValue)
                {
                    var almacenModel = almacenesService.get(pieza.fkalmacenes) as AlmacenesModel;

                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (pieza.fkalmaceneszona ?? 0)).Descripcion;
                }

                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = pieza.fkarticulos;
                item.Fkalmaceneszona = pieza.fkalmaceneszona;
                item.Referenciaproveedor = pieza.referenciaproveedor;
                item.Lote = pieza.lote;
                item.Loteid = pieza.loteid;
                item.Tag = pieza.tag;
                item.Fkunidadesmedida = pieza.fkunidadesmedida;
                item.Cantidad = pieza.cantidadtotal;
                item.Largo = pieza.largo;
                item.Ancho = pieza.ancho;
                item.Grueso = pieza.grueso;
                item.Descripcion = articuloObj.Descripcion;
                item.Decimalesmedidas = unidadesObj.Decimalestotales;
                item.Documentomovimiento = "";
                item.Tipooperacion = TipoOperacionStock.MovimientoStock;
                item.Fkcalificacioncomercial = pieza.fkcalificacioncomercial;
                item.Fktipograno = pieza.fktipograno;
                item.Fktonomaterial = pieza.fktonomaterial;
                item.Fkincidenciasmaterial = pieza.fkincidenciasmaterial;
                item.Fkvariedades = pieza.fkvariedades;
                item.Tipopieza=TipoPieza.Pieza;
                item.Lotefraccionable = articuloObj.Lotefraccionable;
                return item;
            }
            else if (kitobj != null)
            {
                var item = new StockPiezaSingle();
                if (kitobj.fkzonalamacen.HasValue)
                {
                    var almacenModel = almacenesService.get(kitobj.fkalmacen) as AlmacenesModel;

                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (kitobj.fkzonalamacen ?? 0)).Descripcion;
                }
                item.Lote = referencialote;
                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = kitobj.referencia;
                item.Fkalmaceneszona = kitobj.fkzonalamacen;
                item.Referenciaproveedor = string.Empty;
                item.Descripcion = kitobj.descripcion;
                item.Tipopieza = TipoPieza.Kit;
                return item;
            }
            else if (bundleobj != null)
            {
                var item = new StockPiezaSingle();
                if (bundleobj.fkzonaalmacen.HasValue)
                {
                    var almacenModel = almacenesService.get(bundleobj.fkalmacen) as AlmacenesModel;
                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (bundleobj.fkzonaalmacen ?? 0)).Descripcion;
                }
                item.Lote = referencialote;
                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = bundleobj.lote + bundleobj.id;
                item.Fkalmaceneszona = bundleobj.fkzonaalmacen;
                item.Referenciaproveedor = string.Empty;
                item.Descripcion = bundleobj.descripcion;
                item.Tipopieza = TipoPieza.Bundle;
                return item;
            }

            throw new ValidationException(string.Format("La pieza {0} no existe", referencialote));
        }


        //Aqui llega cuando pinchas en una fila de la lista de lotes disponibles. Llamado desde BusquedaArticulosConSinStockSingleLotesApiController
        public IStockPiezaSingle GetArticuloPorLoteEntradaSingleOCodigoHistorico(string referencialote, string fkalmacen, string empresa)
        {
            Stockhistorico pieza = null;
            Kit kitobj = null;
            Bundle bundleobj = null;

            if (!string.IsNullOrEmpty(referencialote) && _db.Stockhistorico.Any(f =>
                f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                f.lote == referencialote))
            {
                pieza = _db.Stockhistorico.First(f =>
                    f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                    f.lote == referencialote);
            }
            else if (_db.Stockhistorico.Any(f =>
                 f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                 f.fkarticulos == referencialote))
            {
                pieza = _db.Stockhistorico.Single(f =>
                f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                f.fkarticulos == referencialote);
            }
            else if (_db.Kit.Any(f => f.empresa == empresa && f.fkalmacen == fkalmacen && f.referencia == referencialote && (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado)))
            {
                kitobj = _db.Kit.Include("KitLin").Single(f => f.empresa == empresa && f.fkalmacen == fkalmacen && f.referencia == referencialote && f.empresa == empresa && (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado));
            }
            else if (_db.Bundle.Any(f => (f.lote + f.id) == referencialote && f.empresa == empresa))
            {

                bundleobj = _db.Bundle.Include("BundleLin").Single(f => f.empresa == empresa && f.fkalmacen == fkalmacen && (f.lote + f.id) == referencialote && f.empresa == empresa);
            }
            var articuloService = FService.Instance.GetService(typeof(ArticulosModel), _context) as ArticulosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context) as UnidadesService;
            var almacenesService = FService.Instance.GetService(typeof(AlmacenesModel), _context) as AlmacenesService;
            if (pieza != null)
            {

                var articuloObj = articuloService.GetArticulo(pieza.fkarticulos);
                var unidadesObj = unidadesService.get(pieza.fkunidadesmedida) as UnidadesModel;
                var item = new StockPiezaSingle();
                if (pieza.fkalmaceneszona.HasValue)
                {
                    var almacenModel = almacenesService.get(pieza.fkalmacenes) as AlmacenesModel;

                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (pieza.fkalmaceneszona ?? 0)).Descripcion;
                }

                //En el caso del lote en stock historico, queremos que los metros sean los metros de salida
                if(pieza.metros.Value==0)
                {
                    if(pieza.metrossalida == null)
                    {
                        item.Metros = 0;
                    }

                    else
                    {
                        item.Metros = pieza.metrossalida.Value;
                    }
                }

                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = pieza.fkarticulos;
                item.Fkalmaceneszona = pieza.fkalmaceneszona;
                item.Referenciaproveedor = pieza.referenciaproveedor;
                item.Lote = pieza.lote;
                item.Loteid = pieza.loteid;
                item.Tag = pieza.tag;
                item.Fkunidadesmedida = pieza.fkunidadesmedida;
                item.Cantidad = pieza.cantidadtotal;
                item.Largo = pieza.largo;
                item.Ancho = pieza.ancho;
                item.Grueso = pieza.grueso;
                item.Descripcion = articuloObj.Descripcion;
                item.Decimalesmedidas = unidadesObj.Decimalestotales;
                item.Documentomovimiento = "";
                item.Tipooperacion = TipoOperacionStock.MovimientoStock;
                item.Fkcalificacioncomercial = pieza.fkcalificacioncomercial;
                item.Fktipograno = pieza.fktipograno;
                item.Fktonomaterial = pieza.fktonomaterial;
                item.Fkincidenciasmaterial = pieza.fkincidenciasmaterial;
                item.Fkvariedades = pieza.fkvariedades;
                item.Tipopieza = TipoPieza.Pieza;
                item.Lotefraccionable = articuloObj.Lotefraccionable;
                return item;
            }
            else if (kitobj != null)
            {
                var item = new StockPiezaSingle();
                if (kitobj.fkzonalamacen.HasValue)
                {
                    var almacenModel = almacenesService.get(kitobj.fkalmacen) as AlmacenesModel;

                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (kitobj.fkzonalamacen ?? 0)).Descripcion;
                }
                item.Lote = referencialote;
                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = kitobj.referencia;
                item.Fkalmaceneszona = kitobj.fkzonalamacen;
                item.Referenciaproveedor = string.Empty;
                item.Descripcion = kitobj.descripcion;
                item.Tipopieza = TipoPieza.Kit;
                return item;
            }
            else if (bundleobj != null)
            {
                var item = new StockPiezaSingle();
                if (bundleobj.fkzonaalmacen.HasValue)
                {
                    var almacenModel = almacenesService.get(bundleobj.fkalmacen) as AlmacenesModel;
                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (bundleobj.fkzonaalmacen ?? 0)).Descripcion;
                }
                item.Lote = referencialote;
                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = bundleobj.lote + bundleobj.id;
                item.Fkalmaceneszona = bundleobj.fkzonaalmacen;
                item.Referenciaproveedor = string.Empty;
                item.Descripcion = bundleobj.descripcion;
                item.Tipopieza = TipoPieza.Bundle;
                return item;
            }

            throw new ValidationException(string.Format("La pieza {0} no existe", referencialote));
        }

        #endregion

        #region Busqueda de un lote

        public IStockPieza GetArticuloPorLoteOCodigo(string referencialote, string fkalmacen, string empresa)
        {
            Stockactual pieza = null;
            Kit kitobj = null;
            Bundle bundleobj = null;
            string lote;
            string loteid;

            CalcularPartesLote(referencialote, out lote, out loteid);

            if (!string.IsNullOrEmpty(lote) && _db.Stockactual.Any(f =>
                f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                f.lote == lote &&
                f.loteid == loteid))
            {
                pieza = _db.Stockactual.Single(f =>
                    f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                    f.lote == lote &&
                    f.loteid == loteid);
            }
            else if (_db.Stockactual.Any(f =>
                 f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                 f.fkarticulos == referencialote))
            {
                pieza = _db.Stockactual.Single(f =>
                f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                f.fkarticulos == referencialote);
            }
            else if (_db.Kit.Any(f => f.empresa == empresa && f.fkalmacen == fkalmacen && f.referencia == referencialote && (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado)))
            {
                kitobj = _db.Kit.Include("KitLin").Single(f => f.empresa == empresa && f.fkalmacen == fkalmacen && f.referencia == referencialote && f.empresa == empresa && (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado));
            }
            else if (_db.Bundle.Any(f => (f.lote + f.id) == referencialote && f.empresa == empresa))
            {

                bundleobj = _db.Bundle.Include("BundleLin").Single(f => f.empresa == empresa && f.fkalmacen == fkalmacen && (f.lote + f.id) == referencialote && f.empresa == empresa);
            }
            var articuloService = FService.Instance.GetService(typeof(ArticulosModel), _context) as ArticulosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context) as UnidadesService;
            var almacenesService = FService.Instance.GetService(typeof(AlmacenesModel), _context) as AlmacenesService;
            if (pieza != null)
            {

                var articuloObj = articuloService.GetArticulo(pieza.fkarticulos);
                var unidadesObj = unidadesService.get(pieza.fkunidadesmedida) as UnidadesModel;
                var item = new MovimientosstockModel();
                if (pieza.fkalmaceneszona.HasValue)
                {
                    var almacenModel = almacenesService.get(pieza.fkalmacenes) as AlmacenesModel;

                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (pieza.fkalmaceneszona ?? 0)).Descripcion;
                }

                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = pieza.fkarticulos;
                item.Fkalmaceneszona = pieza.fkalmaceneszona;
                item.Referenciaproveedor = pieza.referenciaproveedor;
                item.Lote = pieza.lote;
                item.Loteid = pieza.loteid;
                item.Tag = pieza.tag;
                item.Fkunidadesmedida = pieza.fkunidadesmedida;
                item.Cantidad = pieza.cantidadtotal;
                item.Largo = pieza.largo;
                item.Ancho = pieza.ancho;
                item.Grueso = pieza.grueso;
                item.Descripcion = articuloObj.Descripcion;
                item.Decimalesmedidas = unidadesObj.Decimalestotales;
                item.Documentomovimiento = "";
                item.Tipooperacion = TipoOperacionStock.MovimientoStock;
                item.Fkcalificacioncomercial = pieza.fkcalificacioncomercial;
                item.Fktipograno = pieza.fktipograno;
                item.Fktonomaterial = pieza.fktonomaterial;
                item.Fkincidenciasmaterial = pieza.fkincidenciasmaterial;
                item.Fkvariedades = pieza.fkvariedades;
                return item;
            }
            else if (kitobj != null)
            {
                var item = new KitStockActualModel();
                if (kitobj.fkzonalamacen.HasValue)
                {
                    var almacenModel = almacenesService.get(kitobj.fkalmacen) as AlmacenesModel;

                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (kitobj.fkzonalamacen ?? 0)).Descripcion;
                }

                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = kitobj.referencia;
                item.Fkalmaceneszona = kitobj.fkzonalamacen;
                item.Referenciaproveedor = string.Empty;
                item.Descripcion = kitobj.descripcion;

                return item;
            }
            else if (bundleobj != null)
            {
                var item = new BundleStockActualModel();
                if (bundleobj.fkzonaalmacen.HasValue)
                {
                    var almacenModel = almacenesService.get(bundleobj.fkalmacen) as AlmacenesModel;
                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (bundleobj.fkzonaalmacen ?? 0)).Descripcion;
                }

                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = bundleobj.lote + bundleobj.id;
                item.Fkalmaceneszona = bundleobj.fkzonaalmacen;
                item.Referenciaproveedor = string.Empty;
                item.Descripcion = bundleobj.descripcion;
                return item;
            }

            throw new ValidationException(string.Format("La pieza {0} no existe", referencialote));
        }

        public IStockPieza GetArticuloPorLoteOCodigoHistorico(string referencialote,string fkalmacen,string empresa)
        {
            Stockhistorico pieza = null;
            Kit kitobj = null;
            Bundle bundleobj = null;
            string lote;
            string loteid;

            CalcularPartesLote(referencialote, out lote, out loteid);

            if (!string.IsNullOrEmpty(lote) && _db.Stockhistorico.Any(f =>
                f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                f.lote == lote &&
                f.loteid == loteid))
            {
                pieza = _db.Stockhistorico.Single(f =>
                    f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                    f.lote == lote &&
                    f.loteid == loteid);
            }
            else if (_db.Stockhistorico.Any(f =>
                 f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                 f.fkarticulos == referencialote))
            {
                pieza = _db.Stockhistorico.Single(f =>
                f.empresa == empresa && f.fkalmacenes == fkalmacen &&
                f.fkarticulos == referencialote);
            }
            else if (_db.Kit.Any(f => f.empresa == empresa && f.fkalmacen == fkalmacen && f.referencia == referencialote && (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado)))
            {
                kitobj = _db.Kit.Include("KitLin").Single(f => f.empresa == empresa && f.fkalmacen == fkalmacen && f.referencia == referencialote && f.empresa == empresa && (f.estado == (int)EstadoKit.EnProceso || f.estado == (int)EstadoKit.Montado));
            }
            else if (_db.Bundle.Any(f => (f.lote + f.id) == referencialote && f.empresa == empresa))
            {
                
                bundleobj = _db.Bundle.Include("BundleLin").Single(f => f.empresa == empresa && f.fkalmacen == fkalmacen && (f.lote + f.id) == referencialote && f.empresa == empresa);
            }
            var articuloService = FService.Instance.GetService(typeof(ArticulosModel), _context) as ArticulosService;
            var unidadesService = FService.Instance.GetService(typeof(UnidadesModel), _context) as UnidadesService;
            var almacenesService = FService.Instance.GetService(typeof(AlmacenesModel), _context) as AlmacenesService;

            //Existe la pieza
            if (pieza != null)
            {
                
                var articuloObj = articuloService.GetArticulo(pieza.fkarticulos);
                var unidadesObj = unidadesService.get(pieza.fkunidadesmedida) as UnidadesModel;
                var item = new MovimientosstockModel();
                if (pieza.fkalmaceneszona.HasValue)
                {
                    var almacenModel = almacenesService.get(pieza.fkalmacenes) as AlmacenesModel;
                    
                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f=>f.Id == (pieza.fkalmaceneszona??0)).Descripcion;
                }

                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = pieza.fkarticulos;
                item.Fkalmaceneszona = pieza.fkalmaceneszona;
                item.Referenciaproveedor = pieza.referenciaproveedor;
                item.Lote = pieza.lote;
                item.Loteid = pieza.loteid;
                item.Tag = pieza.tag;
                item.Fkunidadesmedida = pieza.fkunidadesmedida;
                item.Cantidad = pieza.cantidadtotal;
                item.Largo = pieza.largo;
                item.Ancho = pieza.ancho;
                item.Grueso = pieza.grueso;
                item.Descripcion = articuloObj.Descripcion;
                item.Decimalesmedidas = unidadesObj.Decimalestotales;
                item.Documentomovimiento = "";
                item.Tipooperacion=TipoOperacionStock.MovimientoStock;
                item.Fkcalificacioncomercial = pieza.fkcalificacioncomercial;
                item.Fktipograno = pieza.fktipograno;
                item.Fktonomaterial = pieza.fktonomaterial;
                item.Fkincidenciasmaterial = pieza.fkincidenciasmaterial;
                item.Fkvariedades = pieza.fkvariedades;
                return item;
            }
            else if (kitobj != null)
            {
                var item = new KitStockActualModel();
                if (kitobj.fkzonalamacen.HasValue)
                {
                    var almacenModel = almacenesService.get(kitobj.fkalmacen) as AlmacenesModel;
                    
                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (kitobj.fkzonalamacen ?? 0)).Descripcion;
                }

                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = kitobj.referencia;
                item.Fkalmaceneszona = kitobj.fkzonalamacen;
                item.Referenciaproveedor = string.Empty;
                item.Descripcion = kitobj.descripcion;

                return item;
            }
            else if (bundleobj != null)
            {
                var item = new BundleStockActualModel();
                if (bundleobj.fkzonaalmacen.HasValue)
                {
                    var almacenModel = almacenesService.get(bundleobj.fkalmacen) as AlmacenesModel;
                    item.Fkzonaalmacedescripcion = almacenModel.Lineas.FirstOrDefault(f => f.Id == (bundleobj.fkzonaalmacen ?? 0)).Descripcion;
                }

                item.Empresa = empresa;
                item.Fecha = DateTime.Now;
                item.Fkalmacenes = fkalmacen;
                item.Fkarticulos = bundleobj.lote + bundleobj.id;
                item.Fkalmaceneszona = bundleobj.fkzonaalmacen;
                item.Referenciaproveedor = string.Empty;
                item.Descripcion = bundleobj.descripcion;
                return item;
            }

            throw new ValidationException(string.Format("La pieza {0} no existe",referencialote));
        }

        #endregion

        #endregion

        #region Busqueda articulos remedir

        public IEnumerable<StockActualRemedirModel> GetArticulosLotesRemedir(string fkalmacen, string empresa, string fkproveedores, string desderecepcionstock, string hastarecepcionstock, string fechadesde, string fechahasta, string lotedesde, string lotehasta)
        {
            return _db.Database.SqlQuery<StockActualRemedirModel>(GetCadenaBusquedaLotesRemedir(empresa, fkalmacen, fkproveedores, desderecepcionstock, hastarecepcionstock, fechadesde, fechahasta, lotedesde, lotehasta),
                new SqlParameter("empresa",empresa),
                new SqlParameter("almacenes",fkalmacen),
                new SqlParameter("fkproveedores", fkproveedores),
                new SqlParameter("desderecepcionstock", desderecepcionstock),
                new SqlParameter("hastarecepcionstock", hastarecepcionstock),
                new SqlParameter("fechadesde", fechadesde),
                new SqlParameter("fechahasta", fechahasta),
                new SqlParameter("lotedesde", lotedesde),
                new SqlParameter("lotehasta", lotehasta)).ToList();
        }

        private string GetCadenaBusquedaLotesRemedir(string empresa, string fkalmacen, string fkproveedores, string desderecepcionstock, string hastarecepcionstock, string fechadesde, string fechahasta, string lotedesde, string lotehasta)
        {
            return string.Format("select distinct  s.fkalmacenes as [Fkalmacenes], s.fkarticulos as [Fkarticulos],a.descripcionabreviada as [Descripcion], s.lote as [Lote],s.loteid as [Loteid],s.largo as [Largo],s.ancho as [Ancho],s.grueso as [Grueso], s.cantidaddisponible as [Cantidaddisponible],s.metros as [Metros],u.codigounidad as [UM],bl.fkbundle as [Bundle], u.decimalestotales as [_decimales], cm.descripcion as [CC],tm.descripcion as [Tono],tg.descripcion as [Grano],az.descripcion as [Zona],s.referenciaproveedor as [Loteproveedor],a.Kilosud as [Kilosum] from stockactual as s " +
                                 " inner join articulos as a on a.empresa= s.empresa and a.id=s.fkarticulos" +
                                 " inner join unidades as u on u.id= s.fkunidadesmedida" +
                                 GenerarFiltrosProveedoresYAlbaranes(fkproveedores,desderecepcionstock,hastarecepcionstock,fechadesde,fechahasta) + 
                                 " left join bundlelin as bl on bl.empresa= s.empresa and bl.lote= s.lote and bl.loteid=s.loteid and bl.fkalmacenes=s.fkalmacenes and bl.fkarticulos= s.fkarticulos " +
                                 " left join calificacioncomercial as cm on cm.valor= s.fkcalificacioncomercial " +
                                 " left join tonomaterial as tm on tm.valor= s.fktonomaterial " +
                                 " left join tipograno as tg on tg.valor= s.fktipograno " +
                                 " left join almaceneszona as az on az.empresa=s.empresa and az.fkalmacenes=s.fkalmacenes and az.id=s.fkalmaceneszona " +
                                 " where s.empresa=@empresa and s.fkalmacenes=@almacenes and isnull(s.lote,'')<>'' "+ GenerarFiltrosLotes(lotedesde,lotehasta) + " order by  s.lote asc");
        }

        private string GenerarFiltrosLotes(string lotedesde, string lotehasta)
        {
            var sb=new StringBuilder();

            if (!string.IsNullOrEmpty(lotedesde))
            {
                sb.AppendFormat(" AND s.lote >= @lotedesde ");
            }

            if (!string.IsNullOrEmpty(lotehasta))
            {
                sb.AppendFormat(" AND s.lote <= @lotehasta ");
            }

            return sb.ToString();
        }

        private string GenerarFiltrosProveedoresYAlbaranes(string fkproveedores, string desderecepcionstock,string hastarecepcionstock,string fechadesde,string fechahasta)
        {
            if (!string.IsNullOrEmpty(fkproveedores) || !string.IsNullOrEmpty(desderecepcionstock) ||
                !string.IsNullOrEmpty(hastarecepcionstock) || !string.IsNullOrEmpty(fechadesde) ||
                !string.IsNullOrEmpty(fechahasta))
            {
                var sb=new StringBuilder();
                sb.Append(" inner join albaranescompras as ac on ac.empresa=s.empresa ");
                if (!string.IsNullOrEmpty(fkproveedores))
                {
                    sb.Append(" AND ac.fkproveedores=@fkproveedores ");
                }

                if (!string.IsNullOrEmpty(desderecepcionstock))
                {
                    sb.Append(" AND ac.referencia>=@desderecepcionstock ");
                }
                if (!string.IsNullOrEmpty(hastarecepcionstock))
                {
                    sb.Append(" AND ac.referencia<=@hastarecepcionstock ");
                }
                if (!string.IsNullOrEmpty(fechadesde))
                {
                    sb.Append(" AND ac.fechadocumento<=@fechadesde ");
                }
                if (!string.IsNullOrEmpty(fechahasta))
                {
                    sb.Append(" AND ac.fechadocumento<=@fechahasta ");
                }
                sb.Append(" inner join albaranescompraslin as acl on acl.empresa=ac.empresa and acl.fkalbaranes=ac.id and acl.lote=s.lote and convert(varchar(12),isnull(acl.tabla,''))=s.loteid ");
                return sb.ToString();
            }
            return string.Empty;
        }

        #endregion


        #region consulta visual
        public List<String> getFamiliasByAlmacen(string idAlmacen)
        {
            var familiaArticulos = _db.Stockactual.Where(f => f.fkalmacenes == idAlmacen).Select(f => f.fkarticulos.Substring(0,3)).Distinct().ToList();

            if(familiaArticulos == null)
            {
                familiaArticulos = new List<String>();
            }

            return familiaArticulos;
        }
        #endregion
    }
}
