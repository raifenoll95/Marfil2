using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Configuracion;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    internal class UnidadesStartup:IStartup
    {
        #region Members

        private readonly UnidadesService _tablasVariasService;
        private IContextService _context;

        #endregion

        #region CTR

        public UnidadesStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _tablasVariasService = new UnidadesService(context, db);
        }

        #endregion

        public void CrearDatos(string fichero)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CrearUnidades(contenido);
            }
        }

        private void CrearUnidades(string xml)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearUnidad(item);
            }
                
        }

        private void CrearUnidad(string linea)
        {
            var vector = linea.Split(';');
            var model= new UnidadesModel()
            {
                Id = vector[0],
                Codigounidad = vector[1],
                Descripcion = vector[2],
                Descripcion2 = vector[3],
                Textocorto = vector[4],
                Textocorto2 = vector[5],
                Decimalestotales = Int32.Parse(vector[6]),
                Formula = (TipoStockFormulas)Int32.Parse(vector[7]),
                Tiposmovimientostock = (TiposStockMovimientos)Int32.Parse(vector[8]),
                Tipostock = (TiposStock)Int32.Parse(vector[9]),
                Tipototal = (TipoStockTotal)Int32.Parse(vector[10])
            };

            _tablasVariasService.create(model);
        }

        public void Dispose()
        {
            _tablasVariasService?.Dispose();
        }
    }
}
