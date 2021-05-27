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

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    internal class MonedasStartup:IStartup
    {
        #region Members

        private readonly MonedasService _tablasVariasService;
        private IContextService _context;

        #endregion

        #region CTR

        public MonedasStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _tablasVariasService = new MonedasService(context, db);
        }

        #endregion

        public void CrearDatos(string fichero)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CrearMonedas(contenido);
            }
        }

        private void CrearMonedas(string xml)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearMoneda(item);
            }
                
        }

        private void CrearMoneda(string linea)
        {
            var vector = linea.Split(';');
            var model= new MonedasModel()
            {
                UsuarioId = Guid.Empty,
                Usuario = ApplicationHelper.UsuariosAdministrador,
                Id= int.Parse(vector[1]),
                Abreviatura = vector[0],
                Descripcion = vector[2],
                CambioMonedaAdicional = 0,
                CambioMonedaBase = 0,
                Decimales=2,
                Activado = bool.Parse(vector[3])
            };

            _tablasVariasService.create(model);
        }

        public void Dispose()
        {
            _tablasVariasService?.Dispose();
        }
    }
}
