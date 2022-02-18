using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;
using System.Transactions;
using System.Web;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Startup;
using Marfil.Inf.Genericos;
using System.Web.Hosting;
using System.Threading;
using Marfil.Inf.Genericos.Helper;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Contabilidad.Maes;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IEmpresasService
    {

    }

    public class EmpresasService : GestionService<EmpresaModel, Empresas>, IEmpresasService
    {
        #region CTR

        public EmpresasService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion


        #region Create

        public override void create(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {
                try
                {
                    var model = obj as EmpresaModel;

                    if (model.Id == "-1")
                        model.Id = NextId();

                    base.create(model);
                    GuardarDirecciones(obj as EmpresaModel);
                    CrearTarifasBase(model.Id);
                    CrearEjercicio(model.Id, model.EjercicioNuevo);
                    //RAI -- Plan general antes se hacía aquí, ahora lo he hecho en Segundo plano -> EmpresasController
                    //CrearPlanGeneral(model.Id, model.Fkplangeneralcontable, model.Fkpais);
                    CrearCarpetas(model.Id);
                    CrearDatosDefecto(model.Id);
                    tran.Complete();
                    _db.SaveChanges();

                }
                catch (Exception ex)
                {
                    throw;
                }

            }

        }
        
        #region Configuracion documentos

        private void CrearDatosDefecto(string id)
        {
            CrearContadores("~/App_Data/Otros/contadores.csv",id) ;
            CrearContadoresLotes("~/App_Data/Otros/contadoreslotes.csv", id);
            CrearSeries("~/App_Data/Otros/series.csv", id);
            CrearDatos<GruposIvaModel>("~/App_Data/Otros/gruposiva.csv", id);
            //CrearDatos<UnidadesModel>("~/App_Data/Otros/unidades.csv", id);
            CrearDatos<TiposIvaModel>("~/App_Data/Otros/tiposiva.csv", id);           
            CrearDatos<RegimenIvaModel>("~/App_Data/Otros/regimeniva.csv", id);
            CrearDatos<GuiascontablesModel>("~/App_Data/Otros/guiascontables.csv", id);
            CrearDatos<AlmacenesModel>("~/App_Data/Otros/almacen.csv", id);
        }
        //Estas funciones de crear datos deberiamos migrarlas a un servicio que nos generase los datos por defecto a partir de una lista de archivos y una empresa
        private void CrearDatos<T>(string ruta, string empresa) where T : class
        {
            
            var csvFile = _context.ServerMapPath(ruta);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CreateDatosModel<T>(contenido, empresa);
            }
        }

        private void CreateDatosModel<T>(string contenido, string empresa) where T : class
        {
            var lineas = contenido.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearDatosModel<T>(item, empresa);
            }
        }

        private void CrearDatosModel<T>(string linea, string empresa) where T : class
        {
            var archivo = _context.ServerMapPath(linea);
            var service = new Serializer<T>();
            var model = service.SetXml(File.ReadAllText(archivo)) as IModelView;
            if(model.getProperties().Any(f=>f.property.Name.ToLower()=="empresa"))
            {
                model.set("empresa", empresa);
            }

            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };
            var serviceModelo = FService.Instance.GetService(typeof(T), newContext, _db);
            serviceModelo.create(model);
        }

        #region Contadores documentos

        private void CrearContadores(string fichero,string empresa)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CreateContadorModel(contenido,empresa);
            }
        }

        

        private void CreateContadorModel(string contenido,string empresa)
        {
            var lineas = contenido.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearContadorModel(item, empresa);
            }
        }

        private void CrearContadorModel(string linea,string empresa)
        {
            var archivo = _context.ServerMapPath(Path.Combine("~/App_Data/", linea));
            var service = new Serializer<ContadorModel>();
            var model = service.SetXml(File.ReadAllText(archivo));
            model.Empresa = empresa;


            var serviceContador = FService.Instance.GetService(typeof (ContadoresModel), _context, _db);
            serviceContador.create(new ContadoresModel()
            {
                Empresa = model.Empresa,
                Id = model.Id,
                Descripcion = model.Descripcion,
                Lineas= model.Lineas,
               Primerdocumento = model.Primerdocumento,
               Tipocontador = model.Tipocontador,
               Tipoinicio = model.Tipoinicio
            });

        }

        #endregion

        #region Crear contadores de lotes

        private void CrearContadoresLotes(string fichero, string empresa)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CreateContadorLoteModel(contenido, empresa);
            }
        }

        private void CreateContadorLoteModel(string contenido, string empresa)
        {
            var lineas = contenido.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearContadorLoteModel(item, empresa);
            }
        }

        private void CrearContadorLoteModel(string linea, string empresa)
        {
            var archivo = _context.ServerMapPath(Path.Combine("~/App_Data/", linea));
            var service = new Serializer<ContadoresLotesModel>();
            var model = service.SetXml(File.ReadAllText(archivo));
            model.Empresa = empresa;


            var serviceContador = FService.Instance.GetService(typeof(ContadoresLotesModel), _context, _db);
            serviceContador.create(new ContadoresLotesModel()
            {
                Empresa = model.Empresa,
                Id = model.Id,
                Descripcion = model.Descripcion,
                Lineas = model.Lineas,
                Primerdocumento = model.Primerdocumento,
                Tipocontador = model.Tipocontador,
                Tipoinicio = model.Tipoinicio
            });

        }

        #endregion

        private void CrearSeries(string fichero, string empresa)
        {
            var csvFile = _context.ServerMapPath(fichero);
            using (var reader = new StreamReader(csvFile, Encoding.Default, true))
            {
                var contenido = reader.ReadToEnd();
                CreateSerieModel(contenido, empresa);
            }
        }

        private void CreateSerieModel(string contenido, string empresa)
        {
            var lineas = contenido.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearSerieModel(item, empresa);
            }
        }

        private void CrearSerieModel(string linea, string empresa)
        {
            var archivo = _context.ServerMapPath(Path.Combine("~/App_Data/", linea));
            var service = new Serializer<SeriesSerializableModel>();
            var model = service.SetXml(File.ReadAllText(archivo));
            model.Empresa = empresa;


            var serviceContador = FService.Instance.GetService(typeof(SeriesModel), _context, _db);
            serviceContador.create(new SeriesModel(model));

        }

        #endregion

        private void CrearCarpetas(string id)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = id,
                Id = _context.Id,
                RoleId = _context.RoleId
            };
            var serviceCarpetas = FService.Instance.GetService(typeof (CarpetasModel), newContext, _db);
            serviceCarpetas.create(new CarpetasModel()
            {
                Empresa = id,
                Id=Guid.Empty,
                Nombre = "Root",
                Ruta = "Root"
            });
        }

        private void EliminarCarpetas(string id)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = id,
                Id = _context.Id,
                RoleId = _context.RoleId
            };
            var serviceCarpetas = FService.Instance.GetService(typeof(CarpetasModel), newContext, _db);
            var list = serviceCarpetas.getAll().OfType<CarpetasModel>();

            foreach (var item in list.Where(f => f.Empresa == id))
            {

                serviceCarpetas.delete(item);
            }
        }

        private void CrearEjercicio(string empresa, EjerciciosModel ejercicioNuevo)
        {
            ejercicioNuevo.Empresa = empresa;
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };
            
            var service = new EjerciciosService(newContext, _db);
            service.create(ejercicioNuevo);
        }
        private void EliminarEjercicio(string empresa, IEnumerable<EjerciciosModel> ejercicios)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new EjerciciosService(newContext, _db);
            foreach (var item in ejercicios)
            {
                service.delete(item);
            }
            
        }

        public void CrearPlanGeneral(string empresa, string plangeneralid, string pais)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new PlanesGeneralesService(newContext, _db);
            var item = service.get(plangeneralid) as PlanesGeneralesModel;

            var cuentasStartup = new Startup.CuentasStartup(newContext, _db, empresa, pais);
            cuentasStartup.CrearDatos(item.Fichero);
        }

        private void CrearTarifasBase(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };
            var serviceTarifasDefecto = new TarifasbaseService(newContext, _db);
            var list = serviceTarifasDefecto.getAll().OfType<TarifasbaseModel>();

            var service = new TarifasService(newContext, _db);
            foreach (var item in list)
            {
                var newItem = new TarifasModel();
                newItem.Empresa = empresa;
                newItem.Id = item.Fktarifa;
                newItem.Descripcion = item.Descripcion;
                newItem.Tipoflujo = item.Tipoflujo;
                newItem.Tipotarifa = TipoTarifa.Sistema;
                service.create(newItem);
            }
        }

        private void EliminarTarifasBase(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new TarifasService(newContext, _db);
            var list = service.getAll().OfType<TarifasModel>().ToList();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {
                
                service.delete(item);
            }
        }


        #endregion

        #region Edit

        public override void edit(IModelView obj)
        {
            using (var tran = Marfil.Inf.Genericos.Helper.TransactionScopeBuilder.CreateTransactionObject())
            {

                base.edit(obj);
                GuardarDirecciones(obj as EmpresaModel);
                tran.Complete();
            }
        }

        #endregion

        #region delete

        public override void delete(IModelView obj)
        {
            using (var tran = TransactionScopeBuilder.CreateTransactionObject())
            {
                var model = obj as EmpresaModel;
                if (model.Id == "0001")
                {
                    throw new ValidationException("No se permite eliminar la empresa por defecto");
                }
                
                EliminarDirecciones(model);
                EliminarTarifasBase(model.Id);
                EliminarMaes(model.Id);
                EliminarClientes(model.Id);
                EliminarProveedores(model.Id);
                EliminarAcreedores(model.Id);
                EliminarCuentas(model.Id);
                EliminarEjercicio(model.Id, model.Ejercicios);
                EliminarCarpetas(model.Id);
                EliminarContadores(model.Id);
                EliminarContadoresLotes(model.Id);
                EliminarSeries(model.Id);
                EliminarSeriesContables(model.Id);
                EliminarGruposIva(model.Id);
                EliminarTiposIva(model.Id);
                EliminarRegimenIva(model.Id);
                EliminarGuiasContables(model.Id);
                EliminarGuiasBalances(model.Id);
                EliminarAlmacenes(model.Id);               
                EliminarDocumentos(model.Id);
                base.delete(obj);

                tran.Complete();
            }
        }

        private void EliminarAcreedores(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new AcreedoresService(newContext, _db);
            var list = service.getAll().OfType<AcreedoresModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarProveedores(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new ProveedoresService(newContext, _db);
            var list = service.getAll().OfType<ProveedoresModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarClientes(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new ClientesService(newContext, _db);
            var list = service.getAll().OfType<ClientesModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarMaes(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new MaesService(newContext, _db);
            var list = service.getAll().OfType<MaesModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarGuiasBalances(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new GuiasBalancesService(newContext, _db);
            var list = service.getAll().OfType<GuiasBalancesModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarDocumentos(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var servicePreferencias = new PreferenciasUsuarioService(_db);
            var documentos = _db.DocumentosUsuario.Where(f => f.empresa == empresa )
                .ToList();

            foreach (var item in documentos)
            {
                _db.DocumentosUsuario.Remove(item);
                _db.SaveChanges();
            }
        }

        private void EliminarCuentas(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new CuentasService(newContext, _db);
            var list = service.getAll().OfType<CuentasModel>().OrderByDescending(f => f.Id.Length);

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarSeriesContables(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new SeriesContablesService(newContext, _db);
            var list = service.getAll().OfType<SeriesContablesModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarContadores(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new ContadoresService(newContext, _db);
            var list = service.getAll().OfType<ContadoresModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarContadoresLotes(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new ContadoresLotesService(newContext, _db);
            var list = service.getAll().OfType<ContadoresLotesModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarSeries(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new SeriesService(newContext, _db);
            var list = service.getAll().OfType<SeriesModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarGruposIva(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new GruposIvaService(newContext, _db);
            var list = service.getAll().OfType<GruposIvaModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarTiposIva(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new TiposivaService(newContext, _db);
            var list = service.getAll().OfType<TiposIvaModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarRegimenIva(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new RegimenivaService(newContext, _db);
            var list = service.getAll().OfType<RegimenIvaModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarGuiasContables(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new GuiascontablesService(newContext, _db);
            var list = service.getAll().OfType<GuiascontablesModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        private void EliminarAlmacenes(string empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = _context.BaseDatos,
                Empresa = empresa,
                Id = _context.Id,
                RoleId = _context.RoleId
            };

            var service = new AlmacenesService(newContext, _db);
            var list = service.getAll().OfType<AlmacenesModel>();

            foreach (var item in list.Where(f => f.Empresa == empresa))
            {

                service.delete(item);
            }
        }

        #endregion

        #region ListIndex

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            var propiedadesVisibles = new[] { "Id", "Nombre", "Razonsocial", "Nifdescripcion" };
            var propiedades = Helpers.Helper.getProperties<EmpresaModel>();
            model.ExcludedColumns =
                propiedades.Where(f => !propiedadesVisibles.Any(j => j == f.property.Name)).Select(f => f.property.Name).ToList();

            return model;
        }

        public override string GetSelectPrincipal()
        {
            return "select id, nombre,razonsocial,nif as Nifdescripcion from Empresas ";
        }

        #endregion

        #region Helper Direcciones

        private void ProcessDirecciones(EmpresaModel model)
        {
            foreach (var item in model.Direcciones.Direcciones)
            {
                item.Empresa = model.Id;
                item.Fkentidad = model.Id;
                item.Tipotercero = -1;
                if (item.Id < 0)
                {
                    item.Id = GetNextId(model);
                }
            }
        }

        private int GetNextId(EmpresaModel model)
        {
            var result = 1;

            if (model.Direcciones != null && model.Direcciones.Direcciones.Any())
            {
                result = model.Direcciones.Direcciones.Max(f => f.Id) + 1;
            }

            return result;
        }

        private void GuardarDirecciones(EmpresaModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var auxContext = _context;
            auxContext.Empresa = model.Id;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), auxContext, _db) as DireccionesService;
            ProcessDirecciones(model);

            direccionesService.CleanAllDirecciones(model.Id, -1, model.Id);
            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.create(item);
            }
        }

        private void EditarDirecciones(EmpresaModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var auxContext = _context;
            auxContext.Empresa = model.Id;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), auxContext, _db) as DireccionesService;
            ProcessDirecciones(model);

            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.edit(item);
            }
        }
        private void EliminarDirecciones(EmpresaModel model)
        {
            if (model.Direcciones == null) return;
            var fservice = FService.Instance;
            var auxContext = _context;
            auxContext.Empresa = model.Id;
            var direccionesService = fservice.GetService(typeof(DireccionesLinModel), auxContext, _db) as DireccionesService;
            ProcessDirecciones(model);

            foreach (var item in model.Direcciones.Direcciones)
            {
                direccionesService.delete(item);
            }
        }

        #endregion

        #region Helper

        public string NextId()
        {
            if (_db.Empresas.Any())
            {
                return (int.Parse(_db.Empresas.Max(f => f.id)) + 1).ToString().PadLeft(4, '0');
            }
            return "0001";
        }

        #endregion


    }
}
