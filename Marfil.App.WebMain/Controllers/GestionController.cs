using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class GestionController : BaseController
    {
        #region Properties

        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        #endregion

        public GestionController(IContextService context):base(context)
        {
            
        }

        // GET: Paises
        public ActionResult Index(string id, string mantenimiento)
        {
            using (var gestionService = createService(mantenimiento))
            {
               // loadDisplayData(mantenimiento);
                return View(gestionService.getAll());
            }
        }

        // GET: Paises/Details/5
        public ActionResult Details(string id, string mantenimiento)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var gestionService = createService(mantenimiento))
            {
                loadDisplayData(mantenimiento);
                var paises = gestionService.get(id);
                if (paises == null)
                {
                    return HttpNotFound();
                }
                return View(paises);
            }
        }

        // GET: Paises/Create
        public ActionResult Create(string mantenimiento)
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            loadDisplayData(mantenimiento);

            if (TempData["model"] == null)
            {
                var newModel = Activator.CreateInstance(Helper.getTypeFromString(mantenimiento)) as IModelView;
                return View(newModel);
            }

            return View(TempData["model"]);
            
        }

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOperacion(string id, string mantenimiento)
        {
            var model = Helper.createModelFromForm(mantenimiento);
            if (ModelState.IsValid)
            {
                loadDisplayData(mantenimiento);
                using (var gestionService = createService(mantenimiento))
                {
                    gestionService.create(model);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                    return RedirectToAction("Index","Gestion", new { @mantenimiento = mantenimiento });
                }
            }
            TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage));
            TempData["model"] = model;
            return RedirectToAction("Create","Gestion",new { @mantenimiento = mantenimiento});
        }

        // GET: Paises/Edit/5
        public ActionResult Edit(string id, string mantenimiento)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            loadDisplayData(mantenimiento);
            using (var gestionService = createService(mantenimiento))
            {
               
                if (TempData["model"] != null)
                    return View(TempData["model"]);

                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                return View(model);
            }
        }

        // POST: Paises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditOperacion(string id,string mantenimiento)
        {
            var model = Helper.createModelFromForm(mantenimiento);
            if (ModelState.IsValid)
            {
                using (var gestionService = createService(mantenimiento))
                {
                    loadDisplayData(mantenimiento);
                    gestionService.edit(model);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                    return RedirectToAction("Index","Gestion", new { @mantenimiento = mantenimiento });
                }
            }
            TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                .SelectMany(x => x.Errors)
                .Select(x => x.ErrorMessage));
            TempData["model"] = model;
            return RedirectToAction("Edit","Gestion", new { @mantenimiento = mantenimiento });
        }

        // GET: Paises/Delete/5
        public ActionResult Delete(string id, string mantenimiento)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            using (var gestionService = createService(mantenimiento))
            {
                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                loadDisplayData(mantenimiento);
                return View(model);
            }
        }

        // POST: Paises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id, string mantenimiento)
        {
            
            using (var gestionService = createService(mantenimiento))
            {
                loadDisplayData(mantenimiento);
                var model = gestionService.get(id);
                gestionService.delete(model);
                return RedirectToAction("Index","Gestion",new { @mantenimiento = mantenimiento});
            }
        }

        #region Helpers

        private IGestionService createService(string mantenimiento)
        {
            var modelType = Helper.getTypeFromString(mantenimiento);
            var model = Activator.CreateInstance(modelType) as IModelViewExtension;
            var genericType = typeof(GestionService<,>);
            Type[] typeArgs = { modelType, model.persistencyType };
            var repositoryType = genericType.MakeGenericType(typeArgs);

            return Activator.CreateInstance(repositoryType) as IGestionService;
        }

        private void loadDisplayData(string mantenimiento)
        {
            ViewBag.Columnas = typeof(Helper).GetMethod("getProperties")
                            .MakeGenericMethod(new Type[] { Helper.getTypeFromString(mantenimiento) }).Invoke(null, null);
            ViewBag.TituloMantenimiento = Helper.getTitleFromString(mantenimiento);
            ViewBag.Mantenimiento = mantenimiento;
        }

        protected override void CargarParametros()
        {
            MenuName = System.Web.HttpContext.Current.Request.Params["mantenimiento"];
            var permisos = appService.GetPermisosMenu(MenuName);
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanEliminar = permisos.CanEliminar;
            CanModificar = permisos.CanModificar;
        }

        #endregion

       
    }
}
