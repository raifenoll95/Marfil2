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
    
    public partial class Usuarios
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Usuarios()
        {
            this.AppPermisos = new HashSet<AppPermisos>();
            this.Roles = new HashSet<Roles>();
        }
    
        public System.Guid id { get; set; }
        public string usuario { get; set; }
        public string password { get; set; }
        public Nullable<bool> puedecambiarempresa { get; set; }
        public string usuariomail { get; set; }
        public string passwordmail { get; set; }
        public string smtp { get; set; }
        public Nullable<int> puerto { get; set; }
        public Nullable<bool> ssl { get; set; }
        public string email { get; set; }
        public string firma { get; set; }
        public string nombre { get; set; }
        public Nullable<int> copiaremitente { get; set; }
        public Nullable<int> nivel { get; set; }
        public Nullable<bool> cambiarempresa { get; set; }
        public Nullable<bool> cambiaralmacen { get; set; }
        public Nullable<bool> usuario_cliente { get; set; }
        public string codigoclienteusuario { get; set; }
        public Nullable<bool> operario { get; set; }
        public string codigooperariousuario { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AppPermisos> AppPermisos { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Roles> Roles { get; set; }
    }
}
