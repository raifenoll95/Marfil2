using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Web;

namespace Marfil.App.WebMain.Misc.DataAnnotations
{
    public class CustomDisplayNameAttribute : DisplayNameAttribute
    {
        public CustomDisplayNameAttribute(string resourceId)
            : base(GetMessageFromResource(resourceId))
        {

        }

        private static string GetMessageFromResource(string resourceId)
        {
            return Unobtrusive.ResourceManager.GetString(resourceId);
        }
    }
}