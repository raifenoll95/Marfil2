using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Helpers
{
    
    public class CustomDisplayDescriptionAttribute : DisplayNameAttribute
    {
        public enum TipoDescripcion
        {
            Principal,
            Secundaria
        }

        private readonly TipoDescripcion _tipodescripcion;
        private readonly Type _resource;
        private readonly string _displayName;
        private readonly ApplicationHelper _appService;
        public CustomDisplayDescriptionAttribute(Type resource,string displayName, TipoDescripcion tipo) : base(displayName)
        {
            _resource = resource;
            _tipodescripcion = tipo;
            _displayName = displayName;
            
        }

        public override string DisplayName
        {
            get
            {
                var rm = new ResourceManager(_resource);
                return rm.GetString(_displayName) + " " +  (_tipodescripcion ==TipoDescripcion.Principal ? ApplicationHelper.IdiomaPrincipal: ApplicationHelper.IdiomaSecundario);
            }
        }
    }
}
