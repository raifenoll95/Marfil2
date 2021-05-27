using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Kit
{
    

    class KitReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public KitReport(IContextService user,string primarykey)
        {
            
            
                var server = ConfigurationManager.AppSettings["Server"];
                var usuario = ConfigurationManager.AppSettings["User"];
                var password = ConfigurationManager.AppSettings["Password"];
                DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
                DataSource.Name = "Report";
                var mainQuery = new CustomSqlQuery("Kit", "SELECT *,'' as [ZonaDescripcion],'' as [OperarioDescripcion],''  as [AlmacenDescripcion] FROM [Kit] ");
               
                if (!string.IsNullOrEmpty(primarykey))
                {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia",typeof(string),primarykey));
                    mainQuery.Sql = "SELECT k.*,a.descripcion as [AlmacenDescripcion], az.descripcion as [ZonaDescripcion],c.descripcion as [OperarioDescripcion] FROM [Kit]  as k " +
                                    " inner join almacenes as a on a.empresa=k.empresa and a.id=k.fkalmacen " +
                                    " left join almaceneszona as  az on az.empresa=a.empresa and az.fkalmacenes=a.id and az.id=k.fkzonalamacen " +
                                    " left join cuentas as c on c.empresa=k.empresa and c.id=k.fkoperarios " +
                                    " where k.empresa=@empresa and k.id=@referencia";
                }
            
            
                DataSource.Queries.Add(mainQuery);
                DataSource.Queries.Add(new CustomSqlQuery("Kitlin", "SELECT * FROM [KitLin]"));
            
            

                DataSource.Relations.Add("Kit", "Kitlin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkkit")});

                DataSource.RebuildResultSchema();
                
            
        }
    }
}
