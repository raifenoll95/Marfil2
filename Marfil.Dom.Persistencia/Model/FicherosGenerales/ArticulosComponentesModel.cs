using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using RArticulosComponentes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ArticulosComponentes;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class ArticulosComponentesModel : BaseModel<ArticulosComponentesModel, ArticulosComponentes>
    {

        #region CTR

        public ArticulosComponentesModel()
        {

        }

        public ArticulosComponentesModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override string DisplayName => RArticulosComponentes.TituloEntidad;

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

        [Display(ResourceType = typeof(RArticulosComponentes), Name = "FkArticulo")]
        public string FkArticulo { get; set; }

        public int Id { get; set; }

        [Display(ResourceType = typeof(RArticulosComponentes), Name = "ArticuloComponente")]
        public string IdComponente { get; set; }

        [Display(ResourceType = typeof(RArticulosComponentes), Name = "DescripcionComponente")]
        public string DescripcionComponente { get; set; }

        [Display(ResourceType = typeof(RArticulosComponentes), Name = "Piezas")]
        public int Piezas { get; set; }

        [Display(ResourceType = typeof(RArticulosComponentes), Name = "Largo")]
        public float Largo { get; set; }

        [Display(ResourceType = typeof(RArticulosComponentes), Name = "Ancho")]
        public float Ancho { get; set; }

        [Display(ResourceType = typeof(RArticulosComponentes), Name = "Grueso")]
        public float Grueso { get; set; }

        public float Metros { get; set; }

        [Display(ResourceType = typeof(RArticulosComponentes), Name = "Merma")]
        public int Merma { get; set; }

        public string UnidadMedida { get; set; }

        public double Precio { get; set; }

        public double PrecioInicial { get; set; }

        #endregion
    }
}
