using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class ConfiguracionController :  GenericController<ConfiguracionModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            
            IsActivado = ContextService.IsSuperAdmin;
            CanModificar = CanCrear = CanEliminar = IsActivado;
        }

        #region CTR

        public ConfiguracionController(IContextService context) : base(context)
        {

        }

        #endregion

        // GET: Configuracion
        public override ActionResult Index()
        {
            using (var gestionService = createService(new ConfiguracionModel(ContextService)) as ConfiguracionService)
            {
                
                return View(gestionService.GetModel());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ConfiguracionModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var modelview = Helper.fModel.GetModel<ConfiguracionModel>(ContextService);
                    using (var gestionService = createService(modelview) as ConfiguracionService)
                    {
                        gestionService.CreateOrUpdate(model);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return View(model);
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                   .SelectMany(x => x.Errors)
                   .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                
                return View(model);
            }
        }

       
    }
}