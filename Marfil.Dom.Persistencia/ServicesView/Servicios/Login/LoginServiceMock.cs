using System;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Login
{
    public class LoginServiceMock : ILoginService,IDisposable
    {
        public bool Login(string dominio,string usuario, string contraseña,  out HttpCookie securityCookie)
        {
            securityCookie = null;
            try
            {
                
                var modelSecurity = new SecurityTicket
                {
                    Usuario = ApplicationHelper.UsuariosAdministrador,
                    Id = Guid.Empty,
                    Idconexion = Guid.Empty,
                    RoleId = Guid.Empty,
                    BaseDatos = "marfil",
                    Empresa = "0001",
                    Ejercicio = "2"
                    //serializeModel.Roles = db.USUARIOROLE.Where(i => i.IDUSUARIO == u.ID).Select(i => i.ROLE.NOMBRE).ToList();
                };
                var serializer = new JavaScriptSerializer();
                var userData = serializer.Serialize(modelSecurity);
                var authTicket = new FormsAuthenticationTicket(
                            1,
                        usuario,
                            DateTime.Now,
                            DateTime.Now.AddMinutes(120),//meter en settings
                            false,
                            userData);

                var encTicket = FormsAuthentication.Encrypt(authTicket);
                securityCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                return true;
                    
                
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return true;
        }

        public void SetEmpresaUser(string dominio,string basedatos, string user, string empresa, string ejercicio, string almacen,Guid idconexion,out HttpCookie cookie)
        {
            cookie = null;
            try
            {
                var modelSecurity = new SecurityTicket();
                var serializer = new JavaScriptSerializer();
                var userData = serializer.Serialize(modelSecurity);
                var authTicket = new FormsAuthenticationTicket(
                            1,
                            user,
                            DateTime.Now,
                            DateTime.Now.AddMinutes(120),//meter en settings
                            false,
                            userData);

                var encTicket = FormsAuthentication.Encrypt(authTicket);
                cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encTicket);
                


            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            
        }

        public void Logout(ICustomPrincipal customPrincipal)
        {
            throw new NotImplementedException();
        }

        public void Forzardesconexion(string basedatos,string idusuario)
        {
            throw new NotImplementedException();
        }

        public bool Existeusuarioactivo(string basedatos, Guid idconexion)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }

        public bool checkPassword(string dominio, string usuario, string password, out string basedatos)
        {
            throw new NotImplementedException();
        }

        public ICustomPrincipal getUserActivo(string dominio,  string usuario, string password, Guid idconexion)
        {
            throw new NotImplementedException();
        }

        public bool puedeCambiarEmpresa(string baseDatos, string usuario, string id)
        {
            throw new NotImplementedException();
        }

        public bool puedeCambiarEmpresa(string baseDatos, string usuario, string id, string dnsSafeHost)
        {
            throw new NotImplementedException();
        }

        public bool CambiarEmpresa(string baseDatos, string usuario, string id, string dnsSafeHost)
        {
            throw new NotImplementedException();
        }
    }
}
