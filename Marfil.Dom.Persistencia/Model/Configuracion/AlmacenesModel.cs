using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Inf.Genericos;
using Marfil.Inf.ResourcesGlobalization.Textos.GeneralUI;
using Resources;
using RAlmacenes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Almacenes;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.Model.Configuracion
{
    public class AlmacenesModel : BaseModel<AlmacenesModel, Almacenes>
    {
        private List<AlmacenesZonasModel> _lineas = new List<AlmacenesZonasModel>();
        private List<ArticulosStockSeguridadModel> _articulosStockSeguridad = new List<ArticulosStockSeguridadModel>();

        #region Properties

        [Required]
        public string Empresa { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlmacenes), Name = "Id")]
        public string Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlmacenes), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RAlmacenes), Name = "Coordenadas")]
        [XmlIgnore]
        [MaxLength(40, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Coordenadas { get; set; }

        [XmlIgnore]
        [Display(ResourceType = typeof(RAlmacenes), Name = "Privado")]
        public bool Privado { get; set; }

        [XmlIgnore]
        public int numFamilias { get; set; }

        [XmlIgnore]
        private List<FicheroGaleria> _ficheros = new List<FicheroGaleria>();

        [XmlIgnore]
        public List<FicheroGaleria> Ficheros
        {
            get { return _ficheros; }
            set { _ficheros = value; }
        }

        [XmlIgnore]
        public Guid? Fkcarpetas { get; set; }

        [XmlIgnore]
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

        [XmlIgnore]
        private GaleriaModel _galeria;

        [XmlIgnore]
        public List<AlmacenesZonasModel> Lineas
        {
            get { return _lineas; }
            set { _lineas = value; }
        }
        public List<ArticulosStockSeguridadModel> ArticulosStockSeguridad
        {
            get { return _articulosStockSeguridad; }
            set { _articulosStockSeguridad = value; }
        }

        [XmlIgnore]
        public DireccionesModel Direcciones { get; set; }

        #endregion

        #region CTR

        public AlmacenesModel()
        {

        }

        public AlmacenesModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override object generateId(string id)
        {
            return id;
        }

        public override string DisplayName => RAlmacenes.TituloEntidad;

        public override string GetPrimaryKey()
        {
            return Id;
        }
    }

    public class AlmacenesZonasModel
    {
        [Required]
        [Display(ResourceType = typeof(RAlmacenes), Name = "Id")]
        public int Id { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlmacenes), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required]
        [Display(ResourceType = typeof(RAlmacenes), Name = "Fktipoubicacion")]
        public string Fktipoubicacion { get; set; }

        [Display(ResourceType = typeof(RAlmacenes), Name = "Coordenadas")]
        public string Coordenadas { get; set; }
    }
}
