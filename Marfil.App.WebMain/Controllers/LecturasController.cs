using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marfil.App.WebMain.Controllers
{
    public class LecturasController : GenericController<LecturasModel>
    {
        #region CTR
        public LecturasController(IContextService context) : base(context)
        {
        }
        #endregion

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "Lecturas";
            var permisos = appService.GetPermisosMenu("Lecturas");
            IsActivado = permisos.IsActivado;
            CanCrear = false;
            CanModificar = false;
            CanEliminar = permisos.CanEliminar;
        }
    }
}