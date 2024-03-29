﻿using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using System.Data.SqlClient;
using System.Data;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.Collections.Generic;
using System;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;

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

            var mainQuery = new CustomSqlQuery("ReportGuiasBalances", "Select * from ReportGuiasBalances");
            var mainQuery2 = new CustomSqlQuery("ReportAnaliticaGuiasBalances", "Select * from ReportAnaliticaGuiasBalances");
            var mainQuery3 = new CustomSqlQuery("ReportGuiasBalancesFuncional", "Select * from ReportGuiasBalancesFuncional");
            var mainQuery4 = new CustomSqlQuery("ReportGuiasBalancesBalanceAnual", "Select * from ReportGuiasBalancesBalanceAnual");

            if (dictionary != null)
            {
                var Ejercicio = dictionary["Ejercicio"].ToString();
                var Ejercicioanterior = dictionary["Ejercicioanterior"].ToString();
                var Guia = dictionary["Guia"].ToString();
                var SinSaldo = dictionary["SinSaldo"].ToString();
                var Desglosar = dictionary["Desglosar"].ToString();
                Dictionary<string, object> ValoresParametros = new Dictionary<string, object>();

                ValoresParametros.Clear();

                //Inicializar los parámetros a NULL porque siempre tiene que llegar un valor
                ValoresParametros.Add("EMPRESA", user.Empresa);
                ValoresParametros.Add("EJERCICIO", DBNull.Value);
                ValoresParametros.Add("USUARIO_ACUMULADO", DBNull.Value);
                ValoresParametros.Add("EJERCICIO_ANT", DBNull.Value);
                ValoresParametros.Add("USUARIO_ACUMULADO_ANT", DBNull.Value);
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

                    //Pasar el parámetro para se pueda usar bien en el Report
                    /*using (var service = FService.Instance.GetService(typeof(GuiasBalancesModel), user) as GuiasBalancesService)
                    {
                        Ejercicio = service.EjercicioParametro(paramEjercicio[0]);
                    }*/

                    //flag = true;
                }

                if (!string.IsNullOrEmpty(Ejercicioanterior))
                {
                    /*if (flag)
                        sb.Append(" AND ");*/
                    var paramEjercicio = Ejercicioanterior.Split('-');
                    if (paramEjercicio.Length > 1)
                    {
                        ValoresParametros["USUARIO_ACUMULADO_ANT"] = paramEjercicio[1];
                    }
                    ValoresParametros["EJERCICIO_ANT"] = paramEjercicio[0];

                    //flag = true;
                }

                if (!string.IsNullOrEmpty(Guia))
                {
                    /*if (flag)
                        sb.Append(" AND ");*/

                    //En la tabla el orden empieza en 0. se resta uno al valor.
                    var valorguia = int.Parse(Guia) - 1;

                    ValoresParametros["GUIA"] = valorguia.ToString();

                    //flag = true;
                }

                if (SinSaldo == "false")
                {
                    mainQuery.Sql += "where saldo <> 0 or saldo is null";
                    mainQuery2.Sql += "where saldo <> 0 or saldo is null";
                    mainQuery3.Sql += "where saldo <> 0 or saldo is null";
                    mainQuery4.Sql += "where saldo <> 0 or saldo is null";

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

                //ExecuteProcedure(user.BaseDatos, ValoresParametros);
            }
                       
            DataSource.Queries.Add(new CustomSqlQuery("ReportGuiasBalancesLineas", "SELECT * FROM ReportGuiasBalancesLineas"));
            DataSource.Queries.Add(new CustomSqlQuery("CuentasNoAsignadas", "SELECT * FROM CuentasNoAsignadas"));
            DataSource.Queries.Add(new CustomSqlQuery("ReportAnaliticaGuiasBalancesLineas", "SELECT * FROM ReportAnaliticaGuiasBalancesLineas"));
            DataSource.Queries.Add(new CustomSqlQuery("CuentasNoAsignadasAnalitica", "SELECT * FROM CuentasNoAsignadasAnalitica"));
            DataSource.Queries.Add(new CustomSqlQuery("ReportGuiasBalancesLineasFuncional", "SELECT * FROM ReportGuiasBalancesLineasFuncional"));
            DataSource.Queries.Add(new CustomSqlQuery("CuentasNoAsignadasFuncional", "SELECT * FROM CuentasNoAsignadasFuncional"));
            DataSource.Queries.Add(new CustomSqlQuery("ReportGuiasBalancesLineasBalanceAnual", "SELECT * FROM ReportGuiasBalancesLineasBalanceAnual"));
            DataSource.Queries.Add(new CustomSqlQuery("CuentasNoAsignadasBalanceAnual", "SELECT * FROM CuentasNoAsignadasBalanceAnual"));

            DataSource.Queries.Add(new CustomSqlQuery("Ejercicios", "SELECT empresa, id, descripcion, descripcioncorta FROM Ejercicios WHERE id = '" + user.Ejercicio + "'"));

            DataSource.Queries.Add(mainQuery);

            DataSource.Relations.Add("ReportGuiasBalances", "ReportGuiasBalancesLineas", new[] {
                new RelationColumnInfo("Id", "GuiasBalancesId"),
                new RelationColumnInfo("InformeId", "InformeId"),
                new RelationColumnInfo("GuiaId", "GuiaId"),
                new RelationColumnInfo("orden", "orden")
            });

            DataSource.Queries.Add(mainQuery2);

            DataSource.Relations.Add("ReportAnaliticaGuiasBalances", "ReportAnaliticaGuiasBalancesLineas", new[] {
                new RelationColumnInfo("Id", "GuiasBalancesId"),
                new RelationColumnInfo("InformeId", "InformeId"),
                new RelationColumnInfo("GuiaId", "GuiaId"),
                new RelationColumnInfo("orden", "orden")
            });

            DataSource.Queries.Add(mainQuery3);

            DataSource.Relations.Add("ReportGuiasBalancesFuncional", "ReportGuiasBalancesLineasFuncional", new[] {
                new RelationColumnInfo("Id", "GuiasBalancesId"),
                new RelationColumnInfo("InformeId", "InformeId"),
                new RelationColumnInfo("GuiaId", "GuiaId"),
                new RelationColumnInfo("orden", "orden")
            });

            DataSource.Queries.Add(mainQuery4);

            DataSource.Relations.Add("ReportGuiasBalancesBalanceAnual", "ReportGuiasBalancesLineasBalanceAnual", new[] {
                new RelationColumnInfo("Id", "GuiasBalancesId"),
                new RelationColumnInfo("InformeId", "InformeId"),
                new RelationColumnInfo("GuiaId", "GuiaId"),
                new RelationColumnInfo("orden", "orden")
            });

            DataSource.RebuildResultSchema();           
             
        }

        //Ejecutamos el procedimiento almacenado en BBDD para carga las tablas ReportGuiasBalances y Líneas con los filtros indicados
        //Este proceso se hace con un botón desde la pantalla ahora, se mantiene aquí este ejemplo por si acaso
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
