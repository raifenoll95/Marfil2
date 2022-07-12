using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RCobrosYPagos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CobrosYPagos;
using RFacturas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Facturas;
using RCrm = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CRM;
using RPedidos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Pedidos;
using Marfil.Inf.Genericos;
using Marfil.Dom.ControlsUI.Toolbar;
using RAlbaranesCompras = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.AlbaranesCompras;
using System.Collections.Generic;
using Marfil.Inf.Genericos.Helper;

namespace Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos
{

    public class StAsistenteTesoreria
    {
        public string Tipo { get; set; }
        public string Circuitotesoreria { get; set; }
        public string Fkformaspago { get; set; }
        public string Fkcuentas { get; set; }
        public string TerceroDesde { get; set; }
        public string TerceroHasta { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string FechaContable { get; set; }
        public string Fecharemesa { get; set; }
        public string Referenciaremesa { get; set; }

        public string Vencimientos { get; set; }

        public string Fkcuentatesoreria { get; set; }
        public string Fkseriescontables { get; set; }
        public string ImporteCargo2 { get; set; }
        public string ImporteAbono2 { get; set; }
        public string Banco { get; set; }
        public string Letra { get; set; }
        public string FechaPago { get; set; }
        public string Comentario { get; set; }

        public string ImportePantalla3 { get; set; }
        public string Importe { get; set; }
        public string Situacion { get; set; }
        public string FechaVencimiento { get; set; }
    }

    public class ToolbarAsistenteMovimientosTesoreriaModel : ToolbarModel
    {
        public ToolbarAsistenteMovimientosTesoreriaModel()
        {
            Operacion = TipoOperacion.Custom;
            Titulo = RCobrosYPagos.MovimientosTesoreria;
        }

        public override string GetCustomTexto()
        {
            return RCobrosYPagos.AsistenteMovimientosTesoreria;
        }
    }

    public class AsistenteMovimientosTesoreriaModel: IToolbar
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

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkmodospago")]
        public string Fkmodospago { get; set; }

        [Display(ResourceType = typeof(RCrm), Name = "TerceroDesde")]
        public string TerceroDesde { get; set; }

        [Display(ResourceType = typeof(RCrm), Name = "TerceroHasta")]
        public string TerceroHasta { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "FechaDesde")]
        public DateTime? FechaDesde { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "FechaHasta")]
        public DateTime? FechaHasta { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Importe")]
        public int? Importe { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fkseries")]
        public string Fkseries { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Fecharemesa")]
        public DateTime? FechaRemesa { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Referenciaremesa")]
        public string ReferenciaRemesa { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "SituacionFinal")]
        public string SituacionFinal { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "CuentaTesoreria")]
        public string Fkcuentatesoreria { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "FechaContable")]
        public DateTime? FechaContableFiltro { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "FechaContable")]
        public DateTime? FechaContable { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "FechaPagoActualizar")]
        public DateTime? FechaPago { get; set; }

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

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Cuentacargo2")]
        public string Cuentacargo2 { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Importe2Importe3")]
        public string ImporteCargo2 { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Cuentaabono2")]
        public string Cuentaabono2 { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Importe2Importe6")]
        public string ImporteAbono2 { get; set; }

        #endregion  

        public AsistenteMovimientosTesoreriaModel(IContextService context)
        {
            _toolbar = new ToolbarAsistenteMovimientosTesoreriaModel();
        }

        public AsistenteMovimientosTesoreriaModel()
        {
        }
    }

    //Se ha tenido que crear esta clase porque en este grid puede que salgan tanto registros de prevision como de cartera
    public class GridAsistenteMovimientosTesoreriaModel : BaseModel<GridAsistenteMovimientosTesoreriaModel, Persistencia.Vencimientos>
    {
        #region CTR
        public GridAsistenteMovimientosTesoreriaModel()
        {
        }

        public GridAsistenteMovimientosTesoreriaModel(IContextService context) : base(context)
        {
        }
        #endregion

        public int? Id { get; set; }
        public int? Tipo { get; set; }
        public string Referencia { get; set; }
        public string Fkcuentas { get; set; }
        public string FkcuentasDescripcion { get; set; }
        public string Traza { get; set; }
        public string Fechavencimiento { get; set; }
        public string FechaStrfactura { get; set; }
        public string FechaStrvencimiento { get; set; }
        public double? Importegiro { get; set; }
        public string SImportegiro
        {
            get { return (Importegiro ?? 0).ToString(string.Format("N{0}", (2))); }
            set { Importegiro = (Funciones.Qdouble(value) ?? 0); }
        }
        public double? ImporteAsignado { get; set; }
        public string Situacion { get; set; }
        public string Fkformaspago { get; set; }
        public string FkcuentaTesoreria { get; set; }
        public string Fkseriescontables { get; set; }
        public string ReferenciaRemesa { get; set; }

        public override string DisplayName => RCobrosYPagos.TituloCartera;

        public override object generateId(string id)
        {
            return id;
        }
    }
}
