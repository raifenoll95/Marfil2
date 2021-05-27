using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.ControlsUI.BusquedaTerceros;
using Marfil.Dom.Persistencia.Model.Documentos;

namespace Marfil.Dom.Persistencia.ServicesView.Interfaces
{
    public interface IBuscarDocumento:IDisposable
    {
        IEnumerable<DocumentosBusqueda> Buscar(IDocumentosFiltros filtros, out int registrostotales);
        IEnumerable<IItemResultadoMovile> BuscarDocumento(string referencia);
    }
}
