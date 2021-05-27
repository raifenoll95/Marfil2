using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadoGuiasBalances : ListadosModel
    {
        public override string TituloListado => "Listado de Guias de Balances";

        public override string IdListado => FListadosModel.ListadoGuiasBalances;
        public ListadoGuiasBalances()
        {

        }
        public ListadoGuiasBalances(IContextService context) : base(context)
        {

        }
        internal override string GenerarSelect()
        {
            var sb = new StringBuilder();
            sb.Append("Select * from GuiasBalances as gb join GuiasBalancesLinas as gbl on gb.Id = dbl.GuiasBalancesLineas");
            return sb.ToString();
        }
    }
}
