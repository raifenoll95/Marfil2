using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.FacturasCompras
{


    class FacturasComprasReport : IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public FacturasComprasReport(IContextService user, string primarykey)
        {


            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";
            var mainQuery = new CustomSqlQuery("FacturasCompras", "SELECT * FROM [FacturasCompras] ");

            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia", typeof(string), primarykey));
                mainQuery.Sql = "SELECT * FROM [FacturasCompras] where empresa=@empresa and referencia=@referencia";
            }
            DataSource.Queries.Add(new CustomSqlQuery("proveedores", string.Format("SELECT c.*,d.direccion as [Direccionproveedor],d.poblacion as [Poblacionproveedor],d.cp as [Cpproveedor],d.telefono as [Telefonoproveedor] FROM [Proveedores] as c left join direcciones as d on d.empresa=c.empresa and d.tipotercero={0} and d.fkentidad=c.fkcuentas", (int)TiposCuentas.Proveedores)));
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa], d.email as [Email], d.web as [Web], d.notas as [Notas], d.defecto as [Defecto], d.tipotercero as [TipoTercero], " +
                                      "d.fkprovincia as [codProvincia], p.nombre as [nombreProvincia], d.fkpais as [codPais], pa.Descripcion as [nombrePais] " +
                                      "FROM [Empresas] as e " +
                                      "left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id " +
                                      "left join Provincias as p on p.id = d.fkprovincia and p.codigopais = d.fkpais " +
                                      "left join Paises as pa on pa.valor = d.fkpais and pa.Valor = d.fkpais"));
            DataSource.Queries.Add(mainQuery);
            DataSource.Queries.Add(new CustomSqlQuery("FacturasCompraslin", "SELECT fl.*,u.textocorto as [Unidadesdescripcion] FROM [FacturasComprasLin] as fl  " +
                                                                    " inner join Familiasproductos as fp on fp.empresa=fl.empresa and fp.id=substring(fl.fkarticulos,0,3)" +
                                                                     " left join unidades as u on fp.fkunidadesmedida=u.id"));
            DataSource.Queries.Add(new CustomSqlQuery("FacturasComprastotales", "SELECT * FROM [FacturasComprasTotales]"));
            DataSource.Queries.Add(new CustomSqlQuery("FacturasComprasvencimientos", "SELECT * FROM [FacturasComprasvencimientos]"));
            DataSource.Queries.Add(new CustomSqlQuery("Formaspago", "SELECT * FROM [formaspago]"));
            // MONEDAS
            DataSource.Queries.Add(new CustomSqlQuery("Monedas", "SELECT id, descripcion, abreviatura FROM Monedas"));

            // Create a master-detail relation between the queries.
            DataSource.Relations.Add("FacturasCompras", "FacturasCompraslin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkfacturascompras")
                    });

            DataSource.Relations.Add("FacturasCompras", "FacturasComprastotales", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkfacturascompras")});

            DataSource.Relations.Add("FacturasCompras", "FacturasComprasvencimientos", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkfacturascompras")});

            DataSource.Relations.Add("FacturasCompras", "proveedores", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkproveedores", "fkcuentas")});
             
            DataSource.Relations.Add("FacturasCompras", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            DataSource.Relations.Add("FacturasCompras", "Monedas", new[] {
                        new RelationColumnInfo("fkmonedas", "id")});

            DataSource.Relations.Add("FacturasCompras", "Formaspago", new[] {
                    new RelationColumnInfo("fkformaspago", "id")});

            DataSource.RebuildResultSchema();


        }
    }
}
