using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosProductosPedidosCompras : ListadosProductosDocumentos
    {
        public override string TituloListado => "Listado de productos en pedidos Compras";

        public override string IdListado => FListadosModel.ProductosEnPedidosCompras;

        public ListadosProductosPedidosCompras():base(ListadoTipoDocumento.PedidosCompras, null)
        {

        }

        public ListadosProductosPedidosCompras(IContextService context) : base(ListadoTipoDocumento.PedidosCompras,context)
        {

        }
    }
}
