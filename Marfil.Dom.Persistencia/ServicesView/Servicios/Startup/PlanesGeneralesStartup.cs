using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    class PlanesGeneralesStartup : IStartup
    {
        #region Members

        private readonly GestionService<PlanesGeneralesModel, Planesgenerales> _planesgeneralesService;
        private IContextService _context;

        #endregion

        #region CTR

        public PlanesGeneralesStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _planesgeneralesService =
                FService.Instance.GetService(typeof (PlanesGeneralesModel), context, db) as PlanesGeneralesService;
        }

        #endregion

        public void CrearDatos(string fichero)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CrearModels(contenido);
            }
        }

        private void CrearModels(string xml)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearModel(item);
            }

        }

        private void CrearModel(string linea)
        {
            var vector = linea.Split(';');
            var model = new PlanesGeneralesModel()
            {
                Nombre = vector[0],
                Fichero = vector[1],
                Defecto = Funciones.Qbool(vector[2])
            };

            _planesgeneralesService.create(model);
        }

        public void Dispose()
        {
            _planesgeneralesService?.Dispose();
        }
    }
}
