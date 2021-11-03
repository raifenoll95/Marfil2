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
using RReservasstock=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Reservas;
using RCliente = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RCriteriosagrupacion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Criteriosagrupacion;

namespace Marfil.Dom.Persistencia.Model.Documentos.Reservasstock
{

    public class ReservasstockLinVistaModel
    {
        public TipoPieza Tipopieza { get; set; }
        public string Fkarticulos { get; set; }
        public string Fkalmacen { get; set; }
        public string Lote { get; set; }
        public bool Modificarmedidas { get; set; }
        public int Cantidad { get; set; }
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

    public partial class ReservasstockLinSerialized
    {
        public string empresa { get; set; }
        public int fkreservasstock { get; set; }
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
    }

        public class ReservasstockDiarioStockSerializable
    {
        public int Id { get; set; }
        public string Referencia { get; set; }
        public DateTime? Fechadocumento { get; set; }
        public string Codigocliente { get; set; }
        public ReservasstockLinSerialized Linea { get; set; }
    }

    public class ReservasstockModel : BaseModel<ReservasstockModel, Persistencia.Reservasstock>, IDocument, IGaleria
    {
        private List<ReservasstockLinModel> _lineas = new List<ReservasstockLinModel>();
        private List<ReservasstockTotalesModel> _totales = new List<ReservasstockTotalesModel>();
        private GaleriaModel _galeria;
      

        #region Properties

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        public int Id { get; set; }

