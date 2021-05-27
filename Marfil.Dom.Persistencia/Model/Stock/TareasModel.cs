using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Iva;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Marfil.Inf.Genericos.Helper;
using Resources;
using RTareas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Tareas;
using System.Linq;

namespace Marfil.Dom.Persistencia.Model.Stock
{

    // Histórico precios
    [Serializable]
    public class TareasLinModel
    {

        #region Properties
        [Required]
        public string Empresa { get; set; }

        [Required]
        public string Fktareas { get; set; }

        [Required]
        [Key]
        public int Id { get; set; }

        [Required]
        [Range(2000, 2100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RTareas), Name = "Año")]
        public short Año { get; set; }
        
        [Range(0, 9999999.99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RTareas), Name = "Precio")]
        public double Precio { get; set; }

        #endregion
        
        public TareasLinModel ()
        {
            Empresa = "0000";
            Fktareas = "0000";
            Id = 0;
            Año = 0;
            Precio = 0;        
        }
    }
    // Fin histórico precios

        public enum Imputacion
    {        
        [Display(ResourceType = typeof(RTareas), Name = "Metros")]
        Metros,        
        [Display(ResourceType = typeof(RTareas), Name = "CantidadPiezas")]
        CantidadPiezas,        
        [Display(ResourceType = typeof(RTareas), Name = "Pedido")]
        Pedido
    }

    public enum Unidad
    {
        [Display(ResourceType = typeof(RTareas), Name = "Importe")]
        Importe,
        [Display(ResourceType = typeof(RTareas), Name = "Horas")]
        Horas,
        [Display(ResourceType = typeof(RTareas), Name = "Minutos")]
        Minutos
    }


    public class TareasModel : BaseModel<TareasModel, Tareas>
    {
        #region Members


        #endregion

        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [MaxLength(5, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RTareas), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTareas), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RTareas), Name = "SeccionProduccion")]
        public string SeccionProduccion { get; set; }

        [Display(ResourceType = typeof(RTareas), Name = "Imputacion")]
        public Imputacion Imputacion { get; set; }

        //[Range(0, 100, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RTareas), Name = "Capacidad")]        
        public double Capacidad { get; set; }

        //[Range(0, 9999999.99, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "RangeClient")]
        [Display(ResourceType = typeof(RTareas), Name = "PrecioReferencia")]
        public double Precio { get; set; }

        [Display(ResourceType = typeof(RTareas), Name = "Unidad")]
        public Unidad Unidad { get; set; }

        private List<TareasLinModel> _lineas = new List<TareasLinModel>();

        public IEnumerable<TareasLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value.ToList(); }
        }

        public int NumeroLineasHistorico => Lineas.Count();

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name == "Id").Select(f => f.property);
        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RTareas.TituloEntidad;

        #region CTR

        public TareasModel()
        {

        }

        public TareasModel(IContextService context) : base(context)
        {

        }

        #endregion
    }
}
