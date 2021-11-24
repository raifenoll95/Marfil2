﻿using Marfil.Dom.Persistencia.Listados.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using System.Data.SqlClient;
using System.Data;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Helpers;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using RPerdidasYGanancias = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PerdidasYGanancias;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadoPerdidasYGanancias : ListadosModel
    {
        #region Properties
        public override string TituloListado => "Listado Pérdidas y Ganancias";

        public override string IdListado => FListadosModel.PerdidasYGanancias;

        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Ejercicio")]
        public string Ejercicio { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Guia")]
        public string Guia { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Lineassinsaldo")]
        public bool Lineassinsaldo { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Desglosarniveltres")]
        public bool Desglosarniveltres { get; set; }

        #endregion

        public ListadoPerdidasYGanancias()
        {

        }

        public ListadoPerdidasYGanancias(IContextService context) : base(context)
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

                ValoresParametros["GUIA"] = Guia;

                //flag = true;
            }

            if (!Lineassinsaldo)
            {
                if (flag)
                    sb.Append(" AND ");

                sb.Append(" saldo > 0");
                ValoresParametros["SIN_SALDO"] = Lineassinsaldo;

                flag = true;
            }

            if (Desglosarniveltres)
            {
                /*if (flag)
                    sb.Append(" AND ");*/

                ValoresParametros["NIVEL_TRES"] = Desglosarniveltres;

                //flag = true;
            }

            ExecuteProcedure(Context, ValoresParametros);
            return sb.ToString();
        }

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
            sb.Append("Select orden as Orden, textogrupo as [Texto Grupo], descrip as [Descripción], saldo as Saldo from ReportGuiasBalances");

            return sb.ToString();
        }

        //Ejecutamos el procedimiento almacenado en BBDD para carga las tablas ReportGuiasBalances y Líneas con los filtros indicados
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
