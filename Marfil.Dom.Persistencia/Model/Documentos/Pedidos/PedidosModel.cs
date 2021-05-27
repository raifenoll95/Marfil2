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
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RPedidos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Pedidos;
using RCliente = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;

namespace Marfil.Dom.Persistencia.Model.Documentos.Pedidos
{
    public class PedidosModel : BaseModel<PedidosModel, Persistencia.Pedidos>, IDocument, IGaleria
    {
        private List<PedidosLinModel> _lineas = new List<PedidosLinModel>();
        private List<PedidosTotalesModel> _totales = new List<PedidosTotalesModel>();
        private List<PedidosCostesFabricacionModel> _costes = new List<PedidosCostesFabricacionModel>();
        private List<AlbaranesLinModel> _materiales = new List<AlbaranesLinModel>();
        private List<AlbaranesTotalesModel> _totalesimputacion = new List<AlbaranesTotalesModel>();

        #region Properties

        public int Id { get; set; }
        public bool? Importado { get; set; }
        
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidos), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidos), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }

        
        [Display(ResourceType = typeof(RPedidos), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidos), Name = "Fechadocumento")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Fechadocumento { get; set; }

        public string Fechadocumentocadena
        {
            get { return Fechadocumento?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? string.Empty; }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Fechavalidez")]
        public DateTime? Fechavalidez { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fechaentrega")]
        public DateTime? Fechaentrega { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fecharevision")]
        public DateTime? Fecharevision { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fkalmacen")]
        public int? Fkalmacen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidos), Name = "Fkclientes")]
        public string Fkclientes { get; set; }

        public bool? Usuariocliente { get; set; }

        public string Codigoclienteusuario { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Nombrecliente")]
        public string Nombrecliente { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Clientedireccion")]
        public string Clientedireccion { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Clientepoblacion")]
        public string Clientepoblacion { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Clientecp")]
        public string Clientecp { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Clientepais")]
        public string Clientepais { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Clienteprovincia")]
        public string Clienteprovincia { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Clientetelefono")]
        public string Clientetelefono { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Clientefax")]
        public string Clientefax { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Clienteemail")]
        public string Clienteemail { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Clientenif")]
        public string Clientenif { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidos), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fkagentes")]
        public string Fkagentes { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fkagentes")]
        public string Nombreagentes { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fkcomerciales")]
        public string Fkcomerciales { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fkcomerciales")]
        public string Nombrecomercial { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Comisionagente")]
        public double? Comisionagente { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Comisioncomercial")]
        public double? Comisioncomercial { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Cambioadicional")]
        public double? Cambioadicional { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Estado")]
        public string Fkestados { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.PedidosVentas, Tipoestado(Context));
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
        [Display(ResourceType = typeof(RPedidos), Name = "Estado")]
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

        [Display(ResourceType = typeof(RPedidos), Name = "Importebruto")]
        public double? Importebruto { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importebaseimponible")]
        public double? Importebaseimponible { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importeportes")]
        public double? Importeportes { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importetotaldoc")]
        public double? Importetotaldoc { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importetotalmonedabase")]
        public double? Importetotalmonedabase { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Notas")]
        public string Notas { get; set; }



        [Display(ResourceType = typeof(RPedidos), Name = "Fkobras")]
        public string Fkobras { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Incoterm")]
        public string Incoterm { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Descripcionincoterm")]
        public string Descripcionincoterm { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Peso")]
        public int Peso { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Confianza")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public int Confianza { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Costemateriales")]
        public double? Costemateriales { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Tiempooficinatecnica")]
        public double? Tiempooficinatecnica { get; set; }

        public int Decimalesmonedas { get; set; }

        public string Totaldocumento
        {
            get { return (Importetotaldoc ?? 0.0).ToString("N" + Decimalesmonedas); }
        }

        [Required]
        [Display(ResourceType = typeof(RPedidos), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fktransportista")]
        public string Fktransportista { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Tipocambio")]
        public double? Tipocambio { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Unidadnegocio")]
        public string Unidadnegocio { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fkbancosmandatos")]
        public string Fkbancosmandatos { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPedidos), Name = "Cartacredito")]
        public string Cartacredito { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Vencimientocartacredito")]
        public DateTime? Vencimientocartacredito { get; set; }

        [Range(0, 99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RPedidos), Name = "Contenedores")]
        public int? Contenedores { get; set; }

        [Display(ResourceType = typeof(RCliente), Name = "Cuentatesoreria")]
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

        public List<PedidosLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region Totales

        public List<PedidosTotalesModel> Totales
        {
            get { return _totales; }
            set { _totales = value; }
        }

        #endregion

        #region Costes fabricación

        public List<PedidosCostesFabricacionModel> CostesFabricacion
        {
            get { return _costes; }
            set { _costes = value; }
        }

        #endregion

        #region Imputación materiales

        public List<AlbaranesLinModel> ImputacionMateriales
        {
            get { return _materiales; }
            set { _materiales = value; }
        }

        public List<AlbaranesTotalesModel> ImputacionMaterialesTotales
        {
            get { return _totalesimputacion; }
            set { _totalesimputacion = value; }
        }

        #endregion


        #region CTR
        
        public PedidosModel()
        {

        }

        public PedidosModel(IContextService context) : base(context)
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

        public override string DisplayName => RPedidos.TituloEntidad;
        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = base.Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.PedidosVentas.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.PedidosVentas, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.PedidosVentas,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }
        }
    }
    
    public class PedidosLinModel: ILineasDocumentosBusquedaMovil
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
        [Display(ResourceType = typeof(RPedidos), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [MaxLength(120,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPedidos), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [MaxLength(12,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPedidos), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Tabla")]
        public int? Tabla { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Canal")]
        public string Canal { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Cantidadpedida")]
        public double? Cantidadpedida { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Largo")]
        public double? Largo { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Ancho")]
        public double? Ancho { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Grueso")]
        public double? Grueso { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Largo")]
        public string SLargo
        {
            get { return (Largo ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Largo = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Ancho")]
        public string SAncho
        {
            get { return (Ancho ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Ancho = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Grueso")]
        public string SGrueso
        {
            get { return (Grueso ?? 0.0).ToString("N" + Decimalesmedidas,CultureInfo.CurrentUICulture); }
            set { Grueso = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Metros")]
        public string SMetros
        {
            get { return (Metros ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Metros = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Precio")]
        public string SPrecio {
            get { return (Precio ?? 0.0).ToString(); }
            set { Precio = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Porcentajedescuento")]
        public double? Porcentajedescuento { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importedescuento")]
        public double? Importedescuento { get; set; }
        
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Cuotarecargoequivalencia")]
        public double? Cuotarecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "ImporteBase")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "ImporteBase")]
        public string SImporte
        {
            get { return (Importe ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Notas")]
        public string Notas { get; set; }

        

        [Display(ResourceType = typeof(RPedidos), Name = "Precioanterior")]
        public double? Precioanterior { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPedidos), Name = "Revision")]
        public string Revision { get; set; }

        //clave ajena
        public int? Fkpresupuestos { get; set; }
        public int? Fkpresupuestosid { get; set; }
        public string Fkpresupuestosreferencia { get; set; }

        public string idAlbaranSalidasVarias { get; set; }
        public bool EnAlbaran { get; set; }

        public int? Orden
        {
            get { return _orden ?? Id * ApplicationHelper.EspacioOrdenLineas; }
            set { _orden = value; }
        }

        public List<StDocumentoReferencia> Fkalbaranreferencia { get; set; }
    }

    public class PedidosTotalesModel: ITotalesDocumentosBusquedaMovil
    {
        public int? Decimalesmonedas { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "BrutoTotal")]
        public double? Brutototal { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "BrutoTotal")]
        public string SBrutototal {
            get { return (Brutototal??0.0).ToString("N" + Decimalesmonedas); }
            set { Brutototal = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Basetotal")]
        public double? Baseimponible { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Basetotal")]
        public string SBaseimponible
        {
            get { return (Baseimponible ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Baseimponible = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Cuotaiva")]
        public double ? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Cuotaiva")]
        public string SCuotaiva
        {
            get { return (Cuotaiva ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Cuotaiva = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Cuotarecargoequivalencia")]
        public double? Importerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Cuotarecargoequivalencia")]
        public string SImporterecargoequivalencia
        {
            get { return (Importerecargoequivalencia ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importerecargoequivalencia = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importedescuentoprontopago")]
        public string SImportedescuentoprontopago
        {
            get { return (Importedescuentoprontopago ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentoprontopago = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Importedescuentocomercial")]
        public string SImportedescuentocomercial
        {
            get { return (Importedescuentocomercial ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentocomercial = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidos), Name = "Subtotal")]
        public double? Subtotal { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Subtotal")]
        public string SSubtotal
        {
            get { return (Subtotal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Subtotal = Funciones.Qdouble(value); }
        }
    }

    public class PedidosLinImportarModel : PedidosLinModel
    {
        public int? Fkpedidos { get; set; }
        public string Fkpedidosreferencia { get; set; }
        public int? Fkmonedas { get; set; }
        public PedidosModel Cabecera { get; set; }

    }

    // Modelo costes fabricación
    public class PedidosCostesFabricacionModel
    {
        
        [Required]
        public string Empresa { get; set; }

        [Required]
        public int Fkpedido { get; set; }

        [Required]
        [Key]
        public int Id { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "FechaFabricacion")]
        public DateTime? Fecha { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidos), Name = "Fkoperario")]
        public string Fkoperario { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "NombreOperario")]
        public string DescripcionOperario { get; set; }   

        [Required]
        [Display(ResourceType = typeof(RPedidos), Name = "Fktarea")]
        public string Fktarea { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "DescripcionTarea")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Cantidad")]
        public double Cantidad { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Precio")]
        public double Precio { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Total")]
        public double Total { get; set; }        

        public PedidosCostesFabricacionModel()
        {
            Empresa = "0000";
            Fkpedido = 0;
            Id = 0;
        }
    }

}
