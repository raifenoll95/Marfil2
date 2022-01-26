using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Documentos.Regularizacion;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
{
    public class RegularizacionRevertirEstadoController : GenericController<AsistenteRervertirEstadoEjercicioModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "regularizacionrevertirestado";
            var permisos = appService.GetPermisosMenu("regularizacionrevertirestado");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public RegularizacionRevertirEstadoController(IContextService context) : base(context)
        {

        }

        #endregion

        #region index
        //Redirigir a la pantalla principal
        public override ActionResult Index()
        {
            return RedirectToAction("AsistenteRegularizacionRevertirEstado");
        }

        public ActionResult AsistenteRegularizacionRevertirEstado()
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService)
                {
                    var estadoact = service.GetEstadoEjercicio();
                    if (estadoact == 0)
                    {
                        throw new ValidationException("Un ejercicio en estado Abierto no se puede revertir");
                    }
                    return View(new AsistenteRervertirEstadoEjercicioModel(ContextService)
                    {
                    });
                }
            }
            catch (Exception ex)
            {

                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return Redirect("~/");
            }
        }

        //Fin del asistente
        [HttpPost]
        public ActionResult GenerarAsientoContable(string estado)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(VencimientosModel), ContextService) as VencimientosService)
                {
                    service.AnularAsientos(estado);
                    TempData[Constantes.VariableMensajeExito] = string.Format("Se han revertido correctamente el/los asiento/s");
                }
            }
            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return RedirectToAction("AsistenteRegularizacionRevertirEstado");
            }

            return RedirectToAction("Index", "Movs");
        }

        #endregion
    }
}