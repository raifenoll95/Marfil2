﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Marfil.Dom.Persistencia
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class MarfilEntities : DbContext
    {
        public MarfilEntities()
            : base("name=MarfilEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Administrador> Administrador { get; set; }
        public virtual DbSet<Agentes> Agentes { get; set; }
        public virtual DbSet<Almacenes> Almacenes { get; set; }
        public virtual DbSet<AlmacenesZona> AlmacenesZona { get; set; }
        public virtual DbSet<AppPermisos> AppPermisos { get; set; }
        public virtual DbSet<Aseguradoras> Aseguradoras { get; set; }
        public virtual DbSet<Bancos> Bancos { get; set; }
        public virtual DbSet<BancosMandatos> BancosMandatos { get; set; }
        public virtual DbSet<Caracteristicas> Caracteristicas { get; set; }
        public virtual DbSet<CaracteristicasLin> CaracteristicasLin { get; set; }
        public virtual DbSet<Carpetas> Carpetas { get; set; }
        public virtual DbSet<Comerciales> Comerciales { get; set; }
        public virtual DbSet<Configuracion> Configuracion { get; set; }
        public virtual DbSet<Contactos> Contactos { get; set; }
        public virtual DbSet<Contadores> Contadores { get; set; }
        public virtual DbSet<ContadoresLin> ContadoresLin { get; set; }
        public virtual DbSet<Criteriosagrupacion> Criteriosagrupacion { get; set; }
        public virtual DbSet<CriteriosagrupacionLin> CriteriosagrupacionLin { get; set; }
        public virtual DbSet<Cuentas> Cuentas { get; set; }
        public virtual DbSet<Cuentastesoreria> Cuentastesoreria { get; set; }
        public virtual DbSet<Direcciones> Direcciones { get; set; }
        public virtual DbSet<Estados> Estados { get; set; }
        public virtual DbSet<FacturasLin> FacturasLin { get; set; }
        public virtual DbSet<FacturasTotales> FacturasTotales { get; set; }
        public virtual DbSet<FacturasVencimientos> FacturasVencimientos { get; set; }
        public virtual DbSet<Ficheros> Ficheros { get; set; }
        public virtual DbSet<FormasPago> FormasPago { get; set; }
        public virtual DbSet<FormasPagoLin> FormasPagoLin { get; set; }
        public virtual DbSet<GruposIva> GruposIva { get; set; }
        public virtual DbSet<GruposIvaLin> GruposIvaLin { get; set; }
        public virtual DbSet<Guiascontables> Guiascontables { get; set; }
        public virtual DbSet<GuiascontablesLin> GuiascontablesLin { get; set; }
        public virtual DbSet<Incidencias> Incidencias { get; set; }
        public virtual DbSet<Materiales> Materiales { get; set; }
        public virtual DbSet<MaterialesLin> MaterialesLin { get; set; }
        public virtual DbSet<Monedas> Monedas { get; set; }
        public virtual DbSet<MonedasLog> MonedasLog { get; set; }
        public virtual DbSet<Obras> Obras { get; set; }
        public virtual DbSet<Operarios> Operarios { get; set; }
        public virtual DbSet<Pedidos> Pedidos { get; set; }
        public virtual DbSet<PedidosCompras> PedidosCompras { get; set; }
        public virtual DbSet<PedidosComprasLin> PedidosComprasLin { get; set; }
        public virtual DbSet<PedidosComprasTotales> PedidosComprasTotales { get; set; }
        public virtual DbSet<PedidosLin> PedidosLin { get; set; }
        public virtual DbSet<PedidosTotales> PedidosTotales { get; set; }
        public virtual DbSet<Planesgenerales> Planesgenerales { get; set; }
        public virtual DbSet<PreferenciasUsuario> PreferenciasUsuario { get; set; }
        public virtual DbSet<Presupuestos> Presupuestos { get; set; }
        public virtual DbSet<PresupuestosCompras> PresupuestosCompras { get; set; }
        public virtual DbSet<PresupuestosComprasLin> PresupuestosComprasLin { get; set; }
        public virtual DbSet<PresupuestosComprasTotales> PresupuestosComprasTotales { get; set; }
        public virtual DbSet<PresupuestosLin> PresupuestosLin { get; set; }
        public virtual DbSet<PresupuestosTotales> PresupuestosTotales { get; set; }
        public virtual DbSet<Prospectos> Prospectos { get; set; }
        public virtual DbSet<Provincias> Provincias { get; set; }
        public virtual DbSet<Puertos> Puertos { get; set; }
        public virtual DbSet<RegimenIva> RegimenIva { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Tablasvarias> Tablasvarias { get; set; }
        public virtual DbSet<TablasvariasLin> TablasvariasLin { get; set; }
        public virtual DbSet<Tarifas> Tarifas { get; set; }
        public virtual DbSet<Tarifasbase> Tarifasbase { get; set; }
        public virtual DbSet<TarifasLin> TarifasLin { get; set; }
        public virtual DbSet<Tiposcuentas> Tiposcuentas { get; set; }
        public virtual DbSet<TiposcuentasLin> TiposcuentasLin { get; set; }
        public virtual DbSet<TiposIva> TiposIva { get; set; }
        public virtual DbSet<Transportistas> Transportistas { get; set; }
        public virtual DbSet<Usuarios> Usuarios { get; set; }
        public virtual DbSet<Reservasstock> Reservasstock { get; set; }
        public virtual DbSet<ReservasstockLin> ReservasstockLin { get; set; }
        public virtual DbSet<ReservasstockTotales> ReservasstockTotales { get; set; }
        public virtual DbSet<InventariosLin> InventariosLin { get; set; }
        public virtual DbSet<Configuraciongraficas> Configuraciongraficas { get; set; }
        public virtual DbSet<Articulos> Articulos { get; set; }
        public virtual DbSet<Familiasproductos> Familiasproductos { get; set; }
        public virtual DbSet<Traspasosalmacen> Traspasosalmacen { get; set; }
        public virtual DbSet<TraspasosalmacenCostesadicionales> TraspasosalmacenCostesadicionales { get; set; }
        public virtual DbSet<TraspasosalmacenLin> TraspasosalmacenLin { get; set; }
        public virtual DbSet<Bundle> Bundle { get; set; }
        public virtual DbSet<BundleLin> BundleLin { get; set; }
        public virtual DbSet<ContadoresLotes> ContadoresLotes { get; set; }
        public virtual DbSet<ContadoresLotesLin> ContadoresLotesLin { get; set; }
        public virtual DbSet<Kit> Kit { get; set; }
        public virtual DbSet<KitLin> KitLin { get; set; }
        public virtual DbSet<Trabajos> Trabajos { get; set; }
        public virtual DbSet<Transformacioneslotescostesadicionales> Transformacioneslotescostesadicionales { get; set; }
        public virtual DbSet<Transformacionescostesadicionales> Transformacionescostesadicionales { get; set; }
        public virtual DbSet<FacturasComprasLin> FacturasComprasLin { get; set; }
        public virtual DbSet<FacturasComprasTotales> FacturasComprasTotales { get; set; }
        public virtual DbSet<FacturasComprasVencimientos> FacturasComprasVencimientos { get; set; }
        public virtual DbSet<AlbaranesComprasCostesadicionales> AlbaranesComprasCostesadicionales { get; set; }
        public virtual DbSet<AlbaranesComprasTotales> AlbaranesComprasTotales { get; set; }
        public virtual DbSet<AlbaranesLin> AlbaranesLin { get; set; }
        public virtual DbSet<AlbaranesTotales> AlbaranesTotales { get; set; }
        public virtual DbSet<Usuariosactivos> Usuariosactivos { get; set; }
        public virtual DbSet<Facturas> Facturas { get; set; }
        public virtual DbSet<FacturasCompras> FacturasCompras { get; set; }
        public virtual DbSet<Albaranes> Albaranes { get; set; }
        public virtual DbSet<Stockactual> Stockactual { get; set; }
        public virtual DbSet<Stockhistorico> Stockhistorico { get; set; }
        public virtual DbSet<Movimientosstock> Movimientosstock { get; set; }
        public virtual DbSet<Transformaciones> Transformaciones { get; set; }
        public virtual DbSet<Transformacionesentradalin> Transformacionesentradalin { get; set; }
        public virtual DbSet<Transformacionessalidalin> Transformacionessalidalin { get; set; }
        public virtual DbSet<AlbaranesCompras> AlbaranesCompras { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<Inventarios> Inventarios { get; set; }
        public virtual DbSet<Transformacioneslotes> Transformacioneslotes { get; set; }
        public virtual DbSet<Transformacionesloteslin> Transformacionesloteslin { get; set; }
        public virtual DbSet<ContadoresContabilidadLin> ContadoresContabilidadLin { get; set; }
        public virtual DbSet<ContadoresContabilidad> ContadoresContabilidad { get; set; }
        public virtual DbSet<Maes> Maes { get; set; }
        public virtual DbSet<SeriesContables> SeriesContables { get; set; }
        public virtual DbSet<Movs> Movs { get; set; }
        public virtual DbSet<Tareas> Tareas { get; set; }
        public virtual DbSet<TareasLin> TareasLin { get; set; }
        public virtual DbSet<PedidosCostesFabricacion> PedidosCostesFabricacion { get; set; }
        public virtual DbSet<Empresas> Empresas { get; set; }
        public virtual DbSet<MovsLin> MovsLin { get; set; }
        public virtual DbSet<Seccionesanaliticas> Seccionesanaliticas { get; set; }
        public virtual DbSet<AlbaranesComprasLin> AlbaranesComprasLin { get; set; }
        public virtual DbSet<Oportunidades> Oportunidades { get; set; }
        public virtual DbSet<IncidenciasCRM> IncidenciasCRM { get; set; }
        public virtual DbSet<Proyectos> Proyectos { get; set; }
        public virtual DbSet<PeticionesAsincronas> PeticionesAsincronas { get; set; }
        public virtual DbSet<CostesVariablesPeriodo> CostesVariablesPeriodo { get; set; }
        public virtual DbSet<CostesVariablesPeriodoLin> CostesVariablesPeriodoLin { get; set; }
        public virtual DbSet<DivisionLotes> DivisionLotes { get; set; }
        public virtual DbSet<DivisionLotesentradalin> DivisionLotesentradalin { get; set; }
        public virtual DbSet<DivisionLotessalidalin> DivisionLotessalidalin { get; set; }
        public virtual DbSet<DivisionLotescostesadicionales> DivisionLotescostesadicionales { get; set; }
        public virtual DbSet<ArticulosTercero> ArticulosTercero { get; set; }
        public virtual DbSet<ArticulosComponentes> ArticulosComponentes { get; set; }
        public virtual DbSet<ImputacionCostes> ImputacionCostes { get; set; }
        public virtual DbSet<ImputacionCostesLin> ImputacionCostesLin { get; set; }
        public virtual DbSet<ImputacionCostescostesadicionales> ImputacionCostescostesadicionales { get; set; }
        public virtual DbSet<TrabajosLin> TrabajosLin { get; set; }
        public virtual DbSet<Campañas> Campañas { get; set; }
        public virtual DbSet<CampañasTercero> CampañasTercero { get; set; }
        public virtual DbSet<Seguimientos> Seguimientos { get; set; }
        public virtual DbSet<SeguimientosCorreo> SeguimientosCorreo { get; set; }
        public virtual DbSet<Municipios> Municipios { get; set; }
        public virtual DbSet<GrupoMateriales> GrupoMateriales { get; set; }
        public virtual DbSet<Acabados> Acabados { get; set; }
        public virtual DbSet<Grosores> Grosores { get; set; }
        public virtual DbSet<SituacionesTesoreria> SituacionesTesoreria { get; set; }
        public virtual DbSet<Unidades> Unidades { get; set; }
        public virtual DbSet<Diariostock> Diariostock { get; set; }
        public virtual DbSet<ListadoStockValorado> ListadoStockValorado { get; set; }
        public virtual DbSet<Lotes> Lotes { get; set; }
        public virtual DbSet<CircuitosTesoreriaCobros> CircuitosTesoreriaCobros { get; set; }
        public virtual DbSet<TipoGuia> TipoGuia { get; set; }
        public virtual DbSet<TipoInforme> TipoInforme { get; set; }
        public virtual DbSet<CarteraVencimientos> CarteraVencimientos { get; set; }
        public virtual DbSet<PrevisionesCartera> PrevisionesCartera { get; set; }
        public virtual DbSet<Vencimientos> Vencimientos { get; set; }
        public virtual DbSet<Ejercicios> Ejercicios { get; set; }
        public virtual DbSet<SaldosAcomuladosPeriodos> SaldosAcomuladosPeriodos { get; set; }
        public virtual DbSet<SaldosAcumuladosPeriodosLin> SaldosAcumuladosPeriodosLin { get; set; }
        public virtual DbSet<PresupuestosComponentesLin> PresupuestosComponentesLin { get; set; }
        public virtual DbSet<Inmuebles> Inmuebles { get; set; }
        public virtual DbSet<InformeMargen> InformeMargen { get; set; }
        public virtual DbSet<PreciosEspeciales> PreciosEspeciales { get; set; }
        public virtual DbSet<AcumuladorPeriodos> AcumuladorPeriodos { get; set; }
        public virtual DbSet<FiltrosAcumulador> FiltrosAcumulador { get; set; }
        public virtual DbSet<CuadernosBancariosLin> CuadernosBancariosLin { get; set; }
        public virtual DbSet<MapeoRemesas> MapeoRemesas { get; set; }
        public virtual DbSet<Transformacioneslotesnave> Transformacioneslotesnave { get; set; }
        public virtual DbSet<Transformacioneslotesnavecostesadicionales> Transformacioneslotesnavecostesadicionales { get; set; }
        public virtual DbSet<Transformacioneslotesnavelin> Transformacioneslotesnavelin { get; set; }
        public virtual DbSet<CuadernosBancarios> CuadernosBancarios { get; set; }
        public virtual DbSet<Remesas> Remesas { get; set; }
        public virtual DbSet<CuentasNoAsignadas> CuentasNoAsignadas { get; set; }
        public virtual DbSet<ReportGuiasBalances> ReportGuiasBalances { get; set; }
        public virtual DbSet<ReportGuiasBalancesLineas> ReportGuiasBalancesLineas { get; set; }
        public virtual DbSet<DocumentosUsuario> DocumentosUsuario { get; set; }
        public virtual DbSet<GuiasBalances> GuiasBalances { get; set; }
        public virtual DbSet<GuiasBalancesLineas> GuiasBalancesLineas { get; set; }
        public virtual DbSet<FiltrosPYG> FiltrosPYG { get; set; }
        public virtual DbSet<AnaliticaFiltrosPYG> AnaliticaFiltrosPYG { get; set; }
        public virtual DbSet<CuentasNoAsignadasAnalitica> CuentasNoAsignadasAnalitica { get; set; }
        public virtual DbSet<ReportAnaliticaGuiasBalances> ReportAnaliticaGuiasBalances { get; set; }
        public virtual DbSet<ReportAnaliticaGuiasBalancesLineas> ReportAnaliticaGuiasBalancesLineas { get; set; }
        public virtual DbSet<ArticulosStockSeguridad> ArticulosStockSeguridad { get; set; }
        public virtual DbSet<LogStockSeguridad> LogStockSeguridad { get; set; }
        public virtual DbSet<FiltrosPYGFuncional> FiltrosPYGFuncional { get; set; }
        public virtual DbSet<ReportGuiasBalancesFuncional> ReportGuiasBalancesFuncional { get; set; }
        public virtual DbSet<ReportGuiasBalancesLineasFuncional> ReportGuiasBalancesLineasFuncional { get; set; }
        public virtual DbSet<CuentasNoAsignadasFuncional> CuentasNoAsignadasFuncional { get; set; }
        public virtual DbSet<VerificarContabilidad> VerificarContabilidad { get; set; }
        public virtual DbSet<CuentasNoAsignadasBalanceAnual> CuentasNoAsignadasBalanceAnual { get; set; }
        public virtual DbSet<FiltrosPYGBalanceAnual> FiltrosPYGBalanceAnual { get; set; }
        public virtual DbSet<ReportGuiasBalancesBalanceAnual> ReportGuiasBalancesBalanceAnual { get; set; }
        public virtual DbSet<ReportGuiasBalancesLineasBalanceAnual> ReportGuiasBalancesLineasBalanceAnual { get; set; }
        public virtual DbSet<TiposFacturas> TiposFacturas { get; set; }
        public virtual DbSet<Proveedores> Proveedores { get; set; }
        public virtual DbSet<Acreedores> Acreedores { get; set; }
        public virtual DbSet<Clientes> Clientes { get; set; }
        public virtual DbSet<Lecturas> Lecturas { get; set; }
        public virtual DbSet<Tiposretenciones> Tiposretenciones { get; set; }
        public virtual DbSet<RegistroIVARepercutido> RegistroIVARepercutido { get; set; }
        public virtual DbSet<RegistroIVARepercutidoRectificadas> RegistroIVARepercutidoRectificadas { get; set; }
        public virtual DbSet<RegistroIVARepercutidoTotales> RegistroIVARepercutidoTotales { get; set; }
    
        public virtual ObjectResult<spLotes_Result> spLotes()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<spLotes_Result>("spLotes");
        }
    }
}
