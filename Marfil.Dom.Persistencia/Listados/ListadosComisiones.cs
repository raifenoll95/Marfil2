using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using RListadoComision = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ListadoComisiones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RFacturas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Facturas;
namespace Marfil.Dom.Persistencia.Listados
{
    public enum TipoComision
    {
        [StringValue(typeof(RListadoComision), "TipoComisionAgentes")]
        Agentes = 5,
        [StringValue(typeof(RListadoComision), "TipoComisionComerciales")]
        Comerciales = 8
    }

    public class ListadosComisiones : ListadosModel
    {
        #region Members

        private TipoComision _tipo = TipoComision.Agentes;
        private List<string> _series = new List<string>();
        #endregion

        #region Properties

        public IEnumerable<SeriesModel> SeriesListado
        {
            get
            {
                var service = FService.Instance.GetService(typeof(SeriesModel), Context) as SeriesService;
                return service.GetSeriesTipoDocumento(TipoDocumento.FacturasVentas);
            }
        }

        public override string TituloListado => RListadoComision.TituloListado;

        public override string IdListado => FListadosModel.Comisiones;

        public string Order
        {
            get; set;
        }

        public TipoComision Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "SeriesListado")]
        public List<string> Series
        {
            get { return _series; }
            set { _series = value; }
        }

        [XmlIgnore]
        [IgnoreDataMember]
        public List<EstadosModel> Estados
        {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {
                    var list = estadoService.GetStates(DocumentoEstado.FacturasVentas).ToList();
                    list.Insert(0, new EstadosModel());

                    return list;
                }
            }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Estado")]
        public string Estado { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(Rcuentas), Name = "CuentaDesde")]
        public string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(Rcuentas), Name = "CuentaHasta")]
        public string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fksituacioncomision")]
        public string SituacionComision { get; set; }

        #endregion

        public ListadosComisiones()
        {
            
        }

        public ListadosComisiones(IContextService context) :base(context)
        {
           
        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.Append(" fa.empresa = '" + Empresa + "' ");
            

            if (Series?.Any() ?? false)
            {
                if (flag)
                    sb.Append(" AND ");

                foreach (var item in Series)
                    ValoresParametros.Add(item, item);

                sb.Append(" fa.fkseries in ( " + string.Join(", ", Series.ToArray().Select(f => "@" + f)) + " ) ");
                Condiciones.Add(string.Format("{0}: {1}", RFacturas.SeriesListado, string.Join(", ", Series.ToArray())));
                flag = true;
            }
            if (!string.IsNullOrEmpty(Estado) && !Estado.Equals("0-"))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("estado", Estado);
                sb.Append(" fa.fkestados=@estado ");
                Condiciones.Add(string.Format("{0}: {1}", RFacturas.Estado, Estados?.First(f => f.CampoId == Estado).Descripcion??string.Empty));
                flag = true;
            }
            if (FechaDesde.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechadesde", FechaDesde.Value);
                sb.Append(" fa.fechadocumento>=@fechadesde ");
                Condiciones.Add(string.Format("{0}: {1}", RFacturas.FechaDesde, FechaDesde));
                flag = true;
            }

            if (FechaHasta.HasValue)
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("fechahasta", FechaHasta.Value);
                sb.Append(" fa.fechadocumento<=@fechahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RFacturas.FechaHasta, FechaHasta));
                flag = true;
            }

            var tipocuenta = Tipo == TipoComision.Agentes ? "fkagentes" : "fkcomerciales";

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentadesde", CuentaDesde);
                sb.AppendFormat(" fa.{0}>=@cuentadesde ", tipocuenta);
                Condiciones.Add(string.Format("{0}: {1}", Funciones.GetEnumByStringValueAttribute(Tipo), CuentaDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("cuentahasta", CuentaHasta);
                sb.AppendFormat(" fa.{0}<=@cuentahasta ", tipocuenta);
                Condiciones.Add(string.Format("{0}: {1}", Funciones.GetEnumByStringValueAttribute(Tipo), CuentaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(SituacionComision))
            {
                if (flag)
                    sb.Append(" AND ");

                ValoresParametros.Add("situacioncomision", SituacionComision);
                sb.AppendFormat(" fa.fksituacioncomision=@situacioncomision ");
                Condiciones.Add(string.Format("{0}: {1}", RFacturas.Fksituacioncomision, SituacionComision));
                flag = true;
            }
            if (flag)
                    sb.Append(" AND ");
                sb.AppendFormat(" (fa.{0} is not null and fa.{0} <>'' )", tipocuenta);
                flag = true;
            
            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
            var columnacuenta = Tipo == TipoComision.Agentes
                ? "isnull(fa.fkagentes,'')"
                : "isnull(fa.fkcomerciales,'')";
            var porcentajecomision = Tipo == TipoComision.Agentes
                ? "isnull(fa.comisionagente,0)"
                : "isnull(fa.comisioncomercial,0)";
            var importecomision = Tipo == TipoComision.Agentes
                ? "isnull(fa.importecomisionagente,0)"
                : "isnull(fa.importecomisioncomercial,0)";

            sb.AppendFormat("select {2} as {3},cu.descripcion as [Descripción], fa.fechadocumento as [Fecha], fa.Referencia as [Referencia],fa.nombrecliente as [Nombre cliente],fa.clientepoblacion as [Población],fa.netobasecomision as [Base comisión],{0} as [% Comisión],{1} as [Importe comisión] from facturas as fa" +
                            " inner join cuentas as cu on cu.empresa= fa.empresa and cu.id= {2}",porcentajecomision,importecomision,columnacuenta,Funciones.GetEnumByStringValueAttribute(Tipo));

            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var columnacuenta = Tipo == TipoComision.Agentes
                   ? "fkagentes"
                   : "fkcomerciales";

            return string.Format(" order by fa.{0}, fa.fechadocumento",columnacuenta);
        }
    }
}
