using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Resources;

namespace Marfil.App.WebMain.Misc
{
    public abstract class ApiBaseController : ApiController
    {
        #region properties
      
        public  IContextService ContextService { get; }
        public ApplicationHelper appService { get; }
        #endregion

        public ApiBaseController(IContextService context)
        {
            ContextService = context;
            appService = new ApplicationHelper(context);
        }

   
    }
}