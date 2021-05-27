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
using RFamiliasproductos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Familiasproductos;
using RArticulos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Articulos;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;

namespace Marfil.Dom.Persistencia.Model.FicherosGenerales
{
    public enum PartesDeLaDescripcion
    {
        [StringValue(typeof(RFamiliasproductos), "PartesDescripcionFamilia")]
        Familia,
        [StringValue(typeof(RFamiliasproductos), "PartesDescripcionMaterial")]
        Material,
        [StringValue(typeof(RFamiliasproductos), "PartesDescripcionCaracteristica")]
        Caracteristica,
        [StringValue(typeof(RFamiliasproductos), "PartesDescripcionGrosor")]
        Grosor,
        [StringValue(typeof(RFamiliasproductos), "PartesDescripcionAcabado")]
        Acabado,
        [StringValue(typeof(RFamiliasproductos), "PartesDescripcionFamiliamaterial")]
        Familiamaterial
    }

    public enum TipoFamilia
    {
        [StringValue(typeof(RFamiliasproductos), "TipoFamiliaGeneral")]
        General,
        [StringValue(typeof(RFamiliasproductos), "TipoFamiliaBloque")]
        Bloque,
        [StringValue(typeof(RFamiliasproductos), "TipoFamiliaTabla")]
        Tabla,
        [StringValue(typeof(RFamiliasproductos), "TipoLibreTabla")]
        Libre
    }

    public class FamiliasproductosModel : BaseModel<FamiliasproductosModel, Familiasproductos>
    {
        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [MaxLength(2, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [CustomDisplayDescription(typeof(RFamiliasproductos), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]  
        public string Descripcion { get; set; }

        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [CustomDisplayDescription(typeof(RFamiliasproductos), "Descripcion", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        public string Descripcion2 { get; set; }

        [MaxLength(20, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Descripcionabreviada")]
        public string Descripcionabreviada { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Fkcontador")]
        public string Fkcontador { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Fkguiascontables")]
        public string Fkguiascontables { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Tipofamilia")]
        public TipoFamilia Tipofamilia { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Gestionstock")]
        public bool Gestionstock { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Articulonegocio")]
        public bool Articulonegocio { get; set; }

        [Required]
        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Fkunidadesmedida")]
        public string Fkunidadesmedida { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Editarlargo")]
        public bool Editarlargo { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Editarancho")]
        public bool Editarancho { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Editargrueso")]
        public bool Editargrueso { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Validarmaterial")]
        public bool Validarmaterial { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Validarcaracteristica")]
        public bool Validarcaracteristica { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Validargrosor")]
        public bool Validargrosor { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Validaracabado")]
        public bool Validaracabado { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Fkgruposiva")]
        public string Fkgruposiva { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "ValidarDimensiones")]
        public bool Validardimensiones { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Consumibles")]
        public bool Consumibles { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "MinLargo")]
        public double Minlargo { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "MaxLargo")]
        public double Maxlargo { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "MinAncho")]
        public double Minancho { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "MaxAncho")]
        public double Maxancho { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "MinGrueso")]
        public double Mingrueso { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "MaxGrueso")]
        public double Maxgrueso { get; set; }

        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Clasificacion")]
        public string Clasificacion { get; set; }

        public int numMateriales { get; set; }

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

        private GaleriaModel _galeria;

        //Descripcion 1 generada
        [CustomDisplayDescription(typeof(RFamiliasproductos), "Descripcion1generada", CustomDisplayDescriptionAttribute.TipoDescripcion.Principal)]
        public string Descripcion1generada { get; set; }

        public PartesDeLaDescripcion? Descripcion1Generada_1 { get; set; }
        public PartesDeLaDescripcion? Descripcion1Generada_2 { get; set; }
        public PartesDeLaDescripcion? Descripcion1Generada_3 { get; set; }
        public PartesDeLaDescripcion? Descripcion1Generada_4 { get; set; }
        public PartesDeLaDescripcion? Descripcion1Generada_5 { get; set; }
        public PartesDeLaDescripcion? Descripcion1Generada_6 { get; set; }
        //end Descripcion 1 generada

        //Descripcion 2 generada
        
        [CustomDisplayDescription(typeof(RFamiliasproductos), "Descripcion1generada", CustomDisplayDescriptionAttribute.TipoDescripcion.Secundaria)]
        public string Descripcion2generada { get; set; }

        public PartesDeLaDescripcion? Descripcion2Generada_1 { get; set; }
        public PartesDeLaDescripcion? Descripcion2Generada_2 { get; set; }
        public PartesDeLaDescripcion? Descripcion2Generada_3 { get; set; }
        public PartesDeLaDescripcion? Descripcion2Generada_4 { get; set; }
        public PartesDeLaDescripcion? Descripcion2Generada_5 { get; set; }
        public PartesDeLaDescripcion? Descripcion2Generada_6 { get; set; }
        //end Descripcion 2 generada

            //Descripcion abreviada generada
        [Display(ResourceType = typeof(RFamiliasproductos), Name = "Descripcionabreviadagenerada")]
        public string Descripcionabreviadagenerada { get; set; }

        public PartesDeLaDescripcion? Descripcionabreviadagenerada_1 { get; set; }
        public PartesDeLaDescripcion? Descripcionabreviadagenerada_2 { get; set; }
        public PartesDeLaDescripcion? Descripcionabreviadagenerada_3 { get; set; }
        public PartesDeLaDescripcion? Descripcionabreviadagenerada_4 { get; set; }
        public PartesDeLaDescripcion? Descripcionabreviadagenerada_5 { get; set; }
        public PartesDeLaDescripcion? Descripcionabreviadagenerada_6 { get; set; }
        //end Descripcion abreviada generada
        
        
        [Display(ResourceType = typeof(RArticulos), Name = "Tipogestionlotes")]
        public Tipogestionlotes Tipogestionlotes { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Stocknegativoautorizado")]
        public bool Stocknegativoautorizado { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Lotefraccionable")]
        public bool Lotefraccionable { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Existenciasminimasmetros")]
        public double? Existenciasminimasmetros { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Existenciasmaximasmetros")]
        public double? Existenciasmaximasmetros { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Existenciasminimasunidades")]
        public double? Existenciasminimasunidades { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Existenciasmaximasunidades")]
        public double? Existenciasmaximasunidades { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Web")]
        public bool Web { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Gestionarcaducidad")]
        public bool Gestionarcaducidad { get; set; }

        [Display(ResourceType = typeof(RArticulos), Name = "Categoria")]
        public TipoCategoria Categoria { get; set; }
        
        #endregion

        #region CTR

        public FamiliasproductosModel()
        {

        }

        public FamiliasproductosModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RFamiliasproductos.TituloEntidad;
    }
}
