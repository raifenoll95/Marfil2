using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Inf.Genericos
{
    public static class StringExtension
    {
        public static string FirstToUpper(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;
            return input.First().ToString().ToUpper() + input.Substring(1);
        }
    }
}
