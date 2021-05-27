using System.Collections.Generic;

namespace Marfil.Dom.ControlsUI.Busquedas
{


    public interface IColumnDefinition
    {
        string displayName { get; set; }
        string field { get; set; }
        bool visible { get; set; }
        Filter filter { get; set; }
    }

    public interface IResultBusquedas<T>
    {
        IEnumerable<IColumnDefinition> columns { get; set; }
        IEnumerable<T> values { get; set; }
    }

    public class Filter
    {
        public int condition { get; set; }
        public List<ComboListItem> selectOptions { get; set; }
        public string type { get; set; }
    }

    public class ComboListItem
    {
        public string value { get; set; }
        public string label { get; set; }
    }

    public class ColumnDefinition : IColumnDefinition
    {
        public const int STARTS_WITH = 2;
        public const int ENDS_WITH = 4;
        public const int EXACT = 8;
        public const int CONTAINS = 16;
        public const int GREATER_THAN = 32;
        public const int GREATER_THAN_OR_EQUAL = 64;
        public const int LESS_THAN = 128;
        public const int LESS_THAN_OR_EQUAL = 256;
        public const int NOT_EQUAL = 512;

        public bool enableCellEdit { get; set; }
        public string displayName { get; set; }
        public string field { get; set; }
        public bool visible { get; set; }
        public Filter filter { get; set; }
        public int? width { get; set; }
        public string type { get; set; }
    }

    public interface ICuentasFiltros
    {
        string Id { get; set; }
        string Filtros { get; set; }
        string Pagina { get; set; }
        string RegistrosPagina { get; set; }
        string Tipocuenta { get; set; }
    }

    public interface IArticulosFiltros
    {
        
        string Filtros { get; set; }
        string Pagina { get; set; }
        string RegistrosPagina { get; set; }
    
    }

    public interface IDocumentosFiltros
    {
        string Id { get; set; }
        string Filtros { get; set; }
        string Pagina { get; set; }
        string RegistrosPagina { get; set; }
        string Tipodocumento { get; set; }
    }


    public interface IFiltrosMobile
    {
        string Id { get; set; }
        string Filtros { get; set; }
        string Pagina { get; set; }
        string RegistrosPagina { get; set; }
        string ToString();
    }


    //public interface ICuentasFiltros : IFiltrosMobile
    //{

    //    string Tipocuenta { get; set; }
    //}

    //public interface IDocumentosFiltros : IFiltrosMobile
    //{
    //    string Tipodocumento { get; set; }
    //}


    public class ResultBusquedas<T> : IResultBusquedas<T>
    {
        public IEnumerable<IColumnDefinition> columns { get; set; }
        public IEnumerable<T> values { get; set; }
    }

    //public class ResultBusquedasPaginados<T> : ResultBusquedas<T>
    //{
    //    public int RegistrosTotales { get; set; }
    //    public int PaginaActual { get; set; }
    //}

    public class ResultBusquedasPaginados<T>
    {
        public IEnumerable<ColumnDefinition> columns { get; set; }
        public IEnumerable<T> values { get; set; }
        public int RegistrosTotales { get; set; }

        public int PaginaActual { get; set; }
    }
}
