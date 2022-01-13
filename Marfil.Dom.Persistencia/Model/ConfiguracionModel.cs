using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using RConfiguracion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Configuracionaplicacion;
using RGrosores = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Grosores;
using RCriteriosagrupacion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Criteriosagrupacion;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;

namespace Marfil.Dom.Persistencia.Model
{
    public enum TipoFormatoGrosor
    {
        [StringValue(typeof(RGrosores), "TipoFormatoGrosorCodigo")]
        Codigo,
        [StringValue(typeof(RGrosores), "TipoFormatoGrosorCodigoyUnidad")]
        CodigoyUnidad,
        [StringValue(typeof(RGrosores), "TipoFormatoGrosorGrosorcm")]
        Grosorcm,
        [StringValue(typeof(RGrosores), "TipoFormatoGrosorGrosorcmyUnidad")]
        GrosorcmyUnidad,
        [StringValue(typeof(RGrosores), "TipoFormatoGrosorGrosorm")]
        Grosorm,
        [StringValue(typeof(RGrosores), "TipoFormatoGrosorGrosormyUnidad")]
        GrosormyUnidad
    }

    public enum TipoFormatoUnidad
    {
        CM,
        cm,
        Cm
    }

    
    public class ConfiguracionModel : BaseModel<ConfiguracionModel, Persistencia.Configuracion>
    {
        private InternalConfiguracionModel _model= new InternalConfiguracionModel();

        #region CTR

        public ConfiguracionModel():base(new ContextConfiguracion())
        {
         
            var serviceEstados = new EstadosService(Context);
            Estados = serviceEstados.GetStates(DocumentoEstado.PresupuestosVentas, TipoMovimientos.Todos).ToList();
            Estadospedidosventas = serviceEstados.GetStates(DocumentoEstado.PedidosVentas, TipoMovimientos.Todos).ToList();
            Estadosalbaranesventas = serviceEstados.GetStates(DocumentoEstado.AlbaranesVentas, TipoMovimientos.Todos).ToList();
            Estadosfacturasventas = serviceEstados.GetStates(DocumentoEstado.FacturasVentas, TipoMovimientos.Todos).ToList();
            Estadospresupuestoscompras = serviceEstados.GetStates(DocumentoEstado.PresupuestosCompras, TipoMovimientos.Todos).ToList();
            Estadospedidoscompras = serviceEstados.GetStates(DocumentoEstado.PedidosCompras, TipoMovimientos.Todos).ToList();
            Estadosalbaranescompras = serviceEstados.GetStates(DocumentoEstado.AlbaranesCompras, TipoMovimientos.Todos).ToList();
            Estadosfacturascompras = serviceEstados.GetStates(DocumentoEstado.FacturasCompras, TipoMovimientos.Todos).ToList();
            Estadosreservas = serviceEstados.GetStates(DocumentoEstado.Reservasstock, TipoMovimientos.Todos).ToList();
            Estadostraspasosalmacen = serviceEstados.GetStates(DocumentoEstado.Traspasosalmacen, TipoMovimientos.Todos).ToList();
            Estadosoportunidades = serviceEstados.GetStates(DocumentoEstado.Oportunidades, TipoMovimientos.Todos).ToList();
            Estadosproyectos = serviceEstados.GetStates(DocumentoEstado.Proyectos, TipoMovimientos.Todos).ToList();
            Estadoscampañas = serviceEstados.GetStates(DocumentoEstado.Campañas, TipoMovimientos.Todos).ToList();
            Estadosincidencias = serviceEstados.GetStates(DocumentoEstado.Incidencias, TipoMovimientos.Todos).ToList();            

            DescripcionesAsientos = TablasVariasService.GetListDescripcionAsientos(Context);
        }

