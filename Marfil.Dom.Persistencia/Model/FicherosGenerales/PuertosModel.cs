using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RPuertos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Puertos;
namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class PuertoscontrolModel
    {
        
        
        [Display(ResourceType = typeof(RPuertos), Name = "Fkpaises")]
        public string Fkpaises { get; set; }

        [Display(ResourceType = typeof(RPuertos), Name = "Puerto")]
        public string Id { get; set; }

       
    }
    public class PuertosModel : BaseModel<PuertosModel, Puertos>
    {
        #region Properties

        public string CustomId
        {
            get { return Fkpaises + "-" + Id; }
        }

        [Required]
        [Display(ResourceType = typeof(RPuertos), Name = "Fkpaises")]
        public string Fkpaises { get; set; }

        [Display(ResourceType = typeof(RPuertos), Name = "DescripcionPais")]
        public string DescripcionPais { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPuertos), Name = "Id")]
        [MaxLength(4,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RPuertos), Name = "Descripcion")]
        public string Descripcion { get; set; }

        #endregion

        #region CTR

        public PuertosModel()
        {

        }

        public PuertosModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id.Split('-');
        }

        public override string GetPrimaryKey()
        {
            return CustomId;
        }

        public override string DisplayName => RPuertos.TituloEntidad;
       
    }
}
