using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.ControlsUI.NifCif;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Facturas;
using Marfil.Dom.Persistencia.Model.Documentos.FacturasCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Inventarios;
using Marfil.Dom.Persistencia.Model.Documentos.Pedidos;
using Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.Documentos.PresupuestosCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Reservasstock;
using Marfil.Dom.Persistencia.Model.Documentos.Transformaciones;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes;
using Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Graficaslistados;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.Model.Terceros;

//contabilidad
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.Model.Contabilidad.Maes;


using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Inf.Genericos.Helper;
using RTarifas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Tarifas;
using RCuentas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Cuentas;
using RFormasPago = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Formaspago;
using RSeries = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Series;
using RSeriesContables = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.SeriesContables;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.Model.Documentos.DivisionLotes;
using Marfil.Dom.Persistencia.Model.Documentos.GrupoMateriales;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Configuracion.Inmueble;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotesnave;
using Marfil.Dom.Persistencia.Model.Documentos.RegularizacionExistencias;

namespace Marfil.Dom.Persistencia.Model
{
    public class FModel
    {

        public T GetModel<T>(IContextService contextService, MarfilEntities db = null) where T : class
        {

            var context = contextService ?? new MockupContextService();

            var appService = new ApplicationHelper(context);

            if (typeof(SeccionesanaliticasModel) == typeof(T))
            {
                var result = new SeccionesanaliticasModel(context);
                var empresa = appService.GetCurrentEmpresa();

                result.Empresa = empresa.Id;
                return result as T;
            }

            //if (typeof(EmpresaModel) == typeof(T))
            //{
            //    var result = new EmpresaModel(context);
            //    var empresa = appService.GetCurrentEmpresa();

            //    result.Id = empresa.Id;
            //    return result as T;

            //}

            if (typeof (TrabajosModel) == typeof (T))
            {
                var result = new TrabajosModel(context);
                var empresa = appService.GetCurrentEmpresa();

                result.Empresa = empresa.Id;
                return result as T;

            }
            if (typeof(TareasModel) == typeof(T))
            {
                var result = new TareasModel(context);
                var empresa = appService.GetCurrentEmpresa();

                result.Empresa = empresa.Id;
                return result as T;

            }
            if (typeof(TransformacionesModel) == typeof(T))
            {
                var result = new TransformacionesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();
                var estadosService =
                   FService.Instance.GetService(typeof(EstadosModel), context) as EstadosService;
                var listestados = estadosService.GetStates(DocumentoEstado.Transformacioneslotes);
                var estado = listestados.First(f => f.Tipoestado == Configuracion.TipoEstado.Curso);
                result.Empresa = empresa.Id;
                result.Fkalmacen = almacen.Id;
                result.Fechadocumento = DateTime.Now;
                result.Fkejercicio = Funciones.Qint(context?.Ejercicio).Value;
                result.Fkestados = estado.CampoId;
                result.Materialsalidaigualentrada = appService.GetConfiguracion().Materialentradaigualsalida;

                return result as T;
            }
            if (typeof(TransformacioneslotesModel) == typeof(T))
            {
                var result = new TransformacioneslotesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();
                var estadosService =
                    FService.Instance.GetService(typeof (EstadosModel), context) as EstadosService;
                var listestados =estadosService.GetStates(DocumentoEstado.Transformacioneslotes);
                var estado = listestados.First(f => f.Tipoestado == Configuracion.TipoEstado.Curso);
                result.Empresa = empresa.Id;
                result.Fkalmacen = almacen.Id;
                result.Fechadocumento = DateTime.Now;
                result.Fkejercicio = Funciones.Qint(context?.Ejercicio).Value;
                result.Fkestados = estado.CampoId;
               
                return result as T;
            }
            if (typeof(TransformacioneslotesnaveModel) == typeof(T))
            {
                var result = new TransformacioneslotesnaveModel(context);
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();
                var estadosService =
                    FService.Instance.GetService(typeof(EstadosModel), context) as EstadosService;
                var listestados = estadosService.GetStates(DocumentoEstado.Transformacioneslotes);
                var estado = listestados.First(f => f.Tipoestado == Configuracion.TipoEstado.Curso);
                result.Empresa = empresa.Id;
                result.Fkalmacen = almacen.Id;
                result.Fechadocumento = DateTime.Now;
                result.Fkejercicio = Funciones.Qint(context?.Ejercicio).Value;
                result.Fkestados = estado.CampoId;

                return result as T;
            }
            if (typeof(DivisionLotesModel) == typeof(T))
            {
                var result = new DivisionLotesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();
                var estadosService =
                    FService.Instance.GetService(typeof(EstadosModel), context) as EstadosService;
                var listestados = estadosService.GetStates(DocumentoEstado.DivisionesLotes);
                var estado = listestados.First(f => f.Tipoestado == Configuracion.TipoEstado.Curso);
                result.Empresa = empresa.Id;
                result.Fkalmacen = almacen.Id;
                result.Fechadocumento = DateTime.Now;
                result.Fkejercicio = Funciones.Qint(context?.Ejercicio).Value;
                result.Fkestados = estado.CampoId;

                return result as T;
            }
            if (typeof(ImputacionCostesModel) == typeof(T))
            {
                var result = new ImputacionCostesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                var estadosService =
                    FService.Instance.GetService(typeof(EstadosModel), context) as EstadosService;
                var listestados = estadosService.GetStates(DocumentoEstado.ImputacionCostes);
                var estado = listestados.First(f => f.Tipoestado == Configuracion.TipoEstado.Curso);
                result.Empresa = empresa.Id;
                result.Fechadocumento = DateTime.Now;
                result.Fkejercicio = Funciones.Qint(context?.Ejercicio).Value;
                result.Fkestados = estado.CampoId;

                return result as T;
            }
            else if (typeof(CriteriosagrupacionModel) == typeof(T))
            {
                var result = new CriteriosagrupacionModel(context);
                return result as T;
            }
            else if (typeof(ConfiguraciongraficasModel) == typeof(T))
            {
                var result = new ConfiguraciongraficasModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;


                return result as T;
            }
            else if (typeof(AsistenteFacturacionModel) == typeof(T))
            {
                var result = new AsistenteFacturacionModel();
                result.Fechafactura = DateTime.Now;
                var empresa = appService.GetCurrentEmpresa();
                var configuracion = appService.GetConfiguracion();
                result.Fkcriterioagrupacion = configuracion.Fkcriterioagrupacion;
                using (var service = FService.Instance.GetService(typeof(CriteriosagrupacionModel), context))
                {
                    result.Criteriosagrupacion = service.getAll().Select(f => (CriteriosagrupacionModel)f);
                }

                return result as T;
            }
            else if (typeof(AsistenteRegularizacionExistenciasModel) == typeof(T))
            {
                var result = new AsistenteRegularizacionExistenciasModel();
                result.Fecharegularizacion = new DateTime(DateTime.Now.Year, 12, 31);

                using (var service = FService.Instance.GetService(typeof(ConfiguracionModel), context) as ConfiguracionService)
                {
                    result.ComentarioExistenciasIniciales = service.GetComentarioIni();
                    result.ComentarioExistenciasFinales = service.GetComentarioFin();
                }

                return result as T;
            }
            else if (typeof(IncidenciasModel) == typeof(T))
            {
                var result = new IncidenciasModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Tipomaterial = TipoMaterial.Material;
                result.Documento = TipoDocumentoMaterial.Todos;
                return result as T;
            }
            else if (typeof(EstadosModel) == typeof(T))
            {
                var result = new EstadosModel(context);
                return result as T;
            }
            else if (typeof(KitModel) == typeof(T))
            {
                var result = new KitModel(context);
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();
                result.Empresa = empresa.Id;
                result.Fkalmacen = almacen.Id;
                result.Fechadocumento = DateTime.Now;
                return result as T;
            }
            else if (typeof(InventariosModel) == typeof(T))
            {
                var result = new InventariosModel(context);
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();

                result.Empresa = empresa.Id;
                result.Fechadocumento = DateTime.Now;
                result.Fkalmacenes = almacen.Id;
                result.Fkejercicio = Funciones.Qint(context?.Ejercicio).Value;
                return result as T;
            }
            else if (typeof(BundleModel) == typeof(T))
            {
                var result = new BundleModel(context);
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();
                result.Empresa = empresa.Id;
                result.Fecha = DateTime.Now;
                result.Fkalmacen = almacen.Id;
                return result as T;
            }
            else if (typeof(DiarioStockModel) == typeof(T))
            {
                var result = new DiarioStockModel();
                var ejercicio = appService.GetCurrentEjercicio();
                result.FechaDesde = ejercicio.Desde;
                result.FechaHasta = ejercicio.Hasta;
                return result as T;
            }
            else if (typeof(StockActualModel) == typeof(T))
            {
                var result = new StockActualModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(AlmacenesModel) == typeof(T))
            {
                var result = new AlmacenesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Direcciones = new DireccionesModel() { Tipotercero = ApplicationHelper.ALMACENDIRECCIONINT };
                result.Empresa = empresa.Id;
                result.Lineas = new List<AlmacenesZonasModel>();
                return result as T;
            }
            else if (typeof(EjerciciosModel) == typeof(T))
            {
                var result = new EjerciciosModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Criterioiva = (int)empresa.Criterioiva;
                return result as T;
            }
            else if (typeof(PresupuestosModel) == typeof(T))
            {
                var result = new PresupuestosModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Fechavalidez = DateTime.Now.AddMonths(1);
                result.Fkestados = configuracion?.Estadoinicial ?? string.Empty;
                return result as T;
            }
            else if (typeof(PresupuestosComprasModel) == typeof(T))
            {
                var result = new PresupuestosComprasModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();

                result.Empresa = empresa.Id;
                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Fechavalidez = DateTime.Now.AddMonths(1);
                result.Fkestados = configuracion?.Estadopresupuestoscomprasinicial ?? string.Empty;
                return result as T;
            }
            else if (typeof(PedidosComprasModel) == typeof(T))
            {
                var result = new PedidosComprasModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();

                result.Empresa = empresa.Id;

                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Fechavalidez = DateTime.Now.AddMonths(1);
                result.Fkestados = configuracion?.Estadopedidoscomprasinicial ?? string.Empty;
                return result as T;
            }
            else if (typeof(PedidosModel) == typeof(T))
            {

                var result = new PedidosModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Fechavalidez = DateTime.Now.AddMonths(1);
                result.Fkestados = configuracion?.Estadopedidosventasinicial ?? string.Empty;

                //Comprobacion usuario - cliente
                if (db == null)
                    db = MarfilEntities.ConnectToSqlServer(context.BaseDatos);

                var user = db.Usuarios.Where(f => f.usuario == context.Usuario).SingleOrDefault();
                
                result.Usuariocliente = user.usuario_cliente;

                if(result.Usuariocliente.HasValue && result.Usuariocliente == true)
                {
                    result.Fkclientes = user.codigoclienteusuario;
                }

                return result as T;
            }
            else if (typeof(AlbaranesModel) == typeof(T))
            {
                var result = new AlbaranesModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();
                result.Empresa = empresa.Id;

                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Fechavalidez = DateTime.Now.AddMonths(1);
                result.Fkestados = configuracion?.Estadoalbaranesventasinicial ?? string.Empty;
                result.Tipoalbaran = (int)TipoAlbaran.Habitual;
                result.Fkcriteriosagrupacion = configuracion.Fkcriterioagrupacion;
                result.Fkalmacen = almacen.Id;
                result.Integridadreferencial = Guid.NewGuid();
                using (var service = FService.Instance.GetService(typeof(CriteriosagrupacionModel), context))
                {
                    result.Criteriosagrupacionlist = service.getAll().Select(f => (CriteriosagrupacionModel)f);
                }

                return result as T;
            }
            else if (typeof(RecepcionesStockModel) == typeof(T))
            {
                var result = new RecepcionesStockModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();
                result.Empresa = empresa.Id;

                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Fechavalidez = DateTime.Now.AddMonths(1);
                result.Fkestados = configuracion?.Estadoalbaranescomprasinicial ?? string.Empty;
                result.Tipoalbaran = (int)TipoAlbaran.Habitual;
                result.Fkcriteriosagrupacion = configuracion.Fkcriterioagrupacion;
                result.Fkalmacen = almacen.Id;
                result.Integridadreferencialflag = Guid.NewGuid();
                using (var service = FService.Instance.GetService(typeof(CriteriosagrupacionModel), context))
                {
                    result.Criteriosagrupacionlist = service.getAll().Select(f => (CriteriosagrupacionModel)f);
                }

                return result as T;
            }
            else if (typeof(EntregasStockModel) == typeof(T))
            {
                var result = new EntregasStockModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();

                result.Empresa = empresa.Id;

                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Fechavalidez = DateTime.Now.AddMonths(1);
                result.Fkestados = configuracion?.Estadoalbaranesventasinicial ?? string.Empty;
                result.Tipoalbaran = (int)TipoAlbaran.Habitual;
                result.Fkcriteriosagrupacion = configuracion.Fkcriterioagrupacion;
                result.Fkalmacen = almacen.Id;
                result.Integridadreferencial = Guid.NewGuid();
                using (var service = FService.Instance.GetService(typeof(CriteriosagrupacionModel), context))
                {
                    result.Criteriosagrupacionlist = service.getAll().Select(f => (CriteriosagrupacionModel)f);
                }

                return result as T;
            }
            else if (typeof(ReservasstockModel) == typeof(T))
            {
                var result = new ReservasstockModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();
                result.Empresa = empresa.Id;

                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Fechavalidez = DateTime.Now.AddMonths(1);
                result.Fkestados = configuracion?.Estadoreservasinicial ?? string.Empty;
                result.Fkalmacen = almacen.Id;
                result.Integridadreferencial = Guid.NewGuid();

                return result as T;
            }
            else if (typeof(FacturasModel) == typeof(T))
            {
                var result = new FacturasModel(context);
                var empresa = appService.GetCurrentEmpresa();
                var configuracion = appService.GetConfiguracion();
                var ejercicio = appService.GetCurrentEjercicio();
                result.Empresa = empresa.Id;
                result.Fkalmacen = appService.GetCurrentAlmacen().Id;
                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Comisiondescontardescuentocomercial = configuracion.Descontardescuentocomercial;
                result.Comsiondescontardescuentoprontopago = configuracion.Descontardescuentoprontopago;
                result.Comisiondescontarrecargofinancieroformapago = configuracion.Descontarrecargofinanciero;
                result.Fksituacioncomision = configuracion.Fksituacioncomisioncrear;
                result.Fkestados = configuracion?.Estadofacturasventasinicial ?? string.Empty;
                result.Criterioiva = (Model.Terceros.CriterioIVA)empresa.Criterioiva;

                return result as T;
            }
            else if (typeof(FacturasComprasModel) == typeof(T))
            {
                var result = new FacturasComprasModel(context);
                var empresa = appService.GetCurrentEmpresa();
                var configuracion = appService.GetConfiguracion();
                result.Empresa = empresa.Id;

                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Comisiondescontardescuentocomercial = configuracion.Descontardescuentocomercial;
                result.Comsiondescontardescuentoprontopago = configuracion.Descontardescuentoprontopago;
                result.Comisiondescontarrecargofinancieroformapago = configuracion.Descontarrecargofinanciero;
                result.Fksituacioncomision = configuracion.Fksituacioncomisioncrear;
                result.Fkestados = configuracion?.Estadofacturascomprasinicial ?? string.Empty;

                return result as T;
            }
            else if (typeof(AlbaranesComprasModel) == typeof(T))
            {
                var result = new AlbaranesComprasModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();                
                var almacen = appService.GetCurrentAlmacen();
                result.Empresa = empresa.Id;                

                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Fechavalidez = DateTime.Now.AddMonths(1);
                result.Fkestados = configuracion?.Estadoalbaranescomprasinicial ?? string.Empty;
                result.Tipoalbaran = (int)TipoAlbaran.Habitual;
                result.Fkcriteriosagrupacion = configuracion.Fkcriterioagrupacion;
                result.Integridadreferencialflag = Guid.NewGuid();
                result.Fkalmacen = almacen.Id;                

                using (var service = FService.Instance.GetService(typeof(CriteriosagrupacionModel), context))

                {
                    result.Criteriosagrupacionlist = service.getAll().Select(f => (CriteriosagrupacionModel)f);
                }

                return result as T;
            }

            else if (typeof(TraspasosalmacenModel) == typeof(T))
            {
                var result = new TraspasosalmacenModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                var almacen = appService.GetCurrentAlmacen();
                result.Empresa = empresa.Id;

                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fechadocumento = DateTime.Now;
                result.Fechavalidez = DateTime.Now.AddMonths(1);
                result.Fkestados = configuracion?.Estadotraspasosalmaceninicial ?? string.Empty;
                result.Integridadreferenciaflag = Guid.NewGuid();
                result.Fkalmacen = almacen.Id;



                return result as T;
            }
            else if (typeof(TarifasModel) == typeof(T))
            {
                var result = new TarifasModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Fkmonedas = Funciones.Qint(empresa.FkMonedaBase);
                result.BloqueoModel = new BloqueoEntidadModel() { Entidad = RTarifas.TituloEntidad };

                return result as T;
            }
            else if (typeof(PreciosEspecialesModel) == typeof(T))
            {
                var result = new PreciosEspecialesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;

                return result as T;
            }
            else if (typeof(SeriesModel) == typeof(T))
            {
                var result = new SeriesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Bloqueo = new BloqueoEntidadModel() { Entidad = RSeries.TituloEntidad };
                result.Fkmonedas = Funciones.Qint(empresa.FkMonedaBase);
                return result as T;
            }
            else if (typeof(SeriesContablesModel) == typeof(T))
            {
                var result = new SeriesContablesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Bloqueo = new BloqueoEntidadModel() { Entidad = RSeriesContables.TituloEntidad };
                result.Fkmonedas = Funciones.Qint(empresa.FkMonedaBase);
                return result as T;
            }


            else if (typeof(ContadoresModel) == typeof(T))
            {
                var result = new ContadoresModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                result.Primerdocumento = 1;
                return result as T;
            }

            else if (typeof(ContadoresLotesModel) == typeof(T))
            {
                var result = new ContadoresLotesModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                result.Primerdocumento = 1;
                return result as T;
            }
         
            else if (typeof(ArticulosModel) == typeof(T))
            {
                var result = new ArticulosModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                result.TarifasSistemaVenta = appService.GetTarifasSistema(TipoFlujo.Venta).ToList();
                result.TarifasSistemaCompra = appService.GetTarifasSistema(TipoFlujo.Compra).ToList();
                result.Tipogestionlotes = Tipogestionlotes.Singestion;

                return result as T;
            }
            else if (typeof(CaracteristicasModel) == typeof(T))
            {
                var result = new CaracteristicasModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                return result as T;
            }
            else if (typeof(MaterialesModel) == typeof(T))
            {
                var result = new MaterialesModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                return result as T;
            }
            else if (typeof(AcabadosModel) == typeof(T))
            {
                var result = new AcabadosModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                return result as T;
            }
            else if (typeof(GrosoresModel) == typeof(T))
            {
                var result = new GrosoresModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                return result as T;
            }
            else if (typeof(FamiliasproductosModel) == typeof(T))
            {
                var result = new FamiliasproductosModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;                
                result.Tipogestionlotes = Tipogestionlotes.Singestion;
                return result as T;
            }
            else if (typeof(BancosMandatosLinModel) == typeof(T))
            {
                var result = new BancosMandatosLinModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                result.Fkpaises = appService.GetPaisesDefecto();
                return result as T;
            }
            else if (typeof(ConfiguracionModel) == typeof(T))
            {
                var result = new ConfiguracionModel(context);
                result.Id = ConfiguracionService.Id;
                return result as T;
            }
            else if (typeof(BancosMandatosModel) == typeof(T))
            {
                var result = new BancosMandatosModel();
                result.Empresa = appService.GetCurrentEmpresa().Id;
                result.BancosMandatos = new List<BancosMandatosLinModel>();
                return result as T;
            }
            else if (typeof(TiposRetencionesModel) == typeof(T))
            {
                var result = new TiposRetencionesModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                return result as T;
            }
            else if (typeof(MonedasModel) == typeof(T))
            {
                var result = new MonedasModel(context);
                result.Activado = true;
                return result as T;

            }
            else if (typeof(GuiascontablesModel) == typeof(T))
            {
                var result = new GuiascontablesModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                return result as T;

            }
            else if (typeof(FormasPagoModel) == typeof(T))
            {
                var result = new FormasPagoModel(context);
                result.BloqueoModel = new BloqueoEntidadModel() { Entidad = RFormasPago.TituloEntidad };
                return result as T;

            }
            else if (typeof(EmpresaModel) == typeof(T))
            {
                var result = new EmpresaModel(context);
                if (db == null)
                    db = MarfilEntities.ConnectToSqlServer(context.BaseDatos);
                var service = new TablasVariasService(context, db);
                var servicePlanes = new PlanesGeneralesService(context, db);
                result.Paises = service.GetListPaises();
                result.PlanesGenerales = servicePlanes.getAll().Select(f => f as PlanesGeneralesModel).OrderByDescending(f => f.Defecto);
                result.Fkpais = appService.GetPaisesDefecto();
                result.Nif = new NifCifModel();
                result.NifCifAdministrador = new NifCifModel();
                result.DigitosCuentas = "8";
                result.NivelCuentas = "4";
                result.Direcciones = new DireccionesModel();
                result.Direcciones.Id = Guid.NewGuid();
                result.Direcciones.Direcciones = new List<DireccionesLinModel>();
                return result as T;
            }
            else if (typeof(CuentasModel) == typeof(T))
            {
                var result = new CuentasModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                result.FkPais = appService.GetPaisesDefecto();
                result.Nif = new NifCifModel();
                result.BloqueoModel = new BloqueoEntidadModel() { Entidad = RCuentas.TituloEntidad };
                return result as T;
            }
            else if (typeof(MunicipiosModel) == typeof(T))
            {
                var result = new MunicipiosModel(context);                
                result.Codigopais = appService.GetPaisesDefecto();
                return result as T;
            }
            else if (typeof(ProvinciasModel) == typeof(T))
            {
                var result = new ProvinciasModel(context);
                result.Codigopais = appService.GetPaisesDefecto();
                return result as T;
            }
            else if (typeof(PuertosModel) == typeof(T))
            {
                var result = new PuertosModel(context);
                result.Fkpaises = appService.GetPaisesDefecto();
                return result as T;
            }
            else if (typeof(TiposIvaModel) == typeof(T))
            {
                var result = new TiposIvaModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                return result as T;
            }
            else if (typeof(GruposIvaModel) == typeof(T))
            {
                var result = new GruposIvaModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                return result as T;
            }
            else if (typeof(RegimenIvaModel) == typeof(T))
            {
                var result = new RegimenIvaModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                result.SoportableRepercutidoAmbos = Sra.Ambos;
                return result as T;
            }
            else if (typeof(ObrasModel) == typeof(T))
            {
                var result = new ObrasModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;

                return result as T;
            }
            else if (typeof(HeaderModel) == typeof(T))
            {
                var result = new HeaderModel();

                var empresaobj = appService.GetCurrentEmpresa();
                var ejercicioobj = appService.GetCurrentEjercicio();
                var almacenobj = appService.GetCurrentAlmacen();

                result.Empresa = empresaobj?.Nombre;
                result.EmpresaId = empresaobj?.Id;
                result.EmpresasList = appService.GetEmpresas();

                result.Ejercicio = ejercicioobj?.Descripcioncorta;
                result.EjercicioId = ejercicioobj?.Id.ToString();
                result.EjerciciosList = appService.GetEjercicios();

                result.Almacen = almacenobj?.Descripcion;
                result.AlmacenId = almacenobj?.Id;
                result.AlmacenesList = appService.GetAlmacenes();

                result.Azureblob = context.Azureblob;

                return result as T;
            }
            else if (typeof(UnidadesModel) == typeof(T))
            {
                var result = new UnidadesModel(context);
                result.Decimalestotales = 3;
                return result as T;
            }
            else if (typeof(ContactosModel) == typeof(T))
            {
                var result = new ContactosModel();
                result.Empresa = appService.GetCurrentEmpresa().Id;
                result.Id = Guid.NewGuid();
                result.Contactos = new List<ContactosLinModel>();
                result.CargosEmpresa = appService.GetListCargosEmpresa();
                result.Direcciones = GetModel<DireccionesModel>(context);
                return result as T;
            }
            else if (typeof(ContactosLinModel) == typeof(T))
            {
                var result = new ContactosLinModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;

                using (var service = new TablasVariasService(context, MarfilEntities.ConnectToSqlServer(context.BaseDatos)))
                {
                    result.IdiomasList = service.GetTablasVariasByCode(1100).Lineas.Select(f => (TablasVariasIdiomasAplicacion)f); //idiomas
                    result.CargosEmpresaList = service.GetTablasVariasByCode(2050).Lineas.Select(f => (TablasVariasCargosEmpresaModel)f); //idiomas
                    result.TiposContactoList = service.GetTablasVariasByCode(2040).Lineas.Select(f => (TablasVariasGeneralModel)f); //idiomas
                }

                return result as T;
            }
            else if (typeof(DireccionesModel) == typeof(T))
            {
                var result = new DireccionesModel();
                //result.Empresa = appService.GetCurrentEmpresa()?.Id;
                result.Id = Guid.NewGuid();
                result.Direcciones = new List<DireccionesLinModel>();
                return result as T;
            }
            else if (typeof(DireccionesLinModel) == typeof(T))
            {

                var result = new DireccionesLinModel(context);
                result.Empresa = appService.GetCurrentEmpresa()?.Id;
                return result as T;
            }
            else if (typeof(TiposCuentasModel) == typeof(T))
            {

                var result = new TiposCuentasModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Categoria = CategoriasCuentas.Contables;
                return result as T;
            }
            else if (typeof(InmueblesModel) == typeof(T))
            {

                var result = new InmueblesModel();
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(CuadernosBancariosModel) == typeof(T))
            {

                var result = new CuadernosBancariosModel();
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(CuadernosBancariosLinModel) == typeof(T))
            {

                var result = new CuadernosBancariosLinModel();
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(MapeoRemesasModel) == typeof(T))
            {

                var result = new MapeoRemesasModel();
                return result as T;
            }
            else if (typeof(AgentesModel) == typeof(T))
            {
                var result = new AgentesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Fkregimeniva = empresa.Fkregimeniva;
                result.Cuentas = GetModel<CuentasModel>(context);
                result.Direcciones = GetModel<DireccionesModel>(context);
                result.Direcciones.Tipotercero = (int)TiposCuentas.Agentes;
                result.BancosMandatos = GetModel<BancosMandatosModel>(context);
                result.BancosMandatos.Tipo = TipoBancosMandatos.Banco;

                return result as T;
            }
            else if (typeof(ComercialesModel) == typeof(T))
            {
                var result = new ComercialesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Fkregimeniva = empresa.Fkregimeniva;
                result.Cuentas = GetModel<CuentasModel>(context);
                result.Direcciones = GetModel<DireccionesModel>(context);
                result.Direcciones.Tipotercero = (int)TiposCuentas.Comerciales;
                result.BancosMandatos = GetModel<BancosMandatosModel>(context);
                result.BancosMandatos.Tipo = TipoBancosMandatos.Banco;

                return result as T;
            }
            else if (typeof(AseguradorasModel) == typeof(T))
            {

                var result = new AseguradorasModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Cuentas = GetModel<CuentasModel>(context);
                result.Direcciones = GetModel<DireccionesModel>(context);
                result.Direcciones.Tipotercero = (int)TiposCuentas.Aseguradoras;
                result.Contactos = GetModel<ContactosModel>(context);
                result.Contactos.Tipotercero = (int)TiposCuentas.Aseguradoras;
                result.BancosMandatos = GetModel<BancosMandatosModel>(context);
                result.BancosMandatos.Tipo = TipoBancosMandatos.Banco;

                return result as T;
            }
            else if (typeof(ProveedoresModel) == typeof(T))
            {

                var result = new ProveedoresModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Fkregimeniva = empresa.Fkregimeniva;
                result.Fkmonedas = Funciones.Qint(appService.GetCurrentEmpresa().FkMonedaBase) ?? 0;
                result.Cuentas = GetModel<CuentasModel>(context);
                result.Direcciones = GetModel<DireccionesModel>(context);
                result.Direcciones.Tipotercero = (int)TiposCuentas.Proveedores;
                result.Contactos = GetModel<ContactosModel>(context);
                result.Contactos.Tipotercero = (int)TiposCuentas.Proveedores;
                result.BancosMandatos = GetModel<BancosMandatosModel>(context);
                result.BancosMandatos.Tipo = TipoBancosMandatos.Banco;

                result.LstTarifas = appService.GetTarifasCuenta(TipoFlujo.Compra, "", empresa.Id)
                                    .OrderBy(f => f.Tipotarifa)
                                    .Select(f => new SelectListItem() { Value = f.Id, Text = f.Descripcion });


                return result as T;
            }
            else if (typeof(AcreedoresModel) == typeof(T))
            {
                var result = new AcreedoresModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Fkregimeniva = empresa.Fkregimeniva;
                result.Fkmonedas = Funciones.Qint(appService.GetCurrentEmpresa().FkMonedaBase) ?? 0;
                result.Cuentas = GetModel<CuentasModel>(context);
                result.Direcciones = GetModel<DireccionesModel>(context);
                result.Direcciones.Tipotercero = (int)TiposCuentas.Acreedores;
                result.Contactos = GetModel<ContactosModel>(context);
                result.Contactos.Tipotercero = (int)TiposCuentas.Acreedores;
                result.BancosMandatos = GetModel<BancosMandatosModel>(context);
                result.BancosMandatos.Tipo = TipoBancosMandatos.Banco;

                result.LstTarifas = appService.GetTarifasCuenta(TipoFlujo.Compra, "", empresa.Id)
                    .OrderBy(f => f.Tipotarifa)
                    .Select(f => new SelectListItem() { Value = f.Id, Text = f.Descripcion });

                return result as T;
            }
            else if (typeof(CuentastesoreriaModel) == typeof(T))
            {
                var result = new CuentastesoreriaModel(context);
                result.Empresa = appService.GetCurrentEmpresa().Id;
                result.Cuentas = GetModel<CuentasModel>(context);
                result.Direcciones = GetModel<DireccionesModel>(context);
                result.Direcciones.Tipotercero = (int)TiposCuentas.Cuentastesoreria;
                result.Contactos = GetModel<ContactosModel>(context);
                result.Contactos.Tipotercero = (int)TiposCuentas.Cuentastesoreria;
                result.BancosMandatos = GetModel<BancosMandatosModel>(context);
                result.BancosMandatos.Tipo = TipoBancosMandatos.Banco;

                return result as T;
            }
            else if (typeof(TransportistasModel) == typeof(T))
            {
                var result = new TransportistasModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Fkregimeniva = empresa.Fkregimeniva;
                result.Fkmonedas = Funciones.Qint(empresa.FkMonedaBase) ?? 0;
                result.Cuentas = GetModel<CuentasModel>(context);
                result.Direcciones = GetModel<DireccionesModel>(context);
                result.Direcciones.Tipotercero = (int)TiposCuentas.Transportistas;
                result.Contactos = GetModel<ContactosModel>(context);
                result.Contactos.Tipotercero = (int)TiposCuentas.Transportistas;
                result.BancosMandatos = GetModel<BancosMandatosModel>(context);
                result.BancosMandatos.Tipo = TipoBancosMandatos.Banco;

                return result as T;
            }
            else if (typeof(OperariosModel) == typeof(T))
            {
                var result = new OperariosModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Cuentas = GetModel<CuentasModel>(context);
                result.Fechaalta = DateTime.Now;
                result.Direcciones = GetModel<DireccionesModel>(context);
                result.Direcciones.Tipotercero = (int)TiposCuentas.Operarios;
                result.BancosMandatos = GetModel<BancosMandatosModel>(context);
                result.BancosMandatos.Tipo = TipoBancosMandatos.Banco;

                return result as T;
            }
            else if (typeof(ProspectosModel) == typeof(T))
            {
                var empresa = appService.GetCurrentEmpresa();
                var result = new ProspectosModel(context);
                result.Empresa = empresa.Id;
                result.Fkmonedas = empresa.FkMonedaBase;
                result.Fkregimeniva = empresa.Fkregimeniva;
                result.Cuentas = GetModel<CuentasModel>(context);
                result.Direcciones = GetModel<DireccionesModel>(context);
                result.Direcciones.Tipotercero = (int)TiposCuentas.Clientes;
                result.Contactos = GetModel<ContactosModel>(context);
                result.Contactos.Tipotercero = (int)TiposCuentas.Clientes;

                result.LstTarifas = appService.GetTarifasCuenta(TipoFlujo.Venta, "", empresa.Id).OrderBy(f => f.Tipotarifa).Select(f => new SelectListItem() { Value = f.Id, Text = f.Descripcion });
                return result as T;
            }
            else if (typeof(ClientesModel) == typeof(T))
            {
                var empresa = appService.GetCurrentEmpresa();
                var result = new ClientesModel(context);
                result.Empresa = empresa.Id;
                result.Fkmonedas = Funciones.Qint(empresa.FkMonedaBase) ?? 0;
                result.Fkregimeniva = empresa.Fkregimeniva;
                result.Cuentas = GetModel<CuentasModel>(context);
                result.Direcciones = GetModel<DireccionesModel>(context);
                result.Direcciones.Tipotercero = (int)TiposCuentas.Clientes;
                result.Contactos = GetModel<ContactosModel>(context);
                result.Contactos.Tipotercero = (int)TiposCuentas.Clientes;
                result.BancosMandatos = GetModel<BancosMandatosModel>(context);
                result.BancosMandatos.Tipo = TipoBancosMandatos.Mandato;
                result.LstTarifas = appService.GetTarifasCuenta(TipoFlujo.Venta, "", empresa.Id).OrderBy(f => f.Tipotarifa).Select(f => new SelectListItem() { Value = f.Id, Text = f.Descripcion });
                return result as T;
            }
            else if (typeof(MovimientosstockModel) == typeof(T))
            {
                var empresa = appService.GetCurrentEmpresa();
                var result = new MovimientosstockModel();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(CambioUbicacionModel) == typeof(T))
            {
                var result = new CambioUbicacionModel();
                var almacen = appService.GetCurrentAlmacen();
                result.Fkalmacen = almacen.Id;
                return result as T;
            }
            else if (typeof(ActualizarCostesModel) == typeof(T))
            {
                var result = new ActualizarCostesModel();

                return result as T;
            }
            else if (typeof(MovsModel) == typeof(T))
            {
                var result = new MovsModel(context);
                result.Empresa = context.Empresa;
                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                result.Fecha = DateTime.Now;
                result.Integridadreferencial = Guid.NewGuid();

                //Daba error al importar, se ha movido al constructor de MovsModel
                //var ejercicio = appService.GetCurrentEjercicio();
                //if (!String.IsNullOrEmpty(ejercicio.Fkseriescontables))
                //{
                //    result.Fkseriescontables = ejercicio.Fkseriescontables;
                //}
                
                return result as T;
            }
            else if (typeof(MaesModel) == typeof(T))
            {
                var result = new MaesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Fkejercicio = Funciones.Qint(context.Ejercicio).Value;
                return result as T;
            }
            else if (typeof(OportunidadesModel) == typeof(T))
            {
                var result = new OportunidadesModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                result.Fechadocumento = DateTime.Now;
                result.Empresa = empresa.Id;
                result.Fketapa = configuracion?.Estadooportunidadesinicial ?? string.Empty;
                return result as T;
            }
            else if (typeof(SeguimientosModel) == typeof(T))
            {
                var result = new SeguimientosModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                result.Fechadocumento = DateTime.Now;
                result.Empresa = empresa.Id;
                result.Usuario = context.Usuario;                
                return result as T;
            }
            else if (typeof(SeguimientosCorreoModel) == typeof(T))
            {
                var result = new SeguimientosCorreoModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(ProyectosModel) == typeof(T))
            {
                var result = new ProyectosModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                result.Fechadocumento = DateTime.Now;
                result.Empresa = empresa.Id;
                result.Fketapa = configuracion?.Estadoproyectosinicial ?? string.Empty;
                return result as T;
            }
            else if (typeof(CampañasModel) == typeof(T))
            {
                var result = new CampañasModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                result.Fechadocumento = DateTime.Now;
                result.Empresa = empresa.Id;
                result.Fketapa = configuracion?.Estadocampañasinicial ?? string.Empty;
                return result as T;
            }
            else if (typeof(IncidenciasCRMModel) == typeof(T))
            {
                var result = new IncidenciasCRMModel(context);
                var configuracion = appService.GetConfiguracion();
                var empresa = appService.GetCurrentEmpresa();
                result.Fechadocumento = DateTime.Now;
                result.Empresa = empresa.Id;
                result.Fketapa = configuracion?.Estadoincidenciasinicial ?? string.Empty;
                return result as T;
            }
            else if (typeof(PeticionesAsincronasModel) == typeof(T))
            {
                var result = new PeticionesAsincronasModel(context);
                return result as T;
            }
            else if (typeof(CostesVariablesPeriodoModel) == typeof(T))
            {
                var result = new CostesVariablesPeriodoModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(DivisionLotesModel) == typeof(T))
            {
                var result = new DivisionLotesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(DivisionLotessalidaLinModel) == typeof(T))
            {
                var result = new DivisionLotessalidaLinModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(GrupoMaterialesModel) == typeof(T))
            {
                var result = new GrupoMaterialesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(ArticulosTerceroModel) == typeof(T))
            {
                var result = new ArticulosTerceroModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(ArticulosComponentesModel) == typeof(T))
            {
                var result = new ArticulosComponentesModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(ArticulosStockSeguridadModel) == typeof(T))
            {
                var result = new ArticulosStockSeguridadModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(LogStockSeguridadModel) == typeof(T))
            {
                var result = new LogStockSeguridadModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(VerificarContabilidadModel) == typeof(T))
            {
                var result = new VerificarContabilidadModel(context);

                return result as T;
            }
            else if (typeof(ConsultaVisualFullModel) == typeof(T))
            {
                var result = new ConsultaVisualFullModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(SituacionesTesoreriaModel) == typeof(T))
            {
                var result = new SituacionesTesoreriaModel(context);
                return result as T;
            }
            else if (typeof(VencimientosModel) == typeof(T))
            {
                var result = new VencimientosModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Usuario = context.Usuario;
                return result as T;
            }
            else if (typeof(CircuitoTesoreriaCobrosModel) == typeof(T))
            {
                var result = new CircuitoTesoreriaCobrosModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(CarteraVencimientosModel) == typeof(T))
            {
                var result = new CarteraVencimientosModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Usuario = context.Usuario;
                return result as T;
            }
            else if (typeof(RemesasModel) == typeof(T))
            {
                var result = new RemesasModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                result.Usuario = context.Usuario;
                return result as T;
            }
            else if (typeof(PrevisionesCarteraModel) == typeof(T))
            {
                var result = new PrevisionesCarteraModel(context);
                var empresa = appService.GetCurrentEmpresa();
                result.Empresa = empresa.Id;
                return result as T;
            }
            else if (typeof(GuiasBalancesModel) == typeof(T))
            {

                var result = new GuiasBalancesModel();
                return result as T;
            }
            else if (typeof(GuiasBalancesLineasModel) == typeof(T))
            {

                var result = new GuiasBalancesLineasModel();
                return result as T;
            }
            var ctor = typeof(T).GetConstructor(new[] { typeof(IContextService) });
            return ctor.Invoke(new object[] { context }) as T;

        }

    }
}
