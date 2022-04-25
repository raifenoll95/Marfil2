using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.DataAccess.Sql;
using System.Configuration;
using DevExpress.DataAccess.ConnectionParameters;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using DevExpress.XtraReports;
using Marfil.Dom.Persistencia.Helpers;


using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.Model.Documentos.Albaranes
{

    class SumasYSaldosReport : IReport
    {

        public SqlDataSource DataSource { get; private set; }

        public SumasYSaldosReport(IContextService user, Dictionary<string, object> dictionary = null)
        {

            var server = ConfigurationManager.AppSettings["Server"];
            var usuario = ConfigurationManager.AppSettings["User"];
            var password = ConfigurationManager.AppSettings["Password"];
            DataSource = new SqlDataSource("Report", new MsSqlConnectionParameters(server, user.BaseDatos, usuario, password, MsSqlAuthorizationType.SqlServer));
            DataSource.Name = "Report";

            var mainQuery = new CustomSqlQuery("Cuentas", "select m.fkcuentas as Cuenta, c.descripcion as Descripcion, m.debe as Debe, m.haber as Haber ," +
                                                "(case when m.saldo >= 0 THEN m.saldo else null END) AS Deudor , " +
                                                "(case when m.saldo < 0 THEN(m.saldo * -1) else null END) AS Acreedor, " +
                                                "c.nivel as Nivel  " +
                                                "from cuentas as c left join maes as m on c.id = m.fkcuentas " +
                                                "where c.empresa = @empresa and(m.fkejercicio = @fkejercicio or m.fkejercicio is null) ");

            mainQuery.Parameters.Add(new QueryParameter("fkejercicio", typeof(string), user.Ejercicio));
            mainQuery.Parameters.Add(new QueryParameter("empresa", typeof(string), user.Empresa));

            if (dictionary != null)
            {

                var CuentaDesde = dictionary["CuentaDesde"].ToString();
                var CuentaHasta = dictionary["CuentaHasta"].ToString();
                var PorGrupos = Convert.ToBoolean(dictionary["PorGrupos"]);
                var PorSubgrupos = Convert.ToBoolean(dictionary["PorSubgrupos"]);
                var PorMayor = Convert.ToBoolean(dictionary["PorMayor"]);
                var PorSubmayor = Convert.ToBoolean(dictionary["PorSubmayor"]);
                var PorNivelCinco = Convert.ToBoolean(dictionary["PorNivelCinco"]);
                var PorSubcuenta = Convert.ToBoolean(dictionary["PorSubcuenta"]);
                var MostrarCuentasSinSaldo = Convert.ToBoolean(dictionary["MostrarCuentasSinSaldo"]);
                var MostrarCuentasSinMovimientos = Convert.ToBoolean(dictionary["MostrarCuentasSinMovimientos"]);
                var FechaInforme = dictionary["FechaInforme"];

                //Obtenemos el numero de digitos de la cuenta que tiene la empresa
                var service = FService.Instance.GetService(typeof(EmpresaModel),user);
                var empresaModel = service.get(user.Empresa) as EmpresaModel;
                int digitos = Funciones.Qint(empresaModel.DigitosCuentas).Value;

                CuentaDesde = (CuentaDesde + "00000000000000000000000000000000000").Substring(0, digitos);
                CuentaHasta = (CuentaHasta + "99999999999999999999999999999999999").Substring(0, digitos);

                var cuentaDesdeAux1 = CuentaDesde.ToString().Substring(0, 1);
                var cuentaDesdeAux2 = CuentaDesde.ToString().Substring(0, 2);
                var cuentaDesdeAux3 = CuentaDesde.ToString().Substring(0, 3);
                var cuentaDesdeAux4 = CuentaDesde.ToString().Substring(0, 4);

                mainQuery.Sql += ("AND ( ");

                if(PorGrupos)
                {
                    mainQuery.Sql += ("((c.id='" + cuentaDesdeAux1 + "') or (c.id>='" + CuentaDesde + "' and c.id<='" + CuentaHasta + "' and c.nivel=1)) ");
                }

                if(PorSubgrupos)
                {
                    if(PorGrupos)
                    {
                        mainQuery.Sql += (" or ");
                    }

                    mainQuery.Sql += ("((c.id='" + cuentaDesdeAux2 + "') or (c.id>='" + CuentaDesde + "' and c.id<='" + CuentaHasta + "' and c.nivel=2)) ");
                }

                if(PorMayor)
                {
                    if(PorGrupos || PorSubgrupos)
                    {
                        mainQuery.Sql += (" or ");
                    }

                    mainQuery.Sql += ("((c.id='" + cuentaDesdeAux3 + "') or (c.id>='" + CuentaDesde + "' and c.id<='" + CuentaHasta + "' and c.nivel=3)) ");
                }

                if(PorSubmayor)
                {
                    if (PorGrupos || PorSubgrupos || PorMayor)
                    {
                        mainQuery.Sql += (" or ");
                    }

                    mainQuery.Sql += ("((c.id='" + cuentaDesdeAux4 + "') or (c.id>='" + CuentaDesde + "' and c.id<='" + CuentaHasta + "' and c.nivel=4)) ");
                }

                if (PorSubcuenta)
                {
                    if (PorGrupos || PorSubgrupos || PorMayor || PorSubmayor)
                    {
                        mainQuery.Sql += (" or ");
                    }

                    mainQuery.Sql += ("(c.id>='" + CuentaDesde + "' and c.id<='" + CuentaHasta + "' and c.nivel=0) ");
                }

                /*
                if (PorSubcuenta)
                {
                    mainQuery.Sql += ("(c.id>='"+CuentaDesde+ "' and c.id<='"+CuentaHasta+"' and c.nivel=0) ");
                }

                if (PorGrupos)
                {
                    if(PorSubcuenta || PorGrupos || PorSubgrupos || PorMayor || PorSubmayor)
                    {
                        mainQuery.Sql += (" or ");
                    }
                    mainQuery.Sql += ("((c.id='"+cuentaDesdeAux1+ "') or (c.id>='"+CuentaDesde+ "' and c.id<='"+CuentaHasta+"' and c.nivel=1)) ");    
                }

                if (PorSubgrupos)
                {
                    if (PorSubcuenta || PorGrupos || PorSubgrupos || PorMayor || PorSubmayor)
                    {
                        mainQuery.Sql += (" or ");
                    }
                    mainQuery.Sql += ("((c.id='"+cuentaDesdeAux2+ "') or (c.id>='"+CuentaDesde+ "' and c.id<='"+CuentaHasta+"' and c.nivel=2)) ");
                }

                if (PorMayor)
                {
                    if (PorSubcuenta || PorGrupos || PorSubgrupos || PorMayor || PorSubmayor)
                    {
                        mainQuery.Sql += (" or ");
                    }
                    mainQuery.Sql += ("((c.id='"+cuentaDesdeAux3+ "') or (c.id>='"+CuentaDesde+ "' and c.id<='"+CuentaHasta+"' and c.nivel=3)) ");
                }

                if (PorSubmayor)
                {
                    if (PorSubcuenta || PorGrupos || PorSubgrupos || PorMayor || PorSubmayor)
                    {
                        mainQuery.Sql += (" or ");
                    }
                    mainQuery.Sql += ("((c.id='"+cuentaDesdeAux4+ "') or (c.id>='"+CuentaDesde+ "' and c.id<='"+CuentaHasta+"' and c.nivel=4)) ");
                }
                */

                mainQuery.Sql += (")");

                //En caso de que el usuario quiera mostrar cuentas sin saldo, se mostraran las de con saldo y la de sin saldo
                if (!MostrarCuentasSinSaldo)
                {
                    mainQuery.Sql += (" and (m.debe<>m.haber)");
                }

                if(!MostrarCuentasSinMovimientos)
                {

                    mainQuery.Sql += ("AND (m.id is not null)");

                }

                if (PorSubcuenta)
                {
                    mainQuery.Sql += (" UNION select '99999999', 'Suma total nivel 0', " +
                        "SUM(case when c.nivel = 0 then debe end) as SumDebeNivel0, SUM(case when c.nivel = 0 then haber end) as SumHaberNivel0, " +
                        "SUM(case when c.nivel = 0 and m.saldo >= 0 then m.saldo else null end) as SumDeudorNivel0, " +
                        "SUM(case when c.nivel = 0 and  m.saldo < 0 then(m.saldo * -1) else null end) as SumAcreedorNivel0, 0 " +
                        "from cuentas as c " +
                        "left join maes as m on c.id = m.fkcuentas " +
                        "where c.empresa = @empresa and(m.fkejercicio = @fkejercicio or m.fkejercicio is null) " +
                        "AND( " +
                        "((c.id = '" + cuentaDesdeAux1 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 1)) " +
                        "    or((c.id = '" + cuentaDesdeAux2 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 2)) " +
                        "    or((c.id = '" + cuentaDesdeAux3 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 3)) " +
                        "    or((c.id = '" + cuentaDesdeAux4 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 4)) " +
                        "    or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 0) " +
                        ") ");
                }
                else if (PorSubmayor)
                {
                    mainQuery.Sql += (" UNION select '99999999', 'Suma total nivel 4', " +
                        "SUM(case when c.nivel = 4 then debe end) as SumDebeNivel4, SUM(case when c.nivel = 4 then haber end) as SumHaberNivel4, " +
                        "SUM(case when c.nivel = 4 and m.saldo >= 0 then m.saldo else null end) as SumDeudorNivel4, " +
                        "SUM(case when c.nivel = 4 and  m.saldo < 0 then(m.saldo * -1) else null end) as SumAcreedorNivel4, 4 " +
                        "from cuentas as c " +
                        "left join maes as m on c.id = m.fkcuentas " +
                        "where c.empresa = @empresa and(m.fkejercicio = @fkejercicio or m.fkejercicio is null) " +
                        "AND( " +
                        "((c.id = '" + cuentaDesdeAux1 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 1)) " +
                        "    or((c.id = '" + cuentaDesdeAux2 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 2)) " +
                        "    or((c.id = '" + cuentaDesdeAux3 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 3)) " +
                        "    or((c.id = '" + cuentaDesdeAux4 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 4)) " +
                        ") ");
                }
                else if (PorMayor)
                {
                    mainQuery.Sql += (" UNION select '99999999', 'Suma total nivel 3', " +
                        "SUM(case when c.nivel = 3 then debe end) as SumDebeNivel3, SUM(case when c.nivel = 3 then haber end) as SumHaberNivel3, " +
                        "SUM(case when c.nivel = 3 and m.saldo >= 0 then m.saldo else null end) as SumDeudorNivel3, " +
                        "SUM(case when c.nivel = 3 and  m.saldo < 0 then(m.saldo * -1) else null end) as SumAcreedorNivel3, 3 " +
                        "from cuentas as c " +
                        "left join maes as m on c.id = m.fkcuentas " +
                        "where c.empresa = @empresa and(m.fkejercicio = @fkejercicio or m.fkejercicio is null) " +
                        "AND( " +
                        "((c.id = '" + cuentaDesdeAux1 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 1)) " +
                        "    or((c.id = '" + cuentaDesdeAux2 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 2)) " +
                        "    or((c.id = '" + cuentaDesdeAux3 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 3)) " +
                        ") ");
                }
                else if (PorSubgrupos)
                {
                    mainQuery.Sql += (" UNION select '99999999', 'Suma total nivel 2', " +
                        "SUM(case when c.nivel = 2 then debe end) as SumDebeNivel2, SUM(case when c.nivel = 2 then haber end) as SumHaberNivel2, " +
                        "SUM(case when c.nivel = 2 and m.saldo >= 0 then m.saldo else null end) as SumDeudorNivel2, " +
                        "SUM(case when c.nivel = 2 and  m.saldo < 0 then(m.saldo * -1) else null end) as SumAcreedorNivel2, 2 " +
                        "from cuentas as c " +
                        "left join maes as m on c.id = m.fkcuentas " +
                        "where c.empresa = @empresa and(m.fkejercicio = @fkejercicio or m.fkejercicio is null) " +
                        "AND( " +
                        "((c.id = '" + cuentaDesdeAux1 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 1)) " +
                        "    or((c.id = '" + cuentaDesdeAux2 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 2)) " +
                        ") ");
                }
                else if (PorGrupos)
                {
                    mainQuery.Sql += (" UNION select '99999999', 'Suma total nivel 1', " +
                        "SUM(case when c.nivel = 1 then debe end) as SumDebeNivel1, SUM(case when c.nivel = 1 then haber end) as SumHaberNivel1, " +
                        "SUM(case when c.nivel = 1 and m.saldo >= 0 then m.saldo else null end) as SumDeudorNivel1, " +
                        "SUM(case when c.nivel = 1 and  m.saldo < 0 then(m.saldo * -1) else null end) as SumAcreedorNivel1, 1 " +
                        "from cuentas as c " +
                        "left join maes as m on c.id = m.fkcuentas " +
                        "where c.empresa = @empresa and(m.fkejercicio = @fkejercicio or m.fkejercicio is null) " +
                        "AND( " +
                        "((c.id = '" + cuentaDesdeAux1 + "') or(c.id >= '" + CuentaDesde + "' and c.id <= '" + CuentaHasta + "' and c.nivel = 1)) " +
                        ") ");
                }
            }

            DataSource.Queries.Add(new CustomSqlQuery("Empresa", "SELECT id, nombre FROM Empresas WHERE id = '" + user.Empresa + "'"));
            DataSource.Queries.Add(new CustomSqlQuery("Ejercicios", "SELECT empresa, id, descripcion FROM Ejercicios WHERE id = '" + user.Ejercicio + "'"));
            DataSource.Queries.Add(mainQuery);
            DataSource.RebuildResultSchema();
        }
    }
}



