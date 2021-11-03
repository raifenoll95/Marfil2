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
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RPresupuestosCompras=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PresupuestosCompras;

namespace Marfil.Dom.Persistencia.Model.Documentos.PresupuestosCompras
{
    public class PresupuestosComprasModel : BaseModel<PresupuestosComprasModel, Persistencia.PresupuestosCompras>, IDocument, IGaleria
    {
        private List<PresupuestosComprasLinModel> _lineas=new List<PresupuestosComprasLinModel>();
        private List<PresupuestosComprasTotalesModel> _totales = new List<PresupuestosComprasTotalesModel>();

        #region Properties

        public int? Id { get; set; }
        
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }
        
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fechadocumento")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Fechadocumento { get; set; }

        public string Fechadocumentocadena
        {
            get { return Fechadocumento?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? string.Empty; }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fechavalidez")]
        public DateTime? Fechavalidez { get; set; }
        public string Fechavalidezcadena
        {
            get { return Fechavalidez?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? string.Empty; }
        }
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fechaentrega")]
        public DateTime? Fechaentrega { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fecharevision")]
        public DateTime? Fecharevision { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkalmacen")]
        public int? Fkalmacen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkproveedores")]
        public string Fkproveedores { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Nombrecliente")]
        public string Nombrecliente { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Clientedireccion")]
        public string Clientedireccion { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Clientepoblacion")]
        public string Clientepoblacion { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Clientecp")]
        public string Clientecp { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Clientepais")]
        public string Clientepais { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Clienteprovincia")]
        public string Clienteprovincia { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Clientetelefono")]
        public string Clientetelefono { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Clientefax")]
        public string Clientefax { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Clienteemail")]
        public string Clienteemail { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Clientenif")]
        public string Clientenif { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkagentes")]
        public string Fkagentes { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkagentes")]
        public string Nombreagentes { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkcomerciales")]
        public string Fkcomerciales { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkcomerciales")]
        public string Nombrecomercial { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Comisionagente")]
        public double? Comisionagente { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Comisioncomercial")]
        public double? Comisioncomercial { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Cambioadicional")]
        public double? Cambioadicional { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Estado")]
        public string Fkestados { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Numerodocumentoproveedor")]
        public string Numerodocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fechadocumentoproveedor")]
        public DateTime? Fechadocumentoproveedor { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                   return serviceEstados.GetStates(DocumentoEstado.PresupuestosCompras, Tipoestado(Context));
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
                    using (var estadosService = FService.Instance.GetService(typeof (EstadosModel), Context))
                    {
                        return estadosService.get(Fkestados) as EstadosModel;
                    }
                }
                return null;
            }
        }

        [XmlIgnore]
        [IgnoreDataMember]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Estado")]
        public string Estadodescripcion
        {
            get { return Estado?.Descripcion; }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importebruto")]
        public double? Importebruto { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importebaseimponible")]
        public double? Importebaseimponible { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importeportes")]
        public double? Importeportes { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importetotaldoc")]
        public double? Importetotaldoc { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importetotalmonedabase")]
        public double? Importetotalmonedabase { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkobras")]
        public string Fkobras { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Incoterm")]
        public string Incoterm { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Descripcionincoterm")]
        public string Descripcionincoterm { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Peso")]
        public int Peso { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Confianza")]
        [Range(0,100,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "RangeClient")]
        public int Confianza { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Costemateriales")]
        public double? Costemateriales { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Tiempooficinatecnica")]
        public double? Tiempooficinatecnica { get; set; }

        public int Decimalesmonedas { get; set; }

        public string Totaldocumento
        {
            get { return (Importetotaldoc??0.0).ToString("N" + Decimalesmonedas); }
        }

        [Required]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fktransportista")]
        public string Fktransportista { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Tipocambio")]
        public double? Tipocambio { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Unidadnegocio")]
        public string Unidadnegocio { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkbancosmandatos")]
        public string Fkbancosmandatos { get; set; }

        [MaxLength(25,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Cartacredito")]
        public string Cartacredito { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Vencimientocartacredito")]
        public DateTime? Vencimientocartacredito { get; set; }

        public string Vencimientocartacreditocadena
        {
            get { return Vencimientocartacredito?.ToShortDateString().ToString(CultureInfo.InvariantCulture)??string.Empty; }
        }

        [Range(0,99,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Contenedores")]
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

        public List<PresupuestosComprasLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region Totales

        public List<PresupuestosComprasTotalesModel> Totales
        {
            get { return _totales; }
            set { _totales = value; }
        }

        #endregion

        #region CTR

        public PresupuestosComprasModel()
        {

        }

        public PresupuestosComprasModel(IContextService context) : base(context)
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

        public override string DisplayName => RPresupuestosCompras.TituloEntidad;
        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.PresupuestosCompras.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(Context, db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.PresupuestosCompras, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.PresupuestosCompras,
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
    }
    
    public class PresupuestosComprasLinModel
    {
        private int? _decimalesmonedas=2;
        private int? _decimalesmedidas=3;
        public int Id { get; set; }
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
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [MaxLength(120,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [MaxLength(12,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Tabla")]
        public int? Tabla { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Canal")]
        public string Canal { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Cantidadpedida")]
        public double? Cantidadpedida { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Largo")]
        public double? Largo { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Ancho")]
        public double? Ancho { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Grueso")]
        public double? Grueso { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Largo")]
        public string SLargo
        {
            get { return (Largo ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Largo = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Ancho")]
        public string SAncho
        {
            get { return (Ancho ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Ancho = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Grueso")]
        public string SGrueso
        {
            get { return (Grueso ?? 0.0).ToString("N" + Decimalesmedidas,CultureInfo.CurrentUICulture); }
            set { Grueso = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Metros")]
        public string SMetros
        {
            get { return (Metros ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Metros = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Precio")]
        public string SPrecio {
            get { return (Precio ?? 0.0).ToString(); }
            set { Precio = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Porcentajedescuento")]
        public double? Porcentajedescuento { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importedescuento")]
        public double? Importedescuento { get; set; }
        
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Cuotarecargoequivalencia")]
        public double? Cuotarecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "ImporteBase")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "ImporteBase")]
        public string SImporte
        {
            get { return (Importe ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Notas")]
        public string Notas { get; set; }

        

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Precioanterior")]
        public double? Precioanterior { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Revision")]
        public string Revision { get; set; }

        public bool EnPedido { get; set; }

        public List<StDocumentoReferencia> Fkpedidoreferencia { get; set; }

        public int? Orden
        {
            get { return _orden ?? Id * ApplicationHelper.EspacioOrdenLineas; }
            set { _orden = value; }
        }
    }

    public class PresupuestosComprasLinImportarModel:PresupuestosComprasLinModel
    {
        public int FkPresupuestosCompras { get; set; }        
        public string FkPresupuestosComprasreferencia { get; set; }
        public int? Fkmonedas { get; set; }
        public PresupuestosComprasModel Cabecera { get; set; }
        
    }

    public struct StDocumentoReferencia
    {
        public string Referencia { get; set; }
        public string CampoId { get; set; }
    }

    public class PresupuestosComprasTotalesModel
    {
        public int? Decimalesmonedas { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "BrutoTotal")]
        public double? Brutototal { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "BrutoTotal")]
        public string SBrutototal {
            get { return (Brutototal??0.0).ToString("N" + Decimalesmonedas); }
            set { Brutototal = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Basetotal")]
        public double? Baseimponible { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Basetotal")]
        public string SBaseimponible
        {
            get { return (Baseimponible ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Baseimponible = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Cuotaiva")]
        public double ? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Cuotaiva")]
        public string SCuotaiva
        {
            get { return (Cuotaiva ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Cuotaiva = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Cuotarecargoequivalencia")]
        public double? Importerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Cuotarecargoequivalencia")]
        public string SImporterecargoequivalencia
        {
            get { return (Importerecargoequivalencia ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importerecargoequivalencia = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importedescuentoprontopago")]
        public string SImportedescuentoprontopago
        {
            get { return (Importedescuentoprontopago ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentoprontopago = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Importedescuentocomercial")]
        public string SImportedescuentocomercial
        {
            get { return (Importedescuentocomercial ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentocomercial = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Subtotal")]
        public double? Subtotal { get; set; }

        [Display(ResourceType = typeof(RPresupuestosCompras), Name = "Subtotal")]
        public string SSubtotal
        {
            get { return (Subtotal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Subtotal = Funciones.Qdouble(value); }
        }
    }

}
