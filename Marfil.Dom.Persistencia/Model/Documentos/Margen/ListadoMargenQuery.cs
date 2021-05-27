using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.DataAccess.Sql;
using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using DevExpress.XtraReports;

namespace Marfil.Dom.Persistencia.Model.Documentos.Margen
{
    class ListadoMargenQuery : IReport
    {

        public SqlDataSource DataSource { get; private set; }



        public ListadoMargenQuery(IContextService user, Dictionary<string, object> dictionary = null)
        {

            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";

            var Valorado = "0";
            if (dictionary != null)
                Valorado = dictionary["Valorado"].ToString();


            //  var mainQuery = new CustomSqlQuery("Albaranes", "SELECT *, [Total compra] * contador AS [Total compra total], [PMPC] * contador AS [PMPC total], ([Total venta] - [Total compra]) AS [Margen], " +
            //"  (CASE WHEN([Total venta] != 0 OR[Total compra] != 0) THEN(100 - (([Total compra] * 100) / ([Total venta]))) ELSE 0 END) AS [% Margen] " +
            //"  FROM( " +
            //" SELECT COUNT(l.lote) AS contador, a.empresa, a.fechadocumento, a.fkclientes, a.fkagentes, a.fkcomerciales, s.fktonomaterial, l.lote AS [Lote], " +
            //" l.fkarticulos AS[Cod.Artículo], l.descripcion AS [Descripción], " +
            //" art.descripcionabreviada AS[Descp.abreviada], f.descripcion AS[Descripción fam], u.codigounidad AS [U.M.], SUM(l.cantidad) AS [Piezas], " +
            //" (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN s.largoentrada * s.anchoentrada * s.gruesoentrada * SUM(s.cantidadentrada) ELSE s.largoentrada * s.anchoentrada * SUM(l.cantidad) END) AS[Metros compra],  " +
            //" (((s.netocompra");
            //  if (Valorado == "1")
            //      mainQuery.Sql += " + s.costeacicionalvariable + s.costeadicionalmaterial + s.costeadicionalotro + s.costeadicionalportes ";
            //  mainQuery.Sql += ") / SUM(s.cantidadentrada)) * SUM(l.cantidad)) AS[Total compra], " +
            // " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN l.largo * l.ancho * l.grueso * SUM(l.cantidad) ELSE l.largo * l.ancho * SUM(l.cantidad) END) AS[Metros salida], " +
            // " SUM(l.importe) AS[Total venta],  (s.netocompra / SUM(s.metrosentrada)) AS[PMPC],(SUM(l.importe) / SUM(l.metros)) AS[PMPV], " +
            // " a.referencia[Albarán], a.nombrecliente[Cliente],ag.descripcion AS[Agente], c.descripcion AS[Comercial], SUM(l.cantidad) as [Piezas salida], " +
            // " alc.fkproveedores, alc.nombreproveedor [Proveedor] " +
            // " FROM Albaranes AS a " +
            // " LEFT JOIN AlbaranesLin AS l ON l.empresa = a.empresa AND l.fkalbaranes = a.id " +
            // " LEFT JOIN Stockhistorico AS s ON s.empresa = a.empresa AND s.fkarticulos = l.fkarticulos AND s.lote = ISNULL(l.lote, ' ') AND s.loteid = ISNULL(l.tabla, 0) " +
            // " LEFT JOIN Unidades AS u ON u.empresa = a.empresa AND l.fkunidades = u.id " +
            // " LEFT JOIN Cuentas AS ag ON ag.empresa = a.empresa AND ag.id = a.fkagentes " +
            // " LEFT JOIN Cuentas AS c ON c.empresa = a.empresa AND c.id = a.fkcomerciales " +
            // " LEFT JOIN Articulos AS art ON art.empresa = a.empresa AND art.id = l.fkarticulos " +
            // " LEFT JOIN AlbaranesCompras AS alc ON alc.empresa = a.empresa AND alc.referencia = s.referenciaentrada " +
            // " LEFT JOIN Familiasproductos AS f ON f.empresa = a.empresa AND f.id = SUBSTRING(l.fkarticulos, 0, 3)";

            var mainQuery = new CustomSqlQuery("Albaranes", "SELECT *," +
            " PMPC / Piezas AS[PMPC total], PMPV / Piezas AS[PMPV total], ([Total venta] - [Total compra]) AS[Margen]," +
            " (CASE WHEN([Total venta] != 0 OR[Total compra] != 0) THEN(100 - (([Total compra] * 100) / ([Total venta]))) ELSE 0 END) AS[% Margen]" +
            " FROM(" +
            " SELECT a.empresa, a.fechadocumento, s.fktonomaterial, l.lote AS[Lote], l.fkarticulos AS[Cod.Artículo], l.descripcion AS[Descripción]," +
            " art.descripcionabreviada AS[Descp.abreviada], f.descripcion AS[Descripción fam], u.codigounidad AS[U.M.], SUM(l.cantidad) AS[Piezas]," +
            " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN SUM(s.largoentrada * s.anchoentrada * s.gruesoentrada * s.cantidadentrada) ELSE SUM(s.largoentrada * s.anchoentrada * l.cantidad) END) AS[Metros compra]," +
            " SUM(((s.netocompra");
            if (Valorado == "1")
                mainQuery.Sql += " + s.costeacicionalvariable + s.costeadicionalmaterial + s.costeadicionalotro + s.costeadicionalportes ";
            mainQuery.Sql += ") / s.cantidadentrada) * l.cantidad) AS[Total compra], " +
            " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN SUM(l.largo * l.ancho * l.grueso * l.cantidad) ELSE SUM(l.largo * l.ancho * l.cantidad) END) AS[Metros salida], " +
            " SUM(l.importe) AS[Total venta],  SUM(s.netocompra / s.metrosentrada) AS[PMPC], SUM(l.importe / l.metros) AS[PMPV]," +
            " alc.fkproveedores AS[Cod.Proveedor], alc.nombreproveedor AS[Proveedor]" +
            " FROM Albaranes AS a" +
            " LEFT JOIN AlbaranesLin AS l ON l.empresa = a.empresa AND l.fkalbaranes = a.id" +
            " LEFT JOIN Stockhistorico AS s ON s.empresa = a.empresa AND s.fkarticulos = l.fkarticulos AND s.lote = ISNULL(l.lote, ' ') AND s.loteid = ISNULL(l.tabla, 0)" +
            " LEFT JOIN Unidades AS u ON l.fkunidades = u.id" +
            " LEFT JOIN Cuentas AS ag ON ag.empresa = a.empresa AND ag.id = a.fkagentes" +
            " LEFT JOIN Cuentas AS c ON c.empresa = a.empresa AND c.id = a.fkcomerciales" +
            " LEFT JOIN Articulos AS art ON art.empresa = a.empresa AND art.id = l.fkarticulos" +
            " LEFT JOIN AlbaranesCompras AS alc ON alc.empresa = a.empresa AND alc.referencia = s.referenciaentrada" +
            " LEFT JOIN Familiasproductos AS f ON f.empresa = a.empresa AND f.id = SUBSTRING(l.fkarticulos, 0, 3)";

            var DetalladoPorCliente = new CustomSqlQuery("DetalladoPorCliente", "SELECT *," +
            " PMPC / Piezas AS[PMPC total], PMPV / Piezas AS[PMPV total], ([Total venta] - [Total compra]) AS[Margen]," +
            " (CASE WHEN([Total venta] != 0 OR[Total compra] != 0) THEN(100 - (([Total compra] * 100) / ([Total venta]))) ELSE 0 END) AS[% Margen]" +
            " FROM(" +
            " SELECT a.empresa, a.fechadocumento, s.fktonomaterial, l.lote AS[Lote], l.fkarticulos AS[Cod.Artículo], l.descripcion AS[Descripción]," +
            " art.descripcionabreviada AS[Descp.abreviada], f.descripcion AS[Descripción fam], u.codigounidad AS[U.M.], SUM(l.cantidad) AS[Piezas]," +
            " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN SUM(s.largoentrada * s.anchoentrada * s.gruesoentrada * s.cantidadentrada) ELSE SUM(s.largoentrada * s.anchoentrada * l.cantidad) END) AS[Metros compra]," +
            " SUM(((s.netocompra");
            if (Valorado == "1")
                DetalladoPorCliente.Sql += " + s.costeacicionalvariable + s.costeadicionalmaterial + s.costeadicionalotro + s.costeadicionalportes ";
            DetalladoPorCliente.Sql += ") / s.cantidadentrada) * l.cantidad) AS[Total compra], " +
            " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN SUM(l.largo * l.ancho * l.grueso * l.cantidad) ELSE SUM(l.largo * l.ancho * l.cantidad) END) AS[Metros salida], " +
            " SUM(l.importe) AS[Total venta],  SUM(s.netocompra / s.metrosentrada) AS[PMPC], SUM(l.importe / l.metros) AS[PMPV]," +
            " a.fkclientes, a.nombrecliente, alc.fkproveedores AS[Cod.Proveedor], alc.nombreproveedor AS[Proveedor]" +
            " FROM Albaranes AS a" +
            " LEFT JOIN AlbaranesLin AS l ON l.empresa = a.empresa AND l.fkalbaranes = a.id" +
            " LEFT JOIN Stockhistorico AS s ON s.empresa = a.empresa AND s.fkarticulos = l.fkarticulos AND s.lote = ISNULL(l.lote, ' ') AND s.loteid = ISNULL(l.tabla, 0)" +
            " LEFT JOIN Unidades AS u ON l.fkunidades = u.id" +
            " LEFT JOIN Cuentas AS ag ON ag.empresa = a.empresa AND ag.id = a.fkagentes" +
            " LEFT JOIN Cuentas AS c ON c.empresa = a.empresa AND c.id = a.fkcomerciales" +
            " LEFT JOIN Articulos AS art ON art.empresa = a.empresa AND art.id = l.fkarticulos" +
            " LEFT JOIN AlbaranesCompras AS alc ON alc.empresa = a.empresa AND alc.referencia = s.referenciaentrada" +
            " LEFT JOIN Familiasproductos AS f ON f.empresa = a.empresa AND f.id = SUBSTRING(l.fkarticulos, 0, 3)";            

            var AgrupadoPorArticulo = new CustomSqlQuery("AgrupadoPorArticulo", "SELECT [Cod.Artículo], [Descp.abreviada], [U.M.], SUM(Piezas) AS Piezas, (SUM([Total compra]) / SUM([Metros compra])) AS PMPC,"+
            " SUM([Metros compra]) AS[Metros compra], SUM([Total compra]) AS[Total compra], (SUM([Total venta]) / SUM([Metros salida])) AS PMPV," +
            " SUM([Metros salida]) AS [Metros salida], SUM([Total venta]) AS [Total venta]" +
            "  FROM( " +
            " SELECT a.empresa, a.fechadocumento, a.fkclientes, a.fkagentes, a.fkcomerciales, s.fktonomaterial, l.lote AS [Lote], " +
            " s.loteid AS[Loteid], l.fkarticulos AS[Cod.Artículo], l.descripcion AS [Descripción], " +
            " art.descripcionabreviada AS[Descp.abreviada], f.descripcion AS[Descripción fam], u.codigounidad AS [U.M.], SUM(l.cantidad) AS [Piezas], " +
            //" (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN s.largoentrada * s.anchoentrada * s.gruesoentrada * SUM(s.cantidadentrada) ELSE s.largoentrada * s.anchoentrada * SUM(l.cantidad) END) AS[Metros compra],  " +
            " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN SUM(s.largoentrada * s.anchoentrada * s.gruesoentrada * s.cantidadentrada)" +
            " WHEN LOWER(u.codigounidad) = 'm2' THEN SUM(s.largoentrada * s.anchoentrada * l.cantidad) ELSE SUM(l.cantidad) END) AS[Metros compra],"+
            " (((s.netocompra");
            if (Valorado == "1")
                AgrupadoPorArticulo.Sql += " + s.costeacicionalvariable + s.costeadicionalmaterial + s.costeadicionalotro + s.costeadicionalportes ";
            AgrupadoPorArticulo.Sql += ") / SUM(s.cantidadentrada)) * SUM(l.cantidad)) AS[Total compra], " +
            //" (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN l.largo * l.ancho * l.grueso * SUM(l.cantidad) ELSE l.largo * l.ancho * SUM(l.cantidad) END) AS[Metros salida], " +
            " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN SUM(l.largo * l.ancho * l.grueso * l.cantidad)" +
            " WHEN LOWER(u.codigounidad) = 'm2' THEN SUM(l.largo * l.ancho * l.cantidad) ELSE SUM(l.cantidad) END) AS[Metros salida], " +
            " SUM(l.importe) AS[Total venta],  (s.netocompra / SUM(s.metrosentrada)) AS[PMPC],(SUM(l.importe) / SUM(l.metros)) AS[PMPV], " +
            " a.referencia[Albarán], a.nombrecliente[Cliente],ag.descripcion AS[Agente], c.descripcion AS[Comercial], SUM(l.cantidad) as [Piezas salida], " +
            " alc.fkproveedores, alc.nombreproveedor [Proveedor] " +
            " FROM Albaranes AS a " +
            " LEFT JOIN AlbaranesLin AS l ON l.empresa = a.empresa AND l.fkalbaranes = a.id " +
            " LEFT JOIN Stockhistorico AS s ON s.empresa = a.empresa AND s.fkarticulos = l.fkarticulos AND s.lote = ISNULL(l.lote, ' ') AND s.loteid = ISNULL(l.tabla, 0) " +
            " LEFT JOIN Unidades AS u ON l.fkunidades = u.id " +
            " LEFT JOIN Cuentas AS ag ON ag.empresa = a.empresa AND ag.id = a.fkagentes " +
            " LEFT JOIN Cuentas AS c ON c.empresa = a.empresa AND c.id = a.fkcomerciales " +
            " LEFT JOIN Articulos AS art ON art.empresa = a.empresa AND art.id = l.fkarticulos " +
            " LEFT JOIN AlbaranesCompras AS alc ON alc.empresa = a.empresa AND alc.referencia = s.referenciaentrada " +
            " LEFT JOIN Familiasproductos AS f ON f.empresa = a.empresa AND f.id = SUBSTRING(l.fkarticulos, 0, 3)";

            var AgrupadoPorCliente = new CustomSqlQuery("AgrupadoPorCliente", "SELECT [Cod.Cliente], [Cliente], [U.M.], SUM(Piezas) AS Piezas, (SUM([Total compra]) / SUM([Metros compra])) AS PMPC," +
            " SUM([Metros compra]) AS[Metros compra], SUM([Total compra]) AS[Total compra], (SUM([Total venta]) / SUM([Metros salida])) AS PMPV," +
            " SUM([Metros salida]) AS [Metros salida], SUM([Total venta]) AS [Total venta]" +
            "  FROM( " +
            " SELECT a.empresa, a.fechadocumento, a.fkclientes AS [Cod.Cliente], a.fkagentes, a.fkcomerciales, s.fktonomaterial, l.lote AS [Lote], " +
            " s.loteid AS[Loteid], l.fkarticulos AS[Cod.Artículo], l.descripcion AS [Descripción], " +
            " art.descripcionabreviada AS[Descp.abreviada], f.descripcion AS[Descripción fam], u.codigounidad AS [U.M.], SUM(l.cantidad) AS [Piezas], " +
            //" (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN s.largoentrada * s.anchoentrada * s.gruesoentrada * SUM(s.cantidadentrada) ELSE s.largoentrada * s.anchoentrada * SUM(l.cantidad) END) AS[Metros compra],  " +
            " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN SUM(s.largoentrada * s.anchoentrada * s.gruesoentrada * s.cantidadentrada)" +
            " WHEN LOWER(u.codigounidad) = 'm2' THEN SUM(s.largoentrada * s.anchoentrada * l.cantidad) ELSE SUM(l.cantidad) END) AS[Metros compra]," +
            " (((s.netocompra");
            if (Valorado == "1")
                AgrupadoPorCliente.Sql += " + s.costeacicionalvariable + s.costeadicionalmaterial + s.costeadicionalotro + s.costeadicionalportes ";
            AgrupadoPorCliente.Sql += ") / SUM(s.cantidadentrada)) * SUM(l.cantidad)) AS[Total compra], " +
            //" (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN l.largo * l.ancho * l.grueso * SUM(l.cantidad) ELSE l.largo * l.ancho * SUM(l.cantidad) END) AS[Metros salida], " +
            " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN SUM(l.largo * l.ancho * l.grueso * l.cantidad)" +
            " WHEN LOWER(u.codigounidad) = 'm2' THEN SUM(l.largo * l.ancho * l.cantidad) ELSE SUM(l.cantidad) END) AS[Metros salida], " +
            " SUM(l.importe) AS[Total venta],  (s.netocompra / SUM(s.metrosentrada)) AS[PMPC],(SUM(l.importe) / SUM(l.metros)) AS[PMPV], " +
            " a.referencia[Albarán], a.nombrecliente[Cliente],ag.descripcion AS[Agente], c.descripcion AS[Comercial], SUM(l.cantidad) as [Piezas salida], " +
            " alc.fkproveedores, alc.nombreproveedor [Proveedor] " +
            " FROM Albaranes AS a " +
            " LEFT JOIN AlbaranesLin AS l ON l.empresa = a.empresa AND l.fkalbaranes = a.id " +
            " LEFT JOIN Stockhistorico AS s ON s.empresa = a.empresa AND s.fkarticulos = l.fkarticulos AND s.lote = ISNULL(l.lote, ' ') AND s.loteid = ISNULL(l.tabla, 0) " +
            " LEFT JOIN Unidades AS u ON l.fkunidades = u.id " +
            " LEFT JOIN Cuentas AS ag ON ag.empresa = a.empresa AND ag.id = a.fkagentes " +
            " LEFT JOIN Cuentas AS c ON c.empresa = a.empresa AND c.id = a.fkcomerciales " +
            " LEFT JOIN Articulos AS art ON art.empresa = a.empresa AND art.id = l.fkarticulos " +
            " LEFT JOIN AlbaranesCompras AS alc ON alc.empresa = a.empresa AND alc.referencia = s.referenciaentrada " +
            " LEFT JOIN Familiasproductos AS f ON f.empresa = a.empresa AND f.id = SUBSTRING(l.fkarticulos, 0, 3)";

            var AgrupadoPorProveedor = new CustomSqlQuery("AgrupadoPorProveedor", "SELECT [Cod.Proveedor], [Proveedor], [U.M.], SUM(Piezas) AS Piezas, (SUM([Total compra]) / SUM([Metros compra])) AS PMPC," +
            " SUM([Metros compra]) AS[Metros compra], SUM([Total compra]) AS[Total compra], (SUM([Total venta]) / SUM([Metros salida])) AS PMPV," +
            " SUM([Metros salida]) AS [Metros salida], SUM([Total venta]) AS [Total venta]" +
            "  FROM( " +
            " SELECT a.empresa, a.fechadocumento, a.fkclientes, a.fkagentes, a.fkcomerciales, s.fktonomaterial, l.lote AS [Lote], " +
            " s.loteid AS[Loteid], l.fkarticulos AS[Cod.Artículo], l.descripcion AS [Descripción], " +
            " art.descripcionabreviada AS[Descp.abreviada], f.descripcion AS[Descripción fam], u.codigounidad AS [U.M.], SUM(l.cantidad) AS [Piezas], " +
            //" (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN s.largoentrada * s.anchoentrada * s.gruesoentrada * SUM(s.cantidadentrada) ELSE s.largoentrada * s.anchoentrada * SUM(l.cantidad) END) AS[Metros compra],  " +
            " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN SUM(s.largoentrada * s.anchoentrada * s.gruesoentrada * s.cantidadentrada)" +
            " WHEN LOWER(u.codigounidad) = 'm2' THEN SUM(s.largoentrada * s.anchoentrada * l.cantidad) ELSE SUM(l.cantidad) END) AS[Metros compra]," +
            " (((s.netocompra");
            if (Valorado == "1")
                AgrupadoPorProveedor.Sql += " + s.costeacicionalvariable + s.costeadicionalmaterial + s.costeadicionalotro + s.costeadicionalportes ";
            AgrupadoPorProveedor.Sql += ") / SUM(s.cantidadentrada)) * SUM(l.cantidad)) AS[Total compra], " +
            //" (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN l.largo * l.ancho * l.grueso * SUM(l.cantidad) ELSE l.largo * l.ancho * SUM(l.cantidad) END) AS[Metros salida], " +
            " (CASE WHEN LOWER(u.codigounidad) = 'm3' THEN SUM(l.largo * l.ancho * l.grueso * l.cantidad)" +
            " WHEN LOWER(u.codigounidad) = 'm2' THEN SUM(l.largo * l.ancho * l.cantidad) ELSE SUM(l.cantidad) END) AS[Metros salida], " +
            " SUM(l.importe) AS[Total venta],  (s.netocompra / SUM(s.metrosentrada)) AS[PMPC],(SUM(l.importe) / SUM(l.metros)) AS[PMPV], " +
            " a.referencia[Albarán], a.nombrecliente[Cliente],ag.descripcion AS[Agente], c.descripcion AS[Comercial], SUM(l.cantidad) as [Piezas salida], " +
            " alc.fkproveedores AS [Cod.Proveedor], alc.nombreproveedor [Proveedor] " +
            " FROM Albaranes AS a " +
            " LEFT JOIN AlbaranesLin AS l ON l.empresa = a.empresa AND l.fkalbaranes = a.id " +
            " LEFT JOIN Stockhistorico AS s ON s.empresa = a.empresa AND s.fkarticulos = l.fkarticulos AND s.lote = ISNULL(l.lote, ' ') AND s.loteid = ISNULL(l.tabla, 0) " +
            " LEFT JOIN Unidades AS u ON l.fkunidades = u.id " +
            " LEFT JOIN Cuentas AS ag ON ag.empresa = a.empresa AND ag.id = a.fkagentes " +
            " LEFT JOIN Cuentas AS c ON c.empresa = a.empresa AND c.id = a.fkcomerciales " +
            " LEFT JOIN Articulos AS art ON art.empresa = a.empresa AND art.id = l.fkarticulos " +
            " LEFT JOIN AlbaranesCompras AS alc ON alc.empresa = a.empresa AND alc.referencia = s.referenciaentrada " +
            " LEFT JOIN Familiasproductos AS f ON f.empresa = a.empresa AND f.id = SUBSTRING(l.fkarticulos, 0, 3)";


            if (dictionary != null)
            {
                var Series = dictionary["Series"].ToString();
                var fechaDesde = dictionary["FechaDesde"];
                var fechaHasta = dictionary["FechaHasta"];
                var ClienteDesde = dictionary["ClienteDesde"].ToString();
                var ClienteHasta = dictionary["ClienteHasta"].ToString();
                var AgenteDesde = dictionary["AgenteDesde"].ToString();
                var AgenteHasta = dictionary["AgenteHasta"].ToString();
                var ComercialDesde = dictionary["ComercialDesde"].ToString();
                var ComercialHasta = dictionary["ComercialHasta"].ToString();
                var ArticuloDesde = dictionary["ArticuloDesde"].ToString();
                var ArticuloHasta = dictionary["ArticuloHasta"].ToString();
                var MaterialDesde = dictionary["MaterialDesde"].ToString();
                var MaterialHasta = dictionary["MaterialHasta"].ToString();
                var FechaInforme = dictionary["FechaInforme"].ToString();

                // Condiciones.Clear();
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Sql += " WHERE a.empresa = '" + user.Empresa + "'";
                mainQuery.Sql += " AND s.metrosentrada > 0 AND s.cantidadentrada > 0 AND l.metros > 0 AND l.importe > 0 AND s.netocompra > 0 AND s.lote != ' ' ";
                
                DetalladoPorCliente.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                DetalladoPorCliente.Sql += " WHERE a.empresa = '" + user.Empresa + "'";
                DetalladoPorCliente.Sql += " AND s.metrosentrada > 0 AND s.cantidadentrada > 0 AND l.metros > 0 AND l.importe > 0 AND s.netocompra > 0 AND s.lote != ' ' ";

                AgrupadoPorArticulo.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                AgrupadoPorArticulo.Sql += " WHERE a.empresa = '" + user.Empresa + "'";
                AgrupadoPorArticulo.Sql += " AND s.metrosentrada > 0 AND s.cantidadentrada > 0 AND l.metros > 0 AND l.importe > 0 AND s.netocompra > 0 AND s.lote != ' ' ";

                AgrupadoPorCliente.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                AgrupadoPorCliente.Sql += " WHERE a.empresa = '" + user.Empresa + "'";
                AgrupadoPorCliente.Sql += " AND s.metrosentrada > 0 AND s.cantidadentrada > 0 AND l.metros > 0 AND l.importe > 0 AND s.netocompra > 0 AND s.lote != ' ' ";

                AgrupadoPorProveedor.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                AgrupadoPorProveedor.Sql += " WHERE a.empresa = '" + user.Empresa + "'";
                AgrupadoPorProveedor.Sql += " AND s.metrosentrada > 0 AND s.cantidadentrada > 0 AND l.metros > 0 AND l.importe > 0 AND s.netocompra > 0 AND s.lote != ' ' ";

                if (!string.IsNullOrEmpty(Series))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("series", typeof(string), Series));
                    mainQuery.Sql += (" a.fkseries=@series ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("series", typeof(string), Series));
                    DetalladoPorCliente.Sql += (" a.fkseries=@series ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("series", typeof(string), Series));
                    AgrupadoPorArticulo.Sql += (" a.fkseries=@series ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("series", typeof(string), Series));
                    AgrupadoPorCliente.Sql += (" a.fkseries=@series ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("series", typeof(string), Series));
                    AgrupadoPorProveedor.Sql += (" a.fkseries=@series ");
                }

                if (fechaDesde != null)
                {
                    //mainQuery
                    mainQuery.Parameters.Add(new QueryParameter("fechaDesde", typeof(DateTime), fechaDesde));
                    mainQuery.Sql += " AND a.fechadocumento>=@fechaDesde";

                    DetalladoPorCliente.Parameters.Add(new QueryParameter("fechaDesde", typeof(DateTime), fechaDesde));
                    DetalladoPorCliente.Sql += " AND a.fechadocumento>=@fechaDesde";

                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("fechaDesde", typeof(DateTime), fechaDesde));
                    AgrupadoPorArticulo.Sql += " AND a.fechadocumento>=@fechaDesde";

                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("fechaDesde", typeof(DateTime), fechaDesde));
                    AgrupadoPorCliente.Sql += " AND a.fechadocumento>=@fechaDesde";

                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("fechaDesde", typeof(DateTime), fechaDesde));
                    AgrupadoPorProveedor.Sql += " AND a.fechadocumento>=@fechaDesde";

                }

                if (fechaHasta != null)
                {
                    //mainQuery
                    mainQuery.Parameters.Add(new QueryParameter("fechaHasta", typeof(DateTime), fechaHasta));
                    mainQuery.Sql += " AND a.fechadocumento<=@fechaHasta";

                    DetalladoPorCliente.Parameters.Add(new QueryParameter("fechaHasta", typeof(DateTime), fechaHasta));
                    DetalladoPorCliente.Sql += " AND a.fechadocumento<=@fechaHasta";

                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("fechaHasta", typeof(DateTime), fechaHasta));
                    AgrupadoPorArticulo.Sql += " AND a.fechadocumento<=@fechaHasta";

                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("fechaHasta", typeof(DateTime), fechaHasta));
                    AgrupadoPorCliente.Sql += " AND a.fechadocumento<=@fechaHasta";

                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("fechaHasta", typeof(DateTime), fechaHasta));
                    AgrupadoPorProveedor.Sql += " AND a.fechadocumento<=@fechaHasta";

                }

                if (!string.IsNullOrEmpty(ClienteDesde))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("clientedesde", typeof(string), ClienteDesde));
                    mainQuery.Sql += (" a.fkclientes>=@clientedesde ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("clientedesde", typeof(string), ClienteDesde));
                    DetalladoPorCliente.Sql += (" a.fkclientes>=@clientedesde ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("clientedesde", typeof(string), ClienteDesde));
                    AgrupadoPorArticulo.Sql += (" a.fkclientes>=@clientedesde ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("clientedesde", typeof(string), ClienteDesde));
                    AgrupadoPorCliente.Sql += (" a.fkclientes>=@clientedesde ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("clientedesde", typeof(string), ClienteDesde));
                    AgrupadoPorProveedor.Sql += (" a.fkclientes>=@clientedesde ");
                }

                if (!string.IsNullOrEmpty(ClienteHasta))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("clientehasta", typeof(string), ClienteHasta));
                    mainQuery.Sql += (" a.fkclientes<=@clientehasta ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("clientehasta", typeof(string), ClienteHasta));
                    DetalladoPorCliente.Sql += (" a.fkclientes<=@clientehasta ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("clientehasta", typeof(string), ClienteHasta));
                    AgrupadoPorArticulo.Sql += (" a.fkclientes<=@clientehasta ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("clientehasta", typeof(string), ClienteHasta));
                    AgrupadoPorCliente.Sql += (" a.fkclientes<=@clientehasta ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("clientehasta", typeof(string), ClienteHasta));
                    AgrupadoPorProveedor.Sql += (" a.fkclientes<=@clientehasta ");
                }

                if (!string.IsNullOrEmpty(AgenteDesde))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("agentedesde", typeof(string), AgenteDesde));
                    mainQuery.Sql += (" a.fkagentes>=@agentedesde ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("agentedesde", typeof(string), AgenteDesde));
                    DetalladoPorCliente.Sql += (" a.fkagentes>=@agentedesde ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("agentedesde", typeof(string), AgenteDesde));
                    AgrupadoPorArticulo.Sql += (" a.fkagentes>=@agentedesde ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("agentedesde", typeof(string), AgenteDesde));
                    AgrupadoPorCliente.Sql += (" a.fkagentes>=@agentedesde ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("agentedesde", typeof(string), AgenteDesde));
                    AgrupadoPorProveedor.Sql += (" a.fkagentes>=@agentedesde ");
                }

                if (!string.IsNullOrEmpty(AgenteHasta))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("agentehasta", typeof(string), AgenteHasta));
                    mainQuery.Sql += (" a.fkagentes<=@agentehasta ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("agentehasta", typeof(string), AgenteHasta));
                    DetalladoPorCliente.Sql += (" a.fkagentes<=@agentehasta ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("agentehasta", typeof(string), AgenteHasta));
                    AgrupadoPorArticulo.Sql += (" a.fkagentes<=@agentehasta ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("agentehasta", typeof(string), AgenteHasta));
                    AgrupadoPorCliente.Sql += (" a.fkagentes<=@agentehasta ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("agentehasta", typeof(string), AgenteHasta));
                    AgrupadoPorProveedor.Sql += (" a.fkagentes<=@agentehasta ");

                }

                if (!string.IsNullOrEmpty(ComercialDesde))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("comercialdesde", typeof(string), ComercialDesde));
                    mainQuery.Sql += (" a.fkcomerciales>=@comercialdesde ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("comercialdesde", typeof(string), ComercialDesde));
                    DetalladoPorCliente.Sql += (" a.fkcomerciales>=@comercialdesde ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("comercialdesde", typeof(string), ComercialDesde));
                    AgrupadoPorArticulo.Sql += (" a.fkcomerciales>=@comercialdesde ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("comercialdesde", typeof(string), ComercialDesde));
                    AgrupadoPorCliente.Sql += (" a.fkcomerciales>=@comercialdesde ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("comercialdesde", typeof(string), ComercialDesde));
                    AgrupadoPorProveedor.Sql += (" a.fkcomerciales>=@comercialdesde ");
                }

                if (!string.IsNullOrEmpty(ComercialHasta))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("comercialhasta", typeof(string), ComercialHasta));
                    mainQuery.Sql += (" a.fkcomerciales<=@comercialhasta ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("comercialhasta", typeof(string), ComercialHasta));
                    DetalladoPorCliente.Sql += (" a.fkcomerciales<=@comercialhasta ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("comercialhasta", typeof(string), ComercialHasta));
                    AgrupadoPorArticulo.Sql += (" a.fkcomerciales<=@comercialhasta ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("comercialhasta", typeof(string), ComercialHasta));
                    AgrupadoPorCliente.Sql += (" a.fkcomerciales<=@comercialhasta ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("comercialhasta", typeof(string), ComercialHasta));
                    AgrupadoPorProveedor.Sql += (" a.fkcomerciales<=@comercialhasta ");
                }

                if (!string.IsNullOrEmpty(ArticuloDesde))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("articulodesde", typeof(string), ArticuloDesde));
                    mainQuery.Sql += (" l.fkarticulos>=@articulodesde ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("articulodesde", typeof(string), ArticuloDesde));
                    DetalladoPorCliente.Sql += (" l.fkarticulos>=@articulodesde ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("articulodesde", typeof(string), ArticuloDesde));
                    AgrupadoPorArticulo.Sql += (" l.fkarticulos>=@articulodesde ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("articulodesde", typeof(string), ArticuloDesde));
                    AgrupadoPorCliente.Sql += (" l.fkarticulos>=@articulodesde ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("articulodesde", typeof(string), ArticuloDesde));
                    AgrupadoPorProveedor.Sql += (" l.fkarticulos>=@articulodesde ");
                }

                if (!string.IsNullOrEmpty(ArticuloHasta))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("articulohasta", typeof(string), ArticuloHasta));
                    mainQuery.Sql += (" l.fkarticulos<=@articulohasta ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("articulohasta", typeof(string), ArticuloHasta));
                    DetalladoPorCliente.Sql += (" l.fkarticulos<=@articulohasta ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("articulohasta", typeof(string), ArticuloHasta));
                    AgrupadoPorArticulo.Sql += (" l.fkarticulos<=@articulohasta ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("articulohasta", typeof(string), ArticuloHasta));
                    AgrupadoPorCliente.Sql += (" l.fkarticulos<=@articulohasta ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("articulohasta", typeof(string), ArticuloHasta));
                    AgrupadoPorProveedor.Sql += (" l.fkarticulos<=@articulohasta ");
                }

                if (!string.IsNullOrEmpty(MaterialDesde))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("materialdesde", typeof(string), MaterialDesde));
                    mainQuery.Sql += (" s.fktonomaterial>=@materialdesde ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("materialdesde", typeof(string), MaterialDesde));
                    DetalladoPorCliente.Sql += (" s.fktonomaterial>=@materialdesde ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("materialdesde", typeof(string), MaterialDesde));
                    AgrupadoPorArticulo.Sql += (" s.fktonomaterial>=@materialdesde ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("materialdesde", typeof(string), MaterialDesde));
                    AgrupadoPorCliente.Sql += (" s.fktonomaterial>=@materialdesde ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("materialdesde", typeof(string), MaterialDesde));
                    AgrupadoPorProveedor.Sql += (" s.fktonomaterial>=@materialdesde ");
                }

                if (!string.IsNullOrEmpty(MaterialHasta))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("materialhasta", typeof(string), MaterialHasta));
                    mainQuery.Sql += (" s.fktonomaterial<=@materialhasta ");

                    DetalladoPorCliente.Sql += (" AND ");
                    DetalladoPorCliente.Parameters.Add(new QueryParameter("materialhasta", typeof(string), MaterialHasta));
                    DetalladoPorCliente.Sql += (" s.fktonomaterial<=@materialhasta ");

                    AgrupadoPorArticulo.Sql += (" AND ");
                    AgrupadoPorArticulo.Parameters.Add(new QueryParameter("materialhasta", typeof(string), MaterialHasta));
                    AgrupadoPorArticulo.Sql += (" s.fktonomaterial<=@materialhasta ");

                    AgrupadoPorCliente.Sql += (" AND ");
                    AgrupadoPorCliente.Parameters.Add(new QueryParameter("materialhasta", typeof(string), MaterialHasta));
                    AgrupadoPorCliente.Sql += (" s.fktonomaterial<=@materialhasta ");

                    AgrupadoPorProveedor.Sql += (" AND ");
                    AgrupadoPorProveedor.Parameters.Add(new QueryParameter("materialhasta", typeof(string), MaterialHasta));
                    AgrupadoPorProveedor.Sql += (" s.fktonomaterial<=@materialhasta ");
                }

            }
            else
            {
                mainQuery.Sql += " WHERE s.metrosentrada > 0 AND s.cantidadentrada > 0 AND l.metros > 0 AND l.importe > 0 AND s.netocompra > 0 AND s.lote != '' AND s.lote IS NOT NULL AND l.lote != '' AND l.lote IS NOT NULL ";

                DetalladoPorCliente.Sql += " WHERE s.metrosentrada > 0 AND s.cantidadentrada > 0 AND l.metros > 0 AND l.importe > 0 AND s.netocompra > 0 AND s.lote != '' AND s.lote IS NOT NULL AND l.lote != '' AND l.lote IS NOT NULL ";

                AgrupadoPorArticulo.Sql += " WHERE s.metrosentrada > 0 AND s.cantidadentrada > 0 AND l.metros > 0 AND l.importe > 0 AND s.netocompra > 0 AND s.lote != '' AND s.lote IS NOT NULL AND l.lote != '' AND l.lote IS NOT NULL ";

                AgrupadoPorCliente.Sql += " WHERE s.metrosentrada > 0 AND s.cantidadentrada > 0 AND l.metros > 0 AND l.importe > 0 AND s.netocompra > 0 AND s.lote != '' AND s.lote IS NOT NULL AND l.lote != '' AND l.lote IS NOT NULL ";

                AgrupadoPorProveedor.Sql += " WHERE s.metrosentrada > 0 AND s.cantidadentrada > 0 AND l.metros > 0 AND l.importe > 0 AND s.netocompra > 0 AND s.lote != '' AND s.lote IS NOT NULL AND l.lote != '' AND l.lote IS NOT NULL ";
            }

            mainQuery.Sql += " GROUP BY a.empresa, a.fechadocumento, s.fktonomaterial, l.lote, l.fkarticulos, l.descripcion, u.codigounidad, art.descripcionabreviada, f.descripcion, " +
                             " alc.fkproveedores, alc.nombreproveedor)t";

            DetalladoPorCliente.Sql += " GROUP BY a.empresa, a.fechadocumento, s.fktonomaterial, l.lote, l.fkarticulos, l.descripcion, u.codigounidad, art.descripcionabreviada, f.descripcion, " +
                             " a.fkclientes, a.nombrecliente, alc.fkproveedores, alc.nombreproveedor)t";

            AgrupadoPorArticulo.Sql += " GROUP BY a.empresa, a.fechadocumento, a.fkclientes, a.fkagentes, a.fkcomerciales, s.fktonomaterial, l.lote, s.loteid, l.fkarticulos, l.descripcion, u.codigounidad, " +
                             " l.cantidad, l.metros, s.largoentrada, s.anchoentrada, s.gruesoentrada, s.netocompra, s.metrosentrada, l.largo, l.ancho, l.grueso, l.importe, a.referencia, " +
                             " a.nombrecliente, ag.descripcion, c.descripcion, art.descripcionabreviada, f.descripcion, s.costeacicionalvariable, s.costeadicionalmaterial, " +
                             " s.costeadicionalotro, s.costeadicionalportes, alc.fkproveedores, alc.nombreproveedor)t" +
                             " GROUP BY [Cod.Artículo], [Descp.abreviada], [U.M.]";

            AgrupadoPorCliente.Sql += " GROUP BY a.empresa, a.fechadocumento, a.fkclientes, a.fkagentes, a.fkcomerciales, s.fktonomaterial, l.lote, s.loteid, l.fkarticulos, l.descripcion, u.codigounidad, " +
                             " l.cantidad, l.metros, s.largoentrada, s.anchoentrada, s.gruesoentrada, s.netocompra, s.metrosentrada, l.largo, l.ancho, l.grueso, l.importe, a.referencia, " +
                             " a.nombrecliente, ag.descripcion, c.descripcion, art.descripcionabreviada, f.descripcion, s.costeacicionalvariable, s.costeadicionalmaterial, " +
                             " s.costeadicionalotro, s.costeadicionalportes, alc.fkproveedores, alc.nombreproveedor)t" +
                             " GROUP BY [Cod.Cliente], [Cliente], [U.M.]";

            AgrupadoPorProveedor.Sql += " GROUP BY a.empresa, a.fechadocumento, a.fkclientes, a.fkagentes, a.fkcomerciales, s.fktonomaterial, l.lote, s.loteid, l.fkarticulos, l.descripcion, u.codigounidad, " +
                             "  l.cantidad, l.metros, s.largoentrada, s.anchoentrada, s.gruesoentrada, s.netocompra, s.metrosentrada, l.largo, l.ancho, l.grueso, l.importe, a.referencia, " +
                             "  a.nombrecliente, ag.descripcion, c.descripcion, art.descripcionabreviada, f.descripcion, s.costeacicionalvariable, s.costeadicionalmaterial, " +
                             " s.costeadicionalotro, s.costeadicionalportes, alc.fkproveedores, alc.nombreproveedor)t" +
                             " GROUP BY [Cod.Proveedor], [Proveedor], [U.M.]";

            DataSource.Queries.Add(new CustomSqlQuery("Empresa", "SELECT id, nombre FROM Empresas WHERE id = '" + user.Empresa + "'"));
            DataSource.Queries.Add(new CustomSqlQuery("Ejercicios", "SELECT empresa, id, descripcion FROM Ejercicios WHERE id = '" + user.Ejercicio + "'"));


            DataSource.Queries.Add(mainQuery);
            DataSource.Queries.Add(DetalladoPorCliente);
            DataSource.Queries.Add(AgrupadoPorArticulo);
            DataSource.Queries.Add(AgrupadoPorCliente);
            DataSource.Queries.Add(AgrupadoPorProveedor);

            DataSource.RebuildResultSchema();

        }

    }
}