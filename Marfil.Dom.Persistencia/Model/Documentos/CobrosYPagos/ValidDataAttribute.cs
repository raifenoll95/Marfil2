using System;
using System.ComponentModel.DataAnnotations;

namespace Marfil.Dom.Persistencia.Model.Documentos.CobrosYPagos
{

    public class ValidDataAttribute : ValidationAttribute
    {

        #region CTR
        public ValidDataAttribute()
        {
        }
        #endregion

        public override bool IsValid(object value)
        {
            var stringDate = value.ToString();
            DateTime temp;
            return (DateTime.TryParse(stringDate, out temp));
        }
    }

}
