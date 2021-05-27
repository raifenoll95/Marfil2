using System;
using System.Collections.Generic;

namespace Marfil.Dom.Persistencia.Model.Contabilidad.Movs
{
    public interface IMovsLinModel
    {
        int? Decimalesmonedas { get; set; }
        int Id { get; set; }
        int? Orden { get; set; }
        Guid Flagidentifier { get; set; }
        string Fkcuentas { get; set; }
        string Fkseccionesanaliticas { get; set; }
        decimal? Importemonedadocumento { get; set; }  
        string Comentario { get; set; }
        bool? Conciliado { get; set; }

        // Debe - Haber
        decimal Importe { get; set; }
        short Esdebe { get; set; }
        decimal? Debe { get; set; }
        decimal? Haber { get; set; }
        string SDebe { get; }
        string SHaber { get; }
        string SImporte { get; }

    }
}