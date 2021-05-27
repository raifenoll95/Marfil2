using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Marfil.Dom.Persistencia.Helpers;

namespace Marfil.App.WebMain.Misc
{
    
    public class CustomModelBinder : DefaultModelBinder
    {
        protected override object CreateModel(ControllerContext controllerContext,
        ModelBindingContext bindingContext, Type modelType)
        {
            HttpRequestBase request = controllerContext.HttpContext.Request;
            string clase = request.Form.Get("cpClase");
            var tipo = Helper.GetTypeFromFullName(clase);


           
            var obj = Activator.CreateInstance(tipo);

            foreach (var p in tipo.GetProperties())
            {
                var valor = request.Form.Get(p.Name);
                if(!string.IsNullOrEmpty(valor))
                {
                    valor = valor.Trim('\"');
                    if (p.PropertyType == typeof(int))
                    {
                        tipo.GetProperty(p.Name).SetValue(obj, int.Parse(valor));
                    }
                    else if (p.PropertyType == typeof(bool))
                    {
                        valor = string.IsNullOrEmpty(valor) ? "false" : valor;
                        tipo.GetProperty(p.Name).SetValue(obj, bool.Parse(valor));
                    }
                    else
                        tipo.GetProperty(p.Name).SetValue(obj, valor);
                }
                
            }

            return obj;
        }
    }
}