using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Documentos.Facturas
{


    class FacturasReport : IReport
    {
        public SqlDataSource DataSource { get; private set; }

        public FacturasReport(IContextService user, string primarykey)
        {


            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";
            var mainQuery = new CustomSqlQuery("Facturas", "SELECT *, REPLACE(clientenif, 'ES', '') AS [clientenif_corto] FROM [Facturas]");

            if (!string.IsNullOrEmpty(primarykey))
            {
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Parameters.Add(new QueryParameter("referencia", typeof(string), primarykey));
                mainQuery.Sql = "SELECT * FROM [Facturas] where empresa=@empresa and referencia=@referencia";
            }

            // CLIENTES
            DataSource.Queries.Add(new CustomSqlQuery("clientes", string.Format("SELECT c.*, d.direccion as [Direccioncliente],d.poblacion as [Poblacioncliente],d.cp as [Cpcliente],d.telefono as [Telefonocliente] FROM [Clientes] as c left join direcciones as d on d.empresa=c.empresa and d.tipotercero={0} and d.fkentidad=c.fkcuentas", (int)TiposCuentas.Clientes)));

            // EMPRESAS
            //DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa], d.email as [Emailempresa], d.web as [Webempresa], d.telefonomovil as [TelefonoMovilempresa], d.email as [Email], d.web as [Web], d.notas as [Notas] FROM [Empresas] as e left join direcciones as d on d.empresa=e.id and d.tipotercero=-1 and d.fkentidad=e.id"));
            DataSource.Queries.Add(new CustomSqlQuery("empresa", "SELECT e.*,d.direccion as [Direccionempresa],d.poblacion as [Poblacionempresa],d.cp as [Cpempresa],d.telefono as [Telefonoempresa], d.email as [Email], d.web as [Web], d.notas as [Notas], d.defecto as [Defecto], d.tipotercero as [TipoTercero]," +
                "d.fkprovincia as [codProvincia], p.nombre as [nombreProvincia], d.fkpais as [codPais], pa.Descripcion as [nombrePais] " +
                "FROM[Empresas] as e " +
                "left join direcciones as d on d.empresa = e.id and d.tipotercero = -1 and d.fkentidad = e.id " +
                "left join Provincias as p on p.id = d.fkprovincia and p.codigopais = d.fkpais " +
                "left join Paises as pa on pa.valor = d.fkpais and pa.Valor = d.fkpais"));
            DataSource.Queries.Add(mainQuery);

            //DIRECCIONES
            DataSource.Queries.Add(new CustomSqlQuery("direcciones", "select * from direcciones where defecto = 1 and tipotercero = -1"));

            // FACTURASLIN
            DataSource.Queries.Add(new CustomSqlQuery("Facturaslin", "SELECT fl.*, (fl.ancho * 100) AS ancho_cm, (fl.largo * 100) AS largo_cm, (fl.grueso * 100) AS grueso_cm, u.textocorto AS [Unidadesdescripcion], fp.descripcion AS [FamiliaArticulo], ar.descripcion2 FROM [FacturasLin] AS fl " +
                                                                     " INNER JOIN Familiasproductos AS fp ON fp.empresa=fl.empresa AND fp.id=substring(fl.fkarticulos,0,3)" +
                                                                     " LEFT JOIN unidades AS u ON fp.fkunidadesmedida=u.id" +
                                                                     " INNER JOIN Articulos AS ar ON fl.fkarticulos = ar.id and fl.empresa = ar.empresa"));
            // FACTURAS TOTALES
            DataSource.Queries.Add(new CustomSqlQuery("Facturastotales", "SELECT * FROM [FacturasTotales]"));

            // Actualizo query de FacturasVencimientos (LL)
            DataSource.Queries.Add(new CustomSqlQuery("Facturasvencimientos", "SELECT * FROM [FacturasVencimientos], Facturas f WHERE f.id = [FacturasVencimientos].fkfacturas ORDER BY fkfacturas"));

            // Nueva Querie para mostrar los vencimientos(fecha e importe) en una sola línea (LLuís) 
            DataSource.Queries.Add(new CustomSqlQuery("VencimientosFila", "SELECT fv1.empresa, fv1.fkfacturas," +
                                                                          "CONVERT(VARCHAR(10), fv1.fechavencimiento, 103) + ' -- ' + CAST(fv1.importevencimiento AS VARCHAR(10)) vencimiento_3," +
                                                                          "CONVERT(VARCHAR(10), fv2.fechavencimiento, 103) + ' -- ' + CAST(fv2.importevencimiento AS VARCHAR(10)) vencimiento_2," +
                                                                          "CONVERT(VARCHAR(10), fv3.fechavencimiento, 103) + ' -- ' + CAST(fv3.importevencimiento AS VARCHAR(10)) vencimiento_1 " +
                                                                          "FROM FacturasVencimientos fv1 " +
                                                                          "LEFT JOIN FacturasVencimientos fv2 ON fv1.id > fv2.id AND fv1.fkfacturas = fv2.fkfacturas " +
                                                                          "LEFT JOIN FacturasVencimientos fv3 ON fv2.id > fv3.id AND fv2.fkfacturas = fv3.fkfacturas " +
                                                                          "ORDER BY fv1.empresa, fv1.fkfacturas, vencimiento_1 DESC"));

            // Actualizo query de Formaspago (LL)
            DataSource.Queries.Add(new CustomSqlQuery("Formaspago", "SELECT * FROM [formaspago], Facturas f WHERE f.fkformaspago = [formaspago].id"));

            // FORMASPAGOLIN
            DataSource.Queries.Add(new CustomSqlQuery("FormaspagoLin", "SELECT * FROM [formaspagoLin]"));

            // ALBARANES
            DataSource.Queries.Add(new CustomSqlQuery("Albaranes", "SELECT * FROM FacturasLin fl, Albaranes a WHERE fl.fkalbaranes = a.id and fl.empresa = a.empresa"));

            // MONEDAS (LL)
            DataSource.Queries.Add(new CustomSqlQuery("Monedas", "SELECT id, descripcion, abreviatura FROM Monedas"));

            // ARTÍCULOS, con ella poodremos analizar si el Artículo que va a pintarse es un 'Artículo Texto' o si es un 'Medidas Libres' (Lluís)
            DataSource.Queries.Add(new CustomSqlQuery("Articulos", "SELECT * FROM Articulos"));

            // BANCOSMANDATOS -> Para acceder a los datos bancarios tanto del cliente como de la misma empresa emisora (LL)
            DataSource.Queries.Add(new CustomSqlQuery("BancosMandatos", "SELECT bm.empresa, bm.fkcuentas, bm.descripcion, bm.iban, bm.bic, bm.direccion, bm.cpostal, bm.ciudad, bm.fkprovincias FROM BancosMandatos bm"));

            // CUENTAS -> Para modificar el nif del cliente y hacerlo más corto
            DataSource.Queries.Add(new CustomSqlQuery("Cuentas", "SELECT cu.empresa, cu.id, cu.nif, REPLACE(cu.nif, 'ES', '') AS clientenif_corto FROM Cuentas cu"));

            // PROVINCIAS
            DataSource.Queries.Add(new CustomSqlQuery("Provincias", "SELECT * FROM Provincias"));

            // PAÍSES
            DataSource.Queries.Add(new CustomSqlQuery("Paises", "SELECT * FROM Paises"));

            // PUERTOS
            DataSource.Queries.Add(new CustomSqlQuery("Puertos", "SELECT * FROM Puertos"));

            // Añadimos la consulta necesaria para devolver los datos del contenedor, que se encuentra en FacturasLin (LL)
            // Comentada porque ya no es necesaria
            // DataSource.Queries.Add(new CustomSqlQuery("Albaraneslin", "SELECT * FROM Albaranes a, AlbaranesLin al WHERE a.id = al.fkalbaranes"));


            ///////////////////////////////////////////////////////////////////////////////////////////////////


            // Create a master-detail relation between the queries.
            DataSource.Relations.Add("Facturas", "Facturaslin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkfacturas")
                    });

            DataSource.Relations.Add("Facturas", "Facturastotales", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkfacturas")});

            DataSource.Relations.Add("Facturas", "Facturasvencimientos", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkfacturas")});

            // VENCIMIENTOSFILA -> Relación con la nueva consulta de Facturasvencimientos
            DataSource.Relations.Add("Facturas", "VencimientosFila", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkfacturas")});

            DataSource.Relations.Add("Facturas", "clientes", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkclientes", "fkcuentas")});

            DataSource.Relations.Add("Facturas", "empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            //EMPRESA -- DIRECCIONES
            //DataSource.Relations.Add("empresa", "direcciones", new[] {
            //       new RelationColumnInfo("id", "empresa")});

            DataSource.Relations.Add("Facturas", "direcciones", new[] {
                    new RelationColumnInfo("empresa", "empresa")});

            ////DIRECCIONES -- PAISES
            //DataSource.Relations.Add("direcciones", "Paises", new[] {
            //        new RelationColumnInfo("fkpais", "Valor")});

            ////DIRECCIONES -- PROVINCIAS
            //DataSource.Relations.Add("direcciones", "Provincias", new[] {
            //        new RelationColumnInfo("fkprovincia", "id")});

            ////PAISES -- PROVINCIAS
            //DataSource.Relations.Add("Paises", "Provincias", new[] {
            //        new RelationColumnInfo("Valor", "codigopais")});

            DataSource.Relations.Add("Facturas", "Formaspago", new[] {
                    // Añado relación (Lluís)
                    new RelationColumnInfo("empresa", "empresa" ),
                    new RelationColumnInfo("fkformaspago", "id")});

            DataSource.Relations.Add("FormasPago", "FormaspagoLin", new[] {
                    new RelationColumnInfo("id", "fkFormasPago")});

            // Añadir relación entre Facturas y Albaranes (Lluís)
            // Comentada porque ya no es necesaria
            //DataSource.Relations.Add("Albaranes", "Facturas", new[] {
            //        new RelationColumnInfo("empresa", "empresa"),
            //        new RelationColumnInfo("fkfacturas", "id")});

            // Añado relación entre Albaranes y AlbaranesLin, de esta manera traemos el contenedor y sus características
            DataSource.Relations.Add("Albaranes", "AlbaranesLin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkalbaranes")});

            // Añado relación entre Facturas y Monedas (Lluís)
            DataSource.Relations.Add("Facturas", "Monedas", new[] {
                    new RelationColumnInfo("fkmonedas", "id")});

            // FACTURASLIN <-> ARTÍCULOS (Lluís)
            DataSource.Relations.Add("Facturaslin", "Articulos", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkarticulos", "id")});

            // FACTURAS <-> BANCOSMANDATOS (LL)
            DataSource.Relations.Add("Facturas", "BancosMandatos", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkcuentastesoreria", "fkcuentas")});

            // CLIENTES <-> BANCOSMANDATOS (LL)
            DataSource.Relations.Add("clientes", "BancosMandatos", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkcuentas", "fkcuentas")});

            // CLIENTES <-> CUENTAS - Para acceder al NIF y modificarlo
            DataSource.Relations.Add("clientes", "Cuentas", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkcuentas", "id")});

            // BANCOSMANDATOS <-> PROVINCIAS
            DataSource.Relations.Add("BancosMandatos", "Provincias", new[] {
                    new RelationColumnInfo("fkprovincias", "id")});

            // FACTURAS <-> PUERTOS
            DataSource.Relations.Add("Facturas", "Puertos", new[] {
                    new RelationColumnInfo("fkpuertosid", "id")});

            // PUERTOS <-> PAISES
            DataSource.Relations.Add("Puertos", "Paises", new[] {
                    new RelationColumnInfo("fkpaises", "Valor")});

            DataSource.RebuildResultSchema();


        }
    }
}
