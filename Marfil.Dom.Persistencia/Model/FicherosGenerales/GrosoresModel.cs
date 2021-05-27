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
using RGrosores= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Grosores;
namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class GrosoresModel : BaseModel<GrosoresModel, Grosores>
    {
        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Key]
        [Required]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType=typeof(RGrosores),Name = "Id")]
        public string Id { get; set; }

        [Required]
        [CustomDisplayDescription(typeof(RGrosores), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Descripcion { get; set; }

        [CustomDisplayDescription(typeof(RGrosores), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Descripcion2 { get; set; }

        [Display(ResourceType = typeof(RGrosores), Name = "Descripcionabreviada")]
        public string Descripcionabreviada { get; set; }

        [Required]
        [Display(ResourceType = typeof(RGrosores), Name = "Grosor")]
        public double? Grosor { get; set; }

        [Display(ResourceType = typeof(RGrosores), Name = "Coficientecortabloques")]
        public double? Coficientecortabloques { get; set; }

        [Display(ResourceType = typeof(RGrosores), Name = "Coeficientetelares")]
        public double? Coeficientetelares { get; set; }

        /*[Display(ResourceType = typeof(RGrosores), Name = "Grosor")]
        public string GrosorCadena {
            get { return (Grosor ?? 0.0).ToString("N3"); }
        }

        [Display(ResourceType = typeof(RGrosores), Name = "Coficientecortabloques")]
        public string CoficientecortabloquesCadena
        {
            get
            {
                return (Coficientecortabloques ?? 0.0).ToString("N3");
            }
        }

        [Display(ResourceType = typeof(RGrosores), Name = "Coeficientetelares")]
        public string CoeficientetelaresCadena {
            get
            {
                return (Coeficientetelares ?? 0.0).ToString("N3");
            }
        }*/

        #endregion

        #region CTR

        public GrosoresModel()
        {

        }

        public GrosoresModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RGrosores.TituloEntidad;
    }
}
