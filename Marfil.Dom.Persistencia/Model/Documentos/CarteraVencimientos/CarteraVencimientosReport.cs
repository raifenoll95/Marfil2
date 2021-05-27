using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Kit
{
    class CarteraVencimientosReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public CarteraVencimientosReport(IContextService user,string primarykey)
        {
            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";
            var mainQuery = new CustomSqlQuery("CarteraVencimientos", "SELECT * FROM [CarteraVencimientos] ");
            
            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia",typeof(string),primarykey));
                mainQuery.Sql = "SELECT * FROM [CarteraVencimientos] where empresa=@empresa and referencia=@referencia";
            }
             
            DataSource.Queries.Add(mainQuery);
            DataSource.Queries.Add(new CustomSqlQuery("Cuentas", "SELECT * FROM [Cuentas]"));
            
            DataSource.Relations.Add("CarteraVencimientos", "Cuentas", new[] {
                        new RelationColumnInfo("empresa", "empresa"),
                        new RelationColumnInfo("fkcuentas", "id")});

            DataSource.RebuildResultSchema();
        }
    }
}
