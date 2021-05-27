using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Persistencia.Model
{
    public class HeaderModel
    {
        public bool IsInitial { get; set; }

        public string Empresa { get; set; }
        public string EmpresaId { get; set; }
        public IEnumerable<Tuple<string,string>> EmpresasList { get; set; } 

        public string Ejercicio { get; set; }
        public string EjercicioId { get; set; }
        public string Almacen { get; set; }
        public string AlmacenId { get; set; }
        public IEnumerable<Tuple<string, string>> EjerciciosList { get; set; }
        public IEnumerable<Tuple<string, string>> AlmacenesList { get; set; }

        public string Azureblob { get; set; }

        public HeaderModel()
        {
            IsInitial = false;
        }
    }
}
