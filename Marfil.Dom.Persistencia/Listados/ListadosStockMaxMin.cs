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
    public class ListadosStockMaxMin : ListadosModel
    {
        private List<string> _series = new List<string>();
        /*private string _agrupacion = "0";*/
        private string _mostrar = "0";

        #region Properties

        public override string TituloListado => Menuaplicacion.listadostockmaxmin;

        public override string IdListado
        {
            get
            {
                var resultado = FListadosModel.StockMaxMin ;
                
                return resultado;
            }
        }



        public override string WebIdListado => FListadosModel.StockMaxMin;

        public string Order { get; set; }


        public string Mostrar
        {
            get { return _mostrar; }
            set { _mostrar = value; }
        }

        /*public string Agrupacion
        {
            get { return _agrupacion; }
            set { _agrupacion = value; }
        }*/

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkzonas")]
        public string Fkzonasalmacen { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FkarticulosDesde")]
        public string FkarticulosDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FkarticulosHasta")]
        public string FkarticulosHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FamiliaMateriales")]
        public string Fkfamiliasmateriales { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FamiliaDesde")]
        public string FkfamiliasDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FamiliaHasta")]
        public string FkfamiliasHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "MaterialesDesde")]
        public string FkmaterialesDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "MaterialesHasta")]
        public string FkmaterialesHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "CaracteristicasDesde")]
        public string FkcaracteristicasDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "CaracteristicasHasta")]
        public string FkcaracteristicasHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "GrosoresDesde")]
        public string FkgrosoresDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "GrosoresHasta")]
        public string FkgrosoresHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "AcabadosDesde")]
        public string FkacabadosDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "AcabadosHasta")]
        public string FkacabadosHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }


        // Listado de lotes
        [Display(ResourceType = typeof(RAlbaranes), Name = "LoteDesde")]
        public string LoteDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "LoteHasta")]
        public string LoteHasta { get; set; }


        #endregion

        public ListadosStockMaxMin()
        {

        }

        public ListadosStockMaxMin(IContextService context) : base(context)
        {
            ConfiguracionColumnas.Add("L", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("A", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("G", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            //Agrupacion = "0";

        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.AppendFormat(" s.empresa='{0}' ", Empresa);
            if (!string.IsNullOrEmpty(Fkalmacen))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkalmacen", Fkalmacen);
                sb.Append(" s.fkalmacenes = @fkalmacen  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Fkalmacen, Fkalmacen));
                flag = true;
            }


            if (!string.IsNullOrEmpty(Fkzonasalmacen))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkzonasalmacen", Fkzonasalmacen);
                sb.Append(" s.fkalmaceneszona = @fkzonasalmacen  ");
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
                sb.Append(" s.tipoalmacenlote = @tipoalmacenlote  ");

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
                            Tipoalmacenlote = RAlbaranesCompras.TipoAlmacenLotePropio;
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
                sb.Append(" s.fkarticulos >= @fkarticulosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosDesde, FkarticulosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticuloshasta", FkarticulosHasta);
                sb.Append(" s.fkarticulos <= @fkarticuloshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosHasta, FkarticulosHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkfamiliasmateriales))
            {
                if (flag)
                    sb.Append(" AND ");
                AppService = new ApplicationHelper(Context);
                ValoresParametros.Add("fkfamiliasmateriales", Fkfamiliasmateriales);
                sb.Append("  exists(select mm.* from materiales as mm where mm.id=Substring(s.fkarticulos,3,3) and mm.fkfamiliamateriales=@fkfamiliasmateriales)  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaMateriales, AppService.GetListFamiliaMateriales().SingleOrDefault(f => f.Valor == Fkfamiliasmateriales).Descripcion));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliasdesde", FkfamiliasDesde);
                sb.Append(" Substring(s.fkarticulos,0,3) >= @fkfamiliasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaDesde, FkfamiliasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliashasta", FkfamiliasHasta);
                sb.Append(" Substring(s.fkarticulos,0,3) <= @fkfamiliashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaHasta, FkfamiliasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialesdesde", FkmaterialesDesde);
                sb.Append(" Substring(s.fkarticulos,3,3) >= @fkmaterialesdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesDesde, FkmaterialesDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialeshasta", FkmaterialesHasta);
                sb.Append(" Substring(s.fkarticulos,3,3) <= @fkmaterialeshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesHasta, FkmaterialesHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicasdesde", FkcaracteristicasDesde);
                sb.Append(" Substring(s.fkarticulos,6,2) >= @fkcaracteristicasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.CaracteristicasDesde, FkcaracteristicasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicashasta", FkcaracteristicasHasta);
                sb.Append(" Substring(s.fkarticulos,6,2) <= @fkcaracteristicashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.CaracteristicasHasta, FkcaracteristicasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoresdesde", FkgrosoresDesde);
                sb.Append(" Substring(s.fkarticulos,8,2) >= @fkgrosoresdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.GrosoresDesde, FkgrosoresDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoreshasta", FkgrosoresHasta);
                sb.Append(" Substring(s.fkarticulos,8,2) <= @fkgrosoreshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.GrosoresHasta, FkgrosoresHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadosdesde", FkacabadosDesde);
                sb.Append(" Substring(s.fkarticulos,10,2) >= @fkacabadosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.AcabadosDesde, FkacabadosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadoshasta", FkacabadosHasta);
                sb.Append(" Substring(s.fkarticulos,10,2) <= @fkacabadoshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.AcabadosHasta, FkacabadosHasta));
                flag = true;
            }         

            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            return CadenaAgrupadoArticulos();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            result = CadenaAgrupacionAgrupadoArticulos();
            result += " order by s.fkarticulos ";

            return result;
        }

        public IEnumerable<SeriesModel> SeriesListado
        {
            get
            {

                var service = FService.Instance.GetService(typeof(SeriesModel), Context) as SeriesService;
                return service.GetSeriesTipoDocumento(TipoDocumento.AlbaranesVentas);
            }
        }

        #region Helper

        #region Cadena Select

        private string GetColumnasAlmacen()
        {
            return " s.fkalmacenes as [Cod. Almacén],alm.descripcion as [Almacén], ";
        }

        private string GetColumnasAgrupacionAlmacen()
        {            
            return " ,s.fkalmacenes,alm.descripcion ";
        }

        private string CadenaAgrupadoArticulos()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("select {0} ", GetColumnasAlmacen());
            sb.AppendFormat(" s.fkarticulos as [Cod. Artículo],a.descripcion as [Artículo],");
            //columnas sum
            sb.AppendFormat("sum( s.cantidaddisponible) as [Cant. Disponible],sum(s.metros) as [Metros],");

            sb.AppendFormat("u.codigounidad as [UM], ");
            sb.AppendFormat("dbo._fn_enum_TipoStockSeguridad(sseg.stockseguridad) as [Tipo de stock], sseg.stockminimo as [Stock mínimo], sseg.stockmaximo as [Stock máximo], "+
                            "CASE WHEN sseg.stockseguridad = 0 THEN (CASE WHEN (sseg.stockmaximo - sum(s.metros)) <= 0 THEN 0 ELSE (sseg.stockmaximo - sum(s.metros)) END)" +
                            " ELSE (CASE WHEN (sseg.stockmaximo - sum( s.cantidaddisponible)) <= 0 THEN 0 ELSE (sseg.stockmaximo - sum( s.cantidaddisponible)) END) END as [Pedido óptimo], ");
            sb.AppendFormat("u.decimalestotales as [_decimales] ");
            sb.AppendFormat(" from stockactual as s ");

            sb.AppendFormat(" inner join articulos as a on a.id = s.fkarticulos and a.empresa= s.empresa ");
            sb.AppendFormat(" inner join familiasproductos as fp on fp.id = substring(s.fkarticulos, 0, 3) and fp.empresa= a.empresa ");
            sb.AppendFormat(" inner join unidades as u on u.id = fp.fkunidadesmedida ");
            sb.AppendFormat(" inner join almacenes as alm on alm.empresa=s.empresa and alm.id=s.fkalmacenes ");
            sb.AppendFormat(" inner join ArticulosStockSeguridad as sseg on sseg.empresa = s.empresa and sseg.codarticulo = s.fkarticulos and sseg.codalmacen = s.fkalmacenes ");
            sb.AppendFormat(" left join almaceneszona as almz on almz.empresa=s.empresa and almz.fkalmacenes=s.fkalmacenes and almz.id=s.fkalmaceneszona ");
            sb.AppendFormat(" left join materiales as ml on ml.id = substring(s.fkarticulos, 3, 3) and ml.empresa= a.empresa ");
            sb.AppendFormat(" left join Familiamateriales  as fm on fm.valor=ml.fkfamiliamateriales ");

            return sb.ToString();
        }

        #endregion

        #region Cadena agrupacion

        public string CadenaAgrupacionAgrupadoArticulos()
        {
            if(Mostrar == "0")
            {
                return " group by s.fkarticulos, a.descripcion,u.codigounidad,u.decimalestotales " +
                        GetColumnasAgrupacionAlmacen() +
                        ", sseg.stockseguridad,sseg.stockminimo,sseg.stockmaximo " +
                        "having CASE WHEN sseg.stockseguridad = 0 THEN(sseg.stockmaximo - sum(s.metros)) ELSE(sseg.stockmaximo - sum(s.cantidaddisponible)) END > 0";
            }

            return " group by s.fkarticulos, a.descripcion,u.codigounidad,u.decimalestotales " +
                        GetColumnasAgrupacionAlmacen() +
                        ", sseg.stockseguridad,sseg.stockminimo,sseg.stockmaximo ";
        }

        #endregion

        #endregion
    }
}

