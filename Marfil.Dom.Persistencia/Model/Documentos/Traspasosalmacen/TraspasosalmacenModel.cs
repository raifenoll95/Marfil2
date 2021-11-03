using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RTraspasosalmacen = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Traspasosalmacen;
using RCliente = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
using RCriteriosagrupacion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Criteriosagrupacion;

namespace Marfil.Dom.Persistencia.Model.Documentos.Traspasosalmacen
{

    public class TraspasosalmacenLinVistaModel
    {
        public TipoPieza Tipopieza { get; set; }
        public string Fkarticulos { get; set; }
        public string Fkalmacen { get; set; }
        public string Lote { get; set; }
        public bool Modificarmedidas { get; set; }
        public double Cantidad { get; set; }
        public double Largo { get; set; }
        public double Ancho { get; set; }
        public double Grueso { get; set; }
        public double Metros { get; set; }
        public double Precio { get; set; }
        public double Descuento { get; set; }
        public int Decimalesmonedas { get; set; }
        public string Descuentoprontopago { get; set; }
        public string Descuentocomercial { get; set; }
        public string Portes { get; set; }
        public List<MovimientosstockModel> Lineas { get; set; }
        public string Fkcuenta { get; set; }
        public string Fkmonedas { get; set; }
        public string Fkregimeniva { get; set; }
        public TipoFlujo Flujo { get; set; }
        public int? Caja { get; set; }
        public string Canal { get; set; }
    }

    public class TraspasosalmacenDiarioStockSerializable
    {
        public int Id { get; set; }
        public string Referencia { get; set; }
        public string Fkalmacenorigen { get; set; }
        public TraspasosalmacenLinModel Linea { get; set; }
        public string Fkalmacendestino { get; set; }
    }

    public class TraspasosalmacenModel : BaseModel<TraspasosalmacenModel, Persistencia.Traspasosalmacen>, IDocument, IGaleria
    {
        private List<TraspasosalmacenLinModel> _lineas = new List<TraspasosalmacenLinModel>();
        private List<TraspasosalmacenTotalesModel> _totales = new List<TraspasosalmacenTotalesModel>();
        private List<TraspasosalmacenCostesadicionalesModel> _costes = new List<TraspasosalmacenCostesadicionalesModel>();

        #region Properties

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        public int Id { get; set; }

        public bool? Importado { get; set; }

        public string Empresa { get; set; }

        [Required]
        public Guid? Integridadreferenciaflag { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }


        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fechadocumento")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Fechadocumento { get; set; }

