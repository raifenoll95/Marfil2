using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RFormasPago = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Formaspago;
using RSituacionesTesoreria = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.SituacionesTesoreria;
using Marfil.Inf.Genericos;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{

    public enum TipoPrevision
    {
        [StringValue(typeof(RSituacionesTesoreria), "Cobros")]
        Cobros,
        [StringValue(typeof(RSituacionesTesoreria), "Pagos")]
        Pagos,
        [StringValue(typeof(RSituacionesTesoreria), "Ambos")]
        Ambos
    }

    public enum TipoRiesgo
    {
        [StringValue(typeof(RSituacionesTesoreria), "Blanco")]
        Empty,
        [StringValue(typeof(RSituacionesTesoreria), "Regularizado")]
        Regularizado,
        [StringValue(typeof(RSituacionesTesoreria), "Noregularizado")]
        Noregularizado,
        [StringValue(typeof(RSituacionesTesoreria), "Impagado")]
        Impagado
    }

    public class SituacionesTesoreriaModel : BaseModel<SituacionesTesoreriaModel, SituacionesTesoreria>
    {

        #region Properties

        [Display(ResourceType = typeof(RFormasPago),Name = "Id")]
        [Required]
        public string Cod { get; set; }

        [CustomDisplayDescription(typeof(RSituacionesTesoreria), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        public string Descripcion { get; set; }
        
        [CustomDisplayDescription(typeof(RSituacionesTesoreria), "Descripcion2", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        public string Descripcion2 { get; set; }

        [Display(ResourceType = typeof(RSituacionesTesoreria), Name = "InicialCobro")]
        public bool Valorinicialcobros { get; set; }

        [Display(ResourceType = typeof(RSituacionesTesoreria), Name = "InicialPago")]
        public bool Valorinicialpagos { get; set; }

        [Display(ResourceType = typeof(RSituacionesTesoreria), Name = "Prevision")]
        public TipoPrevision Prevision { get; set; }

        [Display(ResourceType = typeof(RSituacionesTesoreria), Name = "Editable")]
        public bool Editable { get; set; }

        [Display(ResourceType = typeof(RSituacionesTesoreria), Name = "Remesable")]
        public bool Remesable { get; set; }

        [Display(ResourceType = typeof(RSituacionesTesoreria), Name = "Riesgo")]
        public TipoRiesgo Riesgo { get; set; }

        #endregion

        #region CTR

        public SituacionesTesoreriaModel()
        {

        }

        public SituacionesTesoreriaModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override string DisplayName => RSituacionesTesoreria.TituloEntidad;

        public override object generateId(string cod)
        {
            return cod;
        }

        public override string GetPrimaryKey()
        {
            return this.Cod;
        }
    }
}
