using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Documentos.DivisionLotes
{
    class DivisionLotesReport : IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public DivisionLotesReport(IContextService user, string primarykey)
        {
            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";
            var mainQuery = new CustomSqlQuery("Divisionlotes", "SELECT * FROM [DivisionLotes] ");

            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referenciaremesa", typeof(string), primarykey));
                mainQuery.Sql = "SELECT * FROM [DivisionLotes] where empresa=@empresa and id=@id";
            }

            DataSource.Queries.Add(mainQuery);
            DataSource.Queries.Add(new CustomSqlQuery("Cuentas", "SELECT * FROM [Cuentas]"));
            DataSource.Queries.Add(new CustomSqlQuery("Divisionlotessalidalin", "SELECT * FROM [Cuentas]"));
            DataSource.Queries.Add(new CustomSqlQuery("Divisionlotesentradalin", "SELECT * FROM [Cuentas]"));
            DataSource.Queries.Add(new CustomSqlQuery("DivisionLotescostesadicionales", "SELECT * FROM [Cuentas]"));


            // cuentas <-> divisionlotes
            DataSource.Relations.Add("Divisionlotes", "Cuentas", new[] { 
                new RelationColumnInfo("empresa", "empresa") ,
                new RelationColumnInfo("fkoperario", "id") });

            // divisionlotes <-> divisionlotessalidalin
            DataSource.Relations.Add("Divisionlotes", "Divisionlotessalidalin", new[] {
                new RelationColumnInfo("empresa", "empresa"),
                new RelationColumnInfo("id", "fkdivisioneslotes") });

            // divisionlotes <-> divisionlotesentradalin
            DataSource.Relations.Add("Divisionlotes", "Divisionlotesentradalin", new[] { 
                new RelationColumnInfo("empresa", "empresa"),
                new RelationColumnInfo("id", "fkdivisioneslotes") });

            // divisionlotes <-> DivisionLotescostesadicionales
            DataSource.Relations.Add("Divisionlotes", "DivisionLotescostesadicionales", new[] {
                new RelationColumnInfo("empresa", "empresa"),
                new RelationColumnInfo("id", "fkdivisioneslotes") });


            DataSource.RebuildResultSchema();
        }
    }
}
