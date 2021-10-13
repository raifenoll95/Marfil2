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
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using RPedidoscompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PedidosCompras;
using RCliente = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Proveedores;
using RCriteriosagrupacion = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Criteriosagrupacion;

namespace Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras
{
    public enum TipoCosteAdicional
    {
        [StringValue(typeof(RAlbaranesCompras), "Importefijo")]
        Importefijo,
        [StringValue(typeof(RAlbaranesCompras), "Documento")]
        Documento,
        [StringValue(typeof(RAlbaranesCompras), "Costexm2")]
        Costexm2,
        [StringValue(typeof(RAlbaranesCompras), "Costexm3")]
        Costexm3
    }

    public enum TipoCoste
    {
        [StringValue(typeof(RAlbaranesCompras), "Material")]
        Material,
        [StringValue(typeof(RAlbaranesCompras), "Portes")]
        Portes,
        [StringValue(typeof(RAlbaranesCompras), "Otros")]
        Otros
    }

    public enum TipoReparto
    {
        [StringValue(typeof(RAlbaranesCompras), "Metros")]
        Metros,
        [StringValue(typeof(RAlbaranesCompras), "Peso")]
        Peso,
        [StringValue(typeof(RAlbaranesCompras), "Cantidad")]
        Cantidad,
        [StringValue(typeof(RAlbaranesCompras), "Importe")]
        Importe
    }

    public enum TipoAlmacenlote
    {
        [StringValue(typeof(RAlbaranesCompras), "TipoAlmacenLoteMercaderia")]
        Mercaderia,
        [StringValue(typeof(RAlbaranesCompras), "TipoAlmacenLotePropio")]
        Propio,
        [StringValue(typeof(RAlbaranesCompras), "TipoAlmacenLoteGestionado")]
        Gestionado,

    }

    public class StDocumentosCompras
    {
        public string Tipo { get; set; }
        public string Referencia { get; set; }
        public DateTime? Fecha { get; set; }

        public string Fechadocumento
        {
            get { return Fecha.Value.ToShortDateString(); }
        }

        public string Proveedor { get; set; }

        public double? Base { get; set; }
    }

    public class AlbaranesComprasLinVistaModel : IDocumentoLinVistaModel
    {
        public string Fkarticulos { get; set; }
        public string Fkalmacen { get; set; }
        public bool Loteautomatico { get; set; }
        public string Lote { get; set; }
        public int Cantidad { get; set; }
        public double Largo { get; set; }
        public double Ancho { get; set; }
        public double Grueso { get; set; }
        public double Metros { get; set; }
        public double Precio { get; set; }
        public double Descuento { get; set; }
        public double Subtotal { get; set; }
        public int Decimalesmonedas { get; set; }
        public int Decimalesmedidas { get; set; }
        public string Fkunidades { get; set; }
        public string Fktiposiva { get; set; }
        public TipoFamilia Tipofamilia { get; set; }
        public TipoStockFormulas Formulas { get; set; }
        public string Descuentoprontopago { get; set; }
        public string Descuentocomercial { get; set; }
        public string Portes { get; set; }
        public string Bundle { get; set; }

        public TipoAlmacenlote? Tipodealmacenlote { get; set; }
    }

