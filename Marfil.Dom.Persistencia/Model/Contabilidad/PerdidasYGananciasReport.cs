using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using System.Data.SqlClient;
using System.Data;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Collections.Generic;
using System;

namespace Marfil.Dom.Persistencia.Model.Documentos.Albaranes
{
    

    class PerdidasYGananciasReport: IReport
    {
        public SqlDataSource DataSource { get; private set; }
        
        public PerdidasYGananciasReport(IContextService user, Dictionary<string, object> dictionary = null)
        {

            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";            

            var mainQuery = new CustomSqlQuery("ReportGuiasBalances", "Select orden as Orden, textogrupo as [Texto Grupo], descrip as [Descripción], saldo as Saldo from ReportGuiasBalances");            

            if (dictionary != null)
            {
                var Ejercicio = dictionary["Ejercicio"].ToString();
                var Guia = dictionary["Guia"].ToString();
                var SinSaldo = dictionary["SinSaldo"].ToString();
                var Desglosar = dictionary["Desglosar"].ToString();
                Dictionary<string, object> ValoresParametros = new Dictionary<string, object>();

                ValoresParametros.Clear();

                //Inicializar los parámetros a NULL porque siempre tiene que llegar un valor
                ValoresParametros.Add("EMPRESA", user.Empresa);
                ValoresParametros.Add("EJERCICIO", DBNull.Value);
                ValoresParametros.Add("USUARIO_ACUMULADO", DBNull.Value);
                ValoresParametros.Add("GUIA", DBNull.Value);
                ValoresParametros.Add("SIN_SALDO", DBNull.Value);
                ValoresParametros.Add("NIVEL_TRES", DBNull.Value);


                if (!string.IsNullOrEmpty(Ejercicio))
                {
                    /*if (flag)
                        sb.Append(" AND ");*/
                    var paramEjercicio = Ejercicio.Split('-');
                    if (paramEjercicio.Length > 1)
                    {
                        ValoresParametros["USUARIO_ACUMULADO"] = paramEjercicio[1];
                    }
                    ValoresParametros["EJERCICIO"] = paramEjercicio[0];

                    //flag = true;
                }

                if (!string.IsNullOrEmpty(Guia))
                {
                    /*if (flag)
                        sb.Append(" AND ");*/

                    ValoresParametros["GUIA"] = Guia;

                    //flag = true;
                }

                if (SinSaldo == "false")
                {
                    mainQuery.Sql += "where saldo <> 0 or saldo is null";
                    
                    ValoresParametros["SIN_SALDO"] = false;

                    //flag = true;
                }

                if (Desglosar == "true")
                {
                    /*if (flag)
                        sb.Append(" AND ");*/

                    ValoresParametros["NIVEL_TRES"] = true;

                    //flag = true;
                }

                ExecuteProcedure(user.BaseDatos, ValoresParametros);
            }
                       
            DataSource.Queries.Add(new CustomSqlQuery("ReportGuiasBalancesLineas", "SELECT * FROM ReportGuiasBalancesLineas"));
            DataSource.Queries.Add(new CustomSqlQuery("CuentasNoAsignadas", "SELECT * FROM CuentasNoAsignadas"));

            DataSource.Queries.Add(mainQuery);

            DataSource.Relations.Add("ReportGuiasBalances", "ReportGuiasBalancesLineas", new[] {
                new RelationColumnInfo("Id", "GuiasBalancesId"),
                new RelationColumnInfo("InformeId", "InformeId"),
                new RelationColumnInfo("GuiaId", "GuiaId"),
                new RelationColumnInfo("orden", "orden")
            });


            DataSource.RebuildResultSchema();           
             
        }

        //Ejecutamos el procedimiento almacenado en BBDD para carga las tablas ReportGuiasBalances y Líneas con los filtros indicados
        private void ExecuteProcedure(string baseDatos, Dictionary<string, object> parametros)
        {
            var dbconnection = "";
            using (var db = MarfilEntities.ConnectToSqlServer(baseDatos))
            {
                dbconnection = db.Database.Connection.ConnectionString;
            }
            using (var con = new SqlConnection(dbconnection))
            {
                con.Open();
                using (var cmd = new SqlCommand("CARGAR_REPORTGUIAS", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    foreach (var item in parametros)
                    {
                        cmd.Parameters.AddWithValue(item.Key, item.Value);
                    }

                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
        }
    }
}
