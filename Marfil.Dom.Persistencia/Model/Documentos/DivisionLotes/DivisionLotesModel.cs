using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
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
using RDivisionLotes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.DivisionLotes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace Marfil.Dom.Persistencia.Model.Documentos.DivisionLotes
{
    public class DivisionLotesModel : BaseModel<DivisionLotesModel, Persistencia.DivisionLotes>, IDocument
    {

        //Lotes de entrada y salida
        private List<DivisionLotesentradaLinModel> _lineasentrada = new List<DivisionLotesentradaLinModel>();
        private List<DivisionLotessalidaLinModel> _lineassalida = new List<DivisionLotessalidaLinModel>();
        private List<DivisionLotesCostesadicionalesModel> _costes = new List<DivisionLotesCostesadicionalesModel>();

        #region properties
        //grip principal
        public string Fkarticulos { get; set; }
        public string Lote { get; set; }
        public string Descripcion { get; set; }


        public int? Id { get; set; }

        public string Empresa { get; set; }

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

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkalmacen")]
        public string Fkalmacen { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Nombrecliente")]
        public string Nombreproveedor { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Notas")]
        public string Notas { get; set; }

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
                    return serviceEstados.GetStates(DocumentoEstado.DivisionesLotes, Tipoestado(Context));
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

        [Display(ResourceType = typeof(Inf.ResourcesGlobalization.Textos.Entidades.Albaranes), Name = "Estado")]
        public string Fkestados { get; set; }

        public string Lotesalida { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        #endregion

        #region Líneas

        public List<DivisionLotesentradaLinModel> LineasEntrada
        {
            get { return _lineasentrada; }
            set { _lineasentrada = value; }
        }

        public List<DivisionLotessalidaLinModel> LineasSalida
        {
            get { return _lineassalida; }
            set { _lineassalida = value; }
        }

        
        public List<DivisionLotesCostesadicionalesModel> Costes
        {
            get { return _costes; }
            set { _costes = value; }
        }
        
        #endregion

        #region CTR

        public DivisionLotesModel()
        {

        }

        public DivisionLotesModel(IContextService context) : base(context)
        {

        }

        #endregion


        #region atributos

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

        public override string DisplayName => RDivisionLotes.TituloEntidad;
        public bool Materialsalidaigualentrada { get; set; }

        public string Material
        {
            get { return LineasEntrada.Any() ? ArticulosService.GetCodigoMaterial(LineasSalida.First().Fkarticulos) : string.Empty; }
        }

        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.DivisionLotes.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(Context,db);
                {
                    var lst =
                        service.GetDocumentosParaImprimir(TipoDocumentoImpresion.DivisionLotes, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.DivisionLotes,
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


#endregion

    }

    public class DivisionLotesCostesadicionalesModel
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

    public class DivisionLotesentradaLinModel
    {
        private int? _decimalesmonedas = 2;
        private int? _decimalesmedidas = 3;
        public int Id { get; set; }
        private int? _orden;
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

        public Guid Flagidentifier { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        public bool LoteAutomatico { get; set; }

        private string _loteautomaticoid;

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

    }

    public class DivisionLotessalidaLinModel : BaseModel<DivisionLotessalidaLinModel, Persistencia.DivisionLotes>
    {
        private int? _decimalesmonedas = 2;
        private int? _decimalesmedidas = 3;
        public int Id { get; set; }
        private int? _orden;
        private string _loteautomaticoid;

        public DivisionLotessalidaLinModel(IContextService context) : base(context)
        {
        }

        public string Empresa { get; set; }

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

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public int? Tipoalmacenlote { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        public override string DisplayName => RDivisionLotes.TituloEntidad;

        public override object generateId(string id)
        {
            return id;
        }
    }

    public class DivisionLotessalidaLinVistaModel
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
        public List<MovimientosstockModel> Lineas { get; set; }
        public string Fkcuenta { get; set; }
        public string Fkmonedas { get; set; }
        public string Fkregimeniva { get; set; }
        public TipoFlujo Flujo { get; set; }
        public int? Caja { get; set; }
        public string Canal { get; set; }

        public TipoAlmacenlote? Tipodealmacenlote { get; set; }
    }

    public class DivisionLotesentradaLinVistaModel : IDocumentoLinVistaModel
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

    public partial class DivisionLotesSalidaLinSerialized
    {
        public string empresa { get; set; }
        public int fktransformaciones { get; set; }
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
        public Nullable<int> orden { get; set; }
        public System.Guid flagidentifier { get; set; }
        public Nullable<double> precio { get; set; }
        public Nullable<int> tipoalmacenlote { get; set; }
    }


    public class DivisionLotesSalidaSerializable
    {
        public int Id { get; set; }
        public string Referencia { get; set; }
        public DateTime? Fechadocumento { get; set; }
        public DivisionLotesSalidaLinSerialized Linea { get; set; }
    }



    public partial class DivisionLotesEntradaLinSerialized
    {
        public string empresa { get; set; }
        public int fktransformaciones { get; set; }
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
        public Nullable<bool> nuevo { get; set; }
        public string loteautomaticoid { get; set; }
        public Nullable<int> lotenuevocontador { get; set; }
        public Nullable<int> tipoalmacenlote { get; set; }
    }


    public class DivisionLotesEntradaSerializable
    {
        public int Id { get; set; }
        public string Referencia { get; set; }
        public DateTime? Fechadocumento { get; set; }
        public string Codigoproveedor { get; set; }
        public DivisionLotesEntradaLinSerialized Linea { get; set; }
    }


}
