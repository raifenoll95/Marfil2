using Marfil.Dom.Persistencia.Model.Ficheros;
using Marfil.Dom.Persistencia.Model.GaleriaImagenes;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using RGrupoMateriales = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.GrupoMateriales;

namespace Marfil.Dom.Persistencia.Model.Documentos.GrupoMateriales
{
    public class GrupoMaterialesModel : BaseModel<GrupoMaterialesModel, Persistencia.GrupoMateriales>
    {

        #region CTR

        public GrupoMaterialesModel()
        {

        }

        public GrupoMaterialesModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override string DisplayName => RGrupoMateriales.TituloEntidad;

        
        public override object generateId(string id)
        {

            return Cod;
        }

        //Clave primaria
        public override string GetPrimaryKey()
        {
            return this.Cod;
        }

        #region properties

        private List<FicheroGaleria> _ficheros = new List<FicheroGaleria>();
        public List<FicheroGaleria> Ficheros
        {
            get { return _ficheros; }
            set { _ficheros = value; }
        }

        [Display(ResourceType = typeof(RGrupoMateriales), Name = "Fkejercicio")]
        public int Fkejercicio { get; set; }


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
        public GaleriaModel galeriaconsultavisual;

        [Required]
        public string Empresa { get; set; }

        [Key]
        [Required]
        [MaxLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        [MinLength(3, ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "MinLength")]
        [Display(ResourceType = typeof(RGrupoMateriales), Name = "Codigo")]
        public String Cod { get; set; }

        [Display(ResourceType = typeof(RGrupoMateriales), Name = "Descripcion")]
        public String Descripcion { get; set; }

        public int NumMateriales { get; set; }

        [Display(ResourceType = typeof(RGrupoMateriales), Name = "Descripcion2")]
        public String InglesDescripcion { get; set; }

        #endregion
    }

    public class GrupoMaterialesComparerModel : IEqualityComparer<GrupoMaterialesModel>
    {
        public bool Equals(GrupoMaterialesModel x, GrupoMaterialesModel y)
        {
            return x.Cod == y.Cod;
        }

        public int GetHashCode(GrupoMaterialesModel obj)
        {
            return obj.Cod.GetHashCode();
        }
    }
}
