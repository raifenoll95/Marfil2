using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Validation;

namespace Marfil.Dom.Persistencia.ServicesView
{
    internal class FValidationService
    {
        readonly Dictionary<Type, Type> _validationServices = new Dictionary<Type, Type>();

        private static FValidationService _instance;
        public static FValidationService Instance
        {
            get
            {
                if(_instance==null)
                    _instance=new FValidationService();

                return _instance;
            }
        }

        private FValidationService()
        {
            _validationServices.Add(typeof(Seccionesanaliticas), typeof(SeccionesAnaliticasValidation));
            _validationServices.Add(typeof(Carpetas), typeof(CarpetasValidation));
            _validationServices.Add(typeof(Ficheros), typeof(FicherosValidation));
            _validationServices.Add(typeof(Configuracion),typeof(ConfiguracionValidation));
            _validationServices.Add(typeof(BancosMandatos),typeof(BancosMandatosValidation));
            _validationServices.Add(typeof(Roles),typeof(RolesValidation));
            _validationServices.Add(typeof(Usuarios),typeof(UsuariosValidation));
            _validationServices.Add(typeof(Tablasvarias),typeof(TablasVariasValidation));
            _validationServices.Add(typeof(FormasPago),typeof(FormasPagoValidation));
            _validationServices.Add(typeof(SituacionesTesoreria), typeof(SituacionesTesoreriaValidation));
            _validationServices.Add(typeof(Tiposretenciones),typeof(TiposRetencionesValidation));
            _validationServices.Add(typeof(TiposIva),typeof(TiposIvaValidation));
            _validationServices.Add(typeof(GruposIva),typeof(GruposIvaValidation));
            _validationServices.Add(typeof(RegimenIva),typeof(RegimenIvaValidation));
            _validationServices.Add(typeof(Bancos),typeof(BancosValidation));
            _validationServices.Add(typeof(Puertos),typeof(PuertosValidation));
            _validationServices.Add(typeof(Municipios), typeof(MunicipiosValidation));
            _validationServices.Add(typeof(Provincias),typeof(ProvinciasValidation));
            _validationServices.Add(typeof(Monedas),typeof(MonedasValidation));
            _validationServices.Add(typeof(Empresas),typeof(EmpresasValidation));
            _validationServices.Add(typeof(Planesgenerales),typeof(PlanesGeneralesValidation));
            _validationServices.Add(typeof(Tarifasbase), typeof(TarifasbaseValidation));
            _validationServices.Add(typeof(Cuentas),typeof(CuentasValidation));
            _validationServices.Add(typeof(Tiposcuentas),typeof(TiposcuentasValidation));
            _validationServices.Add(typeof(Inmuebles), typeof(InmueblesValidation));
            _validationServices.Add(typeof(Lecturas), typeof(LecturasValidation));
            _validationServices.Add(typeof(Unidades),typeof(UnidadesValidation));
            _validationServices.Add(typeof(Direcciones),typeof(DireccionesValidation));
            _validationServices.Add(typeof(Contactos),typeof(ContactosValidation));
            _validationServices.Add(typeof(Acabados),typeof(AcabadosValidation));
            _validationServices.Add(typeof(Grosores),typeof(GrosoresValidation));
            _validationServices.Add(typeof(Guiascontables),typeof(GuiascontablesValidation));
            _validationServices.Add(typeof(GrupoMateriales), typeof(GrupoMaterialesValidation));
            _validationServices.Add(typeof(Familiasproductos),typeof(FamiliasproductosValidation));
            _validationServices.Add(typeof(Materiales), typeof(MaterialesValidation));
            _validationServices.Add(typeof(Caracteristicas), typeof(CaracteristicasValidation));
            _validationServices.Add(typeof(Articulos), typeof(ArticulosValidation));
            _validationServices.Add(typeof(Contadores), typeof(ContadoresValidation));
            _validationServices.Add(typeof(ContadoresLotes), typeof(ContadoresLotesValidation));
            _validationServices.Add(typeof(Series), typeof(SeriesValidation));
            _validationServices.Add(typeof(SeriesContables), typeof(SeriesContablesValidation));
            _validationServices.Add(typeof(Tarifas), typeof(TarifasValidation));
            _validationServices.Add(typeof(PreciosEspeciales), typeof(PreciosEspecialesValidation));
            _validationServices.Add(typeof(Estados), typeof(EstadosValidation));
            _validationServices.Add(typeof(Obras), typeof(ObrasValidation));
            _validationServices.Add(typeof(Criteriosagrupacion), typeof(CriteriosagrupacionValidation));
            _validationServices.Add(typeof(Incidencias), typeof(IncidenciasValidation));
            _validationServices.Add(typeof(Reservasstock), typeof(ReservasstockValidation));
            _validationServices.Add(typeof(Traspasosalmacen), typeof(TraspasosalmacenValidation));
            _validationServices.Add(typeof(Inventarios), typeof(InventariosValidation));
            _validationServices.Add(typeof(Configuraciongraficas), typeof(ConfiguraciongraficasValidation));
            _validationServices.Add(typeof(Trabajos), typeof(TrabajosValidation));
            _validationServices.Add(typeof(Tareas), typeof(TareasValidation));
            //terceros
            _validationServices.Add(typeof(Clientes),typeof(ClientesValidation));
            _validationServices.Add(typeof(Aseguradoras),typeof(AseguradorasValidation));
            _validationServices.Add(typeof(Agentes),typeof(AgentesValidation));
            _validationServices.Add(typeof(Comerciales), typeof(ComercialesValidation));
            _validationServices.Add(typeof(Proveedores),typeof(ProveedoresValidation));
            _validationServices.Add(typeof(Acreedores),typeof(AcreedoresValidation));
            _validationServices.Add(typeof(Transportistas), typeof(TransportistasValidation));
            _validationServices.Add(typeof(Operarios),typeof(OperariosValidation));
            _validationServices.Add(typeof(Cuentastesoreria), typeof(CuentastesoreriaValidation));
            _validationServices.Add(typeof(Prospectos), typeof(ProspectosValidation));

            //Ejercicios
            _validationServices.Add(typeof(Ejercicios), typeof(EjerciciosValidation));

            //documentos
            _validationServices.Add(typeof(Presupuestos), typeof(PresupuestosValidation));
            _validationServices.Add(typeof(Pedidos), typeof(PedidosValidation));
            _validationServices.Add(typeof(Albaranes), typeof(AlbaranesValidation));
            _validationServices.Add(typeof(Facturas), typeof(FacturasValidation));
            _validationServices.Add(typeof(PresupuestosCompras), typeof(PresupuestosComprasValidation));
            _validationServices.Add(typeof(PedidosCompras), typeof(PedidosComprasValidation));
            _validationServices.Add(typeof(AlbaranesCompras), typeof(AlbaranesComprasValidation));
            _validationServices.Add(typeof(FacturasCompras), typeof(FacturasComprasValidation));
            _validationServices.Add(typeof(Transformaciones), typeof(TransformacionesValidation));
            _validationServices.Add(typeof(Transformacioneslotes), typeof(TransformacioneslotesValidation));
            _validationServices.Add(typeof(Transformacioneslotesnave), typeof(TransformacioneslotesnaveValidation));
            _validationServices.Add(typeof(DivisionLotes), typeof(DivisionLotesValidation));
            _validationServices.Add(typeof(ImputacionCostes), typeof(ImputacionCostesValidation));

            //stock
            _validationServices.Add(typeof(Almacenes), typeof(AlmacenesValidation));
            _validationServices.Add(typeof(Kit), typeof(KitValidation));
            _validationServices.Add(typeof(Bundle), typeof(BundleValidation));

            //contabilidad
            _validationServices.Add(typeof(Movs), typeof(MovsValidation));
            _validationServices.Add(typeof(Maes), typeof(MaesValidation));
            _validationServices.Add(typeof(Vencimientos), typeof(VencimientosValidation));
            _validationServices.Add(typeof(CircuitosTesoreriaCobros), typeof(CircuitosTesoreriaCobrosValidation));
            _validationServices.Add(typeof(TiposFacturas), typeof(TiposFacturasIvaValidation));
            _validationServices.Add(typeof(RegistroIVARepercutido), typeof(RegistroIvaRepercutidoValidation));
            _validationServices.Add(typeof(CarteraVencimientos), typeof(CarteraVencimientosValidation));
            _validationServices.Add(typeof(Remesas), typeof(RemesasValidation));
            _validationServices.Add(typeof(GuiasBalances), typeof(GuiasBalancesValidation));
            //_validationServices.Add(typeof(GuiasBalancesLineas), typeof(GuiasBalancesLineasValidation));
            _validationServices.Add(typeof(CuadernosBancarios), typeof(CuadernosBancariosValidation));

            _validationServices.Add(typeof(Oportunidades), typeof(OportunidadesValidation));
            _validationServices.Add(typeof(Seguimientos), typeof(SeguimientosValidation));
            _validationServices.Add(typeof(SeguimientosCorreo), typeof(SeguimientosCorreoValidation));
            _validationServices.Add(typeof(Proyectos), typeof(ProyectosValidation));
            _validationServices.Add(typeof(Campañas), typeof(CampañasValidation));
            _validationServices.Add(typeof(IncidenciasCRM), typeof(IncidenciasCRMValidation));

            _validationServices.Add(typeof(PeticionesAsincronas), typeof(PeticionesAsincronasValidation));
            _validationServices.Add(typeof(CostesVariablesPeriodo), typeof(CostesVariablesPeriodoValidation));
        }

        public IValidationService<T> CreateConverterModelService<T>(IContextService context, MarfilEntities db) where T : class
        {
            
            if (Instance._validationServices.ContainsKey(typeof(T)))
            {
                var type = Instance._validationServices[typeof(T)];
                var ctor = type.GetConstructor(new[] { typeof(IContextService),typeof(MarfilEntities) });
                var result = ctor.Invoke(new object[] { context, db }) as IValidationService<T>;
                return result;
            }
            return null;
        }
    }
}

