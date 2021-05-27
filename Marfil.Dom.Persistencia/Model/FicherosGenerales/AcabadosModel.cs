using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Resources;
using RAcabados= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Acabados;
namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class AcabadosModel : BaseModel<AcabadosModel, Acabados>
    {
        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Key]
        [Required]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType=typeof(RAcabados),Name = "Id")]
        public string Id { get; set; }

        [Required]
        [CustomDisplayDescription(typeof(RAcabados), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Descripcion { get; set; }
        
        [CustomDisplayDescription(typeof(RAcabados), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Descripcion2 { get; set; }
        
        [Display(ResourceType = typeof(RAcabados), Name = "Descripcionabreviada")]
        [MaxLength(20, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Descripcionabreviada { get; set; }

        [Display(ResourceType = typeof(RAcabados), Name = "Bruto")]
        public bool Bruto { get; set; }

        #endregion

        #region CTR

        public AcabadosModel()
        {

        }

        public AcabadosModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RAcabados.TituloEntidad;
    }
}
