using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marfil.Inf.Genericos.Helper
{
    public static class Funciones
    {

        public static string RellenaCod(string input, int cantidad)
        {
            return input.PadLeft(cantidad, '0');
        }

        public static int? Qint(object valor)
        {
            if (valor is int)
                return (int)valor;
            int result;
            return int.TryParse(valor?.ToString(), out result) ? result : (int?)null;
        }
        // RFL movs
        public static decimal? Qdecimal(object valor)
        {
            if (valor is decimal)
                return (decimal)valor;
            decimal result;
            //   return double.TryParse(valor?.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result) ? result : (double?)null;
            return decimal.TryParse(valor?.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out result) ? result : (decimal?)null;
        }


        public static double? Qdouble(object valor)
        {
            if (valor is double)
                return (double)valor;
            double result;
         //   return double.TryParse(valor?.ToString(), NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out result) ? result : (double?)null;
            return double.TryParse(valor?.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out result) ? result : (double?)null;
        }

        public static bool Qbool(object valor)
        {
            bool result;
            if (valor == null)
                return false;
            return bool.TryParse(valor.ToString(), out result) && result;
        }

        public static string Qnull(object valor)
        {
            return valor?.ToString() ?? string.Empty;
        }

        public static string GetEnumByStringValueAttribute<TEnum>(TEnum value)
        {
            var enumType = Nullable.GetUnderlyingType(value.GetType()) ?? value.GetType();
            foreach (TEnum val in Enum.GetValues(enumType))
            {
                if (val.Equals(value))
                {
                    var fi = enumType.GetField(val.ToString());
                    var attributes = (StringValueAttribute[])fi.GetCustomAttributes(
                        typeof(StringValueAttribute), false);
                    if (attributes.Any())
                    {
                        StringValueAttribute attr = attributes[0];
                        return attr.Value;

                    }

                    return value.ToString();
                }


            }
            throw new ArgumentException("The value '" + value + "' is not supported.");
        }

        public static string GetIntEnumByStringValueAttribute<TEnum>(TEnum value)
        {
            var enumType = Nullable.GetUnderlyingType(typeof(TEnum)) ?? typeof(TEnum);
            foreach (var val in Enum.GetValues(enumType))
            {
                if (val.Equals(value))
                {
                    return ((int)val).ToString();
                }


            }
            return string.Empty;
        }

        public static string GetNextCode(string code, int length)
        {
            if (string.IsNullOrEmpty(code))
            {
                return RellenaCod("1", length);
            }
            else
            {
                var numeroCode = Qint(code);
                if (numeroCode.HasValue)
                {
                    if (numeroCode < 0)
                        numeroCode *= -1;
                    numeroCode++;
                    return RellenaCod(numeroCode.ToString(), length);
                }
            }

            throw new Exception("Caso de GetNextCode no soportado");
        }

        public static bool Qbool(string s)
        {
            bool result;
            bool.TryParse(s, out result);

            return result;
        }

        public static DateTime? Qdate(object o)
        {
            DateTime result;
            DateTime.TryParse(o?.ToString(), out result);

            return result;
        }

        public static byte[] ReadAllBytes(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            int readCount;
            using (MemoryStream ms = new MemoryStream())
            {
                while ((readCount = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, readCount);
                }
                return ms.ToArray();
            }
        }

        public static List<T> ClonarLista<T>(List<T> lista)
        {
            var result = new List<T>();
            var properties = typeof(T).GetProperties();
            foreach (var item in lista)
            {

                var newItem = Activator.CreateInstance<T>();

                foreach (var p in properties)
                {
                    if (p.CanWrite && p.CanRead)
                        p.SetValue(newItem, p.GetValue(item));
                }

                result.Add(newItem);
            }
            return result;
        }

        public static T ConverterGeneric<T>(object obj)
        {
            if (typeof(T).GetConstructor(Type.EmptyTypes) == null)
            {
                return default(T);
            }

            T result = Activator.CreateInstance<T>();

            foreach (var item in typeof(T).GetProperties())
            {
                if ((obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false) &&
                    (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.GetGenericTypeDefinition() !=
                    typeof(ICollection<>)))
                {
                    if (item.PropertyType == typeof(double) || item.PropertyType == typeof(Nullable<double>))
                    {
                        item.SetValue(result, Math.Round(Qdouble(obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null)) ?? 0, 4));
                    }
                    else
                    {
                        item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                    }                    
                }
                else if (obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsEnum ?? false)
                {
                    item.SetValue(result, (int)obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
                else if (!obj.GetType().GetProperty(item.Name.FirstToUpper())?.PropertyType.IsGenericType ?? false)
                {
                    item.SetValue(result, obj.GetType().GetProperty(item.Name.FirstToUpper())?.GetValue(obj, null));
                }
            }

            return result;
        }

    }
}
