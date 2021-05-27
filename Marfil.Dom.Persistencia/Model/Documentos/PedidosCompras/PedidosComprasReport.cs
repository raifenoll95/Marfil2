using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using System.Configuration;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras
{
    

    class PedidosComprasReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public PedidosComprasReport(IContextService user,string primarykey)
        {
            
            
                var server = ConfigurationManager.AppSettings["Server"];
                var usuario = ConfigurationManager.AppSettings["User"];
                var password = ConfigurationManager.AppSettings["Password"];
                DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
                DataSource.Name = "Report";
                var mainQuery = new CustomSqlQuery("PedidosCompras", "SELECT * FROM [PedidosCompras] ");
                
                if (!string.IsNullOrEmpty(primarykey))
                {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia",typeof(string),primarykey));
                    mainQuery.Sql = "SELECT * FROM [PedidosCompras] where empresa=@empresa and referencia=@referencia";
                }
            DataSource.Queries.Add(new CustomSqlQuery("proveedores", string.Format("SELECT c.*,d.direccion as [Direccionproveedor],d.poblacion as [Poblacionproveedor],d.cp as [Cpproveedor],d.telefono as [Telefonoproveedor] FROM [Clientes] as c left join direcciones as d on d.empresa=c.empresa and d.tipotercero={0} and d.fkentidad=c.fkcuentas", (int)TiposCuentas.Proveedores)));
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa] FROM [Empresas] as e left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id"));
            DataSource.Queries.Add(mainQuery);
                DataSource.Queries.Add(new CustomSqlQuery("PedidosCompraslin", "SELECT pl.*,u.textocorto as [Unidadesdescripcion] FROM [PedidosComprasLin] as pl" +
                    " inner join Familiasproductos as fp on fp.empresa=pl.empresa and fp.id=substring(pl.fkarticulos,0,3)" +
                    "  left join unidades as u on fp.fkunidadesmedida=u.id"));
                DataSource.Queries.Add(new CustomSqlQuery("PedidosComprastotales", "SELECT * FROM [PedidosComprasTotales]"));
             DataSource.Queries.Add(new CustomSqlQuery("Formaspago", "SELECT * FROM [formaspago]"));
            // Create a master-detail relation between the queries.
            DataSource.Relations.Add("PedidosCompras", "PedidosCompraslin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkpedidoscompras")
                    });

                DataSource.Relations.Add("PedidosCompras", "PedidosComprastotales", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkpedidoscompras")});

                DataSource.Relations.Add("PedidosCompras", "proveedores", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkproveedores", "fkcuentas")});

                DataSource.Relations.Add("PedidosCompras", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            DataSource.Relations.Add("PedidosCompras", "Formaspago", new[] {
                    new RelationColumnInfo("fkformaspago", "id")});


            DataSource.RebuildResultSchema();
                
            
        }
    }
}
