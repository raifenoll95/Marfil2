using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Bundle
{
    

    class BundleReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public BundleReport(IContextService user,string primarykey)
        {
            
            
                var server = ConfigurationManager.AppSettings["Server"];
                var usuario = ConfigurationManager.AppSettings["User"];
                var password = ConfigurationManager.AppSettings["Password"];
                DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
                DataSource.Name = "Report";
                var mainQuery = new CustomSqlQuery("Bundle", "SELECT *,'' as [ZonaDescripcion],'' as [OperarioDescripcion],''  as [AlmacenDescripcion] FROM [Bundle] ");
               
                if (!string.IsNullOrEmpty(primarykey))
                {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia",typeof(string),primarykey));
                    mainQuery.Sql = "SELECT k.*,a.descripcion as [AlmacenDescripcion], az.descripcion as [ZonaDescripcion],c.descripcion as [OperarioDescripcion] FROM [Bundle]  as k " +
                                    " inner join almacenes as a on a.empresa=k.empresa and a.id=k.fkalmacen " +
                                    " left join almaceneszona as  az on az.empresa=a.empresa and az.fkalmacenes=a.id and az.id=k.fkzonaalmacen " +
                                    " left join cuentas as c on c.empresa=k.empresa and c.id=k.fkoperarios " +
                                    " where k.empresa=@empresa and concat(k.lote,k.id)=@referencia";
                }
            
            
                DataSource.Queries.Add(mainQuery);
                DataSource.Queries.Add(new CustomSqlQuery("Bundlelin", "SELECT * FROM [BundleLin]"));
            
            

                DataSource.Relations.Add("Bundle", "Bundlelin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("lote", "fkbundlelote"),
                    new RelationColumnInfo("id", "fkbundle")});

                DataSource.RebuildResultSchema();
                
            
        }
    }
}
