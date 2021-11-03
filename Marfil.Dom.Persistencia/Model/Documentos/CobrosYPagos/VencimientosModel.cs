using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RCobrosYPagos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CobrosYPagos;
using RFacturas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Facturas;
using RPedidos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Pedidos;
using Marfil.Inf.Genericos;
using Marfil.Dom.ControlsUI.Toolbar;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using System.Collections.Generic;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;

namespace Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos
{
    public enum TipoVencimiento
    {
        [StringValue(typeof(RCobrosYPagos), "Cobros")]
        Cobros,
        [StringValue(typeof(RCobrosYPagos), "Pagos")]
        Pagos
    }

    public enum TipoOrigen
    {
        [StringValue(typeof(RCobrosYPagos), "EntradaManual")]
        EntradaManual,
        [StringValue(typeof(RCobrosYPagos), "RegIVA")]
        RegIVA,
        [StringValue(typeof(RCobrosYPagos), "FacturaCompra")]
        FacturaCompra,
        [StringValue(typeof(RCobrosYPagos), "FacturaVenta")]
        FacturaVenta,
        [StringValue(typeof(RCobrosYPagos), "FacturaConciliada")]
        FacturaConciliada,
        [StringValue(typeof(RCobrosYPagos), "ConciliarAsiento")]
        ConciliarAsiento      
    }

    public enum TipoEstado
    {
        [StringValue(typeof(RCobrosYPagos), "Inicial")]
        Inicial,
        [StringValue(typeof(RCobrosYPagos), "Cubierto")]
        CubiertoParcial,
        [StringValue(typeof(RCobrosYPagos), "Total")]
        Total
    }

    public enum TipoEstadoRemesa
    {
        [StringValue(typeof(RCobrosYPagos), "Valida")]
        Valida,
        [StringValue(typeof(RCobrosYPagos), "Anulada")]
        Anulada
    }

    public enum TipoSituacion
    {
        [StringValue(typeof(RCobrosYPagos), "Inicial")]
        Inicial,
        [StringValue(typeof(RCobrosYPagos), "Pagado")]
        Pagado
    }

    public class VencimientosModel : BaseModel<VencimientosModel, Persistencia.Vencimientos>
    {

        private List<CarteraVencimientosModel> _cartera = new List<CarteraVencimientosModel>();

        #region CTR
        public VencimientosModel()
        {
        }

        public VencimientosModel(IContextService context) : base(context)
        {
        }
        #endregion

        #region properties

