using Marfil.App.WebMain.Controllers;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain
{
    [Authorize]
    public class AsistenteLecturasController : GenericController<AsistenteLecturasModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "asistentelecturas";
            var permisos = appService.GetPermisosMenu("asistentelecturas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public AsistenteLecturasController(IContextService context) : base(context)
        {

        }

        #endregion

        #region index

        //Redirigir a la pantalla principal
        public override ActionResult Index()
        {
            return RedirectToAction("AsistenteLecturas");
        }

        public ActionResult AsistenteLecturas()
        {
            try
            {
                var appService = new ApplicationHelper(ContextService);

                var model = new AsistenteLecturasModel(ContextService);
                //Ayuda
                var aux = model as IToolbar;
                aux.Toolbar.Acciones = HelpItem();
                return View(model);
            }
            catch (Exception ex)
            {

                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return Redirect("~/");
            }

        }

        #endregion

        //Fin del asistente
        /*[HttpPost]
        public ActionResult GenerarAsientoContable(string fecharegularizacion, string seriecontable, string comentarioiniciales, string comentariofinales, string cuentasexistencias, string saldoiniciales, string cuentasvariacion, string importefinales)
        {
            var model = Helper.fModel.GetModel<MovsModel>(ContextService);
            try
            {
                using (var service = FService.Instance.GetService(typeof(VencimientosModel), ContextService) as VencimientosService)
                {
                    var listacuentasexistencias = cuentasexistencias.Split(';');
                    var listasaldoiniciales = saldoiniciales.Split(';');
                    var listacuentasvariacion = cuentasvariacion.Split(';');
                    var listaimportefinales = importefinales.Split(';');

                    model = service.GenerarAsientoRegularizacionExistencias(fecharegularizacion, seriecontable, comentarioiniciales, comentariofinales, listacuentasexistencias, listasaldoiniciales, listacuentasvariacion, listaimportefinales);
                    TempData[Constantes.VariableMensajeExito] = string.Format("Se ha generado correctamente el asiento");
                }
            }
            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return RedirectToAction("AsistenteRegularizacionExistencias");
            }

            //return RedirectToAction("AsistenteRegularizacionExistencias");
            return RedirectToAction("Details", "Movs", new { id = model.Id });
        }*/

    }
}