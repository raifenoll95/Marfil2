using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using Resources;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using RSeccionesAnaliticas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.SeccionesAnaliticas;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class SeccionesanaliticasModel : BaseModel<SeccionesanaliticasModel, Seccionesanaliticas>
    {
        #region CTR
        public SeccionesanaliticasModel() : base()
        {

        }

        public SeccionesanaliticasModel(IContextService context):base(context)
        {

        }
        #endregion


        #region Properties
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RSeccionesAnaliticas), Name = "Id")]
        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RSeccionesAnaliticas), Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(ResourceType = typeof(RSeccionesAnaliticas), Name = "Grupo")]
        public string Grupo { get; set; }



        #endregion


        #region Implement Base Class
        public override string DisplayName => RSeccionesAnaliticas.TituloEntidad;


        public override object generateId(string id)
        {
           return int.Parse(id);
        }
        

        public override void createNewPrimaryKey()
        {
            primaryKey = new[] { GetType().GetProperty("Id") };
        }

        #endregion




    }


}
