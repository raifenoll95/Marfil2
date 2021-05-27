using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Stock
{
    public class StockActualRemedirModel
    {
        public string Fkalmacenes { get; set; }
        public string Fkarticulos { get; set; }
        public string Descripcion { get; set; }
        public string Lote { get; set; }
        public string Loteid { get; set; }
        public double? Largo { get; set; }
        public double? Ancho { get; set; }
        public double? Grueso { get; set; }
        public double? Cantidaddisponible { get; set; }
        public double? Metros { get; set; }
        public string SLargo
        {
            get { return Largo?.ToString("N" + (_decimales ?? 0)) ?? string.Empty; }
        }
        public string SAncho
        {
            get { return Ancho?.ToString("N" + (_decimales ?? 0)) ?? string.Empty; }
        }
        public string SGrueso
        {
            get { return Grueso?.ToString("N" + (_decimales ?? 0)) ?? string.Empty; }
        }
        public string SMetros
        {
            get { return Metros?.ToString("N" + (_decimales ?? 0)) ?? string.Empty; }
        }
        public string UM { get; set; }
        public string Bundle { get; set; }
        public string Cc { get; set; }
        public string Tono { get; set; }
        public string Grano { get; set; }
        public string Zona { get; set; }
        public string Loteproveedor { get; set; }

        public double? Kilosum { get; set; }
        public int? _decimales { get; set; }
    }

    public class StockActualMobileModel
    {
        public string Fkarticulos { get; set; }
        public string Articulo { get; set; }
        public string Descripcion { get; set; }
        public string Lote { get; set; }
        public string Familia { get; set; }
        public string Material { get; set; }
        public string Familiamaterial { get; set; }
        public double? Cantidaddisponible { get; set; }
        public double? Metros { get; set; }
        public string SMetros
        {
            get { return Metros?.ToString("N" + (_decimales ?? 0)) ?? string.Empty; }
        }
        public string UM { get; set; }
        public int? _decimales { get; set; }
    }

    public class StockActualVistaModel
    {
        public string Empresa { get; set; }

        public DateTime Fecha { get; set; }

        public string Fkalmacenes { get; set; }

        public int? Fkalmaceneszona { get; set; }

        public string Fkalmaceneszonadescripcion { get; set; }

        public string Fkarticulos { get; set; }

        public string Fkfamilias { get; set; }

        public string Descripcion { get; set; }

        public int Decimalesmedidas { get; set; }

        public string Referenciaproveedor { get; set; }

        public string Lote { get; set; }

        public string Loteid { get; set; }

        public string Loteidentificador { get; set; }

        public string Tag { get; set; }

        public string Fkunidadesmedida { get; set; }

        public double Cantidad { get; set; }

        public double Largo { get; set; }

        public double Ancho { get; set; }

        public double Grueso { get; set; }

        public double? Metros { get; set; }

        public double? MetrosSalida { get; set; }

        public string Bundle { get; set; }

        public Guid Integridadreferencialflag { get; set; }

    }

    public class StockActualVistaModelHistorico
    {
        public string Empresa { get; set; }

        public DateTime Fecha { get; set; }

        public string Fkalmacenes { get; set; }

        public int? Fkalmaceneszona { get; set; }

        public string Fkalmaceneszonadescripcion { get; set; }

        public string Fkarticulos { get; set; }

        public string Fkfamilias { get; set; }

        public string Descripcion { get; set; }

        public int Decimalesmedidas { get; set; }

        public string Referenciaproveedor { get; set; }

        public string Lote { get; set; }

        public string Loteid { get; set; }

        public string Loteidentificador { get; set; }

        public string Tag { get; set; }

        public string Fkunidadesmedida { get; set; }

        public double? Cantidadentrada { get; set; }

        public double? Largoentrada { get; set; }

        public double? Anchoentrada { get; set; }

        public double? Gruesoentrada { get; set; }

        public double? Metros { get; set; }

        public double? MetrosEntrada { get; set; }

        public string Bundle { get; set; }

        public Guid Integridadreferencialflag { get; set; }


    }


    //:IStockPieza
    public class StockActualModel : BaseModel<StockActualModel, Stockactual>
    {

        #region CTR

        public StockActualModel()
        {

        }

        public StockActualModel(IContextService context) : base(context)
        {

        }

        #endregion


        public string Empresa { get; set; }

        public DateTime Fecha { get; set; }

        public string Fkalmacenes { get; set; }

        public string Fkalmaceneszona { get; set; }

        public string Fkalmaceneszonadescripcion { get; set; }

        public string Fkarticulos { get; set; }

        public string Descripcion { get; set; }

        public int Decimalesmedidas { get; set; }

        public string Referenciaproveedor { get; set; }

        public string Lote { get; set; }

        public string Loteid { get; set; }

        public string Tag { get; set; }

        public string Fkunidadesmedida { get; set; }

        public double Cantidad { get; set; }

        public double Largo { get; set; }

        public double Ancho { get; set; }

        public double Grueso { get; set; }

        public double? Metros { get; set; }

        public string Fkcalificacioncomercial { get; set; }

        public string Fktipograno { get; set; }

        public string Fktonomaterial { get; set; }

        public string Fkincidenciasmaterial { get; set; }

        public string Fkvariedades { get; set; }

        public double? Costeadicionalmaterial { get; set; }

        public double? Costeadicionalportes { get; set; }

        public double? Costeadicionalotro { get; set; }

        public double? Costeadicionalvariable { get; set; }

        public Guid Integridadreferencialflag { get; set; }

        public override string DisplayName
        {
            get
            {
                return "Stock Actual";
            }
        }

        public override object generateId(string id)
        {
            throw new NotImplementedException();
        }
    }

    public class KitStockActualModel: MovimientosstockModel, IKitStockPieza
    {
        
    }

    public class BundleStockActualModel : MovimientosstockModel, IBundleStockPieza
    {

    }
}
