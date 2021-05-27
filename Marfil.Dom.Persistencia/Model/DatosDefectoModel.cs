using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Marfil.Dom.Persistencia.Model
{
    public class DatosDefectoItemModel
    {
        public string Id { get; set; }
        public bool Selected { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Fichero { get; set; }
        public bool Readonly { get; set; }
    }
 }
