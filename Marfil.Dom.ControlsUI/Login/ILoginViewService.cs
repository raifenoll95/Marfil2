using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Dom.ControlsUI.Login
{
    public interface IContextLogin
    {
        string Codigo { get; set; }
    }

    public interface ILoginViewService
    {
        LoginViewModel GetConfiguracionModel(IContextLogin context);
        void SaveCookie(LoginViewModel model);
    }
}
