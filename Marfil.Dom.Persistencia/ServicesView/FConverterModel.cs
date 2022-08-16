using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Converter;
using Marfil.Dom.Persistencia.Model.CRM;

namespace Marfil.Dom.Persistencia.ServicesView
{
    internal class FConverterModel
    {
        private static FConverterModel _instance;

        public static FConverterModel Instance
        {
            get
            {
                if(_instance==null)
                    _instance=new FConverterModel();
                return _instance;
            }
        }


        private readonly Dictionary<Type, Type> _converterServicesTypes = new Dictionary<Type, Type>();
        
        private FConverterModel()
        {
            _converterServicesTypes.Add(typeof(Seccionesanaliticas), typeof(SeccionesanaliticasConverterService));
            _converterServicesTypes.Add(typeof(Carpetas), typeof(CarpetasConverterService));
            _converterServicesTypes.Add(typeof(Ficheros), typeof(FicherosConverterService));
            _converterServicesTypes.Add(typeof(Bancos), typeof(BancosConverterService));
            _converterServicesTypes.Add(typeof(BancosMandatos), typeof(BancosMandatosConverterService));
            _converterServicesTypes.Add(typeof(Roles), typeof(RolesConverterService));
            _converterServicesTypes.Add(typeof(Tablasvarias), typeof(TablasVariasConverterService<TablasVariasPaisesModel, TablasvariasLin>));
            _converterServicesTypes.Add(typeof(TablasVariasPaisesModel), typeof(TablasVariasLinConverterService<TablasVariasPaisesModel>));
            _converterServicesTypes.Add(typeof(FormasPago), typeof(FormasPagoConverterService));
            _converterServicesTypes.Add(typeof(SituacionesTesoreria), typeof(SituacionesTesoreriaConverterService));
            _converterServicesTypes.Add(typeof(Configuracion), typeof(ConfiguracionConverterService));
            _converterServicesTypes.Add(typeof(Tiposretenciones), typeof(TiposRetencionesConverterService));
            _converterServicesTypes.Add(typeof(TiposIva), typeof(TiposIvaConverterService));
            _converterServicesTypes.Add(typeof(GruposIva), typeof(GruposIvaConverterService));
            _converterServicesTypes.Add(typeof(RegimenIva), typeof(RegimenIvaConverterService));
            _converterServicesTypes.Add(typeof(Municipios), typeof(MunicipiosConverterService));
            _converterServicesTypes.Add(typeof(Provincias), typeof(ProvinciasConverterService));            
            _converterServicesTypes.Add(typeof(Puertos), typeof(PuertosConverterService));
            _converterServicesTypes.Add(typeof(Monedas), typeof(MonedasConverterService));
            _converterServicesTypes.Add(typeof(Empresas), typeof(EmpresasConverterService));
            _converterServicesTypes.Add(typeof(Cuentas), typeof(CuentasConverterService));
            _converterServicesTypes.Add(typeof(Tiposcuentas), typeof(TiposcuentasConverterService));
            _converterServicesTypes.Add(typeof(Inmuebles), typeof(InmueblesConverterService));
            _converterServicesTypes.Add(typeof(Unidades), typeof(UnidadesConverterService));
            _converterServicesTypes.Add(typeof(Direcciones), typeof(DireccionesConverterService));
            _converterServicesTypes.Add(typeof(Contactos), typeof(ContactosConverterService));
            _converterServicesTypes.Add(typeof(Guiascontables), typeof(GuiascontablesConverterService));
            _converterServicesTypes.Add(typeof(GrupoMateriales), typeof(GrupoMaterialesConverterService));
            _converterServicesTypes.Add(typeof(Familiasproductos), typeof(FamiliasproductosConverterService));
            _converterServicesTypes.Add(typeof(Materiales), typeof(MaterialesConverterService));
            _converterServicesTypes.Add(typeof(Acabados), typeof(AcabadosConverterService));
            _converterServicesTypes.Add(typeof(Grosores), typeof(GrosoresConverterService));
            _converterServicesTypes.Add(typeof(Caracteristicas), typeof(CaracteristicasConverterService));
            _converterServicesTypes.Add(typeof(Articulos), typeof(ArticulosConverterService));
            _converterServicesTypes.Add(typeof(LogStockSeguridad), typeof(LogStockSeguridadConverterService));
            _converterServicesTypes.Add(typeof(VerificarContabilidad), typeof(VerificarContabilidadConverterService));
            _converterServicesTypes.Add(typeof(Contadores), typeof(ContadoresConverterService));
            _converterServicesTypes.Add(typeof(ContadoresLotes), typeof(ContadoresLotesConverterService));
            _converterServicesTypes.Add(typeof(Series), typeof(SeriesConverterService));
            _converterServicesTypes.Add(typeof(SeriesContables), typeof(SeriesContablesConverterService));
            _converterServicesTypes.Add(typeof(Tarifas), typeof(TarifasConverterService));
            _converterServicesTypes.Add(typeof(PreciosEspeciales), typeof(PreciosEspecialesConverterService));
            _converterServicesTypes.Add(typeof(Estados), typeof(EstadosConverterService));
            _converterServicesTypes.Add(typeof(Obras), typeof(ObrasConverterService));
            _converterServicesTypes.Add(typeof(Criteriosagrupacion), typeof(CriteriosagrupacionConverterService));
            _converterServicesTypes.Add(typeof(Usuarios), typeof(UsuariosConverterService));
            _converterServicesTypes.Add(typeof(Incidencias), typeof(IncidenciasConverterService));
            _converterServicesTypes.Add(typeof(Reservasstock), typeof(ReservasstockConverterService));
            _converterServicesTypes.Add(typeof(Traspasosalmacen), typeof(TraspasosalmacenConverterService));
            _converterServicesTypes.Add(typeof(Inventarios), typeof(InventariosConverterService));
            _converterServicesTypes.Add(typeof(Configuraciongraficas), typeof(ConfiguraciongraficasConverterService));
            _converterServicesTypes.Add(typeof(Trabajos), typeof(TrabajosConverterService));
            _converterServicesTypes.Add(typeof(Tareas), typeof(TareasConverterService));

            //Terceros 
            _converterServicesTypes.Add(typeof(Clientes), typeof(ClientesConverterService));
            _converterServicesTypes.Add(typeof(Aseguradoras), typeof(AseguradorasConverterService));
            _converterServicesTypes.Add(typeof(Agentes), typeof(AgentesConverterService));
            _converterServicesTypes.Add(typeof(Comerciales), typeof(ComercialesConverterService));
            _converterServicesTypes.Add(typeof(Proveedores), typeof(ProveedoresConverterService));
            _converterServicesTypes.Add(typeof(Acreedores), typeof(AcreedoresConverterService));
            _converterServicesTypes.Add(typeof(Transportistas), typeof(TransportistasConverterService));
            _converterServicesTypes.Add(typeof(Operarios), typeof(OperariosConverterService));
            _converterServicesTypes.Add(typeof(Cuentastesoreria), typeof(CuentastesoreriaConverterService));
            _converterServicesTypes.Add(typeof(Prospectos), typeof(ProspectosConverterService));

            //Ejercicio
            _converterServicesTypes.Add(typeof(Ejercicios), typeof(EjerciciosConverterService));

            //Documentos
            _converterServicesTypes.Add(typeof(Presupuestos), typeof(PresupuestosConverterService));
            _converterServicesTypes.Add(typeof(Pedidos), typeof(PedidosConverterService));
            _converterServicesTypes.Add(typeof(Albaranes), typeof(AlbaranesConverterService));
            _converterServicesTypes.Add(typeof(Facturas), typeof(FacturasConverterService));
            _converterServicesTypes.Add(typeof(PresupuestosCompras), typeof(PresupuestosComprasConverterService));
            _converterServicesTypes.Add(typeof(PedidosCompras), typeof(PedidosComprasConverterService));
            _converterServicesTypes.Add(typeof(AlbaranesCompras), typeof(AlbaranesComprasConverterService));
            _converterServicesTypes.Add(typeof(FacturasCompras), typeof(FacturasComprasConverterService));
            _converterServicesTypes.Add(typeof(Transformaciones), typeof(TransformacionesConverterService));
            _converterServicesTypes.Add(typeof(Transformacioneslotes), typeof(TransformacioneslotesConverterService));
            _converterServicesTypes.Add(typeof(Transformacioneslotesnave), typeof(TransformacioneslotesnaveConverterService));
            _converterServicesTypes.Add(typeof(DivisionLotes), typeof(DivisionLotesConverterService));
            _converterServicesTypes.Add(typeof(ImputacionCostes), typeof(ImputacionCostesConverterService));

            //stock
            _converterServicesTypes.Add(typeof(Almacenes), typeof(AlmacenesConverterService));
            _converterServicesTypes.Add(typeof(Kit), typeof(KitConverterService));
            _converterServicesTypes.Add(typeof(Bundle), typeof(BundleConverterService));

            //contabilidad
            _converterServicesTypes.Add(typeof(Movs), typeof(MovsConverterService));
            _converterServicesTypes.Add(typeof(Maes), typeof(MaesConverterService));
            _converterServicesTypes.Add(typeof(GuiasBalances), typeof(GuiasBalancesConvertService));
            //_converterServicesTypes.Add(typeof(GuiasBalancesLineas), typeof(GuiasBalancesLineasConvertService));
            _converterServicesTypes.Add(typeof(SaldosAcomuladosPeriodos), typeof(SaldosAcomuladosPeriodosConverteService));
            _converterServicesTypes.Add(typeof(CuadernosBancarios), typeof(CuadernosBancariosConverterServices));
            _converterServicesTypes.Add(typeof(Vencimientos), typeof(VencimientosConverterService));
            _converterServicesTypes.Add(typeof(CircuitosTesoreriaCobros), typeof(CircuitosTesoreriaConverterCobrosConverterService));
            _converterServicesTypes.Add(typeof(TiposFacturas), typeof(TiposFacturasIvaConverterService));
            _converterServicesTypes.Add(typeof(CarteraVencimientos), typeof(CarteraVencimientosConverterService));
            _converterServicesTypes.Add(typeof(Remesas), typeof(RemesasConverterService));
            _converterServicesTypes.Add(typeof(Oportunidades), typeof(OportunidadesConverterService));
            _converterServicesTypes.Add(typeof(Seguimientos), typeof(SeguimientosConverterService));
            _converterServicesTypes.Add(typeof(SeguimientosCorreo), typeof(SeguimientosCorreoConverterService));
            _converterServicesTypes.Add(typeof(Proyectos), typeof(ProyectosConverterService));
            _converterServicesTypes.Add(typeof(Campañas), typeof(CampañasConverterService));
            _converterServicesTypes.Add(typeof(IncidenciasCRM), typeof(IncidenciasCRMConverterService));
            _converterServicesTypes.Add(typeof(PeticionesAsincronas), typeof(PeticionesAsincronasConverterModel));
            _converterServicesTypes.Add(typeof(CostesVariablesPeriodo), typeof(CostesVariablesPeriodoConverterService));
            _converterServicesTypes.Add(typeof(RegistroIVARepercutido), typeof(RegistroIvaRepercutidoConverterService));
        }

        public IConverterModelService<TView, TPersistance> CreateConverterModelService<TView, TPersistance>(IContextService context, MarfilEntities db,string empresa) where TPersistance : class where TView: class
        {
            
            if (_converterServicesTypes.ContainsKey(typeof(TPersistance)))
            {
                var type = _converterServicesTypes[typeof(TPersistance)];
                var ctor = type.GetConstructor(new[] { typeof(IContextService), typeof(MarfilEntities) });
                var result= ctor.Invoke(new object[] { context,db }) as IConverterModelService<TView, TPersistance>;
                result.Empresa = empresa;
               
                return result;
            }
           
            return new BaseConverterModel<TView, TPersistance>(context,db);
        } 
    }
}
