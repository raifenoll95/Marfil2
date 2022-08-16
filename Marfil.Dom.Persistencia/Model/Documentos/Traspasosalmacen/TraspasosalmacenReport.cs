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
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa], d.email as [Email], d.web as [Web], d.notas as [Notas], d.defecto as [Defecto], d.tipotercero as [TipoTercero], " +
               "d.fkprovincia as [codProvincia], p.nombre as [nombreProvincia], d.fkpais as [codPais], pa.Descripcion as [nombrePais] " +
               "FROM [Empresas] as e " +
               "left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id " +
               "left join Provincias as p on p.id = d.fkprovincia and p.codigopais = d.fkpais " +
               "left join Paises as pa on pa.valor = d.fkpais and pa.Valor = d.fkpais"));
            DataSource.Queries.Add(new CustomSqlQuery("direcciones", "SELECT * FROM [Direcciones] WHERE defecto = 1"));
            DataSource.Queries.Add(mainQuery);
            DataSource.Queries.Add(new CustomSqlQuery("Traspasosalmacenlin", "SELECT * FROM [TraspasosalmacenLin]"));
            DataSource.Queries.Add(new CustomSqlQuery("unidades", "SELECT id, textocorto AS [Unidadesdescripcion], textocorto2 AS [Unidadesdescripcion2] FROM Unidades"));
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


            // Create a master-detail relation between the queries.
            DataSource.Relations.Add("Traspasosalmacen", "Traspasosalmacenlin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fktraspasosalmacen")});

            DataSource.Relations.Add("Traspasosalmacen", "proveedores", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkproveedores", "fkcuentas")});

            DataSource.Relations.Add("Traspasosalmacen", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            // TRASPASO <-> TRANSPORTISTAS
            DataSource.Relations.Add("Traspasosalmacen", "Transportistas", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fktransportista", "fkcuentas")});

            // TRASPASO <-> OPERADOR TRANSPORTISTAS
            DataSource.Relations.Add("Traspasosalmacen", "OperadorTransportistas", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkoperadortransporte", "fkcuentas")});

            DataSource.Relations.Add("Traspasosalmacen", "direcciones", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkalmacen", "fkentidad")});

            DataSource.Relations.Add("Traspasosalmacenlin", "unidades", new[] {
                    new RelationColumnInfo("fkunidades", "id")});




            DataSource.RebuildResultSchema();


        }
    }
}
