using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras
{


    class AlbaranesComprasReport : IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public AlbaranesComprasReport(IContextService user, string primarykey)
        {


            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";
            var mainQuery = new CustomSqlQuery("AlbaranesCompras", "SELECT * FROM [AlbaranesCompras] ");

            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia", typeof(string), primarykey));
                mainQuery.Sql = "SELECT * FROM [AlbaranesCompras] where empresa=@empresa and referencia=@referencia";
            }
            DataSource.Queries.Add(new CustomSqlQuery("clientes", "SELECT * FROM [Proveedores]"));

            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa], d.email as [Email], d.web as [Web], d.notas as [Notas], d.defecto as [Defecto], d.tipotercero as [TipoTercero], " +
            "d.fkprovincia as [codProvincia], p.nombre as [nombreProvincia], d.fkpais as [codPais], pa.Descripcion as [nombrePais] " +
            "FROM [Empresas] as e " +
            "left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id " +
            "left join Provincias as p on p.id = d.fkprovincia and p.codigopais = d.fkpais " +
            "left join Paises as pa on pa.valor = d.fkpais and pa.Valor = d.fkpais"));

            DataSource.Queries.Add(mainQuery);
            //DataSource.Queries.Add(new CustomSqlQuery("AlbaranesComprasLin", "SELECT * FROM [AlbaranesComprasLin]"));


            // ALBARANESCOMPRASLIN
            DataSource.Queries.Add(new CustomSqlQuery("AlbaranesComprasLin", "SELECT al.*, (al.ancho * 100) AS ancho_cm, (al.largo * 100) AS largo_cm, (al.grueso * 100) AS grueso_cm, u.textocorto as [Unidadesdescripcion], ar.descripcion2, " +
                "sa.cantidadtotal as [CantidadTotal_StockActual], sa.largo as [Largo_StockActual], sa.ancho as [Ancho_StockActual], sa.grueso as [Grueso_StockActual], sa.metros as [Metros_StockActual] " +
                " FROM [AlbaranesComprasLin] as al" +
                " inner join Familiasproductos as fp on fp.empresa=al.empresa and fp.id=substring(al.fkarticulos,0,3)" +
                " left join unidades as u on fp.fkunidadesmedida=u.id" +
                " LEFT JOIN Articulos AS ar ON al.fkarticulos = ar.id" +
                " left join Stockactual sa on al.empresa = sa.empresa AND al.fkarticulos = sa.fkarticulos AND al.lote = sa.lote AND al.tabla = sa.loteid"));


            DataSource.Queries.Add(new CustomSqlQuery("Albaranestotales", "SELECT * FROM [AlbaranesComprasTotales]"));
            //DataSource.Queries.Add(new CustomSqlQuery("StockActual", "SELECT * FROM [Stockactual]"));

            // MONEDAS
            DataSource.Queries.Add(new CustomSqlQuery("Monedas", "SELECT id, descripcion, abreviatura FROM Monedas"));


            // Create a master-detail relation between the queries.
            DataSource.Relations.Add("AlbaranesCompras", "AlbaranesComprasLin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkalbaranes")});

            DataSource.Relations.Add("AlbaranesCompras", "Albaranestotales", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkalbaranes")});

            DataSource.Relations.Add("AlbaranesCompras", "clientes", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkproveedores", "fkcuentas")});

            DataSource.Relations.Add("AlbaranesCompras", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            // ALBARANES <-> MONEDAS
            DataSource.Relations.Add("AlbaranesCompras", "Monedas", new[] {
                        new RelationColumnInfo("fkmonedas", "id")});

            /*DataSource.Relations.Add("AlbaranesComprasLin", "StockActual", new[] {
                        new RelationColumnInfo("empresa", "empresa"),
                        new RelationColumnInfo("fkarticulos", "fkarticulos"),
                        new RelationColumnInfo("lote", "lote"),
                        new RelationColumnInfo("tabla", "loteid")});*/

            DataSource.RebuildResultSchema();


        }
    }
}
