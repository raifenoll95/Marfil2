using System.Security.Cryptography.X509Certificates;

namespace Marfil.Dom.ControlsUI.BusquedaTerceros
{
    public enum TipoItemCuentaTercero
    {
        General,
        Cabecera,
        Separador
    }
    public interface IItemResultadoMovile
    {
        TipoItemCuentaTercero Tipo { get; }
        string Campo { get; set; }
        string Valor { get; set; }
    }

    public class ItemResultadoMovile : IItemResultadoMovile
    {
        public string Campo { get; set; }
        public string Valor { get; set; }
        public TipoItemCuentaTercero Tipo { get; set; }

        public ItemResultadoMovile()
        {
            Tipo=TipoItemCuentaTercero.General;
        }
    }

    public class ItemCabeceraResultadoMoviles : IItemResultadoMovile
    {
        public string Valor { get; set; }
        public TipoItemCuentaTercero Tipo { get; private set; }
        public string Campo { get; set; }

        public ItemCabeceraResultadoMoviles()
        {
            Tipo = TipoItemCuentaTercero.Cabecera;
            Campo = string.Empty;
        }
    }

    public class ItemSeparadorResultadoMoviles:IItemResultadoMovile
    {
        public TipoItemCuentaTercero Tipo { get; private set; }
        public string Campo { get; set; }
        public string Valor { get; set; }

        public ItemSeparadorResultadoMoviles()
        {
            Tipo = TipoItemCuentaTercero.Separador;
            Campo = string.Empty;
            Valor = string.Empty;
        }
    }
}
