using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView;
using Xunit;
using System.Data.Entity;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia_UT.Seguridad
{
    public class Roles: IClassFixture<BaseFixture>
    {
        private BaseFixture _fixture;
        public Roles(BaseFixture fixture)
        {
            _fixture = fixture;
        }

        public static IEnumerable<object[]> NuevosRoles => new[]
       {
            new object[] { "test1", "", new UsuariosModel[] {}, true },

            new object[] { "test2", "", new []
            {
                new UsuariosModel() {Id = Guid.NewGuid(), Usuario = "test3",Password="12345",Confirmacionpassword = "12345" }
            }, true},

            new object[] { "test4", "", new []
            {
                new UsuariosModel() {Id = Guid.NewGuid(),Usuario = "test5",Password="12345",Confirmacionpassword = "12345" } ,
                new UsuariosModel() {Id = Guid.NewGuid(),Usuario = "test5",Password="12345",Confirmacionpassword = "12345" }
            }, false}

        };

        
       

        [Theory, MemberData("NuevosRoles")]
        public void Alta(string nombre, string xml,IEnumerable<UsuariosModel> lstUsuarios, bool result)
        {
            var role = new RolesModel
            {
                Role = nombre,
                Permisos = new PermisosModel(),
                Usuarios = new GruposUsuariosModel() { usuarios = lstUsuarios.Select(f => new UsuariosRelacionRolesModel() { id = f.Id, usuario = f.Usuario }).ToList() }
            };

           
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {

                
                    using (var usuariosService = FService.Instance.GetService(typeof(UsuariosModel), _fixture.Context, db))
                    
                    {
                        try
                        {
                            foreach(var item in lstUsuarios)
                                usuariosService.create(item);
                        }
                        catch (Exception ex)
                        {
                            Assert.False(result);
                        }
                    }
                
            }
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {
                using (var usuariosService = FService.Instance.GetService(typeof (UsuariosModel), _fixture.Context, db) as UsuariosService)
                {
                    role.Usuarios = new GruposUsuariosModel()
                    {
                        usuarios =
                            lstUsuarios.Select(
                                f =>
                                    new UsuariosRelacionRolesModel()
                                    {
                                        id = usuariosService.Get(f.Usuario).Id,
                                        usuario = f.Usuario
                                    }).ToList()
                    };

                    using (var rolesService = FService.Instance.GetService(typeof (RolesModel), _fixture.Context, db))
                    {
                        try
                        {
                            rolesService.create(role);
                            Assert.True((db.Roles.SingleOrDefault(f => f.id == role.Id) != null) == result);
                            Assert.True(
                                (db.Roles.Include(f => f.Usuarios).Single(f => f.id == role.Id).Usuarios.Count() ==
                                 role.Usuarios.usuarios.Count()) == result);
                        }
                        catch (Exception ex)
                        {
                            Assert.False(result);
                        }
                    }
                }

            }
        }

       
    }
}
