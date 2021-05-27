using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    public class TablaVariaStartup:IStartup
    {
        private readonly FStartup _factory;
        private readonly MarfilEntities _db;
        private readonly IContextService _context;
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Clase { get; set; }
        public TipoTablaVaria Tipo { get; set; }
        public bool NoEditable { get; set; }

        public TablaVariaStartup(IContextService context,FStartup factory,MarfilEntities db)
        {
            _factory = factory;
            _db = db;
            _context = context;
        }

        public void CrearDatos(string fichero)
        {
            var service = _factory.CreateService(Id);
            var varias = service as IStartupTablasVarias;
            if (varias != null)
            {
                varias.Id = Id;
                varias.Nombre = Nombre;
                varias.Clase = Clase;
                varias.NoEditable = NoEditable;
                varias.Tipo = Tipo;
            }
            
            service.CrearDatos(fichero);
        }
    }
}
