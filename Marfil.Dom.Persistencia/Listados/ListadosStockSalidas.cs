using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Inf.ResourcesGlobalization.Textos.MenuAplicacion;
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RProveedor = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosStockSalidas : ListadosModel
    {
        private List<string> _series = new List<string>();
        private string _agrupacion ="0";
        private string _detallepor ="0";

        #region Properties

        public override string TituloListado => Menuaplicacion.listadosstocksalidas;

        public override string IdListado
        {
            get
            {
                var resultado = string.Empty;
                switch (Agrupacion)
                {
                    case "0":
                        resultado = FListadosModel.StockGeneral + Detallepor;
                        break;
                    case "1":
                        resultado = FListadosModel.StockAgrupadoArticulo + Detallepor;
                        break;
                    case "2":
                        resultado = FListadosModel.StockAgrupadoArticuloLote + Detallepor;
                        break;
                    case "3":
                        resultado = FListadosModel.StockAgrupadoArticuloLoteMedidas + Detallepor;
                        break;
                    case "4":
                        resultado = FListadosModel.StockAgrupadoArticuloMedidas + Detallepor;
                        break;
                }

                return resultado;
            }
        }

        

        public override string WebIdListado => FListadosModel.StockSalidas;

        public string Order { get; set; }


        public string Detallepor
        {
            get { return _detallepor; }
            set { _detallepor = value; }
        }

        public string Agrupacion
        {
            get { return _agrupacion; }
            set { _agrupacion = value; }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkzonas")]
        public string Fkzonasalmacen { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FkarticulosDesde")]
        public string FkarticulosDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FkarticulosHasta")]
        public string FkarticulosHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ProveedorDesde")]
        public string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ProveedorHasta")]
        public string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        // Listado de lotes
        [Display(ResourceType = typeof(RAlbaranes), Name = "LoteDesde")]
        public string LoteDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "LoteHasta")]
        public string LoteHasta { get; set; }


        #endregion

        public ListadosStockSalidas()
        {
            
        }

        public ListadosStockSalidas(IContextService context):base(context)
        {
            ConfiguracionColumnas.Add("L", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() {Columna = "_decimales"} });
            ConfiguracionColumnas.Add("A", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("G", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            Agrupacion = "0";

        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            // Condición para coger el stockhistorico que no está en stockactual
            sb.AppendFormat("s.lote is NULL AND");
            sb.AppendFormat(" h.empresa='{0}' ", Empresa);
            if (!string.IsNullOrEmpty(Fkalmacen))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkalmacen", Fkalmacen);
                sb.Append(" h.fkalmacenes = @fkalmacen  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Fkalmacen, Fkalmacen));
                flag = true;
            }


            if (!string.IsNullOrEmpty(Fkzonasalmacen))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkzonasalmacen", Fkzonasalmacen);
                sb.Append(" h.fkalmaceneszona = @fkzonasalmacen  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Fkzonas, Fkzonasalmacen));
                flag = true;
            }

            if (Tipodealmacenlote.HasValue)
            {
                int index = Array.IndexOf(Enum.GetValues(Tipodealmacenlote?.GetType()), Tipodealmacenlote);
                string Tipoalmacenlote = index.ToString();
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("tipoalmacenlote", Tipoalmacenlote);
                sb.Append(" h.tipoalmacenlote = @tipoalmacenlote  ");

                switch (Tipodealmacenlote)
                {
                    case TipoAlmacenlote.Mercaderia:
                        {
                            Tipoalmacenlote = RAlbaranesCompras.TipoAlmacenLoteMercaderia;
                            break;
                        }
                    case TipoAlmacenlote.Gestionado:
                        {
                            Tipoalmacenlote = RAlbaranesCompras.TipoAlmacenLoteGestionado;
                            break;
                        }
                    case TipoAlmacenlote.Propio:
                    {
                            Tipoalmacenlote=RAlbaranesCompras.TipoAlmacenLotePropio;
                            break;
                    }
                }
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranesCompras.TipoAlmacenLote, Tipoalmacenlote));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticulosdesde", FkarticulosDesde);
                sb.Append(" h.fkarticulos >= @fkarticulosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosDesde, FkarticulosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticuloshasta", FkarticulosHasta);
                sb.Append(" h.fkarticulos <= @fkarticuloshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosHasta, FkarticulosHasta));
                flag = true;
            }

            if (FechaDesde.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechadesde", FechaDesde.Value);
                sb.Append(" alb.fechadocumento>=@fechadesde ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FechaDesde, FechaDesde));
                flag = true;
            }

            if (FechaHasta.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechahasta", FechaHasta.Value);
                sb.Append(" alb.fechadocumento<=@fechahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FechaHasta, FechaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentadesde", CuentaDesde);
                sb.Append(" comp.fkproveedores>=@cuentadesde ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaDesde, CuentaDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentahasta", CuentaHasta);
                sb.Append(" comp.fkproveedores<=@cuentahasta ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaHasta, CuentaHasta));
                flag = true;
            }

            return sb.ToString();
        }

        internal override string GenerarSelect()
        { 
            if (Agrupacion == "0")
            {
                return CadenaSinagrupacion();
            }
            else if (Agrupacion == "1")
            {
                return CadenaAgrupadoArticulos();
            }
            else if (Agrupacion == "2")
            {
                return CadenaAgrupadoArticulosLote();
            }
            else if (Agrupacion == "3")
            {
                return CadenaAgrupadoArticulosLoteMedidas();
            }
            else if (Agrupacion == "4")
            {
                return CadenaAgrupadoArticulosMedidas();
            }

            return CadenaSinagrupacion();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            if (Agrupacion == "0")
            {
                result = CadenaAgrupacionSinagrupacion();
            }
            else if (Agrupacion == "1")
            {
                result = CadenaAgrupacionAgrupadoArticulos();
            }
            else if (Agrupacion == "2")
            {
                result = CadenaAgrupacionAgrupadoArticulosLote();
            }
            else if (Agrupacion == "3")
            {
                result = CadenaAgrupacionAgrupadoArticulosLoteMedidas();
            }
            else if (Agrupacion == "4")
            {
                result = CadenaAgrupacionAgrupadoArticulosMedidas();
            }

            switch (Order)
            {
                case "1":
                    result += " order by h.fkarticulos ";
                    break;
            }



            return result;
        }

        public IEnumerable<SeriesModel> SeriesListado
        {
            get
            {
                
                var service = FService.Instance.GetService(typeof (SeriesModel), Context) as SeriesService;
                return service.GetSeriesTipoDocumento(TipoDocumento.AlbaranesVentas);
            }
        }

        #region Helper

        #region Cadena Select

        private string GetColumnasAlmacen()
        {
            switch (_detallepor)
            {
                case "0":
                    return string.Empty;
                case "1":
                    return " h.fkalmacenes as [Cod. Almacén], alm.descripcion as [Almacén], ";
                case "2":
                    return " h.fkalmacenes as [Cod. Almacén], alm.descripcion as [Almacén], almz.descripcion as [Zona], ";
            }

            return string.Empty;
        }

        private string GetColumnasAgrupacionAlmacen()
        {
            switch (_detallepor)
            {
                case "0":
                    return string.Empty;
                case "1":
                    return " ,h.fkalmacenes, alm.descripcion ";
                case "2":
                    return ", h.fkalmacenes, alm.descripcion, almz.descripcion ";
            }

            return string.Empty;
        }
        private string CadenaSinagrupacion()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT  {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" comp.fkproveedores as [Proveedor], alb.fechadocumento as [Fecha de Salida],");
            sb.AppendFormat(" h.fkarticulos as [Cod. Artículo], a.descripcion as [Artículo], h.lote as [Lote],");
            sb.AppendFormat(" h.loteid [Tabla],");
            sb.AppendFormat(" dbo._fn_enum_TipoAlmacenlote(h.tipoalmacenlote) as [Tipo de Lote],");
            sb.AppendFormat(" h.largo as [L], h.ancho as [A], h.grueso as [G], h.metros as [Metros]");            
            sb.AppendFormat(" FROM Stockhistorico h");
            sb.AppendFormat(" LEFT JOIN Articulos AS a ON a.empresa = h.empresa AND a.id = h.fkarticulos");
            sb.AppendFormat(" LEFT JOIN AlbaranesLin AS salida ON salida.empresa = h.empresa AND salida.lote = h.lote AND salida.tabla = h.loteid");
            sb.AppendFormat(" LEFT JOIN Albaranes AS alb ON alb.empresa = h.empresa AND salida.fkalbaranes = alb.id");
            sb.AppendFormat(" LEFT JOIN AlbaranesComprasLin AS entrada ON entrada.empresa = h.empresa AND entrada.lote = h.lote AND entrada.tabla = h.loteid");
            sb.AppendFormat(" LEFT JOIN AlbaranesCompras AS comp ON comp.empresa = h.empresa AND entrada.fkalbaranes = comp.id");
            sb.AppendFormat(" LEFT JOIN Transformacionessalidalin AS ts ON ts.empresa = h.empresa AND ts.lote = h.lote AND ts.tabla = h.loteid");

            if (_detallepor == "1" || _detallepor == "2")
            {
                sb.AppendFormat(" inner join almacenes as alm on alm.empresa=h.empresa and alm.id=h.fkalmacenes ");
                sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=h.empresa and almz.fkalmacenes=h.fkalmacenes and almz.id=h.fkalmaceneszona ");
            }

            sb.AppendFormat(" LEFT JOIN Stockactual AS s ON s.empresa = h.empresa AND s.lote = h.lote ");//WHERE s.lote is NULL");       

            return sb.ToString();
        }

        private string CadenaAgrupadoArticulos()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT  {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" comp.fkproveedores as [Proveedor], alb.fechadocumento as [Fecha salida],");
            sb.AppendFormat(" h.fkarticulos as [Cod.Artículo], a.descripcion as [Artículo],");
            sb.AppendFormat(" dbo._fn_enum_TipoAlmacenlote(h.tipoalmacenlote) as [Tipo de Lote],");
            sb.AppendFormat(" COUNT (h.fkarticulos) as [Cantidad], SUM (h.metros) as [Metros]");
            sb.AppendFormat(" FROM Stockhistorico h");
            sb.AppendFormat(" LEFT JOIN Articulos AS a ON a.empresa = h.empresa AND a.id = h.fkarticulos");
            sb.AppendFormat(" LEFT JOIN AlbaranesLin AS salida ON salida.empresa = h.empresa AND salida.lote = h.lote AND salida.tabla = h.loteid");
            sb.AppendFormat(" LEFT JOIN Albaranes AS alb ON alb.empresa = h.empresa AND salida.fkalbaranes = alb.id");
            sb.AppendFormat(" LEFT JOIN AlbaranesComprasLin AS entrada ON entrada.empresa = h.empresa AND entrada.lote = h.lote AND entrada.tabla = h.loteid");
            sb.AppendFormat(" LEFT JOIN AlbaranesCompras AS comp ON comp.empresa = h.empresa AND entrada.fkalbaranes = comp.id");
            sb.AppendFormat(" LEFT JOIN Transformacionessalidalin AS ts ON ts.empresa = h.empresa AND ts.lote = h.lote AND ts.tabla = h.loteid");

            if (_detallepor == "1" || _detallepor == "2")
            {
                sb.AppendFormat(" inner join almacenes as alm on alm.empresa=h.empresa and alm.id=h.fkalmacenes ");
                sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=h.empresa and almz.fkalmacenes=h.fkalmacenes and almz.id=h.fkalmaceneszona ");
            }

            sb.AppendFormat(" LEFT JOIN Stockactual AS s ON s.empresa = h.empresa AND s.lote = h.lote ");//WHERE s.lote is NULL");                   

            return sb.ToString();
        }

        private string CadenaAgrupadoArticulosLote()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT  {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" comp.fkproveedores as [Proveedor], alb.fechadocumento as [Fecha salida],");
            sb.AppendFormat(" h.fkarticulos as [Cod.Artículo], a.descripcion as [Artículo], h.lote as [Lote],");
            sb.AppendFormat(" dbo._fn_enum_TipoAlmacenlote(h.tipoalmacenlote) as [Tipo de Lote],");
            sb.AppendFormat(" COUNT (h.lote) as [Cantidad], SUM (h.metros) as [Metros]");
            sb.AppendFormat(" FROM Stockhistorico h");
            sb.AppendFormat(" LEFT JOIN Articulos AS a ON a.empresa = h.empresa AND a.id = h.fkarticulos");
            sb.AppendFormat(" LEFT JOIN AlbaranesLin AS salida ON salida.empresa = h.empresa AND salida.lote = h.lote AND salida.tabla = h.loteid");
            sb.AppendFormat(" LEFT JOIN Albaranes AS alb ON alb.empresa = h.empresa AND salida.fkalbaranes = alb.id");
            sb.AppendFormat(" LEFT JOIN AlbaranesComprasLin AS entrada ON entrada.empresa = h.empresa AND entrada.lote = h.lote AND entrada.tabla = h.loteid");
            sb.AppendFormat(" LEFT JOIN AlbaranesCompras AS comp ON comp.empresa = h.empresa AND entrada.fkalbaranes = comp.id");
            sb.AppendFormat(" LEFT JOIN Transformacionessalidalin AS ts ON ts.empresa = h.empresa AND ts.lote = h.lote AND ts.tabla = h.loteid");

            if (_detallepor == "1" || _detallepor == "2")
            {
                sb.AppendFormat(" inner join almacenes as alm on alm.empresa=h.empresa and alm.id=h.fkalmacenes ");
                sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=h.empresa and almz.fkalmacenes=h.fkalmacenes and almz.id=h.fkalmaceneszona ");
            }

            sb.AppendFormat(" LEFT JOIN Stockactual AS s ON s.empresa = h.empresa AND s.lote = h.lote ");//WHERE s.lote is NULL");                   

            return sb.ToString();
        }

        private string CadenaAgrupadoArticulosLoteMedidas()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT  {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" comp.fkproveedores as [Proveedor], alb.fechadocumento as [Fecha salida],");
            sb.AppendFormat(" h.fkarticulos as [Cod.Artículo], a.descripcion as [Artículo], h.lote as [Lote],");
            sb.AppendFormat(" dbo._fn_enum_TipoAlmacenlote(h.tipoalmacenlote) as [Tipo de Lote],");
            sb.AppendFormat(" COUNT(h.lote) as [Cantidad],");
            sb.AppendFormat(" h.largo as [L], h.ancho as [A], h.grueso as [G],");
            sb.AppendFormat(" SUM (h.metros) as [Metros]");
            sb.AppendFormat(" FROM Stockhistorico h");
            sb.AppendFormat(" LEFT JOIN Articulos AS a ON a.empresa = h.empresa AND a.id = h.fkarticulos");
            sb.AppendFormat(" LEFT JOIN AlbaranesLin AS salida ON salida.empresa = h.empresa AND salida.lote = h.lote AND salida.tabla = h.loteid");
            sb.AppendFormat(" LEFT JOIN Albaranes AS alb ON alb.empresa = h.empresa AND salida.fkalbaranes = alb.id");
            sb.AppendFormat(" LEFT JOIN AlbaranesComprasLin AS entrada ON entrada.empresa = h.empresa AND entrada.lote = h.lote AND entrada.tabla = h.loteid");
            sb.AppendFormat(" LEFT JOIN AlbaranesCompras AS comp ON comp.empresa = h.empresa AND entrada.fkalbaranes = comp.id");
            sb.AppendFormat(" LEFT JOIN Transformacionessalidalin AS ts ON ts.empresa = h.empresa AND ts.lote = h.lote AND ts.tabla = h.loteid");

            if (_detallepor == "1" || _detallepor == "2")
            {
                sb.AppendFormat(" inner join almacenes as alm on alm.empresa=h.empresa and alm.id=h.fkalmacenes ");
                sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=h.empresa and almz.fkalmacenes=h.fkalmacenes and almz.id=h.fkalmaceneszona ");
            }

            sb.AppendFormat(" LEFT JOIN Stockactual AS s ON s.empresa = h.empresa AND s.lote = h.lote ");//WHERE s.lote is NULL");                   

            return sb.ToString();
        }

        public string CadenaAgrupadoArticulosMedidas()
        {
            var sb = new StringBuilder();
            sb.AppendFormat(" SELECT  {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" comp.fkproveedores as [Proveedor], alb.fechadocumento as [Fecha salida],");
            sb.AppendFormat(" h.fkarticulos as [Cod.Artículo], a.descripcion as [Artículo],");
            sb.AppendFormat(" dbo._fn_enum_TipoAlmacenlote(h.tipoalmacenlote) as [Tipo de Lote],");
            sb.AppendFormat(" COUNT (h.fkarticulos) as [Cantidad],");
            sb.AppendFormat(" h.largo as [L], h.ancho as [A], h.grueso as [G],");
            sb.AppendFormat(" SUM (h.metros) as [Metros]");
            sb.AppendFormat(" FROM Stockhistorico h");
            sb.AppendFormat(" LEFT JOIN Articulos AS a ON a.empresa = h.empresa AND a.id = h.fkarticulos");
            sb.AppendFormat(" LEFT JOIN AlbaranesLin AS salida ON salida.empresa = h.empresa AND salida.lote = h.lote AND salida.tabla = h.loteid");
            sb.AppendFormat(" LEFT JOIN Albaranes AS alb ON alb.empresa = h.empresa AND salida.fkalbaranes = alb.id");
            sb.AppendFormat(" LEFT JOIN AlbaranesComprasLin AS entrada ON entrada.empresa = h.empresa AND entrada.lote = h.lote AND entrada.tabla = h.loteid");
            sb.AppendFormat(" LEFT JOIN AlbaranesCompras AS comp ON comp.empresa = h.empresa AND entrada.fkalbaranes = comp.id");
            sb.AppendFormat(" LEFT JOIN Transformacionessalidalin AS ts ON ts.empresa = h.empresa AND ts.lote = h.lote AND ts.tabla = h.loteid");

            if (_detallepor == "1" || _detallepor == "2")
            {
                sb.AppendFormat(" inner join almacenes as alm on alm.empresa=h.empresa and alm.id=h.fkalmacenes ");
                sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=h.empresa and almz.fkalmacenes=h.fkalmacenes and almz.id=h.fkalmaceneszona ");
            }

            sb.AppendFormat(" LEFT JOIN Stockactual AS s ON s.empresa = h.empresa AND s.lote = h.lote ");//WHERE s.lote is NULL");                   

            return sb.ToString();
        }

        #endregion

        #region Cadena agrupacion

        public string CadenaAgrupacionSinagrupacion()
        {
            return " GROUP BY " 
                    + " h.fkarticulos, a.descripcion, h.lote, dbo._fn_enum_TipoAlmacenlote(h.tipoalmacenlote),"
                    + " h.loteid, h.largo, h.ancho, h.grueso, h.metros, alb.fechadocumento, comp.fkproveedores"
                    + GetColumnasAgrupacionAlmacen();
        }

        public string CadenaAgrupacionAgrupadoArticulos()
        {
            return " GROUP BY h.fkarticulos, a.descripcion, dbo._fn_enum_TipoAlmacenlote(h.tipoalmacenlote)," 
                    + " alb.fechadocumento, comp.fkproveedores" + GetColumnasAgrupacionAlmacen();
        }

        public string CadenaAgrupacionAgrupadoArticulosLote()
        {
            return " GROUP BY h.fkarticulos, a.descripcion, h.lote, dbo._fn_enum_TipoAlmacenlote(h.tipoalmacenlote),"
                    + " alb.fechadocumento, comp.fkproveedores" + GetColumnasAgrupacionAlmacen();
        }

        public string CadenaAgrupacionAgrupadoArticulosLoteMedidas()
        {
            return " GROUP BY h.fkarticulos, a.descripcion, h.lote, dbo._fn_enum_TipoAlmacenlote(h.tipoalmacenlote),"
                    + " h.largo, h.ancho, h.grueso, h.metros, alb.fechadocumento, comp.fkproveedores"
                    + GetColumnasAgrupacionAlmacen();
        }

        public string CadenaAgrupacionAgrupadoArticulosMedidas()
        {
            return " GROUP BY h.fkarticulos, a.descripcion, dbo._fn_enum_TipoAlmacenlote(h.tipoalmacenlote),"
                    + " h.largo, h.ancho, h.grueso, h.metros, alb.fechadocumento, comp.fkproveedores"
                    + GetColumnasAgrupacionAlmacen();

        }

        #endregion

        #endregion
    }
}
