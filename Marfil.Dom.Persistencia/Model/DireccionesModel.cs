using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Interfaces;
using System.ComponentModel.DataAnnotations;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RDirecciones= Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Direcciones;
namespace Marfil.Dom.Persistencia.Model
{
    public class DireccionesModel
    {
        public string Empresa { get; set; }
        public int Tipotercero { get; set; }
        public Guid Id { get; set; }
        public IEnumerable<DireccionesLinModel> Direcciones { get; set; }
        public bool ReadOnly { get; set; }
    }

    public class DireccionesLinModel : BaseModel<DireccionesLinModel, Direcciones>
    {
        public const char SeparatorPk = ';';

        #region Properties

        public string IdView
        {
            get
            {
                return CreateId((int)Tipotercero,Fkentidad,Id);
            }
        }
        
        public string Empresa { get; set; }
        
        public int Tipotercero { get; set; }

        public string Fkentidad { get; set; }

        [Required]
        public int Id { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Fktipodireccion")]
        public string Fktipodireccion { get; set; }

        public string Tipodireccion { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Defecto")]
        public bool Defecto { get; set; }

        [Required]
        [Display(ResourceType = typeof(RDirecciones), Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Fktipovia")]
        public string Fktipovia { get; set; }

        public string Tipovia { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Direccion")]
        public string Direccion { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Poblacion")]
        public string Poblacion { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Cp")]
        [MaxLength(10, ErrorMessageResourceType = typeof(Resources.Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Cp { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Fkpais")]
        public string Fkpais { get; set; }

        public string Pais { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Fkprovincia")]
        public string Fkprovincia { get; set; }

        public string Provincia { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Personacontacto")]
        public string Personacontacto { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Telefono")]
        [MaxLength(15, ErrorMessageResourceType = typeof(Resources.Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Telefono { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Telefonomovil")]
        [MaxLength(15, ErrorMessageResourceType = typeof(Resources.Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Telefonomovil { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Fax")]
        [MaxLength(15, ErrorMessageResourceType = typeof(Resources.Unobtrusive), ErrorMessageResourceName = "MaxLength")]
        public string Fax { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Web")]
        [Url]
        public string Web { get; set; }

        [Display(ResourceType = typeof(RDirecciones), Name = "Notas")]
        public string Notas { get; set; }

        #endregion

        #region CTR

        public DireccionesLinModel()
        {
            
        }

        public DireccionesLinModel(IContextService context) : base(context)
        {

        }

        #endregion

        public override string DisplayName => RDirecciones.TituloEntidad;

        public override object generateId(string id)
        {
            return id;
        }

        public static string CreateId(int tipotercero, string entidad, int id)
        {
            return string.Format("{0}{1}{2}{1}{3}", tipotercero, SeparatorPk, entidad, id);
        }

    }
}
