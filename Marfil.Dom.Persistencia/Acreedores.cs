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
    
    public partial class Acreedores
    {
        public string empresa { get; set; }
        public string fkcuentas { get; set; }
        public string fkidiomas { get; set; }
        public string fkfamiliaacreedor { get; set; }
        public string fkzonaacreedor { get; set; }
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
        public string tarifa { get; set; }
        public string fkcriteriosagrupacion { get; set; }
        public Nullable<int> previsionpagosperiodicos { get; set; }
    }
}
