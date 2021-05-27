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
    public class AgregarLineaDocumentosLinModel
    {
        public string Fkarticulos { get; set; }
        public string Descripcion { get; set; }
        public string Lote { get; set; }
        public string Cantidad { get; set; }
        public string Largo { get; set; }
        public string Ancho { get; set; }
        public string Grueso { get; set; }
        public string Metros { get; set; }
    }

    public class AgregarLineaDocumentosModel
    {
        public string Error { get; set; }
        public string Referencia { get; set; }
        public string Fecha { get; set; }
        public List<AgregarLineaDocumentosLinModel> Lineas { get; set; }
        public bool IsEnabled { set; get; }
    }

    
    
    public interface IAgregarLineaDocumentoMovile : IDisposable
    {
        AgregarLineaDocumentosModel AgregarLinea(string referencia, string lote);
        AgregarLineaDocumentosModel EliminarLinea(string referencia, string lote);
    }


}
