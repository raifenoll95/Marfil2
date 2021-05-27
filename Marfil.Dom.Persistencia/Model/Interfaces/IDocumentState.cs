using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Documentos;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Model.Interfaces
{
    public interface IDocumentState
    {
        TipoEstado Tipoestado(IContextService context);
        string Fkestados { get;set; }
    }

    public interface IDocument: IModelView, IModelViewExtension, IDocumentosImpresion, IDocumentState
    {
        int Fkejercicio { set; get; }
    }

    public interface IGaleria
    {
        GaleriaModel Galeria{ get; }
    }
}
