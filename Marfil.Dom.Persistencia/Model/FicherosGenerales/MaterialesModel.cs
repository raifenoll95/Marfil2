using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Inf.ResourcesGlobalization.Textos.Entidades;
using Resources;
using RMateriales = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Materiales;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public class MaterialesLinModel
    {
        [Key]
        [Display(ResourceType = typeof(RMateriales), Name = "Codigovariedad")]
        public string Codigovariedad { get; set; }

        [CustomDisplayDescription(typeof(RMateriales), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        public string Descripcion { get; set; }

        [CustomDisplayDescription(typeof(RMateriales), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        public string Descripcion2 { get; set; }
    }

    public class MaterialesModel : BaseModel<MaterialesModel, Materiales>
    {
        private List<MaterialesLinModel> _lineas= new List<MaterialesLinModel>();
        private GaleriaModel _galeria;

        #region Propiedades

        [Required]
        public string Empresa { get; set; }
        
        [Required]
        [Display(ResourceType = typeof(RMateriales), Name = "Id")]
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Id { get; set; }

        [CustomDisplayDescription(typeof(RMateriales), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Descripcion { get; set; }

        [CustomDisplayDescription(typeof(RMateriales), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Descripcion2 { get; set; }

        [Display(ResourceType = typeof(RMateriales), Name = "Descripcionabreviada")]
        [MaxLength(20,ErrorMessageResourceType = typeof(Unobtrusive),ErrorMessageResourceName = "MaxLength")]
        public string Descripcionabreviada { get; set; }

        [Display(ResourceType = typeof(RMateriales), Name = "Fkfamiliasmateriales")]
        public string Fkfamiliamateriales { get; set; }


        [Display(ResourceType = typeof(RMateriales), Name = "Fkfamiliasmateriales")]
        public string Familiamateriales { get; set; }

        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [MinLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        [Display(ResourceType = typeof(RMateriales), Name = "Fkgruposmateriales")]
        public string Fkgruposmateriales { get; set; }

        [Display(ResourceType = typeof(RMateriales), Name = "Fkgruposmateriales")]
        public string Gruposmateriales { get; set; }

        [Display(ResourceType = typeof(RMateriales), Name = "Dureza")]
        public string Dureza { get; set; }


        public Double MetrosCV { get; set; }
        public Double LotesCV { get; set; }
        public Double PiezasCV { get; set; }

        public List<MaterialesLinModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }

        private List<FicheroGaleria> _ficheros = new List<FicheroGaleria>();
        public List<FicheroGaleria> Ficheros
        {
            get { return _ficheros; }
            set { _ficheros = value; }
        }

        public Guid? Fkcarpetas { get; set; }

        public GaleriaModel Galeria
        {
            get
            {
                _galeria = new GaleriaModel();
                if (Fkcarpetas.HasValue)
                {
                    _galeria.Empresa = Empresa;
                    _galeria.DirectorioId = Fkcarpetas.Value;
                }
                return _galeria;
            }
        }

        #endregion

        #region CTR

        public MaterialesModel()
        {

        }

        public MaterialesModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RMateriales.TituloEntidad;
    }
}
