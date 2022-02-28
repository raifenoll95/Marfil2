using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marfil.App.WebMain.Controllers
{
    public class LogStockSeguridadController : GenericController<LogStockSeguridadModel>
    {
        #region CTR
        public LogStockSeguridadController(IContextService context) : base(context)
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
            MenuName = "LogStockSeguridad";
            var permisos = appService.GetPermisosMenu("LogStockSeguridad");
            IsActivado = true;
            CanCrear = false;
            CanModificar = false;
            CanEliminar = false;
        }
    }
}