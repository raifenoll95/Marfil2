using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RPresupuestos=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Presupuestos;

namespace Marfil.Dom.Persistencia.Model.Documentos.Presupuestos
{
    public class PresupuestosModel : BaseModel<PresupuestosModel, Persistencia.Presupuestos>, IDocument, IGaleria
    {
        private List<PresupuestosLinModel> _lineas = new List<PresupuestosLinModel>();
        private List<PresupuestosTotalesModel> _totales = new List<PresupuestosTotalesModel>();
        private List<PresupuestosComponentesLinModel> _componentes = new List<PresupuestosComponentesLinModel>();

        #region Properties

        public int? Id { get; set; }

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Fechadocumento")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Fechadocumento { get; set; }

        public string Fechadocumentocadena
        {
            get { return Fechadocumento?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? string.Empty; }
        }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fechavalidez")]
        public DateTime? Fechavalidez { get; set; }
        public string Fechavalidezcadena
        {
            get { return Fechavalidez?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? string.Empty; }
        }
        [Display(ResourceType = typeof(RPresupuestos), Name = "Fechaentrega")]
        public DateTime? Fechaentrega { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fecharevision")]
        public DateTime? Fecharevision { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkalmacen")]
        public int? Fkalmacen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkclientes")]
        public string Fkclientes { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Nombrecliente")]
        public string Nombrecliente { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Clientedireccion")]
        public string Clientedireccion { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Clientepoblacion")]
        public string Clientepoblacion { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Clientecp")]
        public string Clientecp { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Clientepais")]
        public string Clientepais { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Clienteprovincia")]
        public string Clienteprovincia { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Clientetelefono")]
        public string Clientetelefono { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Clientefax")]
        public string Clientefax { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Clienteemail")]
        public string Clienteemail { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Clientenif")]
        public string Clientenif { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkagentes")]
        public string Fkagentes { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkagentes")]
        public string Nombreagentes { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkcomerciales")]
        public string Fkcomerciales { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkcomerciales")]
        public string Nombrecomercial { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Comisionagente")]
        public double? Comisionagente { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Comisioncomercial")]
        public double? Comisioncomercial { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Cambioadicional")]
        public double? Cambioadicional { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Estado")]
        public string Fkestados { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.PresupuestosVentas, Tipoestado(Context));
                }
            }
        }

        [XmlIgnore]
        [IgnoreDataMember]
        public EstadosModel Estado
        {
            get
            {
                if (!string.IsNullOrEmpty(Fkestados))
                {
                    using (var estadosService = FService.Instance.GetService(typeof(EstadosModel), Context))
                    {
                        return estadosService.get(Fkestados) as EstadosModel;
                    }
                }
                return null;
            }
        }


        [Display(ResourceType = typeof(RPresupuestos), Name = "Importebruto")]
        public double? Importebruto { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Importebaseimponible")]
        public double? Importebaseimponible { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Importeportes")]
        public double? Importeportes { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Importetotaldoc")]
        public double? Importetotaldoc { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Importetotalmonedabase")]
        public double? Importetotalmonedabase { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkobras")]
        public string Fkobras { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Incoterm")]
        public string Incoterm { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Descripcionincoterm")]
        public string Descripcionincoterm { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Peso")]
        public int Peso { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Confianza")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public int Confianza { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Costemateriales")]
        public double? Costemateriales { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Tiempooficinatecnica")]
        public double? Tiempooficinatecnica { get; set; }

        public int Decimalesmonedas { get; set; }

        public string Totaldocumento
        {
            get { return (Importetotaldoc ?? 0.0).ToString("N" + Decimalesmonedas); }
        }

        [Required]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fktransportista")]
        public string Fktransportista { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Tipocambio")]
        public double? Tipocambio { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Unidadnegocio")]
        public string Unidadnegocio { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkbancosmandatos")]
        public string Fkbancosmandatos { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Cartacredito")]
        public string Cartacredito { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Vencimientocartacredito")]
        public DateTime? Vencimientocartacredito { get; set; }

        public string Vencimientocartacreditocadena
        {
            get { return Vencimientocartacredito?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? string.Empty; }
        }

        [Range(0, 99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Contenedores")]
        public int? Contenedores { get; set; }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Clientes), Name = "Cuentatesoreria")]
        public string Fkcuentastesoreria { get; set; }

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        public Guid? Fkcarpetas { get; set; }

        private GaleriaModel _galeria;

        public GaleriaModel Galeria
        {
            get
            {
                _galeria = new GaleriaModel();
                if (Fkcarpetas.HasValue)
                {
                    _galeria.Empresa = Empresa;
                    _galeria.DirectorioId = Fkcarpetas.Value;
                }
                return _galeria;
            }
        }

        #endregion

        #region Líneas

        public List<PresupuestosLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region Totales

        public List<PresupuestosTotalesModel> Totales
        {
            get { return _totales; }
            set { _totales = value; }
        }

        #endregion

        #region Componentes

        public List<PresupuestosComponentesLinModel> Componentes
        {
            get { return _componentes; }
            set { _componentes = value; }
        }

        #endregion

        #region CTR

        public PresupuestosModel()
        {

        }

        public PresupuestosModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
        }

        public override string GetPrimaryKey()
        {
            return Id.ToString();
        }

        public override string DisplayName => RPresupuestos.TituloEntidad;
        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.PresupuestosVentas.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(db);
                {
                    var lst =
                        service.GetDocumentosParaImprimir(TipoDocumentoImpresion.PresupuestosVentas, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.PresupuestosVentas,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }
        }

        public TipoEstado Tipoestado(IContextService context)
        {
            if (!string.IsNullOrEmpty(Fkestados))
            {
                using (var estadosService = FService.Instance.GetService(typeof(EstadosModel), Context))
                {
                    var estadoObj = estadosService.get(Fkestados) as EstadosModel;
                    return estadoObj?.Tipoestado ?? TipoEstado.Diseño;
                }
            }
            return TipoEstado.Diseño;
        }
    }

    public class PresupuestosLinModel : ILineasDocumentosBusquedaMovil
    {
        private int? _decimalesmonedas = 2;
        private int? _decimalesmedidas = 3;
        
        public int Id { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Idlineaarticulo")]
        public int? Intaux { get; set; }

        private int? _orden;

        public int? Decimalesmonedas
        {
            get { return _decimalesmonedas; }
            set { _decimalesmonedas = value; }
        }

        public int? Decimalesmedidas
        {
            get { return _decimalesmedidas; }
            set { _decimalesmedidas = value; }
        }

        [Required]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [MaxLength(120, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [MaxLength(12, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Tabla")]
        public int? Tabla { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Canal")]
        public string Canal { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Cantidadpedida")]
        public double? Cantidadpedida { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Largo")]
        public double? Largo { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Ancho")]
        public double? Ancho { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Grueso")]
        public double? Grueso { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Largo")]
        public string SLargo
        {
            get { return (Largo ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Largo = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Ancho")]
        public string SAncho
        {
            get { return (Ancho ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Ancho = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Grueso")]
        public string SGrueso
        {
            get { return (Grueso ?? 0.0).ToString("N" + Decimalesmedidas, CultureInfo.CurrentUICulture); }
            set { Grueso = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Metros")]
        public string SMetros
        {
            get { return (Metros ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Metros = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Precio")]
        public string SPrecio
        {
            get { return (Precio ?? 0.0).ToString(); }
            set { Precio = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Porcentajedescuento")]
        public double? Porcentajedescuento { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Importedescuento")]
        public double? Importedescuento { get; set; }

        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Cuotarecargoequivalencia")]
        public double? Cuotarecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "ImporteBase")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RPresupuestos), Name = "ImporteBase")]
        public string SImporte
        {
            get { return (Importe ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestos), Name = "Notas")]
        public string Notas { get; set; }



        [Display(ResourceType = typeof(RPresupuestos), Name = "Precioanterior")]
        public double? Precioanterior { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPresupuestos), Name = "Revision")]
        public string Revision { get; set; }

        public bool EnPedido { get; set; }

        public List<StDocumentoReferencia> Fkpedidoreferencia { get; set; }

        public int? Orden
        {
            get { return _orden ?? Id * ApplicationHelper.EspacioOrdenLineas; }
            set { _orden = value; }
        }

        public Guid? Integridadreferenciaflag { get; set; }
    }

        public class PresupuestosLinImportarModel : PresupuestosLinModel
        {
            public int Fkpresupuestos { get; set; }
            public string Fkpresupuestosreferencia { get; set; }
            public int? Fkmonedas { get; set; }
            public PresupuestosModel Cabecera { get; set; }

        }

        public struct StDocumentoReferencia
        {
            public string Referencia { get; set; }
            public string CampoId { get; set; }
        }

        public class PresupuestosTotalesModel : ITotalesDocumentosBusquedaMovil
        {
            public int? Decimalesmonedas { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Fktiposiva")]
            public string Fktiposiva { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Porcentajeiva")]
            public double? Porcentajeiva { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "BrutoTotal")]
            public double? Brutototal { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "BrutoTotal")]
            public string SBrutototal
            {
                get { return (Brutototal ?? 0.0).ToString("N" + Decimalesmonedas); }
                set { Brutototal = Funciones.Qdouble(value); }
            }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Basetotal")]
            public double? Baseimponible { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Basetotal")]
            public string SBaseimponible
            {
                get { return (Baseimponible ?? 0.0).ToString("N" + Decimalesmonedas); }
                set { Baseimponible = Funciones.Qdouble(value); }
            }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Cuotaiva")]
            public double? Cuotaiva { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Cuotaiva")]
            public string SCuotaiva
            {
                get { return (Cuotaiva ?? 0.0).ToString("N" + Decimalesmonedas); }
                set { Cuotaiva = Funciones.Qdouble(value); }
            }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Porcentajerecargoequivalencia")]
            public double? Porcentajerecargoequivalencia { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Cuotarecargoequivalencia")]
            public double? Importerecargoequivalencia { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Cuotarecargoequivalencia")]
            public string SImporterecargoequivalencia
            {
                get { return (Importerecargoequivalencia ?? 0.0).ToString("N" + Decimalesmonedas); }
                set { Importerecargoequivalencia = Funciones.Qdouble(value); }
            }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Porcentajedescuentoprontopago")]
            public double? Porcentajedescuentoprontopago { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Importedescuentoprontopago")]
            public double? Importedescuentoprontopago { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Importedescuentoprontopago")]
            public string SImportedescuentoprontopago
            {
                get { return (Importedescuentoprontopago ?? 0.0).ToString("N" + Decimalesmonedas); }
                set { Importedescuentoprontopago = Funciones.Qdouble(value); }
            }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Porcentajedescuentocomercial")]
            public double? Porcentajedescuentocomercial { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Importedescuentocomercial")]
            public double? Importedescuentocomercial { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Importedescuentocomercial")]
            public string SImportedescuentocomercial
            {
                get { return (Importedescuentocomercial ?? 0.0).ToString("N" + Decimalesmonedas); }
                set { Importedescuentocomercial = Funciones.Qdouble(value); }
            }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Subtotal")]
            public double? Subtotal { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Subtotal")]
            public string SSubtotal
            {
                get { return (Subtotal ?? 0.0).ToString("N" + Decimalesmonedas); }
                set { Subtotal = Funciones.Qdouble(value); }
            }
        }

        public class ToolbarPresupuestosAsistenteModel : ToolbarModel
        {
            public ToolbarPresupuestosAsistenteModel()
            {
                Operacion = TipoOperacion.Custom;
                Titulo = RPresupuestos.AsistenteFabricacion;
            }

            public override string GetCustomTexto()
            {
                return RPresupuestos.AsistenteFabricacionComponentes;
            }
        }


        public class PresupuestosAsistenteModel : IToolbar
        {
            private ToolbarModel _toolbar;

            public ToolbarModel Toolbar
            {
                get { return _toolbar; }
                set { _toolbar = value; }
            }

            public PresupuestosAsistenteModel(IContextService context)
            {
                _toolbar = new ToolbarPresupuestosAsistenteModel();
            }

            public PresupuestosAsistenteModel()
            {
            }

            public int Id { get; set; }
            public bool Presupuestado { get; set; }
        }

        public class PresupuestosComponentesLinModel : BaseModel<PresupuestosComponentesLinModel, PresupuestosComponentesLin>
        {
            public PresupuestosComponentesLinModel()
            {
            }

            public PresupuestosComponentesLinModel(IContextService context) : base(context)
            {

            }

            public override string DisplayName => "";

            public override object generateId(string id)
            {
                return id;
            }

            public override void createNewPrimaryKey()
            {
                primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
            }

            public override string GetPrimaryKey()
            {
                return this.Id.ToString();
            }

            public string Empresa { get; set; }

            public int Fkpresupuestos { get; set; }

            //1,2,3,4,5
            public int Id { get; set; }

            //Id Articulo Componente
            [Display(ResourceType = typeof(RPresupuestos), Name = "IdComponente")]
            public string IdComponente { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Idlineaarticulo")]
            public int? Idlineaarticulo { get; set; }

            //REFERENCIA DEL PADRE (INTEGRIDAD REFERENCIAL)
            public Guid? Integridadreferenciaflag { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "DescripcionLin")]
            public string Descripcioncomponente { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "PiezasLin")]
            public int? Piezas { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Largo")]
            public double? Largo { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Ancho")]
            public double? Ancho { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Grueso")]
            public double? Grueso { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Merma")]
            public int? Merma { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Subtotal")]
            public double? Precio { get; set; }

            [Display(ResourceType = typeof(RPresupuestos), Name = "Precio")]
            public double? PrecioInicial { get; set; }
        }
    }

