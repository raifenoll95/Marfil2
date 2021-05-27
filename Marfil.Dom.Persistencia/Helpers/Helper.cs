using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Marfil.Dom.Persistencia.Model;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.Helpers
{
    public static class Helper
    {

        private static FModel _fmodel;

        public static FModel fModel
        {
            get
            {
                if (_fmodel == null)
                {
                    _fmodel=new FModel();
                }

                return _fmodel;
            }
        }

        public static Type GetTypeFromFullName(string clase)
        {

            Type type = Type.GetType(clase);
            if (type != null)
                return type;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies().Where(f => f.FullName.Contains("Marfil")))
            {
                type = asm.GetType(clase);
                if (type != null)
                    return type;
            }
            return null;
        }

        
        public static IEnumerable<ViewProperty> getProperties<T>()
        {
            var listNames= typeof(T).GetProperties().Select(f=>f.Name).Except(typeof(IModelView).GetProperties().Select(h=>h.Name)).Except(typeof(ICanDisplayName).GetProperties().Select(f=>f.Name)).Except(typeof(IModelViewExtension).GetProperties().Select(h=>h.Name));
            var properties = typeof (T).GetProperties().Where(f => listNames.Any(h=> h == f.Name));

            return properties.Select(item => new ViewProperty
            {
                property = item, attributes = item.GetCustomAttributes(true)
            }).ToList();
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

    }
}
