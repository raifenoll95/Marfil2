using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Marfil.Inf.Genericos.NifValidators.Validators
{
    class NifValidator_Europeo : INifValidator
    {
        #region Members

        private readonly ICheckPaisEuropeo _checkPaisEuropeoService;
        #endregion

        public bool Validate(ref string nif)
        {
            if (Regex.IsMatch(nif.Substring(0, 2), "^[a-zA-Z]{2}$"))
            {
                var codigoPais = nif.Substring(0, 2);
                if (!_checkPaisEuropeoService.ExistePaisEuropeoPorLetras(codigoPais))
                    throw new Exception(string.Format("El código de país {0} no es correcto", codigoPais));
            }
            else
                nif = _checkPaisEuropeoService.GetCodigoEuropeo() + nif;

            return true;
        }

        public NifValidator_Europeo(ICheckPaisEuropeo checkPaisEuropeoService)
        {
            _checkPaisEuropeoService = checkPaisEuropeoService;
        }
    }
}
