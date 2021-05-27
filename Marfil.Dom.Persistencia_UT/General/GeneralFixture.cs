using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia_UT.General
{
    public class GeneralFixture:BaseFixture
    {
        public string Empresa => Context.Empresa;

        public GeneralFixture()
        {
            CrearDatosDefecto();
        }
    }
}
