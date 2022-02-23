using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Marfil.App.WebMain.Controllers
{
    public class ArticulosStockSeguridadController : GenericController<ArticulosStockSeguridadModel>
    {
        #region ctr
        public ArticulosStockSeguridadController(IContextService context) : base(context)
        {
        }

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "articulosstockseguridad";
            var permisos = appService.GetPermisosMenu("articulosstockseguridad");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }
        #endregion

    }
}