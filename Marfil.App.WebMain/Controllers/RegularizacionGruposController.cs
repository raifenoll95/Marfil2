using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
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
    [Authorize]
    public class RegularizacionGruposController : GenericController<AsistenteRegularizacionGruposModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "regularizaciongrupos";
            var permisos = appService.GetPermisosMenu("regularizaciongrupos");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public RegularizacionGruposController(IContextService context) : base(context)
        {

        }

        #endregion

        #region index

        //Redirigir a la pantalla principal
        public override ActionResult Index()
        {
            return RedirectToAction("AsistenteRegularizacionGrupos");
        }

        public ActionResult AsistenteRegularizacionGrupos()
        {
            try
            {
                var appService = new ApplicationHelper(ContextService);

                //Comprobamos el estado del ejercicio
                var serviceEjercicios = new EjerciciosService(ContextService);
                var idejerc = int.Parse(ContextService.Ejercicio);
                var ejercicio = serviceEjercicios.getAll().Where(f => ((EjerciciosModel)f).Id == idejerc).Select(f => f as EjerciciosModel).FirstOrDefault();

                if (ejercicio.Estado != EstadoEjercicio.Existencias)
                {
                    throw new ValidationException("No se puede realizar una regularización de grupos 6 y 7 en un ejercicio que no esté en estado Regularización Existencias");
                }

                using (var service = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService)
                {
                    return View(new AsistenteRegularizacionGruposModel(ContextService)
                    {
                        Fecharegularizacion = service.GetFechaHastaEjercicio(),
                        Fkseriescontables = service.GetSerieContable(),
                        ComentarioDebePYG = service.GetComentarioDebe(),
                        ComentarioHaberPYG = service.GetComentarioHaber(),
                        ComentarioCuentasDetalle = service.GetComentarioDetalle()
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
        public ActionResult GenerarAsientoContable(string fecharegularizacion, string seriecontable, string cuentapyg, string comentariodebepyg, string comentariohaberpyg, string comentariocuentasdetalle, string cuentasgrupos, string saldodeudor, string saldoacreedor)
        {
            var model = Helper.fModel.GetModel<MovsModel>(ContextService);
            try
            {
                using (var service = FService.Instance.GetService(typeof(VencimientosModel), ContextService) as VencimientosService)
                {
                    var listacuentasgrupos = cuentasgrupos.Split(';');
                    var listasaldodeudor = saldodeudor.Split(';');
                    var listasaldoacreedor = saldoacreedor.Split(';');

                    service.GenerarAsientoRegularizacionGrupos(fecharegularizacion, seriecontable, cuentapyg, comentariodebepyg, comentariohaberpyg, comentariocuentasdetalle, listacuentasgrupos, listasaldodeudor, listasaldoacreedor);
                    TempData[Constantes.VariableMensajeExito] = string.Format("Se ha generado correctamente el asiento");
                }
            }
            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return RedirectToAction("AsistenteRegularizacionGrupos");
            }

            return RedirectToAction("Index", "Movs");
        }

        #endregion
    }
}