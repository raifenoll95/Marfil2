using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RArticulosTercero = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ArticulosTercero;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class ArticulosTerceroModel : BaseModel<ArticulosTerceroModel, ArticulosTercero>
    {

        #region CTR

        public ArticulosTerceroModel()
        {

        }

        public ArticulosTerceroModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override string DisplayName => RArticulosTercero.TituloEntidad;

        public override object generateId(string id)
        {
            return id; 
        }

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name.ToLower() == "id").Select(f => f.property);
        }

        public override string GetPrimaryKey()
        {
            return this.Id.ToString();
        }


        #region propiedades

        public string Empresa { get; set; }

        [Display(ResourceType = typeof(RArticulosTercero), Name = "CodArticulo")]
        public string CodArticulo { get; set; }

        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RArticulosTercero), Name = "CodTercero")]
        public string CodTercero { get; set; }

        [Display(ResourceType = typeof(RArticulosTercero), Name = "DescripcionTercero")]
        public string DescripcionTercero { get; set; }

        [Display(ResourceType = typeof(RArticulosTercero), Name = "CodArticuloTercero")]
        public string CodArticuloTercero { get; set; }

        [Display(ResourceType = typeof(RArticulosTercero), Name = "DescripcionArticuloTercero")]
        public string Descripcion { get; set; }

        #endregion
    }
}
