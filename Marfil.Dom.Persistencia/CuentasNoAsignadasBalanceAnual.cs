//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Marfil.Dom.Persistencia
{
    using System;
    using System.Collections.Generic;
    
    public partial class CuentasNoAsignadasBalanceAnual
    {
        public string empresa { get; set; }
        public int fkejercicio { get; set; }
        public string fkcuentas { get; set; }
        public int id { get; set; }
        public Nullable<decimal> debe { get; set; }
        public Nullable<decimal> haber { get; set; }
        public Nullable<decimal> saldo { get; set; }
        public Nullable<bool> procesado { get; set; }
    }
}
