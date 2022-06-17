using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;
using RCobrosYPagos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.ControlsUI.Toolbar;
using Resources;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.Listados
{

    public enum TipoVencimiento
    {
        [StringValue(typeof(RCobrosYPagos), "Cobros")]
        Cobros,
        [StringValue(typeof(RCobrosYPagos), "Pagos")]
        Pagos
    }

    public class ListadosConsultaTesoreria : ListadosModel
    {

        #region CTR

        public ListadosConsultaTesoreria()
        {
        }

        public ListadosConsultaTesoreria(IContextService context) : base(context)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(context.BaseDatos))
            {
                var idEjercicio = Convert.ToInt32(context.Ejercicio);
                InicioEjercicio = db.Ejercicios.Where(f => f.empresa == context.Empresa && f.id == idEjercicio).Select(f => f.desde).SingleOrDefault();
            }
            FechaInforme = DateTime.Today;
        }

        #endregion

        public override string TituloListado => "Consulta de tesorería";
        public override string IdListado => FListadosModel.ConsultaTesoreria;

        #region Propierties        

        [Display(ResourceType = typeof(RMovs), Name = "FechaInforme")]
        public DateTime? FechaInforme { get; set; }

        //Tipo
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Tipo")]
        public TipoVencimiento Tipo { get; set; }

        //Cuenta desde- cuenta hasta
        [Display(ResourceType = typeof(RMovs), Name = "CuentaDesde")]
        public string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "CuentaHasta")]
        public string CuentaHasta { get; set; }

        //Cuenta desde tesoreria- cuenta hasta tesoreria
        [Display(ResourceType = typeof(RMovs), Name = "CuentaDesdeTesoreria")]
        public string CuentaDesdeTesoreria { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "CuentaHastaTesoreria")]
        public string CuentaHastaTesoreria { get; set; }

        //Situacion
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Situación")]
        public string Situacion { get; set; }

        //Fecha desde factura- fecha hasta factura
        [Display(ResourceType = typeof(RMovs), Name = "FechaDesde")]
        public DateTime? FechaDesdeFactura { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "FechaHasta")]
        public DateTime? FechaHastaFactura { get; set; }

        //Fecha desde vencimiento- fecha hasta vencimiento
        [Display(ResourceType = typeof(RMovs), Name = "FechaDesdeVencimiento")]
        public DateTime? FechaDesdeVencimiento { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "FechaHastaVencimiento")]
        public DateTime? FechaHastaVencimiento { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "FormaPago")]
        public int? Fkformaspago { get; set; }

        public DateTime? InicioEjercicio { get; set; }

        #endregion Propierties

        #region Filters


        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            ValoresParametros.Clear();
            Condiciones.Clear();

            Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaInforme, FechaInforme));

            sb.Append("[Empresa] = '" + Empresa + "'");

            if(Tipo == TipoVencimiento.Cobros)
            {
                ValoresParametros.Add("tipo", Tipo);
                sb.Append(" AND ([Tipo] = '0')");
            }

            if (Tipo == TipoVencimiento.Pagos)
            {
                ValoresParametros.Add("tipo", Tipo);
                sb.Append(" AND ([Tipo] = '1')");
            }

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                ValoresParametros.Add("cuentadesde", CuentaDesde);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.CuentaDesde, CuentaDesde));
                sb.Append(" AND ([Tercero]>='" + CuentaDesde + "')");
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                ValoresParametros.Add("cuentahasta", CuentaHasta);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.CuentaHasta, CuentaHasta));
                sb.Append(" AND ([Tercero]<='" + CuentaHasta + "')");
            }

            if (!string.IsNullOrEmpty(CuentaDesdeTesoreria))
            {
                ValoresParametros.Add("cuentadesdetesoreria", CuentaDesdeTesoreria);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.CuentaDesdeTesoreria, CuentaDesdeTesoreria));
                sb.Append(" AND ([Cta.Tesoreria] >='" + CuentaDesdeTesoreria + "')");
            }

            if (!string.IsNullOrEmpty(CuentaHastaTesoreria))
            {
                ValoresParametros.Add("cuentahastatesoreria", CuentaHastaTesoreria);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.CuentaHastaTesoreria, CuentaHastaTesoreria));
                sb.Append(" AND ([Cta.Tesoreria] <='" + CuentaHastaTesoreria + "')");
            }

            if (!string.IsNullOrEmpty(Situacion))
            {
                ValoresParametros.Add("situacion", Situacion);
                Condiciones.Add(string.Format("{0}: {1}", RCobrosYPagos.Situación, Situacion));
                sb.Append(" AND ([Situación] ='" + Situacion + "')");
            }

            if (!string.IsNullOrEmpty(FechaDesdeFactura.ToString()))
            {
                ValoresParametros.Add("fechadesdefactura", FechaDesdeFactura);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaDesde, FechaDesdeFactura.ToString()));
                sb.Append(" AND ([Fecha Factura] >='" + FechaDesdeFactura + "')");
            }

            if (!string.IsNullOrEmpty(FechaHastaFactura.ToString()))
            {
                ValoresParametros.Add("fechahastafactura", FechaHastaFactura);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaHasta, FechaHastaFactura.ToString()));
                sb.Append(" AND ([Fecha Factura] <='" + FechaHastaFactura + "')");
            }

            if (!string.IsNullOrEmpty(FechaDesdeVencimiento.ToString()))
            {
                ValoresParametros.Add("fechadesdevencimiento", FechaDesdeVencimiento);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaDesdeVencimiento, FechaDesdeVencimiento.ToString()));
                sb.Append(" AND ([Fecha Vencimiento] >='" + FechaDesdeVencimiento + "')");
            }

            if (!string.IsNullOrEmpty(FechaHastaVencimiento.ToString()))
            {
                ValoresParametros.Add("fechahastavencimiento", FechaHastaVencimiento);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaHastaVencimiento, FechaHastaVencimiento.ToString()));
                sb.Append(" AND ([Fecha Vencimiento] <='" + FechaHastaVencimiento + "')");
            }

            if (!string.IsNullOrEmpty(Fkformaspago.ToString()))
            {
                ValoresParametros.Add("formapago", Fkformaspago);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.FormaPago, Fkformaspago));
                sb.Append(" AND ([Forma Pago] ='" + Fkformaspago + "')");
            }

            return sb.ToString();
        }

        #endregion Filters

        #region Select

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("select [Tercero], [Descripción], [Factura], [Fecha Factura], [Cta.Tesoreria], [Descripcion Tesoreria], [Importe], [Fecha Vencimiento], [Mes Vto], [Asignado], [Faltan], [Situación], [Descripción Situación], [Forma Pago], [Descripcion Pago], [Observaciones], [Fecha Remesa], [Fecha Pago], [Referencia] from " +
                            "(select * from(select v.empresa as [Empresa], v.tipo as [Tipo], v.fkcuentas as [Tercero], c.descripcion as [Descripción], v.traza as [Factura], " +
                            "v.fechafactura as [Fecha Factura], v.fkcuentatesoreria as [Cta.Tesoreria], cteso.descripcion as [Descripcion Tesoreria], v.importegiro - v.importeasignado as [Importe], " +
                            "v.fechavencimiento as [Fecha Vencimiento], Concat(Convert(varchar(2), FORMAT(v.fechavencimiento, 'MM')), Convert(varchar(2), RIGHT(YEAR(v.fechavencimiento), 2))) as [Mes Vto], " +
                            "Concat(convert(varchar(10), datediff(day, fechafactura, fechavencimiento)), ' ', 'DÍAS') as [Asignado], Concat(convert(varchar(10), datediff(day, GETDATE(), fechavencimiento)), ' ', 'DÍAS') as [Faltan], v.situacion as [Situación], s.descripcion as [Descripción Situación], convert(varchar(10), v.fkformaspago) as [Forma Pago], " +
                            "f.nombre as [Descripcion Pago], v.comentario as [Observaciones], '' as [Fecha Remesa], v.fechapago as [Fecha Pago], v.referencia as [Referencia] from vencimientos as v " +
                            "left join cuentas as c on v.fkcuentas = c.id and v.empresa = c.empresa left join cuentas as cteso on v.fkcuentatesoreria = cteso.id and v.empresa = cteso.empresa left join FormasPago as f on v.fkformaspago = f.id left join SituacionesTesoreria as s on v.situacion = s.cod " +
                            "where v.situacion != 'C') Vencimientos " +
                            "union all " +
                            "(select cart.empresa as [Empresa], cart.tipovencimiento as [Tipo], cart.fkcuentas as [Tercero], c.descripcion as [Descripción], cart.traza as [Factura], cart.fechacreacion as [Fecha Factura], cart.fkcuentastesoreria as [Cta.Tesoreria], " +
                            "cteso.descripcion as [Descripcion Tesoreria], cart.importegiro as [Importe], cart.fechavencimiento as [Fecha Vencimiento], Concat(Convert(varchar(2), FORMAT(cart.fechavencimiento, 'MM')), " +
                            "Convert(varchar(2), RIGHT(YEAR(cart.fechavencimiento), 2))) as [Mes Vto], Concat(convert(varchar(10), datediff(day, fechacreacion, fechavencimiento)), ' ', 'DÍAS') as [Asignado], Concat(convert(varchar(10), datediff(day, GETDATE(), fechavencimiento)), ' ', 'DÍAS') as [Faltan], " +
                            "cart.situacion as [Situación], s.descripcion as [Descripción Situación], convert(varchar(10), cart.fkformaspago) as [Forma Pago], f.nombre as [Descripcion Pago], '' as [Observaciones], cart.fecharemesa as [Fecha Remesa], cart.fechapago as [Fecha Pago], cart.referencia as [Referencia] " +
                            "from CarteraVencimientos as cart left join cuentas as c on cart.fkcuentas = c.id and cart.empresa = c.empresa left join cuentas as cteso on cart.fkcuentastesoreria = cteso.id and cart.empresa = cteso.empresa left join SituacionesTesoreria as s on cart.situacion = s.cod left join FormasPago as f on cart.fkformaspago = f.id)) consulta_tesoreria");

            return sb.ToString();
        }

        #endregion

        #region Orden

        internal override string GenerarOrdenColumnas()
        {
            return " ORDER BY Tercero";
        }

        #endregion
    }
}
