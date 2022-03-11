using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;
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
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.Graficaslistados;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Stock;
//contabilidad
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.Model.Contabilidad.Maes;
using Marfil.Dom.Persistencia.Model.CRM;
using Marfil.Dom.Persistencia.Model.Documentos.DivisionLotes;
using Marfil.Dom.Persistencia.Model.Documentos.GrupoMateriales;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Contabilidad;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using Marfil.Dom.Persistencia.Model.Configuracion.Inmueble;
using Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotesnave;

namespace Marfil.Dom.Persistencia.ServicesView
{
    public class FService
    {
        private static FService _instance;
        public static FService Instance
        {
            get
            {
                if(_instance==null)
                    _instance=new FService();

                return _instance;
            }
        }

        #region Members
       
        private readonly Dictionary<Type, Type> _servicesTypes = new Dictionary<Type, Type>();

        #endregion

        private FService()
        {
            _servicesTypes.Add(typeof(SeccionesanaliticasModel), typeof(SeccionesanaliticasService));
            _servicesTypes.Add(typeof(CarpetasModel), typeof(CarpetasService));
            _servicesTypes.Add(typeof(FicherosGaleriaModel), typeof(FicherosService));
            _servicesTypes.Add(typeof(BancosModel), typeof(BancosService));
            _servicesTypes.Add(typeof(BancosMandatosLinModel), typeof(BancosMandatosService));
            _servicesTypes.Add(typeof(CuentasModel), typeof(CuentasService));
            _servicesTypes.Add(typeof(FormasPagoModel), typeof(FormasPagoService));
            _servicesTypes.Add(typeof(SituacionesTesoreriaModel), typeof(SituacionesTesoreriaService));
            _servicesTypes.Add(typeof(MonedasModel), typeof(MonedasService));
            _servicesTypes.Add(typeof(ConfiguracionModel), typeof(ConfiguracionService));
            _servicesTypes.Add(typeof(TiposCuentasModel), typeof(TiposcuentasService));
            _servicesTypes.Add(typeof(InmueblesModel), typeof(InmueblesService));
            _servicesTypes.Add(typeof(GuiascontablesModel), typeof(GuiascontablesService));
            _servicesTypes.Add(typeof(TiposRetencionesModel), typeof(TiposRetencionesService));
            _servicesTypes.Add(typeof(TiposIvaModel), typeof(TiposivaService));
            _servicesTypes.Add(typeof(GruposIvaModel), typeof(GruposIvaService));
            _servicesTypes.Add(typeof(RegimenIvaModel), typeof(RegimenivaService));
            _servicesTypes.Add(typeof(MunicipiosModel), typeof(MunicipiosService));
            _servicesTypes.Add(typeof(ProvinciasModel), typeof(ProvinciasService));            
            _servicesTypes.Add(typeof(PuertosModel), typeof(PuertosService));
            _servicesTypes.Add(typeof(BaseTablasVariasModel), typeof(TablasVariasService));
            _servicesTypes.Add(typeof(UsuariosModel), typeof(UsuariosService));
            _servicesTypes.Add(typeof(RolesModel), typeof(GruposService));
            _servicesTypes.Add(typeof(EmpresaModel), typeof(EmpresasService));
            _servicesTypes.Add(typeof(UnidadesModel), typeof(UnidadesService));
            _servicesTypes.Add(typeof(DireccionesLinModel), typeof(DireccionesService));
            _servicesTypes.Add(typeof(ContactosLinModel), typeof(ContactosService));
            _servicesTypes.Add(typeof(GrosoresModel), typeof(GrosoresService));
            _servicesTypes.Add(typeof(AcabadosModel), typeof(AcabadosService));
            _servicesTypes.Add(typeof(FamiliasproductosModel), typeof(FamiliasproductosService));
            _servicesTypes.Add(typeof(MaterialesModel), typeof(MaterialesService));
            _servicesTypes.Add(typeof(CaracteristicasModel), typeof(CaracteristicasService));
            _servicesTypes.Add(typeof(ArticulosModel), typeof(ArticulosService));
            _servicesTypes.Add(typeof(ArticulosTerceroModel), typeof(ArticulosTerceroService));
            _servicesTypes.Add(typeof(ArticulosStockSeguridadModel), typeof(ArticulosStockSeguridadService));
            _servicesTypes.Add(typeof(LogStockSeguridadModel), typeof(LogStockSeguridadService));
            _servicesTypes.Add(typeof(VerificarContabilidadModel), typeof(VerificarContabilidadService));
            _servicesTypes.Add(typeof(GrupoMaterialesModel), typeof(GrupoMaterialesService));
            _servicesTypes.Add(typeof(ContadoresModel), typeof(ContadoresService));
            _servicesTypes.Add(typeof(ContadoresLotesModel), typeof(ContadoresLotesService));
            _servicesTypes.Add(typeof(SeriesModel), typeof(SeriesService));
            _servicesTypes.Add(typeof(SeriesContablesModel), typeof(SeriesContablesService));
            _servicesTypes.Add(typeof(TarifasModel), typeof(TarifasService));
            _servicesTypes.Add(typeof(PreciosEspecialesModel), typeof(PreciosEspecialesService));
            _servicesTypes.Add(typeof(EstadosModel), typeof(EstadosService));
            _servicesTypes.Add(typeof(ObrasModel), typeof(ObrasService));
            _servicesTypes.Add(typeof(CriteriosagrupacionModel), typeof(CriteriosagrupacionService));
            _servicesTypes.Add(typeof(PlanesGeneralesModel), typeof(PlanesGeneralesService));
            _servicesTypes.Add(typeof(IncidenciasModel), typeof(IncidenciasService));
            _servicesTypes.Add(typeof(EntregasStockModel), typeof(EntregasService));
            _servicesTypes.Add(typeof(ReservasstockModel), typeof(ReservasstockService));
            _servicesTypes.Add(typeof(TraspasosalmacenModel), typeof(TraspasosalmacenService));
            _servicesTypes.Add(typeof(InventariosModel), typeof(InventariosService));
            _servicesTypes.Add(typeof(ConfiguraciongraficasModel), typeof(ConfiguraciongraficasService));
            _servicesTypes.Add(typeof(TrabajosModel), typeof(TrabajosService));
            _servicesTypes.Add(typeof(TareasModel), typeof(TareasService));
            //terceros
            _servicesTypes.Add(typeof(AseguradorasModel), typeof(AseguradorasService));
            _servicesTypes.Add(typeof(AgentesModel), typeof(AgentesService));
            _servicesTypes.Add(typeof(ComercialesModel), typeof(ComercialesService));
            _servicesTypes.Add(typeof(ProveedoresModel), typeof(ProveedoresService));
            _servicesTypes.Add(typeof(ClientesModel), typeof(ClientesService));
            _servicesTypes.Add(typeof(AcreedoresModel), typeof(AcreedoresService));
            _servicesTypes.Add(typeof(TransportistasModel), typeof(TransportistasService));
            _servicesTypes.Add(typeof(OperariosModel), typeof(OperariosService));
            _servicesTypes.Add(typeof(CuentastesoreriaModel), typeof(CuentastesoreriaService));
            _servicesTypes.Add(typeof(ProspectosModel), typeof(ProspectosService));

            //Ejercicios
            _servicesTypes.Add(typeof(EjerciciosModel), typeof(EjerciciosService));

            //Documentos
            _servicesTypes.Add(typeof(PresupuestosModel), typeof(PresupuestosService));
            _servicesTypes.Add(typeof(PedidosModel), typeof(PedidosService));
            _servicesTypes.Add(typeof(AlbaranesModel), typeof(AlbaranesService));
            _servicesTypes.Add(typeof(FacturasModel), typeof(FacturasService));

            _servicesTypes.Add(typeof(PresupuestosComprasModel), typeof(PresupuestosComprasService));
            _servicesTypes.Add(typeof(PedidosComprasModel), typeof(PedidosComprasService));
            _servicesTypes.Add(typeof(AlbaranesComprasModel), typeof(AlbaranesComprasService));
            _servicesTypes.Add(typeof(FacturasComprasModel), typeof(FacturasComprasService));
            _servicesTypes.Add(typeof(RecepcionesStockModel), typeof(RecepcionStockService));

            _servicesTypes.Add(typeof(TransformacionesModel), typeof(ImputacionCosteservice));
            _servicesTypes.Add(typeof(TransformacioneslotesModel), typeof(TransformacioneslotesService));
            _servicesTypes.Add(typeof(TransformacioneslotesnaveModel), typeof(TransformacioneslotesnaveService));
            _servicesTypes.Add(typeof(DivisionLotesModel), typeof(DivisionLotesService));
            _servicesTypes.Add(typeof(ImputacionCostesModel), typeof(ImputacionCostesService));
            //stock
            _servicesTypes.Add(typeof(ConsultaVisualModel), typeof(ConsultaVisualService));
            _servicesTypes.Add(typeof(StockActualModel), typeof(StockactualService));
            _servicesTypes.Add(typeof(AlmacenesModel), typeof(AlmacenesService));
            _servicesTypes.Add(typeof(KitModel), typeof(KitService));
            _servicesTypes.Add(typeof(BundleModel), typeof(BundleService));

            //contabilidad
            _servicesTypes.Add(typeof(MovsModel), typeof(MovsService));
            _servicesTypes.Add(typeof(MaesModel), typeof(MaesService));
            _servicesTypes.Add(typeof(GuiasBalancesModel),typeof(GuiasBalancesService));
            //_servicesTypes.Add(typeof(GuiasBalancesLineasModel), typeof(GuiasBalancesLineasService));
            _servicesTypes.Add(typeof(SaldosAcumuladorPeriodosModel), typeof(SaldosAcumuladosPeriodosService));
            _servicesTypes.Add(typeof(CuadernosBancariosModel), typeof(CuadernosBancariosServices));
            _servicesTypes.Add(typeof(MapeoRemesasModel), typeof(MapeoRemesasService));
            _servicesTypes.Add(typeof(VencimientosModel), typeof(VencimientosService));
            _servicesTypes.Add(typeof(CircuitoTesoreriaCobrosModel), typeof(CircuitosTesoreriaCobrosService));
            _servicesTypes.Add(typeof(CarteraVencimientosModel), typeof(CarteraVencimientosService));
            _servicesTypes.Add(typeof(RemesasModel), typeof(RemesasService));
            //_servicesTypes.Add(typeof(CarteraModel), typeof(CarteraService));

            _servicesTypes.Add(typeof(OportunidadesModel), typeof(OportunidadesService));
            _servicesTypes.Add(typeof(SeguimientosModel), typeof(SeguimientosService));
            _servicesTypes.Add(typeof(SeguimientosCorreoModel), typeof(SeguimientosCorreoService));
            _servicesTypes.Add(typeof(ProyectosModel), typeof(ProyectosService));
            _servicesTypes.Add(typeof(CampañasModel), typeof(CampañasService));
            _servicesTypes.Add(typeof(IncidenciasCRMModel), typeof(IncidenciasCRMService));

            _servicesTypes.Add(typeof(PeticionesAsincronasModel), typeof(PeticionesAsincronasService));
            _servicesTypes.Add(typeof(CostesVariablesPeriodoModel), typeof(CostesVariablesPeriodoService));
        }

        #region API

        public IGestionService GetService(Type tipo, IContextService context,MarfilEntities db=null)
        {
            if (_servicesTypes.ContainsKey(tipo))
            {
                if (db == null)
                    db = MarfilEntities.ConnectToSqlServer(context.BaseDatos);
                
               

                var type = _servicesTypes[tipo];
                var ctor = type.GetConstructor(new[] { typeof(IContextService), typeof(MarfilEntities) });
                var result = ctor.Invoke(new object[] { context,db }) as IGestionService;
                result.Empresa = context.Empresa;
                return result;
            }

            return null;

        }

        #endregion
    }
}
