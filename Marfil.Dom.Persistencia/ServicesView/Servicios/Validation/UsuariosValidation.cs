using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Resources;
using RUsuarios =Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Usuarios;
namespace Marfil.Dom.Persistencia.ServicesView.Servicios.Validation
{
    class UsuariosValidation : BaseValidation<Usuarios>
    {
        public UsuariosValidation(IContextService context, MarfilEntities db) : base(context,db)
        {
        }

        public override bool ValidarGrabar(Usuarios model)
        {
            if (string.IsNullOrEmpty(model.usuario))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RUsuarios.usuario));

            if(string.IsNullOrEmpty(model.password))
                throw new ValidationException(string.Format(General.ErrorCampoObligatorio, RUsuarios.password));

            if(_db.Usuarios.Count(f=>f.id!=model.id && f.usuario==model.usuario)>0)
                throw new ValidationException(string.Format(RUsuarios.ErrorUsuarioDuplicado,model.usuario));

            return base.ValidarGrabar(model);
        }


    }
}
