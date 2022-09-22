using Marfil.App.WebMain.Controllers;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Documentos.Albaranes;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Controllers
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

        public ActionResult AsistenteLecturas(int referencia)
        {
            try
            {
                var appService = new ApplicationHelper(ContextService);

                var model = new AsistenteLecturasModel(ContextService);
                model.Referencia = referencia;
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
        [HttpPost]
        public ActionResult GenerarLecturas(string identificador, string fecha, int registros, int referencia)
        {            
            try
            {
                using (var tran = TransactionScopeBuilder.CreateTransactionObject())
                {
                    using (var service = FService.Instance.GetService(typeof(EntregasStockModel), ContextService) as EntregasService)
                    {
                        var idparse = referencia.ToString();
                        var model = service.get(idparse) as AlbaranesModel;
                        var vueltas = 0;

                        var lineas = service.GetLecturas(identificador);

                        foreach (var item in lineas)
                        {
                            item.Id = vueltas + 1;

                            var tarifa = service.GetTarifa(item.Fkarticulos, model.Fkclientes);
                            item.Precio = tarifa;

                            model.Lineas.Add(item);

                            vueltas++;
                        }

                        service.edit(model);

                        TempData[Constantes.VariableMensajeExito] = string.Format("Se ha generado correctamente");
                    }
                    tran.Complete();
                }
                
            }
            catch (Exception ex)
            {
                TempData[Constantes.VariableMensajeWarning] = ex.Message;
                return RedirectToAction("AsistenteLecturas", new { referencia = referencia});
            }

            return RedirectToAction("Edit", "EntregasStock", new { id = referencia });
            
        }

    }
}