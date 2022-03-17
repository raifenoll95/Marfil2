using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Reservasstock
{
    

    class ReservasstockReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public ReservasstockReport(IContextService user,string primarykey)
        {
            
            
                var server = ConfigurationManager.AppSettings["Server"];
                var usuario = ConfigurationManager.AppSettings["User"];
                var password = ConfigurationManager.AppSettings["Password"];
                DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
                DataSource.Name = "Report";
                var mainQuery = new CustomSqlQuery("Reservasstock", "SELECT * FROM [Reservasstock] ");
               
                if (!string.IsNullOrEmpty(primarykey))
                {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia",typeof(string),primarykey));
                    mainQuery.Sql = "SELECT * FROM [Reservasstock] where empresa=@empresa and referencia=@referencia";
                }
            DataSource.Queries.Add(new CustomSqlQuery("clientes", string.Format("SELECT c.*,d.direccion as [Direccioncliente],d.poblacion as [Poblacioncliente],d.cp as [Cpcliente],d.telefono as [Telefonocliente] FROM [Clientes] as c left join direcciones as d on d.empresa=c.empresa and d.tipotercero={0} and d.fkentidad=c.fkcuentas", (int)TiposCuentas.Clientes)));
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa], d.email as [Email], d.web as [Web], d.notas as [Notas], d.defecto as [Defecto], d.tipotercero as [TipoTercero], " +
                "d.fkprovincia as [codProvincia], p.nombre as [nombreProvincia], d.fkpais as [codPais], pa.Descripcion as [nombrePais] " +
                "FROM [Empresas] as e " +
                "left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id " +
                "left join Provincias as p on p.id = d.fkprovincia and p.codigopais = d.fkpais " +
                "left join Paises as pa on pa.valor = d.fkpais and pa.Valor = d.fkpais"));
                DataSource.Queries.Add(mainQuery);
                DataSource.Queries.Add(new CustomSqlQuery("Reservasstocklin", "SELECT al.*,u.textocorto as [Unidadesdescripcion] FROM [ReservasstockLin] as al" +
                                                                          " inner join Familiasproductos as fp on fp.empresa=al.empresa and fp.id=substring(al.fkarticulos,0,3)" +
                                                                          " left join unidades as u on fp.fkunidadesmedida=u.id"));
                DataSource.Queries.Add(new CustomSqlQuery("Reservasstocktotales", "SELECT * FROM [ReservasstockTotales]"));
                DataSource.Queries.Add(new CustomSqlQuery("Formaspago", "SELECT * FROM [formaspago]"));

            // BANCOS MANDATOS
            DataSource.Queries.Add(new CustomSqlQuery("BancosMandatos", "SELECT * FROM BancosMandatos"));

            // PROVINCIAS
            DataSource.Queries.Add(new CustomSqlQuery("Provincias", "SELECT * FROM Provincias"));

            // PAÍSES
            DataSource.Queries.Add(new CustomSqlQuery("Paises", "SELECT Valor, Descripcion, Descripcion2 FROM Paises"));

            // PUERTOS
            DataSource.Queries.Add(new CustomSqlQuery("Puertos", "SELECT * FROM Puertos"));

            // MONEDAS
            DataSource.Queries.Add(new CustomSqlQuery("Monedas", "SELECT id, descripcion, abreviatura FROM Monedas"));

            // Create a master-detail relation between the queries.

            DataSource.Relations.Add("Reservasstock", "Reservasstocklin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkreservasstock")});

            // RESERVAS <-> BANCOS MANDATOS
            DataSource.Relations.Add("Reservasstock", "BancosMandatos", new[] {
                    new RelationColumnInfo("fkcuentastesoreria", "fkcuentas")});

            DataSource.Relations.Add("Reservasstock", "Formaspago", new[] {
                    new RelationColumnInfo("fkformaspago", "id")});

            DataSource.Relations.Add("Reservasstock", "Reservasstocktotales", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkreservasstock")});

                DataSource.Relations.Add("Reservasstock", "clientes", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkclientes", "fkcuentas")});

                DataSource.Relations.Add("Reservasstock", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            // BANCOS MANDATOS <-> PROVINCIAS
            DataSource.Relations.Add("BancosMandatos", "Provincias", new[] {
                    new RelationColumnInfo("fkprovincias", "id")});

            // PROVINCIAS <-> PAISES
            DataSource.Relations.Add("Provincias", "Paises", new[] {
                    new RelationColumnInfo("codigopais", "Valor")});

            // RESERVAS <-> PUERTOS
            DataSource.Relations.Add("Reservasstock", "Puertos", new[] {
                    new RelationColumnInfo("fkpuertosid", "id")});

            // PUERTOS <-> PAISES
            DataSource.Relations.Add("Puertos", "Paises", new[] {
                    new RelationColumnInfo("fkpaises", "Valor")});

            // RESERVAS <-> MONEDAS
            DataSource.Relations.Add("Reservasstock", "Monedas", new[] {
                    new RelationColumnInfo("fkmonedas", "id")});


            DataSource.RebuildResultSchema();
                
            
        }
    }
}
