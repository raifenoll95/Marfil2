using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Login
{
    public class LoginContextService : IContextService
    {
        public LoginContextService(string empresaid, string basedatos)
        {
            Empresa = empresaid;
            BaseDatos = basedatos;
        }

        public Guid Id { get; set; }
        public Guid Idconexion { get; set; }
        public Guid RoleId { get; set; }
        public string Usuario { get; set; }
        public string BaseDatos { get; set; }
        public string AzureBlob { get; set; }
        public string Empresa { get; set; }
        public string Ejercicio { get; set; }
        public string Fkalmacen { get; set; }

        public bool IsSuperAdmin
        {
            get
            {
                return ApplicationHelper.UsuariosAdministrador.Equals(Usuario);
            }
        }

        public TipoLicencia Tipolicencia { get; set; }

        public string Azureblob
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string ServerMapPath(string ruta)
        {
            throw new NotImplementedException();
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

    public class LoginService : ILoginService,IDisposable
    {
        
        public bool Login(string dominio, string usuario, string contraseña,out SecurityTicket model)
        {
            return LoginInternal(dominio,usuario, contraseña,out model,string.Empty,string.Empty,string.Empty);
        }

        public bool Login(string dominio, string usuario, string contraseña,  out HttpCookie securityCookie)
        {
            return Login(dominio, usuario, contraseña,  out securityCookie, string.Empty,string.Empty,string.Empty);
        }

        public bool Login(string dominio,string usuario, string contraseña,  out HttpCookie securityCookie,string empresa,string ejercicio,string almacen)
        {
            securityCookie = null;
            try
            {
                
                var modelSecurity = new SecurityTicket();
                if (LoginInternal(dominio, usuario, contraseña,  out modelSecurity, empresa, ejercicio,almacen))
                {
                    if (modelSecurity != null)
                    {
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
                }
            }
            catch (Exception ex)
            {
                if (ex is LicenciaException)
                    throw;
                if (ex is UsuarioactivoException)
                    throw;
                if (ex is UsuarioensuoException)
                    throw;
                if (ex is CambiarEmpresaException)
                    throw;
                Console.Write(ex.Message);
            }

            return false;
        }

        private void Validarlicencia(Guid idusuario,string usuario,LicenciasaplicacionService licenciasService,MarfilEntities db)
        {
            
            if(!licenciasService.Activado)
                throw new LicenciaException("No tiene activada ninguna licencia. Consulte a su administrador.");

            if(db.Usuariosactivos.Any(f=>f.fkusuarios== idusuario))
                throw new UsuarioensuoException(string.Format("El usuario {0} ya está en uso.", usuario));

            if (db.Usuariosactivos.Count()>=licenciasService.Usuarioslicencia)
                throw new UsuarioactivoException(string.Format("Se ha alcanzado el número máximo de usuarios activos ({0}) y no puede conectarse a la aplicación.", licenciasService.Usuarioslicencia));
            
        }

        private bool LoginInternal(string dominio,string usuario, string contraseña,  out SecurityTicket model,string empresa,string ejercicio,string almacen)
        {
            var result = true;
            model = null;
            try
            {
                
                var tusuario = usuario;
                var tid = Guid.Empty;
                var troleid = Guid.Empty;
                var tbasedatos = "";
                var tempresa = string.Empty;

                var tazureblob = "";


                var licenciaModel = new LicenciasaplicacionService(dominio);
                tbasedatos = licenciaModel.Basedatos;
                tazureblob = licenciaModel.Azureblob;

                using (var db = MarfilEntities.ConnectToSqlServer(licenciaModel.Basedatos))
                {
                 
                    if (usuario.Equals(ApplicationHelper.UsuariosAdministrador))
                    {
                        var admin = db.Administrador.FirstOrDefault(u => u.password == contraseña);
                        if (admin == null)
                            return false;
                    }
                    else
                    {
                        var objUser = db.Usuarios.FirstOrDefault(u => u.usuario == usuario && u.password == contraseña);
                        if (objUser != null)
                        {
                            var objRole = db.Roles.First(f => f.Usuarios.Any(j => j.id == objUser.id));
                            tid = objUser.id;
                            troleid = objRole.id;
                        }
                        else
                            return false;
                    }

                    //Rai
                    tempresa = string.IsNullOrEmpty(empresa) ? GetEmpresaDefecto(tid, db) : empresa;
                    var descripcionempresa = db.Empresas.Where(f => f.id == tempresa).Select(f => f.nombre).SingleOrDefault();

                    //En el Startup aun no se tiene empresa, por lo que tambien debe dejar entrar (solo está el admin)
                    if (puedeEntrarEmpresa(tbasedatos, tusuario, tempresa, dominio) || (tempresa == "mock" && db.Usuarios.Count() == 1))
                    {
                        Validarlicencia(tid, usuario, licenciaModel, db);
                        
                        var context = new LoginContextService(tempresa, licenciaModel.Basedatos);
                        var ejercicioService = FService.Instance.GetService(typeof(EjerciciosModel), context, db) as EjerciciosService;
                        var almacenService = FService.Instance.GetService(typeof(AlmacenesModel), context, db) as AlmacenesService;
                        var talmacen = string.IsNullOrEmpty(almacen) ? almacenService.GetAlmacenDefecto(tid, db, tempresa) : almacen;
                        var tejercicio = string.IsNullOrEmpty(ejercicio) ? ejercicioService.GetEjercicioDefecto(tid, db, tempresa) : ejercicio;

                        model = new SecurityTicket
                        {
                            Usuario = tusuario,
                            Id = tid,
                            RoleId = troleid,
                            BaseDatos = tbasedatos,
                            Empresa = tempresa,
                            Ejercicio = tejercicio,
                            Fkalmacen = talmacen,
                            Tipolicencia = licenciaModel.TipoLicencia,
                            Azureblob = tazureblob
                            //serializeModel.Roles = db.USUARIOROLE.Where(i => i.IDUSUARIO == u.ID).Select(i => i.ROLE.NOMBRE).ToList();
                        };
                        CreateUsuarioActivo(model, db);
                        // esto es la validación
                        //var u = db.Usuarios.FirstOrDefault(i => i.Usuario == usu.Email && i.PWD == usu.Password && i.IFACTIVO);

                        var preferencias = new PreferenciasUsuarioService(db);
                        preferencias.SetPreferencia(TiposPreferencias.EmpresaDefecto, tid, PreferenciaEmpresaDefecto.Id, PreferenciaEmpresaDefecto.Nombre, new PreferenciaEmpresaDefecto() { Empresa = tempresa });

                        if (tempresa != ApplicationHelper.EmpresaMock)
                        {
                            var ejercicioPreferencia = new PreferenciaEjercicioDefecto();
                            ejercicioPreferencia.SetEjercicio(tempresa, tejercicio);

                            var almacenPreferencia = new PreferenciaAlmacenDefecto();
                            almacenPreferencia.SetAlmacen(tempresa, talmacen);

                            preferencias.SetPreferencia(TiposPreferencias.EjercicioDefecto, tid, PreferenciaEjercicioDefecto.Id, PreferenciaEjercicioDefecto.Nombre, ejercicioPreferencia);
                            preferencias.SetPreferencia(TiposPreferencias.AlmacenDefecto, tid, PreferenciaAlmacenDefecto.Id, PreferenciaAlmacenDefecto.Nombre, almacenPreferencia);


                        }
                    }
                    
                    else
                    {
                        throw new CambiarEmpresaException("No tiene permisos sobre la empresa "  + descripcionempresa + " Consulte con su administrador");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                if (ex is LicenciaException)
                    throw;
                if (ex is UsuarioactivoException)
                    throw;
                if (ex is UsuarioensuoException)
                    throw;
                if (ex is CambiarEmpresaException)
                    throw;
                result = false;
            }

            return result;
        }


        private void CreateUsuarioActivo(SecurityTicket model,MarfilEntities db)
        {
            try
            {
                var nuevoUsuarioActivo = db.Usuariosactivos.Create();
                nuevoUsuarioActivo.fkusuarios = model.Id;
                nuevoUsuarioActivo.fechaconexion = DateTime.Now;
                nuevoUsuarioActivo.fechaultimaoperacion = DateTime.Now;
                nuevoUsuarioActivo.idconexion = Guid.NewGuid();
                model.Idconexion = nuevoUsuarioActivo.idconexion;
                db.Usuariosactivos.Add(nuevoUsuarioActivo);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw new UsuarioensuoException(string.Format("El usuario {0} ya está en uso.",model.Usuario));
            }
           
            
        }
        private string GetEmpresaDefecto(Guid tusuario,MarfilEntities db)
        {
            using (var preferencias = new PreferenciasUsuarioService(db))
            {
                var preferenciaItem =preferencias.GePreferencia(TiposPreferencias.EmpresaDefecto, tusuario, PreferenciaEmpresaDefecto.Id, PreferenciaEmpresaDefecto.Nombre) as PreferenciaEmpresaDefecto;
                return preferenciaItem?.Empresa ?? db.Empresas.First().id;
            }
        }

        public void SetEmpresaUserAdmin(string dominio,string basedatos,string empresa,string ejercicio,string almacen,Guid idconexion,out HttpCookie cookie)
        {
            SetEmpresaUser(dominio,basedatos, ApplicationHelper.UsuariosAdministrador, empresa, ejercicio,almacen, idconexion, out cookie);
        }

        public void SetEmpresaUser(string dominio,string basedatos,string user, string empresa,string ejercicio,string almacen, Guid idconexion,out HttpCookie cookie)
        {
            var tpassword = string.Empty;
            var tusuario = string.Empty;
            var tbasedatos = string.Empty;

            using (var db = MarfilEntities.ConnectToSqlServer(basedatos))
            {
                tpassword = user == ApplicationHelper.UsuariosAdministrador ? db.Administrador.Single().password : db.Usuarios.Single(f=>f.usuario == user).password;
                tusuario = user;

                FormsAuthentication.SignOut();
               
                    var usuarioActivo = db.Usuariosactivos.SingleOrDefault(f => f.idconexion == idconexion);
                    if (usuarioActivo != null)
                    {
                        db.Usuariosactivos.Remove(usuarioActivo);
                        db.SaveChanges();
                    }

               
                Login(dominio,tusuario, tpassword,  out cookie, empresa, ejercicio,almacen);
            }
        }

        //Rai ---> puedeCambiarEempresa para el botón de arriba del header, se tienen en cuenta el check de poder cambiar de empresa
        public bool puedeCambiarEmpresa(string basedatos, string user, string empresa, string dominio)
        {
            var result = true;        
            try
            {
                var licenciaModel = new LicenciasaplicacionService(dominio);
                using (var db = MarfilEntities.ConnectToSqlServer(licenciaModel.Basedatos))
                {
                    var descripcionempresa = db.Empresas.Where(f => f.id == empresa).Select(f => f.nombre).SingleOrDefault();
                    var usuariodb = db.Usuarios.Where(f => f.usuario == user).Single();
                    var empresadb = db.Empresas.Where(f => f.id == empresa).Single();

                    if (!(ApplicationHelper.UsuariosAdministrador == user || ((usuariodb.nivel >= empresadb.nivel || (usuariodb.nivel == null && empresadb.nivel == 0)) && usuariodb.cambiarempresa == true)))
                    {
                        throw new CambiarEmpresaException("No tiene permisos sobre la empresa " + descripcionempresa + " Consulte con su administrador");
                    }
                }
                    
            }
            catch(Exception ex)
            {
                if (ex is CambiarEmpresaException)
                    throw;
                result = false;
            }

            return result;
        }

        //Rai ---> puedeEntrarEempresa para el botón de arriba del header, NO se tienen en cuenta el check de poder cambiar de empresa
        public bool puedeEntrarEmpresa(string basedatos, string user, string empresa, string dominio)
        {
            var result = true;
            try
            {
                var licenciaModel = new LicenciasaplicacionService(dominio);
                using (var db = MarfilEntities.ConnectToSqlServer(licenciaModel.Basedatos))
                {
                    var descripcionempresa = db.Empresas.Where(f => f.id == empresa).Select(f => f.nombre).SingleOrDefault();
                    var usuariodb = db.Usuarios.Where(f => f.usuario == user).Single();
                    var empresadb = db.Empresas.Where(f => f.id == empresa).Single();

                    if (!(ApplicationHelper.UsuariosAdministrador == user || (usuariodb.nivel >= empresadb.nivel) || (usuariodb.nivel == null && empresadb.nivel == 0)))
                    {
                        throw new CambiarEmpresaException("No tiene permisos sobre la empresa " + descripcionempresa + " Consulte con su administrador");
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex is CambiarEmpresaException)
                    throw;
                result = false;
            }

            return result;
        }

        public void Logout(ICustomPrincipal customPrincipal)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(customPrincipal.BaseDatos))
            {
                var usuarioActivo = db.Usuariosactivos.SingleOrDefault(f => f.idconexion == customPrincipal.Idconexion);
                if (usuarioActivo != null)
                {
                    db.Usuariosactivos.Remove(usuarioActivo);
                    db.SaveChanges();
                }
                
            }
        }

        public void Forzardesconexion(string basedatos,string idusuario)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(basedatos))
            {
                var usuario = db.Usuarios.Single(f => f.usuario == idusuario);
                var usuarioActivo = db.Usuariosactivos.Single(f => f.fkusuarios == usuario.id);
                db.Usuariosactivos.Remove(usuarioActivo);
                db.SaveChanges();
            }
        }

        public bool Existeusuarioactivo(string basedatos, Guid idconexion)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(basedatos))
            {
                return  db.Usuariosactivos.Any(f => f.idconexion == idconexion);
                
            }
        }

        public bool checkPassword(string dominio, string usuario, string password, out string basedatos)
        {
            bool retorno = false;

            var licenciaModel = new LicenciasaplicacionService(dominio);
            if (!licenciaModel.Activado)
                throw new LicenciaException("No tiene activada ninguna licencia. Consulte a su administrador.");

            basedatos = licenciaModel.Basedatos;

            using (var db = MarfilEntities.ConnectToSqlServer(basedatos))
            {

                if (usuario.Equals(ApplicationHelper.UsuariosAdministrador))
                {
                    var admin = db.Administrador.FirstOrDefault(u => u.password == password);
                    if (admin != null)
                        return true;
                }
                else
                {
                    var objUser = db.Usuarios.FirstOrDefault(u => u.usuario == usuario && u.password == password);
                    if (objUser != null)
                        return true;
                }

                return retorno;
            }
        }

        public ICustomPrincipal getUserActivo(string dominio, string usuario, string password, Guid idconexion)
        {
            CustomPrincipal principal = null;
            string basedatos;
            if (checkPassword(dominio,usuario,password, out basedatos) && Existeusuarioactivo(basedatos,idconexion))
            {
                using (var db = MarfilEntities.ConnectToSqlServer(basedatos))
                {
                    var objUser = db.Usuarios.Single(f => f.usuario == usuario);
                    if (objUser != null)
                    {
                        if (objUser != null)
                        {
                            var objRole = db.Roles.First(f => f.Usuarios.Any(j => j.id == objUser.id));
                            var troleid = objRole?.id;

                            var tempresa = GetEmpresaDefecto(objUser.id, db);
                            var context = new LoginContextService(tempresa, basedatos);
                            var ejercicioService = FService.Instance.GetService(typeof(EjerciciosModel), context, db) as EjerciciosService;
                            var almacenService = FService.Instance.GetService(typeof(AlmacenesModel), context, db) as AlmacenesService;
                            var talmacen =  almacenService.GetAlmacenDefecto(objUser.id, db, tempresa) ;
                            var tejercicio = ejercicioService.GetEjercicioDefecto(objUser.id, db, tempresa);

                            principal = new CustomPrincipal(usuario)
                            {
                                Id = objUser.id,
                                RoleId = troleid ?? Guid.NewGuid(),
                                Usuario = objUser.usuario,
                                BaseDatos = basedatos,
                                Roles = objUser.Roles.Select(x => x.role).ToList(),
                                Empresa = tempresa,
                                Ejercicio = tejercicio,
                                Idconexion = idconexion,
                                Fkalmacen = talmacen
                            };
                        }
                    }
                    
                }
            }
            return principal;
        }

        public void Dispose()
        {
            
        }

        public bool CambiarEmpresa(string baseDatos, string usuario, string id, string dnsSafeHost)
        {
            throw new NotImplementedException();
        }
    }
}
