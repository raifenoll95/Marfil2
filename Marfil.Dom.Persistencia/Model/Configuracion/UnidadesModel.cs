using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Resources;
using RUnidades = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Unidades;
namespace Marfil.Dom.Persistencia.Model.Configuracion
{
    public enum TiposStockMovimientos
    {
        [StringValue(typeof(RUnidades), "TiposStockMovimientosCantidad")]
        Cantidad,
        [StringValue(typeof(RUnidades), "TiposStockMovimientosCantidadYTotal")]
        CantidadYTotal,
        [StringValue(typeof(RUnidades), "TiposStockMovimientosTotal")]
        Total
    }

    public enum TiposStock
    {
        [StringValue(typeof(RUnidades), "TiposStockUnitario")]
        Unitario,
        [StringValue(typeof(RUnidades), "TiposStockLote")]
        Lote
    }

    public enum TipoStockTotal
    {
        [StringValue(typeof(RUnidades), "TipoStockTotalCalculado")]
        Calculado,
        [StringValue(typeof(RUnidades), "TipoStockTotalEditado")]
        Editado
    }

    public enum TipoStockFormulas
    {
        [StringValue(typeof(RUnidades), "TipoStockFormulasSuperficie")]
        Superficie,
        [StringValue(typeof(RUnidades), "TipoStockFormulasVolumen")]
        Volumen,
        [StringValue(typeof(RUnidades), "TipoStockFormulasLinear")]
        Linear,
        [StringValue(typeof(RUnidades), "TipoStockFormulasCantidad")]
        Cantidad,
        [StringValue(typeof(RUnidades), "TipoStockFormulasTotal")]
        Total
    }


    public class UnidadesModel : BaseModel<UnidadesModel, Tiposcuentas>
    {

        #region Properties

        [Display(ResourceType = typeof(RUnidades), Name = "Id")]
        [Required]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [Display(ResourceType = typeof(RUnidades), Name = "Codigounidad")]
        [Required]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Codigounidad { get; set; }

        [CustomDisplayDescriptionAttribute(typeof(RUnidades), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        public string Descripcion { get; set; }

        [CustomDisplayDescriptionAttribute(typeof(RUnidades), "Descripcion2",CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        public string Descripcion2 { get; set; }

        [CustomDisplayDescriptionAttribute(typeof(RUnidades), "Textocorto", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        [Required]
        public string Textocorto { get; set; }

        [CustomDisplayDescriptionAttribute(typeof(RUnidades), "Textocorto2", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        [MaxLength(10, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Textocorto2 { get; set; }

        [Display(ResourceType = typeof(RUnidades), Name = "Decimalestotales")]
        [Range(0,6,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "Range")]
        public int Decimalestotales { get; set; }

        [Display(ResourceType = typeof(RUnidades), Name = "Formula")]
        [Required]
        public TipoStockFormulas Formula { get; set; }

        [Display(ResourceType = typeof(RUnidades), Name = "Tiposmovimientostock")]
        [Required]
        public TiposStockMovimientos Tiposmovimientostock { get; set; }

        [Display(ResourceType = typeof(RUnidades), Name = "Tipostock")]
        [Required]
        public TiposStock Tipostock { get; set; }

        [Display(ResourceType = typeof(RUnidades), Name = "Tipototal")]
        [Required]
        public TipoStockTotal Tipototal { get; set; }

        #endregion

        #region CTR

        public UnidadesModel()
        {

        }

        public UnidadesModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RUnidades.TituloEntidad;
    }
}
