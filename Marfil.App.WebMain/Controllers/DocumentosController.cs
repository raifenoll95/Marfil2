using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Diseñador;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Documentos;
using Marfil.Dom.Persistencia.ServicesView.Servicios.Preferencias;
using Marfil.Inf.Genericos.Helper;
using Resources;

namespace Marfil.App.WebMain.Controllers
{
    public class DocumentosController : Controller
    {
        protected IContextService ContextService { get; set; }
        public DocumentosController(IContextService context)
        {
            ContextService = context;
        }
        // GET: Documentos
        public ActionResult Index(string id)
        {
            
            using (var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
            using (var service = new DocumentosUsuarioService(db))
            {
                var model = new DocumentosWrapperModel();
                var tipoDocumento = (TipoDocumentoImpresion) Enum.Parse(typeof (TipoDocumentoImpresion), id);

                model.Titulo = string.Format(General.ConfiguradorDocumentos,Funciones.GetEnumByStringValueAttribute(tipoDocumento));
                model.Tipo = tipoDocumento;
                model.Lineas = service.GetDocumentos(tipoDocumento, ContextService.Id);
                ((IToolbar)model).Toolbar= new ToolbarModel()
                {
                    Titulo = model.Titulo,
                    Operacion = TipoOperacion.Index,
                    Acciones= IndexToolbar(tipoDocumento, ContextService.Id)
                };
                Session[id] = model;
                return View(model);
            }   
        }

        protected virtual IEnumerable<IToolbaritem> IndexToolbar(TipoDocumentoImpresion tipo,Guid usuario)
        {
            var result = new List<IToolbaritem>();
            
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-plus",
                    Texto = General.BtnNuevoRegistro,
                    Url = Url.Action("Index","Designer",new { nuevo=true,reportId= DocumentosUsuarioService.CreateCustomId(tipo,usuario,"Nuevo"),returnUrl=Url.Action("Index","Documentos",new {id=(int)tipo}) })
                });

            return result;
        }

        public ActionResult Delete(string id)
        {
            if (TempData["errors"] != null)
                ModelState.AddModelError("", TempData["errors"].ToString());
           
            using (var db = MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
            using (var service = new DocumentosUsuarioService(db))
            {
                TipoDocumentoImpresion tipoDocumento;
                Guid usuario;
                string name;
                DocumentosUsuarioService.GetFromCustomId(id, out tipoDocumento, out usuario, out name);

                ViewBag.TituloMantenimiento = string.Format(General.ConfiguradorDocumentos, Funciones.GetEnumByStringValueAttribute(tipoDocumento));
                var model = new DocumentosDeleteModel()
                {
                    CustomId = id,
                    Nombre = name,
                    Tipo = tipoDocumento,
                    Titulo = string.Format(General.ConfiguradorDocumentos, Funciones.GetEnumByStringValueAttribute(tipoDocumento)),
                    Toolbar = new ToolbarModel()
                    {
                        Titulo = string.Format(General.ConfiguradorDocumentos, Funciones.GetEnumByStringValueAttribute(tipoDocumento)),
                        Operacion = TipoOperacion.Baja,
                        Acciones = new IToolbaritem[] {}
                        
                    }

            };
                
                return View(model);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual ActionResult DeleteConfirmed(DocumentosModel model)
        {
            try
            {
               
                using(var db= MarfilEntities.ConnectToSqlServer(ContextService.BaseDatos))
                using (var service = new DocumentosUsuarioService(db))
                {
                    TipoDocumentoImpresion tipoDocumento;
                    Guid usuario;
                    string name;
                    DocumentosUsuarioService.GetFromCustomId(model.CustomId, out tipoDocumento,out usuario,out name);
                    ViewBag.TituloMantenimiento = string.Format(General.ConfiguradorDocumentos, Funciones.GetEnumByStringValueAttribute(tipoDocumento));
                    service.RemovePreferencia(tipoDocumento,usuario,name);
                    
                    return RedirectToAction("Index",new {id=(int)tipoDocumento});
                }
                    
            }
            catch (Exception ex)
            {
               
                TempData["errors"] = ex.Message;
            }

              return RedirectToAction("Delete", new { id = model.CustomId});
        }

        [ValidateInput(false)]
        public ActionResult _listDocumentos(string id)
        {
            return PartialView("_listDocumentos", Session[id] as DocumentosWrapperModel);
        }
    }
}