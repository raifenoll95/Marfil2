using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Security;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Configuracion.Empresa;
using Marfil.Dom.Persistencia.Model.Configuracion.Planesgenerales;
using Marfil.Dom.Persistencia.Model.Documentos.Presupuestos;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;
using System.IO;
using System.Data;
using System.Text;
using Marfil.Dom.Persistencia.Model.Configuracion.Cuentas;
using Marfil.Dom.ControlsUI.NifCif;

namespace Marfil.App.WebMain.Controllers
{
    public class EmpresasController:GenericController<EmpresaModel>
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

        public EmpresasController(IContextService context) : base(context)
        {

        }

        #endregion

        #region Create

        public override ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var modelview = Helper.fModel.GetModel<EmpresaModel>(ContextService);

            var aux = TempData["model"] == null ? modelview : TempData["model"] as EmpresaModel;
            aux.Paises = modelview.Paises;
            aux.PlanesGenerales = modelview.PlanesGenerales;
            aux.LstTarifasVentas = modelview.LstTarifasVentas;
            aux.LstTarifasCompras = modelview.LstTarifasCompras;
            using (var gestionService = FService.Instance.GetService(typeof(EmpresaModel), ContextService))
            {
                ((IToolbar)aux).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, aux);
            }
            return View(aux);
        }

        //create operacion empresas
        public override ActionResult CreateOperacion(EmpresaModel model)
        {
            try
            {
                using (var gestionService = createService(model) as EmpresasService)
                {
                    ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model);
                    if (ModelState.IsValid)
                    {
                        gestionService.create(model);
                        HostingEnvironment.QueueBackgroundWorkItem(async token => await generarPlanContabilidad(model, token));
                        TempData["Success"] = General.MensajeExitoOperacion;
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

        //Plan de cuentas en segundo plano
        public async Task generarPlanContabilidad(EmpresaModel model, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                var service = new PlanesGeneralesService(ContextService);
                var item = service.get(model.Fkplangeneralcontable) as PlanesGeneralesModel;
                var csvFile = ContextService.ServerMapPath(item.Fichero);

                using (var reader = new StreamReader(csvFile, Encoding.Default, true))
                {
                    var contenido = reader.ReadToEnd();
                    await Task.Run(() => CrearModels(contenido, model));
                }
            }
            catch (TaskCanceledException tce)
            {
            }
            catch (Exception ex)
            {
            }
        }

        public void CrearModels(string xml, EmpresaModel model)
        {
            var lineas = xml.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var item in lineas)
            {
                if (!string.IsNullOrEmpty(item))
                    CrearModel(item, model);
            }

        }

        public void CrearModel(string linea, EmpresaModel empresa)
        {
            var newContext = new ContextLogin()
            {
                BaseDatos = ContextService.BaseDatos,
                Empresa = empresa.Id,
                Id = ContextService.Id,
                RoleId = ContextService.RoleId
            };

            var vector = linea.Split(';');
            var model = new CuentasModel()
            {
                Empresa = empresa.Id,
                Id = vector[0],
                Descripcion2 = vector[1],
                Descripcion = vector[2],
                Nivel = int.Parse(vector[3]),
                FkPais = empresa.Fkpais,
                UsuarioId = Guid.Empty.ToString(),
                Nif = new NifCifModel(),
                Fechaalta = DateTime.Now,
                FechaModificacion = DateTime.Now
            };

            var service = new CuentasService(newContext);

            service.create(model);
        }

        public async Task GenerarContabilidadAsync(EmpresaModel model, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var service = FService.Instance.GetService(typeof(EmpresaModel), ContextService) as EmpresasService)
            {
                await Task.Run(() => service.CrearPlanGeneral(model.Id, model.Fkplangeneralcontable, model.Fkpais));
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

            var modelview = Helper.fModel.GetModel<EmpresaModel>(ContextService);
            using (var gestionService = createService(modelview as IModelView))
            {
                EmpresaModel model;
                if (TempData["model"] != null)
                {
                    model = TempData["model"] as EmpresaModel;
                    Session["_empresa_" + id] = null;
                }
                else
                    model = gestionService.get(id) as EmpresaModel;

                if (model == null)
                {
                    return HttpNotFound();
                }

                model.Paises = modelview.Paises;
                model.PlanesGenerales = modelview.PlanesGenerales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Editar, model);
                
                
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public override ActionResult EditOperacion(EmpresaModel model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    using (var gestionService = createService(model))
                    {
                        gestionService.edit(model);
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

        #region Details

        public override ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var modelview = Helper.fModel.GetModel<EmpresaModel>(ContextService);
            using (var gestionService = createService(modelview as IModelView))
            {
                
                var model = gestionService.get(id) as EmpresaModel;
                if (model == null)
                {
                    return HttpNotFound();
                }

                model.Paises = modelview.Paises;
                model.PlanesGenerales = modelview.PlanesGenerales;
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);

                return View(model);
            }
        }

        #endregion

        #region delete

        public override ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            using (var gestionService = FService.Instance.GetService(typeof(EmpresaModel), ContextService) as EmpresasService)
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

                var newmodel = Helper.fModel.GetModel<EmpresaModel>(ContextService);
                using (var gestionService = FService.Instance.GetService(typeof(EmpresaModel), ContextService) as EmpresasService)
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

    }
}