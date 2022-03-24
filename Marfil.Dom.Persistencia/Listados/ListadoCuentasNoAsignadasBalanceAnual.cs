using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadoCuentasNoAsignadasBalanceAnual : ListadosModel
    {
        public override string TituloListado => "Listado Cuentas no asignadas balance anual";

        public override string IdListado => FListadosModel.CuentasNoAsignadasBalanceAnual;

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
            sb.Append("select asi.fkcuentas as Cuenta, c.descripcion as [Descripción], asi.debe as Debe, asi.haber as Haber, asi.saldo as Saldo from CuentasNoAsignadasBalanceAnual as asi, Cuentas as c");

            return sb.ToString();
        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            sb.Append(" asi.fkcuentas = c.id and asi.empresa = c.empresa and asi.procesado = 0 and c.nivel = 4");

            return sb.ToString();
        }

        public ListadoCuentasNoAsignadasBalanceAnual()
        {

        }

        public ListadoCuentasNoAsignadasBalanceAnual(IContextService context) : base(context)
        {

        }
    }
}