        public string Fechadocumentocadena
        {
            get { return Fechadocumento?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? ""; }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fechavalidez")]
        public DateTime? Fechavalidez { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fechaentrega")]
        public DateTime? Fechaentrega { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fecharevision")]
        public DateTime? Fecharevision { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkalmacen")]
        public string Almacenorigen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkalmacendestino")]
        public string Fkalmacendestino { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkalmacendestino")]
        public string Almacendestino { get; set; }

        
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkproveedores")]
        public string Fkproveedores { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Nombrecliente")]
        public string Nombreproveedor { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Clientedireccion")]
        public string Proveedordireccion { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Clientepoblacion")]
        public string Proveedorpoblacion { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Clientecp")]
        public string Proveedorcp { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Clientepais")]
        public string Proveedorpais { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Clienteprovincia")]
        public string Proveedorprovincia { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Clientetelefono")]
        public string Proveedortelefono { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Clientefax")]
        public string Proveedorfax { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Clienteemail")]
        public string Proveedoremail { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Clientenif")]
        public string Proveedornif { get; set; }
        
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkagentes")]
        public string Fkagentes { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkagentes")]
        public string Nombreagentes { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkcomerciales")]
        public string Fkcomerciales { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkcomerciales")]
        public string Nombrecomercial { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Comisionagente")]
        public double? Comisionagente { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Comisioncomercial")]
        public double? Comisioncomercial { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Cambioadicional")]
        public double? Cambioadicional { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Estado")]
        public string Fkestados { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.Traspasosalmacen, Tipoestado(Context));
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

        [XmlIgnore]
        [IgnoreDataMember]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Estado")]
        public string Estadodescripcion
        {
            get { return Estado?.Descripcion; }
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

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importebruto")]
        public double? Importebruto { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importebaseimponible")]
        public double? Importebaseimponible { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importeportes")]
        public double? Importeportes { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        public string Porcentajedescuentocomercialcadena
        {
            get { return Porcentajedescuentocomercial?.ToString("N2") ?? (0.0).ToString("N2"); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        public string Porcentajedescuentoprontopagocadena
        {
            get { return Porcentajedescuentoprontopago?.ToString("N2") ?? (0.0).ToString("N2"); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importetotaldoc")]
        public double? Importetotaldoc { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importetotalmonedabase")]
        public double? Importetotalmonedabase { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Notas")]
        public string Notas { get; set; }


        [Display(ResourceType = typeof(RCliente), Name = "Tipodeportes")]
        public Tipoportes? Tipodeportes { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Costeportes")]
        public double? Costeportes { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkobras")]
        public string Fkobras { get; set; }

        public string Obradescripcion { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Incoterm")]
        public string Incoterm { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Descripcionincoterm")]
        public string Descripcionincoterm { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Peso")]
        public int Peso { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Confianza")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public int Confianza { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Costemateriales")]
        public double? Costemateriales { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Tiempooficinatecnica")]
        public double? Tiempooficinatecnica { get; set; }

        public int Decimalesmonedas { get; set; }

        public string Totaldocumento
        {
            get { return (Importetotaldoc ?? 0.0).ToString("N" + Decimalesmonedas); }
        }
        
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fktransportista")]
        public string Fktransportista { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Tipocambio")]
        public double? Tipocambio { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();
        private string _fechafactura = DateTime.Now.ToShortDateString().ToString(CultureInfo.InvariantCulture);

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Unidadnegocio")]
        public string Unidadnegocio { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Cartacredito")]
        public string Cartacredito { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Vencimientocartacredito")]
        public DateTime? Vencimientocartacredito { get; set; }

        [Range(0, 99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Contenedores")]
        public int? Contenedores { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkcuentastesoreria")]
        public string Fkcuentastesoreria { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Numerodocumentoproveedor")]
        public string Numerodocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fechadocumentoproveedor")]
        public DateTime? Fechadocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkclientesreserva")]
        public string Fkclientesreserva { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fktiposalbaranes")]
        public int Tipoalbaran { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fktiposalbaranes")]
        public TipoAlbaran Tipoalbaranenum
        {
            get { return (TipoAlbaran)Tipoalbaran; }
            set
            {
                Tipoalbaran = (int)value;
            }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkmotivosdevolucion")]
        public string Fkmotivosdevolucion { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Nombretransportista")]
        public string Nombretransportista { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Conductor")]
        public string Conductor { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Matricula")]
        public string Matricula { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Bultos")]
        public int? Bultos { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Pesoneto")]
        public double? Pesoneto { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Volumen")]
        public double? Volumen { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Envio")]
        public string Envio { get; set; }
        
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkoperarios")]
        public string Fkoperarios { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkoperariostransporte")]
        public string Fkoperariostransporte { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkzonas")]
        public string Fkzonas { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkdireccionfacturacion")]
        public int? Fkdireccionfacturacion { get; set; }

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

        public List<TraspasosalmacenLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region Totales

        public List<TraspasosalmacenTotalesModel> Totales
        {
            get { return _totales; }
            set { _totales = value; }
        }

        #endregion

        #region Costes adicionales

        public List<TraspasosalmacenCostesadicionalesModel> Costes
        {
            get { return _costes; }
            set { _costes = value; }
        }

        #endregion

        #region CTR

        public TraspasosalmacenModel()
        {

        }

        public TraspasosalmacenModel(IContextService context) : base(context)
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

        public override string DisplayName => RTraspasosalmacen.TituloEntidad;
        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.Traspasosalmacen.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(Context, db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.Traspasosalmacen, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.Traspasosalmacen,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }

        }
    }

    public class TraspasosalmacenLinModel
    {
        private int? _decimalesmonedas = 2;
        private int? _decimalesmedidas = 3;
        private int? _orden;
        private bool _enFactura = false;
        private string _loteautomaticoid;

        public int Id { get; set; }
        public bool Nueva { get; set; }
        public string Fkcontadoreslotes { get; set; }
        public int Lotenuevocontador { get; set; }
        public int Orden
        {
            get { return _orden ?? (Id * ApplicationHelper.EspacioOrdenLineas); }
            set { _orden = value; }
        }

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
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [MaxLength(120, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Descripcion")]
        public string Descripcion { get; set; }

        public string Loteautomaticoid
        {
            get { return _loteautomaticoid?.ToLower(); }
            set { _loteautomaticoid = value; }
        }

        [MaxLength(12, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Tabla")]
        public int? Tabla { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Canal")]
        public string Canal { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Cantidadpedida")]
        public double? Cantidadpedida { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Largo")]
        public double? Largo { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Ancho")]
        public double? Ancho { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Grueso")]
        public double? Grueso { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Largo")]
        public string SLargo
        {
            get { return (Largo ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Largo = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Ancho")]
        public string SAncho
        {
            get { return (Ancho ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Ancho = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Grueso")]
        public string SGrueso
        {
            get { return (Grueso ?? 0.0).ToString("N" + Decimalesmedidas, CultureInfo.CurrentUICulture); }
            set { Grueso = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Metros")]
        public string SMetros
        {
            get { return (Metros ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Metros = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Precio")]
        public string SPrecio
        {
            get { return (Precio ?? 0.0).ToString("N" + Decimalesmonedas, CultureInfo.CurrentUICulture); }
            set { Precio = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Porcentajedescuento")]
        public double? Porcentajedescuento { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importedescuento")]
        public double? Importedescuento { get; set; }

        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Cuotarecargoequivalencia")]
        public double? Cuotarecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "ImporteBase")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "ImporteBase")]
        public string SImporte
        {
            get { return (Importe ?? 0.0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Notas")]
        public string Notas { get; set; }



        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Precioanterior")]
        public double? Precioanterior { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Revision")]
        public string Revision { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Contenedor")]
        public string Contenedor { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Sello")]
        public string Sello { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Caja")]
        public int? Caja { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Pesoneto")]
        public double? Pesoneto { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Bundle")]
        public string Bundle { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Seccion")]
        public string Seccion { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importenetolinea")]
        public double? Importenetolinea { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Costeadicionalmaterial")]
        public double? Costeadicionalmaterial { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Costeadicionalportes")]
        public double? Costeadicionalportes { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Costeadicionalotro")]
        public double? Costeadicionalotro { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Costeadicionalvariable")]
        public double? Costeadicionalvariable { get; set; }

        //clave ajena
        public int? Fkpedidos { get; set; }
        public int? Fkpedidosid { get; set; }
        public string Fkpedidosreferencia { get; set; }

        public int? Tblnum { get; set; }

        public int? Lineaasociada { get; set; }

        public List<StDocumentoReferencia> Fkfacturasreferencia { get; set; }

        public bool EnFactura
        {
            get { return _enFactura; }
            set { _enFactura = value; }
        }

        public Guid Flagidentifier { get; set; }
    }

    public class TraspasosalmacenTotalesModel
    {
        public int? Decimalesmonedas { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "BrutoTotal")]
        public double? Brutototal { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "BrutoTotal")]
        public string SBrutototal
        {
            get { return (Brutototal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Brutototal = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Basetotal")]
        public double? Baseimponible { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Basetotal")]
        public string SBaseimponible
        {
            get { return (Baseimponible ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Baseimponible = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Cuotaiva")]
        public string SCuotaiva
        {
            get { return (Cuotaiva ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Cuotaiva = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Cuotarecargoequivalencia")]
        public double? Importerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Cuotarecargoequivalencia")]
        public string SImporterecargoequivalencia
        {
            get { return (Importerecargoequivalencia ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importerecargoequivalencia = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importedescuentoprontopago")]
        public string SImportedescuentoprontopago
        {
            get { return (Importedescuentoprontopago ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentoprontopago = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importedescuentocomercial")]
        public string SImportedescuentocomercial
        {
            get { return (Importedescuentocomercial ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentocomercial = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Subtotal")]
        public double? Subtotal { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Subtotal")]
        public string SSubtotal
        {
            get { return (Subtotal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Subtotal = Funciones.Qdouble(value); }
        }
    }

    public class TraspasosalmacenCostesadicionalesModel
    {
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Tipodocumento")]
        public TipoCosteAdicional Tipodocumento { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importe")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Importe")]
        public string SImporte
        {
            get { return Importe?.ToString("N2") ?? 0.ToString("N2"); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Required]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Porcentaje")]
        [Range(0.01, 100.0, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double? Porcentaje { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Total")]
        public double? Total { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Total")]
        public string STotal
        {
            get { return Total?.ToString("N2") ?? 0.ToString("N2"); }
            set { Total = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Tipocoste")]
        public TipoCoste Tipocoste { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Tiporeparto")]
        public TipoReparto Tiporeparto { get; set; }

        [Display(ResourceType = typeof(RTraspasosalmacen), Name = "Notas")]
        public string Notas { get; set; }
    }

}
