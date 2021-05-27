using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.AgrupacionFacturacion
{
    internal interface IAgrupacionService
    {
        IEnumerable<ILineaImportar> GetLineasImportarAlbaran(AlbaranesService service, MarfilEntities db, string referencia);
    }
}
