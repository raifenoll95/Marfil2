using System.Collections.Generic;

namespace Marfil.Dom.Persistencia.Model.Configuracion
{
    public interface IContadoresModel
    {
        string Descripcion { get; set; }
        string Empresa { get; set; }
        string Id { get; set; }
        List<ContadoresLinModel> Lineas { get; set; }
        int Primerdocumento { get; set; }
        TipoContador Tipocontador { get; set; }
        TipoInicio Tipoinicio { get; set; }
    }

    public interface IContadoresLotesModel
    {
        string Descripcion { get; set; }
        string Empresa { get; set; }
        string Id { get; set; }
        List<ContadoresLotesLinModel> Lineas { get; set; }
        int Primerdocumento { get; set; }
        TipoLoteContador Tipocontador { get; set; }
        TipoLoteInicio Tipoinicio { get; set; }
    }

}