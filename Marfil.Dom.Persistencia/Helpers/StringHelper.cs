using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Marfil.Dom.Persistencia.Helpers
{
    public static class StringHelper
    {
        public static bool IsValidJson(this string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }
            var value = stringValue.Trim();
            try
            {
                var obj = JObject.Parse(value);
                return true;
            }
            catch (JsonReaderException)
            {
                return false;
            }
            return false;
        }
    }
}