        public int? Id { get; set; }

        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fechadocumento")]
        public DateTime? Fecha { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidos), Name = "Fkseries")]
        public string Fkseriescontables { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Referencia")]
        public string Referencia { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        //Factura a la que hace referecia
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkfacturas")]
        public String Traza { get; set; }

        //Cobros o Pagos (C/P)
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Tipo")]
        public TipoVencimiento Tipo { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Origen")]
        public TipoOrigen Origen { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Usuario")]
        public String Usuario { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "FkcuentasDescripcion")]
        public string Descripcioncuenta { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechacreacion")]
        public DateTime? Fechacreacion { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechafactura")]
        public DateTime? Fechafactura { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechafactura")]
        public string FechaStrfactura { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fecharegistrofactura")]
        public DateTime? Fecharegistrofactura { get; set; }

        [Required]
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechavencimiento")]
        public DateTime? Fechavencimiento { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechavencimiento")]
        public string FechaStrvencimiento { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechadescuento")]
        public DateTime? Fechadescuento { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechapago")]
        public DateTime? Fechapago { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Monedabase")]
        public int? Monedabase { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Monedagiro")]
        public int? Monedagiro { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Importegiro")]
        public double? Importegiro { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Monedafactura")]
        public int? Monedafactura { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkcuentastesoreria")]
        public String Fkcuentatesoreria { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Mandato")]
        public String Mandato { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Importeasignado")]
        public double? Importeasignado { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Importeasignado")]
        public double? Importepagado { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Estado")]
        public TipoEstado Estado { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Situación")]
        public String Situacion { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Situación")]
        public string Situaciondescripcion { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Comentario")]
        public String Comentario { get; set; }

        public string urlDocumento { get; set; }

        public string urlDocumentoGeneral { get; set; }

        public string FormaPago { get; set; }

        public List<CarteraVencimientosModel> LineasCartera
        {
            get { return _cartera; }
            set { _cartera = value; }
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

        public string CambiarNombre { get; set; }

        public override string DisplayName => CambiarNombre;

        #endregion
    }

    public class CarteraVencimientosModel : BaseModel<CarteraVencimientosModel, Persistencia.CarteraVencimientos>, IDocument, IGaleria
    {

        //Lineas
        private List<PrevisionesCarteraModel> _lineascartera = new List<PrevisionesCarteraModel>();
        private List<VencimientosModel> _previsiones = new List<VencimientosModel>();

        #region CTR
        public CarteraVencimientosModel()
        {
        }

        public CarteraVencimientosModel(IContextService context) : base(context)
        {
        }
        #endregion

        #region properties

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        //Cobros o Pagos (C/P)
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Tipo")]
        public TipoVencimiento Tipovencimiento { get; set; }

        public int? Id { get; set; }

        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPedidos), Name = "Fkseries")]
        public string Fkseriescontables { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Referencia")]
        public string Referencia { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Identificadorsegmento")]
        public string Identificadorsegmento { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Identificadorsegmento")]
        public string Identificadorsegmentoremesa { get; set; }

        //Factura a la que hace referecia
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkfacturas")]
        public String Traza { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Usuario")]
        public String Usuario { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "FkcuentasDescripcion")]
        public string Descripcioncuenta { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "FechaCreacionCartera")]
        public DateTime? Fechacreacion { get; set; }

        [Required]
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechavencimiento")]
        public DateTime? Fechavencimiento { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechadescuento")]
        public DateTime? Fechadescuento { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechapago")]
        public DateTime? Fechapago { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Monedabase")]
        public int? Monedabase { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Monedagiro")]
        public int? Monedagiro { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Importegiro")]
        public double? Importegiro { get; set; }

        public string Importeletra { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Cambioaplicado")]
        public double? Cambioaplicado { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkcuentastesoreria")]
        public String Fkcuentastesoreria { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Mandato")]
        public String Mandato { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Comentario")]
        public String Comentario { get; set; }

        public string urlDocumento { get; set; }

        public String Codigoremesa { get; set; }

        public int? Tiponumerofactura { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fechadocumento")]
        public DateTime? Fecha { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Situación")]
        public String Situacion { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Situación")]
        public string Situaciondescripcion { get; set; }

        public double? Imputadoaux { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Banco")]
        public string Banco { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Letra")]
        public string Letra { get; set; }

        [Display(ResourceType = typeof(RPedidos), Name = "Fkseries")]
        public string Fkseriescontablesremesa { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Referenciaremesa")]
        public string Referenciaremesa { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fecharemesa")]
        public DateTime? Fecharemesa { get; set; }

        #endregion

        #region lineas
        public List<PrevisionesCarteraModel> LineasCartera
        {
            get { return _lineascartera; }
            set { _lineascartera = value; }
        }

        public List<VencimientosModel> LineasPrevisiones
        {
            get { return _previsiones; }
            set { _previsiones = value; }
        }

        #endregion

        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = base.Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.CarteraVencimientos.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(Context, db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.CarteraVencimientos, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.CarteraVencimientos,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referencia,
                        Defecto = doc?.Name
                    };
                }
            }

        }


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

        public Configuracion.TipoEstado Tipoestado(IContextService context)
        {
            throw new NotImplementedException();
        }

        public override string DisplayName => RCobrosYPagos.TituloCartera;

        public GaleriaModel Galeria => throw new NotImplementedException();

        public int Fkejercicio { get; set; }
        public string Fkestados { get; set; }

        #endregion
    }

    public class ToolbarAsistenteAsignacionModel : ToolbarModel
    {
        public ToolbarAsistenteAsignacionModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RCobrosYPagos.AsignarCartera;
        }

        public override string GetCustomTexto()
        {
            return RCobrosYPagos.AsistenteAsignacionCartera;
        }
    }

    public class AsistenteAsignacionModel: IToolbar
    {
        #region Members

        private ToolbarModel _toolbar;

        #endregion

        #region Properties

        public ToolbarModel Toolbar
        {
            get { return _toolbar; }
            set { _toolbar = value; }
        }

        //Cobros o Pagos (C/P) Pantalla 1
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Tipo")]
        public TipoVencimiento Tipo { get; set; }

        //Pantalla 2
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Circuitotesoreria")]
        public string Circuitotesoreria { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Importe")]
        public double? Importe { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "SituacionFinal")]
        public string SituacionFinal { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "CuentaTesoreria")]
        public string Fkcuentatesoreria { get; set; }

        [Display(ResourceType = typeof(RFacturas), Name = "Fkformaspago")]
        public int? Fkformaspago { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "FechaContable")]
        public DateTime? FechaContable { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Banco")]
        public string Banco { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Letra")]
        public string Letra { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fechavencimiento")]
        public DateTime? FechaVencimiento { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Mandato")]
        public string Mandato { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Comentario")]
        public string Comentario { get; set; }

        #endregion  


        public AsistenteAsignacionModel(IContextService context)
        {
            _toolbar = new ToolbarAsistenteAsignacionModel();
        }

        public AsistenteAsignacionModel()
        {
        }
    }

    public class PrevisionesCarteraModel : BaseModel<PrevisionesCarteraModel, Persistencia.PrevisionesCartera>
    {

        #region CTR
        public PrevisionesCarteraModel()
        {
        }

        public PrevisionesCarteraModel(IContextService context) : base(context)
        {
        }
        
        #endregion

        #region properties

        public string Empresa { get; set; }

        public int? Id { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Idvencimiento")]
        public int? Codvencimiento { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Idcartera")]
        public int? Codcartera{ get; set; }

        public double? Imputado { get; set; }

        public override string DisplayName => RCobrosYPagos.TituloCartera;

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

        #endregion
    }

}
