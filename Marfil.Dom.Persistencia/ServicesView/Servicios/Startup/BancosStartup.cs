using System;
using System.IO;
using System.Text;
using System.Web;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    internal class BancosStartup:IStartup
    {
        #region Members

        private readonly GestionService<BancosModel, Bancos> _tablasVariasService;
        private readonly IContextService _context;
        #endregion

        #region CTR

        public BancosStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _tablasVariasService = FService.Instance.GetService(typeof(BancosModel),context,db) as BancosService ;
        }

        #endregion

        public void CrearDatos(string fichero)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CrearBancos(contenido);
            }
        }

        private void CrearBancos(string xml)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearBanco(item);
            }

        }

        private void CrearBanco(string linea)
        {
            var vector = linea.Split(';');
            var model = new BancosModel()
            {
               Id = vector[0],
               Nombre = vector[1],
               Bic = vector[2]
            };

            _tablasVariasService.create(model);
        }

        public void Dispose()
        {
            _tablasVariasService?.Dispose();
        }
    }
}
