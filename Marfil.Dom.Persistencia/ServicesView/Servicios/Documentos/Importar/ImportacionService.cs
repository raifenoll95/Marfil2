using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar
{
    public interface ILineaImportar
    {
        int Id { get; set; }
        string Canal { get; set; }
        double Cuotaiva { get; set; }
        double Cuotarecargoequivalencia { get; set; }
        int Decimalesmedidas { get; set; }
        int Decimalesmonedas { get; set; }
        string Fkregimeniva { get; set; }
        string Fkunidades { get; set; }
        string Descripcion { get; set; }
        string Fkarticulos { get; set; }
        bool Articulocomentario { get; set; }
        double Cantidad { get; set; }
        double? Largo { get; set; }    
        double? Ancho { get; set; }
        double? Grueso { get; set; }
        double Metros { get; set; }
        double Precio { get; set; }
        string Fktiposiva { get; set; }
        double Importe { get; set; }
        double Importedescuento { get; set; }
        string Lote { get; set; }
        string Notas { get; set; }
        double Porcentajedescuento { get; set; }
        double Porcentajeiva { get; set; }
        double Porcentajerecargoequivalencia { get; set; }
        double Precioanterior { get; set; }
        string Revision { get; set; }
        int? Tabla { get; set; }
        string Fkdocumento { get; set; }
        string Fkdocumentoid { get; set; }
        string Bundle { get; set; }
        string Fkdocumentoreferencia { get; set; }
        int? Orden { get; set; }
        string Contenedor { get; set; }
        string Sello { get; set; }
        int? Caja { get; set; }
        double? Pesoneto { get; set; }
    }

    public class LineaImportarModel:ILineaImportar
    {
        public int Id { get; set; }
        public string Canal { get; set; }
        public double Cuotaiva { get; set; }
        public double Cuotarecargoequivalencia { get; set; }
        public int Decimalesmedidas { get; set; }
        public int Decimalesmonedas { get; set; }
        public string Fkregimeniva { get; set; }
        public string Fkunidades { get; set; }
        public string Descripcion { get; set; }
        public string Fkarticulos { get; set; }
        public bool Articulocomentario { get; set; }
        public double Cantidad { get; set; }
        public double? Largo { get; set; }
        public double? Ancho { get; set; }
        public double? Grueso { get; set; }
        public double Metros { get; set; }
        public double Precio { get; set; }
        public string Fktiposiva { get; set; }
        public double Importe { get; set; }
        public double Importedescuento { get; set; }
        public string Lote { get; set; }
        public string Notas { get; set; }
        public double Porcentajedescuento { get; set; }
        public double Porcentajeiva { get; set; }
        public double Porcentajerecargoequivalencia { get; set; }
        public double Precioanterior { get; set; }
        public string Revision { get; set; }
        public int? Tabla { get; set; }
        public string Fkdocumento { get; set; }
        public string Fkdocumentoid { get; set; }
        public string Fkdocumentoreferencia { get; set; }
        public string Bundle { get; set; }
        public int? Orden { get; set; }
        public string Contenedor { get; set; }
        
        public string Sello { get; set; }

        public int? Caja { get; set; }

        public double? Pesoneto { get; set; }
        public int Fkalbaranes { get; set; }
    }

    public interface IImportacionService
    {
        ILineaImportar ImportarLinea(ILineaImportar linea);
    }

    internal class ImportacionService: IImportacionService
    {
        
        private readonly ArticulosService _serviceArticulos;
        private readonly UnidadesService _serviceUnidades;
        private readonly IContextService _context;

        public ImportacionService(IContextService context)
        {
           
            _context = context;
            _serviceArticulos = FService.Instance.GetService(typeof(ArticulosModel), _context) as ArticulosService;
            _serviceUnidades = FService.Instance.GetService(typeof(UnidadesModel), _context) as UnidadesService;
        }

        public ILineaImportar ImportarLinea(ILineaImportar model)
        {
            
            var articuloObj = _serviceArticulos.get(model.Fkarticulos) as ArticulosModel;
            var unidadesObj = _serviceUnidades.get(articuloObj.Fkunidades) as UnidadesModel;
            model.Metros = UnidadesService.CalculaResultado(unidadesObj, model.Cantidad , model.Largo ?? 0,
                model.Ancho ?? 0, model.Grueso ?? 0,model.Metros);

            model.Importe = model.Metros * model.Precio;

            return model;
        }
    }
}
