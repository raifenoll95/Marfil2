using System;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Xunit;

namespace Marfil.Dom.Persistencia_UT.FicherosGenerales
{

    public class FormasPago : IClassFixture<BaseFixture>
    {
        private BaseFixture _fixture;

        public static IEnumerable<object[]> NuevasFormaspago => new[]
        {
            new object[]
            {5, "FP","FP",true,true,0,true,true,true,"AAA",false,"CHK", true },
            new object[]
            { 6, "","",true,true,0,false,false,false,"AAA",false,"CHK", false}
        };

        public static IEnumerable<object[]> EditarFormaspago => new[]
        {
            new object[]
            {
                1, "FP","FP",true,true,0,true,true,true,"AAA",false,"CHK",
                1, "FPMOD","FPMOD",true,true,0,false,true,false,"LET",false,"CHK", true
            },
            new object[]
            {
                2, "FP","FP",true,true,0,true,true,true,"AAA",false,"CHK",
                3, "FPMOD","FPMOD",true,true,0,false,true,false,"LET",false,"CHK", false
            }
        };

        public static IEnumerable<object[]> EliminarFormaspago => new[]
        {
            new object[]
            {
                7, "FP","FP",true,true,0,true,true,true,"AAA",false,"CHK",
                7, true
            },
            new object[]
            {
                8, "FP","FP",true,true,0,true,true,true,"AAA",false,"CHK",
                9, false
            },
        };


        public FormasPago(BaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory, MemberData("NuevasFormaspago")]
        public void Alta(int codigo, string descripcion,string descripcion2,bool imprimirVencimientoFacturas,bool excluirFestivos,double recargoFinanciero,bool efectivo,bool remesable,bool mandato,string modopago,bool bloqueado,string fkgruposformaspago, bool result)
        {

            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {


                using (var service = FService.Instance.GetService(typeof (FormasPagoModel), _fixture.Context, db))
                {
                    var obj = new FormasPagoModel
                    {
                     Id= codigo,
                     Nombre = descripcion,
                     Nombre2 = descripcion2,
                     ImprimirVencimientoFacturas = imprimirVencimientoFacturas,
                     ExcluirFestivos = excluirFestivos,
                     RecargoFinanciero = recargoFinanciero,
                     Efectivo = efectivo,
                     Remesable = remesable,
                     Mandato = mandato,
                     ModoPago = modopago,
                     BloqueoModel = new BloqueoEntidadModel() { Bloqueada = false},
                     FkGruposformaspago = fkgruposformaspago
                    };

                    try
                    {
                        service.create(obj);
                    }
                    catch (Exception ex)
                    {
                        Assert.False(result);
                        return;
                    }
                    Assert.True((db.FormasPago.SingleOrDefault(f => f.id == codigo) != null) == result);
                }

            }
        }

        [Theory, MemberData("EditarFormaspago")]
        public void Editar(int codigo, string descripcion, string descripcion2, bool imprimirVencimientoFacturas, bool excluirFestivos, double recargoFinanciero, bool efectivo, bool remesable, bool mandato, string modopago, bool bloqueado, string fkgruposformaspago,
            int codigo2, string descripcion21, string descripcion22, bool imprimirVencimientoFacturas2, bool excluirFestivos2, double recargoFinanciero2, bool efectivo2, bool remesable2, bool mandato2, string modopago2, bool bloqueado2, string fkgruposformaspago2, 
            bool result)
        {
            var objOrig = new FormasPagoModel()
            {
                Id = codigo,
                     Nombre = descripcion,
                     Nombre2 = descripcion2,
                     ImprimirVencimientoFacturas = imprimirVencimientoFacturas,
                     ExcluirFestivos = excluirFestivos,
                     RecargoFinanciero = recargoFinanciero,
                     Efectivo = efectivo,
                     Remesable = remesable,
                     Mandato = mandato,
                     ModoPago = modopago,
                     BloqueoModel = new BloqueoEntidadModel() { Bloqueada = bloqueado },
                     FkGruposformaspago = fkgruposformaspago
            };

            var objUpdate = new FormasPagoModel()
            {
                Id = codigo,
                Nombre = descripcion21,
                Nombre2 = descripcion22,
                ImprimirVencimientoFacturas = imprimirVencimientoFacturas2,
                ExcluirFestivos = excluirFestivos2,
                RecargoFinanciero = recargoFinanciero2,
                Efectivo = efectivo2,
                Remesable = remesable2,
                Mandato = mandato2,
                ModoPago = modopago2,
                BloqueoModel = new BloqueoEntidadModel() { Bloqueada = bloqueado2 },
                FkGruposformaspago = fkgruposformaspago2
            };


            try
            {
                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (var service = FService.Instance.GetService(typeof (FormasPagoModel), _fixture.Context, db))
                    {
                        service.create(objOrig);
                    }

                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (
                        var service =
                            FService.Instance.GetService(typeof (FormasPagoModel), _fixture.Context, db) as FormasPagoService)
                    {
                        service.edit(objUpdate);
                    }
                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (
                        var service =
                            FService.Instance.GetService(typeof (FormasPagoModel), _fixture.Context, db) as FormasPagoService)
                    {
                        var fpModel = db.FormasPago.Single(f => f.id == codigo2);
                        var fpBD = service.get(fpModel.id.ToString()) as FormasPagoModel;
                        Assert.True((fpBD.Nombre == objUpdate.Nombre && fpBD.Nombre2 == objUpdate.Nombre2) == result);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.False(result);

            }
        }

        [Theory, MemberData("EliminarFormaspago")]
        public void Eliminar(int codigo, string descripcion, string descripcion2, bool imprimirVencimientoFacturas, bool excluirFestivos, double recargoFinanciero, bool efectivo, bool remesable, bool mandato, string modopago, bool bloqueado, string fkgruposformaspago,int codigoborrar, bool result)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {


                using (var service = FService.Instance.GetService(typeof (FormasPagoModel), _fixture.Context, db))
                {
                    var obj = new FormasPagoModel
                    {
                        Id = codigo,
                        Nombre = descripcion,
                        Nombre2 = descripcion2,
                        ImprimirVencimientoFacturas = imprimirVencimientoFacturas,
                        ExcluirFestivos = excluirFestivos,
                        RecargoFinanciero = recargoFinanciero,
                        Efectivo = efectivo,
                        Remesable = remesable,
                        Mandato = mandato,
                        ModoPago = modopago,
                        BloqueoModel = new BloqueoEntidadModel() { Bloqueada = false },
                        FkGruposformaspago = fkgruposformaspago
                    };

                    try
                    {
                        service.create(obj);

                    }
                    catch (Exception)
                    {
                        Assert.False(result);
                        return;
                    }
                }
            }

            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {
                using (var service = FService.Instance.GetService(typeof(FormasPagoModel), _fixture.Context, db))
                {
                    try
                    {
                        service.delete(service.get(codigoborrar.ToString()));
                    }
                    catch (Exception ex)
                    {
                        Assert.False(result);
                        return;
                    }
                }
            }

            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {
                Assert.True(!db.FormasPago.Any(f => f.id == codigoborrar));
            }
        }
    }
}
