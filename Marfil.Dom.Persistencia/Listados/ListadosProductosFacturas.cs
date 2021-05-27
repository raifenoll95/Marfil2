using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosProductosFacturas : ListadosProductosDocumentos
    {
        public override string TituloListado => "Listado de productos en facturas";

        public override string IdListado => FListadosModel.ProductosEnFacturas;

        public ListadosProductosFacturas():base(ListadoTipoDocumento.Facturas, null)
        {

        }

        public ListadosProductosFacturas(IContextService context) : base(ListadoTipoDocumento.Facturas, context)
        {

        }
    }
}
