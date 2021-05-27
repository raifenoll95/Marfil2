using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Marfil.App.WebMain.Misc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Listados;
using Marfil.Dom.Persistencia.Listados.Base;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Graficaslistados;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Newtonsoft.Json;
using Resources;
using Rconfiguraciongraficas = Marfil.Inf.ResourcesGlobalization.Textos.Entidades.Configuraciongraficas;
namespace Marfil.App.WebMain.Controllers
{
    public class ConfiguraciongraficasController : BaseController
    {
        public override string MenuName { get; set; }
        public override bool IsActivado { get; set; }
        public override bool CanCrear { get; set; }
        public override bool CanModificar { get; set; }
        public override bool CanEliminar { get; set; }

        protected override void CargarParametros()
        {
            MenuName = "configuraciongraficas";
            IsActivado = true;
            CanCrear = true;
            CanModificar = true;
            CanEliminar = true;
        }

        public ConfiguraciongraficasController(IContextService context) : base(context)
        {
        }

        // GET: Configuraciongraficas
        public ActionResult Index(string id)
        {
            

            using (var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
            using (var service = FService.Instance.GetService(typeof(ConfiguraciongraficasModel), ContextService, db) as ConfiguraciongraficasService)
            {
                IListados listadoModel = null;
                if (id != null)
                    listadoModel = FListadosModel.Instance.GetModel(ContextService,id, ContextService.Empresa, ContextService.Ejercicio);
                var model = new ConfiguraciongraficasWrapperModel();
                model.Idlistado = id;
                if (id != null)
                    model.Titulo = string.Format("{0}: {1}", Rconfiguraciongraficas.TituloEntidad, listadoModel.TituloListado);
                model.Lineas = service.GetConfiguracionesListado(id, ContextService.Id);
                
                if (id != null)
                {
                    ((IToolbar)model).Toolbar = new ToolbarModel()
                    {
                        Titulo = model.Titulo,
                        Operacion = TipoOperacion.Index,
                        Acciones = IndexToolbar(model.Idlistado),
                        CustomAction = true,
                        CustomActionName = Url.Action("Index", new { id = "" })
                    };
                }
                else
                {
                    ((IToolbar)model).Toolbar = new ToolbarModel()
                    {
                        Titulo = General.LblGestionDeGraficas + ContextService.Usuario,
                        Operacion = TipoOperacion.Index,
                        Acciones = IndexToolbar(model.Idlistado),
                        CustomAction = true,
                        CustomActionName = Url.Action("Index", new { id = "" })
                    };
                }
                Session[id] = model;
                return View(model);
            }
        }

        protected virtual IEnumerable<IToolbaritem> IndexToolbar(string idlistado)
        {
            var result = new List<IToolbaritem>();

            result.Add(new ToolbarActionModel()
            {
                Icono = "fa fa-plus",
                Texto = General.BtnNuevoRegistro,
                Url = Url.Action("Create", "Configuraciongraficas", new { id = idlistado, returnUrl = Url.Action("Index", "Configuraciongraficas", new { id = idlistado }) })                  
            });
            result.InsertRange(0, HelpItem());
            return result;
        }

        #region Create

        public ActionResult Create(string id)
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            
            using (var service = FService.Instance.GetService(typeof (ConfiguraciongraficasModel), ContextService) as ConfiguraciongraficasService)
            {
                var listadoModel =TempData["configuracion"] as ListadosModel ?? FListadosModel.Instance.GetModel(ContextService,id, ContextService.Empresa, ContextService.Ejercicio) as ListadosModel;
                string.Format("{0}: {1}", Rconfiguraciongraficas.TituloEntidad, listadoModel.TituloListado);
                var model = service.CrearNuevoModel(listadoModel, ContextService.Ejercicio);
                model.ListadoModel = listadoModel;
                model.Toolbar = new ToolbarModel()
                {
                    Titulo = string.Format("{0}: {1}", Rconfiguraciongraficas.TituloEntidad, listadoModel.TituloListado),
                    Operacion = TipoOperacion.Alta,
                    Acciones = new IToolbaritem[]
                    {
                       
                    },
                    CustomAction = true,
                    CustomActionName = Url.Action("Index", new { id = id })

                };

                model.Toolbar.Acciones.ToList().InsertRange(0, HelpItem());
                return View(model);
            }
                
           
        }

        [HttpPost]
        public ActionResult Create(ConfiguraciongraficasModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.ListadoModel = CrearListadoModelDeParametros(model.Idlistado);

                    using (var service = FService.Instance.GetService(typeof(ConfiguraciongraficasModel), ContextService))
                    {
                        service.create(model);
                       return RedirectToAction("Index", new { id = model.Idlistado });
                    }
                }
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
            }

