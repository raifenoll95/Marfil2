using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Marfil.App.WebMain.Misc.DataAnnotations
{
    public class CustomRequiredAttributeAdapter : RequiredAttributeAdapter
    {
        public CustomRequiredAttributeAdapter(
            ModelMetadata metadata,
            ControllerContext context,
            RequiredAttribute attribute
        ) : base(metadata, context, attribute)
        {
            attribute.ErrorMessageResourceType = typeof(Unobtrusive);
            attribute.ErrorMessageResourceName = "PropertyValueRequired";
        }
    }
}