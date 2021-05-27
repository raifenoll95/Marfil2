using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados
{
    public abstract class BasicDerivada
    {
        [HiddenInput]
        public string Clase => GetType().FullName;
    }
}
