using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Stock;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RActualizacionCostes = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.ActualizacionCostes;

namespace Marfil.App.WebMain.Controllers
{
    public class ActualizarCostesController : BaseController
    {
        #region Members

        private readonly IContextService _context;

        #endregion

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        #region CTR

        public ActualizarCostesController(IContextService context) : base(context)
        {
            _context = context;
        }

        #endregion

        protected override void CargarParametros()
        {
            MenuName = "actualizarcostes";
            var permisos = appService.GetPermisosMenu("actualizarcostes");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
        }

        public ActionResult ActualizarCostes()
        {
            var model = new ActualizarCostesModel(ContextService);
            using (var service = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService)
            {
                model.FechaDesde = service.GetFechaDesdeEjercicioAct();
                model.FechaHasta = service.GetFechaHastaEjercicio();
                //Ayuda
                var aux = model as IToolbar;
                aux.Toolbar.Acciones = HelpItem();
                return View(model);
            }                    
        }

        [HttpPost]
        public ActionResult ActualizarCostes(ActualizarCostesModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Series.Count == 0)
                    {
                        throw new ValidationException("Debe indicar, al menos, una serie");
                    }
                    if (model.FechaDesde == null || model.FechaHasta == null)
                    {
                        throw new ValidationException("Las fechas desde y hasta deben tener valor");
                    }
                    var service = new ActualizarCostesService(_context);
                    service.ActualizarCostes(model);
                    TempData[Constantes.VariableMensajeExito] = RActualizacionCostes.RealizadoCorrectamente;
                }
            }
            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
            }

            return RedirectToAction("ActualizarCostes");
        }
    }
}