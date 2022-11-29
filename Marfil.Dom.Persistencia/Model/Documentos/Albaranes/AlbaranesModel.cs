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
using RAlbaranes=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RPedidos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Pedidos;
using RCliente = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RCriteriosagrupacion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Criteriosagrupacion;


using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;

namespace Marfil.Dom.Persistencia.Model.Documentos.Albaranes
{
    public enum ModoAlbaran
    {
        Sinstock,
        Constock
    }

    public enum TipoAlbaran
    {
        [StringValue(typeof(RAlbaranes),"TipoAlbaranHabitual")]
        Habitual,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranAutocomsumo")]
        Autocomsumo,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranDevolucion")]
        Devolucion,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranReclamacion")]
        Reclamacion,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranImputacionMateriales")]
        ImputacionMateriales,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranSalidaRegularizacion")]
        SalidaRegularizacion,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranRoturas")]
        Roturas,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranMuestras")]
        Muestras,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranSalidaRegularizacion")]
        SalidaMaterialElaborado,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranEntradaRegularizacion")]
        EntradaRegularizacion,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranMercanciasConsigna")]
        MercanciasConsigna,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranMaterialElaborarTercero")]
        MaterialElaborarTerceros,
        [StringValue(typeof(RAlbaranes), "TipoAlbaranVarios")]
        VariosAlmacen    
    }

