using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.PresupuestosCompras
{
    

    class PresupuestosComprasReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public PresupuestosComprasReport(IContextService user,string primarykey)
        {
            
            
                var server = ConfigurationManager.AppSettings["Server"];
                var usuario = ConfigurationManager.AppSettings["User"];
                var password = ConfigurationManager.AppSettings["Password"];
                DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password,MsSqlAuthorizationType.SqlServer));
                DataSource.Name = "Report";
                var mainQuery = new CustomSqlQuery("PresupuestosCompras", "SELECT * FROM [PresupuestosCompras] ");
                if (!string.IsNullOrEmpty(primarykey))
                {
                    mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                    mainQuery.Parameters.Add(new QueryParameter("referencia",typeof(string),primarykey));
                    mainQuery.Sql = "SELECT * FROM [PresupuestosCompras] where empresa=@empresa and referencia=@referencia";
                }
            DataSource.Queries.Add(new CustomSqlQuery("proveedores", string.Format("SELECT c.*,d.direccion as [Direccionproveedor],d.poblacion as [Poblacionproveedor],d.cp as [Cpproveedor],d.telefono as [Telefonoproveedor] FROM [Clientes] as c left join direcciones as d on d.empresa=c.empresa and d.tipotercero={0} and d.fkentidad=c.fkcuentas", (int)TiposCuentas.Clientes)));
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa] FROM [Empresas] as e left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id"));
            DataSource.Queries.Add(mainQuery);
                DataSource.Queries.Add(new CustomSqlQuery("PresupuestosCompraslin", "SELECT pr.*,u.textocorto as [Unidadesdescripcion] FROM [PresupuestosComprasLin] as pr" +
                     " inner join Familiasproductos as fp on fp.empresa=pr.empresa and fp.id=substring(pr.fkarticulos,0,3)" +
                    " left join unidades as u on fp.fkunidadesmedida=u.id"));
                DataSource.Queries.Add(new CustomSqlQuery("PresupuestosComprastotales", "SELECT * FROM [PresupuestosComprasTotales]"));
            DataSource.Queries.Add(new CustomSqlQuery("Formaspago", "SELECT * FROM [formaspago]"));
            // Create a master-detail relation between the queries.
            DataSource.Relations.Add("PresupuestosCompras", "PresupuestosCompraslin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkpresupuestoscompras")});

                DataSource.Relations.Add("PresupuestosCompras", "PresupuestosComprastotales", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkpresupuestoscompras")});

                DataSource.Relations.Add("PresupuestosCompras", "proveedores", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkproveedores", "fkcuentas")});

                DataSource.Relations.Add("PresupuestosCompras", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});


            DataSource.Relations.Add("PresupuestosCompras", "Formaspago", new[] {
                    new RelationColumnInfo("fkformaspago", "id")});

            DataSource.RebuildResultSchema();
                
            
        }
    }
}
