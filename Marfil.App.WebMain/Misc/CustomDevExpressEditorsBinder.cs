using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;

namespace Marfil.App.WebMain.Misc
{
    public class CustomDevExpressEditorsBinder: DevExpressEditorsBinder
    {
        protected override object CreateModel(ControllerContext controllerContext,
        ModelBindingContext bindingContext, Type modelType)
        {
            var obj= base.CreateModel(controllerContext, bindingContext, modelType);

            
            var list = modelType.GetProperties().Where(f => f.PropertyType == typeof (DateTime)).ToList();
            if(list.Any())
            {
                foreach (var item in list)
                {
                    if (item.PropertyType == typeof (DateTime))
                    {
                        var valorFecha = controllerContext.HttpContext.Request.Form["cp" + item.Name];
                        if (!string.IsNullOrEmpty(valorFecha))
                        {
                            modelType.GetProperty(item.Name).SetValue(obj,DateTime.Parse(valorFecha));
                        }
                    }
                }
            }


            return obj;
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext,
            PropertyDescriptor propertyDescriptor)
        {
            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}