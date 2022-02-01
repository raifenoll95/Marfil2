using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion;
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
    public class RegularizacionAperturaProvisionalController : GenericController<AsistenteAperturaProvisionalModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "aperturaprovisional";
            var permisos = appService.GetPermisosMenu("aperturaprovisional");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public RegularizacionAperturaProvisionalController(IContextService context) : base(context)
        {

        }

        #endregion

        #region index

        //Redirigir a la pantalla principal
        public override ActionResult Index()
        {
            return RedirectToAction("AsistenteAperturaProvisional");
        }

        public ActionResult AsistenteAperturaProvisional()
        {
            try
            {
                var appService = new ApplicationHelper(ContextService);

                //Comprobamos el estado del ejercicio
                var serviceEjercicios = new EjerciciosService(ContextService);
                var idejerc = int.Parse(ContextService.Ejercicio);
                var idejercicioant = serviceEjercicios.getAll().Where(f => ((EjerciciosModel)f).Id == idejerc).Select(f => f as EjerciciosModel).FirstOrDefault().Fkejercicios;
                var ejercicio = serviceEjercicios.getAll().Where(f => ((EjerciciosModel)f).Id == idejercicioant).Select(f => f as EjerciciosModel).FirstOrDefault();

                if (ejercicio == null)
                {
                    throw new ValidationException("No existe ejercicio anterior");
                }
                else
                {
                    if (ejercicio.Estado != EstadoEjercicio.Existencias && ejercicio.Estado != EstadoEjercicio.Abierto)
                    {
                        throw new ValidationException("Ejercicio anterior en proceso de cierre");
                    }
                }

                var model = new AsistenteAperturaProvisionalModel(ContextService);

                using (var service = FService.Instance.GetService(typeof(ConfiguracionModel), ContextService) as ConfiguracionService)
                {
                    model.Fechaapertura = service.GetFechaDesdeEjercicioAct();
                    model.ComentarioAperturaProvisional = service.GetComentarioAperturaProvisional();
                    model.CuentaDesde = serviceEjercicios.GetCuentaDesdeProvisional();
                    model.CuentaHasta = serviceEjercicios.GetCuentaHastaProvisional();
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
        public ActionResult GenerarAsientoContable(string fechaapertura, string comentarioaperturaprovisional, string cuentaaperturaprovisional, string cuentas, string saldodeudor, string saldoacreedor)
        {
            var model = Helper.fModel.GetModel<MovsModel>(ContextService);
            try
            {
                using (var service = FService.Instance.GetService(typeof(VencimientosModel), ContextService) as VencimientosService)
                {
                    var listacuentas = cuentas.Split(';');
                    var listasaldodeudor = saldodeudor.Split(';');
                    var listasaldoacreedor = saldoacreedor.Split(';');

                    service.GenerarAsientoAperturaProvisional(fechaapertura, comentarioaperturaprovisional, cuentaaperturaprovisional, listacuentas, listasaldodeudor, listasaldoacreedor);
                    TempData[Constantes.VariableMensajeExito] = string.Format("Se ha generado correctamente el asiento de apertura provisional");
                }
            }
            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return RedirectToAction("AsistenteAperturaProvisional");
            }

            return RedirectToAction("Index", "Movs");
        }

        #endregion
    }
}