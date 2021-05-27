using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias
{
    public interface ITablasVariasModel
    {
        int Id { get; set; }
        string Clase { get; set; }
        string Nombre { get; set; }
    }
}