        public ConfiguracionModel(IContextService context) : base(context)
        {
            var serviceEstados = new EstadosService(context);
            Estados = serviceEstados.GetStates(DocumentoEstado.PresupuestosVentas, TipoMovimientos.Todos).ToList();
            Estadospedidosventas = serviceEstados.GetStates(DocumentoEstado.PedidosVentas, TipoMovimientos.Todos).ToList();
            Estadosalbaranesventas = serviceEstados.GetStates(DocumentoEstado.AlbaranesVentas, TipoMovimientos.Todos).ToList();
            Estadosfacturasventas = serviceEstados.GetStates(DocumentoEstado.FacturasVentas, TipoMovimientos.Todos).ToList();
            Estadospresupuestoscompras = serviceEstados.GetStates(DocumentoEstado.PresupuestosCompras, TipoMovimientos.Todos).ToList();
            Estadospedidoscompras = serviceEstados.GetStates(DocumentoEstado.PedidosCompras, TipoMovimientos.Todos).ToList();
            Estadosalbaranescompras = serviceEstados.GetStates(DocumentoEstado.AlbaranesCompras, TipoMovimientos.Todos).ToList();
            Estadosfacturascompras = serviceEstados.GetStates(DocumentoEstado.FacturasCompras, TipoMovimientos.Todos).ToList();
            Estadosreservas = serviceEstados.GetStates(DocumentoEstado.Reservasstock, TipoMovimientos.Todos).ToList();
            Estadostraspasosalmacen = serviceEstados.GetStates(DocumentoEstado.Traspasosalmacen, TipoMovimientos.Todos).ToList();
            Estadosoportunidades = serviceEstados.GetStates(DocumentoEstado.Oportunidades, TipoMovimientos.Todos).ToList();
            Estadosproyectos = serviceEstados.GetStates(DocumentoEstado.Proyectos, TipoMovimientos.Todos).ToList();
            Estadoscampañas = serviceEstados.GetStates(DocumentoEstado.Campañas, TipoMovimientos.Todos).ToList();
            Estadosincidencias = serviceEstados.GetStates(DocumentoEstado.Incidencias, TipoMovimientos.Todos).ToList();            

            DescripcionesAsientos = TablasVariasService.GetListDescripcionAsientos(context);
        }

        #endregion

        #region Properties

