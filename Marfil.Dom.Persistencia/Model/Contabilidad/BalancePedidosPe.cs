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

namespace Marfil.Dom.Persistencia.Model.Documentos.Albaranes
{

    class BalancePedidosPe : IReport
    {

        public SqlDataSource DataSource { get; private set; }

       

        public BalancePedidosPe(IContextService user, Dictionary<string, object> dictionary = null)
        {

            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";

            var mainQuery = new CustomSqlQuery("Pedidos", "SELECT p.referencia AS Referencia, (p.fkclientes + ' ' + p.nombrecliente) AS Cliente," +
               " p.fechadocumento AS Fecha, (l.fkarticulos + ' ' + l.descripcion) AS Código," +
               " l.cantidad AS Piezas, l.metros AS Metros, u.textocorto AS Unidad," +
               " (CASE WHEN l.largo > 0 AND l.ancho > 0 THEN(l.cantidad * l.largo * l.ancho) ELSE NULL END) AS M2," +
               " l.precio AS Precio, l.porcentajedescuento AS[% Dto], l.importe AS Total" +
               " FROM Pedidos AS p" +
               " LEFT JOIN PedidosLin AS l ON l.empresa = p.empresa AND l.fkpedidos = p.id" +
               " LEFT JOIN Unidades AS u ON u.empresa = p.empresa AND u.id = l.fkunidades");

            var albaranesQuery = new CustomSqlQuery("Albaranes", "SELECT a.referencia AS Referencia, a.fechadocumento AS Fecha, (l.fkarticulos + ' ' + l.descripcion) AS Código,"+
               " l.cantidad AS Cantidad, l.metros AS Metros, u.textocorto AS Unidad,"+
               " (CASE WHEN l.largo > 0 AND l.ancho > 0 THEN(l.cantidad * l.largo * l.ancho) ELSE NULL END) AS M2,"+
               " l.precio AS Precio, l.importe AS Total"+
               " FROM Albaranes AS a "+
               " LEFT JOIN Pedidos AS p ON p.empresa = a.empresa AND p.referencia = a.fkpedidos "+
               " LEFT JOIN AlbaranesLin AS l ON l.empresa = a.empresa AND l.fkalbaranes = a.id " +
               " LEFT JOIN Unidades AS u ON u.empresa = a.empresa AND u.id = l.fkunidades");

            var costesQuery = new CustomSqlQuery("PedidosCostesFabricacion", "SELECT(t.id + ' ' + t.descripcion) AS[Tarea realizada],"+
               " c.cantidad AS Cantidad, c.precio AS Precio, c.total AS Total "+
               " FROM PedidosCostesFabricacion c "+
               " LEFT JOIN Pedidos AS p ON p.empresa = c.empresa AND p.id = c.fkpedido "+
               " LEFT JOIN Tareas AS t ON t.empresa = c.empresa AND t.id = c.fktarea");

            if (dictionary != null)
            {
                var Series = dictionary["Series"].ToString();
                var Estado = dictionary["Estado"].ToString();
                var PedidoDesde = dictionary["PedidoDesde"].ToString();
                var PedidoHasta = dictionary["PedidoHasta"].ToString();
                var fechaDesde = dictionary["FechaDesde"];
                var fechaHasta = dictionary["FechaHasta"];
                var ClienteDesde = dictionary["ClienteDesde"].ToString();
                var ClienteHasta = dictionary["ClienteHasta"].ToString();
                var FechaInforme = dictionary["FechaInforme"];


                // Condiciones.Clear();
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Sql += " WHERE p.empresa = '" + user.Empresa + "'";
                albaranesQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                albaranesQuery.Sql += " WHERE p.empresa = '" + user.Empresa + "'"+"AND tipoalbaran = 4";
                costesQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                costesQuery.Sql += " WHERE p.empresa = '" +  user.Empresa + "'";


                if (!string.IsNullOrEmpty(PedidoDesde))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("pedidodesde", typeof(string), PedidoDesde));
                    mainQuery.Sql += (" p.referencia>=@pedidodesde ");
                    //albaranesQuery
                    albaranesQuery.Sql += (" AND ");
                    albaranesQuery.Parameters.Add(new QueryParameter("pedidodesde", typeof(string), PedidoDesde));
                    albaranesQuery.Sql += (" p.referencia>=@pedidodesde ");
                    //costesQuery
                    costesQuery.Sql += (" AND ");
                    costesQuery.Parameters.Add(new QueryParameter("pedidodesde", typeof(string), PedidoDesde));
                    costesQuery.Sql += (" p.referencia>=@pedidodesde ");
                    
                }

