using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RArticulosStockSeguridad = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ArticulosStockSeguiridad;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using RAlmacenes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Almacenes;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class ArticulosStockSeguridadModel : BaseModel<ArticulosStockSeguridadModel, ArticulosStockSeguridad>
    {
        #region CTR

        public ArticulosStockSeguridadModel()
        {

        }

        public ArticulosStockSeguridadModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override string DisplayName => RArticulosStockSeguridad.TituloEntidad;

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

        [Display(ResourceType = typeof(RArticulos), Name = "Id")]
        public string Codarticulo { get; set; }

        [Required]
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlmacenes), Name = "Id")]
        public string Codalmacen { get; set; }

        [Display(ResourceType = typeof(RAlmacenes), Name = "Descripcion")]
        public string Descripcionalmacen { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Stockminimo")]
        public double Stockminimo { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Stockmaximo")]
        public double Stockmaximo { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Stockseguridad")]
        public TipoStockSeguridad Stockseguridad { get; set; }

        #endregion
    }
}
