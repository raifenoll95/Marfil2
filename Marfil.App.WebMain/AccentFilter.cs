using System;
using System.Globalization;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;

namespace Marfil.App.WebMain
{
    public class AccentFilter : ICustomFunctionOperator
    {
        public string Name => "AccentFilter";

        public object Evaluate(params object[] operands)
        {
            var value = RemoveDiacritics(operands[0] as string);
            var filter = RemoveDiacritics(operands[1] as string);
            return value.IndexOf(filter, StringComparison.CurrentCultureIgnoreCase) != -1;
        }

        public string Format(Type providerType, params string[] operands)
        {
            return string.Empty;
        }

        public Type ResultType(params Type[] operands)
        {
            return typeof(bool);
        }

        public string RemoveDiacritics(string text)
        {
            return
                string.Concat(
                    text.Normalize(NormalizationForm.FormD)
                        .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark))
                    .Normalize(NormalizationForm.FormC);
        }

        public static FunctionOperator FindAndRemoveCustomOperator(ref CriteriaOperator co, string fieldName)
        {
            var fo = co as FunctionOperator;
            if (IsRemovable(fo, fieldName))
            {
                co = null;
                return fo;
            }

            var go = co as GroupOperator;
            if (ReferenceEquals(go, null))
            {
                return null;
            }

            //fo = go.Operands.FirstOrDefault(f => IsValidFuncOperator(f as FunctionOperator, fieldName)) as FunctionOperator;
            go.Operands.RemoveAll(f => IsRemovable(f as FunctionOperator, fieldName));
            if (!go.Operands.Any())
                go = null;
            return fo;
        }
        private static bool IsValidFuncOperator(FunctionOperator fo, string fieldName)
        {
            if ((ReferenceEquals(fo, null)
                        || ((fo.OperatorType != FunctionOperatorType.Custom) && !fo.Operands.Any(f => f.ToString().Contains(fieldName)))))
            {
                return false;
            }

            return true;
        }

        private static bool IsRemovable(FunctionOperator fo, string fieldName)
        {
            if ((ReferenceEquals(fo, null)
                        || !fo.Operands.Any(f => f.ToString().Contains(fieldName))))
            {
                return false;
            }

            return true;
        }
    }
}