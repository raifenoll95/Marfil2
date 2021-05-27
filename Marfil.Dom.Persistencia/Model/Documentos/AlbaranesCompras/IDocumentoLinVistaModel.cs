using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras
{
    public interface IDocumentoLinVistaModel
    {
        double Ancho { get; set; }
        int Cantidad { get; set; }
        int Decimalesmedidas { get; set; }
        int Decimalesmonedas { get; set; }
        double Descuento { get; set; }
        string Descuentocomercial { get; set; }
        string Descuentoprontopago { get; set; }
        string Fkalmacen { get; set; }
        string Fkarticulos { get; set; }
        string Fktiposiva { get; set; }
        string Fkunidades { get; set; }
        TipoStockFormulas Formulas { get; set; }
        double Grueso { get; set; }
        double Largo { get; set; }
        string Lote { get; set; }
        bool Loteautomatico { get; set; }
        double Metros { get; set; }
        string Portes { get; set; }
        TipoFamilia Tipofamilia { get; set; }
    }
}