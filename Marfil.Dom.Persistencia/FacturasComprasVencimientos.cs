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
    
    public partial class FacturasComprasVencimientos
    {
        public string empresa { get; set; }
        public int fkfacturascompras { get; set; }
        public int id { get; set; }
        public Nullable<int> diasvencimiento { get; set; }
        public Nullable<System.DateTime> fechavencimiento { get; set; }
        public Nullable<double> importevencimiento { get; set; }
    
        public virtual FacturasCompras FacturasCompras { get; set; }
    }
}
