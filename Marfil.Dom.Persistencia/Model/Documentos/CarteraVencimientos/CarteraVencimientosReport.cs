using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Kit
{
    class CarteraVencimientosReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public CarteraVencimientosReport(IContextService user,string primarykey)
        {
            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";
            var mainQuery = new CustomSqlQuery("CarteraVencimientos", "SELECT * FROM [CarteraVencimientos] ");
            
            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia",typeof(string),primarykey));
                mainQuery.Sql = "SELECT * FROM [CarteraVencimientos] where empresa=@empresa and referencia=@referencia";
            }
             
            DataSource.Queries.Add(mainQuery);
            DataSource.Queries.Add(new CustomSqlQuery("Cuentas", "SELECT * FROM [Cuentas]"));
            DataSource.Queries.Add(new CustomSqlQuery("Empresas", "SELECT id, nombre, razonsocial, nif, administrador, nifcifadministrador FROM [Empresas]"));
            DataSource.Queries.Add(new CustomSqlQuery("Direcciones", "SELECT empresa, fkentidad, fktipovia, direccion, poblacion, cp, telefono, telefonomovil, email, web FROM [Direcciones]"));
            DataSource.Queries.Add(new CustomSqlQuery("Provincias", "SELECT id, nombre, codigopais FROM [Provincias]"));
            DataSource.Queries.Add(new CustomSqlQuery("Paises", "SELECT valor, descripcion FROM [Paises]"));

            DataSource.Relations.Add("CarteraVencimientos", "Cuentas", new[] {
                        new RelationColumnInfo("empresa", "empresa"),
                        new RelationColumnInfo("fkcuentas", "id")});

            DataSource.Relations.Add("CarteraVencimientos", "Cuentas", new[] {
                        new RelationColumnInfo("empresa", "empresa"),
                        new RelationColumnInfo("fkcuentas", "id")});

            DataSource.Relations.Add("CarteraVencimientos", "Empresas", new[] {
                        new RelationColumnInfo("empresa", "id")});

            DataSource.Relations.Add("Empresas", "Direcciones", new[] {
                        new RelationColumnInfo("id", "empresa"),
                        new RelationColumnInfo("id", "fkentidad")});

            DataSource.Relations.Add("Direcciones", "Provincias", new[] {
                        new RelationColumnInfo("fkpais", "codigopais"),
                        new RelationColumnInfo("fkprovincia", "id")});

            DataSource.Relations.Add("Direcciones", "Paises", new[] {
                        new RelationColumnInfo("fkpais", "valor")});

            DataSource.Relations.Add("Cuentas", "Direcciones", new[] {
                        new RelationColumnInfo("empresa", "empresa"),
                        new RelationColumnInfo("id", "fkentidad")});

            DataSource.Relations.Add("Cuentas", "Paises", new[] {
                        new RelationColumnInfo("fkpais", "valor")});

            DataSource.RebuildResultSchema();
        }
    }
}
