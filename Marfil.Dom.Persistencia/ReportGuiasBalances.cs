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
    
    public partial class ReportGuiasBalances
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ReportGuiasBalances()
        {
            this.ReportGuiasBalancesLineas = new HashSet<ReportGuiasBalancesLineas>();
        }
    
        public int Id { get; set; }
        public Nullable<int> InformeId { get; set; }
        public Nullable<int> GuiaId { get; set; }
        public string textogrupo { get; set; }
        public string orden { get; set; }
        public string actpas { get; set; }
        public string detfor { get; set; }
        public string formula { get; set; }
        public string regdig { get; set; }
        public string descrip { get; set; }
        public string listado { get; set; }
        public Nullable<decimal> saldo { get; set; }
        public Nullable<decimal> saldoea { get; set; }
        public string listacuentas { get; set; }
    
        public virtual TipoGuia TipoGuia { get; set; }
        public virtual TipoInforme TipoInforme { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReportGuiasBalancesLineas> ReportGuiasBalancesLineas { get; set; }
    }
}
