using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadoCuentasNoAsignadasFuncional : ListadosModel
    {
        public override string TituloListado => "Listado Cuentas no asignadas funcional";

        public override string IdListado => FListadosModel.CuentasNoAsignadasFuncional;

        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
            sb.Append("select asi.fkcuentas as Cuenta, c.descripcion as [Descripción], asi.debe as Debe, asi.haber as Haber, asi.saldo as Saldo from CuentasNoAsignadasFuncional as asi, Cuentas as c");

            return sb.ToString();
        }

        internal override string GenerarFiltrosColumnas()
        {
            var sb = new StringBuilder();
            sb.Append(" asi.fkcuentas = c.id and asi.empresa = c.empresa and asi.procesado = 0 and c.nivel = 4");

            return sb.ToString();
        }

        public ListadoCuentasNoAsignadasFuncional()
        {

        }

        public ListadoCuentasNoAsignadasFuncional(IContextService context) : base(context)
        {

        }
    }
}
