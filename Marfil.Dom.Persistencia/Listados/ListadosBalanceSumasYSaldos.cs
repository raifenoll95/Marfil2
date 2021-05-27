using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;
using RMovs = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.ControlsUI.Toolbar;
using Resources;

namespace Marfil.Dom.Persistencia.Listados
{

    public class ListadosBalanceSumasYSaldos : ListadosModel
    {

        #region CTR

        public ListadosBalanceSumasYSaldos()
        {
        }

        public ListadosBalanceSumasYSaldos(IContextService context) : base(context)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(context.BaseDatos))
            {
                var idEjercicio = Convert.ToInt32(context.Ejercicio);
                InicioEjercicio = db.Ejercicios.Where(f => f.empresa == context.Empresa && f.id == idEjercicio).Select(f => f.desde).SingleOrDefault();
            }
            FechaInforme = DateTime.Today;
        }

        #endregion

        public override string TituloListado => "Balance de sumas y saldos";
        public override string IdListado => FListadosModel.SumasYSaldos;

        #region Propierties        

        [Display(ResourceType = typeof(RMovs), Name = "FechaInforme")]
        public DateTime? FechaInforme { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "CuentaDesde")]
        public string CuentaDesde { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "CuentaHasta")]
        public string CuentaHasta { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "PorGrupos")]
        public bool PorGrupos { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "PorSubgrupos")]
        public bool PorSubgrupos { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "PorMayor")]
        public bool PorMayor { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "PorSubmayor")]
        public bool PorSubmayor { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "PorNivelCinco")]
        public bool PorNivelCinco { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "PorSubcuenta")]
        public bool PorSubcuenta { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "MostrarCuentasSinSaldo")]
        public bool MostrarCuentasSinSaldo { get; set; }

        [Display(ResourceType = typeof(RMovs), Name = "MostrarCuentasSinMovimientos")]
        public bool MostrarCuentasSinMovimientos { get; set; }

        public DateTime? InicioEjercicio { get; set; }

        #endregion Propierties

        #region Filters

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            ValoresParametros.Clear();
            Condiciones.Clear();

            Condiciones.Add(string.Format("{0}: {1}", RMovs.FechaInforme, FechaInforme));


            //CuentaDesde y CuentaHasta. Hacer los @
            CuentaDesde = (CuentaDesde + "00000000000000000000000000000000000").Substring(0, 8);
            CuentaHasta = (CuentaHasta + "00000000000000000000000000000000000").Substring(0, 8);

            if (!string.IsNullOrEmpty(CuentaDesde))
            {
                ValoresParametros.Add("cuentadesde", CuentaDesde);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.CuentaDesde, CuentaDesde));
            }

            if (!string.IsNullOrEmpty(CuentaHasta))
            {

                ValoresParametros.Add("cuentahasta", CuentaHasta);
                Condiciones.Add(string.Format("{0}: {1}", RMovs.CuentaHasta, CuentaHasta));
            }

            sb.Append("c.empresa = '" + Empresa + "' and(m.fkejercicio = '" + Context.Ejercicio + "'  or m.fkejercicio is null) ");
            sb.Append(" AND ( ");

            var cuentaDesdeAux1 = CuentaDesde.ToString().Substring(0, 1);
            var cuentaDesdeAux2 = CuentaDesde.ToString().Substring(0, 2);
            var cuentaDesdeAux3 = CuentaDesde.ToString().Substring(0, 3);
            var cuentaDesdeAux4 = CuentaDesde.ToString().Substring(0, 4);

            if (PorGrupos)
            {
                sb.Append("((c.id='" + cuentaDesdeAux1 + "') or (c.id>='" + CuentaDesde + "' and c.id<='" + CuentaHasta + "' and c.nivel=1)) ");
            }

            if (PorSubgrupos)
            {
                if (PorGrupos)
                {
                    sb.Append(" or ");
                }

                sb.Append("((c.id='" + cuentaDesdeAux2 + "') or (c.id>='" + CuentaDesde + "' and c.id<='" + CuentaHasta + "' and c.nivel=2)) ");
            }

            if (PorMayor)
            {
                if (PorGrupos || PorSubgrupos)
                {
                    sb.Append(" or ");
                }

                sb.Append("((c.id='" + cuentaDesdeAux3 + "') or (c.id>='" + CuentaDesde + "' and c.id<='" + CuentaHasta + "' and c.nivel=3)) ");
            }

            if (PorSubmayor)
            {
                if (PorGrupos || PorSubgrupos || PorMayor)
                {
                    sb.Append(" or ");
                }

                sb.Append("((c.id='" + cuentaDesdeAux4 + "') or (c.id>='" + CuentaDesde + "' and c.id<='" + CuentaHasta + "' and c.nivel=4)) ");
            }

            if (PorSubcuenta)
            {
                if (PorGrupos || PorSubgrupos || PorMayor || PorSubmayor)
                {
                    sb.Append(" or ");
                }

                sb.Append("(c.id>='" + CuentaDesde + "' and c.id<='" + CuentaHasta + "' and c.nivel=0) ");
            }

            sb.Append(")");

            //En caso de que el usuario quiera mostrar cuentas sin saldo, se mostraran las de con saldo y la de sin saldo
            if (!MostrarCuentasSinSaldo)
            {
                sb.Append(" and (m.debe<>m.haber)");
            }

            if (!MostrarCuentasSinMovimientos)
            {

                sb.Append("AND (m.id is not null)");

            }

            return sb.ToString();
        }

        #endregion Filters

        #region Select

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();



            sb.AppendFormat("select m.fkcuentas as Cuenta, c.descripcion as Descripcion, m.debe as Debe, m.haber as Haber ," +
                                                "(case when m.saldo >= 0 THEN m.saldo else null END) AS Deudor ," +
                                                "(case when m.saldo < 0 THEN(m.saldo * -1) else null END) AS Acreedor from cuentas as c left join maes as m on c.id = m.fkcuentas ");

            return sb.ToString();
        }

        #endregion

        #region Orden

        internal override string GenerarOrdenColumnas()
        {
            return " ORDER BY Cuenta";
        }

        #endregion

    }
}
