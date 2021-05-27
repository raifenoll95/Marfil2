using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Model.FicherosGenerales;

namespace Marfil.Dom.Persistencia.ServicesView.Servicios.AgrupacionFacturacion
{
    internal class FAgrupacionService
    {
        enum TipoAgrupacion
        {
            Normal,
            Ordenado
        }

        private readonly Dictionary<TipoAgrupacion, IAgrupacionService> _dictionary=new Dictionary<TipoAgrupacion, IAgrupacionService>();

        private FAgrupacionService()
        {
            _dictionary.Add(TipoAgrupacion.Normal, new AgrupacionNormalService());
            _dictionary.Add(TipoAgrupacion.Ordenado, new AgrupacionOrdenadaService());
        }

        private static FAgrupacionService _instance;
        public static FAgrupacionService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new FAgrupacionService();

                return _instance;
            }
        }

        public IAgrupacionService GetServicioAgrupacion(CriteriosagrupacionModel model)
        {

            if (model.Ordenaralbaranesvista)
                return _dictionary[TipoAgrupacion.Ordenado];

            return _dictionary[TipoAgrupacion.Normal];
        }
    }
}
