using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosProductosAlbaranes : ListadosProductosDocumentos
    {
        public override string TituloListado => "Listado de productos en albaranes";

        public override string IdListado => FListadosModel.ProductosEnAlbaranes;

        public ListadosProductosAlbaranes():base(ListadoTipoDocumento.Albaranes,null)
        {
            
        }

        public ListadosProductosAlbaranes(IContextService context) : base(ListadoTipoDocumento.Albaranes, context)
        {

        }
    }
}
