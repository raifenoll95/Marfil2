using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marfil.App.WebMain.Misc
{
    public interface IMenuAuthorization
    {
        string MenuName { get;  }
        bool IsActivado { get; set; }
        bool CanCrear { get;  }
        bool CanModificar { get; }
        bool CanEliminar { get;  }
    }
}