using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios
{
    public interface IGruposService
    {

    }

    public class GruposService : GestionService<RolesModel, Roles>, IGruposService
    {
        #region CTR

        public GruposService(IContextService context, MarfilEntities db = null) : base(context, db)
        {

        }

        #endregion

        public override ListIndexModel GetListIndexModel(Type t, bool canEliminar, bool canModificar, string controller)
        {
            var model = base.GetListIndexModel(t, canEliminar, canModificar, controller);
            model.ExcludedColumns = new[] { "Id", "Usuarios", "Permisos","Toolbar" };
            return model;
        }

        //Rai
        public List<RolesModel> getRoles(string user)
        {
            var roles = getAll().Select(f => (RolesModel)f).ToList();
            List<RolesModel> rolesUsuario = new List<RolesModel>();

            //Recorremos todos los roles
            foreach (var rol in roles)
            {
                //Miramos en el usuario del grupo de usuarios de ese rol
                foreach (var usuario in rol.Usuarios.usuarios)
                {
                    if (usuario.usuario == user)
                    {
                        rolesUsuario.Add(rol);
                    }
                }
            }

            return rolesUsuario;
        }
    }
}
