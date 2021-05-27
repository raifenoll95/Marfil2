using System;
using System.Collections.Generic;
using System.Linq;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Xunit;

namespace Marfil.Dom.Persistencia_UT.Administrador
{

    public class Ejercicios : IClassFixture<BaseFixture>
    {
        private BaseFixture _fixture;

        public static IEnumerable<object[]> NuevosEjercicios => new[]
        {
            new object[]
            {
                "Descripcion", "Corta", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja,
                null, true
            },
            new object[]
            {"", "Corta", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja, null, false},
            new object[]
            {
                "Descripcion", "", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null,CriterioIVA.Caja, null,
                false
            },
            new object[]
            {
                "Descripcion", "Corta", DateTime.Now.AddYears(1), DateTime.Now, EstadoEjercicio.Abierto, null, null,CriterioIVA.Caja,
                null, false
            },
            new object[] {"Descripcion", "Corta", null, null, EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja, null, false}
        };

        public static IEnumerable<object[]> EditarEjercicios => new[]
        {
            new object[]
            {
                "Descripcion", "Corta", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja,
                null, "Descripcion2", "Corta2", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null,
                null, CriterioIVA.Caja, null, true
            },
            new object[]
            {
                "Descripcion", "Corta", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja,
                null, "Descripcion2", "Corta2", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Cerrado, null,
                null, CriterioIVA.Caja, null, true
            },
            new object[]
            {
                "Descripcion", "Corta", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja,
                null, "Descripcion2", "Corta2", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Cerrado, null,
                null, CriterioIVA.Caja, null, true
            },
            new object[]
            {
                "Descripcion", "Corta", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja,
                null, "Descripcion2", "Corta2", DateTime.Now.AddYears(1), DateTime.Now, EstadoEjercicio.Cerrado, null,
                null, CriterioIVA.Caja, null, false
            },
            new object[]
            {
                "Descripcion", "Corta", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja,
                null, "Descripcion", "Corta", null, null, EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja, null, false
            },
            new object[]
            {
                "Descripcion", "Corta", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja,
                null, "Descripcion", "Corta", DateTime.Now.AddYears(1), DateTime.Now, EstadoEjercicio.Abierto, null,
                null, CriterioIVA.Caja, null, false
            }

        };

        public static IEnumerable<object[]> EliminarEjercicios => new[]
        {
            new object[]
            {
                "Descripcion", "Corta1", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null,  CriterioIVA.Caja,
                null, "Corta1", true
            },
            new object[]
            {
                "Descripcion", "Corta2", DateTime.Now, DateTime.Now.AddYears(1), EstadoEjercicio.Abierto, null, null, CriterioIVA.Caja,
                null, "Corta3", false
            },
        };


        public Ejercicios(BaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory, MemberData("NuevosEjercicios")]
        public void Alta(string descripcion, string descripcioncorta, DateTime? desde, DateTime? hasta,
            EstadoEjercicio estado, DateTime? contacerradahasta, DateTime? registrocerradahasta,
            CriterioIVA? customCriterioIva, int? fkejercicios, bool result)
        {

            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {


                using (var service = FService.Instance.GetService(typeof (EjerciciosModel), _fixture.Context, db))
                {
                    var obj = new EjerciciosModel
                    {
                        Empresa= _fixture.Context.Empresa,
                        Descripcion = descripcion,
                        Descripcioncorta = descripcioncorta,
                        Desde = desde,
                        Hasta = hasta,
                        Estado = estado,
                        Contabilidadcerradahasta = contacerradahasta,
                        Registroivacerradohasta = registrocerradahasta,
                        CustomCriterioIva = customCriterioIva,
                        Fkejercicios = fkejercicios
                    };

                    try
                    {
                        service.create(obj);
                        Assert.True((db.Ejercicios.SingleOrDefault(f => f.descripcioncorta == descripcioncorta) != null) ==
                                    result);
                    }
                    catch (Exception ex)
                    {
                        Assert.False(result);
                    }
                }

            }
        }

