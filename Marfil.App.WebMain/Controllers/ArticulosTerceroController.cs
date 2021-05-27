using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.App.WebMain.Controllers
{
    public class ArticulosTerceroController : GenericController<ArticulosTerceroModel>
    {
        #region ctr
        public ArticulosTerceroController(IContextService context) : base(context)
        {
        }

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "articulostercero";
            var permisos = appService.GetPermisosMenu("articulostercero");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }
        #endregion

    }
}