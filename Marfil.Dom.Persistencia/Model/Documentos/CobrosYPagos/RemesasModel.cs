using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RCobrosYPagos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.CobrosYPagos;
using RFacturas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Facturas;
using RPedidos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;

namespace Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos
{
    public class RemesasModel : BaseModel<RemesasModel, Remesas>, IDocument, IGaleria
    {

        #region CTR
        public RemesasModel()
        {
        }

        public RemesasModel(IContextService context) : base(context)
        {
        }
        #endregion

        #region properties

        public DocumentosBotonImprimirModel DocumentosImpresion { get; set; }

        //Cobros o Pagos (C/P)
        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Tipo")]
        public TipoVencimiento Tipovencimiento { get; set; }
        
        [Required]
        public int Id { get; set; }
        [Required]
        public string Empresa { get; set; }

        
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

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Cuadernos")]
        public string Cuadernos { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "Ruta")]
        public string Ruta { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "NumeroDocumentos")]
        public int NumeroDocumentos { get; set; }

        [Display(ResourceType = typeof(RCobrosYPagos), Name = "ImporteRemesa")]
        public double ImporteRemesa { get; set; }

        #endregion

        public override string DisplayName => RCobrosYPagos.TituloRemesa;

        public int Fkejercicio { get; set; }
        public string Fkestados { get; set; }

        public GaleriaModel Galeria => throw new NotImplementedException();

        public override object generateId(string id)
        {
            return id;
        }

        public DocumentosBotonImprimirModel GetListFormatos()
        {
            var user = base.Context;
            using (var db = MarfilEntities.ConnectToSqlServer(user.BaseDatos))
            {
                var servicePreferencias = new PreferenciasUsuarioService(db);
                var doc = servicePreferencias.GetDocumentosImpresionMantenimiento(user.Id, TipoDocumentoImpresion.Remesa.ToString(), "Defecto") as PreferenciaDocumentoImpresionDefecto;
                var service = new DocumentosUsuarioService(db);
                {
                    var lst =
                        service.GetDocumentos(TipoDocumentoImpresion.Remesa, user.Id)
                            .Where(f => f.Tiporeport == TipoReport.Report);
                    return new DocumentosBotonImprimirModel()
                    {
                        Tipo = TipoDocumentoImpresion.Remesa,
                        Lineas = lst.Select(f => f.Nombre),
                        Primarykey = Referenciaremesa,
                        Defecto = doc?.Name
                    };
                }
            }

        }

        public Configuracion.TipoEstado Tipoestado(IContextService context)
        {
            throw new NotImplementedException();
        }
    }
}
