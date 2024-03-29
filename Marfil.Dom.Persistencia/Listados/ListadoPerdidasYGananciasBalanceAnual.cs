﻿using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPerdidasYGanancias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PerdidasYGanancias;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadoPerdidasYGananciasBalanceAnual : ListadosModel
    {
        #region Properties
        public override string TituloListado => "Balance de cuentas anuales";

        public override string IdListado => FListadosModel.PerdidasYGananciasBalanceAnual;

        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Ejercicio")]
        public string Ejercicio { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Guia")]
        public string Guia { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Lineassinsaldo")]
        public bool Lineassinsaldo { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Desglosarniveltres")]
        public bool Desglosarniveltres { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Recalculo")]
        public string Recalculo { get; set; }

        #endregion

        public ListadoPerdidasYGananciasBalanceAnual()
        {

        }

        public ListadoPerdidasYGananciasBalanceAnual(IContextService context) : base(context)
        {

        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            var flag = false;
            ValoresParametros.Clear();
            Condiciones.Clear();
            //sb.Append(" empresa = '" + Empresa + "' ");

            //Inicializar los parámetros a NULL porque siempre tiene que llegar un valor
            ValoresParametros.Add("EMPRESA", Empresa);
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

                //En la tabla el orden empieza en 0. se resta uno al valor.
                var valorguia = int.Parse(Guia) - 1;

                ValoresParametros["GUIA"] = valorguia.ToString();

                //flag = true;
            }

            if (Desglosarniveltres)
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["NIVEL_TRES"] = Desglosarniveltres;

                //flag = true;
            }

            if (!Lineassinsaldo)
            {
                if (flag)
                    sb.Append(" AND ");

                if (Desglosarniveltres)
                {
                    sb.Append(" cab.Id = lin.GuiasBalancesId and cab.GuiaId = lin.GuiaId and cab.InformeId = lin.InformeId and cab.orden = lin.orden and lin.saldo <> 0");
                }
                else
                {
                    sb.Append(" saldo <> 0 or saldo is null");
                }

                ValoresParametros["SIN_SALDO"] = Lineassinsaldo;

                flag = true;
            }
            else
            {
                if (flag)
                    sb.Append(" AND ");

                if (Desglosarniveltres)
                {
                    sb.Append(" cab.Id = lin.GuiasBalancesId and cab.GuiaId = lin.GuiaId and cab.InformeId = lin.InformeId and cab.orden = lin.orden");
                }
                else
                {
                    sb.Append(" saldo <> 0 or saldo = 0 or saldo is null");
                }

                ValoresParametros["SIN_SALDO"] = Lineassinsaldo;

                flag = true;
            }

            //ExecuteProcedure(Context, ValoresParametros);
            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
            if (Desglosarniveltres)
            {
                sb.Append("SELECT cab.actpas as [Activo/Pasivo], cab.orden as Orden, cab.textogrupo as [Texto Grupo], cab.descrip as [Descripción], lin.cuenta as Cuenta, lin.saldo as Saldo FROM ReportGuiasBalancesBalanceAnual cab, ReportGuiasBalancesLineasBalanceAnual lin");
            }
            else
            {
                sb.Append("Select actpas as [Activo/Pasivo], orden as Orden, textogrupo as [Texto Grupo], descrip as [Descripción], saldo as Saldo from ReportGuiasBalancesBalanceAnual");
            }

            return sb.ToString();
        }

        internal override string GenerarOrdenColumnas()
        {
            var sb = new StringBuilder();
            sb.Append(" Order by actpas, orden ");
            return sb.ToString();

        }

        //Ejecutamos el procedimiento almacenado en BBDD para carga las tablas ReportGuiasBalances y Líneas con los filtros indicados
        //Este proceso se hace con un botón desde la pantalla ahora, se mantiene aquí este ejemplo por si acaso
        private void ExecuteProcedure(IContextService context, Dictionary<string, object> parametros)
        {
            var dbconnection = "";
            using (var db = MarfilEntities.ConnectToSqlServer(context.BaseDatos))
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
