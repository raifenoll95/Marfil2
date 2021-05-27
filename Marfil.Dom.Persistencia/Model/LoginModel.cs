using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.Dom.Persistencia.Model
{
    public interface ICustomPrincipalData
    {
        Guid Id { get; set; }
        Guid RoleId { get; set; }
        Guid Idconexion { get; set; }
        string Usuario { get; set; }
        string BaseDatos { get; set; }
        string Azureblob { get; set; }
        string Empresa { get; set; }
        string Ejercicio { get; set; }
        string Fkalmacen { get; set; }
        bool IsSuperAdmin { get; }
        TipoLicencia Tipolicencia { get; set; }

    }

    public interface ICustomPrincipal : IPrincipal, ICustomPrincipalData
    {
        bool IsSuperAdmin();
    }

    public class CustomPrincipal : ICustomPrincipal
    {
        public bool IsSuperAdmin()
        {
            return ApplicationHelper.UsuariosAdministrador.Equals(Usuario);
        }

        public bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }

        public CustomPrincipal(string email)
        {
            Identity = new GenericIdentity(email);
            Roles = new List<string>();
        }
        public IIdentity Identity { get; private set; }
        public Guid Id { get; set; }
        public Guid Idconexion { get; set; }
        public Guid RoleId { get; set; }
        public string Usuario { get; set; }
        public string BaseDatos { get; set; }
        public string Azureblob { get; set; }
        public string Empresa { get; set; }
        public string Ejercicio { get; set; }
        public string Fkalmacen { get; set; }

        public TipoLicencia Tipolicencia { get; set; }
        bool ICustomPrincipalData.IsSuperAdmin
        {
            get
            {
                return ApplicationHelper.UsuariosAdministrador.Equals(Usuario);
            }
        }



        public List<string> Roles { get; set; }

    }

    public class SecurityTicket
    {
        public Guid Id { get; set; }
        public Guid Idconexion { get; set; }
        public Guid RoleId { get; set; }
        public string BaseDatos { get; set; }
        public string Azureblob { get; set; }
        public string Usuario { get; set; }
        public string Empresa { get; set; }
        public string Ejercicio { get; set; }
        public string Fkalmacen { get; set; }
        public TipoLicencia Tipolicencia { get; set; }
        public List<string> Roles { get; set; }
        public SecurityTicket()
        {
            Roles = new List<string>();
        }

    }

    public class ContextLogin : IContextService
    {
        public Guid Id { get; set; }
        public Guid RoleId { get; set; }
        public Guid Idconexion { get; set; }
        public string Usuario { get; set; }
        public string BaseDatos { get; set; }
        public string Azureblob { get; set; }
        public string Empresa { get; set; }
        public string Ejercicio { get; set; }
        public string Fkalmacen { get; set; }
        public TipoLicencia Tipolicencia { get; set; }

        public ContextLogin()
        {

        }

        public ContextLogin(ICustomPrincipal context)
        {

        }

        bool ICustomPrincipalData.IsSuperAdmin
        {
            get
            {
                return ApplicationHelper.UsuariosAdministrador.Equals(Usuario);

            }
        }


        public string ServerMapPath(string ruta)
        {
            return HttpContext.Current.Server.MapPath(ruta);
        }

        public bool IsAuthenticated()
        {
            throw new NotImplementedException();
        }

        public UrlHelper GetUrlHelper()
        {
            throw new NotImplementedException();
        }

        public bool IsObjectDictionaryEnabled()
        {
            return false;
        }

        public void SetObject(string key, object elem)
        {
            throw new NotImplementedException();
        }

        public object GetObject(string key)
        {
             throw new NotImplementedException();
            
        }

        public void SetItem(string key, object elem)
        {
            throw new NotImplementedException();
        }

        public object GetItem(string key)
        {
            throw new NotImplementedException();
        }
    }

    public class ContextConfiguracion : IContextService
    {

        private readonly ICustomPrincipal _principal;

        public ContextConfiguracion()
        {
            _principal = HttpContext.Current.User as ICustomPrincipal;
        }

        public Guid Id
        {
            get
            {
                var data = _principal;
                return data.Id;
            }

            set
            {

            }
        }
        public Guid Idconexion
        {
            get { return _principal.Idconexion; }
            set { }
        }
        public Guid RoleId
        {
            get
            {
                var data = _principal;
                return data.RoleId;
            }

            set
            {

            }
        }
        public string Usuario
        {
            get
            {
                var data = _principal;
                return data.Usuario;
            }

            set
            {

            }
        }
        public string BaseDatos
        {
            get
            {
                var data = _principal;
                return data.BaseDatos;
            }

            set
            {

            }
        }
        public string Empresa
        {
            get
            {
                var data = _principal;
                return data.Empresa;
            }

            set
            {

            }
        }
        public string Ejercicio
        {
            get
            {
                var data = _principal;
                return data.Ejercicio;
            }

            set
            {

            }
        }
        public string Fkalmacen
        {
            get
            {
                var data = HttpContext.Current.User as ICustomPrincipal;
                return data.Fkalmacen;
            }

            set
            {

            }
        }

        public bool IsSuperAdmin
        {
            get
            {

                return ApplicationHelper.UsuariosAdministrador.Equals(Usuario);
            }
        }

        public string Azureblob
        {
            get
            {
                var data = _principal;
                return data.Azureblob;
            }

            set
            {
                
            }
        }

        public TipoLicencia Tipolicencia {
            get
            {
                var data = HttpContext.Current.User as ICustomPrincipal;
                return data.Tipolicencia;
            } 
            set {} }

        public object Request
        {
            get { return HttpContext.Current; }
        }

        public string ServerMapPath(string ruta)
        {
            return HttpContext.Current.Server.MapPath(ruta);
        }

        public bool IsAuthenticated()
        {
            return HttpContext.Current.User.Identity.IsAuthenticated;
        }

        public UrlHelper GetUrlHelper()
        {
            return new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        public bool IsObjectDictionaryEnabled()
        {
            return HttpContext.Current.Session != null;
        }

        public void SetObject(string key, object elem)
        {
            HttpContext.Current.Session[key] = elem;
        }

        public object GetObject(string key)
        {
            return HttpContext.Current.Session[key];
        }

        public void SetItem(string key, object elem)
        {
            HttpContext.Current.Items[key] = elem;
        }

        public object GetItem(string key)
        {
            return HttpContext.Current.Items[key];
        }

    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "Usuario", ResourceType = typeof(Login))]
        public string Usuario
        {
            get; set;
        }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Login))]
        public string Contraseña
        {
            get; set;
        }

       
        [Display(Name = "Recordarme", ResourceType = typeof(Login))]
        public bool Recordarme { get; set; }

        public string ReturUrl { get; set; }

        public bool Usuariobloqueado { get; set; }
    }
}
