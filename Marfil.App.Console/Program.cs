using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia;

namespace Marfil.App.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new MarfilEntities())
            {
                System.Console.WriteLine(1);
            }
        }
    }
}
