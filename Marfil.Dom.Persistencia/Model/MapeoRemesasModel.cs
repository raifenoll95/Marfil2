using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model
{
    public class MapeoRemesasModel : BaseModel<MapeoRemesasModel, MapeoRemesas>
    {
        public int Id { get; set; }
        public string Etiqueta { get; set; }
        public string Tabla { get; set; }
        public string Campo { get; set; }
        public string Enlaces { get; set; }
        public string Enlaces2 { get; set; }

        public override string DisplayName => "Mapeo Remesas";

        public override object generateId(string id)
        {
            return id;
        }
    }
}