        [Theory, MemberData("EditarEjercicios")]
        public void Editar(string descripcion, string descripcioncorta, DateTime? desde, DateTime? hasta,
            EstadoEjercicio estado, DateTime? contacerradahasta, DateTime? registrocerradahasta,
            CriterioIVA? customCriterioIva, int? fkejercicios,
            string descripcion2, string descripcioncorta2, DateTime? desde2, DateTime? hasta2, EstadoEjercicio estado2,
            DateTime? contacerradahasta2, DateTime? registrocerradahasta2, CriterioIVA? customCriterioIva2,
            int? fkejercicios2,
            bool result)
        {
            var objOrig = new EjerciciosModel
            {
                Empresa = _fixture.Context.Empresa,
                Descripcion = descripcion,
                Descripcioncorta = descripcioncorta,
                Desde = desde,
                Hasta = hasta,
                Estado = estado,
                Contabilidadcerradahasta = contacerradahasta,
                Registroivacerradohasta = registrocerradahasta,
                CustomCriterioIva = customCriterioIva,
                Fkejercicios = fkejercicios
            };

            var objUpdate = new EjerciciosModel
            {
                Empresa = _fixture.Context.Empresa,
                Descripcion = descripcion2,
                Descripcioncorta = descripcioncorta2,
                Desde = desde2,
                Hasta = hasta2,
                Estado = estado2,
                Contabilidadcerradahasta = contacerradahasta2,
                Registroivacerradohasta = registrocerradahasta2,
                CustomCriterioIva = customCriterioIva2,
                Fkejercicios = fkejercicios2
            };


            try
            {
                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (var service = FService.Instance.GetService(typeof (EjerciciosModel), _fixture.Context, db))
                    {
                        service.create(objOrig);
                    }

                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (
                        var service =
                            FService.Instance.GetService(typeof (EjerciciosModel), _fixture.Context, db) as EjerciciosService)
                    {
                        var ejercicioModel = db.Ejercicios.Single(f => f.descripcioncorta == objOrig.Descripcioncorta && f.empresa == _fixture.Context.Empresa);
                        objUpdate.Id = ejercicioModel.id;
                        service.edit(objUpdate);
                    }
                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (
                        var service =
                            FService.Instance.GetService(typeof (EjerciciosModel), _fixture.Context, db) as EjerciciosService)
                    {
                        var ejercicioModel = db.Ejercicios.Single(f => f.descripcioncorta == objUpdate.Descripcioncorta && f.empresa==_fixture.Context.Empresa);
                        var ejercicioBD = service.get(ejercicioModel.id.ToString()) as EjerciciosModel;
                        Assert.True((ejercicioBD.Descripcioncorta == objUpdate.Descripcioncorta) == result);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.False(result);
            }
        }

        [Theory, MemberData("EliminarEjercicios")]
        public void Eliminar(string descripcion, string descripcioncorta, DateTime? desde, DateTime? hasta,
            EstadoEjercicio estado, DateTime? contacerradahasta, DateTime? registrocerradahasta,
            CriterioIVA? customCriterioIva, int? fkejercicios, string ejercicioEliminar, bool result)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {


                using (var service = FService.Instance.GetService(typeof (EjerciciosModel), _fixture.Context, db))
                {
                    var obj = new EjerciciosModel
                    {
                        Empresa = _fixture.Context.Empresa,
                        Descripcion = descripcion,
                        Descripcioncorta = descripcioncorta,
                        Desde = desde,
                        Hasta = hasta,
                        Estado = estado,
                        Contabilidadcerradahasta = contacerradahasta,
                        Registroivacerradohasta = registrocerradahasta,
                        CustomCriterioIva = customCriterioIva,
                        Fkejercicios = fkejercicios
                    };

                    try
                    {
                        service.create(obj);

                    }
                    catch (Exception)
                    {
                        Assert.False(result);
                    }
                }
            }

            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {
                using (var service = FService.Instance.GetService(typeof(EjerciciosModel), _fixture.Context, db) as EjerciciosService)
                {
                    try
                    {
                        var ejercicioModel = db.Ejercicios.Single(f => f.descripcioncorta == ejercicioEliminar);
                        service.delete(service.get(ejercicioModel.id.ToString()));
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
                using (var service = FService.Instance.GetService(typeof(EjerciciosModel), _fixture.Context, db) as EjerciciosService)
                {
                    try
                    {
                        var ejercicioModel = db.Ejercicios.Single(f => f.descripcioncorta == ejercicioEliminar);
                        service.get(ejercicioModel.id.ToString());
                        Assert.False(result);
                    }
                    catch (Exception)
                    {
                        Assert.True(result);
                    }
                }
            }
        }
    }
}
