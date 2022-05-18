using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using RUsuarios = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Usuarios;
namespace Marfil.Dom.Persistencia.Model
{
    public interface IEmailconfiguracion
    {
        string Email { get; }
        string Usuariomail { get; }
        string Passwordmail { get; }
        string Smtp { get; }
        string Firma { get; }
        int? Puerto { get; }
        bool Ssl { get; }
        string Nombre { get; }
        bool IsValidEmailConfiguration { get; }
        TipoCopiaRemitente? Copiaremitente { get; }
    }

    public enum TipoCopiaRemitente
    {
        Cc,
        Bcc
    }

    public class UsuariosModel: BaseModel<UsuariosModel, Usuarios>, IEmailconfiguracion
    {
        #region Properties

        public override string DisplayName => RUsuarios.TituloEntidad;
        private Guid _id = Guid.NewGuid();
        [Required]
        public Guid Id { get { return _id; } set {  _id = value; } }

        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        [Display(ResourceType = typeof(RUsuarios), Name = "usuario")]
        public string Usuario {get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(5)]
        [MaxLength(12)]
        [Display(ResourceType = typeof(RUsuarios), Name = "password")]
        [IgnoreDataMember]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(5)]
        [MaxLength(12)]
        [Compare("Password", ErrorMessageResourceType = typeof(Unobtrusive), ErrorMessageResourceName = "EqualTo")]
        [Display(ResourceType = typeof(RUsuarios), Name = "confirmacionPassword")]
        [IgnoreDataMember]
        public string Confirmacionpassword { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Usuariomail")]
        public string Usuariomail { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Passwordmail")]
        public string Passwordmail { get; set; }

        [EmailAddress]
        [Display(ResourceType = typeof(RUsuarios), Name = "Email")]
        public string Email { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Smtp")]
        public string Smtp { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Puerto")]
        public int? Puerto { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Ssl")]
        public bool Ssl { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Firma")]
        public string Firma { get; set; }

        
        public string Contenidofirma { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Copiaremitente")]
        public TipoCopiaRemitente? Copiaremitente { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Nivel")]
        public int Nivel { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Cambiarempresa")]
        public bool Cambiarempresa { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Cambiaralmacen")]
        public bool Cambiaralmacen { get; set; }

        [Display(ResourceType = typeof(RUsuarios), Name = "Bloquearseries")]
        public bool Bloquearseries { get; set; }

        public bool Usuario_cliente { get; set; }

        public string Codigoclienteusuario { get; set; }
        [Display(ResourceType = typeof(RUsuarios), Name = "Operario")]
        public bool Operario { get; set; }
        [Display(ResourceType = typeof(RUsuarios), Name = "Codigooperariousuario")]
        public string Codigooperariousuario { get; set; }

        public bool IsValidEmailConfiguration
        {
            get
            {
                return !string.IsNullOrEmpty(Usuariomail) && !string.IsNullOrEmpty(Passwordmail) &&
                       !string.IsNullOrEmpty(Smtp) && Puerto.HasValue && !string.IsNullOrEmpty(Email);
            }
        }

        #endregion

        #region CTR

        public UsuariosModel()
        {
            
        }

        public UsuariosModel(IContextService context) : base(context)
        {
            
        }

        #endregion

        public override object generateId(string id)
        {
            return new Guid(id);
        }

        public override void createNewPrimaryKey()
        {
            primaryKey = getProperties().Where(f => f.property.Name == "Id").Select(f => f.property);
        }

        
       
    }
}