    public class AlbaranesLinVistaModel
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

        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

    }

    public partial class AlbaranesLinSerialized
    {
        public string empresa { get; set; }
        public int fkalbaranes { get; set; }
        public int id { get; set; }
        public string fkarticulos { get; set; }
        public string descripcion { get; set; }
        public string lote { get; set; }
        public Nullable<int> tabla { get; set; }
        public Nullable<double> cantidad { get; set; }
        public Nullable<double> cantidadpedida { get; set; }
        public Nullable<double> largo { get; set; }
        public Nullable<double> ancho { get; set; }
        public Nullable<double> grueso { get; set; }
        public string fkunidades { get; set; }
        public Nullable<double> metros { get; set; }
        public Nullable<double> precio { get; set; }
        public Nullable<double> porcentajedescuento { get; set; }
        public Nullable<double> importedescuento { get; set; }
        public string fktiposiva { get; set; }
        public Nullable<double> porcentajeiva { get; set; }
        public Nullable<double> cuotaiva { get; set; }
        public Nullable<double> porcentajerecargoequivalencia { get; set; }
        public Nullable<double> cuotarecargoequivalencia { get; set; }
        public Nullable<double> importe { get; set; }
        public Nullable<double> importenetolinea { get; set; }
        public string notas { get; set; }
        public string documentoorigen { get; set; }
        public string documentodestino { get; set; }
        public string canal { get; set; }
        public Nullable<double> precioanterior { get; set; }
        public string revision { get; set; }
        public Nullable<int> decimalesmonedas { get; set; }
        public Nullable<int> decimalesmedidas { get; set; }
        public string bundle { get; set; }
        public Nullable<int> tblnum { get; set; }
        public string contenedor { get; set; }
        public string sello { get; set; }
        public Nullable<int> caja { get; set; }
        public Nullable<double> pesoneto { get; set; }
        public Nullable<double> pesobruto { get; set; }
        public string seccion { get; set; }
        public Nullable<double> costeadicionalmaterial { get; set; }
        public Nullable<double> costeadicionalportes { get; set; }
        public Nullable<double> costeadicionalotro { get; set; }
        public Nullable<double> costeacicionalvariable { get; set; }
        public Nullable<int> orden { get; set; }
        public Nullable<int> fkpedidos { get; set; }
        public Nullable<int> fkpedidosid { get; set; }
        public string fkpedidosreferencia { get; set; }
        public System.Guid flagidentifier { get; set; }
        public Nullable<int> tipoalmacenlote { get; set; }
    }

        public class AlbaranesDiarioStockSerializable
    {
        public int Id { get; set; }
        public string Referencia { get; set; }
        public DateTime? Fechadocumento { get; set; }
        public string Codigocliente { get; set; }
        public AlbaranesLinSerialized Linea { get; set; }        
    }

    public class AlbaranesModel : BaseModel<AlbaranesModel, Persistencia.Albaranes>, IDocument, IGaleria
    {
        private List<AlbaranesLinModel> _lineas = new List<AlbaranesLinModel>();
        private List<AlbaranesTotalesModel> _totales = new List<AlbaranesTotalesModel>();
        private GaleriaModel _galeria;
        private ModoAlbaran _modo = ModoAlbaran.Sinstock;

        private TipoAlbaran _tipoalbaran = TipoAlbaran.Habitual;


        #region Properties

        public bool? Pedidosaldado { get; set; }

        public ModoAlbaran Modo
        {
            get { return _modo; }
            set { _modo = value; }
        }


        public TipoAlbaran Tipo
        {
            get { return _tipoalbaran; }
            set { _tipoalbaran = value; }
        }

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        public int idOriginalReclamado { get; set; }

        public int Id { get; set; }

        public bool? Importado { get; set; }
        
        public string Empresa { get; set; }


        [Required]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Referencia")]
        public string Referencia { get; set; }

        private DateTime? _fechadocumento = DateTime.Now;
        [Required]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Fechadocumento")]
        public DateTime? Fechadocumento { //get; set; }
            get { return _fechadocumento; }
            set { _fechadocumento = value; }
        }

        public string Fechadocumentocadena
        {
            get { return Fechadocumento?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? ""; }
        }








        [Display(ResourceType = typeof(RAlbaranes), Name = "Fechavalidez")]
        public DateTime? Fechavalidez { get; set; }
        [Display(ResourceType = typeof(RAlbaranes), Name = "Fechabl")]
        public DateTime? Fechabl { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fechaentrega")]
        public DateTime? Fechaentrega { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fecharevision")]
        public DateTime? Fecharevision { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkclientes")]
        public string Fkclientes { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Nombrecliente")]
        public string Nombrecliente { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Clientedireccion")]
        public string Clientedireccion { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Clientepoblacion")]
        public string Clientepoblacion { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Clientecp")]
        public string Clientecp { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Clientepais")]
        public string Clientepais { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Clienteprovincia")]
        public string Clienteprovincia { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Clientetelefono")]
        public string Clientetelefono { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Clientefax")]
        public string Clientefax { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Clienteemail")]
        public string Clienteemail { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Clientenif")]
        public string Clientenif { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkagentes")]
        public string Fkagentes { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkagentes")]
        public string Nombreagentes { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkcomerciales")]
        public string Fkcomerciales { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkcomerciales")]
        public string Nombrecomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Comisionagente")]
        public double? Comisionagente { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Comisioncomercial")]
        public double? Comisioncomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Cambioadicional")]
        public double? Cambioadicional { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Estado")]
        public string Fkestados { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.AlbaranesVentas, Tipoestado(Context));
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
        [Display(ResourceType = typeof(RAlbaranes), Name = "Estado")]
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

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importebruto")]
        public double? Importebruto { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importebaseimponible")]
        public double? Importebaseimponible { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importeportes")]
        public double? Importeportes { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        public string Porcentajedescuentocomercialcadena
        {
            get { return Porcentajedescuentocomercial?.ToString("N2") ?? (0.0).ToString("N2"); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        public string Porcentajedescuentoprontopagocadena {
            get { return Porcentajedescuentoprontopago?.ToString("N2") ?? (0.0).ToString("N2"); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importetotaldoc")]
        public double? Importetotaldoc { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importetotalmonedabase")]
        public double? Importetotalmonedabase { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Notas")]
        public string Notas { get; set; }


        [Display(ResourceType = typeof(RCliente), Name = "Tipodeportes")]
        public Tipoportes? Tipodeportes { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Costeportes")]
        public double? Costeportes { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkobras")]
        public string Fkobras { get; set; }

        public string Obradescripcion { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Incoterm")]
        public string Incoterm { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Descripcionincoterm")]
        public string Descripcionincoterm { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Peso")]
        public int Peso { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Confianza")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public int Confianza { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Costemateriales")]
        public double? Costemateriales { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Tiempooficinatecnica")]
        public double? Tiempooficinatecnica { get; set; }

        public int Decimalesmonedas { get; set; }

        public string Totaldocumento
        {
            get { return (Importetotaldoc ?? 0.0).ToString("N" + Decimalesmonedas); }
        }

        [Required]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fktransportista")]
        public string Fktransportista { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Tipocambio")]
        public double? Tipocambio { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();
        private string _fechafactura  = DateTime.Now.ToShortDateString().ToString(CultureInfo.InvariantCulture);

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Unidadnegocio")]
        public string Unidadnegocio { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkbancosmandatos")]
        public string Fkbancosmandatos { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Cartacredito")]
        public string Cartacredito { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Vencimientocartacredito")]
        public DateTime? Vencimientocartacredito { get; set; }

        [Range(0, 99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Contenedores")]
        public int? Contenedores { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkcuentastesoreria")]
        public string Fkcuentastesoreria { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Numerodocumentoproveedor")]
        public string Numerodocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fechadocumentoproveedor")]
        public DateTime? Fechadocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkclientesreserva")]
        public string Fkclientesreserva { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fktiposalbaranes")]
        public int Tipoalbaran { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fktiposalbaranes")]
        public TipoAlbaran Tipoalbaranenum { get { return (TipoAlbaran) Tipoalbaran; } set
        {
            Tipoalbaran = (int) value;
        } }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkmotivosdevolucion")]
        public string Fkmotivosdevolucion { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Nombretransportista")]
        public string Nombretransportista { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Conductor")]
        public string Conductor { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Matricula")]
        public string Matricula { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Remolque")]
        public string Remolque { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Bultos")]
        public int? Bultos { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Pesoneto")]
        public double? Pesoneto {get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Volumen")]
        public double? Volumen { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Envio")]
        public string Envio { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkoperarios")]
        public string Fkoperarios { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkoperariostransporte")]
        public string Fkoperariostransporte { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkzonas")]
        public string Fkzonas { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkdireccionfacturacion")]
        public int? Fkdireccionfacturacion { get; set; }

        [Required]
        [Display(ResourceType = typeof(RCriteriosagrupacion), Name = "TituloEntidadSingular")]
        public string Fkcriteriosagrupacion { get; set; }

        public string Descripcioncriterioagrupacion { get; set; }

        public IEnumerable<CriteriosagrupacionModel> Criteriosagrupacionlist { get; set; }

        public string Fkseriefactura { get; set; }

        public string Fechafactura
        {
            get { return _fechafactura; }
            set { _fechafactura = value; }
        }

        public bool IsFacturado { get; set; }

        public Guid? Fkcarpetas { get; set; }

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
        [Display(ResourceType = typeof(RPedidos), Name = "TituloEntidadSingular")]
        public string Fkpedidos { get; set; }

        public Guid Integridadreferencial { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkprospectos")]
        public string Fkprospectos { get; set; }

        //[Required]
        [Display(ResourceType = typeof(RAlbaranes), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        public override object generateId(string id)
        {
            return id;
        }


        #endregion

        #region Líneas

        public List<AlbaranesLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region Totales

        public List<AlbaranesTotalesModel> Totales
        {
            get { return _totales; }
            set { _totales = value; }
        }

        #endregion

        #region CTR

        public AlbaranesModel()
        {
            
        }

        public AlbaranesModel(IContextService context) : base(context)
        {
            
        }

        #endregion

       

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
        }

        public override string GetPrimaryKey()
        {
            return Id.ToString();
        }

        public override string DisplayName => RAlbaranes.TituloEntidad;
        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento( user.Id, TipoDocumentoImpresion.AlbaranesVentas.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(Context, db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.AlbaranesVentas, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.AlbaranesVentas,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }

        }
    }
    
    public class AlbaranesLinModel: ILineasDocumentosBusquedaMovil
    {
        private int? _decimalesmonedas=2;
        private int? _decimalesmedidas=3;
        private int? _orden;
        private bool _enFactura = false;
        public int Fkalbaranes { get; set; }

        public int? Fkreclamado { get; set; }

        public string Fkreclamadoreferencia { get; set; }

        public int Id { get; set; }
        public bool Nueva { get; set; }
        public int? Orden
        {
            get { return _orden ?? Id * ApplicationHelper.EspacioOrdenLineas; }
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
        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [MaxLength(120,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [MaxLength(12,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Tabla")]
        public int? Tabla { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Canal")]
        public string Canal { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Cantidadpedida")]
        public double? Cantidadpedida { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Largo")]
        public double? Largo { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Ancho")]
        public double? Ancho { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Grueso")]
        public double? Grueso { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Largo")]
        public string SLargo
        {
            get { return (Largo ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Largo = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Ancho")]
        public string SAncho
        {
            get { return (Ancho ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Ancho = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Grueso")]
        public string SGrueso
        {
            get { return (Grueso ?? 0.0).ToString("N" + Decimalesmedidas,CultureInfo.CurrentUICulture); }
            set { Grueso = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Metros")]
        public string SMetros
        {
            get { return (Metros ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Metros = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Precio")]
        public string SPrecio {
            get { return (Precio ?? 0.0).ToString(); }
            set { Precio = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Porcentajedescuento")]
        public double? Porcentajedescuento { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importedescuento")]
        public double? Importedescuento { get; set; }
        
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Cuotarecargoequivalencia")]
        public double? Cuotarecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ImporteBase")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "ImporteBase")]
        public string SImporte
        {
            get { return (Importe ?? 0.0).ToString(string.Format("N{0}", (Decimalesmonedas??0) )); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Notas")]
        public string Notas { get; set; }

        public string Documentoorigen { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Precioanterior")]
        public double? Precioanterior { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Revision")]
        public string Revision { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Contenedor")]
        public string Contenedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Sello")]
        public string Sello { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Caja")]
        public int? Caja { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Pesoneto")]
        public double? Pesoneto { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [MaxLength(15,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranes), Name = "Bundle")]
        public string Bundle { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Seccion")]
        public string Seccion { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importenetolinea")]
        public double? Importenetolinea { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Costeadicionalmaterial")]
        public double? Costeadicionalmaterial { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Costeadicionalportes")]
        public double? Costeadicionalportes { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Costeadicionalotro")]
        public double? Costeadicionalotro { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Costeadicionalvariable")]
        public double? Costeadicionalvariable { get; set; }

        public int idAlbaranDevuelto { get; set; }

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

        [Display(ResourceType = typeof(RAlbaranes), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

    }

    public class AlbaranesTotalesModel: ITotalesDocumentosBusquedaMovil
    {
        public int? Decimalesmonedas { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "BrutoTotal")]
        public double? Brutototal { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "BrutoTotal")]
        public string SBrutototal {
            get { return (Brutototal??0.0).ToString("N" + Decimalesmonedas); }
            set { Brutototal = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Basetotal")]
        public double? Baseimponible { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Basetotal")]
        public string SBaseimponible
        {
            get { return (Baseimponible ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Baseimponible = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Cuotaiva")]
        public double ? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Cuotaiva")]
        public string SCuotaiva
        {
            get { return (Cuotaiva ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Cuotaiva = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Cuotarecargoequivalencia")]
        public double? Importerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Cuotarecargoequivalencia")]
        public string SImporterecargoequivalencia
        {
            get { return (Importerecargoequivalencia ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importerecargoequivalencia = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importedescuentoprontopago")]
        public string SImportedescuentoprontopago
        {
            get { return (Importedescuentoprontopago ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentoprontopago = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Importedescuentocomercial")]
        public string SImportedescuentocomercial
        {
            get { return (Importedescuentocomercial ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentocomercial = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Subtotal")]
        public double? Subtotal { get; set; }

        [Display(ResourceType = typeof(RAlbaranes), Name = "Subtotal")]
        public string SSubtotal
        {
            get { return (Subtotal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Subtotal = Funciones.Qdouble(value); }
        }
    }

}
