using System;
using System.Collections;
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

    public class Criteriosagrupacion : IClassFixture<BaseFixture>
    {
        private BaseFixture _fixture;

        public static IEnumerable<object[]> NuevasCriteriosagrupacion => new[]
        {
            new object[]
            {"0001", "Agrupacion1",true,Enumerable.Empty<CriteriosagrupacionLinModel>(), true },
            new object[]
             {"0002", "",true,Enumerable.Empty<CriteriosagrupacionLinModel>(), false },
            new object[]
            {"0003", "Agrupacion2",true,new[] { new CriteriosagrupacionLinModel()
            {
                Id = 1,
                Campoenum = CamposAgrupacionAlbaran.Fkarticulos,
            },
            new CriteriosagrupacionLinModel()
            {
                Id = 2,
                Campoenum = CamposAgrupacionAlbaran.Descripcion
            }}
            , false },
            new object[]
            {"0003", "Agrupacion2",true,new[] { new CriteriosagrupacionLinModel()
            {
                Id = 1,
                Campoenum = CamposAgrupacionAlbaran.Fkarticulos,
            },
            new CriteriosagrupacionLinModel()
            {
                Id = 2,
                Campoenum = CamposAgrupacionAlbaran.Descripcion
            },
            new CriteriosagrupacionLinModel()
            {
                Id = 3,
                Campoenum = CamposAgrupacionAlbaran.Precio
            },
            new CriteriosagrupacionLinModel()
            {
                Id = 4,
                Campoenum = CamposAgrupacionAlbaran.Porcentajedescuento
            }}
            , true },

        };

        public static IEnumerable<object[]> EditarCriteriosagrupacion => new[]
        {
            new object[]
           {"0004", "Agrupacion1",true,Enumerable.Empty<CriteriosagrupacionLinModel>(),
               "0004", "Agrupacionmod",true,new[] { new CriteriosagrupacionLinModel()
            {
                Id = 1,
                Campoenum = CamposAgrupacionAlbaran.Fkarticulos,
            },
            new CriteriosagrupacionLinModel()
            {
                Id = 2,
                Campoenum = CamposAgrupacionAlbaran.Descripcion
            },
               new CriteriosagrupacionLinModel()
            {
                Id = 3,
                Campoenum = CamposAgrupacionAlbaran.Precio
            },
            new CriteriosagrupacionLinModel()
            {
                Id = 4,
                Campoenum = CamposAgrupacionAlbaran.Porcentajedescuento
            }}, true },
            new object[]
            {"0005", "Agrupacion1",true,Enumerable.Empty<CriteriosagrupacionLinModel>(),
               "0006", "Agrupacionmod",true,Enumerable.Empty<CriteriosagrupacionLinModel>(), false },
        };

        public static IEnumerable<object[]> EliminarCriteriosagrupacion => new[]
        {
            new object[]
            {"0007", "Agrupacion1",true,Enumerable.Empty<CriteriosagrupacionLinModel>(), "0007", true },
            new object[]
            {"0008", "Agrupacion1",true,Enumerable.Empty<CriteriosagrupacionLinModel>(), "0009", false },
        };


        public Criteriosagrupacion(BaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory, MemberData("NuevasCriteriosagrupacion")]
        public void Alta(string codigo,string descripcion,bool ordenaralbaranes, IEnumerable<CriteriosagrupacionLinModel> lineas, bool result)
        {

            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {


                using (var service = FService.Instance.GetService(typeof (CriteriosagrupacionModel), _fixture.Context, db))
                {
                    var obj = new CriteriosagrupacionModel()
                    {
                        Id=codigo,
                        Nombre= descripcion,
                        Ordenaralbaranes = ordenaralbaranes,
                        Lineas = lineas.ToList()
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
                    Assert.True((db.Criteriosagrupacion.SingleOrDefault(f => f.id == codigo) != null) == result);
                }

            }
        }

        [Theory, MemberData("EditarCriteriosagrupacion")]
        public void Editar(string codigo, string descripcion, bool ordenaralbaranes, IEnumerable<CriteriosagrupacionLinModel> lineas,
            string codigo2, string descripcion2, bool ordenaralbaranes2, IEnumerable<CriteriosagrupacionLinModel> lineas2,
            bool result)
        {
            var objOrig = new CriteriosagrupacionModel()
            {
                Id = codigo,
                Nombre = descripcion,
                Ordenaralbaranes = ordenaralbaranes,
                Lineas = lineas.ToList()
            };

            var objUpdate = new CriteriosagrupacionModel()
            {
                Id = codigo2,
                Nombre = descripcion2,
                Ordenaralbaranes = ordenaralbaranes2,
                Lineas = lineas2.ToList()
            };


            try
            {
                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (var service = FService.Instance.GetService(typeof (CriteriosagrupacionModel), _fixture.Context, db))
                    {
                        service.create(objOrig);
                    }

                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (
                        var service =
                            FService.Instance.GetService(typeof (CriteriosagrupacionModel), _fixture.Context, db))
                    {
                        service.edit(objUpdate);
                    }
                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (
                        var service =
                            FService.Instance.GetService(typeof (CriteriosagrupacionModel), _fixture.Context, db))
                    {
                        var fpBD = service.get(objUpdate.Id) as CriteriosagrupacionModel;
                        Assert.True((fpBD.Nombre == objUpdate.Nombre) == result);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.False(result);

            }
        }

        [Theory, MemberData("EliminarCriteriosagrupacion")]
        public void Eliminar(string codigo, string descripcion, bool ordenaralbaranes, IEnumerable<CriteriosagrupacionLinModel> lineas,string codigoborrar, bool result)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {


                using (var service = FService.Instance.GetService(typeof (CriteriosagrupacionModel), _fixture.Context, db))
                {
                    var obj = new CriteriosagrupacionModel()
                    {
                        Id = codigo,
                        Nombre = descripcion,
                        Ordenaralbaranes = ordenaralbaranes,
                        Lineas = lineas.ToList()
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
                }
            }

            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {
                using (var service = FService.Instance.GetService(typeof(CriteriosagrupacionModel), _fixture.Context, db))
                {
                    try
                    {
                        service.delete(service.get(codigoborrar));
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
                using (var service = FService.Instance.GetService(typeof(CriteriosagrupacionModel), _fixture.Context, db))
                {
                    try
                    {
                        Assert.True(!service.exists(codigoborrar));
                    }
                    catch (Exception ex)
                    {
                        Assert.False(result);
                        return;
                    }
                }
                
            }
        }
    }
}
