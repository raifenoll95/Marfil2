using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Resources;
using RGrupos = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Gruposusuarios;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class RolesValidation : BaseValidation<Roles>
    {
        #region CTR

        public RolesValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        #endregion

        public override bool ValidarGrabar( Roles model)
        {
            try
            {
                if(string.IsNullOrEmpty(model.role))
                    throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RGrupos.Role));

                if (_db.Roles.Any(f => f.id != model.id && f.role == model.role))
                    throw new ValidationException(RGrupos.ErrorGrupoExistente+ " " + model.role);

                foreach (var item in model.Usuarios)
                {
                    if (model.Usuarios.Count(f => f.id == item.id) > 1)
                    {
                        throw new ValidationException(string.Format(RGrupos.ErrorUsuarioExistente, item.usuario));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ValidationException(ex);
            }
            return true;
        }
    }
}
