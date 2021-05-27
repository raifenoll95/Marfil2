using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Model.Configuracion;

using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    class TarifasbaseStartup : IStartup
    {
        #region Members

        private readonly TarifasbaseService _tarifasbaseService;
        private IContextService _context;

        #endregion

        #region CTR

        public TarifasbaseStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _tarifasbaseService = new TarifasbaseService(context, db);
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
            var model = new TarifasbaseModel()
            {
                Fktarifa = vector[1],
                Tipoflujo = (TipoFlujo)Funciones.Qint(vector[0]).Value,
                Descripcion = vector[2]
            };

            _tarifasbaseService.create(model);
        }

        public void Dispose()
        {
            _tarifasbaseService?.Dispose();
        }
    }
}
