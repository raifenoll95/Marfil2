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
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RProveedores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
namespace Marfil.Dom.Persistencia.Listados
{
    public enum ListadoTipoDocumento
    {
        Presupuestos,
        Pedidos,
        Albaranes,
        Facturas,
        PresupuestosCompras,
        PedidosCompras,
        AlbaranesCompras,
        FacturasCompras
    }

    public abstract class ListadosProductosDocumentos : ListadosModel
    {
        private List<string> _series = new List<string>();
        private List<string> _estados = new List<string>();
        private  ListadoTipoDocumento _tipo;
        private  string _columnatercero;
        private  string _claveajena;
        public readonly string Clientedesdelabel;
        public readonly string Clientehastalabel;
        public readonly string Fkfamiliaclientelabel;
        public readonly string Fkzonaclientelabel;

        #region Properties

        public ListadoTipoDocumento Tipo { get { return _tipo; } }

        public string Agrupacion { get; set; }

        public string Order { get; set; }

        [Display(ResourceType = typeof (RAlbaranes), Name = "SeriesListado")]
        public List<string> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ClienteDesde")]
        public virtual string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ClienteHasta")]
        public virtual string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkfamiliacliente")]
        public virtual string Fkfamiliacliente { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkzonacliente")]
        public virtual string Fkzonacliente { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FkarticulosDesde")]
        public string FkarticulosDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FkarticulosHasta")]
        public string FkarticulosHasta { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "FamiliaMateriales")]
        public string Fkfamiliasmateriales{ get; set; }

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

        [XmlIgnore]
        public List<EstadosModel> ListEstados {
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
                    else if (_tipo == ListadoTipoDocumento.PresupuestosCompras)
                    {
                        doc = DocumentoEstado.PresupuestosCompras;
                        _columnatercero = "fkproveedores";
                        _claveajena = "presupuestoscompras";
                    }
                    else if (_tipo == ListadoTipoDocumento.PedidosCompras)
                    {
                        doc = DocumentoEstado.PresupuestosCompras;
                        _columnatercero = "fkproveedores";
                        _claveajena = "pedidoscompras";
                    }
                    else if (_tipo == ListadoTipoDocumento.FacturasCompras)
                    {
                        doc = DocumentoEstado.PresupuestosCompras;
                        _columnatercero = "fkproveedores";
                        _claveajena = "facturascompras";
                    }

                    var list = estadoService.GetStates(doc).ToList();
                    return list;

                }
            }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Estado")]
        public string Estado { get; set; }
        public List<string> Estados
        {
            get { return _estados; }
            set { _estados = value; }
        }

        #endregion

