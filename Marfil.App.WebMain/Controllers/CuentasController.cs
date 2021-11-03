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

        #region AsistenteImportación

        public ActionResult AsistenteCuentas()
        {
            AsistenteCuentasModel model = new AsistenteCuentasModel();

            model.Iso = new List<SelectListItem> {                
                new SelectListItem { Text = "Iso alfanumérico 2", Value = "CodigoIsoAlfa2" },
                new SelectListItem { Text = "Iso alfanumérico 3", Value = "CodigoIsoAlfa3" },
                new SelectListItem { Text = "Iso numérico", Value = "CodigoIsoNumerico" }
                };                            

            return View("AsistenteCuentas", model);
        }

        [HttpPost]
        public ActionResult AsistenteCuentas(AsistenteCuentasModel model)
        {
            var idPeticion = 0;

            // Para que no de error al devolver la vista, en un futuro cambiar esto
            model.Iso = new List<SelectListItem> {
                new SelectListItem { Text = "Iso alfanumérico 2", Value = "CodigoIsoAlfa2" },
                new SelectListItem { Text = "Iso alfanumérico 3", Value = "CodigoIsoAlfa3" },
                new SelectListItem { Text = "Iso numérico", Value = "CodigoIsoNumerico" }
            };

            var file = model.Fichero;            
            char delimitador = model.Delimitador.ToCharArray()[0];
            string iso = model.SelectedId;            

            if (ModelState.IsValid)
            {
                if (file != null && file.ContentLength > 0)
                {
                    if (file.FileName.ToLower().EndsWith(".csv") || file.FileName.ToLower().EndsWith(".CSV"))
                    {                        
                        var service = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService;
                        StreamReader sr = new StreamReader(file.InputStream);
                        StringBuilder sb = new StringBuilder();
                        DataTable dt = new DataTable();
                        DataRow dr;
                        string s;
                        int j = 0;

                        dt.Columns.Add("Cuenta");
                        dt.Columns.Add("Descripcion");
                        dt.Columns.Add("Razonsocial");
                        dt.Columns.Add("Nif");
                        dt.Columns.Add("TipoNif");
                        dt.Columns.Add(iso);                        
                        
                        while (!sr.EndOfStream)
                        {
                            while ((s = sr.ReadLine()) != null)
                            {            
                                //Ignorar cabecera                    
                                if (j > 0 || !model.Cabecera)
                                {
                                    string[] str = s.Split(delimitador);
                                    dr = dt.NewRow();

                                    for (int i = 0; i < dt.Columns.Count; i++)
                                    {
                                        try { 
                                            dr[dt.Columns[i]] = str[i].Replace("\"", string.Empty).ToString();
                                        }
                                        catch (Exception ex)
                                        {                                            
                                            ModelState.AddModelError("File", General.ErrorDelimitadorFormato);                                            
                                            return View("AsistenteCuentas", model);
                                        }
                                    }
                                    dt.Rows.Add(dr);
                                }

                                j++;
                            }
                        }
                        try
                        {
                            idPeticion = service.CrearPeticionImportacion(ContextService);
                            HostingEnvironment.QueueBackgroundWorkItem(async token => await GetAsync(dt, idPeticion, token));
                            //service.Importar(dt, ContextService);                            
                        }
                        catch (ValidationException ex)
                        {                                                       
                            if (string.IsNullOrEmpty(ex.Message))
                            {
                                TempData["Errors"] = null;
                            }
                            else
                            {
                                TempData["Errors"] = ex.Message;
                            }
                        }
                        sr.Close();

                        //TempData["Success"] = "Importado correctamente!";
                        TempData["Success"] = "Ejecutando, proceso con id = " + idPeticion.ToString() + ", para comprobar su ejecución ir al menú de peticiones asíncronas";
                        return RedirectToAction("AsistenteCuentas", "Cuentas");
                    }
                    else
                    {
                        ModelState.AddModelError("File", General.ErrorFormatoFichero);                        
                    }
                }
                else
                {
                    ModelState.AddModelError("File", General.ErrorFichero);
                }
            }

            return View("AsistenteCuentas", model);
        }

        private async Task GetAsync(DataTable dt, int idPeticion, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var service = FService.Instance.GetService(typeof(CuentasModel), ContextService) as CuentasService)
                {
                    await Task.Run(() => service.Importar(dt, idPeticion, ContextService));
                    return;
                }

            }
            catch (TaskCanceledException tce)
            {

            }
            catch (Exception ex)
            {
                using (var service = FService.Instance.GetService(typeof(PeticionesAsincronasModel), ContextService) as PeticionesAsincronasService)
                {
                    service.CambiarEstado(EstadoPeticion.Error, idPeticion, ex.Message);
                }
            }
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