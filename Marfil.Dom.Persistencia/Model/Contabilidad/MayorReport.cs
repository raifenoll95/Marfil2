using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Collections.Generic;
using System;
using DevExpress.XtraReports;

namespace Marfil.Dom.Persistencia.Model.Documentos.Albaranes
{
    class MayorReport : IReport
    {

        public SqlDataSource DataSource { get; private set; }

        public MayorReport(IContextService user, Dictionary<string, object> dictionary = null)
        {

            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";

            var mainQuery = new CustomSqlQuery("Cuentas", "SELECT Cuenta, [Doc.], Fecha, Comentario ,Debe, Haber, Diferencia," +
                //" SUM(saldo) OVER(PARTITION BY Cuenta ORDER BY Fecha, [Doc.], Orden) AS[Saldo]" +
                " saldo AS [Saldo]" +
                " FROM(" +
                "(SELECT (c.id + ' ' + c.descripcion) AS [Cuenta]," +
                " m.Fecha AS[Fecha], m.referencia AS[Doc.]," +
                " l.comentario as [Comentario]," +
                " (CASE WHEN l.esdebe = 1 THEN l.importe ELSE null END) AS[Debe]," +
                " (CASE WHEN l.esdebe = -1 THEN l.importe ELSE null END) AS[Haber]," +
                " (CASE WHEN l.esdebe = 1 THEN l.importe ELSE l.importe * -1 END) AS [Diferencia]," +
                " (CASE WHEN l.esdebe = 1 THEN l.importe ELSE l.importe * -1 END) AS [Saldo]," +
                " l.orden AS [Orden]" +
                " FROM Cuentas AS c" +
                " LEFT JOIN MovsLin AS l ON c.id = l.fkcuentas AND c.empresa = l.empresa" +
                " LEFT JOIN Movs AS m ON m.id = l.fkmovs AND c.empresa = m.empresa " +
                "inner join maes on maes.empresa = l.empresa and maes.fkejercicio = m.fkejercicio and maes.fkcuentas = c.id");

            if (dictionary != null)
            {
                var serie = dictionary["Serie"].ToString();
                var cuentaDesde = dictionary["CuentaDesde"].ToString();
                var cuentaHasta = dictionary["CuentaHasta"].ToString();
                var fechaDesde = dictionary["FechaDesde"];
                var fechaHasta = dictionary["FechaHasta"];
                var saldosAnteriores = Convert.ToBoolean(dictionary["SaldosAnteriores"]);
                var mostrarcuentassinSaldo = Convert.ToBoolean(dictionary["MostrarCuentasSinSaldo"]);
                // var fechaInforme = dictionary["FechaInforme"];


                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Sql += " WHERE c.nivel = 0 AND c.empresa=@empresa and m.fkejercicio = '" + user.Ejercicio + "'";

                if (!string.IsNullOrEmpty(serie))
                {
                    mainQuery.Parameters.Add(new QueryParameter("serie", typeof(string), serie));
                    mainQuery.Sql += " AND m.fkseriescontables=@serie";
                }

                if (!string.IsNullOrEmpty(cuentaDesde))
                {
                    mainQuery.Parameters.Add(new QueryParameter("cuentaDesde", typeof(string), cuentaDesde));
                    mainQuery.Sql += " AND c.id>=@cuentaDesde";
                }

                if (!string.IsNullOrEmpty(cuentaHasta))
                {
                    mainQuery.Parameters.Add(new QueryParameter("cuentaHasta", typeof(string), cuentaHasta));
                    mainQuery.Sql += " AND c.id<=@cuentaHasta";
                }

                if (fechaDesde != null)
                {
                    var desglose = fechaDesde.ToString().Split('/');
                    fechaDesde = desglose[2] + "-" + desglose[1] + "-" + desglose[0];
                    mainQuery.Parameters.Add(new QueryParameter("fechaDesde", typeof(DateTime), fechaDesde));
                    mainQuery.Sql += " AND m.fecha>=@fechaDesde";
                }

                if (fechaHasta != null)
                {
                    var desglose = fechaHasta.ToString().Split('/');
                    fechaHasta = desglose[2] + "-" + desglose[1] + "-" + desglose[0];
                    mainQuery.Parameters.Add(new QueryParameter("fechaHasta", typeof(DateTime), fechaHasta));
                    mainQuery.Sql += " AND m.fecha<=@fechaHasta";
                }

                if (!mostrarcuentassinSaldo)
                {                    
                    mainQuery.Sql += " AND maes.saldo <> 0";
                }

                mainQuery.Sql += ")";

                if (saldosAnteriores)
                {
                    mainQuery.Sql += " UNION ALL";
                    //ValoresParametros.Add("inicioejercicio", InicioEjercicio.Value);
                    mainQuery.Sql += " (SELECT (c.id + ' ' + c.descripcion) AS [Cuenta]," +
                        " NULL AS [Fecha], 'SUMA ANTERIOR' AS [Doc.]," +
                        " NULL AS [Comentario]," +
                        " SUM((CASE WHEN l.esdebe = 1 THEN l.importe ELSE 0 END)) AS [Debe]," +
                        " SUM((CASE WHEN l.esdebe = -1 THEN l.importe ELSE 0 END)) AS [Haber]," +
                        " SUM((CASE WHEN l.esdebe = 1 THEN l.importe ELSE (l.importe * -1) END)) AS [Diferencia]," + 
                        " SUM((CASE WHEN l.esdebe = 1 THEN l.importe ELSE (l.importe * -1) END)) AS [Saldo]," +
                        " 0 AS [Orden]" +
                        " FROM Cuentas AS c" +
                        " LEFT JOIN MovsLin AS l ON c.id = l.fkcuentas AND c.empresa = l.empresa " +
                        " LEFT JOIN Movs AS m ON m.id = l.fkmovs AND c.empresa = m.empresa " +
                        "inner join maes on maes.empresa = l.empresa and maes.fkejercicio = m.fkejercicio and maes.fkcuentas = c.id" + 
                        " WHERE c.nivel = 0 AND c.empresa='" + user.Empresa + "'" +
                        " AND (m.fkseriescontables = @serie OR m.fkseriescontables IS NULL)";

                    if (!string.IsNullOrEmpty(cuentaDesde))
                    {
                        mainQuery.Sql += " AND c.id>=@cuentaDesde";
                    }

                    if (!string.IsNullOrEmpty(cuentaHasta))
                    {
                        mainQuery.Sql += " AND c.id<=@cuentaHasta";
                    }

                    mainQuery.Sql += " AND ((m.Fecha<@fechadesde) OR m.fecha IS NULL and m.fkejercicio = '" + user.Ejercicio + "')";
                    if (!mostrarcuentassinSaldo)
                    {
                        mainQuery.Sql += " AND ";
                        mainQuery.Sql += "maes.saldo <> 0";
                    }
                    mainQuery.Sql += " GROUP BY c.id, c.descripcion)";
                }

                mainQuery.Sql += ")t";

            }else
            {
                mainQuery.Sql += "))t";
            }

            DataSource.Queries.Add(new CustomSqlQuery("Empresa", "SELECT id, nombre FROM Empresas WHERE id = '" + user.Empresa + "'"));
            DataSource.Queries.Add(new CustomSqlQuery("Ejercicios", "SELECT empresa, id, descripcion FROM Ejercicios WHERE id = '" + user.Ejercicio + "'"));
            DataSource.Queries.Add(new CustomSqlQuery("Movs", "SELECT * FROM Movs"));
            DataSource.Queries.Add(new CustomSqlQuery("MovsLin", "SELECT * FROM MovsLin"));

            DataSource.Queries.Add(mainQuery);

            DataSource.Relations.Add("Cuentas", "Empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            DataSource.Relations.Add("Cuentas", "Ejercicios", new[] {
                   new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkejercicio", "id")});

            DataSource.Relations.Add("Movs", "Empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            DataSource.Relations.Add("Movs", "Ejercicios", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkejercicio", "id")});

            DataSource.Relations.Add("Movs", "MovsLin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkmovs")});

            DataSource.RebuildResultSchema();

        }
    }
}
