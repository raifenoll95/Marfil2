using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Inf.Genericos;
using Newtonsoft.Json;
using Resources;

namespace Marfil.App.WebMain.Controllers
{


    [Authorize]
    public abstract class GenericController<T> : BaseController where T : class
    {
        #region Properties

        public string Empresa
        {
            get
            {
                
                return ContextService != null ? ContextService.Empresa : string.Empty;
            }
        }

        #endregion

        #region CTR

        public GenericController(IContextService context):base(context)
        {
            
        }

        #endregion

        #region Listado ContextService.

        private void ClearSessionColumns(ListIndexModel Model)
        {
            var vector =
                Model.Properties.Where(
                    f =>
                        f.property.PropertyType == typeof(string));

            foreach (var item in vector)
            {
                Session[item.property.Name + "Filter"] = null;
            }



        }

        public virtual ActionResult Index()
        {
            var modelview = Helper.fModel.GetModel<T>(ContextService);
            using (var gestionService = createService(modelview as IModelView))
            {
                var model = gestionService.GetListIndexModel(typeof(T), CanEliminar, CanModificar, ControllerContext.RouteData.Values["controller"].ToString());
                model.Toolbar = GenerateToolbar(gestionService,TipoOperacion.Index,model);
                Session[model.VarSessionName] = model;
                ClearSessionColumns(model);
                return View(model);
            }
        }

        #endregion

        #region Details

        // GET: Paises/Details/5
        public virtual ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var modelview = Helper.fModel.GetModel<T>(ContextService);
            using (var gestionService = createService(modelview as IModelView))
            {
               
                var model = gestionService.get(id);
                ((IToolbar)model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Ver, model);
                if (model == null)
                {
                    return HttpNotFound();
                }
                return View(model);
            }
        }

        #endregion

        #region Create

