using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosProductosPresupuestosCompras : ListadosProductosDocumentos
    {
        public override string TituloListado => "Listado de productos en Presupuestos Compras";

        public override string IdListado => FListadosModel.ProductosEnPresupuestosCompras;

        public ListadosProductosPresupuestosCompras():base(ListadoTipoDocumento.PresupuestosCompras,null)
        {
            
        }

        public ListadosProductosPresupuestosCompras(IContextService context) : base(ListadoTipoDocumento.PresupuestosCompras,context)
        {

        }
    }
}
