using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Inf.Genericos
{
    public static class Tools
    {
        public static string RellenaCeros(string cadena, int longitud)
        {
            return cadena.PadLeft(longitud, '0');
        }

    }
}
