using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http.Results;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Busquedas;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Resources;
using System.IO;
using System.Data;
using Marfil.Dom.Persistencia.Model.Contabilidad;
using System.Web.UI.WebControls;
using System.Web.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Marfil.App.WebMain.Controllers
{
    [Authorize]
    public class CuentasController : GenericController<CuentasModel>
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }
        protected override void CargarParametros()
        {
            MenuName = "cuentas";
            var permisos = appService.GetPermisosMenu("cuentas");
            IsActivado = permisos.IsActivado;
            CanCrear = permisos.CanCrear;
            CanModificar = permisos.CanModificar;
            CanEliminar = permisos.CanEliminar;
            CanBloquear = permisos.CanBloquear;
        }

        #region CTR

        public CuentasController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Create 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult CreateOperacion(CuentasModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    var modelview = Helper.fModel.GetModel<CuentasModel>(ContextService);
                    
                    using (var gestionService = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
                    {
                        gestionService.CrearEditarCuenta(model, GetSuperCuentas(model), OperacionCuenta.Crear);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;

                        var returnUrl = Request.Params["ReturnUrl"];
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return Redirect(returnUrl + "?cp="+ model.Id);
                        }
                        return RedirectToAction("Index");
                    }
                }

                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                   .SelectMany(x => x.Errors)
                   .Select(x => x.ErrorMessage));
                TempData["model"] = model;

                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
        }


        #endregion

        #region Edit

        public override ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            
            var modelview = Helper.fModel.GetModel<CuentasModel>(ContextService);
            
            using (var gestionService = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                var model = TempData["model"] != null ? TempData["model"] as CuentasModel : gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }

                 ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                return View(model);
            }
        }

        // POST: Paises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(CuentasModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    
                    
                    using (var gestionService = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
                    {
                        gestionService.CrearEditarCuenta(model, GetSuperCuentas(model), OperacionCuenta.Editar);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = obj.get(objExt.primaryKey.First().Name) });
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = obj.get(objExt.primaryKey.First().Name) });
            }
        }

        #endregion

        #region Eliminar

        // GET: Paises/Delete/5
        public override ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            
            using (var gestionService = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Baja, model);
                return View(model);
            }
        }

        // POST: Paises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public override ActionResult DeleteConfirmed(string id)
        {
            try
            {
                
                var newmodel = Helper.fModel.GetModel<CuentasModel>(ContextService);
                using (var gestionService = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
                {

                    var model = gestionService.get(id);
                    gestionService.delete(model);
                    TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
               
                TempData["errors"] = ex.Message;
                return RedirectToAction("Delete", new { id = id });
            }

        }

        #endregion

        #region Details

        // GET: Paises/Details/5
        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            using (var gestionService = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                
                var model = gestionService.get(id);
                if (model == null)
                {
                    return HttpNotFound();
                }
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                return View(model);
            }
        }

        #endregion

        #region Api

        [Authorize]
        public ActionResult CuentasCliente()
        {
            
            using (var gestionService = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                var result = new ResultBusquedas<CuentasModel>()
                {
                    values = gestionService.GetCuentasClientes().ToList(),
                    columns = new[]
                   {
                        new ColumnDefinition() { field = "Id", displayName = "Cuenta", visible = true },
                        new ColumnDefinition() { field = "Descripcion", displayName = "Descripción", visible = true },
                    }
                };
                
                return Content(JsonConvert.SerializeObject(result), "application/json");
            }
        }

        [Authorize]
        public ActionResult SuperCuentas(string id)
        {
            
            using (var gestionService = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                
                var list = gestionService.GetSuperCuentas(id ).ToList();
                return Content(JsonConvert.SerializeObject(list), "application/json");
            }
        }
        
        private struct ExistItem
        {
            public bool Existe;
        }

        [Authorize]
        public override ActionResult Exists(string id)
        {
            
            using (var gestionService = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
            {
                var obj = gestionService.exists(id);
                return Content(JsonConvert.SerializeObject(new ExistItem() {Existe= obj}), "application/json");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bloquear(string id,string returnurl,string motivo,bool operacion)
        {
            if (CanBloquear)
            {
                
                using (var gestionService = FService.Instance.GetService(typeof(CuentasModel),ContextService) as CuentasService)
                {
                    gestionService.Bloquear(id, motivo, ContextService.Id.ToString(), operacion);
                }
            }
            else
            {
                ModelState.AddModelError("",General.LblErrorBloqueoNoPermitido);
            }

            return Redirect(returnurl);
        }

        #endregion

        #region Helpers

        protected override IGestionService createService(IModelView model)
        {
            return FService.Instance.GetService(typeof(CuentasModel), ContextService);
        }

        private IEnumerable<CuentasModel> GetSuperCuentas(CuentasModel model)
        {
            var keysId = Request.Params.AllKeys.Where(f => f.StartsWith("id_"));

            var result = new List<CuentasModel>();
            foreach (var item in keysId)
            {
                var id = item.Replace("id_", "");
                var newItem = new CuentasModel()
                {
                    Id=id,
                    Descripcion = Request.Params["nombre_"+id],
                    Descripcion2 = Request.Params["descripcion_"+id],
                    Empresa = Request.Params["Empresa"],
                    Nivel = id.Length,
                    UsuarioId = model.UsuarioId
                };
                result.Add(newItem);
            }

            return result;
        }

        [HttpGet]
        public string CambiarDescripcion(string cuenta)
        {
            using (var service = new CuentasService(ContextService))
            {
                return service.GetDescripcionCuenta(cuenta);
            }
        }

        #endregion
    }
}