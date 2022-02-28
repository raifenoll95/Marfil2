using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using RAlmacenes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Almacenes;
using RLogStockSeguridad = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.LogStockSeguridad;
using System.ComponentModel.DataAnnotations;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class LogStockSeguridadModel : BaseModel<LogStockSeguridadModel, LogStockSeguridad>
    {
        public override string DisplayName => RLogStockSeguridad.TituloEntidad;

        public override object generateId(string id)
        {
            return id;
        }


        #region CTR

        public LogStockSeguridadModel()
        {

        }

        public LogStockSeguridadModel(IContextService context) : base(context)
        {

        }

        #endregion

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

        [Display(ResourceType = typeof(RArticulos), Name = "Descripcion")]
        public string Descripcionarticulo { get; set; }

        [Required]
        public int Id { get; set; }

        [Display(ResourceType = typeof(RLogStockSeguridad), Name = "Documento")]
        public string Documento { get; set; }

        [Display(ResourceType = typeof(RLogStockSeguridad), Name = "Codigounidad")]
        public string Codigounidad { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Stockminimo")]
        public double Stockminimo { get; set; }

        [Display(ResourceType = typeof(RLogStockSeguridad), Name = "Stockactual")]
        public double Stockactual { get; set; }

        [Display(ResourceType = typeof(RLogStockSeguridad), Name = "Pedidooptimo")]
        public double Pedidooptimo { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Stockseguridad")]
        public TipoStockSeguridad Stockseguridad { get; set; }

        [Display(ResourceType = typeof(RLogStockSeguridad), Name = "Fecha")]
        public DateTime Fecha { get; set; }

        [Display(ResourceType = typeof(RLogStockSeguridad), Name = "Almacen")]
        public string Almacen { get; set; }

        #endregion
    }
}
