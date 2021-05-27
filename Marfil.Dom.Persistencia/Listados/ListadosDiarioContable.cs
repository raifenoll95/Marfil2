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

    public class ListadosDiarioContable: ListadosModel
    {

        #region CTR

        public ListadosDiarioContable()
        {            
        }

        public ListadosDiarioContable(IContextService context) : base(context)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(context.BaseDatos))
            {
                Fkseriescontables = db.SeriesContables.Where(f => f.empresa == context.Empresa && f.tipodocumento == "AST" && f.fkejercicios == context.Ejercicio).Select(f => f.id).SingleOrDefault();
            }
            FechaInforme = DateTime.Today;             
        }

        #endregion

        public override string TituloListado => "Listado diario contable";
        public override string IdListado => FListadosModel.DiarioContable;

        #region Propierties

        [Display(ResourceType = typeof(RMovs), Name = "FechaInforme")]
        public DateTime? FechaInforme { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Fkseriescontables")]
        public string Fkseriescontables { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Descripcionasiento")]
        public string Descripcionasiento { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "DocumentoDesde")]
        public string DocumentoDesde { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "DocumentoHasta")]
        public string DocumentoHasta { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }      

        [Display(ResourceType = typeof(RMovs), Name = "TipoAsiento")]
        public string Tipoasiento { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "Canalcontable")]
        public string Canalcontable { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "SumaAnteriorDebe")]
        public float? SumaAnteriorDebe { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "SumaAnteriorHaber")]
        public float? SumaAnteriorHaber { get; set; }        

        #endregion Propierties

        #region Filters

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            ValoresParametros.Clear();
            Condiciones.Clear();
            sb.Append(" m.empresa = '" + Empresa + "' ");

            Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaInforme, FechaInforme));

            if (!string.IsNullOrEmpty(Fkseriescontables))
            {
                sb.Append(" AND ");

                ValoresParametros.Add("fkserie", Fkseriescontables);
                sb.Append(" m.fkseriescontables = @fkserie ");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.Fkseriescontables, Fkseriescontables));
            }

            if (!string.IsNullOrEmpty(DocumentoDesde))
            {
                sb.Append(" AND ");
                ValoresParametros.Add("documentodesde", DocumentoDesde);
                sb.Append(" m.referencia>=@documentodesde ");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.DocumentoDesde, DocumentoDesde));
            }

            if (!string.IsNullOrEmpty(DocumentoHasta))
            {
                sb.Append(" AND ");
                ValoresParametros.Add("documentohasta", DocumentoHasta);
                sb.Append(" m.referencia<=@documentohasta ");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.DocumentoHasta, DocumentoHasta));
            }

            if (FechaDesde.HasValue)
            {
                sb.Append(" AND ");
                ValoresParametros.Add("fechadesde", FechaDesde.Value);
                sb.Append(" m.fecha>=@fechadesde ");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaDesde, FechaDesde));
            }

            if (FechaHasta.HasValue)
            {
                sb.Append(" AND ");
                ValoresParametros.Add("fechahasta", FechaHasta.Value);
                sb.Append(" m.fecha<=@fechahasta ");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaHasta, FechaHasta));
            }

            if (!string.IsNullOrEmpty(Tipoasiento))
            {
                sb.Append(" AND ");
                ValoresParametros.Add("Tipoasiento", Tipoasiento);
                sb.Append(" m.tipoasiento = @Tipoasiento  ");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.TipoAsiento, Tipoasiento));
            }

            if (!string.IsNullOrEmpty(Canalcontable))
            {
                sb.Append(" AND ");
                ValoresParametros.Add("Canalcontable", Canalcontable);
                sb.Append(" m.canalcontable = @Canalcontable ");
                Condiciones.Add(string.Format("{0}: {1}", RMovs.Canalcontable, Canalcontable));
            }

            sb.Append(")");

            if (SumaAnteriorDebe > 0 || SumaAnteriorHaber > 0)
            {
                if (SumaAnteriorDebe == null)
                    SumaAnteriorDebe = 0;
                if (SumaAnteriorHaber == null)
                    SumaAnteriorHaber = 0;

                sb.Append(" UNION ");
                ValoresParametros.Add("Sumaanteriordebe", SumaAnteriorDebe);
                ValoresParametros.Add("Sumaanteriorhaber", SumaAnteriorHaber);
                sb.Append(" (SELECT '' AS [Doc.], NULL AS[Fecha], '' AS[Cuenta Debe], '' AS[Cuenta Haber], '' AS[Descripcion], ");
                sb.Append(" 'SUMA ANTERIOR' AS [Comentario], @Sumaanteriordebe AS [Debe], @Sumaanteriorhaber AS [Haber], 0 AS [Orden])");
            }

            sb.Append(")t");

            return sb.ToString();
        }

        #endregion Filters

        #region Select

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();

            sb.AppendFormat(
                "SELECT [Doc.], Fecha, [Cuenta Debe], [Cuenta Haber], Descripcion, Comentario, Debe, Haber" +
                " FROM(" +
                " (SELECT m.referencia AS [Doc.], m.fecha AS [Fecha]," +
                " (CASE WHEN l.esdebe = 1 THEN l.fkcuentas ELSE '' END) AS [Cuenta Debe]," +
                " (CASE WHEN l.esdebe = -1 THEN l.fkcuentas ELSE '' END) AS [Cuenta Haber]," +
                " c.descripcion AS [Descripcion], l.comentario AS [Comentario]," +
                " (CASE WHEN l.esdebe = 1 THEN l.importe ELSE NULL END) AS [Debe]," +
                " (CASE WHEN l.esdebe = -1 THEN l.importe ELSE NULL END) AS [Haber]," +
                " l.orden as [Orden]" +
                " FROM Movs AS m" +
                " INNER JOIN Movslin AS l ON m.empresa = l.empresa AND m.id = l.fkmovs" +
                " INNER JOIN Cuentas AS c ON m.empresa = c.empresa AND c.id = l.fkcuentas");
            return sb.ToString();
        }

        #endregion

        #region Orden

        internal override string GenerarOrdenColumnas()
        {
            return "ORDER BY [Doc.], Orden ";
        }

        #endregion
     
    }
}
