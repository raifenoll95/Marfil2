using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.App.WebMain.Services
{

    public class ContextService : IContextService
    {
        private readonly ICustomPrincipal _principal;
        private readonly HttpServerUtility _servidor;

        public ContextService()
        {
            _principal = HttpContext.Current.User as ICustomPrincipal;
            _servidor = HttpContext.Current.Server;

            if (_principal == null)
                _principal = new CustomPrincipal("");
        }

        public Guid Idconexion { get { return _principal.Idconexion; } set { } }

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

        public string Azureblob
        {
            get
            {
                var data = _principal;
                return data?.Azureblob;
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
                //var data = HttpContext.Current.User as ICustomPrincipal;
                var data = _principal;
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

        public TipoLicencia Tipolicencia
        {
            get
            {
                var data = HttpContext.Current.User as ICustomPrincipal;
                return data.Tipolicencia;
            }

            set
            {

            }
        }

        public object Request
        {
            get { return HttpContext.Current; }
        }

        public string ServerMapPath(string ruta)
        {
            return _servidor?.MapPath(ruta);
        }

        public bool IsAuthenticated()
        {
            return HttpContext.Current?.User.Identity.IsAuthenticated ?? false;
        }

        public UrlHelper GetUrlHelper()
        {
            return new UrlHelper(HttpContext.Current?.Request.RequestContext);
        }

        public bool IsObjectDictionaryEnabled()
        {
            return HttpContext.Current?.Session != null;
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

        public object Set<T>()
        {
            throw new NotImplementedException();
        }
    }
}