        public bool? Importado { get; set; }
        
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RReservasstock), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Required]
        [Display(ResourceType = typeof(RReservasstock), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }

        
        [Display(ResourceType = typeof(RReservasstock), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RReservasstock), Name = "Fechadocumento")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Fechadocumento { get; set; }

        public string Fechadocumentocadena
        {
            get { return Fechadocumento?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? ""; }
        }

        [Required]
        [Display(ResourceType = typeof(RReservasstock), Name = "Fechavalidez")]
        public DateTime? Fechavalidez { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fechaentrega")]
        public DateTime? Fechaentrega { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fecharevision")]
        public DateTime? Fecharevision { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RReservasstock), Name = "Fkclientes")]
        public string Fkclientes { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Nombrecliente")]
        public string Nombrecliente { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Clientedireccion")]
        public string Clientedireccion { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Clientepoblacion")]
        public string Clientepoblacion { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Clientecp")]
        public string Clientecp { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Clientepais")]
        public string Clientepais { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Clienteprovincia")]
        public string Clienteprovincia { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Clientetelefono")]
        public string Clientetelefono { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Clientefax")]
        public string Clientefax { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Clienteemail")]
        public string Clienteemail { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Clientenif")]
        public string Clientenif { get; set; }

        [Required]
        [Display(ResourceType = typeof(RReservasstock), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkagentes")]
        public string Fkagentes { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkagentes")]
        public string Nombreagentes { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkcomerciales")]
        public string Fkcomerciales { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkcomerciales")]
        public string Nombrecomercial { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Comisionagente")]
        public double? Comisionagente { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Comisioncomercial")]
        public double? Comisioncomercial { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Cambioadicional")]
        public double? Cambioadicional { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Estado")]
        public string Fkestados { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.Reservasstock, Tipoestado(Context));
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
        [Display(ResourceType = typeof(RReservasstock), Name = "Estado")]
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

        [Display(ResourceType = typeof(RReservasstock), Name = "Importebruto")]
        public double? Importebruto { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importebaseimponible")]
        public double? Importebaseimponible { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importeportes")]
        public double? Importeportes { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        public string Porcentajedescuentocomercialcadena
        {
            get { return Porcentajedescuentocomercial?.ToString("N2") ?? (0.0).ToString("N2"); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        public string Porcentajedescuentoprontopagocadena {
            get { return Porcentajedescuentoprontopago?.ToString("N2") ?? (0.0).ToString("N2"); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importetotaldoc")]
        public double? Importetotaldoc { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importetotalmonedabase")]
        public double? Importetotalmonedabase { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Notas")]
        public string Notas { get; set; }


        [Display(ResourceType = typeof(RCliente), Name = "Tipodeportes")]
        public Tipoportes? Tipodeportes { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Costeportes")]
        public double? Costeportes { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkobras")]
        public string Fkobras { get; set; }

        public string Obradescripcion { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Incoterm")]
        public string Incoterm { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Descripcionincoterm")]
        public string Descripcionincoterm { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Peso")]
        public int Peso { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Confianza")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public int Confianza { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Costemateriales")]
        public double? Costemateriales { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Tiempooficinatecnica")]
        public double? Tiempooficinatecnica { get; set; }

        public int Decimalesmonedas { get; set; }

        public string Totaldocumento
        {
            get { return (Importetotaldoc ?? 0.0).ToString("N" + Decimalesmonedas); }
        }

        [Required]
        [Display(ResourceType = typeof(RReservasstock), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fktransportista")]
        public string Fktransportista { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Tipocambio")]
        public double? Tipocambio { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();
        private string _fechafactura  = DateTime.Now.ToShortDateString().ToString(CultureInfo.InvariantCulture);

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Unidadnegocio")]
        public string Unidadnegocio { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkbancosmandatos")]
        public string Fkbancosmandatos { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RReservasstock), Name = "Cartacredito")]
        public string Cartacredito { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Vencimientocartacredito")]
        public DateTime? Vencimientocartacredito { get; set; }

        [Range(0, 99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RReservasstock), Name = "Contenedores")]
        public int? Contenedores { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkcuentastesoreria")]
        public string Fkcuentastesoreria { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Numerodocumentoproveedor")]
        public string Numerodocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fechadocumentoproveedor")]
        public DateTime? Fechadocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkclientesreserva")]
        public string Fkclientesreserva { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkmotivosdevolucion")]
        public string Fkmotivosdevolucion { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Nombretransportista")]
        public string Nombretransportista { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Conductor")]
        public string Conductor { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Matricula")]
        public string Matricula { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Bultos")]
        public int? Bultos { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Pesoneto")]
        public double? Pesoneto {get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Volumen")]
        public double? Volumen { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Envio")]
        public string Envio { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkoperarios")]
        public string Fkoperarios { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkoperariostransporte")]
        public string Fkoperariostransporte { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkzonas")]
        public string Fkzonas { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkdireccionfacturacion")]
        public int? Fkdireccionfacturacion { get; set; }
        
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

        public Guid Integridadreferencial { get; set; }

        #endregion

        #region Líneas

        public List<ReservasstockLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region Totales

        public List<ReservasstockTotalesModel> Totales
        {
            get { return _totales; }
            set { _totales = value; }
        }

        #endregion

        #region CTR

        public ReservasstockModel()
        {

        }

        public ReservasstockModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return int.Parse(id);
        }

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
        }

        public override string GetPrimaryKey()
        {
            return Id.ToString();
        }

        public override string DisplayName => RReservasstock.TituloEntidad;
        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento( user.Id, TipoDocumentoImpresion.Reservasstock.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(Context, db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.Reservasstock, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.Reservasstock,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }

        }
    }
    
    public class ReservasstockLinModel: ILineasDocumentosBusquedaMovil
    {
        private int? _decimalesmonedas=2;
        private int? _decimalesmedidas=3;
        private int? _orden;
        private bool _enFactura = false;
        
        public int Id { get; set; }

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
        [Display(ResourceType = typeof(RReservasstock), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [MaxLength(120,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RReservasstock), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [MaxLength(12,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RReservasstock), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Tabla")]
        public int? Tabla { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Canal")]
        public string Canal { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Cantidadpedida")]
        public double? Cantidadpedida { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Largo")]
        public double? Largo { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Ancho")]
        public double? Ancho { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Grueso")]
        public double? Grueso { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Largo")]
        public string SLargo
        {
            get { return (Largo ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Largo = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Ancho")]
        public string SAncho
        {
            get { return (Ancho ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Ancho = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Grueso")]
        public string SGrueso
        {
            get { return (Grueso ?? 0.0).ToString("N" + Decimalesmedidas,CultureInfo.CurrentUICulture); }
            set { Grueso = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Metros")]
        public string SMetros
        {
            get { return (Metros ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Metros = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Precio")]
        public string SPrecio {
            get { return (Precio ?? 0.0).ToString("N" + Decimalesmonedas, CultureInfo.CurrentUICulture); }
            set { Precio = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Porcentajedescuento")]
        public double? Porcentajedescuento { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importedescuento")]
        public double? Importedescuento { get; set; }
        
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Cuotarecargoequivalencia")]
        public double? Cuotarecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "ImporteBase")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "ImporteBase")]
        public string SImporte
        {
            get { return (Importe ?? 0.0).ToString(string.Format("N{0}", (Decimalesmonedas??0) )); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Notas")]
        public string Notas { get; set; }

        

        [Display(ResourceType = typeof(RReservasstock), Name = "Precioanterior")]
        public double? Precioanterior { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RReservasstock), Name = "Revision")]
        public string Revision { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Contenedor")]
        public string Contenedor { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Sello")]
        public string Sello { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Caja")]
        public int? Caja { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Pesoneto")]
        public double? Pesoneto { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [MaxLength(2,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RReservasstock), Name = "Bundle")]
        public string Bundle { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Seccion")]
        public string Seccion { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importenetolinea")]
        public double? Importenetolinea { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Costeadicionalmaterial")]
        public double? Costeadicionalmaterial { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Costeadicionalportes")]
        public double? Costeadicionalportes { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Costeadicionalotro")]
        public double? Costeadicionalotro { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Costeadicionalvariable")]
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

    public class ReservasstockTotalesModel: ITotalesDocumentosBusquedaMovil
    {
        public int? Decimalesmonedas { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "BrutoTotal")]
        public double? Brutototal { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "BrutoTotal")]
        public string SBrutototal {
            get { return (Brutototal??0.0).ToString("N" + Decimalesmonedas); }
            set { Brutototal = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Basetotal")]
        public double? Baseimponible { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Basetotal")]
        public string SBaseimponible
        {
            get { return (Baseimponible ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Baseimponible = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Cuotaiva")]
        public double ? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Cuotaiva")]
        public string SCuotaiva
        {
            get { return (Cuotaiva ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Cuotaiva = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Cuotarecargoequivalencia")]
        public double? Importerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Cuotarecargoequivalencia")]
        public string SImporterecargoequivalencia
        {
            get { return (Importerecargoequivalencia ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importerecargoequivalencia = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importedescuentoprontopago")]
        public string SImportedescuentoprontopago
        {
            get { return (Importedescuentoprontopago ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentoprontopago = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Importedescuentocomercial")]
        public string SImportedescuentocomercial
        {
            get { return (Importedescuentocomercial ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentocomercial = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RReservasstock), Name = "Subtotal")]
        public double? Subtotal { get; set; }

        [Display(ResourceType = typeof(RReservasstock), Name = "Subtotal")]
        public string SSubtotal
        {
            get { return (Subtotal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Subtotal = Funciones.Qdouble(value); }
        }
    }

}
