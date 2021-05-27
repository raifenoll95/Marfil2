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
using RPedidosCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PedidosCompras;
using RCliente = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos.Importar;

namespace Marfil.Dom.Persistencia.Model.Documentos.PedidosCompras
{
    public class PedidosComprasModel : BaseModel<PedidosComprasModel, Persistencia.PedidosCompras>, IDocument, IGaleria
    {
        private List<PedidosComprasLinModel> _lineas = new List<PedidosComprasLinModel>();
        private List<PedidosComprasTotalesModel> _totales = new List<PedidosComprasTotalesModel>();

        #region Properties

        public int Id { get; set; }
        public bool? Importado { get; set; }
        
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }

        
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Referencia")]
        public string Referencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fechadocumento")]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? Fechadocumento { get; set; }

        public string Fechadocumentocadena
        {
            get { return Fechadocumento?.ToShortDateString().ToString(CultureInfo.InvariantCulture) ?? string.Empty; }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fechavalidez")]
        public DateTime? Fechavalidez { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fechaentrega")]
        public DateTime? Fechaentrega { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fecharevision")]
        public DateTime? Fecharevision { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkalmacen")]
        public int? Fkalmacen { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkproveedores")]
        public string Fkproveedores { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Nombrecliente")]
        public string Nombrecliente { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Clientedireccion")]
        public string Clientedireccion { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Clientepoblacion")]
        public string Clientepoblacion { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Clientecp")]
        public string Clientecp { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Clientepais")]
        public string Clientepais { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Clienteprovincia")]
        public string Clienteprovincia { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Clientetelefono")]
        public string Clientetelefono { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Clientefax")]
        public string Clientefax { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Clienteemail")]
        public string Clienteemail { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Clientenif")]
        public string Clientenif { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Numerodocumentoproveedor")]
        public string Numerodocumentoproveedor { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fechadocumentoproveedor")]
        public DateTime? Fechadocumentoproveedor { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkagentes")]
        public string Fkagentes { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkagentes")]
        public string Nombreagentes { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkcomerciales")]
        public string Fkcomerciales { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkcomerciales")]
        public string Nombrecomercial { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Comisionagente")]
        public double? Comisionagente { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Comisioncomercial")]
        public double? Comisioncomercial { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkmonedas")]
        public int? Fkmonedas { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Cambioadicional")]
        public double? Cambioadicional { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Estado")]
        public string Fkestados { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        public IEnumerable<EstadosModel> EstadosAsociados
        {
            get
            {
                using (var serviceEstados = new EstadosService(Context))
                {
                    return serviceEstados.GetStates(DocumentoEstado.PedidosCompras, Tipoestado(Context));
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
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Estado")]
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

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importebruto")]
        public double? Importebruto { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importebaseimponible")]
        public double? Importebaseimponible { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importeportes")]
        public double? Importeportes { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importetotaldoc")]
        public double? Importetotaldoc { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importetotalmonedabase")]
        public double? Importetotalmonedabase { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Notas")]
        public string Notas { get; set; }



        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkobras")]
        public string Fkobras { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Incoterm")]
        public string Incoterm { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Descripcionincoterm")]
        public string Descripcionincoterm { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Peso")]
        public int Peso { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Confianza")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public int Confianza { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Costemateriales")]
        public double? Costemateriales { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Tiempooficinatecnica")]
        public double? Tiempooficinatecnica { get; set; }

        public int Decimalesmonedas { get; set; }

        public string Totaldocumento
        {
            get { return (Importetotaldoc ?? 0.0).ToString("N" + Decimalesmonedas); }
        }

        [Required]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkregimeniva")]
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fktransportista")]
        public string Fktransportista { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Tipocambio")]
        public double? Tipocambio { get; set; }

        private PuertoscontrolModel _fkpuertos = new PuertoscontrolModel();

        public PuertoscontrolModel Fkpuertos
        {
            get { return _fkpuertos; }
            set { _fkpuertos = value; }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Unidadnegocio")]
        public string Unidadnegocio { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Referenciadocumento")]
        public string Referenciadocumento { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkbancosmandatos")]
        public string Fkbancosmandatos { get; set; }

        [MaxLength(25, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Cartacredito")]
        public string Cartacredito { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Vencimientocartacredito")]
        public DateTime? Vencimientocartacredito { get; set; }

        [Range(0, 99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Contenedores")]
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

        public List<PedidosComprasLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region Totales

        public List<PedidosComprasTotalesModel> Totales
        {
            get { return _totales; }
            set { _totales = value; }
        }

        #endregion

        #region CTR

        public PedidosComprasModel()
        {

        }

        public PedidosComprasModel(IContextService context) : base(context)
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

        public override string DisplayName => RPedidosCompras.TituloEntidad;
        

        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.PedidosCompras.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.PedidosCompras, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.PedidosCompras,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }

        }
    }
    
    public class PedidosComprasLinModel:ILineaImportar
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
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkarticulos")]
        public string Fkarticulos { get; set; }

        [MaxLength(120,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [MaxLength(12,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Lote")]
        public string Lote { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Tabla")]
        public int? Tabla { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Canal")]
        public string Canal { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Cantidad")]
        public double? Cantidad { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Cantidadpedida")]
        public double? Cantidadpedida { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Largo")]
        public double? Largo { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Ancho")]
        public double? Ancho { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Grueso")]
        public double? Grueso { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Largo")]
        public string SLargo
        {
            get { return (Largo ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Largo = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Ancho")]
        public string SAncho
        {
            get { return (Ancho ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Ancho = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Grueso")]
        public string SGrueso
        {
            get { return (Grueso ?? 0.0).ToString("N" + Decimalesmedidas,CultureInfo.CurrentUICulture); }
            set { Grueso = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fkunidades")]
        public string Fkunidades { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Metros")]
        public double? Metros { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Metros")]
        public string SMetros
        {
            get { return (Metros ?? 0.0).ToString("N" + Decimalesmedidas); }
            set { Metros = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Precio")]
        public double? Precio { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Precio")]
        public string SPrecio {
            //get { return (Precio ?? 0.0).ToString("N" + Decimalesmonedas, CultureInfo.CurrentUICulture); }
            get { return (Precio ?? 0.0).ToString(); }
            set { Precio = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Porcentajedescuento")]
        public double? Porcentajedescuento { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importedescuento")]
        public double? Importedescuento { get; set; }
        
        public string Fkregimeniva { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Cuotaiva")]
        public double? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Cuotarecargoequivalencia")]
        public double? Cuotarecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "ImporteBase")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "ImporteBase")]
        public string SImporte
        {
            get { return (Importe ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importe = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Notas")]
        public string Notas { get; set; }

        

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Precioanterior")]
        public double? Precioanterior { get; set; }

        [MaxLength(1, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RPedidosCompras), Name = "Revision")]
        public string Revision { get; set; }

        //clave ajena
        public int? Fkpresupuestos { get; set; }
        public int? Fkpresupuestosid { get; set; }
        public string Fkpresupuestosreferencia { get; set; }

        public bool EnAlbaran { get; set; }

        public int? Orden
        {
            get { return _orden ?? Id * ApplicationHelper.EspacioOrdenLineas; }
            set { _orden = value; }
        }

        public List<StDocumentoReferencia> Fkalbaranreferencia { get; set; }

        public int? Fkpedidosventas { get; set; }

        public string Fkpedidosventasreferencia { get; set; }

        double ILineaImportar.Cuotaiva
        {
            get
            {
                return Cuotaiva??0;
            }

            set { Cuotaiva = value; }
        }

        double ILineaImportar.Cuotarecargoequivalencia
        {
            get { return Cuotarecargoequivalencia ?? 0; }

            set { Cuotarecargoequivalencia = value; }
        }

        int ILineaImportar.Decimalesmedidas
        {
            get { return Decimalesmedidas??0; }

            set
            {
                Decimalesmedidas = value;
            }
        }

        int ILineaImportar.Decimalesmonedas
        {
            get { return Decimalesmonedas ?? 0; }

            set
            {
                Decimalesmonedas=value;
            }
        }

        public bool Articulocomentario
        {
            get { return false; }

            set
            {
                
            }
        }

        double ILineaImportar.Cantidad
        {
            get { return Cantidad??0; }

            set { Cantidad = value; }
        }

        double ILineaImportar.Metros
        {
            get { return Metros ?? 0; }

            set { Metros = value; }
        }

        double ILineaImportar.Precio
        {
            get { return Precio ?? 0; }

            set { Precio = value; }
        }

        double ILineaImportar.Importe
        {
            get { return Importe ?? 0; }

            set { Importe = value; }
        }

        double ILineaImportar.Importedescuento
        {
            get { return Importedescuento??0; }

            set { Importedescuento = value; }
        }

        double ILineaImportar.Porcentajedescuento
        {
            get
            {
               return Porcentajedescuento??0;
            }

            set { Porcentajedescuento = value; }
        }

        double ILineaImportar.Porcentajeiva
        {
            get { return Porcentajeiva??0; }

            set { Porcentajeiva = value; }
        }

        double ILineaImportar.Porcentajerecargoequivalencia
        {
            get { return Porcentajerecargoequivalencia ?? 0; }

            set
            {
                Porcentajerecargoequivalencia=value;
            }
        }

        double ILineaImportar.Precioanterior
        {
            get { return Precioanterior ?? 0; }

            set { Precioanterior = value; }
        }

        public string Fkdocumento
        {
            get
            {
                return Fkpresupuestos?.ToString()??string.Empty;
            }

            set { Fkpresupuestos = Funciones.Qint(value); }
        }

        public string Fkdocumentoid
        {
            get
            {
                return Fkpresupuestosid?.ToString() ?? string.Empty;
            }

            set { Fkpresupuestosid = Funciones.Qint(value); }
        }

        public string Bundle { get; set; }

        public string Fkdocumentoreferencia
        {
            get { return Fkpresupuestosreferencia; }

            set { Fkpresupuestosreferencia = value; }
        }

        public string Contenedor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Sello { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int? Caja { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double? Pesoneto { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    public class PedidosComprasTotalesModel
    {
        public int? Decimalesmonedas { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Fktiposiva")]
        public string Fktiposiva { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Porcentajeiva")]
        public double? Porcentajeiva { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "BrutoTotal")]
        public double? Brutototal { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "BrutoTotal")]
        public string SBrutototal {
            get { return (Brutototal??0.0).ToString("N" + Decimalesmonedas); }
            set { Brutototal = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Basetotal")]
        public double? Baseimponible { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Basetotal")]
        public string SBaseimponible
        {
            get { return (Baseimponible ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Baseimponible = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Cuotaiva")]
        public double ? Cuotaiva { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Cuotaiva")]
        public string SCuotaiva
        {
            get { return (Cuotaiva ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Cuotaiva = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Porcentajerecargoequivalencia")]
        public double? Porcentajerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Cuotarecargoequivalencia")]
        public double? Importerecargoequivalencia { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Cuotarecargoequivalencia")]
        public string SImporterecargoequivalencia
        {
            get { return (Importerecargoequivalencia ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importerecargoequivalencia = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Porcentajedescuentoprontopago")]
        public double? Porcentajedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importedescuentoprontopago")]
        public double? Importedescuentoprontopago { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importedescuentoprontopago")]
        public string SImportedescuentoprontopago
        {
            get { return (Importedescuentoprontopago ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentoprontopago = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Porcentajedescuentocomercial")]
        public double? Porcentajedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importedescuentocomercial")]
        public double? Importedescuentocomercial { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Importedescuentocomercial")]
        public string SImportedescuentocomercial
        {
            get { return (Importedescuentocomercial ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Importedescuentocomercial = Funciones.Qdouble(value); }
        }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Subtotal")]
        public double? Subtotal { get; set; }

        [Display(ResourceType = typeof(RPedidosCompras), Name = "Subtotal")]
        public string SSubtotal
        {
            get { return (Subtotal ?? 0.0).ToString("N" + Decimalesmonedas); }
            set { Subtotal = Funciones.Qdouble(value); }
        }

      
    }

    public class PedidosComprasLinImportarModel : PedidosComprasLinModel
    {
        public int? FkPedidosCompras { get; set; }
        public string FkPedidosComprasreferencia { get; set; }
        public int? Fkmonedas { get; set; }
        public PedidosComprasModel Cabecera { get; set; }

    }

}
