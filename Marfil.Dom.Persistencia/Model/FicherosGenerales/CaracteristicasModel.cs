using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RCaracteristicas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Caracteristicas;
namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class CaracteristicasLinModel
    {
        [Key]
        [Required]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        [Display(ResourceType = typeof(RCaracteristicas), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [CustomDisplayDescription(typeof(RCaracteristicas), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        public string Descripcion { get; set; }

        [CustomDisplayDescription(typeof(RCaracteristicas), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        public string Descripcion2 { get; set; }

        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RCaracteristicas), Name = "Descripcionabreviada")]
        public string Descripcionabreviada { get; set; }
    }

    public class CaracteristicasModel : BaseModel<CaracteristicasModel, Caracteristicas>
    {
        private List<CaracteristicasLinModel> _lineas=new List<CaracteristicasLinModel>();

        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Key]
        [Required]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        [Display(ResourceType = typeof(RCaracteristicas), Name = "Id")]
        public string Id { get; set; }

        [Display(ResourceType = typeof(RCaracteristicas), Name = "Descripcion")]
        public string Descripcion { get; set; }

        public List<CaracteristicasLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region CTR

        public CaracteristicasModel()
        {

        }

        public CaracteristicasModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RCaracteristicas.TituloEntidad;
    }
}
