using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Albaranes
{


    class AlbaranesReport : IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public AlbaranesReport(IContextService user, string primarykey)
        {

            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";
            var mainQuery = new CustomSqlQuery("Albaranes", "SELECT * FROM [Albaranes] ");

            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia", typeof(string), primarykey));
                mainQuery.Sql = "SELECT * FROM [Albaranes] where empresa=@empresa and referencia=@referencia";
            }

            // TABLA CLIENTES
            DataSource.Queries.Add(new CustomSqlQuery("clientes", string.Format("SELECT c.*,d.direccion as [Direccioncliente],d.poblacion as [Poblacioncliente],d.cp as [Cpcliente],d.telefono as [Telefonocliente] FROM [Clientes] as c left join direcciones as d on d.empresa=c.empresa and d.tipotercero={0} and d.fkentidad=c.fkcuentas", (int)TiposCuentas.Clientes)));

            // EMPRESA
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa], d.email as [Email], d.web as [Web], d.notas as [Notas], d.defecto as [Defecto], d.tipotercero as [TipoTercero], " +
                "d.fkprovincia as [codProvincia], p.nombre as [nombreProvincia], d.fkpais as [codPais], pa.Descripcion as [nombrePais] " +
                "FROM [Empresas] as e " +
                "left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id " +
                "left join Provincias as p on p.id = d.fkprovincia and p.codigopais = d.fkpais " +
                "left join Paises as pa on pa.valor = d.fkpais and pa.Valor = d.fkpais"));

            DataSource.Queries.Add(mainQuery);

            // ALBARANESLIN
            DataSource.Queries.Add(new CustomSqlQuery("Albaraneslin", "SELECT al.*, (al.ancho * 100) AS ancho_cm, (al.largo * 100) AS largo_cm, (al.grueso * 100) AS grueso_cm, u.textocorto as [Unidadesdescripcion], ar.descripcion2 FROM [AlbaranesLin] as al" +
                                                                      " inner join Familiasproductos as fp on fp.empresa=al.empresa and fp.id=substring(al.fkarticulos,0,3)" +
                                                                      " left join unidades as u on fp.fkunidadesmedida=u.id" +
                                                                      " LEFT JOIN Articulos AS ar ON al.fkarticulos = ar.id"));

            // ALBARANESLIN AGRUPADO
            DataSource.Queries.Add(new CustomSqlQuery("AlbaraneslinAgrupado", "SELECT al.empresa, al.fkalbaranes, al.descripcion, ar.descripcion2, caja, bundle, lote, SUM(cantidad) AS cantidad, al.largo, al.ancho, al.grueso, SUM(metros) AS metros, precio, SUM(importe) AS importe, u.textocorto AS [Unidadesdescripcion]" +
                                                                              " FROM AlbaranesLin AS al" +
                                                                              " INNER JOIN Familiasproductos AS fp ON fp.empresa = al.empresa AND fp.id = substring(al.fkarticulos,0,3)" +
                                                                              " LEFT JOIN unidades AS u ON fp.fkunidadesmedida = u.id" +
                                                                              " LEFT JOIN Articulos AS ar ON al.fkarticulos = ar.id" +
                                                                              " GROUP BY al.empresa, fkalbaranes, al.descripcion, ar.descripcion2, caja, bundle, lote, al.largo, al.ancho, al.grueso, precio, u.textocorto" +
                                                                              " ORDER BY descripcion, caja, bundle"));


            // ALBARANESTOTALES
            DataSource.Queries.Add(new CustomSqlQuery("Albaranestotales", "SELECT * FROM [AlbaranesTotales]"));

            // FORMASPAGO
            DataSource.Queries.Add(new CustomSqlQuery("Formaspago", "SELECT * FROM [formaspago]"));

            // ARTÍCULOS (LL)
            DataSource.Queries.Add(new CustomSqlQuery("Articulos", "SELECT * FROM Articulos"));

            // MONEDAS
            DataSource.Queries.Add(new CustomSqlQuery("Monedas", "SELECT id, descripcion, abreviatura FROM Monedas"));

            // PAÍSES
            DataSource.Queries.Add(new CustomSqlQuery("Paises", "SELECT * FROM Paises"));

            // PUERTOS
            DataSource.Queries.Add(new CustomSqlQuery("Puertos", "SELECT * FROM Puertos"));

            // TRANSPORTISTAS
            DataSource.Queries.Add(new CustomSqlQuery("Transportistas", "select t.*, c.descripcion,c.nif,d.*,p.nombre as NombreProvincia,pa.Descripcion as NombrePais " +
                " from Transportistas as t " +
                " LEFT JOIN Cuentas AS c ON t.empresa = c.empresa and t.fkcuentas = c.id " +
                " LEFT JOIN Direcciones AS d ON c.empresa =  d.empresa and c.id =  d.fkentidad " +
                " LEFT JOIN Paises AS pa ON pa.valor = d.fkpais " +
                " LEFT JOIN Provincias AS p ON p.id = d.fkprovincia and p.codigopais = d.fkpais "));

            // OPERADOR TRANSPORTISTAS
            DataSource.Queries.Add(new CustomSqlQuery("OperadorTransportistas", "select t.*, c.descripcion,c.nif,d.*,p.nombre as NombreProvincia,pa.Descripcion as NombrePais " +
                " from Transportistas as t " +
                " LEFT JOIN Cuentas AS c ON t.empresa = c.empresa and t.fkcuentas = c.id " +
                " LEFT JOIN Direcciones AS d ON c.empresa =  d.empresa and c.id =  d.fkentidad " +
                " LEFT JOIN Paises AS pa ON pa.valor = d.fkpais " +
                " LEFT JOIN Provincias AS p ON p.id = d.fkprovincia and p.codigopais = d.fkpais "));

            ///////////////////////////////////////////////////////////////////////////////////////////////////

            // Create a master-detail relation between the queries.

            // ALBARANES <-> ALBARANESLIN
            DataSource.Relations.Add("Albaranes", "Albaraneslin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkalbaranes")});

            // ALBARANES <-> ALBARANESLINAGRUPADO
            DataSource.Relations.Add("Albaranes", "AlbaraneslinAgrupado", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkalbaranes")});

            // ALBARANES <-> FORMASPAGO
            DataSource.Relations.Add("Albaranes", "Formaspago", new[] {
                    new RelationColumnInfo("fkformaspago", "id")});

            // ALBARANES <-> ALBARANESTOTALES
            DataSource.Relations.Add("Albaranes", "Albaranestotales", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkalbaranes")});

            // ALBARANES <-> CLIENTES
            DataSource.Relations.Add("Albaranes", "clientes", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkclientes", "fkcuentas")});

            // ALBARANES <-> MONEDAS
            DataSource.Relations.Add("Albaranes", "Monedas", new[] {
                    new RelationColumnInfo("fkmonedas", "id")});

            // ALBARANESLIN <-> ARTICULOS (Lluís)
            DataSource.Relations.Add("Albaraneslin", "Articulos", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkarticulos", "id")});

            // ALBARANES <-> EMPRESA
            DataSource.Relations.Add("Albaranes", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            // ALBARANES <-> TRANSPORTISTAS
            DataSource.Relations.Add("Albaranes", "Transportistas", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fktransportista", "fkcuentas")});

            // ALBARANES <-> OPERADOR TRANSPORTISTAS
            DataSource.Relations.Add("Albaranes", "OperadorTransportistas", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkoperadortransporte", "fkcuentas")});

            // FACTURAS <-> PUERTOS
            DataSource.Relations.Add("Albaranes", "Puertos", new[] {
                    new RelationColumnInfo("fkpuertosid", "id")});

            // PUERTOS <-> PAISES
            DataSource.Relations.Add("Puertos", "Paises", new[] {
                    new RelationColumnInfo("fkpaises", "Valor")});


            DataSource.RebuildResultSchema();


        }
    }
}
