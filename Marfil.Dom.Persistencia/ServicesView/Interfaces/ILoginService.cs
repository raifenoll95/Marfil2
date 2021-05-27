using System;
using System.Web;
using Marfil.Dom.Persistencia.Model;

namespace Marfil.Dom.Persistencia.ServicesView.Interfaces
{
    public interface ILoginService
    {
        bool Login(string dominio, string usuario, string contraseña,out HttpCookie securityCookie);
        void SetEmpresaUser(string dominio,string basedatos, string user, string empresa, string ejercicio,string almacen,Guid idconexion, out HttpCookie cookie);
        void Logout(ICustomPrincipal customPrincipal);
        void Forzardesconexion(string basedatos,string idusuario);
        bool Existeusuarioactivo(string basedatos, Guid idconexion);
        bool checkPassword(string dominio, string usuario, string password, out string basedatos);
        ICustomPrincipal getUserActivo(string dominio, string usuario, string password, Guid idconexion);
        bool puedeCambiarEmpresa(string baseDatos, string usuario, string id, string dnsSafeHost);
        bool CambiarEmpresa(string baseDatos, string usuario, string id, string dnsSafeHost);
    }
}
