using System;
using System.Collections.Generic;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;

namespace Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras
{
    public interface IDocumentosLinModel
    {
        double? Ancho { get; set; }
        double? Cantidad { get; set; }
        int? Decimalesmedidas { get; set; }
        int? Decimalesmonedas { get; set; }
        string Descripcion { get; set; }
        string Fkarticulos { get; set; }
        string Fkcontadoreslotes { get; set; }
        string Fkunidades { get; set; }
        Guid Flagidentifier { get; set; }
        double? Grueso { get; set; }
        int Id { get; set; }
        double? Largo { get; set; }
        string Lote { get; set; }
        string Loteautomaticoid { get; set; }
        int Lotenuevocontador { get; set; }
        double? Metros { get; set; }
        string Notas { get; set; }
        bool Nueva { get; set; }
        int? Orden { get; set; }
        string Revision { get; set; }
        string SAncho { get; set; }
        string SGrueso { get; set; }
        string SLargo { get; set; }
        string SMetros { get; set; }
        int? Tabla { get; set; }
    }
}