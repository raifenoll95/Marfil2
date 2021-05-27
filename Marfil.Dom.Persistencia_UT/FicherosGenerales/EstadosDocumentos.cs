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

    public class EstadosDocumentos : IClassFixture<BaseFixture>
    {
        private BaseFixture _fixture;

        public static IEnumerable<object[]> NuevasEstados => new[]
        {
            new object[]
            {DocumentoEstado.AlbaranesCompras, "NEW","Nuevo",true,TipoEstado.Diseño,TipoMovimiento.Automatico, true },
            new object[]
            {DocumentoEstado.AlbaranesCompras, "NEW","",true,TipoEstado.Diseño,TipoMovimiento.Automatico, false },
        };

        public static IEnumerable<object[]> EditarEstados => new[]
        {
             new object[]
            {DocumentoEstado.AlbaranesCompras, "NEE","Nuevo",true,TipoEstado.Diseño,TipoMovimiento.Automatico,
                DocumentoEstado.AlbaranesCompras, "NEE","Nuevo2",true,TipoEstado.Diseño,TipoMovimiento.Automatico, true },
            new object[]
            {DocumentoEstado.AlbaranesCompras, "NE3","Nuevo",true,TipoEstado.Diseño,TipoMovimiento.Automatico,
                DocumentoEstado.AlbaranesCompras, "NE4","JARL",true,TipoEstado.Diseño,TipoMovimiento.Automatico, false },
        };

        public static IEnumerable<object[]> EliminarEstados => new[]
        {
             new object[]
            {DocumentoEstado.AlbaranesCompras, "NE5","Nuevo",true,TipoEstado.Diseño,TipoMovimiento.Automatico,((int)DocumentoEstado.AlbaranesCompras) + "-NE5", true },
            new object[]
            {DocumentoEstado.AlbaranesCompras, "NE6","Nuevo",true,TipoEstado.Diseño,TipoMovimiento.Automatico,((int)DocumentoEstado.AlbaranesCompras) +"-NE7", false },
        };


        public EstadosDocumentos(BaseFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory, MemberData("NuevasEstados")]
        public void Alta(DocumentoEstado documento,string id,string descripcion,bool imputariesgo, TipoEstado tipoestado, TipoMovimiento tipomovimiento, bool result)
        {

            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {


                using (var service = FService.Instance.GetService(typeof (EstadosModel), _fixture.Context, db))
                {
                    var obj = new EstadosModel()
                    {
                     Documento = documento,
                     Id = id,
                     Descripcion=descripcion,
                     Imputariesgo = imputariesgo,
                     Tipoestado = tipoestado,
                     Tipomovimiento = tipomovimiento
                    
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
                    Assert.True((db.Estados.SingleOrDefault(f => f.id == id && f.documento==(int)documento) != null) == result);
                }

            }
        }

        [Theory, MemberData("EditarEstados")]
        public void Editar(DocumentoEstado documento, string id, string descripcion, bool imputariesgo, TipoEstado tipoestado, TipoMovimiento tipomovimiento,
            DocumentoEstado documento2, string id2, string descripcion2, bool imputariesgo2, TipoEstado tipoestado2, TipoMovimiento tipomovimiento2, 
            bool result)
        {
            var objOrig = new EstadosModel()
            {
                Documento = documento,
                Id = id,
                Descripcion = descripcion,
                Imputariesgo = imputariesgo,
                Tipoestado = tipoestado,
                Tipomovimiento = tipomovimiento

            };

            var objUpdate = new EstadosModel()
            {
                Documento = documento2,
                Id = id2,
                Descripcion = descripcion2,
                Imputariesgo = imputariesgo2,
                Tipoestado = tipoestado2,
                Tipomovimiento = tipomovimiento2

            };


            try
            {
                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (var service = FService.Instance.GetService(typeof (EstadosModel), _fixture.Context, db))
                    {
                        service.create(objOrig);
                    }

                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (
                        var service =
                            FService.Instance.GetService(typeof (EstadosModel), _fixture.Context, db))
                    {
                        service.edit(objUpdate);
                    }
                }

                using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
                {
                    using (
                        var service =
                            FService.Instance.GetService(typeof (EstadosModel), _fixture.Context, db))
                    {
                        
                        var fpBD = service.get(objUpdate.CampoId) as EstadosModel;
                        Assert.True((fpBD.Descripcion == objUpdate.Descripcion) == result);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.False(result);

            }
        }

        [Theory, MemberData("EliminarEstados")]
        public void Eliminar(DocumentoEstado documento, string id, string descripcion, bool imputariesgo, TipoEstado tipoestado, TipoMovimiento tipomovimiento,string codigoborrar, bool result)
        {
            using (var db = MarfilEntities.ConnectToSqlServer(_fixture.DbName))
            {


                using (var service = FService.Instance.GetService(typeof (EstadosModel), _fixture.Context, db))
                {
                    var obj = new EstadosModel()
                    {
                        Documento = documento,
                        Id = id,
                        Descripcion = descripcion,
                        Imputariesgo = imputariesgo,
                        Tipoestado = tipoestado,
                        Tipomovimiento = tipomovimiento

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
                using (var service = FService.Instance.GetService(typeof(EstadosModel), _fixture.Context, db))
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
                using (var service = FService.Instance.GetService(typeof (EstadosModel), _fixture.Context, db))
                {
                    Assert.True(!service.exists(codigoborrar));
                }
            }
        }
    }
}
