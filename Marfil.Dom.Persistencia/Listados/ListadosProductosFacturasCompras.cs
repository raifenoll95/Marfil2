using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosProductosFacturasCompras : ListadosProductosDocumentos
    {
        public override string TituloListado => "Listado de productos en facturas compras";

        public override string IdListado => FListadosModel.ProductosEnFacturasCompras;

        public ListadosProductosFacturasCompras():base(ListadoTipoDocumento.FacturasCompras, null)
        {

        }

        public ListadosProductosFacturasCompras(IContextService context) : base(ListadoTipoDocumento.FacturasCompras, context)
        {

        }
    }
}
