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
    
    public partial class ReportGuiasBalancesLineasBalanceAnual
    {
        public int Id { get; set; }
        public Nullable<int> InformeId { get; set; }
        public Nullable<int> GuiaId { get; set; }
        public Nullable<int> GuiasBalancesId { get; set; }
        public string orden { get; set; }
        public string cuenta { get; set; }
        public string signo { get; set; }
        public string signoea { get; set; }
        public Nullable<decimal> saldo { get; set; }
        public Nullable<decimal> saldoea { get; set; }
    
        public virtual ReportGuiasBalancesBalanceAnual ReportGuiasBalancesBalanceAnual { get; set; }
    }
}
