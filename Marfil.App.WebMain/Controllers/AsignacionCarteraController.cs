using System.Web.Mvc;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using System;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.App.WebMain.Misc;
using Resources;
using System.Linq;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class AsignacionCarteraController : GenericController<VencimientosModel>
    {

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "asignacioncartera";
            var permisos = appService.GetPermisosMenu("asignacioncartera");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public AsignacionCarteraController(IContextService context) : base(context)
        {

        }

        #endregion

        #region index

        //Redirigir a la pantalla principal
        public override ActionResult Index()
        {
            return RedirectToAction("AsistenteAsignacionCartera");
        }

        public ActionResult AsistenteAsignacionCartera()
        {
            var model = new AsistenteAsignacionModel(ContextService);
            model.FechaContable = DateTime.Now;
            var aux = model as IToolbar;
            aux.Toolbar.Acciones = HelpItem();
            return View(model);
        }

        //Fin del asistente
        [HttpPost]
        public ActionResult GenerarCartera(StAsistenteTesoreria model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (
                        var service =
                            FService.Instance.GetService(typeof(VencimientosModel), ContextService) as
                                VencimientosService)
                    {
                        service.AsignarCartera(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                    }
                }
                else
                    TempData["errors"] = string.Join(",", ModelState.Values.Where(f => f.Errors.Any()).Select(f => string.Join(",", f.Errors.Select(j => j.ErrorMessage))));

            }
            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
            }
            return RedirectToAction("AsistenteAsignacionCartera");
        }

        #endregion

    }
}