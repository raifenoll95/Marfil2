using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia_UT
{
    public class UTContextService: IContextService
    {
        private Dictionary<string, object>  _dictionary =new Dictionary<string, object>();
        public UTContextService(string bd)
        {
            Empresa = "0001";
            Ejercicio = "1";
            Fkalmacen = "0010";
            Id= Guid.Empty;
            BaseDatos = bd;
            Azureblob = "";
            
        }

        public Guid Id { get; set; }
        public Guid Idconexion { get; set; }
        public Guid RoleId { get; set; }
        public string Usuario { get; set; }
        public string BaseDatos { get; set; }
        public string Empresa { get; set; }
        public string Ejercicio { get; set; }
        public string Fkalmacen { get; set; }
        public string Azureblob { get; set; }

        public bool IsSuperAdmin
        {
            get { return ApplicationHelper.UsuariosAdministrador.Equals(Usuario); }
        }

        public TipoLicencia Tipolicencia { get; set; }

        public string ServerMapPath(string ruta)
        {
            // get the file attributes for file or directory
            var attr = File.GetAttributes(ruta);

            //detect whether its a directory or file
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                var dio = new DirectoryInfo(ruta);
                return dio.FullName;
            }
            
            var fio = new FileInfo(ruta);
            return fio.FullName;
            
        }

        public bool IsAuthenticated()
        {
            return true;
        }

        public UrlHelper GetUrlHelper()
        {
            throw new NotImplementedException();
        }

        public bool IsObjectDictionaryEnabled()
        {
            return true;
        }

        public void SetObject(string key, object elem)
        {
            if (_dictionary.ContainsKey(key))
                _dictionary.Remove(key);
            _dictionary.Add(key,elem);
        }

        public object GetObject(string key)
        {
            if (_dictionary.ContainsKey(key))
                return _dictionary[key];
            return null;
          
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
}
