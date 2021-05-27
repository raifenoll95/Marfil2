using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.Helpers
{
    internal class ServiceHelper
    {

        #region contabilidad
        // contabilidad usa seriescontables en vez de series

        public static string GetNextIdContable<T>(MarfilEntities db, string empresa, string serie, TipoInicio tipoinicio = TipoInicio.Sinreinicio, DateTime? fecha = null) where T : class
        {
            string max;
            var columna = "identificadorsegmento";
            string whereReinicio = string.Empty;

            if (tipoinicio != TipoInicio.Sinreinicio && fecha.HasValue)
            {
                whereReinicio=" AND YEAR(fecha) = year(@fecha) ";

                if (tipoinicio == TipoInicio.Mensual)
                {
                    whereReinicio=" AND MONTH(fecha) = MONTH(@fecha) ";
                }
            }

            var query = string.Format("select max({2}) from {0} where fkseriescontables=@fkseries and empresa=@empresa {1}", typeof(T).Name, whereReinicio, columna);
            var parametros = new SqlParameter[]
            {
                new SqlParameter() {Value = serie, ParameterName = "@fkseries"},
                new SqlParameter() { Value = empresa, ParameterName = "@empresa" }
            };

            if (tipoinicio != TipoInicio.Sinreinicio && fecha.HasValue)
            {
                parametros = new SqlParameter[]
                {
                    new SqlParameter() {Value = serie, ParameterName = "@fkseries"},
                    new SqlParameter() { Value = empresa, ParameterName = "@empresa" },
                    new SqlParameter() { Value = fecha, ParameterName = "@fecha" }
                };
            }
            max = db.Database.SqlQuery<string>(query, parametros).SingleOrDefault();


            //using (var con = new SqlConnection(db.Database.Connection.ConnectionString))
            //{
            
            
                //using (var cmd = new SqlCommand(string.Format("select max({2}) from {0} where fkseriescontables=@fkseries and empresa=@empresa {1}", typeof(T).Name, GetCadenaFiltroContador(tipoinicio, fecha), columna), (SqlConnection)db.Database.Connection))
                //{
                //    cmd.Parameters.Add(new SqlParameter() { Value = serie, ParameterName = "@fkseries" });
                //    cmd.Parameters.Add(new SqlParameter() { Value = empresa, ParameterName = "@empresa" });
                //    if (tipoinicio != TipoInicio.Sinreinicio && fecha.HasValue)
                //        cmd.Parameters.Add(new SqlParameter() { Value = fecha, ParameterName = "@fecha" });

                //    if (cmd.Connection.State == ConnectionState.Closed)
                //    {
                //        cmd.Connection.Open();
                //    }
                //    max = Funciones.Qnull(cmd.ExecuteScalar());
                //    //using (var ad = new SqlDataAdapter(cmd))
                //    //{
                //    //    var table = new DataTable();
                //    //    ad.Fill(table);
                //    //    max = Funciones.Qnull(table.Rows[0][0]);
                //    //}
                //}
            //}
            var last = Funciones.Qint(max);
            var result = last.HasValue ? (last + 1).ToString() : string.Empty;
            var contador = db.SeriesContables.Single(f => f.empresa == empresa && f.id == serie).fkcontadores;
            var contadorObj = db.Contadores.Include("ContadoresLin").Single(f => f.empresa == empresa && f.id == contador);
            if (string.IsNullOrEmpty(max))
            {
                result = contadorObj.primerdocumento?.ToString() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(result))
                throw new Exception("No se puede generar el siguiente numero");

            var longitudcodigo = contadorObj.ContadoresLin.Single(f => f.tiposegmento == (int)TiposSegmentos.Secuencia).longitud ?? 0;
            return Funciones.RellenaCod(result, longitudcodigo);

        }

        public static string GetReferenceContable<T>(MarfilEntities db, string empresa, string fkseries, string fkcontadores, DateTime fecha, out string identificador) where T : class
        {
            var sb = new StringBuilder(); 
             var serie = db.SeriesContables.Single(f => f.empresa == empresa && f.id == fkseries);
            var contador = db.Contadores.Include("ContadoresLin")
                .Single(f => f.empresa == empresa && f.id == serie.fkcontadores);
            identificador = "";
            foreach (var item in contador.ContadoresLin)
            {
                if (item.tiposegmento.Value == (int)TiposSegmentos.Año)
                {
                    sb.Append(item.longitud == 4 ? fecha.Year.ToString() : fecha.Year.ToString().Substring(2));
                }
                else if ((int)TiposSegmentos.Constante == item.tiposegmento.Value)
                {
                    sb.Append(item.valor);
                }
                else if ((int)TiposSegmentos.Mes == item.tiposegmento.Value)
                {
                    sb.Append(Funciones.RellenaCod(fecha.Month.ToString(), 2));
                }
                else if ((int)TiposSegmentos.Secuencia == item.tiposegmento.Value)
                {
                    identificador = contador.tipoinicio != (int?)TipoInicio.Sinreinicio
                            ? GetNextIdContable<T>(db, empresa, fkseries, (TipoInicio)contador.tipoinicio, fecha)
                            : fkcontadores;

                    sb.Append(identificador);
                }
                else if ((int)TiposSegmentos.Serie == item.tiposegmento.Value)
                {
                    sb.Append(fkseries);
                }

            }

            return sb.ToString();
        }

        public static string GetReferenceContableMovimientosTesoreria<T>(MarfilEntities db, string empresa, string fkseries, string fkcontadores, DateTime fecha, out string identificador) where T : class
        {
            var sb = new StringBuilder();
            var serie = db.SeriesContables.Single(f => f.empresa == empresa && f.id == fkseries);
            var contador = db.Contadores.Include("ContadoresLin")
                .Single(f => f.empresa == empresa && f.id == serie.fkcontadores);
            identificador = "";
            foreach (var item in contador.ContadoresLin)
            {
                if (item.tiposegmento.Value == (int)TiposSegmentos.Año)
                {
                    sb.Append(item.longitud == 4 ? fecha.Year.ToString() : fecha.Year.ToString().Substring(2));
                }
                else if ((int)TiposSegmentos.Constante == item.tiposegmento.Value)
                {
                    sb.Append(item.valor);
                }
                else if ((int)TiposSegmentos.Mes == item.tiposegmento.Value)
                {
                    sb.Append(Funciones.RellenaCod(fecha.Month.ToString(), 2));
                }
                else if ((int)TiposSegmentos.Secuencia == item.tiposegmento.Value)
                {
                    identificador = contador.tipoinicio != (int?)TipoInicio.Sinreinicio
                            ? GetNextIdContableMovimientosTesoreria<T>(db, empresa, fkseries, (TipoInicio)contador.tipoinicio, fecha)
                            : fkcontadores;

                    sb.Append(identificador);
                }
                else if ((int)TiposSegmentos.Serie == item.tiposegmento.Value)
                {
                    sb.Append(fkseries);
                }

            }

            return sb.ToString();
        }


        #endregion



        public static string GetNextId<T>(MarfilEntities db,string empresa,string serie,TipoInicio tipoinicio=TipoInicio.Sinreinicio, DateTime? fecha = null) where T : class
        {
            string max;
            //using (SqlConnection con =  (SqlConnection)db.Database.Connection)// new SqlConnection(db.Database.Connection.ConnectionString))
            //{
            var columna = "identificadorsegmento";

            var query = string.Format("select max({2}) from {0} where fkseries=@fkseries and empresa=@empresa {1}", typeof(T).Name, GetCadenaFiltroContador(tipoinicio, fecha), columna);
            var parametros = new SqlParameter[]
            {
                new SqlParameter() {Value = serie, ParameterName = "@fkseries"},
                new SqlParameter() { Value = empresa, ParameterName = "@empresa" }
            };

            if (tipoinicio != TipoInicio.Sinreinicio && fecha.HasValue)
            {
                parametros = new SqlParameter[]
                {
                    new SqlParameter() {Value = serie, ParameterName = "@fkseries"},
                    new SqlParameter() { Value = empresa, ParameterName = "@empresa" },
                    new SqlParameter() { Value = fecha, ParameterName = "@fecha" }
                };
            }
            max = db.Database.SqlQuery<string>(query, parametros).SingleOrDefault();

            //using (var cmd = new SqlCommand(string.Format("select max({2}) from {0} where fkseries=@fkseries and empresa=@empresa {1}", typeof(T).Name, GetCadenaFiltroContador(tipoinicio,fecha), columna), (SqlConnection)db.Database.Connection))
            //{

            //cmd.Parameters.Add(new SqlParameter() {Value = serie, ParameterName = "@fkseries"});
            //cmd.Parameters.Add(new SqlParameter() { Value = empresa, ParameterName = "@empresa" });
            //if(tipoinicio != TipoInicio.Sinreinicio && fecha.HasValue)
            //    cmd.Parameters.Add(new SqlParameter() { Value = fecha, ParameterName = "@fecha" });

            //if(cmd.Connection.State == ConnectionState.Closed)
            //{
            //    cmd.Connection.Open();
            //}

            //max = Funciones.Qnull(cmd.ExecuteScalar());

            //using (var ad = new SqlDataAdapter(cmd))
            //{
            //    var table = new DataTable();
            //    ad.Fill(table);
            //    max = Funciones.Qnull(table.Rows[0][0]);
            //}
            //}
            //}
            var last = Funciones.Qint(max);
            var result = last.HasValue ? (last + 1).ToString(): string.Empty;
            var contador = db.Series.Single(f => f.empresa == empresa && f.id == serie).fkcontadores;
            var contadorObj = db.Contadores.Include("ContadoresLin").Single(f => f.empresa == empresa && f.id == contador);
            if (string.IsNullOrEmpty(max))
            {
                result = contadorObj.primerdocumento?.ToString() ?? string.Empty;
            }

            if(string.IsNullOrEmpty(result))
                throw new Exception("No se puede generar el siguiente numero");

            var longitudcodigo = contadorObj.ContadoresLin.Single(f => f.tiposegmento == (int) TiposSegmentos.Secuencia).longitud??0;
            return  Funciones.RellenaCod(result, longitudcodigo);
            
        }

        private static string GetCadenaFiltroContador(TipoInicio tipo, DateTime? fecha)
        {
            var result = string.Empty;

            if (tipo != TipoInicio.Sinreinicio && fecha.HasValue)
            {
                var sb=new StringBuilder();
                
                sb.AppendFormat(" AND YEAR(fechadocumento) = year(@fecha) ");
                
                if (tipo == TipoInicio.Mensual)
                {
                    sb.AppendFormat(" AND MONTH(fechadocumento) = MONTH(@fecha) ");
                }
                
                result = sb.ToString();
            }

            return result;
        }
        public static string GetReference<T>(MarfilEntities db, string empresa, string fkseries, string fkcontadores, DateTime fecha,out string identificador) where T : class
        {
            var sb=new StringBuilder();
            var serie = db.Series.Single(f => f.empresa == empresa && f.id == fkseries);
            var contador = db.Contadores.Include("ContadoresLin")
                .Single(f => f.empresa == empresa && f.id == serie.fkcontadores);
            identificador = "";
            foreach (var item in contador.ContadoresLin)
            {
                if (item.tiposegmento.Value == (int) TiposSegmentos.Año)
                {
                    sb.Append(item.longitud == 4? fecha.Year.ToString(): fecha.Year.ToString().Substring(2));
                }
                else if ((int) TiposSegmentos.Constante == item.tiposegmento.Value)
                {
                    sb.Append(item.valor);
                }
                else if ((int) TiposSegmentos.Mes == item.tiposegmento.Value)
                {
                    sb.Append(Funciones.RellenaCod(fecha.Month.ToString(), 2));
                }
                else if ((int) TiposSegmentos.Secuencia == item.tiposegmento.Value)
                {
                    identificador = contador.tipoinicio != (int?) TipoInicio.Sinreinicio
                            ? GetNextId<T>(db, empresa, fkseries, (TipoInicio) contador.tipoinicio, fecha)
                            : fkcontadores;

                    sb.Append(identificador);
                }
                else if ((int) TiposSegmentos.Serie == item.tiposegmento.Value)
                {
                    sb.Append(fkseries);
                }

            }

            return sb.ToString();
        }

        public static string GetNextIdContableMovimientosTesoreria<T>(MarfilEntities db, string empresa, string serie, TipoInicio tipoinicio = TipoInicio.Sinreinicio, DateTime? fecha = null) where T : class
        {
            string max;
            var columna = "identificadorsegmentoremesa";
            string whereReinicio = string.Empty;

            if (tipoinicio != TipoInicio.Sinreinicio && fecha.HasValue)
            {
                whereReinicio = " AND YEAR(fecha) = year(@fecha) ";

                if (tipoinicio == TipoInicio.Mensual)
                {
                    whereReinicio = " AND MONTH(fecha) = MONTH(@fecha) ";
                }
            }

            var query = string.Format("select max({2}) from {0} where fkseriescontablesremesa=@fkseries and empresa=@empresa {1}", typeof(T).Name, whereReinicio, columna);
            var parametros = new SqlParameter[]
            {
                new SqlParameter() {Value = serie, ParameterName = "@fkseries"},
                new SqlParameter() { Value = empresa, ParameterName = "@empresa" }
            };

            if (tipoinicio != TipoInicio.Sinreinicio && fecha.HasValue)
            {
                parametros = new SqlParameter[]
                {
                    new SqlParameter() {Value = serie, ParameterName = "@fkseries"},
                    new SqlParameter() { Value = empresa, ParameterName = "@empresa" },
                    new SqlParameter() { Value = fecha, ParameterName = "@fecha" }
                };
            }
            max = db.Database.SqlQuery<string>(query, parametros).SingleOrDefault();


            //using (var con = new SqlConnection(db.Database.Connection.ConnectionString))
            //{


            //using (var cmd = new SqlCommand(string.Format("select max({2}) from {0} where fkseriescontables=@fkseries and empresa=@empresa {1}", typeof(T).Name, GetCadenaFiltroContador(tipoinicio, fecha), columna), (SqlConnection)db.Database.Connection))
            //{
            //    cmd.Parameters.Add(new SqlParameter() { Value = serie, ParameterName = "@fkseries" });
            //    cmd.Parameters.Add(new SqlParameter() { Value = empresa, ParameterName = "@empresa" });
            //    if (tipoinicio != TipoInicio.Sinreinicio && fecha.HasValue)
            //        cmd.Parameters.Add(new SqlParameter() { Value = fecha, ParameterName = "@fecha" });

            //    if (cmd.Connection.State == ConnectionState.Closed)
            //    {
            //        cmd.Connection.Open();
            //    }
            //    max = Funciones.Qnull(cmd.ExecuteScalar());
            //    //using (var ad = new SqlDataAdapter(cmd))
            //    //{
            //    //    var table = new DataTable();
            //    //    ad.Fill(table);
            //    //    max = Funciones.Qnull(table.Rows[0][0]);
            //    //}
            //}
            //}
            var last = Funciones.Qint(max);
            var result = last.HasValue ? (last + 1).ToString() : string.Empty;
            var contador = db.SeriesContables.Single(f => f.empresa == empresa && f.id == serie).fkcontadores;
            var contadorObj = db.Contadores.Include("ContadoresLin").Single(f => f.empresa == empresa && f.id == contador);
            if (string.IsNullOrEmpty(max))
            {
                result = contadorObj.primerdocumento?.ToString() ?? string.Empty;
            }

            if (string.IsNullOrEmpty(result))
                throw new Exception("No se puede generar el siguiente numero");

            var longitudcodigo = contadorObj.ContadoresLin.Single(f => f.tiposegmento == (int)TiposSegmentos.Secuencia).longitud ?? 0;
            return Funciones.RellenaCod(result, longitudcodigo);

        }


    }
}
