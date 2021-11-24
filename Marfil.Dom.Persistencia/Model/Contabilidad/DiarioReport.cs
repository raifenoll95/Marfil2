using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Collections.Generic;
using System;

namespace Marfil.Dom.Persistencia.Model.Documentos.Albaranes
{
    

    class DiarioReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }
        
        public DiarioReport(IContextService user, Dictionary<string, object> dictionary = null)
        {

            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";            

            var mainQuery = new CustomSqlQuery("Movs", "select m.referencia as doc, " +  
                                                       "m.fecha as fecha, " +
                                                        /*"(CASE WHEN l.esdebe = 1 THEN l.fkcuentas ELSE '' END) AS cuenta_debe, " +
                                                        "(CASE WHEN l.esdebe = -1 THEN l.fkcuentas ELSE '' END) AS cuenta_haber, " +*/
                                                        "l.fkcuentas AS cuenta, " +
                                                        "c.descripcion AS descripcion, " +
		                                                "l.comentario AS comentario, " +
		                                                "(CASE WHEN l.esdebe = 1 THEN l.importe ELSE NULL END) AS debe, " +
                                                        "(CASE WHEN l.esdebe = -1 THEN l.importe ELSE NULL END) AS haber, " +
                                                        "l.orden as orden " +
                                                        "from movs as m, movslin as l, " +
                                                        "cuentas as c " +
                                                        "where m.empresa = l.empresa and m.id = l.fkmovs and m.empresa = c.empresa and c.id = l.fkcuentas");            

            if (dictionary != null)
            {
                var serie = dictionary["Serie"].ToString();
                var documentoDesde = dictionary["DocumentoDesde"].ToString();
                var documentoHasta = dictionary["DocumentoHasta"].ToString();
                var fechaDesde = dictionary["FechaDesde"];
                var fechaHasta = dictionary["FechaHasta"];

                var tipoAsiento = dictionary["TipoAsiento"].ToString();

                switch(tipoAsiento)
                {
                    case "Regularización existencias":
                        tipoAsiento = "R3";
                        break;
                    case "Simulación":
                        tipoAsiento = "F2";
                        break;
                    case "Asiento vinculado":
                        tipoAsiento = "F3";
                        break;
                    case "Normal":
                        tipoAsiento = "F1";
                        break;
                    case "Regularización grupos 6 y 7":
                        tipoAsiento = "R4";
                        break;
                    case "Cierre":
                        tipoAsiento = "R5";
                        break;
                    case "Apertura":
                        tipoAsiento = "R2";
                        break;
                    case "Apertura provisional":
                        tipoAsiento = "R1";
                        break;
                }

                var canalContable = dictionary["CanalContable"].ToString();
                var SumaAnteriorDebe = float.Parse(dictionary["SumaAnteriorDebe"].ToString());
                var SumaAnteriorHaber = float.Parse(dictionary["SumaAnteriorHaber"].ToString());
                                
                mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));
                mainQuery.Sql += " and m.empresa=@empresa";

                if (!string.IsNullOrEmpty(serie))
                {
                    mainQuery.Parameters.Add(new QueryParameter("serie", typeof(string), serie));
                    mainQuery.Sql += " AND m.fkseriescontables=@serie";
                }

                if (!string.IsNullOrEmpty(documentoDesde))
                {
                    mainQuery.Parameters.Add(new QueryParameter("documentoDesde", typeof(string), documentoDesde));
                    mainQuery.Sql += " AND m.referencia>=@documentoDesde";
                }

                if (!string.IsNullOrEmpty(documentoHasta))
                {
                    mainQuery.Parameters.Add(new QueryParameter("documentoHasta", typeof(string), documentoHasta));
                    mainQuery.Sql += " AND m.referencia<=@documentoHasta";
                }

                if (fechaDesde != null)
                {
                    var desglose = fechaDesde.ToString().Split('/');
                    fechaDesde = desglose[2] + "-" + desglose[1] + "-" + desglose[0];
                    mainQuery.Parameters.Add(new QueryParameter("fechaDesde", typeof(DateTime), fechaDesde));
                    mainQuery.Sql += " AND m.fecha>=@fechaDesde";
                }

                if (fechaHasta != null)
                {
                    var desglose = fechaHasta.ToString().Split('/');
                    fechaHasta = desglose[2] + "-" + desglose[1] + "-" + desglose[0];
                    mainQuery.Parameters.Add(new QueryParameter("fechaHasta", typeof(DateTime), fechaHasta));
                    mainQuery.Sql += " AND m.fecha<=@fechaHasta";
                }

                if (!string.IsNullOrEmpty(tipoAsiento))
                {
                    mainQuery.Parameters.Add(new QueryParameter("tipoAsiento", typeof(string), tipoAsiento));
                    mainQuery.Sql += " AND m.tipoasiento=@tipoAsiento";
                }

                if (!string.IsNullOrEmpty(canalContable))
                {
                    mainQuery.Parameters.Add(new QueryParameter("canalContable", typeof(string), canalContable));
                    mainQuery.Sql += " AND m.canalcontable=@canalContable";
                }

                if (SumaAnteriorDebe > 0 || SumaAnteriorHaber > 0)
                {
                    mainQuery.Parameters.Add(new QueryParameter("SumaAnteriorDebe", typeof(float), SumaAnteriorDebe));
                    mainQuery.Parameters.Add(new QueryParameter("SumaAnteriorHaber", typeof(float), SumaAnteriorHaber));
                    mainQuery.Sql += " UNION ";
                    mainQuery.Sql += " SELECT '' AS [Doc.], NULL AS[Fecha], '' AS[Cuenta Debe], '' AS[Cuenta Haber], '' AS[Descripcion], ";
                    mainQuery.Sql += " 'SUMA ANTERIOR' AS[Comentario], @SumaAnteriorDebe AS[Debe], @SumaAnteriorHaber AS[Haber], 0 AS [Orden] ";
                }
            }
                       
            DataSource.Queries.Add(new CustomSqlQuery("Empresa", "SELECT id, nombre FROM Empresas WHERE id = '" + user.Empresa + "'"));
            DataSource.Queries.Add(new CustomSqlQuery("Ejercicios", "SELECT empresa, id, descripcion FROM Ejercicios WHERE id = '" + user.Ejercicio + "'"));
            //DataSource.Queries.Add(new CustomSqlQuery("MovsLin", "SELECT * FROM MovsLin"));

            DataSource.Queries.Add(mainQuery);

            DataSource.Relations.Add("Movs", "Empresa", new[] {
                    new RelationColumnInfo("empresa", "id")});

            DataSource.Relations.Add("Movs", "Ejercicios", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("fkejercicio", "id")});

            /*DataSource.Relations.Add("Movs", "MovsLin", new[] {
                    new RelationColumnInfo("empresa", "empresa"),
                    new RelationColumnInfo("id", "fkmovs")});*/

            DataSource.RebuildResultSchema();           
             
        }
    }
}
