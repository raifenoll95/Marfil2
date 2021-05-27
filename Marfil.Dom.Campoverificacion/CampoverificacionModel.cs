using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.Campoverificacion
{
    public interface ICampoverificacionModel
    {
        Guid id { get; }
        string api { get; }
        string displayName { get; }
        string valueName { get; }
    }
}
