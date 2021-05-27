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
    internal class SituacionesTesoreriaStartup:IStartup
    {
        #region Members

        private readonly SituacionesTesoreriaService _tablasVariasService;
        private IContextService _context;

        #endregion

        #region CTR

        public SituacionesTesoreriaStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _tablasVariasService = new SituacionesTesoreriaService(context, db);
        }

        #endregion

        public void CrearDatos(string fichero)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CrearSituaciones(contenido);
            }
        }

        private void CrearSituaciones(string xml)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearSituacion(item);
            }
                
        }

        private void CrearSituacion(string linea)
        {
            var vector = linea.Split(';');
            var model = new SituacionesTesoreriaModel();
            model.Cod = vector[0];
            model.Descripcion = vector[1];
            model.Descripcion2 = vector[2];
            
            if(vector[3] == "true")
            {
                model.Valorinicialcobros = true;
            }

            else
            {
                model.Valorinicialcobros = false;
            }

            if (vector[4] == "true")
            {
                model.Valorinicialpagos = true;
            }

            else
            {
                model.Valorinicialpagos = false;
            }

            model.Prevision = (TipoPrevision)Int32.Parse(vector[5]);

            if (vector[6] == "true")
            {
                model.Editable = true;
            }

            else
            {
                model.Editable = false;
            }

            if (vector[7] == "true")
            {
                model.Remesable = true;
            }

            else
            {
                model.Remesable = false;
            }

            model.Riesgo = (TipoRiesgo)Int32.Parse(vector[8]);

            _tablasVariasService.create(model);
        }

        public void Dispose()
        {
            _tablasVariasService?.Dispose();
        }
    }
}