        internal ListadosProductosDocumentos(ListadoTipoDocumento tipo,IContextService context):base(context)
        {
            _tipo = tipo;
            _columnatercero = "fkclientes";

            if (_tipo == ListadoTipoDocumento.AlbaranesCompras)
            {
                _columnatercero = "fkproveedores";
            }
            else if (_tipo == ListadoTipoDocumento.PresupuestosCompras)
            {
                _columnatercero = "fkproveedores";
            }
            else if (_tipo == ListadoTipoDocumento.PedidosCompras)
            {
                _columnatercero = "fkproveedores";
            }
            else if (_tipo == ListadoTipoDocumento.FacturasCompras)
            {
                _columnatercero = "fkproveedores";
            }

            Clientedesdelabel = tipo <= ListadoTipoDocumento.Facturas ? RAlbaranes.ClienteDesde : RAlbaranes.ProveedorDesde;
            Clientehastalabel = tipo <= ListadoTipoDocumento.Facturas ? RAlbaranes.ClienteHasta : RAlbaranes.ProveedorHasta;
            Fkfamiliaclientelabel = tipo <= ListadoTipoDocumento.Facturas ? RClientes.Fkfamiliacliente: RProveedores.Fkfamiliaproveedor;
            Fkzonaclientelabel = tipo <= ListadoTipoDocumento.Facturas ? RClientes.Fkzonacliente : RProveedores.Fkzonaproveedor;
            Agrupacion = "1";
            _claveajena = tipo.ToString();
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

            if (Estados?.Any()??false)
            {
                if (flag)
                    sb.Append(" AND ");
                
                sb.Append(" p.fkestados in ( " + string.Join(", ", Estados.ToArray().Select(f => "'" + f+"'")) + " ) ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.Estado, string.Join(", ", Estados.Select(f=> ListEstados.First(j => j.CampoId == f).Descripcion))));
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
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaDesde, CuentaDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentahasta", CuentaHasta);
                sb.AppendFormat(" p.{0}<=@cuentahasta ", _columnatercero);
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaHasta, CuentaHasta));
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

            if (Agrupacion == "1")
            {
                sb.AppendFormat(
                "select mo.decimales as [_decimales],  ud.decimalestotales as [_decimalesunidades], a.id as [Cod. Artículo],a.Descripcionabreviada as  [Artículo],gm.descripcion as [Grupo material],fm.descripcion as [Familia material],sum(pl.cantidad) as [Cantidad],sum(pl.metros) as [Metros],ud.textocorto as [UM], case when sum(pl.metros)=0 then 0 else sum(pl.importe)/sum(pl.metros) end as [Precio medio],sum(pl.importe) as [Total] from {0} as p " +
                " inner join {0}lin as pl on pl.empresa= p.empresa and pl.fk{2}= p.id" +
                " inner join cuentas as cu on p.empresa= cu.empresa and p.{1}=cu.id " +
                " inner join articulos as a on a.empresa=p.empresa and  a.id=pl.fkarticulos and isnull(a.articulocomentario,0)=0 " +
                " inner join familiasproductos as fp on fp.empresa= p.empresa and fp.id=substring(pl.fkarticulos,0,3) " +
                " left join materiales  as m on m.empresa=p.empresa and m.id=substring(pl.fkarticulos,3,3) " +
                " left join Gruposmateriales  as gm on gm.valor=a.fkgruposmateriales " +
                " left join Familiamateriales  as fm on fm.valor=m.fkfamiliamateriales " +
                " left join unidades as ud on ud.id= fp.fkunidadesmedida " +
                " left join monedas as mo on mo.id = p.fkmonedas" +
                " ", _tipo, _columnatercero, _claveajena);
            }
            else if (Agrupacion == "2")
            {
                sb.AppendFormat(
               "select mo.decimales as [_decimales],  ud.decimalestotales as [_decimalesunidades],a.id as [Cod. Artículo],a.Descripcionabreviada as  [Artículo],gm.descripcion as [Grupo material],fm.descripcion as [Familia material],sum(pl.cantidad) as [Piezas],sum(pl.metros) as [Total medidas],ud.textocorto as [UM],case when sum(pl.metros)=0 then 0 else sum(pl.importe)/sum(pl.metros) end as [Precio medio],sum(pl.importe) as [Total],p.{1} as [Cod. Cuenta],cu.descripcion as [Cuenta] from {0} as p " +
               " inner join {0}lin as pl on pl.empresa= p.empresa and pl.fk{2}= p.id" +
               " inner join cuentas as cu on p.empresa= cu.empresa and p.{1}=cu.id " +
               " inner join articulos as a on a.empresa=p.empresa and  a.id=pl.fkarticulos and isnull(a.articulocomentario,0)=0" +
               " inner join familiasproductos as fp on fp.empresa= p.empresa and fp.id=substring(pl.fkarticulos,0,3) " +
               " left join materiales  as m on m.empresa=p.empresa and m.id=substring(pl.fkarticulos,3,3) " +
               " left join Gruposmateriales  as gm on gm.valor=a.fkgruposmateriales " +
               " left join Familiamateriales  as fm on fm.valor=m.fkfamiliamateriales " +
               " left join unidades as ud on ud.id= fp.fkunidadesmedida " +
               " left join monedas as mo on mo.id = p.fkmonedas" +
               " ", _tipo, _columnatercero, _claveajena);
            }
            else if(Agrupacion=="3")
            {
                sb.AppendFormat(
                "select mo.decimales as [_decimales],  ud.decimalestotales as [_decimalesunidades],a.id as [Cod. Artículo],a.Descripcionabreviada as  [Artículo],gm.descripcion as [Grupo material],fm.descripcion as [Familia material],sum(pl.cantidad) as [Piezas],sum(pl.metros) as [Total medidas],ud.textocorto as [UM],case when sum(pl.metros)=0 then 0 else sum(pl.importe)/sum(pl.metros) end as [Precio medio],sum(pl.importe) as [Total],p.referencia as [Documento],p.fechadocumento as [Fecha],p.{1} as [Cod. Cuenta],cu.descripcion as [Cuenta] from {0} as p " +
                " inner join {0}lin as pl on pl.empresa= p.empresa and pl.fk{2}= p.id" +
                " inner join cuentas as cu on p.empresa= cu.empresa and p.{1}=cu.id " +
                " inner join articulos as a on a.empresa=p.empresa and  a.id=pl.fkarticulos and isnull(a.articulocomentario,0)=0" +
                " inner join familiasproductos as fp on fp.empresa= p.empresa and fp.id=substring(pl.fkarticulos,0,3) " +
                " left join materiales  as m on m.empresa=p.empresa and m.id=substring(pl.fkarticulos,3,3) " +
                " left join Gruposmateriales  as gm on gm.valor=a.fkgruposmateriales " +
                " left join Familiamateriales  as fm on fm.valor=m.fkfamiliamateriales " +
                " left join unidades as ud on ud.id= fp.fkunidadesmedida " +
                " left join monedas as mo on mo.id = p.fkmonedas" +
                " ", _tipo, _columnatercero,_claveajena);
            }
                

            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            switch (Agrupacion)
            {
                case "1":
                    result = " group by a.id,a.descripcionabreviada,gm.descripcion,fm.descripcion,ud.textocorto,mo.decimales,  ud.decimalestotales";
                    break;
                case "2":
                    result = string.Format("  group by a.id,a.descripcionabreviada,gm.descripcion,fm.descripcion,ud.textocorto,p.{0},cu.descripcion,mo.decimales,  ud.decimalestotales ", _columnatercero);
                    break;
                case "3":
                    result = string.Format("  group by a.id,a.descripcionabreviada,gm.descripcion,fm.descripcion,ud.textocorto,p.referencia,p.fechadocumento,p.{0},cu.descripcion,mo.decimales,  ud.decimalestotales ",_columnatercero);
                    break;
            }

            
            if (Agrupacion != "1")
            {
                switch (Order)
                {
                    case "1":
                        result += "    order by a.id";
                        break;
                    case "2":
                        result += "   order by p.referencia ";
                        break;
                    case "3":
                        result += "   order by p.fechadocumento";
                        break;
                    case "4":
                        result += string.Format("   order by p.{0}", _columnatercero);
                        break;

                }
            }
            else
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
