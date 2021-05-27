//using Marfil.Dom.Persistencia.Model.Configuracion;
//using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.Model.Contabilidad.Movs
{
    public interface IMovLinVistaModel
    {
        int Orden { get; set; }
        bool Esdebe { get; set; }
        string Fkcuentas { get; set; }
        string Fkseccionesanaliticas { get; set; }
        decimal Importe { get; set; }                 // puesto decimal (es mejor)
        double Importemonedadocumento { get; set; }  // double en la clase del documento
        string Comentario { get; set; }
    }
}