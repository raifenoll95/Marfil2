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
    
    public partial class Clientes
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Clientes()
        {
            this.PreciosEspeciales = new HashSet<PreciosEspeciales>();
        }
    
        public string empresa { get; set; }
        public string fkcuentas { get; set; }
        public string fkidiomas { get; set; }
        public string fkfamiliacliente { get; set; }
        public string fkzonacliente { get; set; }
        public string fktipoempresa { get; set; }
        public string fkunidadnegocio { get; set; }
        public string fkincoterm { get; set; }
        public string fkpuertosfkpaises { get; set; }
        public string fkpuertosid { get; set; }
        public int fkmonedas { get; set; }
        public string fkregimeniva { get; set; }
        public string fkgruposiva { get; set; }
        public int criterioiva { get; set; }
        public string fktiposretencion { get; set; }
        public string fktransportistahabitual { get; set; }
        public Nullable<int> tipoportes { get; set; }
        public string cuentatesoreria { get; set; }
        public int fkformaspago { get; set; }
        public Nullable<double> descuentoprontopago { get; set; }
        public Nullable<double> descuentocomercial { get; set; }
        public Nullable<int> diafijopago1 { get; set; }
        public Nullable<int> diafijopago2 { get; set; }
        public string periodonopagodesde { get; set; }
        public string periodonopagohasta { get; set; }
        public string notas { get; set; }
        public Nullable<int> numerocopiasfactura { get; set; }
        public string fkcuentasagente { get; set; }
        public string fkcuentascomercial { get; set; }
        public string perteneceagrupo { get; set; }
        public Nullable<int> tarifa { get; set; }
        public string fkcuentasaseguradoras { get; set; }
        public string suplemento { get; set; }
        public Nullable<double> porcentajeriesgocomercial { get; set; }
        public Nullable<double> porcentajeriesgopolitico { get; set; }
        public Nullable<int> riesgoconcedidoempresa { get; set; }
        public Nullable<int> riesgosolicitado { get; set; }
        public Nullable<int> riesgoaseguradora { get; set; }
        public Nullable<System.DateTime> fechaclasificacion { get; set; }
        public Nullable<System.DateTime> fechaultimasolicitud { get; set; }
        public Nullable<int> diascondecidos { get; set; }
        public string fktarifas { get; set; }
        public string fkcriteriosagrupacion { get; set; }
        public string fktipofactura { get; set; }
        public string fkdelegacion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PreciosEspeciales> PreciosEspeciales { get; set; }
    }
}
