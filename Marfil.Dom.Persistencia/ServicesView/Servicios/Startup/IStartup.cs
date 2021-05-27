using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Startup
{
    public interface IStartupTablasVarias
    {
        string Id { get; set; }
        string Nombre { get; set; }
        string Clase { get; set; }
        TipoTablaVaria Tipo { get; set; }
        bool NoEditable { get; set; }

    }
    public interface IStartup
    {
        void CrearDatos(string fichero);
    }
}
