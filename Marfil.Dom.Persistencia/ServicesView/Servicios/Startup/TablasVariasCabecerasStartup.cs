using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    class TablasVariasCabecerasStartup:IStartup,IStartupTablasVarias
    {
        #region Members

        private readonly TablasVariasService _tablasVariasService;
        private IContextService _context;

        #endregion

        #region members

        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Clase { get; set; }
        public TipoTablaVaria Tipo { get; set; }
        public bool NoEditable { get; set; }

        #endregion

        #region CTR

        public TablasVariasCabecerasStartup(IContextService context,MarfilEntities db)
        {
            _context = context;
            _tablasVariasService = new TablasVariasService(context,db);
        }

        #endregion

        public void CrearDatos(string fichero)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                var lineas = contenido.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (var linea in lineas)
                {
                    if(!string.IsNullOrEmpty(linea))
                    _tablasVariasService.create(CreateModel(linea));
                }
                
            }
        }

        private BaseTablasVariasModel CreateModel(string contenido)
        {
            var items = contenido.Split(';');
            return new BaseTablasVariasModel
            {
                Id = int.Parse(items[0]),
                Nombre = items[1],
                Clase = items[2],
                Tipo= (TipoTablaVaria)Funciones.Qint(items[3]).Value,
                Noeditable = items[4] == "true",
                Lineas=new List<dynamic>()
            };
        }


    }
}
