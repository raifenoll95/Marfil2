using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Ficheros;

namespace Marfil.Dom.Persistencia.Model.GaleriaImagenes
{
    public class GaleriaModel
    {
        public string Empresa { get; set; }
        public Guid DirectorioId { get; set; }
        public IEnumerable<FicherosGaleriaModel> Ficheros { get; set; }
    }
}
