using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosProductosPedidos : ListadosProductosDocumentos
    {
        public override string TituloListado => "Listado de productos en pedidos";

        public override string IdListado => FListadosModel.ProductosEnPedidos;


        public ListadosProductosPedidos():base(ListadoTipoDocumento.Pedidos, null)
        {

        }

        public ListadosProductosPedidos(IContextService context) : base(ListadoTipoDocumento.Pedidos,context)
        {

        }
    }
}
