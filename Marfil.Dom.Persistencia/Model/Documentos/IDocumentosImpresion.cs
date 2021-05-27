using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Diseñador;

namespace Marfil.Dom.Persistencia.Model.Documentos
{
    public interface IDocumentosImpresion
    {
        DocumentosBotonImprimirModel GetListFormatos();
    }
}
