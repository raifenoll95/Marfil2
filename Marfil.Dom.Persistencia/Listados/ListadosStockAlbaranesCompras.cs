using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.ResourcesGlobalization.Textos.MenuAplicacion;
using RProveedores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosStockAlbaranesCompras : ListadosModel
    {
        private List<string> _series = new List<string>();
        private List<string> _estados = new List<string>();
        private ListadoTipoDocumento _tipo;
        private string _columnatercero;
        private string _claveajena;
        public readonly string Clientedesdelabel;
        public readonly string Clientehastalabel;
        public readonly string Fkfamiliaclientelabel;
        public readonly string Fkzonaclientelabel;
        public override string TituloListado => Menuaplicacion.listadostockalbaranes;
        public override string IdListado => FListadosModel.StockAlbaranesCompras;

        #region Properties

        public ListadoTipoDocumento Tipo { get { return _tipo; } }

        public string Agrupacion { get; set; }

        public string Order { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "SeriesListado")]
        public List<string> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ProveedorDesde")]
        public virtual string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ProveedorHasta")]
        public virtual string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkfamiliacliente")]
        public virtual string Fkfamiliacliente { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Fkzonacliente")]
        public virtual string Fkzonacliente { get; set; }

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

        [IgnoreDataMember]
        [XmlIgnore]
        public IList<EstadosModel> ListEstados {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {
                    var doc = DocumentoEstado.PresupuestosVentas;
                    if (_tipo == ListadoTipoDocumento.Presupuestos)
                        doc = DocumentoEstado.PresupuestosVentas;
                    else if (_tipo == ListadoTipoDocumento.Pedidos)
                        doc = DocumentoEstado.PedidosVentas;
                    else if (_tipo == ListadoTipoDocumento.Albaranes)
                        doc = DocumentoEstado.AlbaranesVentas;
                    else if (_tipo == ListadoTipoDocumento.Facturas)
                        doc = DocumentoEstado.FacturasVentas;
                    else if (_tipo == ListadoTipoDocumento.AlbaranesCompras)
                    {
                        doc = DocumentoEstado.AlbaranesCompras;
                        _columnatercero = "fkproveedores";
                        _claveajena = "albaranes";
                    }

                    return  estadoService.GetStates(doc).ToList();

                }
            } }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Estado")]
        public string Estado { get; set; }
        public List<string> Estados
        {
            get { return _estados; }
            set { _estados = value; }
        }

        #endregion

        public ListadosStockAlbaranesCompras()
        {

        }

        public ListadosStockAlbaranesCompras(IContextService context) : base(context)
        {

            _tipo = ListadoTipoDocumento.AlbaranesCompras;
            _columnatercero = "_fkproveedores";
            Clientedesdelabel = _tipo <= ListadoTipoDocumento.Facturas ? RAlbaranes.ClienteDesde : RAlbaranes.ProveedorDesde;
            Clientehastalabel = _tipo <= ListadoTipoDocumento.Facturas ? RAlbaranes.ClienteHasta : RAlbaranes.ProveedorHasta;
            Fkfamiliaclientelabel = _tipo <= ListadoTipoDocumento.Facturas ? Inf.ResourcesGlobalization.Textos.Entidades.Clientes.Fkfamiliacliente : RProveedores.Fkfamiliaproveedor;
            Fkzonaclientelabel = _tipo <= ListadoTipoDocumento.Facturas ? Inf.ResourcesGlobalization.Textos.Entidades.Clientes.Fkzonacliente : RProveedores.Fkzonaproveedor;
            Agrupacion = "1";
            //_claveajena = _tipo.ToString();
            _claveajena = "albaranes";
            ConfiguracionColumnas.Add("Precio", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("% Dto.", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Subtotal", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Metros", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimalesunidades" } });
            ConfiguracionColumnas.Add("Largo", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimalesunidades" } });
            ConfiguracionColumnas.Add("Ancho", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimalesunidades" } });
            ConfiguracionColumnas.Add("Grueso", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimalesunidades" } });

          
        }


        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.Append(" p.empresa = '" + Empresa + "' ");

            if (Series?.Any() ?? false)
            {
                if (flag)
                    sb.Append(" AND ");

                foreach (var item in Series)
                    ValoresParametros.Add(item, item);

                sb.Append(" p.fkseries in ( " + string.Join(", ", Series.ToArray().Select(f => "@" + f)) + " ) ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.SeriesListado, string.Join(", ", Series.ToArray())));
                flag = true;
            }

            if (Estados?.Any() ?? false)
            {
                if (flag)
                    sb.Append(" AND ");

                sb.Append(" p.fkestados in ( " + string.Join(", ", Estados.ToArray().Select(f => "'" + f + "'")) + " ) ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Estado, string.Join(", ", Estados.Select(f => ListEstados.First(j => j.CampoId == f).Descripcion))));
                flag = true;
            }

            if (FechaDesde.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechadesde", FechaDesde.Value);
                sb.Append(" p.fechadocumento>=@fechadesde ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FechaDesde, FechaDesde));
                flag = true;
            }

            if (FechaHasta.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechahasta", FechaHasta.Value);
                sb.Append(" p.fechadocumento<=@fechahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FechaHasta, FechaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentadesde", CuentaDesde);
                sb.AppendFormat(" p.{0}>=@cuentadesde ", _columnatercero);
                Condiciones.Add(string.Format("{0}: {1}", Inf.ResourcesGlobalization.Textos.Entidades.Cuentas.CuentaDesde, CuentaDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentahasta", CuentaHasta);
                sb.AppendFormat(" p.{0}<=@cuentahasta ", _columnatercero);
                Condiciones.Add(string.Format("{0}: {1}", Inf.ResourcesGlobalization.Textos.Entidades.Cuentas.CuentaHasta, CuentaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticulosdesde", FkarticulosDesde);
                sb.Append(" pl.fkarticulos >= @fkarticulosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosDesde, FkarticulosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkarticulosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkarticuloshasta", FkarticulosHasta);
                sb.Append(" pl.fkarticulos <= @fkarticuloshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FkarticulosHasta, FkarticulosHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkfamiliasmateriales))
            {
                if (flag)
                    sb.Append(" AND ");
                AppService = new ApplicationHelper(Context);
                ValoresParametros.Add("fkfamiliasmateriales", Fkfamiliasmateriales);
                sb.Append("  exists(select mm.* from materiales as mm where mm.id=Substring(pl.fkarticulos,3,3) and mm.fkfamiliamateriales=@fkfamiliasmateriales)  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaMateriales, AppService.GetListFamiliaMateriales().SingleOrDefault(f => f.Valor == Fkfamiliasmateriales).Descripcion));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliasdesde", FkfamiliasDesde);
                sb.Append(" Substring(pl.fkarticulos,0,3) >= @fkfamiliasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaDesde, FkfamiliasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliashasta", FkfamiliasHasta);
                sb.Append(" Substring(pl.fkarticulos,0,3) <= @fkfamiliashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaHasta, FkfamiliasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialesdesde", FkmaterialesDesde);
                sb.Append(" Substring(pl.fkarticulos,3,3) >= @fkmaterialesdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesDesde, FkmaterialesDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkmaterialeshasta", FkmaterialesHasta);
                sb.Append(" Substring(pl.fkarticulos,3,3) <= @fkmaterialeshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesHasta, FkmaterialesHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicasdesde", FkcaracteristicasDesde);
                sb.Append(" Substring(pl.fkarticulos,6,2) >= @fkcaracteristicasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.CaracteristicasDesde, FkcaracteristicasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkcaracteristicasHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkcaracteristicashasta", FkcaracteristicasHasta);
                sb.Append(" Substring(pl.fkarticulos,6,2) <= @fkcaracteristicashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.CaracteristicasHasta, FkcaracteristicasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoresdesde", FkgrosoresDesde);
                sb.Append(" Substring(pl.fkarticulos,8,2) >= @fkgrosoresdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.GrosoresDesde, FkgrosoresDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkgrosoresHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkgrosoreshasta", FkgrosoresHasta);
                sb.Append(" Substring(pl.fkarticulos,8,2) <= @fkgrosoreshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.GrosoresHasta, FkgrosoresHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadosdesde", FkacabadosDesde);
                sb.Append(" Substring(pl.fkarticulos,10,2) >= @fkacabadosdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.AcabadosDesde, FkacabadosDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkacabadosHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkacabadoshasta", FkacabadosHasta);
                sb.Append(" Substring(pl.fkarticulos,10,2) <= @fkacabadoshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.AcabadosHasta, FkacabadosHasta));
                flag = true;
            }



            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();

            //sb.AppendFormat(
            //"select mo.decimales as [_decimales],  ud.decimalestotales as [_decimalesunidades],a.id as [Cod. Artículo],a.Descripcionabreviada as  [Artículo],pl.lote as [Lote],pl.Tabla as [Tabla],gm.descripcion as [Grupo material],fm.descripcion as [Familia material],pl.cantidad as [Cantidad],pl.largo as [Largo],pl.ancho as [Ancho],pl.grueso as [Grueso],pl.metros as [Metros],ud.textocorto as [UM],pl.precio as [Precio],pl.importe as [Subtotal],p.referencia as [Documento],p.fechadocumento as [Fecha],p.{1} as [Cod. Proveedor],cu.descripcion as [Proveedor] from {0} as p " +
            //" inner join {0}lin as pl on pl.empresa= p.empresa and pl.fk{2}= p.id" +
            //" inner join cuentas as cu on p.empresa= cu.empresa and p.{1}=cu.id " +
            //" inner join articulos as a on a.empresa=p.empresa and  a.id=pl.fkarticulos " +
            //" inner join familiasproductos as fp on fp.empresa= p.empresa and fp.id=substring(pl.fkarticulos,0,3) " +
            //" inner join stockactual as s on s.empresa=p.empresa and s.lote=pl.lote and s.loteid=isnull(pl.tabla,0) and pl.fkarticulos = s.fkarticulos and s.fkalmacenes=p.fkalmacen " +
            //" left join materiales  as m on m.empresa=p.empresa and m.id=substring(pl.fkarticulos,3,3) " +
            //" left join Gruposmateriales  as gm on gm.valor=a.fkgruposmateriales " +
            //" left join Familiamateriales  as fm on fm.valor=m.fkfamiliamateriales " +
            //" left join unidades as ud on ud.id= fp.fkunidadesmedida and ud.empresa= p.empresa" +
            //" left join monedas as mo on mo.id = p.fkmonedas" +
            //" ", _tipo, _columnatercero, _claveajena);

            sb.AppendFormat(
            "select mo.decimales as [_decimales],  ud.decimalestotales as [_decimalesunidades],a.id as [Cod. Artículo],a.Descripcionabreviada as  [Artículo],pl.lote as [Lote],pl.Tabla as [Tabla],gm.descripcion as [Grupo material],fm.descripcion as [Familia material],pl.cantidad as [Cantidad],pl.largo as [Largo],pl.ancho as [Ancho],pl.grueso as [Grueso],pl.metros as [Metros],ud.textocorto as [UM],pl.precio as [Precio],pl.importe as [Subtotal],p.referencia as [Documento],p.fechadocumento as [Fecha],p.fkproveedores as [Cod. Proveedor],cu.descripcion as [Proveedor] from AlbaranesCompras as p " +
            " inner join AlbaranesCompraslin as pl on pl.empresa= p.empresa and pl.fkalbaranes= p.id" +
            " inner join cuentas as cu on p.empresa= cu.empresa and p.fkproveedores=cu.id " +
            " inner join articulos as a on a.empresa=p.empresa and  a.id=pl.fkarticulos " +
            " inner join familiasproductos as fp on fp.empresa= p.empresa and fp.id=substring(pl.fkarticulos,0,3) " +
            " inner join stockactual as s on s.empresa=p.empresa and s.lote=pl.lote and s.loteid=isnull(pl.tabla,0) and pl.fkarticulos = s.fkarticulos and s.fkalmacenes=p.fkalmacen " +
            " left join materiales  as m on m.empresa=p.empresa and m.id=substring(pl.fkarticulos,3,3) " +
            " left join Gruposmateriales  as gm on gm.valor=a.fkgruposmateriales " +
            " left join Familiamateriales  as fm on fm.valor=m.fkfamiliamateriales " +
            " left join unidades as ud on ud.id= fp.fkunidadesmedida " +
            " left join monedas as mo on mo.id = p.fkmonedas");

            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            result += "    order by a.id";

            return result;
        }

        public IEnumerable<SeriesModel> SeriesListado
        {
            get
            {
               
                var service = FService.Instance.GetService(typeof(SeriesModel), Context) as SeriesService;
                return service.GetSeriesTipoDocumento((TipoDocumento)(int)_tipo);
            }
        }
    }
}
