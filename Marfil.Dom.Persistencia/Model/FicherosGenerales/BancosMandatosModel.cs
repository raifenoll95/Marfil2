using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.ControlsUI.Bloqueo;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RBancosMandatos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.BancosMandatos;
namespace Marfil.Dom.Persistencia.Model.Configuracion.Cuentas
{
    public enum TipoSecuenciaSepa
    {
        [StringValue(typeof(RBancosMandatos), "TipoSecuenciaSepaPeriodico")]
        Periodico,
        [StringValue(typeof(RBancosMandatos), "TipoSecuenciaSepaUnico")]
        Unico
    }

    public enum TipoAdeudo
    {
        [StringValue(typeof(RBancosMandatos), "TipoAdeudoPrimerAdeudoRecurrente")]
        FRST,
        [StringValue(typeof(RBancosMandatos), "TipoAdeudoAdeudoRecurrente")]
        RCUR,
        [StringValue(typeof(RBancosMandatos), "TipoAdeudoUltimoAdeudoRecurrente")]
        FNAL,
        [StringValue(typeof(RBancosMandatos), "TipoAdeudoUnico")]
        OOFF
    }

    public enum Esquema
    {
        CORE,
        B2B,
        COR1
    }

    public enum TipoBancosMandatos
    {
        [StringValue(typeof(RBancosMandatos), "TiposBancosMandatosBanco")]
        Banco,
        [StringValue(typeof(RBancosMandatos), "TipoBancosMandatosMandato")]
        Mandato
    }
   
    public class BancosMandatosModel
    {
        public string Empresa { get; set; }
        public TipoBancosMandatos Tipo { get; set; }
        public Guid Id { get; set; }
        public List<BancosMandatosLinModel> BancosMandatos { get; set; }
        public bool ReadOnly { get; set; }
    }

    public class BancosMandatosLinModel : BaseModel<BancosMandatosLinModel, BancosMandatos>
    {
        #region Properties
      
        

        public string CampoId
        {
            get { return Fkcuentas + "-" + Id; }
        }

        
        public string Empresa { get; set; }


        [Display(ResourceType = typeof(RBancosMandatos), Name = "Fkcuentas")]
        public string Fkcuentas { get; set; }


        [Display(ResourceType = typeof(RBancosMandatos), Name = "Id")]
        public string Id { get; set; }

        public int? Idnumerico
        {
            get { return Funciones.Qint(Id); }
        }

        [Required]
        [Display(ResourceType = typeof(RBancosMandatos), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Display(ResourceType = typeof(RBancosMandatos), Name = "Fkpaises")]
        public string Fkpaises { get; set; }

        [Required]
        [Display(ResourceType = typeof(RBancosMandatos), Name = "Iban")]
        [MaxLength(30, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [RegularExpression("[a-zA-Z]{2}[0-9]{2}[a-zA-Z0-9]{4}[0-9]{7}([a-zA-Z0-9]?){0,16}", ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "PropertyValueInvalid")]
        public string Iban { get; set; }

        [Required]
        [Display(ResourceType = typeof(RBancosMandatos), Name = "Bic")]
        [MaxLength(12, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [RegularExpression("^([a-zA-Z]){4}([a-zA-Z]){2}([0-9a-zA-Z]){2}([0-9a-zA-Z]{3})?$", ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "PropertyValueInvalid")]
        public string Bic { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Sufijoacreedor")]
        public string Sufijoacreedor { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Contratoconfirmig")]
        public string Contratoconfirmig { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Contadorconfirming")]
        public string Contadorconfirming { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Riesgonacional")]
        public string Riesgonacional { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Riesgoextranjero")]
        public string Riesgoextranjero { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Direccion")]
        public string Direccion { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Cpostal")]
        public string Cpostal { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Ciudad")]
        public string Ciudad { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Fkprovincias")]
        public string Fkprovincias { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Telefonobanco")]
        public string Telefonobanco { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Personacontacto")]
        public string Personacontacto { get; set; }

        //clientes

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Idmandato")]
        public string Idmandato { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Idacreedor")]
        public string Idacreedor { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Tiposecuenciasepa")]
        public TipoSecuenciaSepa? Tiposecuenciasepa { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Tipoadeudo")]
        public TipoAdeudo? Tipoadeudo { get; set; }

        public string Tipoadeudocadena
        {
            get { return Tipoadeudo.HasValue ? Funciones.GetEnumByStringValueAttribute(Tipoadeudo) : string.Empty; }
        }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Importemandato")]
        public double? Importemandato { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Recibosmandato")]
        public int? Recibosmandato { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Importelimiterecibo")]
        public double? Importelimiterecibo { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Fechafirma")]
        public DateTime? Fechafirma { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Fechaexpiracion")]
        public DateTime? Fechaexpiracion { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Fechaultimaremesa")]
        public DateTime? Fechaultimaremesa { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Importeremesados")]
        public double? Importeremesados { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Recibosremesados")]
        public int? Recibosremesados { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Devolvera")]
        public string Devolvera { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Notas")]
        public string Notas { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Defecto")]
        public bool Defecto { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Finalizado")]
        public bool Finalizado { get; set; }


        [Display(ResourceType = typeof(RBancosMandatos), Name = "Bloqueado")]
        public bool Bloqueado { get; set; }

        [Display(ResourceType = typeof(RBancosMandatos), Name = "Esquema")]
        public Esquema? Esquema { get; set; }

        public string Esquemacadena
        {
            get { return Esquema.HasValue ? Funciones.GetEnumByStringValueAttribute(Esquema) : string.Empty; }
        }

        #endregion

        #region CTR

        public BancosMandatosLinModel()
        {

        }

        public BancosMandatosLinModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id.Split('-');
        }

        public override string DisplayName => "Bancos/Mandatos";
    }
}
