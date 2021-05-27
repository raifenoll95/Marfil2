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
    class EstadosdocumentosStartup : IStartup
    {
        #region Members

        private readonly EstadosService _EstadosdocumentosService;
        private IContextService _context;

        #endregion

        #region CTR

        public EstadosdocumentosStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _EstadosdocumentosService = new EstadosService(context,db);
        }

        #endregion

        public void CrearDatos(string fichero)
        {
            if (!_EstadosdocumentosService.getAll().Any())
            {
                var csvFile = _context.ServerMapPath(fichero);
                using (var reader = new StreamReader(csvFile, Encoding.Default, true))
                {
                    var contenido = reader.ReadToEnd();
                    CrearModels(contenido);
                }
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
            var documento =(DocumentoEstado) Enum.Parse(typeof(DocumentoEstado),vector[0]);
            var tipoestado = (TipoEstado) Enum.Parse(typeof (TipoEstado), vector[4]);
            var model = new EstadosModel()
            {
                Documento = documento,
                Id = vector[1],
                Descripcion = vector[2],
                Imputariesgo = Funciones.Qbool(vector[3]),
                Tipoestado = tipoestado
            };

            _EstadosdocumentosService.create(model);
        }

        public void Dispose()
        {
            _EstadosdocumentosService?.Dispose();
        }
    }
}