    public partial class AlbaranesComprasLinSerialized
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
        public string fkcontadoreslotes { get; set; }
        public System.Guid flagidentifier { get; set; }
        public Nullable<int> tipoalmacenlote { get; set; }       
    }

    public class AlbaranesComprasDiarioStockSerializable
    {
        public int Id { get; set; }
        public string Referencia { get; set; }
        public DateTime? Fechadocumento { get; set; }
        public string Codigoproveedor { get; set; }            
        public AlbaranesComprasLinSerialized Linea { get; set; }        
    }

    public class AlbaranesComprasModel : BaseModel<AlbaranesComprasModel, Persistencia.AlbaranesCompras>, IDocument, IGaleria
    {
        private List<AlbaranesComprasLinModel> _lineas = new List<AlbaranesComprasLinModel>();
        private List<AlbaranesComprasTotalesModel> _totales = new List<AlbaranesComprasTotalesModel>();
        private List<AlbaranesComprasCostesadicionalesModel> _costes = new List<AlbaranesComprasCostesadicionalesModel>();


        #region CTR

        public AlbaranesComprasModel()
        {
            Modo = ModoAlbaran.Sinstock;
        }

        public AlbaranesComprasModel(IContextService context) : base(context)
        {
            Modo = ModoAlbaran.Sinstock;
        }

        #endregion

        #region Properties

        //Albaran de reclamacion, necesito saber cual es el id del original
        public int idOriginalReclamado { get; set; }

        public bool? Pedidosaldado { get; set; }

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        public ModoAlbaran Modo { get; set; }

        public int Id { get; set; }

        public bool? Importado { get; set; }

        public string Empresa { get; set; }

        [Required]
        public Guid? Integridadreferencialflag { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }


        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fechadocumento")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Fechadocumento { get; set; }

        public string Fechadocumentocadena
        {
            get { return Fechadocumento?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? ""; }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fechavalidez")]
        public DateTime? Fechavalidez { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fechaentrega")]
        public DateTime? Fechaentrega { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fecharevision")]
        public DateTime? Fecharevision { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkproveedores")]
        public string Fkproveedores { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Nombrecliente")]
        public string Nombreproveedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Clientedireccion")]
        public string Proveedordireccion { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Clientepoblacion")]
        public string Proveedorpoblacion { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Clientecp")]
        public string Proveedorcp { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Clientepais")]
        public string Proveedorpais { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Clienteprovincia")]
        public string Proveedorprovincia { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Clientetelefono")]
        public string Proveedortelefono { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Clientefax")]
        public string Proveedorfax { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Clienteemail")]
        public string Proveedoremail { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Clientenif")]
        public string Proveedornif { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkagentes")]
        public string Fkagentes { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkagentes")]
        public string Nombreagentes { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkcomerciales")]
        public string Fkcomerciales { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkcomerciales")]
        public string Nombrecomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Comisionagente")]
        public double? Comisionagente { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Comisioncomercial")]
        public double? Comisioncomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Cambioadicional")]
        public double? Cambioadicional { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Estado")]
        public string Fkestados { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.AlbaranesCompras, Tipoestado(Context));
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
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Estado")]
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
                    var estadoObj=  estadosService.get(Fkestados) as EstadosModel;
                    return estadoObj?.Tipoestado ?? TipoEstado.Diseño;
                }
            }
            return TipoEstado.Diseño;
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importebruto")]
        public double? Importebruto { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importebaseimponible")]
        public double? Importebaseimponible { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importeportes")]
        public double? Importeportes { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        public string Porcentajedescuentocomercialcadena
        {
            get { return Porcentajedescuentocomercial?.ToString("N2") ?? (0.0).ToString("N2"); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        public string Porcentajedescuentoprontopagocadena
        {
            get { return Porcentajedescuentoprontopago?.ToString("N2") ?? (0.0).ToString("N2"); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importetotaldoc")]
        public double? Importetotaldoc { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importetotalmonedabase")]
        public double? Importetotalmonedabase { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Notas")]
        public string Notas { get; set; }


        [Display(ResourceType = typeof(RCliente), Name = "Tipodeportes")]
        public Tipoportes? Tipodeportes { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeportes")]
        public double? Costeportes { get; set; }

        internal void CalcularCosteTotalMetros(List<AlbaranesComprasLinModel> lineas, List<AlbaranesComprasCostesadicionalesModel> costes)
        {
            throw new NotImplementedException();
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkobras")]
        public string Fkobras { get; set; }

        public string Obradescripcion { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Incoterm")]
        public string Incoterm { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Descripcionincoterm")]
        public string Descripcionincoterm { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Peso")]
        public int Peso { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Confianza")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public int Confianza { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costemateriales")]
        public double? Costemateriales { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Tiempooficinatecnica")]
        public double? Tiempooficinatecnica { get; set; }

        public int Decimalesmonedas { get; set; }

        public string Totaldocumento
        {
            get { return (Importetotaldoc ?? 0.0).ToString("N" + Decimalesmonedas); }
        }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fktransportista")]
        public string Fktransportista { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Tipocambio")]
        public double? Tipocambio { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();
        private string _fechafactura = DateTime.Now.ToShortDateString().ToString(CultureInfo.InvariantCulture);

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Unidadnegocio")]
        public string Unidadnegocio { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkbancosmandatos")]
        public string Fkbancosmandatos { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Cartacredito")]
        public string Cartacredito { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Vencimientocartacredito")]
        public DateTime? Vencimientocartacredito { get; set; }

        [Range(0, 99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Contenedores")]
        public int? Contenedores { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkcuentastesoreria")]
        public string Fkcuentastesoreria { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Numerodocumentoproveedor")]
        public string Numerodocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fechadocumentoproveedor")]
        public DateTime? Fechadocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkclientesreserva")]
        public string Fkclientesreserva { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fktiposalbaranes")]
        public int Tipoalbaran { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fktiposalbaranes")]
        public TipoAlbaran Tipoalbaranenum
        {
            get { return (TipoAlbaran)Tipoalbaran; }
            set
            {
                Tipoalbaran = (int)value;
            }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkmotivosdevolucion")]
        public string Fkmotivosdevolucion { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Nombretransportista")]
        public string Nombretransportista { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Conductor")]
        public string Conductor { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Matricula")]
        public string Matricula { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Bultos")]
        public int? Bultos { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Pesoneto")]
        public double? Pesoneto { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Volumen")]
        public double? Volumen { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Envio")]
        public string Envio { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkoperarios")]
        public string Fkoperarios { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkoperariostransporte")]
        public string Fkoperariostransporte { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkzonas")]
        public string Fkzonas { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkdireccionfacturacion")]
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

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TituloEntidadSingular")]
        public string Fkpedidoscompras { get; set; }
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        #endregion

        #region Líneas

        public List<AlbaranesComprasLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region Totales

        public List<AlbaranesComprasTotalesModel> Totales
        {
            get { return _totales; }
            set { _totales = value; }
        }

        #endregion

        #region Costes adicionales

        public List<AlbaranesComprasCostesadicionalesModel> Costes
        {
            get { return _costes; }
            set { _costes = value; }
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

        public override string DisplayName => RAlbaranesCompras.TituloEntidad;
        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user =Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.AlbaranesCompras.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.AlbaranesCompras, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.AlbaranesCompras,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }

        }
    }

    public class AlbaranesComprasLinModel : IDocumentosLinModel
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
        public string Loteautomaticoid
        {
            get { return _loteautomaticoid?.ToLower(); }
            set { _loteautomaticoid = value; }
        }
        public int? Orden
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
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }
        public int Fkalbaranes { get; set; }

        [MaxLength(120, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Descripcion")]
        public string Descripcion { get; set; }

        

        [MaxLength(12, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Tabla")]
        public int? Tabla { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Canal")]
        public string Canal { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Cantidadpedida")]
        public double? Cantidadpedida { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Largo")]
        public double? Largo { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Ancho")]
        public double? Ancho { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Grueso")]
        public double? Grueso { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Largo")]
        public string SLargo
        {
            get { return (Largo ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Largo = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Ancho")]
        public string SAncho
        {
            get { return (Ancho ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Ancho = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Grueso")]
        public string SGrueso
        {
            get { return (Grueso ?? 0.0).ToString("N" + Decimalesmedidas, CultureInfo.CurrentUICulture); }
            set { Grueso = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Metros")]
        public string SMetros
        {
            get { return (Metros ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Metros = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Precio")]
        public string SPrecio
        {
            get { return (Precio ?? 0.0).ToString(); }
            set { Precio = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentajedescuento")]
        public double? Porcentajedescuento { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importedescuento")]
        public double? Importedescuento { get; set; }

        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Cuotarecargoequivalencia")]
        public double? Cuotarecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "ImporteBase")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "ImporteBase")]
        public string SImporte
        {
            get { return (Importe ?? 0.0).ToString(string.Format("N{0}", (Decimalesmonedas ?? 0))); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Notas")]
        public string Notas { get; set; }



        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Precioanterior")]
        public double? Precioanterior { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Revision")]
        public string Revision { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Contenedor")]
        public string Contenedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Sello")]
        public string Sello { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Caja")]
        public int? Caja { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Pesoneto")]
        public double? Pesoneto { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Bundle")]
        public string Bundle { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Seccion")]
        public string Seccion { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importenetolinea")]
        public double? Importenetolinea { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalmaterial")]
        public double? Costeadicionalmaterial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalportes")]
        public double? Costeadicionalportes { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalotro")]
        public double? Costeadicionalotro { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalvariable")]
        public double? Costeadicionalvariable { get; set; }

        //clave ajena
        public int? Fkpedidos { get; set; }
        public int? Fkpedidosid { get; set; }
        public string Fkpedidosreferencia { get; set; }

        public int? Fkreclamado { get; set; }
        public string Fkreclamadoreferencia { get; set; }

        public int? Tblnum { get; set; }

        public int? Lineaasociada { get; set; }

        public List<StDocumentoReferencia> Fkfacturasreferencia { get; set; }

        public bool EnFactura
        {
            get { return _enFactura; }
            set { _enFactura = value; }
        }

        public Guid Flagidentifier { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

    }

    public class AlbaranesComprasTotalesModel
    {
        public int? Decimalesmonedas { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "BrutoTotal")]
        public double? Brutototal { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "BrutoTotal")]
        public string SBrutototal
        {
            get { return (Brutototal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Brutototal = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Basetotal")]
        public double? Baseimponible { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Basetotal")]
        public string SBaseimponible
        {
            get { return (Baseimponible ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Baseimponible = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Cuotaiva")]
        public string SCuotaiva
        {
            get { return (Cuotaiva ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Cuotaiva = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Cuotarecargoequivalencia")]
        public double? Importerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Cuotarecargoequivalencia")]
        public string SImporterecargoequivalencia
        {
            get { return (Importerecargoequivalencia ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importerecargoequivalencia = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importedescuentoprontopago")]
        public string SImportedescuentoprontopago
        {
            get { return (Importedescuentoprontopago ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentoprontopago = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importedescuentocomercial")]
        public string SImportedescuentocomercial
        {
            get { return (Importedescuentocomercial ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentocomercial = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Subtotal")]
        public double? Subtotal { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Subtotal")]
        public string SSubtotal
        {
            get { return (Subtotal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Subtotal = Funciones.Qdouble(value); }
        }
    }

    public class AlbaranesComprasCostesadicionalesModel
    {
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Tipodocumento")]
        public TipoCosteAdicional Tipodocumento { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importe")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importe")]
        public string SImporte
        {
            get { return Importe?.ToString("N2") ?? 0.ToString("N2"); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentaje")]
        [Range(0.01, 100.0, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double? Porcentaje { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Total")]
        public double? Total { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Total")]
        public string STotal
        {
            get { return Total?.ToString("N2") ?? 0.ToString("N2"); }
            set { Total = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Tipocoste")]
        public TipoCoste Tipocoste { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Tiporeparto")]
        public TipoReparto Tiporeparto { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Notas")]
        public string Notas { get; set; }
    }

}
