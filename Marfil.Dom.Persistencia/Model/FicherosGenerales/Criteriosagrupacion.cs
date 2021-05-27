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
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Resources;
using RCriteriosagrupacion= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Criteriosagrupacion;
using RAlbaranes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Albaranes;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public enum CamposAgrupacionAlbaran
    {
        [StringValue(typeof(RAlbaranes),"Fkarticulos")]
        Fkarticulos,
        [StringValue(typeof(RAlbaranes), "Descripcion")]
        Descripcion,
        [StringValue(typeof(RAlbaranes), "Lote")]
        Lote,
        [StringValue(typeof(RAlbaranes), "Bundle")]
        Bundle,
        [StringValue(typeof(RAlbaranes), "Canal")]
        Canal,
        [StringValue(typeof(RAlbaranes), "Contenedor")]
        Contenedor,
        [StringValue(typeof(RAlbaranes), "Caja")]
        Caja,
        [StringValue(typeof(RAlbaranes), "Largo")]
        Largo,
        [StringValue(typeof(RAlbaranes), "Ancho")]
        Ancho,
        [StringValue(typeof(RAlbaranes), "Grueso")]
        Grueso,
        [StringValue(typeof(RAlbaranes), "Precio")]
        Precio,
        [StringValue(typeof(RAlbaranes), "Porcentajedescuento")]
        Porcentajedescuento,
        [StringValue(typeof(RAlbaranes), "Sello")]
        Sello,
        [StringValue(typeof(RAlbaranes), "Pesoneto")]
        Pesoneto
    }

    public class CriteriosagrupacionLinModel 
    {
        [Display(ResourceType = typeof(RCriteriosagrupacion), Name = "Id")]
        public int Id { get; set; }

        public CamposAgrupacionAlbaran Campoenum
        {
            get { return (CamposAgrupacionAlbaran) Campo; }
            set { Campo = (int)value; }
        }

        [Required]
        [Display(ResourceType = typeof(RCriteriosagrupacion), Name = "Campo")]
        public int Campo { get; set; }

        [Display(ResourceType = typeof(RCriteriosagrupacion), Name = "Orden")]
        public int? Orden { get; set; }
    }

    public class CriteriosagrupacionModel : BaseModel<CriteriosagrupacionModel, Criteriosagrupacion>
    {
        private List<CriteriosagrupacionLinModel> _lineas=new List<CriteriosagrupacionLinModel>();

        #region Properties
        [Key]
        [Required]
        [MaxLength(4, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType=typeof(RCriteriosagrupacion),Name = "Id")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType =typeof(RCriteriosagrupacion), Name = "Nombre")]
        public string Nombre { get; set; }
        
        [Display(ResourceType = typeof(RCriteriosagrupacion), Name = "Ordenaralbaranes")]
        public bool? Ordenaralbaranes { get; set; }

        [Display(ResourceType = typeof(RCriteriosagrupacion), Name = "Ordenaralbaranes")]
        public bool Ordenaralbaranesvista
        {
            get { return Ordenaralbaranes ?? false; }
            set { Ordenaralbaranes = value; }
        }
        public List<CriteriosagrupacionLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        #endregion

        #region CTR

        public CriteriosagrupacionModel()
        {

        }

        public CriteriosagrupacionModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RCriteriosagrupacion.TituloEntidad;
    }
}
