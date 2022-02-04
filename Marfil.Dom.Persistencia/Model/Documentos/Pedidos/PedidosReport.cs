using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Configuration;

namespace Marfil.Dom.Persistencia.Model.Documentos.Pedidos
{
    class PedidosReport : IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public PedidosReport(IContextService user, string primarykey)
        {
            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";
            var mainQuery = new CustomSqlQuery("pedidos", "SELECT * FROM [pedidos] ");

            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia", typeof(string), primarykey));
                mainQuery.Sql = "SELECT * FROM [pedidos] where empresa=@empresa and referencia=@referencia";
            }

            DataSource.Queries.Clear();

            DataSource.Queries.Add(new CustomSqlQuery("clientes", string.Format("SELECT c.*,d.direccion as [Direccioncliente],d.poblacion as [Poblacioncliente],d.cp as [Cpcliente],d.telefono as [Telefonocliente] FROM [Clientes] as c left join direcciones as d on d.empresa=c.empresa and d.tipotercero={0} and d.fkentidad=c.fkcuentas", (int)TiposCuentas.Clientes)));
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa], d.email as [Email], d.web as [Web], d.notas as [Notas], d.defecto as [Defecto], d.tipotercero as [TipoTercero] FROM [Empresas] as e left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id"));
            DataSource.Queries.Add(mainQuery);
            DataSource.Queries.Add(new CustomSqlQuery("pedidoslinprueba", "SELECT pl.*,un.textocorto as [Unidadesdescripcion] FROM [PedidosLin] as pl" +
                    " inner join Familiasproductos as fp on fp.empresa=pl.empresa and fp.id=substring(pl.fkarticulos,0,3)" +
                    "  left join unidades as un on fp.fkunidadesmedida=un.id "));
            DataSource.Queries.Add(new CustomSqlQuery("pedidostotales", "SELECT * FROM [PedidosTotales]"));
            DataSource.Queries.Add(new CustomSqlQuery("Formaspago", "SELECT * FROM [formaspago]"));


            //Elena. Cambio 20/04/2021
            DataSource.Queries.Add(new CustomSqlQuery("Monedas", "SELECT * FROM [Monedas]"));

            DataSource.Queries.Add(new CustomSqlQuery("Direcciones", "SELECT d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa], d.email as [Email], d.web as [Web], d.notas as [Notas], d.defecto as [Defecto], d.tipotercero as [TipoTercero] FROM [Direcciones] as d where d.tipotercero=-1"));

            // Create a master-detail relation between the queries.
            DataSource.Relations.Add("pedidos", "pedidoslinprueba", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkpedidos")
                    });

            DataSource.Relations.Add("pedidos", "pedidostotales", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkpedidos")});

            DataSource.Relations.Add("pedidos", "clientes", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkclientes", "fkcuentas")});

            DataSource.Relations.Add("pedidos", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            DataSource.Relations.Add("pedidos", "Formaspago", new[] {
                    new RelationColumnInfo("fkformaspago", "id")});

            DataSource.Relations.Add("pedidos", "Monedas", new[] {
                    new RelationColumnInfo("fkmonedas", "id")});

            DataSource.Relations.Add("pedidos", "Direcciones", new[] {
                    new RelationColumnInfo("empresa", "empresa")});

            DataSource.RebuildResultSchema();
        }
    }
}
