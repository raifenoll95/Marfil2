using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Marfil.Dom.Campoverificacion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RGrupos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Gruposusuarios;
namespace Marfil.Dom.Persistencia.Model
{
    public class RolesModel : BaseModel<RolesModel, Roles>
    {
        #region Properties

        private Guid _id = Guid.NewGuid();

        [Required]
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        [Required]
        [MaxLength(30)]
        [Display(ResourceType = typeof(RGrupos), Name = "Role")]
        public string Role { get; set; }

        private GruposUsuariosModel _gruposUsuariosModel = new GruposUsuariosModel();
        [Display(ResourceType = typeof(RGrupos), Name = "Usuarios")]
        public GruposUsuariosModel Usuarios
        {
            get { return _gruposUsuariosModel; }
            set { _gruposUsuariosModel = value; }
        }

        private PermisosModel _permisos=new PermisosModel();
        public PermisosModel Permisos
        {
            get { return _permisos; }
            set { _permisos = value; }
        }

        #endregion

        #region CTR

        public RolesModel()
        {
            
        }

        public RolesModel(IContextService context) : base(context)
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

        public override string DisplayName => RGrupos.TituloEntidad;
    }

    public class GruposUsuariosModel: ICampoverificacionModel
    {
        private IEnumerable<UsuariosRelacionRolesModel> _usuarios = new List<UsuariosRelacionRolesModel>();
        [Display(ResourceType = typeof(RGrupos), Name = "Usuarios")]
        public IEnumerable<UsuariosRelacionRolesModel> usuarios
        {
            get { return _usuarios; }
            set { _usuarios = value; }
        }

        public GruposUsuariosModel()
        {
            _id = Guid.NewGuid();
        }

        private readonly  Guid _id;
        public Guid id => _id;
        public string api => "UsuariosApi";
        public string displayName => "usuario";
        public string valueName => "id";
    }

    [Serializable]
    public class UsuariosRelacionRolesModel
    {
        public string usuario { get; set; }
        public Guid id { get; set; }
    }
}
