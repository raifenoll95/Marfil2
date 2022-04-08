using Marfil.Dom.Persistencia.Listados.Base;
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
    public class ComparativaPerdidasYGananciasFuncional : ListadosModel
    {
        #region Properties
        public override string TituloListado => "Comparativa Pérdidas y Ganancias Funcional";

        public override string IdListado => FListadosModel.ComparativaPYGFuncional;

        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Ejercicioactual")]
        public string Ejercicio { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Ejercicioanterior")]
        public string Ejercicioanterior { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Guia")]
        public string Guia { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Lineassinsaldo")]
        public bool Lineassinsaldo { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Desglosarniveltres")]
        public bool Desglosarniveltres { get; set; }
        [Display(ResourceType = typeof(RPerdidasYGanancias), Name = "Recalculo")]
        public string Recalculo { get; set; }

        #endregion

        public ComparativaPerdidasYGananciasFuncional()
        {

        }

        public ComparativaPerdidasYGananciasFuncional(IContextService context) : base(context)
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
                sb.Append("SELECT cab.orden as Orden, cab.descrip as [Descripción], lin.cuenta as Cuenta, lin.saldo as Saldo, lin.saldoea as SaldoEA FROM ReportGuiasBalancesFuncional cab, ReportGuiasBalancesLineasFuncional lin");
            }
            else
            {
                sb.Append("Select orden as Orden, textogrupo as [Texto Grupo], descrip as [Descripción], saldo as Saldo, saldoea as SaldoEA from ReportGuiasBalancesFuncional");
            }

            return sb.ToString();
        }      

    }
}
