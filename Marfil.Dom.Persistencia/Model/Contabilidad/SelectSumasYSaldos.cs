using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Contabilidad
{
    class SelectSumasYSaldos {

        public String getSelect()
        {

            return "select m.fkcuentas as Cuenta, c.descripcion as Descripcion, m.debe as Debe, m.haber as Haber ," +
                            "(case when m.saldo >= 0 THEN m.saldo else null END) AS Deudor ," +
                            "(case when m.saldo < 0 THEN(m.saldo * -1) else null END) AS Acreedor from maes as m, cuentas as c where (m.fkcuentas = c.id ";


        }
    }
}
