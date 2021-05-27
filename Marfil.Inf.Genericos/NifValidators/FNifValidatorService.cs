using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Inf.Genericos.NifValidators.Validators;

namespace Marfil.Inf.Genericos.NifValidators
{
    public interface ICheckPaisEuropeo
    {
        bool ExistePaisEuropeo(string codigo);
        bool ExistePaisEuropeoPorLetras(string letras);
        string GetCodigoEuropeo();
    }
    public interface INifValidator
    {
        bool Validate(ref string nif);
    }

    public class FNifValidatorService
    {
        private readonly Dictionary<string,INifValidator> _dictionary=new Dictionary<string, INifValidator>();
        private readonly ICheckPaisEuropeo _checkPaisEuropeoService;

        public FNifValidatorService(ICheckPaisEuropeo checkPaisEuropeoService)
        {
            _checkPaisEuropeoService = checkPaisEuropeoService;
            _dictionary.Add("070",new NifValidator_ES(checkPaisEuropeoService));
            _dictionary.Add("Europeo", new NifValidator_Europeo(checkPaisEuropeoService));
        }

        public INifValidator GetValidator(string pais)
        {
            if (_checkPaisEuropeoService.ExistePaisEuropeo(pais))
                return _dictionary.ContainsKey(pais) ? _dictionary[pais] : _dictionary["Europeo"];

            return new NifValidator_Common(_checkPaisEuropeoService);
        }
    }
}
