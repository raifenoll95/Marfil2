using System.Configuration;
using Microsoft.Practices.Unity;
using System.Web.Http;
using System.Web.Mvc;
using Marfil.App.WebMain.Services;
using Marfil.Dom.ControlsUI.Login;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Login;
using Marfil.Inf.Genericos.Helper;
using Unity.WebApi;
using Unity.Mvc5;
namespace Marfil.App.WebMain
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IContextService, ContextService>();
            if (Funciones.Qbool(ConfigurationManager.AppSettings["Modopruebaslogin"]))
            {
                container.RegisterType<ILoginService, LoginServiceMock>();
            }
            else
            {
                container.RegisterType<ILoginService, LoginService>();
            }

            container.RegisterType<IMovimientosAlmacen, MovimientosalmacenService>();
            container.RegisterType<ILoginViewService, LoginViewServiceFichero>();
            container.RegisterType<ILotesService, LotesService>();
            container.RegisterType<IAcabadosService, AcabadosService>();
            container.RegisterType<IAcreedoresService, AcreedoresService>();
            container.RegisterType<IAgentesService, AgentesService>();
            container.RegisterType<IAlbaranesComprasService, AlbaranesComprasService>();
            container.RegisterType<IAlbaranesService, AlbaranesService>();
            container.RegisterType<IAlmacenesService, AlmacenesService>();
            container.RegisterType<IArticulosService, ArticulosService>();
            container.RegisterType<IAseguradorasService, AseguradorasService>();
            container.RegisterType<IBancosMandatosService, BancosMandatosService>();
            container.RegisterType<IBancosService, BancosService>();
            container.RegisterType<ICaracteristicasService, CaracteristicasService>();
            container.RegisterType<ICarpetasService, CarpetasService>();
            container.RegisterType<IClientesService, ClientesService>();
            container.RegisterType<IComercialesService, ComercialesService>();
            container.RegisterType<IConfiguraciongraficasService, ConfiguraciongraficasService>();
            container.RegisterType<IConfiguracionService, ConfiguracionService>();
            container.RegisterType<IContactosService, ContactosService>();
            container.RegisterType<IContadoresService, ContadoresService>();
            container.RegisterType<IContadoresLotesService, ContadoresLotesService>();
            container.RegisterType<ICriteriosagrupacionService, CriteriosagrupacionService>();
            container.RegisterType<ICuentasService, CuentasService>();
            container.RegisterType<ICuentastesoreriaService, CuentastesoreriaService>();
            container.RegisterType<IDireccionesService, DireccionesService>();
            container.RegisterType<IEjerciciosService, EjerciciosService>();
            container.RegisterType<IEmpresasService, EmpresasService>();
            container.RegisterType<IEntregasService, EntregasService>();
            container.RegisterType<IFacturasComprasService, FacturasComprasService>();
            container.RegisterType<IFacturasService, FacturasService>();
            container.RegisterType<IFamiliasproductosService, FamiliasproductosService>();
            container.RegisterType<IFicherosService, FicherosService>();
            container.RegisterType<IFormasPagoService, FormasPagoService>();
            container.RegisterType<IGrosoresService, GrosoresService>();
            container.RegisterType<IGruposIvaService, GruposIvaService>();
            container.RegisterType<IGruposService, GruposService>();
            container.RegisterType<IGuiascontablesService, GuiascontablesService>();
            container.RegisterType<IIncidenciasService, IncidenciasService>();
            container.RegisterType<IInventariosService, InventariosService>();
            container.RegisterType<IMaterialesService, MaterialesService>();
            container.RegisterType<IMonedasService, MonedasService>();
            container.RegisterType<IObrasService, ObrasService>();
            container.RegisterType<IOperariosService, OperariosService>();
            container.RegisterType<IPedidosComprasService, PedidosComprasService>();
            container.RegisterType<IPedidosService, PedidosService>();
            container.RegisterType<IPlanesGeneralesService, PlanesGeneralesService>();
            container.RegisterType<IPresupuestosComprasService, PresupuestosComprasService>();
            container.RegisterType<IPresupuestosService, PresupuestosService>();
            container.RegisterType<IProspectosService, ProspectosService>();
            container.RegisterType<IProveedoresService, ProveedoresService>();
            container.RegisterType<IProvinciasService, ProvinciasService>();
            container.RegisterType<IPuertosService, PuertosService>();
            container.RegisterType<IRecepcionStockService, RecepcionStockService>();
            container.RegisterType<IRegimenivaService, RegimenivaService>();
            container.RegisterType<IReservasstockService, ReservasstockService>();
            container.RegisterType<ISeriesService, SeriesService>();
            container.RegisterType<ITablasVariasService, TablasVariasService>();
            container.RegisterType<ITarifasbaseService, TarifasbaseService>();
            container.RegisterType<ITarifasService, TarifasService>();
            container.RegisterType<ITiposcuentasService, TiposcuentasService>();
            container.RegisterType<ITiposivaService, TiposivaService>();
            container.RegisterType<ITiposRetencionesService, TiposRetencionesService>();
            container.RegisterType<ITransformacionesService, ImputacionCosteservice>();
            container.RegisterType<ITransportistasService, TransportistasService>();
            container.RegisterType<ITraspasosalmacenService, TraspasosalmacenService>();
            container.RegisterType<IUnidadesService, UnidadesService>();
            container.RegisterType<IUsuariosService, UsuariosService>();
            container.RegisterInstance(typeof(HttpConfiguration), GlobalConfiguration.Configuration);
            DependencyResolver.SetResolver(new Unity.Mvc5.UnityDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new Unity.WebApi.UnityDependencyResolver(container);
        }
    }
}