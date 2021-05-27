using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Controles
{
    public enum OperacionMantenimiento
    {
        Editar,
        Eliminar,
        Crear,
        Ver
    }
    public class CabeceraMantenimientoModel
    {
        public OperacionMantenimiento Operacion { get; set; }
        public string Mantenimiento { get; set; }
        
        public bool MasOpciones { get; set; }
    }
}
