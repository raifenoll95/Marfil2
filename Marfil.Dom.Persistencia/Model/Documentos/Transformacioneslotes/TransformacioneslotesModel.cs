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
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RTransformacioneslotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformacioneslotes;
using RTransformaciones=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Transformaciones;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
namespace Marfil.Dom.Persistencia.Model.Documentos.Transformacioneslotes
{
    public class TransformacioneslotesModel : BaseModel<TransformacioneslotesModel, Persistencia.Transformacioneslotes>, IDocument
    {
        private List<TransformacioneslotesLinModel> _lineassalida=new List<TransformacioneslotesLinModel>();
        private List<TransformacioneslotesCostesadicionalesModel> _costes = new List<TransformacioneslotesCostesadicionalesModel>();
        private GaleriaModel _galeria;

        #region Properties

        [Key]
        public int? Id { get; set; }
        
        public string Empresa { get; set; }

        public bool PuedeReclasificarMaterial { get; set; }

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
            get { return Fechadocumento?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? string.Empty; }
        }

        [Required]
        [Display(ResourceType = typeof(RTransformaciones), Name = "Fktrabajos")]
        public string Fktrabajos { get; set; }

        [Display(ResourceType = typeof(RTransformaciones), Name = "Fkmateriales")]
        public string Fkmateriales { get; set; }

        [Display(ResourceType = typeof(RTransformaciones), Name = "Fktrabajos")]
        public string Trabajosdescripcion { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Referenciadocumento")]
        public string Referenciadocumentoproveedor { get; set; }

       
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fechadocumentoproveedor")]
        public DateTime? Fechadocumentoproveedor { get; set; }

        public string Fechadocumentoproveedorcadena
        {
            get { return Fechadocumentoproveedor?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? string.Empty; }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkproveedores")]
        public string Fkproveedores { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Nombrecliente")]
        public string Nombreproveedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fktransportista")]
        public string Fktransportista { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Nombretransportista")]
        public string Nombretransportista { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Conductor")]
        public string Conductor { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Matricula")]
        public string Matricula { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkoperarios")]
        public string Fkoperarios { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkoperariostransporte")]
        public string Fkoperariostransporte { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkzonas")]
        public string Fkzonas { get; set; }

        public Guid? Integridadreferencialflag { get; set; }

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.Transformacioneslotes, Tipoestado(Context));
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
        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Albaranes), Name = "Estado")]
        public string Estadodescripcion
        {
            get { return Estado?.Descripcion; }
        }

        #endregion

        #region Líneas

        public List<TransformacioneslotesLinModel> Lineas
        {
            get { return _lineassalida; }
            set { _lineassalida = value; }
        }

        public List<TransformacioneslotesCostesadicionalesModel> Costes
        {
            get { return _costes; }
            set { _costes = value; }
        }

        #endregion

        #region CTR

        public TransformacioneslotesModel()
        {

        }

        public TransformacioneslotesModel(IContextService context) : base(context)
        {

        }

        #endregion

        //public override object generateId(string id)
        //{
        //    return id;
        //}

        //public override void createNewPrimaryKey()
        //{
        //    primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
        //}

        //public override string GetPrimaryKey()
        //{
        //    return Id.ToString();
        //}

        public override string DisplayName => RTransformacioneslotes.TituloEntidad;
        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.TransformacionesAcabados.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(db);
                {
                    var lst =
                        service.GetDocumentosParaImprimir(TipoDocumentoImpresion.TransformacionesAcabados, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.TransformacionesAcabados,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto= doc?.Name
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

        public override object generateId(string id)
        {
            return id;
        }

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Albaranes), Name = "Estado")]
        public string Fkestados { get; set; }

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
    }

    public class TransformacioneslotesLinModel : IDocumentosLinModel
    {
        private int? _decimalesmonedas = 2;
        private int? _decimalesmedidas = 3;
        public int Id { get; set; }
        private int? _orden;
        private string _loteautomaticoid;

        public double? Precio { get; set; }

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

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Notas")]
        public string Notas { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Revision")]
        public string Revision { get; set; }

        public int? Orden
        {
            get { return _orden ?? Id * ApplicationHelper.EspacioOrdenLineas; }
            set { _orden = value; }
        }

        public bool Nueva { get; set; }
        public string Fkcontadoreslotes { get; set; }
        public int Lotenuevocontador { get; set; }
        public string Loteautomaticoid
        {
            get { return _loteautomaticoid?.ToLower(); }
            set { _loteautomaticoid = value; }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalmaterial")]
        public double? Costeadicionalmaterial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalportes")]
        public double? Costeadicionalportes { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalotro")]
        public double? Costeadicionalotro { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalvariable")]
        public double? Costeadicionalvariable { get; set; }

        public Guid Flagidentifier { get; set; }

        public TipoAlmacenlote? Tipodealmacenlote { get; set; }
    }

    public class TransformacioneslotesCostesadicionalesModel
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


    public class TransformacioneslotesLinVistaModel
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

    public partial class TransformacionesloteslinSerialized
    {
        public string empresa { get; set; }
        public int fkTransformacioneslotes { get; set; }
        public int id { get; set; }
        public string fkarticulos { get; set; }
        public string descripcion { get; set; }
        public string lote { get; set; }
        public Nullable<int> tabla { get; set; }
        public Nullable<double> cantidad { get; set; }
        public Nullable<double> largo { get; set; }
        public Nullable<double> ancho { get; set; }
        public Nullable<double> grueso { get; set; }
        public string fkunidades { get; set; }
        public Nullable<double> metros { get; set; }
        public string notas { get; set; }
        public string documentoorigen { get; set; }
        public string documentodestino { get; set; }
        public string canal { get; set; }
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
        public System.Guid flagidentifier { get; set; }
        public string fkcontadoreslotes { get; set; }
        public Nullable<double> precio { get; set; }
    }

        public class TransformacioneslotesDiarioStockSerializable
    {
        public int Id { get; set; }
        public string Referencia { get; set; }
        public DateTime? Fechadocumento { get; set; }
        public string Codigoproveedor { get; set; }
        public TransformacionesloteslinSerialized Linea { get; set; }
        public string Fkarticulosnuevo { get; set; }
    }

   
}
