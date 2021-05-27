using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    public class ProvinciasStartup:IStartup
    {
        #region Members

        private readonly GestionService<ProvinciasModel, Provincias> _tablasVariasService;
        private IContextService _context;

        #endregion

        #region CTR

        public ProvinciasStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _tablasVariasService = FService.Instance.GetService(typeof(ProvinciasModel), context, db) as ProvinciasService;
        }

        #endregion

        public void CrearDatos(string fichero)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CrearModel(contenido);
            }
        }

        private void CrearModel(string xml)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearLinea(item);
            }

        }

        private void CrearLinea(string linea)
        {
            var vector = linea.Split(';');
            var model = new ProvinciasModel()
            {
               Codigopais = vector[0].PadLeft(3,'0'),
               Id = vector[1],
               Nombre = vector[2]
            };

            _tablasVariasService.create(model);
        }

        public void Dispose()
        {
            _tablasVariasService?.Dispose();
        }
    }
}
