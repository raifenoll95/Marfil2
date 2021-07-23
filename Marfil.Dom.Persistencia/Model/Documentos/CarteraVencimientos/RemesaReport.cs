using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Documentos.CarteraVencimientos
{
    class RemesaReport : IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public RemesaReport(IContextService user, string primarykey)
        {
            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";
            var mainQuery = new CustomSqlQuery("Remesas", "SELECT * FROM [Remesas] ");

            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia", typeof(string), primarykey));
                mainQuery.Sql = "SELECT * FROM [Remesas] where empresa=@empresa and referencia=@referencia";
            }

            DataSource.Queries.Add(mainQuery);
            DataSource.Queries.Add(new CustomSqlQuery("Cuentas", "SELECT * FROM [Cuentas]"));

            DataSource.Relations.Add("Remesas", "Cuentas", new[] {
                        new RelationColumnInfo("empresa", "empresa"),
                        new RelationColumnInfo("fkcuentas", "id")});

            DataSource.RebuildResultSchema();
        }
    }
}