            return View(model);
        }

        #endregion

        #region Edit

        public ActionResult Edit(string id)
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());

            
            using (var service = FService.Instance.GetService(typeof(ConfiguraciongraficasModel), ContextService))
            {
                var result = service.get(id) as ConfiguraciongraficasModel;
                var listadoModel = result.ListadoModel;
               
                result.Toolbar = new ToolbarModel()
                {
                    Titulo = string.Format("{0}: {1}", Rconfiguraciongraficas.TituloEntidad, listadoModel.TituloListado),
                    Operacion = TipoOperacion.Editar,
                    Acciones = new IToolbaritem[] { new ToolbarActionModel()
                        {
                            Icono = "fa fa-eye",
                            Texto = "Visualizar",
                            Url = "javascript:Visualizar()"
                        },  },
                    CustomAction = true,
                    CustomActionName = Url.Action("Index", new { id = result.Idlistado })
                };
                result.Toolbar.Acciones.ToList().InsertRange(0, HelpItem());
                return View(result);
            }
        }

        [HttpPost]
        public ActionResult Edit(ConfiguraciongraficasModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    model.ListadoModel = CrearListadoModelDeParametros(model.Idlistado);
                    using (var service = FService.Instance.GetService(typeof(ConfiguraciongraficasModel), ContextService))
                    {
                        service.edit(model);
                       return  RedirectToAction("Index", new { id = model.Idlistado });
                    }
                }
            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
            }
            
                

            return View(model);
        }

        #endregion

        #region Delete

        public ActionResult Delete(string id)
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
            

            using (var service = FService.Instance.GetService(typeof(ConfiguraciongraficasModel), ContextService))
            {
                var result = service.get(id) as ConfiguraciongraficasModel;
                var listadoModel = result.ListadoModel;
                string.Format("{0}: {1}", Rconfiguraciongraficas.TituloEntidad, listadoModel.TituloListado);
                
                result.Toolbar = new ToolbarModel()
                {
                    Titulo = string.Format("{0}: {1}", Rconfiguraciongraficas.TituloEntidad, listadoModel.TituloListado),
                    Operacion = TipoOperacion.Baja,
                    Acciones = new IToolbaritem[] { },
                    CustomAction = true,
                    CustomActionName = Url.Action("Index", new { id = result.Idlistado })

                };
                result.Toolbar.Acciones.ToList().InsertRange(0, HelpItem());
                return View(result);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(ConfiguraciongraficasModel model)
        {
            try
            {
                
                using (var service = FService.Instance.GetService(typeof(ConfiguraciongraficasModel), ContextService))
                {
                    service.delete(service.get(model.Codigo.ToString()));
                    return RedirectToAction("Index", new { id = model.Idlistado });
                }

            }
            catch (Exception ex)
            {
                model.Context = ContextService;
                TempData["errors"] = ex.Message;
            }

            return RedirectToAction("Delete", new { id = model.Codigo });
        }

        #endregion

        #region Ordenar

        [HttpPost]
        public ActionResult Ordenar(string model)
        {
            try
            {
                
                using (var service = FService.Instance.GetService(typeof(ConfiguraciongraficasModel), ContextService) as ConfiguraciongraficasService)
                {
                    service.ActualizarOrdenPanelControl(JsonConvert.DeserializeObject<IEnumerable<StOrdenPanelControl>>(model));
                }
            }
            catch (Exception ex)
            {
                return Json("{\"error\": \"" + ex.Message + "\"}");
            }

            return new EmptyResult();
        }

        #endregion

        [ValidateInput(false)]
        public ActionResult _listConfiguraciongraficas(string id)
        {
            return PartialView("_listConfiguraciongraficas", Session[id] as ConfiguraciongraficasWrapperModel);
        }

        #region Helpers


        private IListados CrearListadoModelDeParametros(string id)
        {
            
            var result = FListadosModel.Instance.GetModel(ContextService, id, ContextService.Empresa, ContextService.Ejercicio);

            var excludedproperties = typeof (ListadosModel).GetProperties();
            var properties = result.GetType().GetProperties().Where(f=>!excludedproperties.Any(j=>j.Name==f.Name));
            foreach (var item in properties)
            {
                if (!string.IsNullOrEmpty(Request.Params[item.Name]) && item.CanWrite)
                {
                    var valor = Request.Params[item.Name];
                    if (item.PropertyType == typeof(int) || item.PropertyType == typeof(int?))
                    {
                        item.SetValue(result, int.Parse(valor));
                    }
                    if (item.PropertyType == typeof(double) || item.PropertyType == typeof(double?))
                    {
                        item.SetValue(result, double.Parse(valor));
                    }
                    else if (item.PropertyType == typeof(bool) || item.PropertyType == typeof(bool?))
                    {
                        valor = string.IsNullOrEmpty(valor) ? "false" : valor;
                        item.SetValue(result, bool.Parse(valor));
                    }
                    else if (item.PropertyType == typeof(DateTime) || item.PropertyType == typeof(DateTime?))
                    {
                        item.SetValue(result, DateTime.Parse(valor));
                    }
                    else if (item.PropertyType.IsEnum)
                    {
                        item.SetValue(result, Enum.Parse(item.PropertyType,valor));
                    }
                    else if (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
                    {
                        
                        if (item.PropertyType.GenericTypeArguments.FirstOrDefault() == typeof(int))
                        {
                            var lista = Activator.CreateInstance(item.PropertyType) as List<int>;
                            var s = valor.Split(',');
                            foreach (var i in s)
                                lista.Add(int.Parse(i));
                            item.SetValue(result, lista);
                        }
                        if (item.PropertyType.GenericTypeArguments.FirstOrDefault() == typeof(double))
                        {
                            var lista = Activator.CreateInstance(item.PropertyType) as List<double>;
                            var s = valor.Split(',');
                            foreach (var i in s)
                                lista.Add(double.Parse(i));
                            item.SetValue(result, lista);
                        }
                        if (item.PropertyType.GenericTypeArguments.FirstOrDefault() == typeof(bool))
                        {
                            var lista = Activator.CreateInstance(item.PropertyType) as List<bool>;
                            var s = valor.Split(',');
                            foreach (var i in s)
                                lista.Add(bool.Parse(i));
                            item.SetValue(result, lista);
                        }
                        else
                        {
                            var lista = Activator.CreateInstance(item.PropertyType) as List<string>;
                            var s = valor.Split(',');
                            foreach (var i in s)
                                lista.Add(i);
                            item.SetValue(result, lista);
                        }
                    }
                    else
                        item.SetValue(result, valor);
                }
                    
            }

            
            return result;
        }
        #endregion

        
    }
}