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
    
    public partial class FormasPago
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FormasPago()
        {
            this.FormasPagoLin = new HashSet<FormasPagoLin>();
        }
    
        public int id { get; set; }
        public string nombre { get; set; }
        public string nombre2 { get; set; }
        public string fkModosPago { get; set; }
        public Nullable<bool> imprimirvencimientos { get; set; }
        public Nullable<double> recargofinanciero { get; set; }
        public Nullable<bool> efectivo { get; set; }
        public Nullable<bool> remesable { get; set; }
        public Nullable<bool> mandato { get; set; }
        public Nullable<bool> excluirfestivos { get; set; }
        public Nullable<bool> bloqueada { get; set; }
        public string fkMotivosbloqueo { get; set; }
        public Nullable<System.DateTime> fechamodificacionbloqueo { get; set; }
        public Nullable<System.Guid> fkUsuariobloqueo { get; set; }
        public string fkgruposformaspago { get; set; }
        public Nullable<int> docsventaimprimircuenta { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FormasPagoLin> FormasPagoLin { get; set; }
    }
}
