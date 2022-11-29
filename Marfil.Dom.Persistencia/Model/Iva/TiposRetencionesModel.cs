using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos;
using Resources;
using RTiposRetenciones= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Tiposretenciones;
namespace Marfil.Dom.Persistencia.Model.Iva
{
    public enum TipoRendimiento
    {
        [StringValue(typeof(RTiposRetenciones),"TipoRendimientoDinerario")]
        Dinerario,
        [StringValue(typeof(RTiposRetenciones), "TipoRendimientoEspecie")]
        Especie
    }

    public enum TipoRetencionSobre
    {
        [StringValue(typeof(RTiposRetenciones), "BaseImponible")]
        BaseImponible,
        [StringValue(typeof(RTiposRetenciones), "TotalFactura")]
        TotalFactura
    }

    public class TiposRetencionesModel : BaseModel<TiposRetencionesModel, Tiposretenciones>
    {
        #region Propierties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposRetenciones), Name = "Id")]
        [MaxLength(4,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposRetenciones), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Range(0,100,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "Range")]
        [Display(ResourceType = typeof(RTiposRetenciones), Name = "Porcentajeretencion")]
        public double Porcentajeretencion { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposRetenciones), Name = "Fkcuentarecargo")]
        public string Fkcuentarecargo { get; set; }

        [Required]
        [Display(ResourceType = typeof(RTiposRetenciones), Name = "Fkcuentaabono")]
        public string Fkcuentaabono { get; set; }

        [Display(ResourceType = typeof(RTiposRetenciones), Name = "Tiporendimiento")]
        public TipoRendimiento? Tiporendimiento { get; set; }

        [Display(ResourceType = typeof(RTiposRetenciones), Name = "Inmueble")]
        public bool Inmueble { get; set; }

        [Display(ResourceType = typeof(RTiposRetenciones), Name = "RetencionSobre")]
        public TipoRetencionSobre RetencionSobre { get; set; }


        public string CuentaRecargoDescripcion { get; set; }

        public string CuentaAbonoDescripcion { get; set; }

        #endregion

        #region CTR

        public TiposRetencionesModel()
        {

        }

        public TiposRetencionesModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RTiposRetenciones.TituloEntidad;
    }
}
