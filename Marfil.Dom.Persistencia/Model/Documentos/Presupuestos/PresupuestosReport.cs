using DevExpress.DataAccess.ConnectionParameters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Presupuestos
{


    class PresupuestosReport : IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public PresupuestosReport(IContextService user, string primarykey)
        {
            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";

            // PRESUPUESTOS Consulta Principal
            var mainQuery = new CustomSqlQuery("presupuestos", "SELECT * FROM [Presupuestos] ");

            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia", typeof(string), primarykey));
                mainQuery.Sql = "SELECT * FROM [Presupuestos] where empresa=@empresa and referencia=@referencia";
            }

            // CLIENTES
            DataSource.Queries.Add(new CustomSqlQuery("clientes", string.Format("SELECT c.*,d.direccion as [Direccioncliente],d.poblacion as [Poblacioncliente],d.cp as [Cpcliente],d.telefono as [Telefonocliente] FROM [Clientes] as c left join direcciones as d on d.empresa=c.empresa and d.tipotercero={0} and d.fkentidad=c.fkcuentas", (int)TiposCuentas.Clientes)));

            // EMPRESA
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa], d.email as [Email], d.web as [Web], d.notas as [Notas], d.defecto as [Defecto], d.tipotercero as [TipoTercero], " +
                "d.fkprovincia as [codProvincia], p.nombre as [nombreProvincia], d.fkpais as [codPais], pa.Descripcion as [nombrePais] " +
                "FROM [Empresas] as e " +
                "left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id " +
                "left join Provincias as p on p.id = d.fkprovincia and p.codigopais = d.fkpais " +
                "left join Paises as pa on pa.valor = d.fkpais and pa.Valor = d.fkpais"));

            DataSource.Queries.Add(mainQuery);

            // PRESUPUESTOS LIN (Actualizada con centímetros de medidas)
            DataSource.Queries.Add(new CustomSqlQuery("presupuestoslin", "SELECT pl.*, (pl.ancho * 100) AS ancho_cm, (pl.largo * 100) AS largo_cm, (pl.grueso * 100) AS grueso_cm, u.textocorto as [Unidadesdescripcion], ar.descripcion2 FROM [PresupuestosLin] as pl" +
                                                                            " inner join Familiasproductos as fp on fp.empresa=pl.empresa and fp.id=substring(pl.fkarticulos,0,3)" +
                                                                            " left join unidades as u on fp.fkunidadesmedida=u.id" +
                                                                            " INNER JOIN Articulos AS ar ON pl.fkarticulos = ar.id"));

            // PRESUPUESTOS TOTALES
            DataSource.Queries.Add(new CustomSqlQuery("presupuestostotales", "SELECT * FROM [PresupuestosTotales]"));

            // FORMAS PAGO
            DataSource.Queries.Add(new CustomSqlQuery("Formaspago", "SELECT * FROM [formaspago]"));

            // PAÍSES
            DataSource.Queries.Add(new CustomSqlQuery("Paises", "SELECT Valor, Descripcion, Descripcion2 FROM Paises"));

            // PUERTOS
            DataSource.Queries.Add(new CustomSqlQuery("Puertos", "SELECT * FROM Puertos"));

            // BANCOS MANDATOS
            DataSource.Queries.Add(new CustomSqlQuery("BancosMandatos", "SELECT * FROM BancosMandatos"));

            // PROVINCIAS
            DataSource.Queries.Add(new CustomSqlQuery("Provincias", "SELECT * FROM Provincias"));

            // MONEDAS
            DataSource.Queries.Add(new CustomSqlQuery("Monedas", "SELECT id, descripcion, abreviatura FROM Monedas"));

            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Create a master-detail relation between the queries.

            // PRESUPUESTOS <-> PRESUPUESTOS LIN
            DataSource.Relations.Add("presupuestos", "presupuestoslin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkpresupuestos")});

            // PRESUPUESTOS <-> PRESUPUESTOS TOTALES
            DataSource.Relations.Add("presupuestos", "presupuestostotales", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkpresupuestos")});

            // PRESUPUESTOS <-> CLIENTES
            DataSource.Relations.Add("presupuestos", "clientes", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkclientes", "fkcuentas")});

            // PRESUPUESTOS <-> EMPRESA
            DataSource.Relations.Add("presupuestos", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            // PRESUPUESTOS <-> FORMASPAGO
            DataSource.Relations.Add("presupuestos", "Formaspago", new[] {
                    new RelationColumnInfo("fkformaspago", "id")});

            // PRESUPUESTOS <-> PUERTOS
            DataSource.Relations.Add("presupuestos", "Puertos", new[] {
                    new RelationColumnInfo("fkpuertosid", "id")});

            // PRESUPUESTOS <-> BANCOS MANDATOS
            DataSource.Relations.Add("presupuestos", "BancosMandatos", new[] {
                    new RelationColumnInfo("fkcuentastesoreria", "fkcuentas")});

            // PRESUPUESTOS <-> MONEDAS
            DataSource.Relations.Add("presupuestos", "Monedas", new[] {
                    new RelationColumnInfo("fkmonedas", "id")});

            // PUERTOS <-> PAISES
            DataSource.Relations.Add("Puertos", "Paises", new[] {
                    new RelationColumnInfo("fkpaises", "Valor")});

            // BANCOS MANDATOS <-> PROVINCIAS
            DataSource.Relations.Add("BancosMandatos", "Provincias", new[] {
                    new RelationColumnInfo("fkprovincias", "id")});

            // PROVINCIAS <-> PAISES
            DataSource.Relations.Add("Provincias", "Paises", new[] {
                    new RelationColumnInfo("codigopais", "Valor")});


            DataSource.RebuildResultSchema();


        }
    }
}
