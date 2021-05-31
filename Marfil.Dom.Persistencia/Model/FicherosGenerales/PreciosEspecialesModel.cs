using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RClientes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Clientes;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using RPreciosEspeciales = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.PreciosEspeciales;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class PreciosEspecialesModel : BaseModel<PreciosEspecialesModel, PreciosEspeciales>
    {
        #region CTR

        public PreciosEspecialesModel()
        {

        }

        public PreciosEspecialesModel(IContextService context) : base(context)
        {

        }

        #endregion

        #region Properties
        [Required]
        public string Empresa { get; set; }
        [Required]
        [Display(ResourceType = typeof(RPreciosEspeciales), Name = "Id")]
        public int Id { get; set; }
        [Required]
        [Display(ResourceType = typeof(RPreciosEspeciales), Name = "Fkclientes")]
        public string Fkclientes { get; set; }
        [Display(ResourceType = typeof(RPreciosEspeciales), Name = "NombreCliente")]
        public string NombreCliente { get; set; }
        [Required]
        [Display(ResourceType = typeof(RPreciosEspeciales), Name = "Fkarticulo")]
        public string Fkarticulo { get; set; }
        [Display(ResourceType = typeof(RPreciosEspeciales), Name = "DescripcionArticulo")]
        public string DescripcionArticulo { get; set; }
        [Required]
        [Display(ResourceType = typeof(RPreciosEspeciales), Name = "Fechavalidez")]
        public DateTime? Fechavalidez { get; set; }
        [Required]
        [Display(ResourceType = typeof(RPreciosEspeciales), Name = "Precio")]
        [Range(0, 999999999.999, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double Precio { get; set; }
        [Display(ResourceType = typeof(RPreciosEspeciales), Name = "Descuento")]
        [Range(0, 99.99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        public double Descuento { get; set; }

        public virtual Articulos Articulos { get; set; }
        public virtual Clientes Clientes { get; set; }

        #endregion

        public override string DisplayName => RPreciosEspeciales.TituloEntidad;

        public override object generateId(string id)
        {
            return id;
        }
    }
}
