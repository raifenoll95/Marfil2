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
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RPedidosCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PedidosCompras;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosPedidosCompras : ListadosModel
    {
        private List<string> _series = new List<string>();

        #region Properties

        public override string TituloListado => "Listado de Pedidos Compras";

        public override string IdListado
        {
            get { return Agrupacion == "2" ? FListadosModel.PedidosComprasCabeceraAgrupado : FListadosModel.PedidosComprasCabecera; }
        }

        public override string WebIdListado => FListadosModel.PedidosComprasCabecera;

        public string Agrupacion { get; set; }

        public string Order { get; set; }

        [Display(ResourceType = typeof (RPedidosCompras), Name = "SeriesListado")]
        public List<string> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "ProveedorDesde")]
        public string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "ProveedorHasta")]
        public string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkfamiliacliente")]
        public string Fkfamiliacliente { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkzonacliente")]
        public string Fkzonacliente { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkcuentasagente")]
        public string Agente { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkcuentascomercial")]
        public string Comercial { get; set; }
       
        [Display(ResourceType = typeof(RDireccion), Name = "Fkpais")]
        public string Fkpais { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkmonedas")]
        public string Fkmonedas { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public List<EstadosModel> Estados
        {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {
                    var list = estadoService.GetStates(DocumentoEstado.PedidosCompras).ToList();
                    list.Insert(0, new EstadosModel());

                    return list;
                }
            }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Estado")]
        public string Estado { get; set; }

        #endregion

        public ListadosPedidosCompras()
        {

        }

        public ListadosPedidosCompras(IContextService context) : base(context)
        {
            ConfiguracionColumnas.Add("Bruto", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() {Columna = "_decimales"} });
            ConfiguracionColumnas.Add("Imp. Dto. Cial.", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Imp. Dto. P.P.", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Base", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });
            ConfiguracionColumnas.Add("Total", new ConfiguracionColumnasModel() { Decimales = new ConfiguracionDecimalesColumnasModel() { Columna = "_decimales" } });

            Agrupacion = "1";
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
                
                sb.Append(" p.fkseries in ( "+ string.Join(", ", Series.ToArray().Select(f => "@" + f))  + " ) ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidosCompras.SeriesListado, string.Join(", ", Series.ToArray())));
                flag = true;
            }
            if (!string.IsNullOrEmpty(Estado) && !Estado.Equals("0-"))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("estado", Estado);
                sb.Append(" p.fkestados=@estado ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidosCompras.Estado, Estados?.First(f => f.CampoId == Estado).Descripcion??string.Empty));
                flag = true;
            }
            if (FechaDesde.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechadesde", FechaDesde.Value);
                sb.Append(" p.fechadocumento>=@fechadesde ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidosCompras.FechaDesde, FechaDesde));
                flag = true;
            }

            if (FechaHasta.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechahasta", FechaHasta.Value);
                sb.Append(" p.fechadocumento<=@fechahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidosCompras.FechaHasta, FechaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentadesde", CuentaDesde);
                sb.Append(" p.fkproveedores>=@cuentadesde ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaDesde, CuentaDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentahasta", CuentaHasta);
                sb.Append(" p.fkproveedores<=@cuentahasta ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaHasta, CuentaHasta));
                flag = true;
            }


            if (!string.IsNullOrEmpty(Fkfamiliacliente))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkfamiliacliente", Fkfamiliacliente);
                sb.Append(" c.fkfamiliaproveedor=@fkfamiliacliente ");
                Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkfamiliacliente, Fkfamiliacliente));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkzonacliente))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fkzonacliente", Fkzonacliente);
                sb.Append(" c.fkzonaproveedor=@fkzonacliente ");
                Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkzonacliente, Fkzonacliente));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Agente))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("agente", Agente);
                sb.Append(" p.fkagentes = @agente ");
                Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkcuentasagente, Agente));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Comercial))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("comercial", Comercial);
                sb.Append(" p.fkcomerciales = @comercial ");
                Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkcuentascomercial, Comercial));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkpais))
            {
                if (flag)
                    sb.Append(" AND ");
                AppService = new ApplicationHelper(Context);
                ValoresParametros.Add("pais", Fkpais);
                sb.Append(" di.fkpais  =@pais ");
                Condiciones.Add(string.Format("{0}: {1}", RDireccion.Fkpais, AppService.GetListPaises().SingleOrDefault(f => f.Valor == Fkpais).Descripcion));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkmonedas))
            {
                if (flag)
                    sb.Append(" AND ");
                var serviceMonedas = FService.Instance.GetService(typeof (MonedasModel), Context);

                ValoresParametros.Add("Fkmonedas", Fkmonedas);
                sb.Append(" p.fkmonedas  =@Fkmonedas ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidosCompras.Fkmonedas, serviceMonedas.getAll().Select(f=>(MonedasModel)f).SingleOrDefault(f => f.Id ==int.Parse(Fkmonedas)).Descripcion));
                flag = true;
            }

            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();

            if (Agrupacion == "1")
            {
                sb.Append(
               "select p.referencia as [Referencia],p.Fechadocumento as [Fecha],es.descripcion as [Estado], cu.id as Id,cu.descripcion as [Descripción],co.Descripcion as Comercial,a.Descripcion as Agente,fc.descripcion as [Familia],zc.descripcion as [Zona],p.importebruto as [Bruto],p.importedescuentocomercial as [Imp. Dto. Cial.],p.importedescuentoprontopago as [Imp. Dto. P.P.], p.importebaseimponible as [Base], p.importetotaldoc as [Total],m.abreviatura as [Moneda],cu.nif as [Nif],di.email as [Email],fp.nombre as [Forma de pago],p.fkobras as [Cod. Obra],ob.nombreobra as [Obra],di.telefono as [Teléfono], di.telefonomovil  as [Teléfono movil],pa.descripcion as [País],pr.nombre [Provincia],di.poblacion [Población],m.decimales as [_decimales] from PedidosCompras as p " +
                " left join proveedores as c on c.empresa= p.empresa and p.fkproveedores= c.fkcuentas" +
                " left join acreedores as ac on ac.empresa= p.empresa and p.fkproveedores= ac.fkcuentas " +
                " inner join cuentas as cu on c.empresa= cu.empresa and c.fkcuentas=cu.id and cu.tipocuenta= " +
                (int)TiposCuentas.Proveedores + " left join direcciones as di on di.empresa=c.empresa and di.tipotercero=" +
                (int)TiposCuentas.Proveedores + " and di.fkentidad=c.fkcuentas and di.defecto='true' " +
               " left join formaspago as fp on fp.id=c.fkformaspago " +
               " left join paises as pa on pa.Valor=di.fkpais " +
               " left join provincias as pr on pr.codigopais=di.fkpais and pr.id=di.fkprovincia " +
               " left join familiasclientes as fc  on fc.valor=isnull(c.fkfamiliaproveedor,ac.fkfamiliaacreedor)  " +
               " left join zonasclientes as zc on zc.valor= isnull(c.fkfamiliaproveedor,ac.fkfamiliaacreedor) " +
               " left join estados as es on p.fkestados=Concat(es.documento,'-',es.id) " +
               " left join Cuentas as a on a.id=p.fkagentes and  a.empresa= p.empresa " +
               " left join Cuentas as co on co.id=p.fkcomerciales and  co.empresa= p.empresa " +
               " left join monedas as m on m.id = p.fkmonedas" +
               " left join obras as ob on ob.empresa=p.empresa and convert(varchar(10),ob.id) = p.fkobras");
            }
            else
            {

                sb.Append(
                "select cu.id as Id,cu.descripcion as [Descripción],co.Descripcion as [Comercial],a.Descripcion as [Agente],fc.descripcion as [Familia],zc.descripcion as [Zona],sum(p.importebruto) as [Bruto],sum(p.importedescuentocomercial) as [Imp. Dto. Cial.],sum(p.importedescuentoprontopago) as [Imp. Dto. P.P.], sum(p.importebaseimponible) as [Base],sum( p.importetotaldoc) as [Total],m.abreviatura as [Moneda],cu.nif as [Nif],pa.descripcion as [País],di.poblacion [Población],di.telefono as [Teléfono],di.email as [Email],m.decimales as [_decimales] from pedidoscompras as p " +
                " left join proveedores as c on c.empresa= p.empresa and p.fkproveedores= c.fkcuentas" +
                " left join acreedores as ac on ac.empresa= p.empresa and p.fkproveedores= ac.fkcuentas " +
                " inner join cuentas as cu on c.empresa= cu.empresa and c.fkcuentas=cu.id and cu.tipocuenta= " +
                (int)TiposCuentas.Proveedores + " left join direcciones as di on di.empresa=c.empresa and di.tipotercero=" +
                (int)TiposCuentas.Proveedores + " and di.fkentidad=c.fkcuentas and di.defecto='true' " +
                " left join paises as pa on pa.Valor=di.fkpais " +
                " left join familiasclientes as fc  on fc.valor=isnull(c.fkfamiliaproveedor,ac.fkfamiliaacreedor)  " +
               " left join zonasclientes as zc on zc.valor= isnull(c.fkfamiliaproveedor,ac.fkfamiliaacreedor) " +
                " left join Cuentas as a on a.id=p.fkagentes and  a.empresa= p.empresa " +
                " left join Cuentas as co on co.id=p.fkcomerciales and  co.empresa= p.empresa " +
                " left join monedas as m on m.id = p.fkmonedas");
            }
               

            

            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            if (Agrupacion == "2")
            {
                return " group by cu.id,cu.descripcion,co.descripcion,a.descripcion,fc.descripcion,fc.descripcion,zc.descripcion,m.abreviatura,cu.nif,di.poblacion,di.email,di.telefono,pa.descripcion,m.decimales";
            }

            switch (Order)
            {
                case "1":
                    return " order by p.referencia";
                case "2":
                    return " order by p.fechadocumento";
                case "3":
                    return " order by cu.id";
                case "4":
                    return " order by cu.descripcion";
                case "5":
                    return " order by pa.descripcion,di.fkprovincia,di.poblacion,cu.id";
                

            }

            return result;
        }

        public IEnumerable<SeriesModel> SeriesListado
        {
            get
            {
                
                var service = FService.Instance.GetService(typeof (SeriesModel), Context) as SeriesService;
                return service.GetSeriesTipoDocumento(TipoDocumento.PedidosCompras);
            }
        }
    }
}
