using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Marfil.Inf.Genericos.Helper
{
    public static class ModeloNegocioFunciones
    {
        public static bool ValidateIban(string iban)
        {
            var reg = new Regex("[a-zA-Z]{2}[0-9]{2}[a-zA-Z0-9]{4}[0-9]{7}([a-zA-Z0-9]?){0,16}");
            return reg.IsMatch(iban);
        }

        public static bool ValidateBic(string bic)
        {
            var reg = new Regex("^([a-zA-Z]){4}([a-zA-Z]){2}([0-9a-zA-Z]){2}([0-9a-zA-Z]{3})?$");
            return reg.IsMatch(bic);
        }

        public static bool ValidateIdAcreedor(string idacreedor)
        {
            var cidrev = idacreedor.Substring(7) + idacreedor.Substring(0, 4);
            return TxtMod97(ReplaceChars(cidrev)) == 1;
        }

        public static double CalculaEquivalentePeso(bool essuperficie,double metrosm, double espesorm, double equivalente = 2,bool calcularequivalentepeso =true)
        {
            if (!essuperficie)
                return metrosm;

            var espesor = espesorm*100.0;//lo convertimos en cm
            var metros = calcularequivalentepeso
                ? metrosm*(espesor/equivalente)
                : 1/(Math.Round(100/(1 + espesor)/33.333333, 3))*metrosm;

            return equivalente == espesor ? metrosm : metros;
        }

        /**
  * Replace letters with numbers using the SEPA scheme A=10, B=11, ...
  * Non-alphanumerical characters are dropped.
  *
  * @param str     The alphanumerical input string
  * @return        The input string with letters replaced
  */
        private static  string ReplaceChars(string str)
        {
            var res = "";
            var vector = str.ToCharArray();
            for (var i = 0; i < vector.Length; ++i)
            {
                var cc = vector[i];
                if (cc >= 65 && cc <= 90)
                {
                    res += (cc - 55).ToString();
                }
                else if (cc >= 97 && cc <= 122)
                {
                    res += (cc - 87).ToString();
                }
                else if (cc >= 48 && cc <= 57)
                {
                    res += str[i];
                }
            }
            return res;
        }

        /**
         * mod97 function for large numbers
         *
         * @param str     The number as a string.
         * @return        The number mod 97.
         */
        private static int TxtMod97(string str)
        {
            var res = 0;
            var vector = str.ToCharArray();
            for (var i = 0; i < vector.Length; ++i)
            {
                res = (res * 10 + int.Parse(str[i].ToString())) % 97;
            }
            return res;
        }
    }
}
