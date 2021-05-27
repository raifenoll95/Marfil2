using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Resources;
using RTrabajos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Trabajos;
using System.Linq;

namespace Marfil.Dom.Persistencia.Model.Stock
{

    // Histórico precios
    [Serializable]
    public class TrabajosLinModel
    {

        #region Properties
        [Required]
        public string Empresa { get; set; }

        [Required]
        public string Fktrabajos { get; set; }

        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(2000, 2100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RTrabajos), Name = "Año")]
        public short Año { get; set; }

        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RTrabajos), Name = "Precio")]
        public double Precio { get; set; }

        #endregion

        public TrabajosLinModel()
        {
            Empresa = "0000";
            Fktrabajos = "0000";
            Id = 0;
            Año = 0;
            Precio = 0;
        }
    }
    // Fin histórico precios

    public enum TipoTrabajo
    {
        [StringValue(typeof(RTrabajos),"Aserrado")]
        Aserrado,
        [StringValue(typeof(RTrabajos), "Elaborado")]
        Elaborado
    }

    public enum TipoImputacion
    {
        [StringValue(typeof(RTrabajos), "M3")]
        M3,
        [StringValue(typeof(RTrabajos), "M2Real")]
        M2Real,
        [StringValue(typeof(RTrabajos), "M2Peso")]
        M2Peso
    }

    public class TrabajosModel : BaseModel<TrabajosModel, Trabajos>
    {
        #region Members

        private List<KitLinModel> _lineas = new List<KitLinModel>();

        #endregion

        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTrabajos), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTrabajos), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RTrabajos), Name = "Tipotrabajo")]
        public TipoTrabajo Tipotrabajo { get; set; }

        [Display(ResourceType = typeof(RTrabajos), Name = "Tipoimputacion")]
        public TipoImputacion Tipoimputacion { get; set; }

        [Display(ResourceType = typeof(RTrabajos), Name = "Fkacabadoinicial")]
        public string Fkacabadoinicial { get; set; }

        [Display(ResourceType = typeof(RTrabajos), Name = "Fkacabadofinal")]
        public string Fkacabadofinal { get; set; }

        [Display(ResourceType = typeof(RTrabajos), Name = "Fkarticulofacturable")]
        public string Fkarticulofacturable { get; set; }

        [Display(ResourceType = typeof(RTrabajos), Name = "Fkacabadoinicial")]
        public string Acabadoinicialdescripcion { get; set; }

        [Display(ResourceType = typeof(RTrabajos), Name = "Fkacabadofinal")]
        public string Acabadofinaldescripcion { get; set; }

        [Display(ResourceType = typeof(RTrabajos), Name = "PrecioReferencia")]
        public double Precio { get; set; }

        #endregion

        #region LINEAS PRECIOS

        private List<TrabajosLinModel> _lineasprecios = new List<TrabajosLinModel>();

        public IEnumerable<TrabajosLinModel> Lineas
        {
            get { return _lineasprecios; }
            set { _lineasprecios = value.ToList(); }
        }

        public int NumeroLineasHistorico => Lineas.Count();

        #endregion

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name == "Id").Select(f => f.property);
        }

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RTrabajos.TituloEntidad;

        #region CTR

        public TrabajosModel()
        {
           
        }

        public TrabajosModel(IContextService context) : base(context)
        {

        }

        #endregion
    }
}
