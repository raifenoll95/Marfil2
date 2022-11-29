using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marfil.App.WebMain.Controllers
{
    public class VerificarContabilidadController : GenericController<VerificarContabilidadModel>
    {
        #region CTR
        public VerificarContabilidadController(IContextService context) : base(context)
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
            MenuName = "VerificarContabilidad";
            var permisos = appService.GetPermisosMenu("VerificarContabilidad");
            IsActivado = true;
            CanCrear = false;
            CanModificar = false;
            CanEliminar = false;
        }
    }
}