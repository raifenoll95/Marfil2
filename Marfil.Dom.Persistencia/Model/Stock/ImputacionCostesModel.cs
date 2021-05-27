using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using RImputacionCostes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ImputacionCostes;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;
using Resources;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Documentos.AlbaranesCompras;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Inf.Genericos.Helper;
using System.Globalization;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;

namespace Marfil.Dom.Persistencia.Model.Stock
{
    public class ImputacionCostesModel : BaseModel<ImputacionCostesModel, Persistencia.ImputacionCostes>, IDocument
    {

        private List<ImputacionCostesLinModel> _lineaslotes = new List<ImputacionCostesLinModel>();
        private List<ImputacionCostesCostesadicionalesModel> _costes = new List<ImputacionCostesCostesadicionalesModel>();

        public override string DisplayName => RImputacionCostes.TituloEntidad;

        public override object generateId(string id)
        {
            return id;
        }

        #region propiedades

        //[Required]
        [Key]
        public int? Id { get; set; }

        [Required]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Referencia")]
        public string Referencia { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkoperarios")]
        public string Fkoperarios { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fechadocumento")]
        public DateTime? Fechadocumento { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Notas")]
        public string Notas { get; set; }

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.ImputacionCostes, Tipoestado(Context));
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

        public Guid? Integridadreferencialflag { get; set; }

        public List<ImputacionCostesLinModel> LineasLotes
        {
            get { return _lineaslotes; }
            set { _lineaslotes = value.ToList(); }
        }

        public List<ImputacionCostesCostesadicionalesModel> LineasCostes
        {
            get { return _costes; }
            set { _costes = value.ToList(); }
        }

        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.ImputacionCostes.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(db);
                {
                    var lst =
                        service.GetDocumentosParaImprimir(TipoDocumentoImpresion.ImputacionCostes, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.ImputacionCostes,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }
        }

        #endregion

        #region CTR

        public ImputacionCostesModel()
        {

        }

        public ImputacionCostesModel(IContextService context) : base(context)
        {

        }

        #endregion
    }

    public class ImputacionCostesLinModel : IDocumentosLinModel
    {

        private int? _decimalesmonedas = 2;
        private int? _decimalesmedidas = 3;
        
        private int? _orden;
        private string _loteautomaticoid;

        public ImputacionCostesLinModel(IContextService context)
        {
        }

        //Empresa
        [Required]
        public string Empresa { get; set; }

        //Fkimputacioncostes
        [Required]
        public int Fkimputacioncostes { get; set; }

        [Required]
        [Key]
        public int Id { get; set; }

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

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Metros")]
        public double? Metros { get; set; }

        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Notas")]
        public string Notas { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Revision")]
        public string Revision { get; set; }

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

        public int? Orden
        {
            get { return _orden ?? Id * ApplicationHelper.EspacioOrdenLineas; }
            set { _orden = value; }
        }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Porcentaje")]
        [Range(0.01, 100.0, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double? Porcentaje { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Importe")]
        public double? Importe { get; set; }

        public Guid Flagidentifier { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalmaterial")]
        public double? Costeadicionalmaterial { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalportes")]
        public double? Costeadicionalportes { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalotro")]
        public double? Costeadicionalotro { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "Costeadicionalvariable")]
        public double? Costeadicionalvariable { get; set; }



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
        public string SMetros
        {
            get { return (Metros ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Metros = Funciones.Qdouble(value); }
        }
        

        public bool Nueva { get; set; }
        public string Fkcontadoreslotes { get; set; }
        public int Lotenuevocontador { get; set; }
        public string Loteautomaticoid
        {
            get { return _loteautomaticoid?.ToLower(); }
            set { _loteautomaticoid = value; }
        }

        

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public int? Tipoalmacenlote { get; set; }

        [Display(ResourceType = typeof(RAlbaranesCompras), Name = "TipoAlmacenLote")]
        public TipoAlmacenlote? Tipodealmacenlote { get; set; }

        //public override string DisplayName => RImputacionCostes.TituloEntidad;

        /*
        public override object generateId(string id)
        {
            return id;
        }
        */
    }

    public class ImputacionCostesCostesadicionalesModel
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

    public class ImputacionCostesLinVistaModel
    {
        public TipoPieza Tipopieza { get; set; }
        public string Fkarticulos { get; set; }
        public List<MovimientosstockModelHistorico> Lineas { get; set; }
        public string Fkalmacen { get; set; }
        public string Lote { get; set; }
        public string LoteId { get; set; }
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
        public string Fkcuenta { get; set; }
        public string Fkmonedas { get; set; }
        public string Fkregimeniva { get; set; }
        public TipoFlujo Flujo { get; set; }
        public int? Caja { get; set; }
        public string Canal { get; set; }

        public TipoAlmacenlote? Tipodealmacenlote { get; set; }
    }

}
