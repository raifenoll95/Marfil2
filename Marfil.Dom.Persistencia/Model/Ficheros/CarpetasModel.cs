using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RFicheros = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Ficheros;
namespace Marfil.Dom.Persistencia.Model.Ficheros
{
    public class CarpetasModel : BaseModel<CarpetasModel, Carpetas>
    {
        #region Properties

        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RFicheros), Name = "Id")]
        public Guid Id { get; set; }

        [Display(ResourceType = typeof(RFicheros), Name = "Nombre")]
        public string Nombre { get; set; }
        
        public Guid Fkcarpetas { get; set; }

        [Display(ResourceType = typeof(RFicheros), Name = "Ruta")]
        public string Ruta { get; set; }

        #endregion

        #region CTR

        public CarpetasModel()
        {

        }

        public CarpetasModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return new Guid(id);
        }

        public override string DisplayName => RFicheros.Carpetas;

    }
}
