using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Inventarios
{
    

    class InventariosReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public InventariosReport(IContextService user,string primarykey)
        {
            
            
                var server = ConfigurationManager.AppSettings["Server"];
                var usuario = ConfigurationManager.AppSettings["User"];
                var password = ConfigurationManager.AppSettings["Password"];
                DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
                DataSource.Name = "Report";
                var mainQuery = new CustomSqlQuery("Inventarios", "SELECT * FROM [Inventarios] ");
               
                if (!string.IsNullOrEmpty(primarykey))
                {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia",typeof(string),primarykey));
                    mainQuery.Sql = "SELECT * FROM [Inventarios] where empresa=@empresa and referencia=@referencia";
                }
            
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa] FROM [Empresas] as e left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id"));
                DataSource.Queries.Add(mainQuery);
                DataSource.Queries.Add(new CustomSqlQuery("Inventarioslin", "SELECT al.*,u.textocorto as [Unidadesdescripcion] FROM [InventariosLin] as al" +
                                                                          " inner join Familiasproductos as fp on fp.empresa=al.empresa and fp.id=substring(al.fkarticulos,0,3)" +
                                                                          " left join unidades as u on fp.fkunidadesmedida=u.id"));
            
            
            // Create a master-detail relation between the queries.

            DataSource.Relations.Add("Inventarios", "Inventarioslin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkinventarios")});
            

                DataSource.Relations.Add("Inventarios", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});


               

                DataSource.RebuildResultSchema();
                
            
        }
    }
}
