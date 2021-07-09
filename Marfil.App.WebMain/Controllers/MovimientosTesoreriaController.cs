using System.Web.Mvc;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using System;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.App.WebMain.Misc;
using Resources;
using System.Linq;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class MovimientosTesoreriaController : GenericController<AsistenteMovimientosTesoreriaModel>
    {

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "movimientostesoreria";
            var permisos = appService.GetPermisosMenu("movimientostesoreria");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public MovimientosTesoreriaController(IContextService context) : base(context)
        {

        }

        #endregion

        #region index

        //Redirigir a la pantalla principal
        public override ActionResult Index()
        {
            return RedirectToAction("AsistenteMovimientosTesoreria");
        }

        public ActionResult AsistenteMovimientosTesoreria()
        {
            return View(new AsistenteMovimientosTesoreriaModel(ContextService)
            {
                FechaContable = DateTime.Now
            });
        }

        //Fin del asistente
        [HttpPost]
        public ActionResult AsignacionMovimientosTesoreria(StAsistenteTesoreria model)
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
                        service.AsignarMovimientosTesoreria(model);
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

            return RedirectToAction("AsistenteMovimientosTesoreria");
        }

        [HttpGet]
        public string ObtenerPreferido(int? valor)
        {
            var modoPago = "";
            var cod = "";
            var gruposPago = WebHelper.GetApplicationHelper().GetListGruposFormasPago();

            using ( var service = FService.Instance.GetService(typeof(CircuitoTesoreriaCobrosModel), ContextService) as CircuitosTesoreriaCobrosService)
            {
                modoPago = service.GetPagoPreferido(valor);
            }

            if (modoPago != null && modoPago != "")
            {
                cod = gruposPago.Where(f => f.Descripcion == modoPago).FirstOrDefault().Valor;
            }

            return cod;
        }

        #endregion

    }
}