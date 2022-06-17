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
    
    public partial class RegistroIVARepercutido
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public RegistroIVARepercutido()
        {
            this.RegistroIVARepercutidoTotales = new HashSet<RegistroIVARepercutidoTotales>();
        }
    
        public string empresa { get; set; }
        public int id { get; set; }
        public string origendoc { get; set; }
        public string tipofactura { get; set; }
        public string referencia { get; set; }
        public Nullable<System.DateTime> fecharegistro { get; set; }
        public Nullable<System.DateTime> fechafactura { get; set; }
        public Nullable<System.DateTime> fechaoperacion { get; set; }
        public string periodo { get; set; }
        public string numfacturacliente { get; set; }
        public string cuentacliente { get; set; }
        public string cuentaventas { get; set; }
        public Nullable<int> criterioivageneral { get; set; }
        public string fkregimeniva { get; set; }
        public Nullable<bool> facturarectificativa { get; set; }
        public string numfacturarectificar { get; set; }
        public Nullable<System.DateTime> fechafacturaoriginal { get; set; }
        public string motivorectificacion { get; set; }
        public Nullable<int> criterioivafactoriginal { get; set; }
        public Nullable<int> criterioivafactura { get; set; }
        public Nullable<bool> bieninversion { get; set; }
        public string fktiporetencion { get; set; }
        public Nullable<double> baseretencion { get; set; }
        public Nullable<double> porcentajeretencion { get; set; }
        public Nullable<double> importeretencion { get; set; }
        public string inmueble { get; set; }
        public Nullable<bool> contabilizar { get; set; }
        public string fkcuentastesoreria { get; set; }
        public Nullable<double> operacionesexluidasbi { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegistroIVARepercutidoTotales> RegistroIVARepercutidoTotales { get; set; }
    }
}
