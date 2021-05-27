using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RProveedores= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
namespace Marfil.Dom.Persistencia.Listados
{
    public class ListadosProductosAlbaranesCompras : ListadosProductosDocumentos
    {
        public override string TituloListado => "Listado de productos en albaranes";

        public override string IdListado => FListadosModel.ProductosEnAlbaranesCompras;

        public ListadosProductosAlbaranesCompras():base(ListadoTipoDocumento.AlbaranesCompras, null)
        {

        }

        public ListadosProductosAlbaranesCompras(IContextService context) : base(ListadoTipoDocumento.AlbaranesCompras,context)
        {

        }
    }
}
