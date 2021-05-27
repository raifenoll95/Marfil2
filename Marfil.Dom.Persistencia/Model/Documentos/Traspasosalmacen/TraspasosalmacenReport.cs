using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen
{


    class TraspasosalmacenReport : IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public TraspasosalmacenReport(IContextService user, string primarykey)
        {


            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";
            var mainQuery = new CustomSqlQuery("Traspasosalmacen", "SELECT *,'' as [Almacenorigen], '' as [Almacendestino],'' as [Zonadestino] FROM [Traspasosalmacen] ");

            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia", typeof(string), primarykey));
                mainQuery.Sql = "SELECT t.*, a.descripcion as [Almacenorigen], a2.descripcion as [Almacendestino], az.descripcion as [Zonadestino] FROM [Traspasosalmacen] as t " +
                                " inner join almacenes as a on a.empresa=t.empresa and a.id=t.fkalmacen " +
                                " inner join almacenes as a2 on a2.empresa = t.empresa and a2.id=t.fkalmacendestino " +
                                " left join almaceneszona as az on az.empresa= a2.empresa and az.fkalmacenes=a2.id and  CONVERT(varchar(3), az.id) =t.fkzonas" +
                                " where t.empresa=@empresa and t.referencia=@referencia";
            }
            DataSource.Queries.Add(new CustomSqlQuery("proveedores", "SELECT * FROM [Proveedores]"));
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT * FROM [Empresas]"));
            DataSource.Queries.Add(mainQuery);
            DataSource.Queries.Add(new CustomSqlQuery("Traspasosalmacenlin", "SELECT * FROM [TraspasosalmacenLin]"));


            // Create a master-detail relation between the queries.
            DataSource.Relations.Add("Traspasosalmacen", "Traspasosalmacenlin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fktraspasosalmacen")});

            DataSource.Relations.Add("Traspasosalmacen", "proveedores", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkproveedores", "fkcuentas")});

            DataSource.Relations.Add("Traspasosalmacen", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});




            DataSource.RebuildResultSchema();


        }
    }
}