        // GET: Paises/Create
        public virtual ActionResult Create()
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            using (var gestionService = FService.Instance.GetService(typeof (T), ContextService))
            {
                var modelview = Helper.fModel.GetModel<T>(ContextService) as IModelView;

                
                if (TempData["model"] != null)
                    modelview = TempData["model"] as IModelView;
                ((IToolbar)modelview).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, modelview);
                return View(modelview);
            }
                

        }

        // POST: Paises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual ActionResult CreateOperacion(T model)
        {
            try
            {
                var modelview = Helper.fModel.GetModel<T>(ContextService);
                using (var gestionService = createService(modelview as IModelView))
                {
                    ((IToolbar) model).Toolbar = GenerateToolbar(gestionService, TipoOperacion.Alta, model as IModelView);
                    if (ModelState.IsValid)
                    {
                        


                        gestionService.create(model as IModelView);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
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
                typeof(T).GetProperty("Context").SetValue(model,ContextService);
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Create");
            }
        }

        #endregion

        #region Edit

        // GET: Paises/Edit/5
        public virtual ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var modelview = Helper.fModel.GetModel<T>(ContextService);
            using (var gestionService = createService(modelview as IModelView))
            {

                var model = TempData["model"] ?? gestionService.get(id);
                
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
        public virtual ActionResult EditOperacion(T model)
        {
            var obj = model as IModelView;
            var objExt = model as IModelViewExtension;
            try
            {
                if (ModelState.IsValid)
                {
                    var modelview = Helper.fModel.GetModel<T>(ContextService);
                    using (var gestionService = createService(modelview as IModelView))
                    {
                        gestionService.edit(model as IModelView);
                        TempData[Constantes.VariableMensajeExito] = General.MensajeExitoOperacion;
                        return RedirectToAction("Index");
                    }
                }
                TempData["errors"] = string.Join("; ", ViewData.ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(x => x.ErrorMessage));
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = obj.GetPrimaryKey() });
            }
            catch (Exception ex)
            {
                typeof(T).GetProperty("Context").SetValue(model, ContextService);
                TempData["errors"] = ex.Message;
                TempData["model"] = model;
                return RedirectToAction("Edit", new { id = obj.GetPrimaryKey() });
            }
        }

        #endregion

        #region Delete

        // GET: Paises/Delete/5
        public virtual ActionResult Delete(string id)
        {
             if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            var modelview = Helper.fModel.GetModel<T>(ContextService);
            using (var gestionService = createService(modelview as IModelView))
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
        public virtual ActionResult DeleteConfirmed(string id)
        {
            try
            {
                var modelview = Helper.fModel.GetModel<T>(ContextService);
                using (var gestionService = createService(modelview as IModelView))
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

        #region Exists

        [Authorize]
        public virtual ActionResult Exists(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Content(JsonConvert.SerializeObject(new ExistItem() { Existe = false }), "application/json");
            }
            var modelview = Helper.fModel.GetModel<T>(ContextService);
            using (var service = createService(modelview as IModelView))
            {
                var obj = service.exists(id);
                return Content(JsonConvert.SerializeObject(new ExistItem() { Existe = obj }), "application/json");
            }
        }

        #endregion

        #region Toobar generator

        protected virtual ToolbarModel GenerateToolbar(IGestionService service,TipoOperacion operacion, dynamic model = null)
        {
            var modelDisplayName = model as ICanDisplayName;
            var modelToolbar = model as IModelView;
            return new ToolbarModel()
            {
                Operacion = operacion,
                Titulo = modelDisplayName.DisplayName,
                Acciones = GenerateActionsToolbar(service,operacion, modelToolbar)
            };
        }

      

        private IEnumerable<IToolbaritem> GenerateActionsToolbar(IGestionService service, TipoOperacion operacion, IModelView model = null)
        {
            var result = new List<IToolbaritem>();
            switch (operacion)
            {
                case TipoOperacion.Index:
                    result=  IndexToolbar().ToList();
                    break;
                case TipoOperacion.Alta:
                    result = CreateToolbar(service,model).ToList();
                    break;
                case TipoOperacion.Baja:
                    result = DeleteToolbar(service, model).ToList();
                    break;
                case TipoOperacion.Custom:
                    result = CustomToolbar(service, model).ToList();
                    break;
                case TipoOperacion.Editar:
                    result = EditToolbar(service, model).ToList();
                    break;
                case TipoOperacion.Ver:
                    result = VerToolbar(service, model).ToList();
                    break;
            }

            result.InsertRange(0, HelpItem());

            return result;
        }

        protected virtual IEnumerable<IToolbaritem> IndexToolbar()
        {
            var result = new List<IToolbaritem>();
            if (CanCrear)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-plus",
                    Texto = General.BtnNuevoRegistro,
                    Url = Url.Action("Create")
                });
            }

            return result;
        }

        protected virtual IEnumerable<IToolbaritem> VerToolbar(IGestionService service,IModelView model)
        {
            var result = new List<IToolbaritem>();
            
            //Guardar
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-plus",
                Texto = General.BtnNuevoRegistro,
                OcultarTextoSiempre = true,
                Url = Url.Action("Create"),
                Desactivado = !CanCrear
            });

            result.Add(new ToolbarSeparatorModel());

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-pencil",
                Texto = General.LblEditar,
                OcultarTextoSiempre = true,
                Url = Url.Action("Edit", new { id = model.GetPrimaryKey() }),
                Desactivado = !CanModificar
            });
            result.Add(new ToolbarSeparatorModel());
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-trash red",
                Texto = General.LblBorrar,
                OcultarTextoSiempre = true,
                Url = Url.Action("Delete", new { id = model.GetPrimaryKey() }),
                Desactivado = !CanEliminar

            });

            result.Add(new ToolbarSeparatorModel());

            //Navegador
            result.AddRange(GenerarNavegadorRegistros(service,TipoOperacion.Ver,model));

            return result;
        }

        protected virtual IEnumerable<IToolbaritem> CreateToolbar(IGestionService service, IModelView model)
        {
            var result = new List<IToolbaritem>();

            //Guardar
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-check green",
                Texto = General.LblCrear,
                OcultarTextoSiempre = true,
                Url = "javascript:$(\"#mainform\").submit()"
            });

            result.Add(new ToolbarSeparatorModel());
            //Navegador
            result.AddRange(GenerarNavegadorRegistros(service,TipoOperacion.Alta,model));

            return result;
        }

        protected virtual IEnumerable<IToolbaritem> EditToolbar(IGestionService service, IModelView model)
        {
            var result = new List<IToolbaritem>();
            
            //Guardar
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-check green",
                Texto = General.BtnGuardar,
                OcultarTextoSiempre = true,
                Url = "javascript:$(\"#mainform\").submit()"
            });

            result.Add(new ToolbarSeparatorModel());
            
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-plus",
                Texto = General.BtnNuevoRegistro,
                OcultarTextoSiempre = true,
                Url = Url.Action("Create"),
                Desactivado = !CanCrear
            });

            result.Add(new ToolbarSeparatorModel());

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-trash red",
                Texto = General.LblBorrar,
                OcultarTextoSiempre = true,
                Url = Url.Action("Delete", new { id = model.GetPrimaryKey() }),
                Desactivado = !CanEliminar
            });
            result.Add(new ToolbarSeparatorModel());
            //Navegador
            result.AddRange(GenerarNavegadorRegistros(service, TipoOperacion.Editar,model));

            return result;
        }

        protected virtual IEnumerable<IToolbaritem> DeleteToolbar(IGestionService service, IModelView model)
        {
            var result = new List<IToolbaritem>();
           
            //Guardar
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-plus",
                Texto = General.BtnNuevoRegistro,
                OcultarTextoSiempre = true,
                Url = Url.Action("Create"),
                Desactivado = !CanCrear
            });

            result.Add(new ToolbarSeparatorModel());

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-pencil",
                Texto = General.LblEditar,
                OcultarTextoSiempre = true,
                Url = Url.Action("Edit", new { id = model.GetPrimaryKey() }),
                Desactivado = !CanModificar
            });

            result.Add(new ToolbarSeparatorModel());

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-eye",
                Texto = General.LblVer,
                OcultarTextoSiempre = true,
                Url = Url.Action("Details", new { id = model.GetPrimaryKey() }),
                Desactivado = !IsActivado

            });

            return result;
        }

        protected virtual IEnumerable<IToolbaritem> CustomToolbar(IGestionService service, IModelView model)
        {
            return new List<IToolbaritem>();
        }

        protected virtual IEnumerable<IToolbaritem> GenerarNavegadorRegistros(IGestionService service,TipoOperacion operacion, IModelView model)
        {
            var result = new List<IToolbaritem>();
            var actionView = operacion==TipoOperacion.Editar && CanModificar ? "Edit" : "Details";

            var firstId = service.FirstRegister();
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-step-backward",
                Texto = General.LblPrimero,
                Url = string.IsNullOrEmpty(firstId)? string.Empty: Url.Action(actionView,new {id= firstId }),
                Desactivado = model.GetPrimaryKey() == firstId || string.IsNullOrEmpty(firstId),
                OcultarTextoSiempre = true
            });

            var prevId = operacion ==TipoOperacion.Editar ||operacion== TipoOperacion.Ver? service.PreviousRegister(model.GetPrimaryKey()): string.Empty;
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-caret-left",
                Texto = General.LblAnterior,
                Url =  Url.Action(actionView,new {id= prevId }),
                Desactivado = operacion == TipoOperacion.Alta  || string.IsNullOrEmpty(prevId),
                OcultarTextoSiempre = true
            });

            var nextId = operacion == TipoOperacion.Editar || operacion == TipoOperacion.Ver ? service.NextRegister(model.GetPrimaryKey()) : string.Empty;
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-caret-right",
                Texto = General.LblSiguiente,
                Url =  Url.Action(actionView, new { id = nextId }),
                Desactivado = operacion == TipoOperacion.Alta || string.IsNullOrEmpty(nextId),
                OcultarTextoSiempre = true
            });

            var lastId = service.LastRegister();
            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-step-forward",
                Texto = General.LblUltimo,
                Url = string.IsNullOrEmpty(lastId) ? string.Empty : Url.Action(actionView, new { id = lastId }),
                Desactivado= model.GetPrimaryKey()==lastId || string.IsNullOrEmpty(lastId),
                OcultarTextoSiempre = true
            });

            return result;
        }
       

        #endregion

        #region Helpers

        protected virtual IGestionService createService(IModelView model)
        {
            var fService = FService.Instance;
            var modelservice = fService.GetService(typeof(T), ContextService);
            if (modelservice != null)
                return modelservice;

            var modelex = model as IModelViewExtension;
            var genericType = typeof(GestionService<,>);
            Type[] typeArgs = { typeof(T), modelex.persistencyType };
            var repositoryType = genericType.MakeGenericType(typeArgs);

            return Activator.CreateInstance(repositoryType) as IGestionService;
        }

        #endregion
    }
}
