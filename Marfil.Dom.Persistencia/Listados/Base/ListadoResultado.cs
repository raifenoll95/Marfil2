using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;

namespace Marfil.Dom.Persistencia.Listados.Base
{
    public class ListadoResultado
    {
        private DataTable _listado = new DataTable();
        private Dictionary<string, ConfiguracionColumnasModel> _configuracionColumnas = new Dictionary<string, ConfiguracionColumnasModel>();

        public string TituloListado { get; set; }
        public string IdListado { get; set; }



        public Dictionary<string, ConfiguracionColumnasModel> ConfiguracionColumnas
        {
            get { return _configuracionColumnas; }
            set { _configuracionColumnas = value; }
        }
        public List<string> Filtros { get; set; }
        public DataTable Listado { get; set; }
        public string WebIdListado { get; set; }
    }
}
