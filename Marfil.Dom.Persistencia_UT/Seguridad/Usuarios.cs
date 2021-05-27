using System;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Xunit;

namespace Marfil.Dom.Persistencia_UT.Seguridad
{
    
    public class Usuarios : IClassFixture<BaseFixture>
    {
        private BaseFixture _fixture;

        public static IEnumerable<object[]> NuevosUsuarios => new[]
        {
            new object[] { "test1", "12345","12345",true },
            new object[] { "test2", "12345","123456",false },
            new object[] { "", "12345","12345", false },
            new object[] { "test3", "","", false },
            new object[] { "test4", "1","1", false }//demasiado corta la contraseña
        };

        public static IEnumerable<object[]> EditarUsuarios => new[]
        {
            new object[] { "test5", "12345","12345","tes1","12345","12345",true },
            new object[] { "test6", "12345","12345", "tes1", "12345", "123456", false },
            new object[] { "test7", "12345","12345", "", "12345", "12345", false },
            new object[] { "test8", "12345","12345", "tes1", "", "", false },
            new object[] { "test9", "12345","12345", "tes1", "1", "1", false }//demasiado corta la contraseña
        };

        public static IEnumerable<object[]> EliminarUsuarios => new[]
      {
            new object[] { "test10", "12345","12345","test10",true },
            new object[] { "test11", "12345","12345","testnoexiste", false }
        };

       
        public Usuarios(BaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory, MemberData("NuevosUsuarios")]
        public void Alta(string nombre, string password, string confirmacion, bool result)
        {
          
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {
                
               
                    using (var usuariosService = FService.Instance.GetService(typeof(UsuariosModel), _fixture.Context, db))
                    {
                        var usuario = new UsuariosModel
                        {
                            Usuario = nombre,
                            Password = password,
                            Confirmacionpassword = password
                        };

                        try
                        {
                            usuariosService.create(usuario);
                            Assert.True((db.Usuarios.SingleOrDefault(f=>f.usuario == usuario.Usuario)!=null) == result);
                        }
                        catch (Exception)
                        {
                            Assert.False(result);
                        }
                    }
               
            }
        }

        [Theory, MemberData("EditarUsuarios")]
        public void Editar(string nombre, string password, string confirmacion,string nombre1,string password1,string confirmacion1, bool result)
        {
            var usuario = new UsuariosModel
            {
                Usuario = nombre,
                Password = password,
                Confirmacionpassword = password
            };

            
            try
            {
                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {

                   
                        using (var usuariosService = FService.Instance.GetService(typeof(UsuariosModel), _fixture.Context, db) as UsuariosService)
                        {
                            usuariosService.create(usuario);
                        }
                    
                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {

                    
                        using (var usuariosService = FService.Instance.GetService(typeof(UsuariosModel), _fixture.Context, db) as UsuariosService)
                        {
                            var usuarionew = usuariosService.Get(usuario.Usuario);
                            usuarionew.Usuario = nombre1;
                            usuarionew.Password = password1;
                            usuarionew.Confirmacionpassword = confirmacion1;
                            usuariosService.edit(usuarionew);
                        }
                    
                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {

                    
                        using (var usuariosService = FService.Instance.GetService(typeof(UsuariosModel), _fixture.Context, db) as UsuariosService)
                        {
                            var usuarionew = usuariosService.Get(nombre1);
                            Assert.True((usuarionew.Usuario== nombre1 && usuarionew.Password == password1 ) == result);
                        }
                    
                }
            }
            catch (Exception ex)
            {
                Assert.False(result);
            }
        }

        [Theory, MemberData("EliminarUsuarios")]
        public void Eliminar(string nombre, string password, string confirmacion,string nombre1, bool result)
        {
            var usuario = new UsuariosModel
            {
                Usuario = nombre,
                Password = password,
                Confirmacionpassword = password
            };
            
            try
            {
                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {

                    
                        using (var usuariosService = FService.Instance.GetService(typeof(UsuariosModel), _fixture.Context, db))
                        {


                            try
                            {
                                usuariosService.create(usuario);
                                Assert.True((db.Usuarios.SingleOrDefault(f => f.usuario == usuario.Usuario) != null) == result);
                            }
                            catch (Exception)
                            {
                                Assert.False(result);
                            }
                        }
                    
                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    
                        using (var usuariosService = FService.Instance.GetService(typeof(UsuariosModel), _fixture.Context, db) as UsuariosService)
                        {
                            usuariosService.delete(usuariosService.Get(nombre1));
                            Assert.True(result);
                        }
                    
                }
            }
            catch (Exception)
            {
                Assert.False(result);
            }
           
        }
    }
}
