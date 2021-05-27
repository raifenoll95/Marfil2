using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using Marfil.Dom.ControlsUI.Toolbar;
using Marfil.Dom.Persistencia.Model.Interfaces;
using Marfil.Dom.Persistencia.Model.Terceros;

namespace Marfil.Dom.Persistencia.Model
{
    public enum FiltroColumnas
    {
        ContenidoEn,
        EmpiezaPor,
        TerminaEn
    }

    public class EstiloFilas
    {
        private List<Tuple<object, string>> _estilos=new List<Tuple<object, string>>();

        public IList<Tuple<object, string>> Estilos
        {
            get { return _estilos; }
            set { _estilos = value.ToList(); }
        }
    }

    public class ListIndexModel : IToolbar, ICanDisplayName
    {
        #region Properties

        private IList<string> _excludedColumns = new List<string>();
        private IList<string> _primaryColumnns = new List<string>();
        private IDictionary<string, int> _anchoColumnas = new Dictionary<string, int>();
        private IDictionary<string, FiltroColumnas> _filtroColumnas = new Dictionary<string, FiltroColumnas>();
        public IDictionary<string, IEnumerable<Tuple<string, string>>> ColumnasCombo = new Dictionary<string, IEnumerable<Tuple<string, string>>>();
        private IDictionary<string, EstiloFilas> _estiloFilas = new Dictionary<string, EstiloFilas>();
        private Dictionary<string, int> _ordenColumnas = new Dictionary<string, int>();
        public string Entidad { get; set; }

        public string VarSessionName { get; set; }

        public bool PermiteModificar { get; set; }
        public bool PermiteEliminar { get; set; }
        public IEnumerable<ViewProperty> Properties { get; set; }
        public IEnumerable<IModelView> List { get; set; }
        

        public Dictionary<string, int> OrdenColumnas
        {
            get { return _ordenColumnas; }
            set { _ordenColumnas = value; }
        }

        public IList<string> ExcludedColumns
        {
            get { return _excludedColumns; }
            set { _excludedColumns = value; }
        }

        public IList<string> PrimaryColumnns
        {
            get { return _primaryColumnns; }
            set { _primaryColumnns = value; }
        }

        public IDictionary<string, int> AnchoColumnas
        {
            get { return _anchoColumnas; }
            set { _anchoColumnas = value; }
        }

        public IDictionary<string, FiltroColumnas> FiltroColumnas
        {
            get { return _filtroColumnas; }
            set { _filtroColumnas = value; }
        }

        public IDictionary<string, EstiloFilas> EstiloFilas
        {
            get { return _estiloFilas; }
            set { _estiloFilas = value; }
        }

        public string Controller { get; set; }


        public string BloqueoColumn { get; set; }
        public string ColumnaColor { get; set; }

        #endregion

        public ToolbarModel Toolbar { get; set; }

        public string DisplayName
        {
            get
            {
                return Entidad;
            }
        }
    }
}