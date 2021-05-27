using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RTiposIva = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Tiposiva;
namespace Marfil.Dom.Persistencia.Model.Iva
{
    public class TiposIvaModel : BaseModel<TiposIvaModel, TiposIva>
    {
        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposIva),Name="Id")]
        [MaxLength(3,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposIva), Name = "Nombre")]
        public string Nombre { get; set; }


        [Required]
        [Display(ResourceType = typeof(RTiposIva), Name = "PorcentajeIva")]
        [Range(0,100,ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double PorcentajeIva { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposIva), Name = "PorcentajeRecargoEquivalencia")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double PorcentajeRecargoEquivalencia { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposIva), Name = "CtaIvaSoportado")]
        public string CtaIvaSoportado { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposIva), Name = "CtaIvaRepercutido")]
        public string CtaIvaRepercutido { get; set; }

        [Display(ResourceType = typeof(RTiposIva), Name = "CtaIvaNoDeducible")]
        public string CtaIvaNoDeducible { get; set; }

        [Display(ResourceType = typeof(RTiposIva), Name = "CtaRecargoEquivalenciaRepercutido")]
        public string CtaRecargoEquivalenciaRepercutido { get; set; }

        [Display(ResourceType = typeof(RTiposIva), Name = "CtaIvaSoportadoCriterioCaja")]
        public string CtaIvaSoportadoCriterioCaja { get; set; }

        [Display(ResourceType = typeof(RTiposIva), Name = "CtaIvaRepercutidoCriterioCaja")]
        public string CtaIvaRepercutidoCriterioCaja { get; set; }

        [Display(ResourceType = typeof(RTiposIva), Name = "CtaRecargoEquivalenciaRepercutidoCriterioCaja")]
        public string CtaRecargoEquivalenciaRepercutidoCriterioCaja { get; set; }

        [Display(ResourceType = typeof(RTiposIva), Name = "IvaSoportado")]
        public bool IvaSoportado { get; set; }

        [Display(ResourceType = typeof(RTiposIva), Name = "IvaDeducible")]
        public bool IvaDeducible { get; set; }

        [Display(ResourceType = typeof(RTiposIva), Name = "PorcentajeDeducible")]
        [Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double PorcentajeDeducible { get; set; }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RTiposIva.TituloEntidad;

        #region CTR

        public TiposIvaModel()
        {

        }

        public TiposIvaModel(IContextService context) : base(context)
        {

        }

        #endregion

        #region Api

        public static IEnumerable<GruposIvaModel> GetGruposIva(IContextService context,string id)
        {
            using (var gruposivaService = FService.Instance.GetService(typeof (GruposIvaModel), context) as GruposIvaService)
            {
                return gruposivaService.GetGruposWithTipoIva(id);
            }
        }

        #endregion
    }
}
