using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
using Marfil.Dom.Persistencia.Model.Contabilidad.Movs;
using Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos;
using Marfil.Dom.Persistencia.Model.Documentos.RegularizacionExistencias;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class RegularizacionExistenciasController : GenericController<AsistenteRegularizacionExistenciasModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "regularizacionexistencias";
            var permisos = appService.GetPermisosMenu("regularizacionexistencias");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public RegularizacionExistenciasController(IContextService context) : base(context)
        {

        }

        #endregion

        #region index

        //Redirigir a la pantalla principal
        public override ActionResult Index()
        {
            return RedirectToAction("AsistenteRegularizacionExistencias");
        }

        public ActionResult AsistenteRegularizacionExistencias()
        {
            try
            {
                var appService = new ApplicationHelper(ContextService);

                //Comprobamos el estado del ejercicio
                var serviceEjercicios = new EjerciciosService(ContextService);
                var idejerc = int.Parse(ContextService.Ejercicio);
                var ejercicio = serviceEjercicios.getAll().Where(f => ((EjerciciosModel)f).Id == idejerc).Select(f => f as EjerciciosModel).FirstOrDefault();

                if (ejercicio.Estado != EstadoEjercicio.Abierto)
                {
                    throw new ValidationException("No se puede realizar una regularización de existencias en un ejercicio que no esté en estado Abierto");
                }

                using (var service = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService)
                {
                    return View(new AsistenteRegularizacionExistenciasModel(ContextService)
                    {
                        Fecharegularizacion = service.GetFechaHastaEjercicio(),
                        Fkseriescontables = service.GetSerieContable(),
                        ComentarioExistenciasIniciales = service.GetComentarioIni(),
                        ComentarioExistenciasFinales = service.GetComentarioFin()
                    });
                }
            }
            catch (Exception ex)
            {

                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return Redirect("~/");
            }
            
        }

        #endregion

        //Fin del asistente
        [HttpPost]
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

                    model =  service.GenerarAsientoRegularizacionExistencias(fecharegularizacion, seriecontable, comentarioiniciales, comentariofinales, listacuentasexistencias, listasaldoiniciales, listacuentasvariacion, listaimportefinales);
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
        }

    }
}