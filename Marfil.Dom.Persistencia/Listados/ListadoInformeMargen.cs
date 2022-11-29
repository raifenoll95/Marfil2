using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Helpers;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RDireccion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
using Rcuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RInformeMargen = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.InformeMargen;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using System.Data.SqlClient;
using System.Data;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadoInformeMargen : ListadosModel
    {

        private List<string> _series = new List<string>();

        #region Properties

        public override string TituloListado => "Listado de margen";

        public override string IdListado => FListadosModel.InformeMargen;

        public string Group { get; set; }

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

        [Display(ResourceType = typeof(RAlbaranes), Name = "ClienteDesde")]
        public string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ClienteHasta")]
        public string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkfamiliacliente")]
        public string Fkfamiliacliente { get; set; }
        [Display(ResourceType = typeof(RInformeMargen), Name = "FkAgentes")]
        public string FkAgentes { get; set; }

        [Display(ResourceType = typeof(RClientes), Name = "Fkzonacliente")]
        public string Fkzonacliente { get; set; }

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

        [XmlIgnore]
        [IgnoreDataMember]
        public List<EstadosModel> Estados
        {
            get
            {
                using (var estadoService = new EstadosService(Context))
                {
                    var list = estadoService.GetStates(DocumentoEstado.AlbaranesVentas).ToList();
                    list.Insert(0, new EstadosModel());

                    return list;
                }
            }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Estado")]
        public string Estado { get; set; }

        #endregion

        public ListadoInformeMargen()
        {

        }

        public ListadoInformeMargen(IContextService context) : base(context)
        {

        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = true;
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.Append(" i.empresa = '" + Empresa + "' ");
            
            //Inicializar los parámetros a NULL porque siempre tiene que llegar un valor
            ValoresParametros.Add("EMPRESA",Empresa);
            ValoresParametros.Add("ARTICULO_DESDE", DBNull.Value);
            ValoresParametros.Add("ARTICULO_HASTA", DBNull.Value);
            ValoresParametros.Add("SERIES", DBNull.Value);
            ValoresParametros.Add("FECHA_DESDE", DBNull.Value);
            ValoresParametros.Add("FECHA_HASTA", DBNull.Value);
            ValoresParametros.Add("CLIENTE_DESDE", DBNull.Value);
            ValoresParametros.Add("CLIENTE_HASTA", DBNull.Value);
            ValoresParametros.Add("FAMILIACLIENTE", DBNull.Value);
            ValoresParametros.Add("ZONACLIENTE", DBNull.Value);
            ValoresParametros.Add("AGENTE", DBNull.Value);
            ValoresParametros.Add("FAMILIAMATERIAL", DBNull.Value);
            ValoresParametros.Add("FAMILIA_DESDE", DBNull.Value);
            ValoresParametros.Add("FAMILIA_HASTA", DBNull.Value);
            ValoresParametros.Add("MATERIAL_DESDE", DBNull.Value);
            ValoresParametros.Add("MATERIAL_HASTA", DBNull.Value);

            if (Series?.Any() ?? false)
            {
                /*if (flag)
                    sb.Append(" AND ");

                foreach (var item in Series)
                    ValoresParametros.Add(item, item);

                sb.Append(" i.fkseries in ( " + string.Join(", ", Series.ToArray().Select(f => "@" + f)) + " ) ");*/
                ValoresParametros["SERIES"] = string.Join("', ", Series.ToArray().Select(f => "'" + f));
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.SeriesListado, string.Join(", ", Series.ToArray())));
                flag = true;
            }

            if (FechaDesde.HasValue)
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["FECHA_DESDE"] = FechaDesde.Value.Year+"-"+ FechaDesde.Value.Month+"-"+ FechaDesde.Value.Day;
                //sb.Append(" p.fechadocumento>=@fechadesde ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FechaDesde, FechaDesde));
                flag = true;
            }

            if (FechaHasta.HasValue)
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["FECHA_HASTA"] = FechaHasta.Value.Year + "-" + FechaHasta.Value.Month + "-" + FechaHasta.Value.Day;
                //sb.Append(" p.fechadocumento<=@fechahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FechaHasta, FechaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["CLIENTE_DESDE"] = CuentaDesde;
                //sb.Append(" p.fkclientes>=@cuentadesde ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaDesde, CuentaDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["CLIENTE_HASTA"] = CuentaHasta;
                //sb.Append(" p.fkclientes<=@cuentahasta ");
                Condiciones.Add(string.Format("{0}: {1}", Rcuentas.CuentaHasta, CuentaHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkfamiliacliente))
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["FAMILIACLIENTE"] = Fkfamiliacliente;
                //sb.Append(" p.fkclientes<=@cuentahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkfamiliacliente, Fkfamiliacliente));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkzonacliente))
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["ZONACLIENTE"] = Fkzonacliente;
                //sb.Append(" p.fkclientes<=@cuentahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RClientes.Fkzonacliente, Fkzonacliente));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkAgentes))
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["AGENTE"] = FkAgentes;
                //sb.Append(" p.fkclientes<=@cuentahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RInformeMargen.FkAgentes, FkAgentes));
                flag = true;
            }

            if (!string.IsNullOrEmpty(Fkfamiliasmateriales))
            {
                /*if (flag)
                    sb.Append(" AND ");*/
                AppService = new ApplicationHelper(Context);
                ValoresParametros["FAMILIAMATERIAL"] = Fkfamiliasmateriales;
                //sb.Append("  exists(select mm.* from materiales as mm where mm.id=Substring(pl.fkarticulos,3,3) and mm.fkfamiliamateriales=@fkfamiliasmateriales)  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaMateriales, AppService.GetListFamiliaMateriales().SingleOrDefault(f => f.Valor == Fkfamiliasmateriales).Descripcion));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasDesde))
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["FAMILIA_DESDE"] = FkfamiliasDesde;
                //sb.Append(" Substring(pl.fkarticulos,0,3) >= @fkfamiliasdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaDesde, FkfamiliasDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkfamiliasHasta))
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["FAMILIA_HASTA"] = FkfamiliasHasta;
                //sb.Append(" Substring(pl.fkarticulos,0,3) <= @fkfamiliashasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.FamiliaHasta, FkfamiliasHasta));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesDesde))
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["MATERIAL_DESDE"] = FkmaterialesDesde;
                //sb.Append(" Substring(pl.fkarticulos,3,3) >= @fkmaterialesdesde  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesDesde, FkmaterialesDesde));
                flag = true;
            }

            if (!string.IsNullOrEmpty(FkmaterialesHasta))
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["MATERIAL_HASTA"] = FkmaterialesHasta;
                //sb.Append(" Substring(pl.fkarticulos,3,3) <= @fkmaterialeshasta  ");
                Condiciones.Add(string.Format("{0}: {1}", RAlbaranes.MaterialesHasta, FkmaterialesHasta));
                flag = true;
            }

            ExecuteProcedure(Context, ValoresParametros);           
            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();

            switch (Group)
            {
                case "1":
                    sb.Append("SELECT i.fkArticulosCod as [Cód. Artículo],i.fkArticulosNom as Descripción,Sum(i.cantidad) as Cantidad,"+
                        "Sum(i.metrosCompra) as [Metros Compra], Sum(i.precioTotalCompra) as [Importe Compra],"+
                        "Sum(i.precioTotalCompra) / NULLIF(Sum(i.metrosCompra), 0) as PMC, sum(i.metrosVenta) as [Metros Venta], Sum(i.precioTotalVenta) as [Importe Venta],"+
                        "Sum(i.precioTotalVenta) / NULLIF(sum(i.metrosVenta), 0) as PMV,"+
                        "Sum(i.precioTotalVenta) - Sum(i.precioTotalCompra) as Margen, 100 - ((Sum(i.precioTotalCompra) / NULLIF(Sum(i.precioTotalVenta), 0)) * 100) as [% Margen] "+
                        "FROM InformeMargen as i ");
                    return sb.ToString();
                case "2":
                    sb.Append("SELECT i.fkArticulosCod as [Cód. Artículo],i.fkArticulosNom as Descripción,i.lote as Lote,Sum(i.cantidad) as Cantidad,"+
                        "i.referenciaAlbVenta as [Alb. Venta], Sum(i.metrosCompra) as [Metros Compra], Sum(i.precioTotalCompra) as [Importe Compra],"+
                        "Sum(i.precioTotalCompra) / NULLIF(Sum(i.metrosCompra), 0) as PMC, sum(i.metrosVenta) as [Metros Venta], Sum(i.precioTotalVenta) as [Importe Venta],"+
                        "Sum(i.precioTotalVenta) / NULLIF(sum(i.metrosVenta), 0) as PMV,"+
                        "Sum(i.precioTotalVenta) - Sum(i.precioTotalCompra) as Margen, 100 - ((Sum(i.precioTotalCompra) / NULLIF(Sum(i.precioTotalVenta), 0)) * 100) as [% Margen] "+
                        "FROM InformeMargen as i ");
                    return sb.ToString();
                case "3":
                    sb.Append("SELECT i.fkClientesCod as [Cód. Cliente],i.fkClientesNom as Nombre,Sum(i.cantidad) as Cantidad,"+
                        "Sum(i.metrosCompra) as [Metros Compra], Sum(i.precioTotalCompra) as [Importe Compra],"+
                        "Sum(i.precioTotalCompra) / NULLIF(Sum(i.metrosCompra), 0) as PMC, sum(i.metrosVenta) as [Metros Venta], Sum(i.precioTotalVenta) as [Importe Venta],"+
                        "Sum(i.precioTotalVenta) / NULLIF(sum(i.metrosVenta), 0) as PMV,"+
                        "Sum(i.precioTotalVenta) - Sum(i.precioTotalCompra) as Margen, 100 - ((Sum(i.precioTotalCompra) / NULLIF(Sum(i.precioTotalVenta), 0)) * 100) as [% Margen] "+
                        "FROM InformeMargen as i ");
                    return sb.ToString();
            }

            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var result = string.Empty;

            switch (Group)
            {
                case "1":
                    return " group by i.fkArticulosCod,i.fkArticulosNom order by i.fkArticulosCod";
                case "2":
                    return " group by i.fkArticulosCod,i.lote,i.fkArticulosNom,i.referenciaAlbVenta order by i.fkArticulosCod,i.lote";
                case "3":
                    return " group by i.fkClientesCod, i.fkClientesNom order by i.fkClientesCod";
            }

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

        //Ejecutamos el procedimiento almaacenado en BBDD para carga la tabla InformeMargen con los filtros indicados
        private void ExecuteProcedure(IContextService context, Dictionary<string, object> parametros)
        {
            var dbconnection = "";
            using (var db = MarfilEntities.ConnectToSqlServer(context.BaseDatos))
            {
                dbconnection = db.Database.Connection.ConnectionString;
            }
            using (var con = new SqlConnection(dbconnection))
            {
                con.Open();
                using (var cmd = new SqlCommand("CARGAR_INFORMEMARGEN", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var item in parametros)
                    {
                        cmd.Parameters.AddWithValue(item.Key, item.Value);
                    }

                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
        }
    }
}