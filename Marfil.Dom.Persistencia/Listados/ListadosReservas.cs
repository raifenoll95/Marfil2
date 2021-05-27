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
using RPedidos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Pedidos;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosReservas : ListadosModel
    {
        private List<string> _series = new List<string>();

        #region Properties

        public override string TituloListado => "Listado de material reservado";

        public override string IdListado => FListadosModel.ListadoReservas;
        

        public string Agrupacion { get; set; }

        public string Order { get; set; }

       
        [Display(ResourceType = typeof(RPedidos), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "ClienteDesde")]
        public string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "ClienteHasta")]
        public string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FkarticulosDesde")]
        public string ArticuloDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FkarticulosHasta")]
        public string ArticuloHasta { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "LoteDesde")]
        public string LoteDesde { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "LoteDesde")]
        public string LoteHasta { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public List<EstadosModel> Estados
        {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {
                    var list = estadoService.GetStates(DocumentoEstado.PedidosVentas).ToList();
                    list.Insert(0, new EstadosModel());

                    return list;
                }
            }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Estado")]
        public string Estado { get; set; }

        #endregion

        public ListadosReservas()
        {

        }

        public ListadosReservas(IContextService context) : base(context)
        {
            Agrupacion = "1";
        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.Append(" p.empresa = '" + Context.Empresa + "' ");

           
            if (!string.IsNullOrEmpty(Estado) && !Estado.Equals("0-"))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("estado", Estado);
                sb.Append(" p.fkestados=@estado ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.Estado, Estados?.First(f => f.CampoId == Estado).Descripcion??string.Empty));
                flag = true;
            }
            if (FechaDesde.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechadesde", FechaDesde.Value);
                sb.Append(" p.fechadocumento>=@fechadesde ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.FechaDesde, FechaDesde));
                flag = true;
            }

            if (FechaHasta.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechahasta", FechaHasta.Value);
                sb.Append(" p.fechadocumento<=@fechahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.FechaHasta, FechaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(ArticuloDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("articulodesde", ArticuloDesde);
                sb.Append(" pl.fkarticulos>=@articulodesde ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.FkarticulosDesde, ArticuloDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(ArticuloHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("articulohasta", ArticuloHasta);
                sb.Append(" pl.fkarticulos<=@articulohasta ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.FkarticulosHasta, ArticuloHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(LoteDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("lotedesde", LoteDesde);
                sb.Append(" pl.lote>=@lotedesde ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.LoteDesde, LoteDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(LoteHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("lotehasta", LoteHasta);
                sb.Append(" pl.lote<=@lotedesde ");
                Condiciones.Add(string.Format("{0}: {1}", RPedidos.LoteHasta, LoteHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentadesde", CuentaDesde);
                sb.Append(" p.fkclientes>=@cuentadesde ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaDesde, CuentaDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentahasta", CuentaHasta);
                sb.Append(" p.fkclientes<=@cuentahasta ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaHasta, CuentaHasta));
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
               "select pl.fkarticulos as [Cod. Articulo],a.descripcionabreviada as [Descripcion], p.fkclientes as [Cliente],cu.descripcion as [Descripción cliente],sum(pl.cantidad) as [Cantidad],sum(sh.metros) as [Metros],ud.textocorto as UM,p.fechadocumento as [Fecha reserva] from reservasstock as p " +
               " inner join reservasstocklin as pl on pl.empresa= p.empresa and pl.fkreservasstock = p.id " +
               " inner join clientes as c on c.empresa= p.empresa and p.fkclientes= c.fkcuentas inner join cuentas as cu on c.empresa= cu.empresa and c.fkcuentas=cu.id and cu.tipocuenta= " + (int)TiposCuentas.Clientes + "  " +
               " inner join articulos as a on a.empresa=p.empresa and a.id= pl.fkarticulos " +
               " left join estados as es on p.fkestados=Concat(es.documento,'-',es.id)" +
               " left join stockhistorico as sh  on sh.empresa= p.empresa and sh.lote= pl.lote and sh.loteid=pl.tabla " +
               " left join unidades as ud on ud.id= pl.fkunidades");
            }
            else
            {
                sb.Append(
               "select pl.fkarticulos as [Cod. Articulo],a.descripcionabreviada as [Descripcion], p.fkclientes as [Cliente],cu.descripcion as [Descripción cliente],pl.lote as [Lote],pl.tabla as[Tabla],pl.cantidad as [Cantidad],sh.largo as [Largo],sh.ancho as [Ancho],sh.grueso as [Grueso],sh.metros as [Metros],ud.textocorto as UM,p.fechadocumento as [Fecha reserva] from reservasstock as p " +
               " inner join reservasstocklin as pl on pl.empresa= p.empresa and pl.fkreservasstock = p.id " +
               " inner join clientes as c on c.empresa= p.empresa and p.fkclientes= c.fkcuentas inner join cuentas as cu on c.empresa= cu.empresa and c.fkcuentas=cu.id and cu.tipocuenta= " + (int)TiposCuentas.Clientes + "  " +
               " inner join articulos as a on a.empresa=p.empresa and a.id= pl.fkarticulos " +
               " left join estados as es on p.fkestados=Concat(es.documento,'-',es.id)" +
               " left join stockhistorico as sh  on sh.empresa= p.empresa and sh.lote= pl.lote and sh.loteid=pl.tabla " +
               " left join unidades as ud on ud.id= pl.fkunidades");
            }
               

            

            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            if (Agrupacion == "1")
            {
                result = " group by pl.fkarticulos,a.descripcionabreviada, p.fkclientes,cu.descripcion,ud.textocorto,p.fechadocumento ";
            }

            result += " order by p.fechadocumento,pl.fkarticulos,p.fkclientes";
            

            return result;
        }

        public IEnumerable<SeriesModel> SeriesListado
        {
            get
            {
               
                var service = FService.Instance.GetService(typeof (SeriesModel), Context) as SeriesService;
                return service.GetSeriesTipoDocumento(TipoDocumento.Reservas);
            }
        }
    }
}
