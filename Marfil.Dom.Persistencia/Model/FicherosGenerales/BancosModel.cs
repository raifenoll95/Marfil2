using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RBancos= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Bancos;
namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class BancosModel : BaseModel<BancosModel, Bancos>
    {
        #region Properties

        [Key]
        [Required]
        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [MinLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        [Display(ResourceType=typeof(RBancos),Name = "Id")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RBancos), Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(ResourceType = typeof(RBancos), Name = "Bic")]
        [MaxLength(12, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [RegularExpression("^([a-zA-Z]){4}([a-zA-Z]){2}([0-9a-zA-Z]){2}([0-9a-zA-Z]{3})?$", ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "PropertyValueInvalid")]
        public string Bic { get; set; }

        #endregion

        #region CTR

        public BancosModel()
        {

        }

        public BancosModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RBancos.TituloEntidad;
    }
}
