using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Documentos.Regularizacion;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
{
    public class RegularizacionCierreAperturaController : GenericController<AsistenteCierreAperturaModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "regularizacioncierreapertura";
            var permisos = appService.GetPermisosMenu("regularizacioncierreapertura");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public RegularizacionCierreAperturaController(IContextService context) : base(context)
        {

        }

        #endregion

        #region index
        //Redirigir a la pantalla principal
        public override ActionResult Index()
        {
            return RedirectToAction("AsistenteRegularizacionCierreApertura");
        }

        public ActionResult AsistenteRegularizacionCierreApertura()
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService)
                {
                    var estadoact = service.GetEstadoEjercicio();
                    if (estadoact == 3)
                    {
                        throw new ValidationException("El ejercicio ya está cerrado");
                    }

                    var model = new AsistenteCierreAperturaModel(ContextService);

                    model.Fechacierre = service.GetFechaHastaEjercicio();
                    model.Fechaapertura = service.GetFechaDesdeEjercicioSig();
                    model.ComentarioCierre = service.GetComentarioCierre();
                    model.ComentarioApertura = service.GetComentarioApertura();
                    //Ayuda
                    var aux = model as IToolbar;
                    aux.Toolbar.Acciones = HelpItem();
                    return View(model);
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
        public ActionResult GenerarAsientoContable(string fechacierre, string fechaapertura, string comentariocierre, string comentarioapertura, string cuentas, string saldodeudor, string saldoacreedor)
        {
            try
            {
                using (var service = FService.Instance.GetService(typeof(VencimientosModel), ContextService) as VencimientosService)
                {
                    var listacuentas = cuentas.Split(';');
                    var listasaldodeudor = saldodeudor.Split(';');
                    var listasaldoacreedor = saldoacreedor.Split(';');

                    service.GenerarAsientoCierreApertura(fechacierre, fechaapertura, comentariocierre, comentarioapertura, listacuentas, listasaldodeudor, listasaldoacreedor);
                    TempData[Constantes.VariableMensajeExito] = string.Format("Se han generado correctamente los asientos");
                }
            }
            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return RedirectToAction("AsistenteRegularizacionCierreApertura");
            }

            return RedirectToAction("Index", "Movs");
        }

        #endregion
    }
}