        public InternalConfiguracionModel Model
        {
            get { return _model; }
            set { _model = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Materialentradaigualsalida")]
        public bool Materialentradaigualsalida
        {
            get { return Model.Materialentradaigualsalida; }
            set { Model.Materialentradaigualsalida = value; }
        }

        [Required]
        public int Id
        {
            get; set; }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Fkidioma1")]
        public string Fkidioma1
        {
            get { return Model.Fkidioma1; }
            set { Model.Fkidioma1 = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Fkidioma2")]
        public string Fkidioma2
        {
            get { return Model.Fkidioma2; }
            set { Model.Fkidioma2 = value; }
        }

        //produccion
        [Display(ResourceType = typeof(RConfiguracion), Name = "Espesorfleje")]
        public double Espesorfleje
        {
            get { return Model.Espesorfleje; }
            set { Model.Espesorfleje = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Espesordisco")]
        public double Espesordisco
        {
            get { return Model.Espesordisco; }
            set { Model.Espesordisco = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Formatogrosor")]
        public TipoFormatoGrosor Formatogrosor
        {
            get { return Model.Formatogrosor; }
            set { Model.Formatogrosor = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Formatounidad")]
        public TipoFormatoUnidad Formatounidad
        {
            get { return Model.Formatounidad; }
            set { Model.Formatounidad = value; }
        }

        //presupuestos

        [Display(ResourceType = typeof (RConfiguracion), Name = "Gestionarrevisiones")]
        public bool Gestionarrevisiones
        {
            get { return Model.Gestionarrevisiones; }
            set { Model.Gestionarrevisiones = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadoinicial
        {
            get { return Model.Estadoinicial; }
            set { Model.Estadoinicial = value; }
        }

        

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoparcial")]
        public string Estadoparcial
        {
            get { return Model.Estadoparcial; }
            set { Model.Estadoparcial = value; }
        }

        public List<EstadosModel> Estados { get; set; }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadototal")]
        public string Estadototal
        {
            get { return Model.Estadototal; }
            set { Model.Estadototal = value; }
        }

        //Pedidos
        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadopedidosventasinicial
        {
            get { return Model.Estadopedidosventasinicial; }
            set { Model.Estadopedidosventasinicial = value; }
        }



        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoparcial")]
        public string Estadopedidosventasparcial
        {
            get { return Model.Estadopedidosventasparcial; }
            set { Model.Estadopedidosventasparcial = value; }
        }

        public List<EstadosModel> Estadospedidosventas { get; set; }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadototal")]
        public string Estadopedidosventastotal
        {
            get { return Model.Estadopedidosventastotal; }
            set { Model.Estadopedidosventastotal = value; }
        }

        //Albaranes
        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadoalbaranesventasinicial
        {
            get { return Model.Estadoalbaranesventasinicial; }
            set { Model.Estadoalbaranesventasinicial = value; }
        }
        

        public List<EstadosModel> Estadosalbaranesventas { get; set; }

        [Display(ResourceType = typeof(RConfiguracion), Name = "AlbaranesEstadototal")]
        public string Estadoalbaranesventastotal
        {
            get { return Model.Estadoalbaranesventastotal; }
            set { Model.Estadoalbaranesventastotal = value; }
        }

        //Facturas
        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadofacturasventasinicial
        {
            get { return Model.Estadofacturasventasinicial; }
            set { Model.Estadofacturasventasinicial = value; }
        }


        public List<EstadosModel> Estadosfacturasventas { get; set; }


        [Display(ResourceType = typeof(RConfiguracion), Name = "AlbaranesEstadototal")]
        public string Estadofacturasventastotal
        {
            get { return Model.Estadofacturasventastotal; }
            set { Model.Estadofacturasventastotal = value; }
        }

        //Reservas
        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadoreservasinicial
        {
            get { return Model.Estadoreservasinicial; }
            set { Model.Estadoreservasinicial = value; }
        }



        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoentregada")]
        public string Estadoreservasparcial
        {
            get { return Model.Estadoreservasparcial; }
            set { Model.Estadoreservasparcial = value; }
        }

        public List<EstadosModel> Estadosreservas { get; set; }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoliberada")]
        public string Estadoreservastotal
        {
            get { return Model.Estadoreservastotal; }
            set { Model.Estadoreservastotal = value; }
        }

        //documentos ventas

        [Display(ResourceType = typeof(RConfiguracion), Name = "VentasUsarCanal")]
        public bool VentasUsarCanal {
            get { return Model.VentasUsarCanal; }
            set { Model.VentasUsarCanal = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "VentasCanalObligatorio")]
        public bool VentasCanalObligatorio {
            get { return Model.VentasCanalObligatorio; }
            set { Model.VentasCanalObligatorio = value; }
        }

        //documentos compras

        [Display(ResourceType = typeof(RConfiguracion), Name = "ComprasUsarCanal")]
        public bool ComprasUsarCanal {
            get { return Model.ComprasUsarCanal; }
            set { Model.ComprasUsarCanal = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "ComprasCanalObligatorio")]
        public bool ComprasCanalObligatorio
        {
            get { return Model.ComprasCanalObligatorio; }
            set { Model.ComprasCanalObligatorio = value; }
        }


        //comisiones

        [Display(ResourceType = typeof(RConfiguracion), Name = "Descontardescuentocomercial")]
        public bool Descontardescuentocomercial
        {
            get { return Model.Descontardescuentocomercial; }
            set { Model.Descontardescuentocomercial = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Descontardescuentoprontopago")]
        public bool Descontardescuentoprontopago
        {
            get { return Model.Descontardescuentoprontopago; }
            set { Model.Descontardescuentoprontopago = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Descontarrecargofinanciero")]
        public bool Descontarrecargofinanciero
        {
            get { return Model.Descontarrecargofinanciero; }
            set { Model.Descontarrecargofinanciero = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Fksituacioncomisioncrear")]
        public string Fksituacioncomisioncrear
        {
            get { return Model.Fksituacioncomisioncrear; }
            set { Model.Fksituacioncomisioncrear = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Fksituacioncomisionliquidar")]
        public string Fksituacioncomisionliquidar
        {
            get { return Model.Fksituacioncomisionliquidar; }
            set { Model.Fksituacioncomisionliquidar = value; }
        }

        //end comisiones

        //criterios agrupacion
        [Display(ResourceType = typeof(RCriteriosagrupacion), Name = "TituloEntidadSingular")]
        public string Fkcriterioagrupacion
        {
            get { return Model.Fkcriterioagrupacion; }
            set { Model.Fkcriterioagrupacion = value; }
        }

        //end criterios agrupacion

        // compras

        //Presupuestos
        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadopresupuestoscomprasinicial
        {
            get { return Model.Estadopresupuestoscomprasinicial; }
            set { Model.Estadopresupuestoscomprasinicial = value; }
        }


        public List<EstadosModel> Estadospresupuestoscompras { get; set; }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoparcial")]
        public string Estadopresupuestoscomprasparcial
        {
            get { return Model.Estadopresupuestoscomprasparcial; }
            set { Model.Estadopresupuestoscomprasparcial = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadototal")]
        public string Estadopresupuestoscomprastotal
        {
            get { return Model.Estadopresupuestoscomprastotal; }
            set { Model.Estadopresupuestoscomprastotal = value; }
        }

        //end presupuestos compras

        //Pedidos
        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadopedidoscomprasinicial
        {
            get { return Model.Estadopedidoscomprasinicial; }
            set { Model.Estadopedidoscomprasinicial = value; }
        }


        public List<EstadosModel> Estadospedidoscompras { get; set; }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoparcial")]
        public string Estadopedidoscomprasparcial
        {
            get { return Model.Estadopedidoscomprasparcial; }
            set { Model.Estadopedidoscomprasparcial = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadototal")]
        public string Estadopedidoscomprastotal
        {
            get { return Model.Estadopedidoscomprastotal; }
            set { Model.Estadopedidoscomprastotal = value; }
        }

        //end pedidos compras

        //Albaranes 
        //Albaranes
        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadoalbaranescomprasinicial
        {
            get { return Model.Estadoalbaranescomprasinicial; }
            set { Model.Estadoalbaranescomprasinicial = value; }
        }


        public List<EstadosModel> Estadosalbaranescompras { get; set; }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadototal")]
        public string Estadoalbaranescomprastotal
        {
            get { return Model.Estadoalbaranescomprastotal; }
            set { Model.Estadoalbaranescomprastotal = value; }
        }
        //end albaranes comras

        //Traspasos
        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadotraspasosalmaceninicial
        {
            get { return Model.Estadotraspasosalmaceninicial; }
            set { Model.Estadotraspasosalmaceninicial = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoparcial")]
        public string Estadotraspasosalmacenparcial
        {
            get { return Model.Estadotraspasosalmacenparcial; }
            set { Model.Estadotraspasosalmacenparcial = value; }
        }

        public List<EstadosModel> Estadostraspasosalmacen { get; set; }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadototal")]
        public string Estadotraspasosalmacentotal
        {
            get { return Model.Estadotraspasosalmacentotal; }
            set { Model.Estadotraspasosalmacentotal = value; }
        }
        //end Traspasos comras

        //facturas compras

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadofacturascomprasinicial
        {
            get { return Model.Estadofacturascomprasinicial; }
            set { Model.Estadofacturascomprasinicial = value; }
        }


        public List<EstadosModel> Estadosfacturascompras { get; set; }


        [Display(ResourceType = typeof(RConfiguracion), Name = "AlbaranesEstadototal")]
        public string Estadofacturascomprastotal
        {
            get { return Model.Estadofacturascomprastotal; }
            set { Model.Estadofacturascomprastotal = value; }

        }

        [Display(ResourceType = typeof (RConfiguracion), Name = "Margenfacturacompra")]
        public double? Margenfacturacompra
        {
            get { return Model.Margenfacturacompra; }
            set { Model.Margenfacturacompra = value??0; }
        }

        //end facturas de compras

        //descripcion asientos facturas
        public IEnumerable<TablasVariasGeneralModel> DescripcionesAsientos { get; set; }

        [Display(ResourceType = typeof(Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs), Name = "Descripcionasiento")]
        public string DescripcionAsientoFacturaVenta
        {
            get { return Model.DescripcionAsientoFacturaVenta; }
            set { Model.DescripcionAsientoFacturaVenta = value; }
        }

        [Display(ResourceType = typeof(Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movs), Name = "Descripcionasiento")]
        public string DescripcionAsientoFacturaCompra
        {
            get { return Model.DescripcionAsientoFacturaCompra; }
            set { Model.DescripcionAsientoFacturaCompra = value; }
        }

        //CRM
        public List<EstadosModel> Estadosoportunidades { get; set; }
        public List<EstadosModel> Estadosproyectos { get; set; }
        public List<EstadosModel> Estadoscampañas { get; set; }
        public List<EstadosModel> Estadosincidencias { get; set; }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadooportunidadesinicial
        {
            get { return Model.Estadooportunidadesinicial; }
            set { Model.Estadooportunidadesinicial = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadofinal")]
        public string Estadooportunidadestotal
        {
            get { return Model.Estadooportunidadestotal; }
            set { Model.Estadooportunidadestotal = value; }
        }


        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadoproyectosinicial
        {
            get { return Model.Estadoproyectosinicial; }
            set { Model.Estadoproyectosinicial = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadofinal")]
        public string Estadoproyectostotal
        {
            get { return Model.Estadoproyectostotal; }
            set { Model.Estadoproyectostotal = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadocampañasinicial
        {
            get { return Model.Estadocampañasinicial; }
            set { Model.Estadocampañasinicial = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadofinal")]
        public string Estadocampañastotal
        {
            get { return Model.Estadocampañastotal; }
            set { Model.Estadocampañastotal = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadoinicial")]
        public string Estadoincidenciasinicial
        {
            get { return Model.Estadoincidenciasinicial; }
            set { Model.Estadoincidenciasinicial = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "Estadofinal")]
        public string Estadoincidenciastotal
        {
            get { return Model.Estadoincidenciastotal; }
            set { Model.Estadoincidenciastotal = value; }
        }

        //Tesorería
        [Display(ResourceType = typeof(RConfiguracion), Name = "Cancelacionriesgoremesa")]
        public int Cancelacionriesgoremesa
        {
            get { return Model.Cancelacionriesgoremesa; }
            set { Model.Cancelacionriesgoremesa = value; }
        }

        //Contabilidad
        [Display(ResourceType = typeof(RConfiguracion), Name = "Invertirasiento")]
        public bool Invertirasiento
        {
            get { return Model.Invertirasiento; }
            set { Model.Invertirasiento = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioExistenciasIniciales")]
        public string ComentarioExistenciasIniciales
        {
            get { return Model.ComentarioExistenciasIniciales; }
            set { Model.ComentarioExistenciasIniciales = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioExistenciasFinales")]
        public string ComentarioExistenciasFinales
        {
            get { return Model.ComentarioExistenciasFinales; }
            set { Model.ComentarioExistenciasFinales = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioDebePYG")]
        public string ComentarioDebePYG
        {
            get { return Model.ComentarioDebePYG; }
            set { Model.ComentarioDebePYG = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioHaberPYG")]
        public string ComentarioHaberPYG
        {
            get { return Model.ComentarioHaberPYG; }
            set { Model.ComentarioHaberPYG = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioCuentasDetalle")]
        public string ComentarioCuentasDetalle
        {
            get { return Model.ComentarioCuentasDetalle; }
            set { Model.ComentarioCuentasDetalle = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioCierreEjercicio")]
        public string ComentarioCierreEjercicio
        {
            get { return Model.ComentarioCierreEjercicio; }
            set { Model.ComentarioCierreEjercicio = value; }
        }

        [Display(ResourceType = typeof(RConfiguracion), Name = "ComentarioAperturaEjercicio")]
        public string ComentarioAperturaEjercicio
        {
            get { return Model.ComentarioAperturaEjercicio; }
            set { Model.ComentarioAperturaEjercicio = value; }
        }

        public int Cargadatos { get; set; }

        #endregion


        public IEnumerable<CriteriosagrupacionModel> GetCriteriosAgrupacion()
        {
            var service = FService.Instance.GetService(typeof(CriteriosagrupacionModel), Context);
            return service.getAll().Select(f => (CriteriosagrupacionModel)f);
        }

        [IgnoreDataMember]
        public override IEnumerable<PropertyInfo> primaryKey
        {
            get { return base.primaryKey; }
            protected set
            {
                base.primaryKey = value;
            }
        }

        public override object generateId(string id)
        {
            return Funciones.Qint(id) ?? 0;
        }

        public override string DisplayName => RConfiguracion.TituloEntidad;
        
    }
            
        

    [Serializable]
    public class InternalConfiguracionModel
    {
        private bool _gestionarrevisiones=true;

        //general
        public string Fkidioma1 { get; set; }
        public string Fkidioma2 { get; set; }

        //produccion
        public double Espesorfleje { get; set; }
        public double Espesordisco { get; set; }
        public TipoFormatoGrosor Formatogrosor { get; set; }
        public TipoFormatoUnidad Formatounidad { get; set; }
        
        public bool VentasUsarCanal { get; set; }
        public bool VentasCanalObligatorio { get; set; }

        //documentos compras
        public bool ComprasUsarCanal { get; set; }
        public bool ComprasCanalObligatorio { get; set; }

        //presupuestos
        public bool Gestionarrevisiones
        {
            get { return _gestionarrevisiones; }
            set { _gestionarrevisiones = value; }
        }
        
        public string Estadoinicial { get; set; }
        public string Estadoparcial { get; set; }
        public string Estadototal { get; set; }

        public string Estadopedidosventasinicial { get; set; }
        public string Estadopedidosventasparcial { get; set; }
        public string Estadopedidosventastotal { get; set; }

        public string Estadoalbaranesventasinicial { get; set; }
        public string Estadoalbaranesventastotal { get; set; }

        public string Estadofacturasventastotal { get; set; }
        public string Estadofacturasventasinicial { get; set; }

        public bool Descontardescuentocomercial { get; set; }
        public bool Descontardescuentoprontopago { get; set; }
        public bool Descontarrecargofinanciero { get; set; }
        public string Fksituacioncomisioncrear { get; set; }
        public string Fksituacioncomisionliquidar { get; set; }
        public string Fkcriterioagrupacion { get; set; }

        public string Estadopresupuestoscomprasinicial { get; set; }
        public string Estadopresupuestoscomprasparcial { get; set; }
        public string Estadopresupuestoscomprastotal { get; set; }

        public string Estadopedidoscomprasinicial { get; set; }
        public string Estadopedidoscomprasparcial { get; set; }
        public string Estadopedidoscomprastotal { get; set; }

        public string Estadofacturascomprastotal { get; set; }
        public string Estadofacturascomprasinicial { get; set; }

        public string Estadoalbaranescomprasinicial { get; set; }
        public string Estadoalbaranescomprastotal { get; set; }

        public string Estadoreservasinicial { get; set; }
        public string Estadoreservasparcial { get; set; }
        public string Estadoreservastotal { get; set; }

        public string Estadotraspasosalmaceninicial { get; set; }
        public string Estadotraspasosalmacenparcial { get; set; }
        public string Estadotraspasosalmacentotal { get; set; }
        public double Margenfacturacompra { get; set; }
        public bool Materialentradaigualsalida { get; set; }

        public string DescripcionAsientoFacturaVenta { get; set; }
        public string DescripcionAsientoFacturaCompra { get; set; }

        public string Estadooportunidadesinicial { get; set; }
        public string Estadooportunidadestotal { get; set; }

        public string Estadoproyectosinicial { get; set; }
        public string Estadoproyectostotal { get; set; }

        public string Estadocampañasinicial { get; set; }
        public string Estadocampañastotal { get; set; }

        public string Estadoincidenciasinicial { get; set; }
        public string Estadoincidenciastotal { get; set; }

        public int Cancelacionriesgoremesa { get; set; }
        public bool Invertirasiento { get; set; }

        public string ComentarioExistenciasIniciales { get; set; }
        public string ComentarioExistenciasFinales { get; set; }
        public string ComentarioDebePYG { get; set; }
        public string ComentarioHaberPYG { get; set; }
        public string ComentarioCuentasDetalle { get; set; }
        public string ComentarioCierreEjercicio { get; set; }
        public string ComentarioAperturaEjercicio { get; set; }
    }
}
