using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.App.WebMain.Misc
{
    public abstract class BaseController : Controller, IMenuAuthorization
    {
        #region properties

        public abstract string MenuName { get; set; }
        public abstract bool IsActivado { get; set; }
        public abstract bool CanCrear { get; set; }
        public abstract bool CanModificar { get; set; }
        public abstract bool CanEliminar { get; set; }
        public  bool CanBloquear { get; set; }
        public  IContextService ContextService { get; }
        public ApplicationHelper appService { get; }
        #endregion

        public BaseController(IContextService context)
        {
            ContextService = context;
            appService= new ApplicationHelper(context);
        }

        protected override void OnActionExecuting(ActionExecutingContext ctx)
        {
            CargarParametros();

            if (!IsActivado)
            {
                ctx.Result = new RedirectToRouteResult(new RouteValueDictionary { {"controller","Home"} , { "action", "Index" } });
                return;
            }

            if (ctx.ActionDescriptor.ActionName == "Edit" && !CanModificar)
            {
                ctx.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                return;
            }

            if (ctx.ActionDescriptor.ActionName == "Delete" && !CanEliminar)
            {
                ctx.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                return;
            }

            if (ctx.ActionDescriptor.ActionName == "Create" && !CanCrear)
            {
                ctx.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                return;
            }

            if (ctx.ActionDescriptor.ActionName == "Bloquear" && !CanBloquear)
            {
                ctx.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
                return;
            }
            base.OnActionExecuting(ctx);
        }

        protected override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            ViewBag.MenuName = MenuName;
            ViewBag.Crear = CanCrear;
            ViewBag.Modificar = CanModificar;
            ViewBag.Eliminar = CanEliminar;
            ViewBag.Bloquear = CanBloquear;
            base.OnResultExecuting(filterContext);
        }
       
        protected abstract void CargarParametros();

        protected override void OnException(ExceptionContext filterContext)
        {
#if DEBUG
            WebHelper.Log.AddLog(filterContext.Exception);
#endif
            base.OnException(filterContext);



        }

        public IEnumerable<IToolbaritem> HelpItem()
        {
            var ayudaModel = appService.GetAyudaModel();
            var action = ControllerContext.RouteData.Values["action"].ToString();
            var controller = ControllerContext.RouteData.Values["controller"].ToString();
            var item = ayudaModel.List.FirstOrDefault(f => f.Controller?.ToLower() == controller?.ToLower() && f.Action?.ToLower() == action?.ToLower());
            if (item == null)
                item = ayudaModel.List.FirstOrDefault(f => f.Controller?.ToLower() == controller?.ToLower() && string.IsNullOrEmpty(f.Action));

            var result = new List<IToolbaritem>();
            if (item != null)
            {
                result.Add(new ToolbarActionModel()
                {
                    Icono = "fa fa-question-circle fa-1-5x green",
                    OcultarTextoSiempre = true,
                    Target = "_blank",
                    Url = item.Url,
                    Texto = General.LblAyuda
                });
                result.Add(new ToolbarSeparatorModel());
            }


            return result;

        }
    }
}