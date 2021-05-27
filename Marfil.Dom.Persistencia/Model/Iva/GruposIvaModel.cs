using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RGruposIva= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Gruposiva;
namespace Marfil.Dom.Persistencia.Model.Iva
{
    public class GruposIvaLinModel
    {
       
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RGruposIva), Name = "FkGruposIva")]
        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string FkGruposIva { get; set; }

        [Key]
        [Required]
        [Display(ResourceType = typeof(RGruposIva), Name = "Id")]
        public int Id { get; set ; }

        [Required]
        [Display(ResourceType = typeof(RGruposIva), Name = "Desde")]
        public DateTime  Desde { get; set; }
        
        [Required]
        [Display(ResourceType = typeof(RGruposIva), Name = "FkTiposIvaSinRecargo")]
        public string FkTiposIvaSinRecargo { get; set; }

        
        [Required]
        [Display(ResourceType = typeof(RGruposIva), Name = "FkTiposIvaConRecargo")]
        public string FkTiposIvaConRecargo { get; set; }

        [Display(ResourceType = typeof(RGruposIva), Name = "FkTiposIvaExentoIva")]
        public string FkTiposIvaExentoIva { get; set; }

    }

    public class GruposIvaModel : BaseModel<GruposIvaModel, GruposIva>
    {
       

        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RGruposIva), Name = "Id")]
        [Required]
        [MaxLength(4,ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RGruposIva), Name = "Descripcion")]
        public string Descripcion { get; set; }

        private List<GruposIvaLinModel> _lineas= new List<GruposIvaLinModel>();
        public List<GruposIvaLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion
        
        #region CTR

        public GruposIvaModel()
        {

        }

        public GruposIvaModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RGruposIva.TituloEntidad;
    }
}
