using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Helpers;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.ServicesView.Servicios;

namespace Marfil.Dom.Persistencia.Listados.Base
{
    public class ConfiguracionDecimalesColumnasModel
    {
        public string Columna { get; set; }
        public int? Decimales { get; set; }
    }

    public class ConfiguracionColumnasModel
    {
        public ConfiguracionDecimalesColumnasModel Decimales { get; set; }
    }

    public abstract class ListadosModel: IListados,IToolbar
    {
        
        public string Empresa { get; set; }
        private List<string> _condiciones=new List<string>();
        private Dictionary<string, object> _valoresParametros = new Dictionary<string, object>();
        private Dictionary<string, ConfiguracionColumnasModel> _configuracionColumnas = new Dictionary<string, ConfiguracionColumnasModel>();

        [XmlIgnore]
        public IContextService Context { get; set; }

        [XmlIgnore]
        public ApplicationHelper AppService { get; set; }

        [XmlIgnore]
        public ToolbarModel Toolbar { get; set; }
        
        public abstract string TituloListado { get; }
        
        public abstract string IdListado { get; }
        
        public virtual string WebIdListado
        {
            get
            {
                return IdListado;
            }
        }
        [XmlIgnore]
        public Dictionary<string, ConfiguracionColumnasModel> ConfiguracionColumnas
        {
            get { return _configuracionColumnas; }
            set { _configuracionColumnas = value; }
        }
        [XmlIgnore]
        public List<string> Condiciones
        {
            get { return _condiciones; }
            set { _condiciones = value; }
        }
        [XmlIgnore]
        internal Dictionary<string, object> ValoresParametros
        {
            get { return _valoresParametros; }
            set { _valoresParametros = value; }
        }

    

        public IEnumerable<ViewProperty> GetProperties()
        {
            var listNames = GetType().GetProperties().Select(f => f.Name).Except(typeof(IListados).GetProperties().Select(h => h.Name));
            var properties = GetType().GetProperties().Where(f => listNames.Any(h => h == f.Name));

            return properties.Select(item => new ViewProperty
            {
                property = item,
                attributes = item.GetCustomAttributes(true)
            }).ToList();
        }
        [XmlIgnore]
        internal string Filtros
        {
            get
            {
                return GenerarFiltros();
            }
        }

        [XmlIgnore]
        public string Select
        {
            get
            {
                var filtros = GenerarFiltros();
                if (!string.IsNullOrEmpty(filtros))
                    filtros = " WHERE " + filtros;
                var a = GenerarSelect() + filtros;
                return a;

            }
        }

        private string GenerarFiltros()
        {
            if (string.IsNullOrEmpty(GenerarFiltrosColumnas()) && string.IsNullOrEmpty(GenerarOrdenColumnas()))
                return string.Empty;

            return GenerarFiltrosColumnas() + " " + GenerarOrdenColumnas();
        }

        internal virtual string GenerarFiltrosColumnas()
        {
            return string.Empty;
        }

        internal virtual string GenerarOrdenColumnas()
        {
            return string.Empty;
        }

        internal abstract string GenerarSelect();

        public ListadosModel()
        {
            
        }

        public ListadosModel(IContextService context)
        {
            Context = context;
            AppService=new ApplicationHelper(context);
        }
    }
}
