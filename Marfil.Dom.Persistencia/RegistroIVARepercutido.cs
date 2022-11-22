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
            this.RegistroIVARepercutidoRectificadas = new HashSet<RegistroIVARepercutidoRectificadas>();
            this.RegistroIVARepercutidoTotales = new HashSet<RegistroIVARepercutidoTotales>();
        }
    
        public string empresa { get; set; }
        public int id { get; set; }
        public Nullable<int> origendoc { get; set; }
        public string tipofactura { get; set; }
        public string fkseriescontables { get; set; }
        public string identificadorsegmento { get; set; }
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
        public Nullable<double> totalfactura { get; set; }
        public Nullable<int> tipoenvio { get; set; }
        public Nullable<System.DateTime> fechaalta { get; set; }
        public Nullable<int> estado { get; set; }
        public string csv { get; set; }
        public string descripcionerror { get; set; }
        public Nullable<bool> variosdestinatarios { get; set; }
        public string cuentaclientecontraparte { get; set; }
        public string nombrecontraparte { get; set; }
        public Nullable<int> tipoidentificacion { get; set; }
        public string nifcontraparte { get; set; }
        public string tipoidentificacionotro { get; set; }
        public string identificacion { get; set; }
        public string codigopais { get; set; }
        public string clavetipofactura { get; set; }
        public Nullable<int> tipofacturarectificativa { get; set; }
        public Nullable<double> baserectificada { get; set; }
        public Nullable<double> cuotaivarectificada { get; set; }
        public Nullable<double> cuotarectificadaeq { get; set; }
        public Nullable<bool> facturaemitidaterceros { get; set; }
        public Nullable<double> importetotalemisor { get; set; }
        public Nullable<double> importetransmisoremisor { get; set; }
        public string claveregimenespecial { get; set; }
        public string descripcionoperacionemisor { get; set; }
        public Nullable<int> tipooperacion { get; set; }
        public Nullable<bool> sujetanoexenta { get; set; }
        public Nullable<int> invsujetopasivo { get; set; }
        public Nullable<double> porcentajeivasujetopasivo { get; set; }
        public Nullable<double> baseimponiblesujetopasivo { get; set; }
        public Nullable<double> cuotaivasujetopasivo { get; set; }
        public Nullable<double> porcentajereceqsujetopasivo { get; set; }
        public Nullable<double> cuotareceqsujetopasivo { get; set; }
        public Nullable<bool> sujetaexenta { get; set; }
        public Nullable<int> causa { get; set; }
        public Nullable<double> baseimponiblesujetaexenta { get; set; }
        public Nullable<bool> nosujeta { get; set; }
        public Nullable<double> importearticulos { get; set; }
        public Nullable<double> importetai { get; set; }
        public Nullable<bool> reenviaraeat { get; set; }
        public Nullable<bool> rectificativa349 { get; set; }
        public Nullable<int> rectificativaano { get; set; }
        public string rectificativaperiodo { get; set; }
        public Nullable<double> baseimponible349 { get; set; }
        public Nullable<double> sumacuotasiva { get; set; }
        public Nullable<bool> enviarmodificacion { get; set; }
        public Nullable<bool> enviarbaja { get; set; }
        public string nifrepresentante { get; set; }
        public Nullable<System.Guid> fkusuarioalta { get; set; }
        public Nullable<System.DateTime> fechaaltaregistro { get; set; }
        public Nullable<System.Guid> fkusuariomodificacion { get; set; }
        public Nullable<System.DateTime> fechamodificacionregistro { get; set; }
        public Nullable<bool> contabilizado { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegistroIVARepercutidoRectificadas> RegistroIVARepercutidoRectificadas { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegistroIVARepercutidoTotales> RegistroIVARepercutidoTotales { get; set; }
    }
}
