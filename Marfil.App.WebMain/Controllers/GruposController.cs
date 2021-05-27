using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.App.WebMain.Controllers
{
    
    public class GruposController : GenericController<RolesModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            
            IsActivado = ContextService.IsSuperAdmin;
            CanModificar = CanCrear = CanEliminar = IsActivado;
        }

        #region CTR

        public GruposController(IContextService context) : base(context)
        {

        }

        #endregion
    }
}