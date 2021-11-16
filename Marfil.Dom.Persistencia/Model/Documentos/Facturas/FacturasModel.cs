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
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;

//criterioiva
using Marfil.Dom.Persistencia.Model.Terceros;


using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RFacturas=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Facturas;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using RCliente = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;

namespace Marfil.Dom.Persistencia.Model.Documentos.Facturas
{

    public class FacturasModel : BaseModel<FacturasModel, Persistencia.Facturas>, IDocument, IGaleria
    {
        private List<FacturasLinModel> _lineas = new List<FacturasLinModel>();
        private List<FacturasTotalesModel> _totales = new List<FacturasTotalesModel>();
        private List<FacturasVencimientosModel> _vencimientos = new List<FacturasVencimientosModel>();

        #region Properties

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        public int Id { get; set; }

        public bool? Importado { get; set; }

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RFacturas), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Required]
        [Display(ResourceType = typeof(RFacturas), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }
        
        [Display(ResourceType = typeof(RFacturas), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RFacturas), Name = "Fechadocumento")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Fechadocumento { get; set; }


        [Display(ResourceType = typeof(RAlbaranes), Name = "Fechabl")]
        public DateTime? Fechabl { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RFacturas), Name = "Fkclientes")]
        public string Fkclientes { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Nombrecliente")]
        public string Nombrecliente { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Clientedireccion")]
        public string Clientedireccion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Clientepoblacion")]
        public string Clientepoblacion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Clientecp")]
        public string Clientecp { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Clientepais")]
        public string Clientepais { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Clienteprovincia")]
        public string Clienteprovincia { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Clientetelefono")]
        public string Clientetelefono { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Clientefax")]
        public string Clientefax { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Clienteemail")]
        public string Clienteemail { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Clientenif")]
        public string Clientenif { get; set; }

        [Required]
        [Display(ResourceType = typeof(RFacturas), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }
        

        [Display(ResourceType = typeof(RFacturas), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cambioadicional")]
        public double? Cambioadicional { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cobrado")]
        public bool? Cobrado { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cobrado")]
        public bool Cobradoview {
            get { return Cobrado ?? false; }
            set { Cobrado = value; }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Facturarectificativa")]
        public bool? Facturarectificativa { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkfacturarectificativa")]
        public string Fkfacturarectificativa { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkmotivorectificacion")]
        public string Fkmotivorectificacion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Estado")]
        public string Fkestados { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.FacturasVentas, Tipoestado(Context));
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
        [Display(ResourceType = typeof(RFacturas), Name = "Estado")]
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

        [Display(ResourceType = typeof(RFacturas), Name = "Importebruto")]
        public double? Importebruto { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importebaseimponible")]
        public double? Importebaseimponible { get; set; }

        public string Fecha { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importeportes")]
        public double? Importeportes { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importetotaldoc")]
        public double? Importetotaldoc { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importetotalmonedabase")]
        public double? Importetotalmonedabase { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkobras")]
        public string Fkobras { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Incoterm")]
        public string Incoterm { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Descripcionincoterm")]
        public string Descripcionincoterm { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Peso")]
        public int Peso { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Confianza")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public int Confianza { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Costemateriales")]
        public double? Costemateriales { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Tiempooficinatecnica")]
        public double? Tiempooficinatecnica { get; set; }

        public int Decimalesmonedas { get; set; }

        public string Totaldocumento
        {
            get { return (Importetotaldoc ?? 0.0).ToString("N" + Decimalesmonedas); }
        }

        [Required]
        [Display(ResourceType = typeof(RFacturas), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fktiposretenciones")]
        public string Fktiposretenciones { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajeretencion")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double? Porcentajeretencion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fktransportista")]
        public string Fktransportista { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Tipocambio")]
        public double? Tipocambio { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();
        

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Unidadnegocio")]
        public string Unidadnegocio { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkbancosmandatos")]
        public string Fkbancosmandatos { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RFacturas), Name = "Cartacredito")]
        public string Cartacredito { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Vencimientocartacredito")]
        public DateTime? Vencimientocartacredito { get; set; }

        [Range(0, 99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RFacturas), Name = "Contenedores")]
        public int? Contenedores { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkcuentastesoreria")]
        public string Fkcuentastesoreria { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Numerodocumentoproveedor")]
        public string Numerodocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fechadocumentoproveedor")]
        public DateTime? Fechadocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkclientesreserva")]
        public string Fkclientesreserva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkmotivosdevolucion")]
        public string Fkmotivosdevolucion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Nombretransportista")]
        public string Nombretransportista { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Conductor")]
        public string Conductor { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Matricula")]
        public string Matricula { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Bultos")]
        public int? Bultos { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Pesoneto")]
        public double? Pesoneto {get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Volumen")]
        public double? Volumen { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Envio")]
        public string Envio { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkoperarios")]
        public string Fkoperarios { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkoperariostransporte")]
        public string Fkoperadortransporte { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkzonas")]
        public string Fkzonas { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkdireccionfacturacion")]
        public int? Fkdireccionfacturacion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Brutocomision")]
        public double? Brutocomision { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Comisiondescontardescuentocomercial")]
        public bool? Comisiondescontardescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Comisiondescontardescuentocomercial")]
        public bool Comisiondescontardescuentocomercialvista {
            get { return Comisiondescontardescuentocomercial ?? false; }
            set { Comisiondescontardescuentocomercial = value; }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Comsiondescontardescuentoprontopago")]
        public bool? Comsiondescontardescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Comsiondescontardescuentoprontopago")]
        public bool Comsiondescontardescuentoprontopagovista
        {
            get { return Comsiondescontardescuentoprontopago ?? false; }
            set { Comsiondescontardescuentoprontopago = value; }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotadescuentocomercialcomision")]
        public double? Cuotadescuentocomercialcomision { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "cuotadescuentoprontopagocomision")]
        public double? Cuotadescuentoprontopagocomision { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Comisiondescontarportesincluidosprecio")]
        public bool? Comisiondescontarportesincluidosprecio { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Comisiondescontarportesincluidosprecio")]
        public bool Comisiondescontarportesincluidospreciovista
        {
            get { return Comisiondescontarportesincluidosprecio ?? false; }
            set { Comisiondescontarportesincluidosprecio = value; }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotadescuentoportesincluidospreciocomision")]
        public double? Cuotadescuentoportesincluidospreciocomision { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Comisiondescontarrecargofinancieroformapago")]
        public bool? Comisiondescontarrecargofinancieroformapago { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Comisiondescontarrecargofinancieroformapago")]
        public bool Comisiondescontarrecargofinancieroformapagovista
        {
            get { return Comisiondescontarrecargofinancieroformapago ?? false; }
            set { Comisiondescontarrecargofinancieroformapago = value; }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotadescuentorecargofinancieroformapagocomision")]
        public double? Cuotadescuentorecargofinancieroformapagocomision { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Netobasecomision")]
        public double? Netobasecomision { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importecomisionagente")]
        public double? Importecomisionagente { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importecomisioncomercial")]
        public double? Importecomisioncomercial { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fksituacioncomision")]
        public string Fksituacioncomision { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkaseguradoras")]
        public string Fkaseguradoras { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Suplemento")]
        public string Suplemento { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkagentes")]
        public string Fkagentes { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkagentes")]
        public string Nombreagentes { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkcomerciales")]
        public string Fkcomerciales { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkcomerciales")]
        public string Nombrecomercial { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Comisionagente")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double? Comisionagente { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Comisioncomercial")]
        public double? Comisioncomercial { get; set; }

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

        //criterioiva
        [Display(ResourceType = typeof(RFacturas), Name = "Criterioiva")]
        public CriterioIVA Criterioiva { get; set; }

        //canalcontable
        [Display(ResourceType = typeof(RFacturas), Name = "Canalcontable")]
        public string Canalcontable { get; set; }

        public int? Fkasiento { get; set; }
        public string urlAsiento { get; set; }

        #endregion

        #region Líneas

        public List<FacturasLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkcriteriosagrupacion")]
        public string Fkcriteriosagrupacion { get; set; }

        #endregion

        #region Totales

        public List<FacturasTotalesModel> Totales
        {
            get { return _totales; }
            set { _totales = value; }
        }

        #endregion

        #region Vencimientos

        public List<FacturasVencimientosModel> Vencimientos
        {
            get { return _vencimientos; }
            set { _vencimientos = value; }
        }

        #endregion

        #region CTR

        public FacturasModel()
        {

        }

        public FacturasModel(IContextService context) : base(context)
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

        public override string DisplayName => RFacturas.TituloEntidad;
        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.FacturasVentas.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(Context, db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.FacturasVentas, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.FacturasVentas,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }

        }
    }
    
    public class FacturasLinModel: ILineasDocumentosBusquedaMovil
    {
        private int? _decimalesmonedas=2;
        private int? _decimalesmedidas=3;
        private int? _orden;
        public int Id { get; set; }

        public int Orden
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
        [Display(ResourceType = typeof(RFacturas), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [MaxLength(120,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RFacturas), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [MaxLength(12,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RFacturas), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Tabla")]
        public int? Tabla { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Canal")]
        public string Canal { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cantidadpedida")]
        public double? Cantidadpedida { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Largo")]
        public double? Largo { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Ancho")]
        public double? Ancho { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Grueso")]
        public double? Grueso { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Largo")]
        public string SLargo
        {
            get { return (Largo ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Largo = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Ancho")]
        public string SAncho
        {
            get { return (Ancho ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Ancho = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Grueso")]
        public string SGrueso
        {
            get { return (Grueso ?? 0.0).ToString("N" + Decimalesmedidas,CultureInfo.CurrentUICulture); }
            set { Grueso = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Metros")]
        public string SMetros
        {
            get { return (Metros ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Metros = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Precio")]
        public string SPrecio {
            get { return (Precio ?? 0.0).ToString(); }
            set { Precio = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajedescuento")]
        public double? Porcentajedescuento { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importedescuento")]
        public double? Importedescuento { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotarecargoequivalencia")]
        public double? Cuotarecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "ImporteBase")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "ImporteBase")]
        public string SImporte
        {
            get { return (Importe ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Precioanterior")]
        public double? Precioanterior { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RFacturas), Name = "Revision")]
        public string Revision { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Contenedor")]
        public string Contenedor { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Sello")]
        public string Sello { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Caja")]
        public int? Caja { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Pesoneto")]
        public double? Pesoneto { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Pesobruto")]
        public double? Pesobruto { get; set; }

        [MaxLength(2,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RFacturas), Name = "Bundle")]
        public string Bundle { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Seccion")]
        public string Seccion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importenetolinea")]
        public double? Importenetolinea { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Costeadicionalmaterial")]
        public double? Costeadicionalmaterial { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Costeadicionalportes")]
        public double? Costeadicionalportes { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Costeadicionalotro")]
        public double? Costeadicionalotro { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Costeadicionalvariable")]
        public double? Costeadicionalvariable { get; set; }

        //clave ajena
        public int? Fkalbaranes { get; set; }
       

        public DateTime? Fkalbaranesfecha { get; set; }

        public string Fkalbaranesreferencia { get; set; }

        public string Fkalbaranesfkcriteriosagrupacion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajecomision")]
        public double? Porcentajecomision { get; set; }

        public int? Tblnum { get; set; }

        public int? Lineaasociada { get; set; }
    }

    public class FacturasTotalesModel: ITotalesDocumentosBusquedaMovil
    {
        public int? Decimalesmonedas { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "BrutoTotal")]
        public double? Brutototal { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "BrutoTotal")]
        public string SBrutototal {
            get { return (Brutototal??0.0).ToString("N" + Decimalesmonedas); }
            set { Brutototal = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Basetotal")]
        public double? Baseimponible { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Basetotal")]
        public string SBaseimponible
        {
            get { return (Baseimponible ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Baseimponible = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotaiva")]
        public double ? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotaiva")]
        public string SCuotaiva
        {
            get { return (Cuotaiva ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Cuotaiva = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotarecargoequivalencia")]
        public double? Importerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Cuotarecargoequivalencia")]
        public string SImporterecargoequivalencia
        {
            get { return (Importerecargoequivalencia ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importerecargoequivalencia = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importedescuentoprontopago")]
        public string SImportedescuentoprontopago
        {
            get { return (Importedescuentoprontopago ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentoprontopago = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importedescuentocomercial")]
        public string SImportedescuentocomercial
        {
            get { return (Importedescuentocomercial ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentocomercial = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Baseretencion")]
        public double? Baseretencion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Baseretencion")]
        public string SBaseretencion
        {
            get { return (Baseretencion ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Baseretencion = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RFacturas), Name = "Porcentajeretencion")]
        public double? Porcentajeretencion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importeretencion")]
        public double? Importeretencion { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Importeretencion")]
        public string SImporteretencion
        {
            get { return (Importeretencion ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importeretencion = Funciones.Qdouble(value); }
        }
        
        [Display(ResourceType = typeof(RFacturas), Name = "Subtotal")]
        public double? Subtotal { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Subtotal")]
        public string SSubtotal
        {
            get { return (Subtotal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Subtotal = Funciones.Qdouble(value); }
        }
    }

    public class FacturasVencimientosModel
    {
        
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RFacturas), Name = "Diasvencimiento")]
        public int? Diasvencimiento { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fechavencimiento")]
        public DateTime? Fechavencimiento { get; set; }

        public string FechaVencimientoCadena
        {
            get { return Fechavencimiento?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? string.Empty; }
        }

        [Required]
        [Display(ResourceType = typeof(RFacturas), Name = "Importevencimiento")]
        public double? Importevencimiento { get; set; }

        [Required]
        [Display(ResourceType = typeof(RFacturas), Name = "Importevencimiento")]
        public string SImporetevencimiento
        {
            get { return (Importevencimiento ?? 0).ToString("N" + Decimalesmonedas); }
            set { Importevencimiento = Funciones.Qdouble(value); }
        }
        public int? Decimalesmonedas { get; set; }
    }

}
