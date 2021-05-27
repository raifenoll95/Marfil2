using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;

namespace Marfil.Dom.Persistencia.Model.Stock
{
    public class StockPiezaSingle : MovimientosstockModel,IStockPiezaSingle
    {
        public TipoPieza Tipopieza { get; set; }
        public bool Lotefraccionable { get; set; }
    }

    public class SalidabusquedaentregaslotesarticulosModel : MovimientosstockModel
    {
        public string Bundle { get; set; }
        public bool Lotefraccionable { get; set; }
    }
    public class MovimientosstockModel:IStockPieza
    {
        public string Empresa { get; set; }

        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Fkalmacenes { get; set; }

        public string Fkzonaalmacedescripcion { get; set; }

        public string Fkarticulos { get; set; }

        public string Descripcion { get; set; }

        public int? Decimalesmedidas { get; set; }

        public string Referenciaproveedor { get; set; }

        public string Fkcontadorlote { get; set; }

        public string Lote { get; set; }

        public string Loteid { get; set; }

        public string Tag { get; set; }

        public string Fkunidadesmedida { get; set; }

        public double Cantidad { get; set; }

        public double Largo { get; set; }

        public double Ancho { get; set; }

        public double Grueso { get; set; }

        public double Metros { get; set; }

        public string SLargo { get { return Largo.ToString("N" + (Decimalesmedidas ?? 2)); } }

        public string SAncho { get { return Ancho.ToString("N" + (Decimalesmedidas ?? 2)); } }

        public string SGrueso { get { return Grueso.ToString("N" + (Decimalesmedidas ?? 2)); } }

        public string SMetros { get { return Metros.ToString("N" + (Decimalesmedidas ?? 2)); } }

        public int? Fkalmaceneszona { get; set; }

        public string Fkcalificacioncomercial { get; set; }

        public string Fktipograno { get; set; }

        public string Fktonomaterial { get; set; }

        public string Fkincidenciasmaterial { get; set; }

        public string Fkvariedades { get; set; }

        public double? Costeadicionalmaterial { get; set; }

        public double? Costeadicionalportes { get; set; }

        public double? Costeadicionalotro { get; set; }

        public double? Costeadicionalvariable { get; set; }

        public TipoOperacionStock Tipooperacion { get; set; }

        public string Documentomovimiento { get; set; }

        public Guid Integridadreferencialflag { get; set; }

        public Guid Fkusuarios { get; set; }

        public double? Pesoneto { get; set; }

        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        public int Ubicaciondestino { get; set; }

        public TipoOperacionService Tipomovimiento { get; set; }
    }

    public class MovimientosstockModelHistorico : IStockPieza
    {
        public string Empresa { get; set; }

        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public string Fkalmacenes { get; set; }

        public string Fkzonaalmacedescripcion { get; set; }

        public string Fkarticulos { get; set; }

        public string Descripcion { get; set; }

        public int? Decimalesmedidas { get; set; }

        public string Referenciaproveedor { get; set; }

        public string Fkcontadorlote { get; set; }

        public string Lote { get; set; }

        public string Loteid { get; set; }

        public string Tag { get; set; }

        public string Fkunidadesmedida { get; set; }

        public double Cantidadentrada { get; set; }

        public double Largoentrada { get; set; }

        public double Anchoentrada { get; set; }

        public double Gruesoentrada { get; set; }

        public double Metros { get; set; }

        public double MetrosEntrada { get; set; }

        public string SLargo { get { return Largoentrada.ToString("N" + (Decimalesmedidas ?? 2)); } }

        public string SAncho { get { return Anchoentrada.ToString("N" + (Decimalesmedidas ?? 2)); } }

        public string SGrueso { get { return Gruesoentrada.ToString("N" + (Decimalesmedidas ?? 2)); } }

        public string SMetros { get { return Metros.ToString("N" + (Decimalesmedidas ?? 2)); } }

        public int? Fkalmaceneszona { get; set; }

        public string Fkcalificacioncomercial { get; set; }

        public string Fktipograno { get; set; }

        public string Fktonomaterial { get; set; }

        public string Fkincidenciasmaterial { get; set; }

        public string Fkvariedades { get; set; }

        public double? Costeadicionalmaterial { get; set; }

        public double? Costeadicionalportes { get; set; }

        public double? Costeadicionalotro { get; set; }

        public double? Costeadicionalvariable { get; set; }

        public TipoOperacionStock Tipooperacion { get; set; }

        public string Documentomovimiento { get; set; }

        public Guid Integridadreferencialflag { get; set; }

        public Guid Fkusuarios { get; set; }

        public double? Pesoneto { get; set; }

        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        public int Ubicaciondestino { get; set; }

        public TipoOperacionService Tipomovimiento { get; set; }
    }
}
