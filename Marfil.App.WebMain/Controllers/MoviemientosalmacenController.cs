using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using RMovimientosAlmacen =Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Movimientosalmacen;
namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class MovimientosalmacenController : BaseController
    {
        #region Members

        private readonly IMovimientosAlmacen _service;
        private readonly IContextService _context;

        #endregion

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        #region CTR

        public MovimientosalmacenController(IMovimientosAlmacen service, IContextService context):base(context)
        {
            _service = service;
            _context = context;
        }

        #endregion

        protected override void CargarParametros()
        {
            MenuName = "cambioubicacion";
            var permisos = appService.GetPermisosMenu("cambioubicacion");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }
        
        public ActionResult CambioUbicacion()
        {
            var fmodel = new FModel();
            var model = fmodel.GetModel<CambioUbicacionModel>(ContextService);
            model.Toolbar.Acciones = HelpItem();
            return View( model);
        }

        [HttpPost]
        public ActionResult CambioUbicacion(CambioUbicacionModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _service.GenerarMovimientoAlmacen(model.Lote,model.Fkalmacen,model.Fkzonaalmacen);
                    TempData[Constantes.VariableMensajeExito] = RMovimientosAlmacen.MovimientoRealizadoCorrectamente;
                    return RedirectToAction("CambioUbicacion");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("",ex.Message);
            }

            return View(model);
        }
    }
}