using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.ControlsUI.Login.Ficheros;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.ControlsUI.Login
{
    public class LoginViewServiceFichero :ILoginViewService
    {
        private struct StCliente
        {
            public string Bd { get; set; }
            public bool AA { get; set; }
        }

        #region API

        public LoginViewModel GetConfiguracionModel(IContextLogin context)
        {
            var contextficheros = context as ContextLoginFicherosModel;

            var estructura = LoadEstructura(contextficheros);
            if (estructura.ContainsKey(contextficheros.Codigo))
            {
                var item = estructura[contextficheros.Codigo];
                return new LoginViewModel()
                {
                    Basedatos = item.Bd,
                    Identificador = contextficheros.Codigo,
                    SoportedobleA = item.AA
                };
            }

            return null;

        }

        public void SaveCookie(LoginViewModel model)
        {
            
            var cookie = HttpContext.Current.Request.Cookies[Constantes.COOKIEAA];
            if (cookie!=null)
            {
                HttpContext.Current.Request.Cookies.Remove(Constantes.COOKIEAA);
            }

            var newcookie = new HttpCookie(Constantes.COOKIEAA)
            {
                Expires = DateTime.Now.AddDays(30),
                Value = model?.SoportedobleA.ToString()??false.ToString()
            };

            HttpContext.Current.Response.Cookies.Add(newcookie);
        }

        #endregion

        #region Helper

        private Dictionary<string, StCliente> LoadEstructura(ContextLoginFicherosModel context)
        {
            var contenido = File.ReadAllText(context.Ruta);
            var vector = contenido.Split(new [] { Environment.NewLine}, StringSplitOptions.None);
            var resultado = new Dictionary<string,StCliente>();
            foreach (var linea in vector)
            {
                if (!string.IsNullOrEmpty(linea))
                {
                    var lineavector = linea.Split(';');
                    var codigo = lineavector[0];
                    var bd = lineavector[1];
                    var aa = Funciones.Qbool(lineavector[2]);

                    resultado.Add(codigo, new StCliente() { AA = aa, Bd = bd });
                }
                
            }

            return resultado;

        }

        #endregion
    }
}
