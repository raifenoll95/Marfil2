using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.Configuracion.TablasVarias.Derivados;
using Marfil.Dom.Persistencia.ServicesView.Servicios;
using Marfil.Inf.Genericos.NifValidators;

namespace Marfil.Dom.Persistencia.Helpers
{
    internal class CheckPaisEuropeo:ICheckPaisEuropeo
    {
        private readonly IEnumerable<TablasVariasPaisesModel> _paises;
        private readonly string _codigopais;
        private readonly IContextService _context;

        public CheckPaisEuropeo(IContextService context, string codigopais)
        {
            _context = context;
            _codigopais = codigopais;
            var appService= new ApplicationHelper(context);
            _paises = appService.GetListPaises();
        }

        public bool ExistePaisEuropeo(string codigo)
        {
            if (_paises.Any())
                return _paises.Any(f => f.Valor == codigo && !string.IsNullOrEmpty(f.NifEuropeo));

            return true;
        }

        public bool ExistePaisEuropeoPorLetras(string letras)
        {
            if (_paises.Any())
                return _paises.Any(f => f.Valor == _codigopais && f.NifEuropeo==letras);

            return true;
        }

        public string GetCodigoEuropeo()
        {
            if (_paises.Any())
                return _paises.FirstOrDefault(f => f.Valor == _codigopais)?.NifEuropeo??string.Empty;

            return string.Empty;
        }
    }
}
