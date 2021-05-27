using System;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView;
using Xunit;

namespace Marfil.Dom.Persistencia_UT.General
{
    public class Cuentas : IClassFixture<GeneralFixture>
    {
        private GeneralFixture _fixture;
        public Cuentas(GeneralFixture fixture)
        {
            _fixture = fixture;
        }

        public static IEnumerable<object[]> NuevasCuentas => new[]
       {
            new object[] { "10000000","Cuenta pruebas", "37879906X", "078",5,true },
            new object[] { "10000000","", "39941255F", "078",0,false },
            new object[] { "10000000","Cuenta pruebas", "28228668D", "078",0,false }
        };

        public static IEnumerable<object[]> NuevasCuentasDuplicadas => new[]
       {
            new object[] { "20000000","Cuenta pruebas", "37006076L", "078",6, "20000001", "Cuenta pruebas", "37006076L", "078",true },
            new object[] { "30000000","Cuenta pruebas", "29396040Q", "078",10, "40000001", "Cuenta pruebas", "09125077B", "078",true },
            new object[] { "50000000","Cuenta pruebas", "55043910B", "078",0, "50000000", "Cuenta pruebas", "37405493H", "078",false }
        };

        public static IEnumerable<object[]> EditarCuentas => new[]
       {
            new object[] { "60000000","Cuenta pruebas", "86743711Q", "078", "MODIFICADO", "86743711Q", "068", true }
            
        };

        public static IEnumerable<object[]> EliminarCuentas => new[]
       {
            new object[] { "70000000","Cuenta pruebas", "86743711Q", "078", "70000000", true },
            new object[] { "80000000","Cuenta pruebas", "86743711Q", "078", "800", false }

        };


        [Theory, MemberData("NuevasCuentas")]
        public void Alta(string id, string razonsocial, string dni, string fkpais, int cuentastotales, bool result)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {
                using (var service = FService.Instance.GetService(typeof(CuentasModel), _fixture.Context, db))
                {
                    try
                    {
                        var model = new CuentasModel()
                        {
                            Empresa = _fixture.Empresa,
                            Id = id,
                            Descripcion = razonsocial,
                            Nif = new NifCifModel() { Nif = dni, TipoNif = "1" },
                            FkPais = fkpais,
                            UsuarioId = Guid.Empty.ToString()

                        };
                        service.create(model);
                    }
                    catch (Exception ex)
                    {
                        Assert.False(result);
                    }
                }
            }
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            using (var service = FService.Instance.GetService(typeof(CuentasModel), _fixture.Context, db))
            {
                try
                {
                    Assert.True(service.getAll().OfType<CuentasModel>().Count(f => f.Id.StartsWith(id.ToCharArray()[0].ToString())) == cuentastotales);
                }
                catch (Exception ex)
                {
                    Assert.False(result);
                }
            }
        }

        [Theory, MemberData("NuevasCuentasDuplicadas")]
        public void AltaDoble(string id, string razonsocial, string dni, string fkpais, int cuentastotales, string id2, string razonsocial2, string dni2, string fkpais2, bool result)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {
                using (var service = FService.Instance.GetService(typeof(CuentasModel), _fixture.Context, db))
                {
                    try
                    {
                        var model = new CuentasModel()
                        {
                            Empresa = _fixture.Empresa,
                            Id = id,
                            Descripcion = razonsocial,
                            Nif = new NifCifModel() { Nif = dni, TipoNif = "1" },
                            FkPais = fkpais,
                            UsuarioId = Guid.Empty.ToString()

                        };

                        var model2 = new CuentasModel()
                        {
                            Empresa = _fixture.Empresa,
                            Id = id2,
                            Descripcion = razonsocial2,
                            Nif = new NifCifModel() { Nif = dni2, TipoNif = "1" },
                            FkPais = fkpais2,
                            UsuarioId = Guid.Empty.ToString()

                        };
                        service.create(model);
                        service.create(model2);
                    }
                    catch (Exception ex)
                    {
                        Assert.False(result);
                    }
                }
            }
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            using (var service = FService.Instance.GetService(typeof(CuentasModel), _fixture.Context, db))
            {
                try
                {
                    Assert.True(service.getAll().OfType<CuentasModel>().Count(f => f.Id.StartsWith(id.ToCharArray()[0].ToString()) || f.Id.StartsWith(id2.ToCharArray()[0].ToString())) == cuentastotales);
                }
                catch (Exception ex)
                {
                    Assert.False(result);
                }
            }
        }

        [Theory, MemberData("EditarCuentas")]
        public void Editar(string id, string razonsocial, string dni, string fkpais,  string razonsocial2, string dni2, string fkpais2, bool result)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {
                using (var service = FService.Instance.GetService(typeof(CuentasModel), _fixture.Context, db))
                {
                    try
                    {
                        var model = new CuentasModel()
                        {
                            Empresa = _fixture.Empresa,
                            Id = id,
                            Descripcion = razonsocial,
                            Nif = new NifCifModel() { Nif = dni, TipoNif = "1" },
                            FkPais = fkpais,
                            UsuarioId = Guid.Empty.ToString()

                        };
                        service.create(model);
                    }
                    catch (Exception ex)
                    {
                        Assert.False(result);
                    }
                }
            }
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            using (var service = FService.Instance.GetService(typeof(CuentasModel), _fixture.Context, db))
            {
                try
                {
                    var cuenta = service.get(id) as CuentasModel;
                    cuenta.Descripcion = razonsocial2;
                    cuenta.Nif.Nif = dni2;
                    cuenta.FkPais = fkpais2;
                    service.edit(cuenta);
                }
                catch (Exception ex)
                {
                    Assert.False(result);
                }
            }

            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            using (var service = FService.Instance.GetService(typeof(CuentasModel), _fixture.Context, db))
            {
                try
                {
                    var cuenta = service.get(id) as CuentasModel;
                    
                    Assert.True(razonsocial2==cuenta.Descripcion && dni2==cuenta.Nif.Nif && fkpais2== cuenta.FkPais);
                }
                catch (Exception ex)
                {
                    Assert.False(result);
                }
            }
        }

        [Theory, MemberData("EliminarCuentas")]
        public void Eliminar(string id, string razonsocial, string dni, string fkpais, string id2, bool result)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {
                using (var service = FService.Instance.GetService(typeof(CuentasModel), _fixture.Context, db))
                {
                    try
                    {
                        var model = new CuentasModel()
                        {
                            Empresa = _fixture.Empresa,
                            Id = id,
                            Descripcion = razonsocial,
                            Nif = new NifCifModel() { Nif = dni, TipoNif = "1" },
                            FkPais = fkpais,
                            UsuarioId = Guid.Empty.ToString()

                        };
                        service.create(model);
                    }
                    catch (Exception ex)
                    {
                        Assert.False(result);
                    }
                }
            }
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            using (var service = FService.Instance.GetService(typeof(CuentasModel), _fixture.Context, db))
            {
                try
                {
                    var cuenta = service.get(id2) as CuentasModel;
                    service.delete(cuenta);
                    Assert.True(!db.Cuentas.Any(f=>f.id==id && f.empresa==_fixture.Context.Empresa) && result);
                }
                catch (Exception ex)
                {
                    Assert.False(result);
                }
            }

            
        }


    }
}
