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
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.ControlsUI.Toolbar;
using Resources;

namespace Marfil.Dom.Persistencia.Listados
{

    public class ListadosMayor : ListadosModel
    {

        #region CTR

        public ListadosMayor()
        {
        }

        public ListadosMayor(IContextService context) : base(context)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(context.BaseDatos))
            {
                Fkseriescontables = db.SeriesContables.Where(f => f.empresa == context.Empresa && f.tipodocumento == "AST").Select(f => f.id).SingleOrDefault();
                var idEjercicio = Convert.ToInt32(context.Ejercicio);
                InicioEjercicio = db.Ejercicios.Where(f => f.empresa == context.Empresa && f.id == idEjercicio).Select(f => f.desde).SingleOrDefault();
                FechaInforme = DateTime.Today;
            }
        }

        #endregion

        public override string TituloListado => "Listado mayor";
        public override string IdListado => FListadosModel.Mayor;

        #region Propierties

        [Display(ResourceType = typeof(RMovs), Name = "FechaInforme")]
        public DateTime? FechaInforme { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Fkseriescontables")]
        public string Fkseriescontables { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Descripcionasiento")]
        public string Descripcionasiento { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "CuentaDesde")]
        public string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "CuentaHasta")]
        public string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "SaldosAnteriores")]
        public bool SaldosAnteriores { get; set; }

        public DateTime? InicioEjercicio { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "MostrarCuentasSinSaldo")]
        public bool MostrarCuentasSinSaldo { get; set; }

        #endregion Propierties

        #region Filters

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.Append(" c.nivel = 0");
            sb.Append(" AND ");
            sb.Append(" c.empresa = '" + Empresa + "' ");

            Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaInforme, FechaInforme));

            if (!string.IsNullOrEmpty(Fkseriescontables))
            {
                sb.Append(" AND ");

                ValoresParametros.Add("fkserie", Fkseriescontables);
                sb.Append(" (m.fkseriescontables = @fkserie OR m.fkseriescontables IS NULL)");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.Fkseriescontables, Fkseriescontables));
            }

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                sb.Append(" AND ");
                ValoresParametros.Add("cuentadesde", CuentaDesde);
                sb.Append(" c.id>=@cuentadesde ");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.CuentaDesde, CuentaDesde));
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                sb.Append(" AND ");
                ValoresParametros.Add("cuentahasta", CuentaHasta);
                sb.Append(" c.id<=@cuentahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.CuentaHasta, CuentaHasta));
            }

            if (FechaDesde.HasValue && FechaHasta.HasValue)
            {
                sb.Append(" AND ");
                ValoresParametros.Add("fechadesde", FechaDesde.Value);
                ValoresParametros.Add("fechahasta", FechaHasta.Value);
                sb.Append(" ((m.fecha>=@fechadesde AND m.fecha<=@fechahasta) OR m.fecha IS NULL)");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaDesde, FechaDesde));
                Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaHasta, FechaHasta));
            }

            //if (FechaHasta.HasValue)
            //{
            //    sb.Append(" AND ");
            //    ValoresParametros.Add("fechahasta", FechaHasta.Value);
            //    sb.Append(" m.fecha<=@fechahasta ");
            //    Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaHasta, FechaHasta));
            //}

            if (!MostrarCuentasSinSaldo)
            {
                sb.Append(" AND ");
                sb.Append(" Saldo IS NOT NULL ");
            }

            sb.Append(")");

            //if(SaldosAnteriores)

            sb.Append(" UNION ALL");
            ValoresParametros.Add("inicioejercicio", InicioEjercicio.Value);
            sb.Append(" (SELECT (c.id + ' ' + c.descripcion) AS [Cuenta]," +
                " NULL AS [Fecha], 'SUMA ANTERIOR' AS [Doc.]," +
                " NULL AS [Comentario]," +
                " SUM((CASE WHEN l.esdebe = 1 THEN l.importe ELSE 0 END)) AS [Debe]," +
                " SUM((CASE WHEN l.esdebe = -1 THEN l.importe ELSE 0 END)) AS [Haber]," +
                " SUM((CASE WHEN l.esdebe = 1 THEN l.importe ELSE (l.importe * -1) END)) AS [Saldo]," +
                " 0 AS [Orden]" +
                " FROM Cuentas AS c" +
                " LEFT JOIN MovsLin AS l ON c.id = l.fkcuentas AND c.empresa = l.empresa " +
                " LEFT JOIN Movs AS m ON m.id = l.fkmovs AND c.empresa = m.empresa " +
                "inner join maes on maes.empresa = l.empresa and maes.fkejercicio = m.fkejercicio and maes.fkcuentas = c.id" +
                " WHERE c.nivel = 0 AND c.empresa='" + Empresa + "'" +
                " AND (m.fkseriescontables = @fkserie OR m.fkseriescontables IS NULL)");

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                sb.Append(" AND c.id>=@cuentaDesde");
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {
                sb.Append(" AND c.id<=@cuentaHasta");
            }

            sb.Append(" AND ((m.Fecha>=@inicioejercicio AND m.Fecha<@fechadesde) OR m.fecha IS NULL)");
            if (!MostrarCuentasSinSaldo)
            {
                sb.Append(" AND ");
                sb.Append(" Saldo IS NOT NULL ");
            }
            sb.Append(" GROUP BY c.id, c.descripcion)");


            sb.Append(")t");

            return sb.ToString();
        }

        #endregion Filters

        #region Select

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(
                "SELECT Cuenta, [Doc.], Comentario, Fecha, Debe, Haber," +
                " SUM(saldo) OVER(PARTITION BY Cuenta ORDER BY Fecha, [Doc.], Orden) AS[Saldo]" +
                " FROM(" +
                " (SELECT (c.id + ' ' + c.descripcion) AS [Cuenta]," +
                " m.Fecha AS[Fecha], m.referencia AS[Doc.]," +
                " l.comentario AS [Comentario]," +
                " (CASE WHEN l.esdebe = 1 THEN l.importe ELSE null END) AS[Debe]," +
                " (CASE WHEN l.esdebe = -1 THEN l.importe ELSE null END) AS[Haber]," +
                " (CASE WHEN l.esdebe = 1 THEN l.importe ELSE l.importe * -1 END) AS [Saldo]," +
                " l.orden AS [Orden]" +
                " FROM Cuentas AS c" +
                " LEFT JOIN MovsLin AS l ON c.id = l.fkcuentas AND c.empresa = l.empresa" +
                " LEFT JOIN Movs AS m ON m.id = l.fkmovs AND c.empresa = m.empresa " +
                "inner join maes on maes.empresa = l.empresa and maes.fkejercicio = m.fkejercicio and maes.fkcuentas = c.id");

            return sb.ToString();
        }

        #endregion

        #region Orden

        internal override string GenerarOrdenColumnas()
        {
            return "ORDER BY Cuenta, Fecha, [Doc.], Orden ";
        }

        #endregion

        //public DocumentosBotonImprimirModel GetListFormatos()
        //{
        //    //var user = Context;
        //    //using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
        //    //{
        //    //    var servicePreferencias = new PreferenciasUsuarioService(db);
        //    //    var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.Asientos.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
        //    //    var service = new DocumentosUsuarioService(db);
        //    //    {
        //    //        var lst =
        //    //            service.GetDocumentos(TipoDocumentoImpresion.Asientos, user.Id)
        //    //                .Where(f => f.Tiporeport == TipoReport.Report);
        //    //        return new DocumentosBotonImprimirModel()
        //    //        {
        //    //            Tipo = TipoDocumentoImpresion.Asientos,
        //    //            Lineas = lst.Select(f => f.Nombre),
        //    //            //Primarykey = Referencia,
        //    //            Defecto = doc?.Name
        //    //        };
        //    //    }

        //    List<string> lista = new List<string>();
        //    lista.Add("Prueba");
        //    return new DocumentosBotonImprimirModel()
        //    {
        //        Tipo = TipoDocumentoImpresion.Asientos,
        //        Lineas = lista,
        //        Primarykey = "Referencia",
        //        Defecto = "Defecto"
        //    };        

        //}
    }
}
