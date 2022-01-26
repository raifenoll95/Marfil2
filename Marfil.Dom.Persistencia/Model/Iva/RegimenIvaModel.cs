using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Resources;
using RRegimenIva=Marfil.Inf.ResourcesGlobalization.Textos.Entidades.RegimenIva;
namespace Marfil.Dom.Persistencia.Model.Iva
{
    public enum Sra
    {
        [StringValue(typeof(RRegimenIva),"SraSoportado")]
        Soportado,
        [StringValue(typeof(RRegimenIva), "SraRepercutido")]
        Repercutido,
        [StringValue(typeof(RRegimenIva), "SraAmbos")]
        Ambos
    }

    public class RegimenIvaModel : BaseModel<RegimenIvaModel, RegimenIva>
    {
        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RRegimenIva), Name = "Id")]
        [MaxLength(5, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RRegimenIva), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "Normal")]
        public bool Normal { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "Recargo")]
        public bool Recargo { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "ExentoTasa")]
        public bool ExentoTasa { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "OperacionUe")]
        public bool OperacionUe { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "InversionSujetoPasivo")]
        public bool InversionSujetoPasivo { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "OperacionesNoSujetas")]
        public bool OperacionesNoSujetas { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "ZonasEspeciales")]
        public bool ZonasEspeciales { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "CanariasIgic")]
        public bool CanariasIgic { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "Extranjero")]
        public bool Extranjero { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "IvaDiferido")]
        public bool IvaDiferido { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "IvaImportacion")]
        public bool IvaImportacion { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "IncompatibleCriterioCaja")]
        public Sra? IncompatibleCriterioCaja { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "SoportableRepercutidoAmbos")]
        public Sra SoportableRepercutidoAmbos { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "BienInversion")]
        public bool BienInversion { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "ExentoVentas")]
        public bool ExentoVentas { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "ClaveOperacion340")]
        public string ClaveOperacion340 { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "Exportacion")]
        public bool Exportacion { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "Incluirmodelo347")]
        public bool Incluirmodelo347 { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "TipoFacturaEmitida")]
        public string TipoFacturaEmitida { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "RegimenEspecialEmitida")]
        public string RegimenEspecialEmitida { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "TipoFacturaRecibida")]
        public string TipoFacturaRecibida { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "RegimenEspecialRecibida")]
        public string RegimenEspecialRecibida { get; set; }

        [Display(ResourceType = typeof(RRegimenIva), Name = "Tipooperacionclassic")]
        [Range(1, 99)]
        public int Tipooperacionclassic { get; set; }

        #endregion

        #region CTR

        public RegimenIvaModel()
        {

        }

        public RegimenIvaModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RRegimenIva.TituloEntidad;
       
    }
}

