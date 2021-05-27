using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosProductosPresupuestos : ListadosProductosDocumentos
    {
        public override string TituloListado => "Listado de productos en presupuestos";

        public override string IdListado => FListadosModel.ProductosEnPresupuestos;

        public ListadosProductosPresupuestos() : base(ListadoTipoDocumento.Presupuestos,null)
        {

        }
        public ListadosProductosPresupuestos(IContextService context) : base(ListadoTipoDocumento.Presupuestos,context)
        {

        }
    }
}