                if (!string.IsNullOrEmpty(PedidoHasta))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("pedidohasta", typeof(string), PedidoHasta));
                    mainQuery.Sql += (" p.referencia<=@pedidohasta ");
                    //albaranesQuery
                    albaranesQuery.Sql += (" AND ");
                    albaranesQuery.Parameters.Add(new QueryParameter("pedidohasta", typeof(string), PedidoHasta));
                    albaranesQuery.Sql += (" p.referencia<=@pedidohasta ");
                    //costesQuery
                    costesQuery.Sql += (" AND ");
                    costesQuery.Parameters.Add(new QueryParameter("pedidohasta", typeof(string), PedidoHasta));
                    costesQuery.Sql += (" p.referencia<=@pedidohasta ");
                }

                if (fechaDesde != null)
                {
                    //mainQuery
                    mainQuery.Parameters.Add(new QueryParameter("fechaDesde", typeof(DateTime), fechaDesde));
                    mainQuery.Sql += " AND p.fechadocumento>=@fechaDesde";
                    //albaranesQuery
                    albaranesQuery.Parameters.Add(new QueryParameter("fechaDesde", typeof(DateTime), fechaDesde));
                    albaranesQuery.Sql += " AND p.fechadocumento>=@fechaDesde";
                    //costesQuery
                    costesQuery.Parameters.Add(new QueryParameter("fechaDesde", typeof(DateTime), fechaDesde));
                    costesQuery.Sql += " AND p.fechadocumento>=@fechaDesde";
                }

                if (fechaHasta != null)
                {
                    //mainQuery
                    mainQuery.Parameters.Add(new QueryParameter("fechaHasta", typeof(DateTime), fechaHasta));
                    mainQuery.Sql += " AND p.fechadocumento<=@fechaHasta";
                    //albaranesQuery
                    albaranesQuery.Parameters.Add(new QueryParameter("fechaHasta", typeof(DateTime), fechaHasta));
                    albaranesQuery.Sql += " AND p.fechadocumento<=@fechaHasta";
                    //costesQuery
                    costesQuery.Parameters.Add(new QueryParameter("fechaHasta", typeof(DateTime), fechaHasta));
                    costesQuery.Sql += " AND p.fechadocumento<=@fechaHasta";
                }

                if (!string.IsNullOrEmpty(ClienteDesde))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("clientedesde", typeof(string), ClienteDesde));
                    mainQuery.Sql += (" p.fkclientes>=@clientedesde ");
                    //albaranesQuery
                    albaranesQuery.Sql += (" AND ");
                    albaranesQuery.Parameters.Add(new QueryParameter("clientedesde", typeof(string), ClienteDesde));
                    albaranesQuery.Sql += (" p.fkclientes>=@clientedesde ");
                    //costesQuery
                    costesQuery.Sql += (" AND ");
                    costesQuery.Parameters.Add(new QueryParameter("clientedesde", typeof(string), ClienteDesde));
                    costesQuery.Sql += (" p.fkclientes>=@clientedesde ");

                }

                if (!string.IsNullOrEmpty(ClienteHasta))
                {
                    //mainQuery
                    mainQuery.Sql += (" AND ");
                    mainQuery.Parameters.Add(new QueryParameter("clientehasta", typeof(string), ClienteHasta));
                    mainQuery.Sql += (" p.fkclientes<=@clientehasta ");
                    //albaranesQuery
                    albaranesQuery.Sql += (" AND ");
                    albaranesQuery.Parameters.Add(new QueryParameter("clientehasta", typeof(string), ClienteHasta));
                    albaranesQuery.Sql += (" p.fkclientes<=@clientehasta ");
                    //costesQuery
                    costesQuery.Sql += (" AND ");
                    costesQuery.Parameters.Add(new QueryParameter("clientehasta", typeof(string), ClienteHasta));
                    costesQuery.Sql += (" p.fkclientes<=@clientehasta ");

                }







                //mainQuery.Sql += ")";

            }
            else
            {

               // mainQuery.Sql += (" GROUP BY c.empresa, c.id, c.descripcion");
               // mainQuery.Sql += (")t");
            }




           DataSource.Queries.Add(new CustomSqlQuery("Empresa", "SELECT id, nombre FROM Empresas WHERE id = '" + user.Empresa + "'"));
           DataSource.Queries.Add(new CustomSqlQuery("Ejercicios", "SELECT empresa, id, descripcion FROM Ejercicios WHERE id = '" + user.Ejercicio + "'"));


            DataSource.Queries.Add(mainQuery);
            DataSource.Queries.Add(albaranesQuery);
            DataSource.Queries.Add(costesQuery);

            //DataSource.Relations.Add("Pedidos", "Empresa", new[] {
            //        new RelationColumnInfo("empresa", "id")});



            DataSource.RebuildResultSchema();

        }
    }
}
